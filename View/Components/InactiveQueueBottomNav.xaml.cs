// InactiveQueueBottomNav.xaml.cs

using Microsoft.Maui.Controls;
using MyKaraoke.View.Animations;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq; // Added for .Any() in baseNavBar's methods if needed elsewhere

namespace MyKaraoke.View.Components
{
    public partial class InactiveQueueBottomNav : ContentView
    {
        #region Events

        public event EventHandler LocaisClicked;
        public event EventHandler BandokeClicked;
        public event EventHandler NovaFilaClicked;
        public event EventHandler HistoricoClicked;
        public event EventHandler AdministrarClicked;

        #endregion

        #region Commands

        public ICommand LocaisCommand { get; private set; }
        public ICommand BandokeCommand { get; private set; }
        public ICommand NovaFilaCommand { get; private set; }
        public ICommand HistoricoCommand { get; private set; }
        public ICommand AdministrarCommand { get; private set; }

        #endregion

        #region Private Fields

        private bool _isInitialized = false;
        // private bool _isShowing = false; // Removed this flag as it's no longer needed with the new strategy

        #endregion


        public InactiveQueueBottomNav()
        {
            try
            {
                InitializeComponent();
                InitializeCommands();
                // SetupButtons() will now be called by OnHandlerChanged on initial setup.
                // This ensures baseNavBar is configured at the right lifecycle point.
                // SetupButtons(); // Removed from constructor to avoid premature setup.

                System.Diagnostics.Debug.WriteLine("InactiveQueueBottomNav refatorado inicializado com sucesso");

                // Set initial state for the *entire* component to be hidden by default
                // This prevents flicker before BaseNavBarComponent animates its children.
                this.Opacity = 0;
                this.IsVisible = false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro na inicialização do InactiveQueueBottomNav refatorado: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Public entry point to trigger the navbar display animation.
        /// This will be called by the parent page (StackPage) OnAppearing.
        /// </summary>
        public async Task StartShowAnimations()
        {
            System.Diagnostics.Debug.WriteLine("InactiveQueueBottomNav: StartShowAnimations chamado pela página pai.");

            // Torna o container visível para que as animações internas possam ocorrer
            this.IsVisible = true;
            this.Opacity = 1;

            if (baseNavBar == null)
            {
                System.Diagnostics.Debug.WriteLine("InactiveQueueBottomNav: baseNavBar é nula. Não é possível iniciar animações.");
                return;
            }

            // CORREÇÃO: Não há mais necessidade de reconstruir os botões aqui.
            // O método HideAsync chamado no OnDisappearing da StackPage já reiniciou o estado.
            // Apenas chamamos ShowAsync, que agora irá funcionar corretamente.
            if (HardwareDetector.SupportsAnimations)
            {
                System.Diagnostics.Debug.WriteLine("InactiveQueueBottomNav: Acionando baseNavBar.ShowAsync()");
                await baseNavBar.ShowAsync();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("InactiveQueueBottomNav: Hardware limitado, mostrando baseNavBar sem animações.");
                baseNavBar.IsVisible = true;
            }

            // O pulse especial ainda é iniciado separadamente, o que está correto.
            await StartNovaFilaAnimationAsync();

            System.Diagnostics.Debug.WriteLine("Animações da InactiveQueueBottomNav acionadas com sucesso.");
        }

        #region Initialization

        private void InitializeCommands()
        {
            LocaisCommand = new Command(async () => await OnLocaisClickedAsync());
            BandokeCommand = new Command(async () => await OnBandokeClickedAsync());
            NovaFilaCommand = new Command(async () => await OnNovaFilaClickedAsync());
            HistoricoCommand = new Command(async () => await OnHistoricoClickedAsync());
            AdministrarCommand = new Command(async () => await OnAdministrarClickedAsync());
        }

        private void SetupButtons() // Original SetupButtons, just preparing the config
        {
            if (_isInitialized)
            {
                System.Diagnostics.Debug.WriteLine("InactiveQueueBottomNav: SetupButtons ignored, already initialized.");
                return;
            }
            // This method will only define the 'buttons' collection.
            // The actual assignment to baseNavBar.Buttons will happen in ConfigureBaseNavBar().
            // This separates button definition from their application to the UI.
            _isInitialized = true; // Mark as initialized after defining the button configs.
            System.Diagnostics.Debug.WriteLine("InactiveQueueBottomNav: Buttons configuration defined.");
        }

        /// <summary>
        /// Configures the BaseNavBarComponent with its buttons.
        /// Calling this method will trigger BaseNavBarComponent's OnButtonsChanged
        /// which then calls RebuildButtons, effectively resetting the animation state
        /// of the internal NavButtonComponents.
        /// </summary>
        private void ConfigureBaseNavBar()
        {
            try
            {
                var buttons = new ObservableCollection<NavButtonConfig>
                {
                    // Locais - botão regular com animações Fade + Translate
                    new NavButtonConfig
                    {
                        Text = "Locais", IconSource = "spot.png", Command = LocaisCommand, IsSpecial = false,
                        AnimationTypes = HardwareDetector.SupportsAnimations ? (NavButtonAnimationType.Fade | NavButtonAnimationType.Translate) : NavButtonAnimationType.None,
                        IsAnimated = true
                    },
                    // Bandokê - botão regular com animações Fade + Translate
                    new NavButtonConfig
                    {
                        Text = "Bandokê", IconSource = "musicos.png", Command = BandokeCommand, IsSpecial = false,
                        AnimationTypes = HardwareDetector.SupportsAnimations ? (NavButtonAnimationType.Fade | NavButtonAnimationType.Translate) : NavButtonAnimationType.None,
                        IsAnimated = true
                    },
                    // Nova Fila - botão especial with all 3 animations (Fade + Translate + Pulse)
                    new NavButtonConfig
                    {
                        Text = "Nova Fila", CenterContent = "+", Command = NovaFilaCommand, IsSpecial = true, GradientStyle = SpecialButtonGradientType.Yellow,
                        SpecialAnimationTypes = HardwareDetector.SupportsAnimations ? (SpecialButtonAnimationType.Fade | SpecialButtonAnimationType.Translate | SpecialButtonAnimationType.Pulse) : SpecialButtonAnimationType.None,
                        IsAnimated = true
                    },
                    // Histórico - botão regular with animations Fade + Translate
                    new NavButtonConfig
                    {
                        Text = "Histórico", IconSource = "historico.png", Command = HistoricoCommand, IsSpecial = false,
                        AnimationTypes = HardwareDetector.SupportsAnimations ? (NavButtonAnimationType.Fade | NavButtonAnimationType.Translate) : NavButtonAnimationType.None,
                        IsAnimated = true
                    },
                    // Administrar - botão regular with animations Fade + Translate
                    new NavButtonConfig
                    {
                        Text = "Administrar", IconSource = "manage.png", Command = AdministrarCommand, IsSpecial = false,
                        AnimationTypes = HardwareDetector.SupportsAnimations ? (NavButtonAnimationType.Fade | NavButtonAnimationType.Translate) : NavButtonAnimationType.None,
                        IsAnimated = true
                    }
                };

                // Assigning this will trigger BaseNavBarComponent.OnButtonsChanged -> RebuildButtons()
                // which will clear and re-add button views, effectively resetting their animation state.
                baseNavBar.Buttons = buttons;
                baseNavBar.IsAnimated = true;
                baseNavBar.ShowAnimationDelay = 80;

                // Ensure event handler is subscribed only once
                baseNavBar.ButtonClicked -= OnBaseNavBarButtonClicked; // Unsubscribe to be safe
                baseNavBar.ButtonClicked += OnBaseNavBarButtonClicked; // Then subscribe

                System.Diagnostics.Debug.WriteLine("InactiveQueueBottomNav: BaseNavBarComponent configured. Rebuild should be triggered.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao configurar BaseNavBarComponent: {ex.Message}");
            }
        }

        #endregion

        #region Event Handlers (remain unchanged)

        private void OnBaseNavBarButtonClicked(object sender, NavBarButtonClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"BaseNavBar: Botão '{e.ButtonConfig.Text}' clicado");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no evento base navbar: {ex.Message}");
            }
        }

