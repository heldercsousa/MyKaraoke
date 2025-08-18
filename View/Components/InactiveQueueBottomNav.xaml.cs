using MyKaraoke.View.Behaviors;
using MyKaraoke.View.Components;
using System.Collections.ObjectModel;
using MyKaraoke.View.Animations;

namespace MyKaraoke.View.Components
{
    /// <summary>
    /// ✅ CORRIGIDO: Eliminar eventos duplicados que causavam múltiplas navegações
    /// 🔧 SIMPLIFICADO: Usa apenas NavBarBehavior + SafeNavigationBehavior
    /// 🛡️ PROTEÇÃO: Anti-eventos duplicados integrada
    /// </summary>
    public partial class InactiveQueueBottomNav : ContentView, IAnimatableNavBar
    {
        #region Events - MANTIDOS para compatibilidade com StackPage

        public event EventHandler LocaisClicked, BandokeClicked, NovaFilaClicked, HistoricoClicked, AdministrarClicked;

        #endregion

        #region Private Fields

        private bool _isInitialized = false;
        // 🛡️ PROTEÇÃO: Anti-eventos duplicados
        private readonly object _eventLock = new object();
        private DateTime _lastEventTime = DateTime.MinValue;

        // 🎯 NAVEGAÇÃO SEGURA: SafeNavigationBehaviors para cada botão
        private SafeNavigationBehavior _locaisNavigationBehavior;

        #endregion

        public InactiveQueueBottomNav()
        {
            InitializeComponent();
            InitializeNavigationBehaviors();
            System.Diagnostics.Debug.WriteLine($"✅ InactiveQueueBottomNav: Construtor chamado");
        }

        #region Initialization

        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();

            if (Handler != null && !_isInitialized)
            {
                InitializeNavBar();
                _isInitialized = true;
                System.Diagnostics.Debug.WriteLine("✅ InactiveQueueBottomNav inicializado com NavBarBehavior");
            }
        }

        /// <summary>
        /// 🎯 NAVEGAÇÃO: Inicializa SafeNavigationBehaviors para navegação segura
        /// </summary>
        private void InitializeNavigationBehaviors()
        {
            try
            {
                // 🎯 NAVEGAÇÃO SEGURA: Para SpotPage
                _locaisNavigationBehavior = new SafeNavigationBehavior
                {
                    TargetPageType = typeof(SpotPage),
                    DebounceMilliseconds = 1000,
                    CreatePageFunc = () =>
                    {
                        var spotPage = new SpotPage();
                        System.Diagnostics.Debug.WriteLine($"✅ InactiveQueueBottomNav: SpotPage criada via SafeNavigationBehavior - Hash: {spotPage.GetHashCode()}");
                        return spotPage;
                    }
                };

                System.Diagnostics.Debug.WriteLine("✅ InactiveQueueBottomNav: SafeNavigationBehaviors inicializados");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ InactiveQueueBottomNav: Erro ao inicializar navigation behaviors: {ex.Message}");
            }
        }

        /// <summary>
        /// ✅ CONFIGURAÇÃO: Define todos os botões através do NavBarBehavior
        /// </summary>
        private void InitializeNavBar()
        {
            var buttons = new ObservableCollection<NavButtonConfig>
            {
                // Botões Regulares
                NavButtonConfig.Regular("Locais", "spot.png", new Command(() => OnLocaisClicked())),
                NavButtonConfig.Regular("Bandokê", "musicos.png", new Command(() => OnBandokeClicked())),

                // ✅ BOTÃO ESPECIAL: Nova Fila com animação pulse
                new NavButtonConfig
                {
                    Text = "Nova Fila",
                    IsSpecial = true,
                    CenterContent = "+",
                    Command = new Command(() => OnNovaFilaClicked()),
                    GradientStyle = SpecialButtonGradientType.Yellow,
                    SpecialAnimationTypes = SpecialButtonAnimationType.ShowHide | SpecialButtonAnimationType.Pulse,
                    IsAnimated = true
                },

                // Botões Regulares
                NavButtonConfig.Regular("Histórico", "historico.png", new Command(() => OnHistoricoClicked())),
                NavButtonConfig.Regular("Administrar", "manage.png", new Command(() => OnAdministrarClicked()))
            };

            // ✅ BEHAVIOR: Configura botões - SEM subscrever eventos duplicados
            navBarBehavior.Buttons = buttons;

            // 🔧 CORREÇÃO: Não subscrevemos ButtonClicked para evitar eventos duplicados
            // Os eventos são disparados diretamente pelos Commands configurados acima

            System.Diagnostics.Debug.WriteLine($"✅ InactiveQueueBottomNav: NavBarBehavior configurado com {buttons.Count} botões SEM eventos duplicados");
        }

