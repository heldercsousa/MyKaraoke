using MyVocaList.View.Behaviors;
using MyVocaList.View.Components;
using System.Collections.ObjectModel;
using MyVocaList.View.Animations;
using Serilog;

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

        #region private Props
        private static readonly ILogger Logger = Log.ForContext<InactiveQueueBottomNav>();
        #endregion

        public InactiveQueueBottomNav()
        {
            InitializeComponent();
            InitializeNavigationBehaviors();

            // 🎯 CORREÇÃO CRÍTICA: Define IsReady=true para disparar inicialização
            IsReady = true;
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
                navBar.EnsureInitialization();
            }
        }

        /// <summary>
        /// 🎯 CORREÇÃO: Garante inicialização mesmo se OnHandlerChanged falhar/atrasar
        /// </summary>
        private void EnsureInitialization()
        {
            if (!_isInitialized)
            {
                if (Handler != null && navBarBehavior != null)
                {
                    InitializeNavBar();
                    _isInitialized = true;
                }
                else
                {
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
                var buttonCount = navBarBehavior?.Buttons?.Count ?? 0;
                if (buttonCount == 0)
                {
                    InitializeNavBar();
                }
            }
            
        }

        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();

            if (Handler != null && !_isInitialized)
            {
                EnsureInitialization();
            }
        }

        /// <summary>
        /// 🎯 NAVEGAÇÃO: Inicializa SafeNavigationBehaviors para navegação segura
        /// </summary>
        private void InitializeNavigationBehaviors()
        {
            _locaisNavigationBehavior = new SafeNavigationBehavior
            {
                TargetPageType = typeof(SpotPage),          // ← Para saber qual página criar
                EnableSmartStackNavigation = true,          // ← Para ser inteligente
                DebounceMilliseconds = 1000,
                CreatePageFunc = () => new SpotPage()       // ← Como criar quando necessário
            };
        }

        /// <summary>
        /// ✅ CONFIGURAÇÃO: Define todos os botões através do NavBarBehavior
        /// 🎯 CORREÇÃO: Mais logs para debug
        /// </summary>
        private void InitializeNavBar()
        {
            if (navBarBehavior == null)
            {
                return;
            }

            var buttons = new ObservableCollection<NavButtonConfig>
            {
                // MD3 SVG icons following iconography guideline
                new NavButtonConfig
                {
                    Text = "Venues",
                    IconName = "nightlife",           // MD3: Venues icon (karaoke atmosphere)
                    Command = new Command(() => OnLocaisClicked()),
                    IsAnimated = true,
                    AnimationTypes = NavButtonAnimationType.ShowHide
                },
                new NavButtonConfig
                {
                    Text = "Musicians",
                    IconName = "music_note",          // MD3: Musicians/Bands icon
                    Command = new Command(() => OnBandokeClicked()),
                    IsAnimated = true,
                    AnimationTypes = NavButtonAnimationType.ShowHide
                },
                new NavButtonConfig
                {
                    Text = "History",
                    IconName = "history",             // MD3: History icon (chronological record)
                    Command = new Command(() => OnHistoricoClicked()),
                    IsAnimated = true,
                    AnimationTypes = NavButtonAnimationType.ShowHide
                },
                new NavButtonConfig
                {
                    Text = "Settings",
                    IconName = "settings",            // MD3: Settings icon (app configuration)
                    Command = new Command(() => OnAdministrarClicked()),
                    IsAnimated = true,
                    AnimationTypes = NavButtonAnimationType.ShowHide
                }
            };

            navBarBehavior.Buttons = buttons;

            // 🎯 VERIFICATION: Confirm if buttons were set
            var setButtonsCount = navBarBehavior.Buttons?.Count ?? 0;
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
                    return false;
                }

                _lastEventTime = now;
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
            }
            catch
            {
                try
                {
                    LocaisClicked?.Invoke(this, EventArgs.Empty);
                }
                catch
                {
                    throw;
                }
                throw;
            }
        }

        private void OnBandokeClicked()
        {
            if (!ShouldProcessEvent("BandokeClicked")) return;
            BandokeClicked?.Invoke(this, EventArgs.Empty);
        }

        private void OnNovaFilaClicked()
        {
            if (!ShouldProcessEvent("NovaFilaClicked")) return;
            NovaFilaClicked?.Invoke(this, EventArgs.Empty);
        }

        private void OnHistoricoClicked()
        {
            if (!ShouldProcessEvent("HistoricoClicked")) return;
            HistoricoClicked?.Invoke(this, EventArgs.Empty);
        }

        private void OnAdministrarClicked()
        {
            if (!ShouldProcessEvent("AdministrarClicked")) return;
            AdministrarClicked?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region IAnimatableNavBar - DELEGADO PARA BEHAVIOR

        /// <summary>
        /// ✅ DELEGADO: ShowAsync via NavBarBehavior
        /// 🎯 CORREÇÃO: Garante inicialização antes de mostrar (igual CrudNavBarComponent)
        /// </summary>
        // InactiveQueueBottomNav.xaml.cs

        public async Task ShowAsync()
        {
            // ✅ FIX: Force execution on Main Thread to ensure UI updates apply
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                this.IsVisible = true;

                // Ensure initialization if it hasn't happened yet
                if (!_isInitialized)
                {
                    EnsureInitialization();
                    // Give the UI a moment to construct the grid children
                    await Task.Delay(50);
                }

                if (navBarBehavior != null)
                {
                    var buttonCount = navBarBehavior.Buttons?.Count ?? 0;

                    await navBarBehavior.ShowAsync();
                }
                else
                {
                    await NavBarExtensions.ShowAsync(navGrid);
                }
                
            });
        }

        /// <summary>
        /// ✅ DELEGADO: HideAsync via NavBarBehavior
        /// </summary>
        public async Task HideAsync()
        {
            if (navBarBehavior != null)
            {
                await navBarBehavior.HideAsync();
            }
            else
            {
                await NavBarExtensions.HideAsync(navGrid);
            }
            this.IsVisible = false;
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
                await navBarBehavior.ShowAsync();
            }

        }

        #endregion
    }
}