        private async Task OnLocaisClickedAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Botão Locais clicado no BottomNav refatorado - navegando para SpotPage");

                var parentPage = FindParentPage();
                if (parentPage != null)
                {
                    try
                    {
                        var serviceProvider = new View.ServiceProvider(
                            parentPage.Handler?.MauiContext?.Services ??
                            throw new InvalidOperationException("MauiContext não disponível")
                        );

                        var spotPage = serviceProvider.GetService<SpotPage>();

                        if (Application.Current?.MainPage is NavigationPage navPage)
                        {
                            await navPage.PushAsync(spotPage);
                            LocaisClicked?.Invoke(this, EventArgs.Empty);
                            return;
                        }
                        else if (parentPage.Navigation != null)
                        {
                            await parentPage.Navigation.PushAsync(spotPage);
                            LocaisClicked?.Invoke(this, EventArgs.Empty);
                            return;
                        }
                    }
                    catch (Exception serviceEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"Erro ao usar ServiceProvider: {serviceEx.Message}");
                    }
                }

                System.Diagnostics.Debug.WriteLine("Usando fallback - criação direta de SpotPage");
                var fallbackSpotPage = new SpotPage();

                if (Application.Current?.MainPage?.Navigation != null)
                {
                    await Application.Current.MainPage.Navigation.PushAsync(fallbackSpotPage);
                    LocaisClicked?.Invoke(this, EventArgs.Empty);
                }
                else if (parentPage?.Navigation != null)
                {
                    await parentPage.Navigation.PushAsync(fallbackSpotPage);
                    LocaisClicked?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    throw new InvalidOperationException("Nenhuma forma de navegação disponível");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro crítico na navegação Locais: {ex.Message}");

                try
                {
                    var parentPage = FindParentPage();
                    if (parentPage != null)
                    {
                        await parentPage.DisplayAlert("Erro", "Não foi possível navegar para a página de locais. Tente novamente.", "OK");
                    }
                }
                catch (Exception alertEx)
                {
                    System.Diagnostics.Debug.WriteLine($"Falha ao exibir alerta: {alertEx.Message}");
                }

                LocaisClicked?.Invoke(this, EventArgs.Empty);
            }
        }

        private async Task OnBandokeClickedAsync()
        {
            await Task.Run(() =>
            {
                System.Diagnostics.Debug.WriteLine("Botão Bandokê clicado no BottomNav refatorado");
                BandokeClicked?.Invoke(this, EventArgs.Empty);
            });
        }

        private async Task OnNovaFilaClickedAsync()
        {
            try
            {
                await StopNovaFilaAnimationAsync();

                await Task.Run(() =>
                {
                    System.Diagnostics.Debug.WriteLine("Botão Nova Fila clicado no BottomNav refatorado");
                    NovaFilaClicked?.Invoke(this, EventArgs.Empty);
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no clique Nova Fila: {ex.Message}");
            }
        }

        private async Task OnHistoricoClickedAsync()
        {
            await Task.Run(() =>
            {
                System.Diagnostics.Debug.WriteLine("Botão Histórico clicado no BottomNav refatorado");
                HistoricoClicked?.Invoke(this, EventArgs.Empty);
            });
        }

        private async Task OnAdministrarClickedAsync()
        {
            await Task.Run(() =>
            {
                System.Diagnostics.Debug.WriteLine("Botão Administrar clicado no BottomNav refatorado");
                AdministrarClicked?.Invoke(this, EventArgs.Empty);
            });
        }

        #endregion

        #region Animation Methods

        public async Task StartNovaFilaAnimationAsync()
        {
            try
            {
                if (!_isInitialized) { System.Diagnostics.Debug.WriteLine("⚠️ InactiveQueueBottomNav ainda não inicializado, aguardando..."); await Task.Delay(200); if (!_isInitialized) { System.Diagnostics.Debug.WriteLine("❌ InactiveQueueBottomNav não inicializou a tempo - abortando animação"); return; } }
                if (!HardwareDetector.SupportsAnimations) { System.Diagnostics.Debug.WriteLine("🚫 Hardware não suporta animações - Nova Fila animation BYPASS ativado"); return; }
                HardwareDetector.LogHardwareInfo();
                System.Diagnostics.Debug.WriteLine("🎬 Iniciando animação Nova Fila com 3 animações (Fade + Translate + Pulse) via BaseNavBarComponent");
                if (baseNavBar == null) { System.Diagnostics.Debug.WriteLine("❌ baseNavBar not available - aborting animation"); return; }
                await baseNavBar.StopSpecialAnimations();
                await baseNavBar.StartSpecialAnimations();
                System.Diagnostics.Debug.WriteLine("✅ Nova Fila pulse animation initiated successfully.");
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"❌ Error in Nova Fila pulse animation: {ex.Message}"); }
        }

        public async Task StopNovaFilaAnimationAsync()
        {
            try { if (baseNavBar != null) { await baseNavBar.StopSpecialAnimations(); System.Diagnostics.Debug.WriteLine("✅ Animação Nova Fila parada no BottomNav refatorado"); } }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"❌ Erro ao parar animação BottomNav refatorado: {ex.Message}"); }
        }

        public bool IsNovaFilaAnimationRunning
        {
            get { try { return _isInitialized && baseNavBar != null && baseNavBar.IsVisible && baseNavBar.IsAnimated && HardwareDetector.SupportsAnimations; } catch { return false; } }
        }

        /// <summary>
        /// Hides the entire InactiveQueueBottomNav component and its internal BaseNavBarComponent.
        /// This should be called when the StackPage is disappearing.
        /// </summary>
        public async Task HideAsync()
        {
            try
            {
                if (baseNavBar != null)
                {
                    // Calling HideAsync on baseNavBar ensures its internal _isShown flag is reset.
                    // This is crucial for re-animation upon next ShowAsync.
                    await baseNavBar.HideAsync();
                    System.Diagnostics.Debug.WriteLine("InactiveQueueBottomNav - baseNavBar hidden and state reset.");
                }
                // Also hide the entire InactiveQueueBottomNav component itself
                this.IsVisible = false;
                this.Opacity = 0;
                System.Diagnostics.Debug.WriteLine("InactiveQueueBottomNav escondida completamente");
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Erro ao esconder InactiveQueueBottomNav: {ex.Message}"); this.IsVisible = false; this.Opacity = 0; }
        }

        #endregion

        #region Helper Methods

        private Page FindParentPage()
        {
            try
            {
                Element current = this.Parent;

                while (current != null)
                {
                    if (current is Page page)
                    {
                        System.Diagnostics.Debug.WriteLine($"Página pai encontrada: {page.GetType().Name}");
                        return page;
                    }
                    current = current.Parent;
                }

                var mainPage = Application.Current?.MainPage;
                if (mainPage != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Usando MainPage como fallback: {mainPage.GetType().Name}");
                    return mainPage;
                }

                System.Diagnostics.Debug.WriteLine("Nenhuma página pai encontrada");
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao encontrar página pai: {ex.Message}");
                return Application.Current?.MainPage;
            }
        }

        #endregion

        #region Lifecycle Methods

        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();
            if (Handler == null)
            {
                System.Diagnostics.Debug.WriteLine("InactiveQueueBottomNav handler removido");
                _isInitialized = false; // Reset if handler is removed for proper re-init
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("🔄 InactiveQueueBottomNav handler disponível.");
                if (!_isInitialized)
                {
                    SetupButtons(); // Defines the configs (sets _isInitialized = true)
                    ConfigureBaseNavBar(); // Applies configs to baseNavBar and triggers its rebuild
                }
            }
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (BindingContext == null) { _ = Task.Run(StopNovaFilaAnimationAsync); System.Diagnostics.Debug.WriteLine("Animação parada devido a mudança de BindingContext"); }
        }

        #endregion
    }
}