        #endregion

        #region Event Handlers - PROTEÇÃO ANTI-DUPLA EXECUÇÃO

        /// <summary>
        /// 🛡️ PROTEÇÃO: Eventos com debounce para evitar cliques múltiplos
        /// </summary>
        private bool ShouldProcessEvent(string eventName)
        {
            lock (_eventLock)
            {
                var now = DateTime.Now;
                var timeSinceLastEvent = now - _lastEventTime;

                if (timeSinceLastEvent < TimeSpan.FromMilliseconds(500))
                {
                    System.Diagnostics.Debug.WriteLine($"🚫 InactiveQueueBottomNav: {eventName} BLOQUEADO - muito recente (gap: {timeSinceLastEvent.TotalMilliseconds}ms)");
                    return false;
                }

                _lastEventTime = now;
                System.Diagnostics.Debug.WriteLine($"✅ InactiveQueueBottomNav: {eventName} AUTORIZADO");
                return true;
            }
        }

        /// <summary>
        /// 🎯 NAVEGAÇÃO SEGURA: Usa SafeNavigationBehavior em vez de evento tradicional
        /// </summary>
        private async void OnLocaisClicked()
        {
            if (!ShouldProcessEvent("LocaisClicked")) return;

            try
            {
                System.Diagnostics.Debug.WriteLine($"✅ InactiveQueueBottomNav: OnLocaisClicked via SafeNavigationBehavior");

                // 🚀 NAVEGAÇÃO SEGURA: Usa SafeNavigationBehavior
                if (_locaisNavigationBehavior != null)
                {
                    await _locaisNavigationBehavior.NavigateToPageAsync();
                    System.Diagnostics.Debug.WriteLine($"✅ InactiveQueueBottomNav: Navegação para SpotPage via SafeNavigationBehavior concluída");
                }
                else
                {
                    // 🛡️ FALLBACK: Dispara evento tradicional se behavior não disponível
                    System.Diagnostics.Debug.WriteLine($"⚠️ InactiveQueueBottomNav: SafeNavigationBehavior não disponível - usando evento tradicional");
                    LocaisClicked?.Invoke(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ InactiveQueueBottomNav: Erro em OnLocaisClicked: {ex.Message}");

                // 🛡️ FALLBACK: Tenta evento tradicional em caso de erro
                try
                {
                    LocaisClicked?.Invoke(this, EventArgs.Empty);
                }
                catch (Exception fallbackEx)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ InactiveQueueBottomNav: Erro no fallback: {fallbackEx.Message}");
                }
            }
        }

        private void OnBandokeClicked()
        {
            if (!ShouldProcessEvent("BandokeClicked")) return;

            try
            {
                System.Diagnostics.Debug.WriteLine($"✅ InactiveQueueBottomNav: Disparando BandokeClicked");
                BandokeClicked?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ InactiveQueueBottomNav: Erro em OnBandokeClicked: {ex.Message}");
            }
        }

        private void OnNovaFilaClicked()
        {
            if (!ShouldProcessEvent("NovaFilaClicked")) return;

            try
            {
                System.Diagnostics.Debug.WriteLine($"✅ InactiveQueueBottomNav: Disparando NovaFilaClicked");
                NovaFilaClicked?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ InactiveQueueBottomNav: Erro em OnNovaFilaClicked: {ex.Message}");
            }
        }

        private void OnHistoricoClicked()
        {
            if (!ShouldProcessEvent("HistoricoClicked")) return;

            try
            {
                System.Diagnostics.Debug.WriteLine($"✅ InactiveQueueBottomNav: Disparando HistoricoClicked");
                HistoricoClicked?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ InactiveQueueBottomNav: Erro em OnHistoricoClicked: {ex.Message}");
            }
        }

        private void OnAdministrarClicked()
        {
            if (!ShouldProcessEvent("AdministrarClicked")) return;

            try
            {
                System.Diagnostics.Debug.WriteLine($"✅ InactiveQueueBottomNav: Disparando AdministrarClicked");
                AdministrarClicked?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ InactiveQueueBottomNav: Erro em OnAdministrarClicked: {ex.Message}");
            }
        }

        #endregion

        #region IAnimatableNavBar - DELEGADO PARA BEHAVIOR

        /// <summary>
        /// ✅ DELEGADO: ShowAsync via NavBarBehavior
        /// </summary>
        public async Task ShowAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"✅ InactiveQueueBottomNav: ShowAsync chamado");

