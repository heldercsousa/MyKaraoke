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

                // Inicia com SplashLoadingPage (substitui tela rosa)
                MainPage = new SplashLoadingPage();
                System.Diagnostics.Debug.WriteLine("[DEBUG] App iniciado com SplashLoadingPage");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR] Erro ao inicializar App: {ex.Message}");

                // Fallback para página de emergência
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

        protected override Window CreateWindow(IActivationState activationState)
        {
            var window = base.CreateWindow(activationState);

            // Inicialização do banco em background (não bloqueia UI)
            Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(500); // Aguarda handler estar disponível

                    var serviceProvider = window?.Handler?.MauiContext?.Services;
                    if (serviceProvider != null)
                    {
                        var queueService = serviceProvider.GetService<IQueueService>();
                        if (queueService != null)
                        {
                            await queueService.InitializeDatabaseAsync();
                            System.Diagnostics.Debug.WriteLine("[SUCCESS] Banco inicializado em background");
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[WARNING] Erro ao inicializar banco em background: {ex.Message}");
                    // Não propaga erro - app funciona com Preferences
                }
            });

            return window;
        }
    }
}