
namespace MyKaraoke.View
{
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();
        }

        private async void OnCreateQueueClicked(object sender, EventArgs e)
        {
            // TODO: Implementar navegação para criação de nova fila
            await DisplayAlert("Em breve", "Funcionalidade de criar nova fila será implementada em breve.", "OK");
        }

        private async void OnClosedQueuesClicked(object sender, EventArgs e)
        {
            // TODO: Implementar navegação para filas encerradas
            await DisplayAlert("Em breve", "Funcionalidade de filas encerradas será implementada em breve.", "OK");
        }

        private async void OnHelpClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Ajuda", "Aqui ficarão algumas informações úteis sobre o App.", "OK");
        }

        private async void OnSettingsClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Configurações", "O ícone tipicamente é uma 'catraca'.", "OK");
        }

        private async void OnExitButtonClicked(object sender, EventArgs e)
        {
            bool result = await DisplayAlert(
                "Sair do App",
                "Tem certeza que deseja sair do aplicativo?",
                "Sim",
                "Cancelar");

            if (result)
            {
                Application.Current?.Quit();
            }
        }

        // Intercepta o botão voltar do dispositivo (Android)
        protected override bool OnBackButtonPressed()
        {
            // Executa a mesma lógica do botão sair
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                bool result = await DisplayAlert(
                    "Sair do App",
                    "Tem certeza que deseja sair do aplicativo?",
                    "Sim",
                    "Cancelar");

                if (result)
                {
                    Application.Current?.Quit();
                }
            });

            // Retorna true para indicar que o evento foi tratado
            return true;
        }
    }
}