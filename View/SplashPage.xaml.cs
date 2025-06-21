using Microsoft.Extensions.DependencyInjection;
using MyKaraoke.Services;

namespace MyKaraoke.View
{
    public partial class SplashPage : ContentPage
    {
        private ServiceProvider _serviceProvider;

        // Garanta que este é o ÚNICO construtor na classe
        public SplashPage()
        {
            InitializeComponent();
            _ = StartLoadingProcess();
        }

        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();
            
            if (Handler != null)
            {
                // Inicializa o ServiceProvider quando o Handler estiver disponível
                _serviceProvider = ServiceProvider.FromPage(this);
            }
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

                // Certifica-se que o ServiceProvider está inicializado
                if (_serviceProvider == null && Handler?.MauiContext?.Services != null)
                {
                    _serviceProvider = ServiceProvider.FromPage(this);
                }

                // Verifica se o idioma já foi selecionado (usando tanto o serviço quanto preferências)
                bool languageSelected = false;
                
                try
                {
                    var languageService = _serviceProvider.GetService<ILanguageService>();
                    languageSelected = languageService.IsLanguageSelected();
                }
                catch
                {
                    // Fallback para preferências locais se o banco de dados falhar
                    languageSelected = !string.IsNullOrEmpty(Preferences.Get("UserLanguage", string.Empty));
                }

                // Navega para a página apropriada
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    if (Application.Current != null)
                    {
                        if (languageSelected)
                        {
                            Application.Current.MainPage = new NavigationPage(new StackPage());
                            System.Diagnostics.Debug.WriteLine("Navegando para StackPage - idioma já selecionado");
                        }
                        else
                        {
                            Application.Current.MainPage = new NavigationPage(new TonguePage());
                            System.Diagnostics.Debug.WriteLine("Navegando para TonguePage - idioma não selecionado");
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao navegar: {ex.Message}");
            }
        }

        // Impede o botão voltar durante o loading
        protected override bool OnBackButtonPressed()
        {
            return true; // Bloqueia o botão voltar
        }
    }
}