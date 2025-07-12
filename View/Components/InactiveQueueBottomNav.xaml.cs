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

            // 🆕 Inicializa o AnimationManager para este componente
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
                var novaFilaStack = FindNovaFilaStackLayout(); // Método próprio
                if (novaFilaStack == null)
                {
                    System.Diagnostics.Debug.WriteLine("StackLayout Nova Fila não encontrado no InactiveQueueBottomNav");
                    return;
                }

                // 🆕 Usa o AnimationManager centralizado
                await _animationManager.StartCallToActionAsync(
                    animationKey: "NovaFilaButton",
                    target: novaFilaStack,
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
            // Navega pela hierarquia: ContentView → Frame → Grid → StackLayout[coluna 2]
            if (this.Content is Frame mainFrame &&
                mainFrame.Content is Grid mainGrid)
            {
                var stackLayouts = mainGrid.Children.OfType<StackLayout>().ToList();

                foreach (var stack in stackLayouts)
                {
                    if (Grid.GetColumn(stack) == 2) // Coluna 2 = Nova Fila
                    {
                        return stack;
                    }
                }
            }
            return null;
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

        // Event handlers existentes permanecem iguais
        private void OnLocaisClicked(object sender, EventArgs e)
        {
            LocaisClicked?.Invoke(sender, e);
        }

        private void OnCantoresClicked(object sender, EventArgs e)
        {
            CantoresClicked?.Invoke(sender, e);
        }

        private void OnNovaFilaClicked(object sender, EventArgs e)
        {
            // Para a animação quando o usuário clica
            _ = Task.Run(StopNovaFilaAnimationAsync);

            NovaFilaClicked?.Invoke(sender, e);
        }

        private void OnBandasMusicosClicked(object sender, EventArgs e)
        {
            BandasMusicosClicked?.Invoke(sender, e);
        }

        private void OnHistoricoClicked(object sender, EventArgs e)
        {
            HistoricoClicked?.Invoke(sender, e);
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
            }
        }
    }
}