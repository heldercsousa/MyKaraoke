// ####################################################################################################
// # Arquivo: GaleraNaFila/App.xaml.cs
// # Descrição: Lógica de inicialização da aplicação.
// #            ATUALIZADO: Agora sempre inicia na SplashPage (tela de loading personalizada).
// ####################################################################################################
using GaleraNaFila.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;

namespace GaleraNaFila
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // MUDANÇA: Sempre inicia na SplashPage (tela de loading personalizada)
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
                        var queueService = serviceProvider.GetRequiredService<QueueService>();
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