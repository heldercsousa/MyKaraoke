using MyKaraoke.Services;

namespace MyKaraoke.View
{
    public partial class App : Application
    {
        public App()
        {
            try
            {
                InitializeComponent();
                MainPage = new SplashLoadingPage();
                System.Diagnostics.Debug.WriteLine("[DEBUG] App iniciado com SplashLoadingPage");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR] Erro ao inicializar App: {ex.Message}");
                CreateFallbackPage();
            }
        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            var window = base.CreateWindow(activationState);

            // **INICIALIZAÇÃO EM SEQUÊNCIA CORRETA**
            Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(1000); // Aguarda estabilizar

                    var serviceProvider = window?.Handler?.MauiContext?.Services;
                    if (serviceProvider != null)
                    {
                        // **1. PRIMEIRO: Inicializar banco (DatabaseService)**
                        var databaseService = serviceProvider.GetService<IDatabaseService>();
                        if (databaseService != null)
                        {
                            await databaseService.InitializeDatabaseAsync();
                            System.Diagnostics.Debug.WriteLine("[SUCCESS] DatabaseService inicializado");
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[WARNING] Erro na inicialização: {ex.Message}");
                }
            });

            return window;
        }

        private void CreateFallbackPage()
        {
            try
            {
                MainPage = new ContentPage
                {
                    BackgroundColor = Color.FromHex("#221b3c"),
                    Content = new Label
                    {
                        Text = "MyKaraoke\nInicializando...",
                        TextColor = Colors.White,
                        FontSize = 24,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalTextAlignment = TextAlignment.Center
                    }
                };
            }
            catch (Exception criticalEx)
            {
                System.Diagnostics.Debug.WriteLine($"[CRITICAL] Fallback falhou: {criticalEx.Message}");
            }
        }
    }
}