using Microsoft.Maui.Controls;
using MyKaraoke.View.Animations;

namespace MyKaraoke.View.Components
{
    public partial class InactiveQueueBottomNav : ContentView
    {
        public event EventHandler LocaisClicked;
        public event EventHandler BandokeClicked;
        public event EventHandler NovaFilaClicked;
        public event EventHandler HistoricoClicked;
        public event EventHandler AdministrarClicked;

        private AnimationManager _animationManager;

        public InactiveQueueBottomNav()
        {
            try
            {
                InitializeComponent();

                // Inicializa o AnimationManager para este componente
                _animationManager = new AnimationManager("InactiveQueueBottomNav");

                System.Diagnostics.Debug.WriteLine("InactiveQueueBottomNav inicializado com sucesso");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro na inicialização do InactiveQueueBottomNav: {ex.Message}");

                // Log do erro mas não interrompe a aplicação
                // O fallback será tratado pela página pai se necessário
                throw;
            }
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

                // 🔥 CONFIGURAÇÃO INTENSA PARA NOVA FILA - 100% MAIOR
                var intensiveConfig = new AnimationConfig
                {
                    FromScale = 1.0,
                    ToScale = 1.25, 
                    PulseDuration = 150, // Muito rápido
                    PulsePause = 100,    // Pausa curta
                    PulseCount = 5,      // 5 pulses por ciclo
                    InitialDelay = 1000, // Inicia rápido
                    CycleInterval = 6000, // Repete a cada 6 segundos
                    ExpandEasing = Easing.BounceOut, // 🎯 Easing mais dramático
                    ContractEasing = Easing.CubicIn,
                    AutoRepeat = true
                };

                System.Diagnostics.Debug.WriteLine($"🎯 Iniciando animação INTENSA: ToScale={intensiveConfig.ToScale}, Duration={intensiveConfig.PulseDuration}ms");

                // ✅ Sistema corrigido: HardwareDetector agora preserva sua configuração
                // Para Pixel 5 e hardware similar, usará EXATAMENTE sua configuração
                await _animationManager.StartPulseAsync(
                    animationKey: "NovaFilaButton",
                    target: novaFilaStack,
                    config: intensiveConfig,
                    shouldContinue: () => this.IsVisible
                );

                System.Diagnostics.Debug.WriteLine("🚀 Animação Nova Fila iniciada - sistema automaticamente detectará se hardware suporta");
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

        private void OnBandokeClicked(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Botão Bandokê clicado no BottomNav");
            BandokeClicked?.Invoke(sender, e);
        }

        private void OnNovaFilaClicked(object sender, EventArgs e)
        {
            // Para a animação quando o usuário clica
            _ = Task.Run(StopNovaFilaAnimationAsync);

            System.Diagnostics.Debug.WriteLine("Botão Nova Fila clicado no BottomNav");
            NovaFilaClicked?.Invoke(sender, e);
        }

        private void OnHistoricoClicked(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Botão Histórico clicado no BottomNav");
            HistoricoClicked?.Invoke(sender, e);
        }

        private void OnAdministrarClicked(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Botão Administrar clicado no BottomNav");
            AdministrarClicked?.Invoke(sender, e);
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