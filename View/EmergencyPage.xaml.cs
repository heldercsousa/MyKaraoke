using Microsoft.Maui.Controls;
using Serilog;

namespace MyVocaList.View
{
    public partial class EmergencyPage : ContentPage
    {
        private static readonly Serilog.ILogger Logger = Log.ForContext<EmergencyPage>();
        public EmergencyPage()
        {
            InitializeComponent();
        }

        private async void OnRetryClicked(object sender, EventArgs e)
        {
            try
            {
                var splashPage = new SplashLoadingPage();
                Application.Current.MainPage = splashPage;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error trying again");

                try
                {
                    var tonguePage = new TonguePage();
                    Application.Current.MainPage = new NavigationPage(tonguePage);
                }
                catch (Exception ex2)
                {
                    Logger.Fatal(ex2, "Error loading TonguePage");

                    await DisplayAlert("Critical Error",
                        "Could not initialize the application. Please reinstall the app.",
                        "OK");
                }
            }
        }
    }
}