using Microsoft.Maui.Controls;
using MyKaraoke.View.Animations;

namespace MyKaraoke.View.Components
{
    public partial class InactiveQueueBottomNav : ContentView
    {
        public event EventHandler LocaisClicked;
        public event EventHandler CantoresClicked;
        public event EventHandler NovaFilaClicked;
        public event EventHandler BandasMusicosClicked;
        public event EventHandler HistoricoClicked;

        private AnimationManager _animationManager;

        public InactiveQueueBottomNav()
        {
            InitializeComponent();

            // Inicializa o AnimationManager para este componente
            _animationManager = new AnimationManager("InactiveQueueBottomNav");
        }

        /// <summary>
        /// Inicia a animação do botão Nova Fila automaticamente
        /// Método público para ser chamado pela view pai
        /// </summary>
        public async Task StartNovaFilaAnimationAsync()
        {
            try
            {
                // Log do hardware para debug
                HardwareDetector.LogHardwareInfo();

                // Encontra o StackLayout do botão Nova Fila (coluna 2)
                var novaFilaStack = FindNovaFilaStackLayout();
                if (novaFilaStack == null)
                {
                    System.Diagnostics.Debug.WriteLine("StackLayout Nova Fila não encontrado no InactiveQueueBottomNav");
                    return;
                }

                // 🎯 Configuração de animação mais rápida e intensa para Nova Fila
                var fastConfig = new AnimationConfig
                {
                    FromScale = 1.0,
                    ToScale = 1.25, // 25% maior conforme solicitado
                    PulseDuration = 250, // Mais rápido (era 400ms)
                    PulsePause = 400,    // Pausa menor (era 800ms)
                    PulseCount = 5,      // Mais pulses (era 3)
                    InitialDelay = 1000, // Delay menor (era 2000ms)
                    CycleInterval = 6000, // Ciclos mais frequentes (era 10000ms)
                    ExpandEasing = Easing.CubicOut,
                    ContractEasing = Easing.CubicIn,
                    AutoRepeat = true
                };

                // Usa o AnimationManager com configuração customizada
                await _animationManager.StartPulseAsync(
                    animationKey: "NovaFilaButton",
                    target: novaFilaStack,
                    config: fastConfig,
                    shouldContinue: () => this.IsVisible // Para quando o componente fica invisível
                );
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro na animação Nova Fila BottomNav: {ex.Message}");
            }
        }

        private StackLayout FindNovaFilaStackLayout()
        {
            try
            {
                // Corrigida navegação: ContentView → Frame → Grid(wrapper) → Grid(main) → StackLayout[coluna 2]
                if (this.Content is Frame mainFrame &&
                    mainFrame.Content is Grid wrapperGrid)
                {
                    // Busca o Grid principal (segunda linha do wrapper)
                    var mainGrid = wrapperGrid.Children
                        .OfType<Grid>()
                        .FirstOrDefault(g => Grid.GetRow(g) == 1);

                    if (mainGrid != null)
                    {
                        var stackLayouts = mainGrid.Children.OfType<StackLayout>().ToList();

                        foreach (var stack in stackLayouts)
                        {
                            if (Grid.GetColumn(stack) == 2) // Coluna 2 = Nova Fila
                            {
                                System.Diagnostics.Debug.WriteLine("StackLayout Nova Fila encontrado na coluna 2");
                                return stack;
                            }
                        }
                    }
                }

                System.Diagnostics.Debug.WriteLine("Estrutura XAML não encontrada para Nova Fila");
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao encontrar StackLayout Nova Fila: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Para a animação do botão Nova Fila
        /// Método público para ser chamado pela view pai
        /// </summary>
        public async Task StopNovaFilaAnimationAsync()
        {
            try
            {
                await _animationManager.StopAnimationAsync("NovaFilaButton");
                System.Diagnostics.Debug.WriteLine("Animação Nova Fila parada no BottomNav");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao parar animação BottomNav: {ex.Message}");
            }
        }

        /// <summary>
        /// Verifica se a animação está rodando
        /// </summary>
        public bool IsNovaFilaAnimationRunning => _animationManager.IsAnimationRunning("NovaFilaButton");

        // Event handlers para os botões
        private void OnLocaisClicked(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Botão Locais clicado no BottomNav");
            LocaisClicked?.Invoke(sender, e);
        }

        private async void OnCantoresClicked(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Botão Cantores clicado no BottomNav - navegando para PersonPage");

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

                        var personPage = serviceProvider.GetService<PersonPage>();

                        if (Application.Current?.MainPage is NavigationPage navPage)
                        {
                            await navPage.PushAsync(personPage);
                            CantoresClicked?.Invoke(sender, e);
                            return;
                        }
                        else if (parentPage.Navigation != null)
                        {
                            await parentPage.Navigation.PushAsync(personPage);
                            CantoresClicked?.Invoke(sender, e);
                            return;
                        }
                    }
                    catch (Exception serviceEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"Erro ao usar ServiceProvider: {serviceEx.Message}");
                        // Continua para o fallback abaixo
                    }
                }

