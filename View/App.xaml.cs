using MyKaraoke.Services;

namespace MyKaraoke.View
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Sempre começa pela SplashPage
            MainPage = new SplashPage();
        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            var window = base.CreateWindow(activationState);

            // Inicialização do banco de dados de forma assíncrona
            Task.Run(async () =>
            {
                try
                {
                    var serviceProvider = Handler?.MauiContext?.Services;
                    if (serviceProvider != null)
                    {
                        var queueService = serviceProvider.GetRequiredService<IQueueService>();
                        await queueService.InitializeDatabaseAsync();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Erro ao inicializar banco: {ex.Message}");
                }
            });

            return window;
        }
    }
}