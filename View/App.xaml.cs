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
                
                // Inicia com página minimalista e auto-contida
                MainPage = new SplashLoadingPage();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao inicializar App: {ex.Message}");
                
                // Fallback para página de emergência
                try
                {
                    MainPage = new EmergencyPage();
                }
                catch
                {
                    // Se até a página de emergência falhar, cria uma ContentPage básica
                    MainPage = new ContentPage
                    {
                        BackgroundColor = Colors.DarkSlateBlue,
                        Content = new Label
                        {
                            Text = "MyKaraoke - Erro de Inicialização",
                            TextColor = Colors.White,
                            FontSize = 24,
                            HorizontalOptions = LayoutOptions.Center,
                            VerticalOptions = LayoutOptions.Center
                        }
                    };
                }
            }
        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            var window = base.CreateWindow(activationState);

            // Inicialização simplificada e robusta do banco de dados
            Task.Run(async () =>
            {
                try
                {
                    // Aguarda brevemente para o handler estar disponível
                    await Task.Delay(300);
                    
                    var serviceProvider = window?.Handler?.MauiContext?.Services;
                    if (serviceProvider != null)
                    {
                        var queueService = serviceProvider.GetService<IQueueService>();
                        if (queueService != null)
                        {
                            await queueService.InitializeDatabaseAsync();
                            System.Diagnostics.Debug.WriteLine("Banco de dados inicializado com sucesso");
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Erro ao inicializar banco: {ex.Message}");
                    // Não propaga exceção para não afetar a UI
                }
            });

            return window;
        }
    }
}