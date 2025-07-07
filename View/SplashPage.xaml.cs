using MyKaraoke.Services;

namespace MyKaraoke.View
{
    public partial class SplashPage : ContentPage
    {
        private ServiceProvider _serviceProvider;

        public SplashPage()
        {
            InitializeComponent();
            System.Diagnostics.Debug.WriteLine("[DEBUG] SplashPage: Iniciado");
            _ = StartLoadingProcess();
        }

        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();

            if (Handler != null)
            {
                // Inicializa o ServiceProvider quando o Handler estiver disponível
                _serviceProvider = ServiceProvider.FromPage(this);
                System.Diagnostics.Debug.WriteLine("[DEBUG] SplashPage: ServiceProvider inicializado");
            }
        }

        private async Task StartLoadingProcess()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("[DEBUG] SplashPage: Iniciando processo de loading");

                // Aguarda um pouco para garantir que a página foi carregada
                await Task.Delay(500);

                // Mostra a imagem por um tempo (experiência visual)
                await Task.Delay(2000);

                // Navega para TonguePage
                await NavigateToTonguePage();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR] SplashPage: {ex.Message}");
                // Em caso de erro, vai direto para TonguePage
                await NavigateToTonguePage();
            }
        }

        private async Task NavigateToTonguePage()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("[DEBUG] SplashPage: Navegando para TonguePage");

                // Verifica se o idioma já foi selecionado
                bool languageSelected = false;

                try
                {
                    // Certifica-se que o ServiceProvider está inicializado
                    if (_serviceProvider == null && Handler?.MauiContext?.Services != null)
                    {
                        _serviceProvider = ServiceProvider.FromPage(this);
                    }

                    if (_serviceProvider != null)
                    {
                        var languageService = _serviceProvider.GetService<ILanguageService>();
                        if (languageService != null)
                        {
                            languageSelected = languageService.IsLanguageSelected();
                            System.Diagnostics.Debug.WriteLine($"[DEBUG] Idioma selecionado via service: {languageSelected}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[WARNING] Erro ao verificar idioma via service: {ex.Message}");
                    // Fallback para preferências locais
                    languageSelected = Preferences.ContainsKey("UserLanguage");
                    System.Diagnostics.Debug.WriteLine($"[DEBUG] Idioma selecionado via preferences: {languageSelected}");
                }

                // Navega para a página apropriada
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    if (languageSelected)
                    {
                        System.Diagnostics.Debug.WriteLine("[DEBUG] Navegando diretamente para StackPage (idioma já selecionado)");
                        Application.Current.MainPage = new NavigationPage(new StackPage());
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("[DEBUG] Navegando para TonguePage (sem idioma selecionado)");
                        Application.Current.MainPage = new NavigationPage(new TonguePage());
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR] Erro ao navegar da SplashPage: {ex.Message}");

                // Fallback final para TonguePage
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Application.Current.MainPage = new NavigationPage(new TonguePage());
                });
            }
        }

        // Impede o botão voltar durante o loading
        protected override bool OnBackButtonPressed()
        {
            return true; // Bloqueia o botão voltar
        }
    }
}