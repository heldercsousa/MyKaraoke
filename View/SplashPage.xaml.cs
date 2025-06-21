namespace MyKaraoke.View
{
    public partial class SplashPage : ContentPage
    {
        public SplashPage()
        {
            InitializeComponent();
            _ = StartLoadingProcess();
        }

        private async Task StartLoadingProcess()
        {
            try
            {
                // Aguarda um pouco para garantir que a página foi carregada
                await Task.Delay(500);

                // Animação simples de fade in do logo
                var logoElement = this.FindByName<Border>("logoFrame");
                if (logoElement != null)
                {
                    logoElement.Opacity = 0;
                    await logoElement.FadeTo(1, 1000);
                }

                // Simula carregamento por 2.5 segundos
                await Task.Delay(2500);

                // Navega para a HomePage
                await NavigateToHomePage();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro na SplashPage: {ex.Message}");
                // Em caso de erro, vai direto para a HomePage
                await NavigateToHomePage();
            }
        }

        private async Task NavigateToHomePage()
        {
            try
            {
                // Para a animação de loading
                var loadingElement = this.FindByName<ActivityIndicator>("loadingIndicator");
                if (loadingElement != null)
                {
                    loadingElement.IsRunning = false;
                }

                // Navega para a HomePage
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    if (Application.Current != null)
                    {
                        Application.Current.PersonPage = new NavigationPage(new HomePage());
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao navegar para HomePage: {ex.Message}");
            }
        }

        // Impede o botão voltar durante o loading
        protected override bool OnBackButtonPressed()
        {
            return true; // Bloqueia o botão voltar
        }
    }
}