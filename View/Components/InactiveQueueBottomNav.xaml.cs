using MyVocaList.View.Behaviors;
using MyVocaList.View.Components;
using System.Collections.ObjectModel;
using MyVocaList.View.Animations;

namespace MyVocaList.View.Components
{
    /// <summary>
    /// ✅ CORRIGIDO: Eliminar eventos duplicados que causavam múltiplas navegações
    /// 🔧 SIMPLIFICADO: Usa apenas NavBarBehavior + SafeNavigationBehavior
    /// 🛡️ PROTEÇÃO: Anti-eventos duplicados integrada
    /// 🎯 CORREÇÃO CRÍTICA: Adicionada propriedade IsReady para trigger automático igual CrudNavBarComponent
    /// </summary>
    public partial class InactiveQueueBottomNav : ContentView, IAnimatableNavBar
    {
        #region Bindable Properties - CORREÇÃO CRÍTICA

        /// <summary>
        /// 🎯 CORREÇÃO: Propriedade que dispara inicialização automática (igual CrudNavBarComponent.SelectionCount)
        /// </summary>
        public static readonly BindableProperty IsReadyProperty =
            BindableProperty.Create(nameof(IsReady), typeof(bool), typeof(InactiveQueueBottomNav), false,
            propertyChanged: OnIsReadyChanged);

        public bool IsReady
        {
            get => (bool)GetValue(IsReadyProperty);
            set => SetValue(IsReadyProperty, value);
        }

        #endregion

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

            // 🎯 CORREÇÃO CRÍTICA: Define IsReady=true para disparar inicialização
            IsReady = true;

            System.Diagnostics.Debug.WriteLine($"✅ InactiveQueueBottomNav: Construtor chamado - IsReady definido como True");
        }

        // ✅ SELF-REGISTRATION: Handled automatically by NavBarBehavior!
        // No OnParentSet() needed - NavBarBehavior registers navbar with page.

        #region Initialization - CORREÇÃO CRÍTICA

