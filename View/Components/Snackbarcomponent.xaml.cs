using Microsoft.Maui.Controls;

namespace MyKaraoke.View.Components
{
    public partial class SnackbarComponent : ContentView
    {
        public SnackbarComponent()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Mostra snackbar de sucesso (verde)
        /// </summary>
        public async Task ShowSuccessAsync(string message, int durationMs = 3000)
        {
            await ShowAsync(message, "success_icon.png", "#2E7D32", durationMs);
        }

        /// <summary>
        /// Mostra snackbar de erro (vermelho)
        /// </summary>
        public async Task ShowErrorAsync(string message, int durationMs = 4000)
        {
            await ShowAsync(message, "error_icon.png", "#D32F2F", durationMs);
        }

        /// <summary>
        /// Mostra snackbar de aviso (laranja)
        /// </summary>
        public async Task ShowWarningAsync(string message, int durationMs = 3500)
        {
            await ShowAsync(message, "warning_icon.png", "#F57C00", durationMs);
        }

        private async Task ShowAsync(string message, string iconSource, string backgroundColor, int durationMs)
        {
            try
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    // Configura conteúdo
                    MessageLabel.Text = message;
                    IconImage.Source = iconSource;
                    SnackbarFrame.BackgroundColor = Color.FromArgb(backgroundColor);

                    // Reseta posição inicial
                    SnackbarFrame.TranslationY = 100;
                    this.IsVisible = true;

                    // Anima entrada (slide up)
                    await SnackbarFrame.TranslateTo(0, 0, 250, Easing.CubicOut);

                    // Aguarda duração
                    await Task.Delay(durationMs);

                    // Anima saída (slide down)
                    await SnackbarFrame.TranslateTo(0, 100, 200, Easing.CubicIn);
                    this.IsVisible = false;
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SnackbarComponent: Erro ao exibir: {ex.Message}");
            }
        }
    }
}