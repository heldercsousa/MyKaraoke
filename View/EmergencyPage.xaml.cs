using Microsoft.Maui.Controls;

namespace MyVocaList.View
{
    public partial class EmergencyPage : ContentPage
    {
        public EmergencyPage()
        {
            InitializeComponent();
        }

        private async void OnRetryClicked(object sender, EventArgs e)
        {
            try
            {
                // Tenta recriar a SplashLoadingPage
                var splashPage = new SplashLoadingPage();
                Application.Current.MainPage = splashPage;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao tentar novamente: {ex.Message}");
                
                // Se falhar, tenta navegar para TonguePage diretamente
                try
                {
                    var tonguePage = new TonguePage();
                    Application.Current.MainPage = new NavigationPage(tonguePage);
                }
                catch (Exception ex2)
                {
                    System.Diagnostics.Debug.WriteLine($"Erro ao carregar TonguePage: {ex2.Message}");
                    
                    // Como �ltimo recurso, mostra mensagem de erro mais detalhada
                    await DisplayAlert("Erro Cr�tico", 
                        "N�o foi poss�vel inicializar o aplicativo. Por favor, reinstale o app.", 
                        "OK");
                }
            }
        }
    }
}