                this.IsVisible = true;

                if (navBarBehavior != null)
                {
                    await navBarBehavior.ShowAsync();
                    System.Diagnostics.Debug.WriteLine($"✅ InactiveQueueBottomNav: navBarBehavior.ShowAsync() concluído");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"❌ InactiveQueueBottomNav: navBarBehavior é NULL - usando fallback");
                    await NavBarExtensions.ShowAsync(navGrid);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ InactiveQueueBottomNav: Erro em ShowAsync: {ex.Message}");
            }
        }

        /// <summary>
        /// ✅ DELEGADO: HideAsync via NavBarBehavior
        /// </summary>
        public async Task HideAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"✅ InactiveQueueBottomNav: HideAsync chamado");

                if (navBarBehavior != null)
                {
                    await navBarBehavior.HideAsync();
                    System.Diagnostics.Debug.WriteLine($"✅ InactiveQueueBottomNav: navBarBehavior.HideAsync() concluído");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"❌ InactiveQueueBottomNav: navBarBehavior é NULL - usando fallback");
                    await NavBarExtensions.HideAsync(navGrid);
                }

                this.IsVisible = false;
                System.Diagnostics.Debug.WriteLine($"✅ InactiveQueueBottomNav: HideAsync concluído - IsVisible={this.IsVisible}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ InactiveQueueBottomNav: Erro em HideAsync: {ex.Message}");
            }
        }

        #endregion

        #region Public Methods for Diagnostics

        /// <summary>
        /// 📊 DIAGNÓSTICO: Retorna estatísticas do componente
        /// </summary>
        public Dictionary<string, object> GetComponentDiagnostics()
        {
            return new Dictionary<string, object>
            {
                { "IsInitialized", _isInitialized },
                { "IsVisible", this.IsVisible },
                { "HasNavBarBehavior", navBarBehavior != null },
                { "ButtonCount", navBarBehavior?.Buttons?.Count ?? 0 },
                { "HasLocaisNavigationBehavior", _locaisNavigationBehavior != null },
                { "LastEventTime", _lastEventTime.ToString("HH:mm:ss.fff") }
            };
        }

        /// <summary>
        /// 🔧 UTILITÁRIO: Força todas as correções conhecidas do componente
        /// </summary>
        public async Task ApplyComponentFixes()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🔧 InactiveQueueBottomNav: Aplicando correções do componente");

                // 🔧 CORREÇÃO 1: Força inicialização se não foi feita
                if (!_isInitialized && Handler != null)
                {
                    InitializeNavBar();
                    _isInitialized = true;
                }

                // 🔧 CORREÇÃO 2: Força visibilidade
                this.IsVisible = true;

                // 🔧 CORREÇÃO 3: Tenta ShowAsync se navBarBehavior disponível
                if (navBarBehavior != null)
                {
                    try
                    {
                        await navBarBehavior.ShowAsync();
                        System.Diagnostics.Debug.WriteLine($"✅ InactiveQueueBottomNav: ShowAsync forçado com sucesso");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"⚠️ InactiveQueueBottomNav: Erro ao forçar ShowAsync: {ex.Message}");
                    }
                }

                System.Diagnostics.Debug.WriteLine($"✅ InactiveQueueBottomNav: Correções do componente aplicadas");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ InactiveQueueBottomNav: Erro ao aplicar correções: {ex.Message}");
            }
        }

        #endregion
    }
}