        /// <summary>
        /// 🎯 CORREÇÃO: PropertyChanged que dispara inicialização automática (igual CrudNavBarComponent)
        /// </summary>
        private static void OnIsReadyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is InactiveQueueBottomNav navBar && (bool)newValue)
            {
                System.Diagnostics.Debug.WriteLine($"🎯 InactiveQueueBottomNav: IsReady mudou para True - disparando inicialização");
                navBar.EnsureInitialization();
            }
        }

        /// <summary>
        /// 🎯 CORREÇÃO: Garante inicialização mesmo se OnHandlerChanged falhar/atrasar
        /// </summary>
        private void EnsureInitialization()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🎯 InactiveQueueBottomNav: EnsureInitialization - Initialized={_isInitialized}, Handler={Handler != null}");

                if (!_isInitialized)
                {
                    if (Handler != null && navBarBehavior != null)
                    {
                        // ✅ INICIALIZAÇÃO DIRETA
                        InitializeNavBar();
                        _isInitialized = true;
                        System.Diagnostics.Debug.WriteLine($"✅ InactiveQueueBottomNav: Inicialização FORÇADA via EnsureInitialization");
                    }
                    else
                    {
                        // 🎯 AGENDA RETRY: Se Handler não está pronto, agenda nova tentativa
                        System.Diagnostics.Debug.WriteLine($"⏰ InactiveQueueBottomNav: Handler não pronto - agendando retry");
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await Task.Delay(100);
                            if (!_isInitialized && Handler != null)
                            {
                                EnsureInitialization();
                            }
                        });
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"✅ InactiveQueueBottomNav: Já inicializado - verificando botões");

                    // 🎯 VERIFICA: Se já inicializado mas sem botões, força reconfiguração
                    var buttonCount = navBarBehavior?.Buttons?.Count ?? 0;
                    if (buttonCount == 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"🎯 InactiveQueueBottomNav: Sem botões - reconfigurando");
                        InitializeNavBar();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ InactiveQueueBottomNav: Erro em EnsureInitialization: {ex.Message}");
            }
        }

        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();

            System.Diagnostics.Debug.WriteLine($"🔧 InactiveQueueBottomNav: OnHandlerChanged - Handler={Handler != null}, Initialized={_isInitialized}");

            if (Handler != null && !_isInitialized)
            {
                // 🎯 CORREÇÃO: Chama EnsureInitialization em vez de InitializeNavBar diretamente
                EnsureInitialization();
            }
        }

        /// <summary>
        /// 🎯 NAVEGAÇÃO: Inicializa SafeNavigationBehaviors para navegação segura
        /// </summary>
        private void InitializeNavigationBehaviors()
        {
            try
            {
                // ✅ SIMPLES: SafeNavigationBehavior decide se cria nova ou reutiliza
                _locaisNavigationBehavior = new SafeNavigationBehavior
                {
                    TargetPageType = typeof(SpotPage),          // ← Para saber qual página criar
                    EnableSmartStackNavigation = true,          // ← Para ser inteligente
                    DebounceMilliseconds = 1000,
                    CreatePageFunc = () => new SpotPage()       // ← Como criar quando necessário
                };

                System.Diagnostics.Debug.WriteLine("✅ InactiveQueueBottomNav: SafeNavigationBehavior configurado - behavior decide tudo");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ InactiveQueueBottomNav: Erro ao inicializar navigation behaviors: {ex.Message}");
            }
        }

        /// <summary>
        /// ✅ CONFIGURAÇÃO: Define todos os botões através do NavBarBehavior
        /// 🎯 CORREÇÃO: Mais logs para debug
        /// </summary>
        private void InitializeNavBar()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🎯 InactiveQueueBottomNav: InitializeNavBar INICIADO - navBarBehavior={navBarBehavior != null}");

                if (navBarBehavior == null)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ InactiveQueueBottomNav: navBarBehavior é NULL - abortando");
                    return;
                }

                var buttons = new ObservableCollection<NavButtonConfig>
                {
                    // Botões Regulares
                    NavButtonConfig.Regular("Locais", "venue.png", new Command(() => OnLocaisClicked())),
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

                System.Diagnostics.Debug.WriteLine($"🎯 InactiveQueueBottomNav: {buttons.Count} botões criados");

                // ✅ BEHAVIOR: Configura botões - SEM subscrever eventos duplicados
                navBarBehavior.Buttons = buttons;

                // 🔧 CORREÇÃO: NÃO subscrevemos ButtonClicked para evitar eventos duplicados
                // Os eventos são disparados diretamente pelos Commands configurados acima

                System.Diagnostics.Debug.WriteLine($"✅ InactiveQueueBottomNav: NavBarBehavior configurado com {buttons.Count} botões SEM eventos duplicados");

                // 🎯 VERIFICAÇÃO: Confirma se foi setado
                var setButtonsCount = navBarBehavior.Buttons?.Count ?? 0;
                System.Diagnostics.Debug.WriteLine($"🔧 InactiveQueueBottomNav: Verificação - NavBarBehavior.Buttons.Count = {setButtonsCount}");

                if (setButtonsCount != buttons.Count)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ InactiveQueueBottomNav: ERRO - Esperava {buttons.Count} botões, mas NavBarBehavior tem {setButtonsCount}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"✅ InactiveQueueBottomNav: Configuração bem-sucedida - {setButtonsCount} botões setados");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ InactiveQueueBottomNav: Erro em InitializeNavBar: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"❌ StackTrace: {ex.StackTrace}");
            }
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

                // 🚀 APENAS SafeNavigationBehavior - SEM fallback que causa dupla navegação
                if (_locaisNavigationBehavior != null)
                {
                    await _locaisNavigationBehavior.NavigateToPageAsync();
                    System.Diagnostics.Debug.WriteLine($"✅ InactiveQueueBottomNav: Navegação para SpotPage via SafeNavigationBehavior concluída");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"❌ InactiveQueueBottomNav: SafeNavigationBehavior não disponível");
                }

                // ❌ REMOVIDO: Fallback que causava dupla navegação
                // LocaisClicked?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ InactiveQueueBottomNav: Erro em OnLocaisClicked: {ex.Message}");

                // 🛡️ FALLBACK: Apenas em caso de erro crítico
                try
                {
                    LocaisClicked?.Invoke(this, EventArgs.Empty);
                    System.Diagnostics.Debug.WriteLine($"🛡️ InactiveQueueBottomNav: Fallback executado após erro");
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
        /// 🎯 CORREÇÃO: Garante inicialização antes de mostrar (igual CrudNavBarComponent)
        /// </summary>
        public async Task ShowAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"✅ InactiveQueueBottomNav: ShowAsync chamado - IsVisible={this.IsVisible}");

                this.IsVisible = true;

                // 🎯 CORREÇÃO CRÍTICA: Garante inicialização antes de mostrar (igual CrudNavBarComponent)
                if (!_isInitialized)
                {
                    System.Diagnostics.Debug.WriteLine($"🎯 InactiveQueueBottomNav: Não inicializado - forçando inicialização");
                    EnsureInitialization();

                    // Aguarda um pouco para garantir que inicializou
                    await Task.Delay(50);
                }

                if (navBarBehavior != null)
                {
                    var buttonCount = navBarBehavior.Buttons?.Count ?? 0;
                    System.Diagnostics.Debug.WriteLine($"🔧 InactiveQueueBottomNav: Chamando navBarBehavior.ShowAsync() - Buttons.Count={buttonCount}");

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
                { "IsReady", IsReady },
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

                // 🔧 CORREÇÃO 1: Força IsReady=true para disparar inicialização
                IsReady = true;

                // 🔧 CORREÇÃO 2: Força inicialização se não foi feita
                if (!_isInitialized && Handler != null)
                {
                    EnsureInitialization();
                }

                // 🔧 CORREÇÃO 3: Força visibilidade
                this.IsVisible = true;

                // 🔧 CORREÇÃO 4: Tenta ShowAsync se navBarBehavior disponível
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