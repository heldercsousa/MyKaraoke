using MyKaraoke.Services;

namespace MyKaraoke.View
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Inicia com a página de carregamento
            MainPage = new SplashLoadingPage();
        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            var window = base.CreateWindow(activationState);

            // Inicialização do banco de dados de forma assíncrona
            Task.Run(async () =>
            {
                try
                {
                    // Aguarda um momento para o handler ser associado completamente
                    await Task.Delay(1000);
                    
                    var serviceProvider = window?.Handler?.MauiContext?.Services;
                    if (serviceProvider != null)
                    {
                        var queueService = serviceProvider.GetService<IQueueService>();
                        if (queueService != null)
                        {
                            await queueService.InitializeDatabaseAsync();
                        }

                        // Atualiza a página principal com o serviço ou cria uma nova instância
                        MainPage = serviceProvider.GetService<SplashLoadingPage>() ?? new SplashLoadingPage();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Erro ao inicializar banco: {ex.Message}");
                    // Não lança exceção para não interromper o fluxo principal
                }
            });

            return window;
        }
    }
}