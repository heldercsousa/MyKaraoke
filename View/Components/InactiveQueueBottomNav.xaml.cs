using Microsoft.Maui.Controls;
using MyKaraoke.View.Animations;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MyKaraoke.View.Components
{
    public partial class InactiveQueueBottomNav : ContentView
    {
        #region Events (mantidos para compatibilidade)

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

        #endregion

        public InactiveQueueBottomNav()
        {
            try
            {
                InitializeComponent();
                InitializeCommands();
                SetupButtons();

                System.Diagnostics.Debug.WriteLine("InactiveQueueBottomNav refatorado inicializado com sucesso");

                // 🎯 ESPECÍFICO DO INACTIVE QUEUE: Executa animações automaticamente ao carregar
                // Este comportamento é desejável neste contexto específico
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Task.Delay(100); // Pequeno delay para garantir que o layout esteja pronto
                    await ShowAsync(); // Inicia animações automaticamente APENAS para InactiveQueue
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro na inicialização do InactiveQueueBottomNav refatorado: {ex.Message}");
                throw;
            }
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

        private void SetupButtons()
        {
            try
            {
                var buttons = new ObservableCollection<NavButtonConfig>
                {
                    // Locais - botão regular com animações padrão (Fade + Translate)
                    new NavButtonConfig
                    {
                        Text = "Locais",
                        IconSource = "spot.png",
                        Command = LocaisCommand,
                        IsSpecial = false,
                        AnimationTypes = HardwareDetector.SupportsAnimations ? NavButtonAnimationType.ShowHide : NavButtonAnimationType.None,
                        IsAnimated = true
                    },
                    
                    // Bandokê - botão regular com animações padrão (Fade + Translate)
                    new NavButtonConfig
                    {
                        Text = "Bandokê",
                        IconSource = "musicos.png",
                        Command = BandokeCommand,
                        IsSpecial = false,
                        AnimationTypes = HardwareDetector.SupportsAnimations ? NavButtonAnimationType.ShowHide : NavButtonAnimationType.None,
                        IsAnimated = true
                    },
                    
                    // Nova Fila - botão especial com TODAS as 3 animações (Fade + Translate + Pulse)
                    new NavButtonConfig
                    {
                        Text = "Nova Fila",
                        CenterContent = "+",
                        Command = NovaFilaCommand,
                        IsSpecial = true,
                        GradientStyle = SpecialButtonGradientType.Yellow,
                        SpecialAnimationTypes = SpecialButtonAnimationType.Fade | SpecialButtonAnimationType.Translate | SpecialButtonAnimationType.Pulse, // 🎯 TODAS as 3 animações
                        IsAnimated = true
                    },
                    
                    // Histórico - botão regular com animações padrão (Fade + Translate)
                    new NavButtonConfig
                    {
                        Text = "Histórico",
                        IconSource = "historico.png",
                        Command = HistoricoCommand,
                        IsSpecial = false,
                        AnimationTypes = HardwareDetector.SupportsAnimations ? NavButtonAnimationType.ShowHide : NavButtonAnimationType.None,
                        IsAnimated = true
                    },
                    
                    // Administrar - botão regular com animações padrão (Fade + Translate)
                    new NavButtonConfig
                    {
                        Text = "Administrar",
                        IconSource = "manage.png",
                        Command = AdministrarCommand,
                        IsSpecial = false,
                        AnimationTypes = HardwareDetector.SupportsAnimations ? NavButtonAnimationType.ShowHide : NavButtonAnimationType.None,
                        IsAnimated = true
                    }
                };

                baseNavBar.Buttons = buttons;

                // Conecta evento do componente base
                baseNavBar.ButtonClicked += OnBaseNavBarButtonClicked;

                _isInitialized = true;
                System.Diagnostics.Debug.WriteLine("InactiveQueueBottomNav: 5 botões configurados - Nova Fila com 3 animações (Fade + Translate + Pulse)");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao configurar botões: {ex.Message}");
            }
        }

        #endregion

        #region Event Handlers

        private void OnBaseNavBarButtonClicked(object sender, NavBarButtonClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"BaseNavBar: Botão '{e.ButtonConfig.Text}' clicado");

                // Os commands já foram executados automaticamente pelo BaseNavBarComponent
                // Este evento é apenas para logging adicional se necessário
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

                // Estratégia 1: Usar ServiceProvider através da página pai
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
                        // Continua para o fallback abaixo
                    }
                }

                // Estratégia 2: Criação direta de SpotPage
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

                // Estratégia 3: Feedback ao usuário sobre o erro
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

                // Ainda assim invoca o evento para permitir tratamento customizado
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
                // Para a animação quando o usuário clica (mantém comportamento original)
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

        #region Animation Methods (compatibilidade com código existente)

        /// <summary>
        /// Inicia a animação do botão Nova Fila automaticamente
        /// Método público para ser chamado pela view pai (mantém compatibilidade)
        /// Agora com as 3 animações: Show/Hide (Fade + Translate) + Pulse especial
        /// </summary>
        public async Task StartNovaFilaAnimationAsync()
        {
            try
            {
                if (!_isInitialized)
                {
                    System.Diagnostics.Debug.WriteLine("InactiveQueueBottomNav ainda não inicializado, aguardando...");
                    await Task.Delay(100);
                }

                // Só executa se o hardware suportar animações
                if (!HardwareDetector.SupportsAnimations)
                {
                    System.Diagnostics.Debug.WriteLine("🚫 Hardware não suporta animações - Nova Fila animation BYPASS ativado");
                    return;
                }

                // Log do hardware para debug (mantém comportamento original)
                HardwareDetector.LogHardwareInfo();

                System.Diagnostics.Debug.WriteLine("Iniciando animação Nova Fila com 3 animações (Fade + Translate + Pulse) via BaseNavBarComponent");

                // Inicia animações especiais (automaticamente verifica se é suportado pelo hardware)
                // Isso irá disparar: 
                // 1. Show (Fade + Translate) quando a navbar aparecer
                // 2. Pulse contínuo para call-to-action
                await baseNavBar.StartSpecialAnimations();

                System.Diagnostics.Debug.WriteLine("Animação Nova Fila iniciada com sucesso - 3 animações ativas");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro na animação Nova Fila BottomNav refatorado: {ex.Message}");
            }
        }

        /// <summary>
        /// Para a animação do botão Nova Fila
        /// Método público para ser chamado pela view pai (mantém compatibilidade)
        /// </summary>
        public async Task StopNovaFilaAnimationAsync()
        {
            try
            {
                await baseNavBar.StopSpecialAnimations();
                System.Diagnostics.Debug.WriteLine("Animação Nova Fila parada no BottomNav refatorado");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao parar animação BottomNav refatorado: {ex.Message}");
            }
        }

        /// <summary>
        /// Verifica se a animação está rodando
        /// Propriedade para compatibilidade com código existente
        /// </summary>
        public bool IsNovaFilaAnimationRunning
        {
            get
            {
                try
                {
                    // Como a animação agora é gerenciada pelo componente SpecialNavButton,
                    // retornamos true se o navbar está visível, animado E o hardware suporta animações
                    return _isInitialized && baseNavBar.IsVisible && baseNavBar.IsAnimated && HardwareDetector.SupportsAnimations;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Mostra a navbar com animação
        /// Isso irá disparar automaticamente as animações Fade + Translate de todos os botões
        /// E depois o Pulse do botão Nova Fila
        /// Só executa se o hardware suportar animações (obedece HardwareDetector)
        /// 
        /// ESPECÍFICO DO INACTIVE QUEUE: Este componente SEMPRE quer animações ao aparecer
        /// (diferente de outros componentes que podem preferir aparecer sem animação)
        /// </summary>
        public async Task ShowAsync()
        {
            try
            {
                // Só executa animações se o hardware suportar (obedece a regra do HardwareDetector)
                if (HardwareDetector.SupportsAnimations)
                {
                    System.Diagnostics.Debug.WriteLine("🎬 InactiveQueue: Iniciando animações da navbar - hardware adequado detectado");
                    await baseNavBar.ShowAsync();
                    System.Diagnostics.Debug.WriteLine("InactiveQueueBottomNav mostrada com animação - botão Nova Fila com 3 animações ativas");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("🚫 InactiveQueue: Hardware limitado - navbar mostrada sem animações (BYPASS ativo)");
                    baseNavBar.IsVisible = true;
                    // Em hardware limitado, apenas torna visível sem animações (respeita HardwareDetector)
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao mostrar InactiveQueueBottomNav: {ex.Message}");
                // Fallback: apenas torna visível
                baseNavBar.IsVisible = true;
            }
        }

        /// <summary>
        /// Esconde a navbar com animação
        /// </summary>
        public async Task HideAsync()
        {
            try
            {
                await baseNavBar.HideAsync();
                System.Diagnostics.Debug.WriteLine("InactiveQueueBottomNav escondida com animação");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao esconder InactiveQueueBottomNav: {ex.Message}");
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Encontra a página pai que contém este ContentView
        /// Necessário porque ServiceProvider.FromPage() espera uma Page
        /// </summary>
        private Page FindParentPage()
        {
            try
            {
                // Navega pela hierarquia visual para encontrar a Page pai
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

                // Se não encontrou pela hierarquia, tenta pela MainPage atual
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
            }
            else if (!_isInitialized)
            {
                // Tenta configurar botões novamente se não foi inicializado
                SetupButtons();
            }
            else
            {
                // 🎯 ESPECÍFICO DO INACTIVE QUEUE: Re-executa animações quando Handler estiver disponível
                // Este comportamento é desejável para este componente específico
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Task.Delay(50); // Pequeno delay para garantir que o layout esteja pronto
                    await ShowAsync(); // Re-inicia animações quando handler estiver pronto
                });
            }
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (BindingContext == null)
            {
                // Para animações quando o contexto muda
                _ = Task.Run(StopNovaFilaAnimationAsync);
                System.Diagnostics.Debug.WriteLine("Animação parada devido a mudança de BindingContext");
            }
        }

        #endregion
    }
}