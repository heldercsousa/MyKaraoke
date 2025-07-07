using MyKaraoke.Services;

namespace MyKaraoke.View
{
    public partial class SplashLoadingPage : ContentPage
    {
        public SplashLoadingPage()
        {
            try
            {
                InitializeComponent();
                System.Diagnostics.Debug.WriteLine("[DEBUG] SplashLoadingPage: InitializeComponent completado");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR] SplashLoadingPage InitializeComponent: {ex.Message}");
                CreateEmergencyUI();
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Inicia o processo de carregamento automaticamente
            Task.Run(async () => await SimulateLoading());
        }

        public void UpdateStatus(string status, double progressPercentage)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (StatusLabel != null)
                {
                    StatusLabel.Text = status;
                }
                if (LoadingProgressBar != null)
                {
                    LoadingProgressBar.Progress = progressPercentage;
                }
                System.Diagnostics.Debug.WriteLine($"[STATUS] {status} - {progressPercentage:P0}");
            });
        }

        public async Task SimulateLoading()
        {
            // Etapas de carregamento básicas
            string[] loadingSteps = new string[] {
                "Configurando ambiente...",
                "Carregando recursos...",
                "Inicializando componentes...",
                "Preparando interface...",
                "Finalizando..."
            };

            try
            {
                for (int i = 0; i < loadingSteps.Length; i++)
                {
                    // Atualiza status e barra de progresso
                    UpdateStatus(loadingSteps[i], (double)(i + 1) / loadingSteps.Length);

                    // Simula tempo de processamento
                    await Task.Delay(300);
                }

                // Aguarda um pouco e navega para SplashPage
                await Task.Delay(500);
                await NavigateToSplashPage();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR] SimulateLoading: {ex.Message}");
                UpdateStatus("Erro detectado - continuando...", 1.0);

                await Task.Delay(1500);
                await NavigateToSplashPage();
            }
        }

        private async Task NavigateToSplashPage()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("[DEBUG] Navegando para SplashPage");

                // Navega para SplashPage
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Application.Current.MainPage = new SplashPage();
                    System.Diagnostics.Debug.WriteLine("[SUCCESS] Navegação para SplashPage realizada");
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR] Erro ao navegar para SplashPage: {ex.Message}");

                // Fallback direto para TonguePage se SplashPage falhar
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Application.Current.MainPage = new NavigationPage(new TonguePage());
                    System.Diagnostics.Debug.WriteLine("[FALLBACK] Navegação direta para TonguePage");
                });
            }
        }

        private void CreateEmergencyUI()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("[DEBUG] Criando UI de emergência");

                Content = new Grid
                {
                    BackgroundColor = Color.FromHex("#221b3c"),
                    Children =
                    {
                        new Label
                        {
                            Text = "MyKaraoke\nCarregando...",
                            TextColor = Colors.White,
                            FontSize = 24,
                            HorizontalOptions = LayoutOptions.Center,
                            VerticalOptions = LayoutOptions.Center,
                            HorizontalTextAlignment = TextAlignment.Center
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CRITICAL] Erro na UI de emergência: {ex.Message}");
            }
        }

        // Impede o botão voltar durante o loading
        protected override bool OnBackButtonPressed()
        {
            return true; // Bloqueia o botão voltar
        }
    }
}