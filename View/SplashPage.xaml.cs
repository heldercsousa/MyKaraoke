using MyKaraoke.Services;

namespace MyKaraoke.View
{
    public partial class SplashPage : ContentPage
    {
        private ServiceProvider _serviceProvider;
        private IDatabaseService _databaseService;
        private ILanguageService _languageService;
        
        public SplashPage()
        {
            InitializeComponent();
            System.Diagnostics.Debug.WriteLine("[DEBUG] SplashPage: Iniciado");
        }

        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();

            if (Handler != null)
            {
                // Inicializa o ServiceProvider quando o Handler estiver disponível
                _serviceProvider = ServiceProvider.FromPage(this);
                System.Diagnostics.Debug.WriteLine("[DEBUG] SplashPage: ServiceProvider inicializado");
                _databaseService = _serviceProvider.GetService<IDatabaseService>();
                _languageService = _serviceProvider.GetService<ILanguageService>();
            }
        }


        protected override async void OnAppearing()
        {
            base.OnAppearing();
            System.Diagnostics.Debug.WriteLine("[DEBUG] SplashPage: OnAppearing chamado");

            // Inicia o processo de carregamento
            await StartLoadingProcess();
        }

        private async Task StartLoadingProcess()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("[DEBUG] SplashPage: Iniciando processo de loading");

                // 1. Executa a inicialização do banco de dados e AGUARDA sua conclusão.
                await InitializeDatabaseAsync();

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
                    if (_languageService != null)
                    {
                        languageSelected = _languageService.IsLanguageSelected();
                        System.Diagnostics.Debug.WriteLine($"[DEBUG] Idioma selecionado via service: {languageSelected}");
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

        private async Task InitializeDatabaseAsync()
        {
            try
            {
                if (_databaseService != null)
                {
                    // A execução em uma thread de fundo previne o congelamento da UI da SplashPage.
                    await Task.Run(_databaseService.InitializeDatabaseAsync);
                    System.Diagnostics.Debug.WriteLine("[SUCCESS] Banco de dados inicializado a partir da SplashPage.");
                }
            }
            catch (Exception ex)
            {
                // Propaga o erro para ser tratado pelo método chamador.
                throw new Exception("Falha ao inicializar o banco de dados.", ex);
            }
        }
    }
}