                // Estratégia 2: Criação direta de PersonPage
                System.Diagnostics.Debug.WriteLine("Usando fallback - criação direta de PersonPage");
                var fallbackPersonPage = new PersonPage();

                if (Application.Current?.MainPage?.Navigation != null)
                {
                    await Application.Current.MainPage.Navigation.PushAsync(fallbackPersonPage);
                    CantoresClicked?.Invoke(sender, e);
                }
                else if (parentPage?.Navigation != null)
                {
                    await parentPage.Navigation.PushAsync(fallbackPersonPage);
                    CantoresClicked?.Invoke(sender, e);
                }
                else
                {
                    throw new InvalidOperationException("Nenhuma forma de navegação disponível");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro crítico na navegação Cantores: {ex.Message}");

                // Estratégia 3: Feedback ao usuário sobre o erro
                try
                {
                    var parentPage = FindParentPage();
                    if (parentPage != null)
                    {
                        await parentPage.DisplayAlert("Erro", "Não foi possível navegar para a página de cantores. Tente novamente.", "OK");
                    }
                }
                catch (Exception alertEx)
                {
                    System.Diagnostics.Debug.WriteLine($"Falha ao exibir alerta: {alertEx.Message}");
                }

                // Ainda assim invoca o evento para permitir tratamento customizado
                CantoresClicked?.Invoke(sender, e);
            }
        }

        private void OnNovaFilaClicked(object sender, EventArgs e)
        {
            // Para a animação quando o usuário clica
            _ = Task.Run(StopNovaFilaAnimationAsync);

            System.Diagnostics.Debug.WriteLine("Botão Nova Fila clicado no BottomNav");
            NovaFilaClicked?.Invoke(sender, e);
        }

        private void OnBandasMusicosClicked(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Botão Bandas/Músicos clicado no BottomNav");
            BandasMusicosClicked?.Invoke(sender, e);
        }

        private void OnHistoricoClicked(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Botão Histórico clicado no BottomNav");
            HistoricoClicked?.Invoke(sender, e);
        }

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

        /// <summary>
        /// Limpa recursos ao destruir o componente
        /// </summary>
        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();

            // Se o handler foi removido, limpa a animação
            if (Handler == null)
            {
                _animationManager?.Dispose();
                System.Diagnostics.Debug.WriteLine("AnimationManager disposed no OnHandlerChanged");
            }
        }

        /// <summary>
        /// Dispose manual para limpeza completa
        /// </summary>
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            // Para animações quando o contexto muda
            if (BindingContext == null)
            {
                _ = Task.Run(StopNovaFilaAnimationAsync);
                System.Diagnostics.Debug.WriteLine("Animação parada devido a mudança de BindingContext");
            }
        }
    }
}