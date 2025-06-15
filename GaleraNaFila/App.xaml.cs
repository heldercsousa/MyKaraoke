// ####################################################################################################
// # Arquivo: GaleraNaFila/App.xaml.cs
// # Descrição: Lógica de inicialização da aplicação.
// #            ATUALIZADO: Construtor de App recebe IServiceProvider e define MainPage.
// ####################################################################################################
using GaleraNaFila.Services; // Ajuste o using para o namespace do seu QueueService
using Microsoft.Extensions.DependencyInjection; // Necessário para IServiceProvider e GetRequiredService
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace GaleraNaFila // Verifique e ajuste o namespace para o nome do seu projeto raiz
{
    public partial class App : Application
    {
        // AGORA O CONSTRUTOR DE APP RECEBE O SERVICEPROVIDER DIRETAMENTE
        public App(IServiceProvider serviceProvider) // <--- NOVO: Construtor com parâmetro
        {
            InitializeComponent();

            // A lógica de definir MainPage e inicializar o BD agora vai aqui, no construtor.
            bool isAdminMode = Preferences.Get("IsAdminMode", false);

            if (isAdminMode)
            {
                MainPage = new NavigationPage(serviceProvider.GetRequiredService<AdminPage>());
            }
            else
            {
                MainPage = new NavigationPage(serviceProvider.GetRequiredService<MainPage>());
            }

            // Inicialização do banco de dados também usa o serviceProvider injetado
            Task.Run(async () =>
            {
                var queueService = serviceProvider.GetRequiredService<QueueService>();
                await queueService.InitializeDatabaseAsync();
            });
        }

        // REMOVA O MÉTODO InitializeApp() COMPLETAMENTE. Ele não é mais necessário.
        /*
        public void InitializeApp(IServiceProvider serviceProvider)
        {
            // ... código antigo ...
        }
        */
    }
}