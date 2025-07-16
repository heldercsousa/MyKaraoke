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