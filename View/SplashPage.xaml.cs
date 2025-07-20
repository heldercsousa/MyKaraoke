using MyKaraoke.Services;
using Microsoft.EntityFrameworkCore;
using MyKaraoke.Infra.Data;

namespace MyKaraoke.View
{
    public partial class SplashPage : ContentPage
    {
        private ServiceProvider _serviceProvider;
        private IDatabaseService _databaseService;
        private ILanguageService _languageService;
        private bool _isInitialized = false;

        public SplashPage()
        {
            try
            {
                InitializeComponent();
                System.Diagnostics.Debug.WriteLine("[SplashPage] Iniciado com sucesso");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SplashPage] ERRO InitializeComponent: {ex.Message}");
                throw;
            }
        }

        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();

            if (Handler != null && !_isInitialized)
            {
                try
                {
                    // Inicializa o ServiceProvider quando o Handler estiver disponível
                    _serviceProvider = ServiceProvider.FromPage(this);
                    System.Diagnostics.Debug.WriteLine("[SplashPage] ServiceProvider inicializado");

                    _databaseService = _serviceProvider.GetService<IDatabaseService>();
                    _languageService = _serviceProvider.GetService<ILanguageService>();

                    System.Diagnostics.Debug.WriteLine($"[SplashPage] DatabaseService: {(_databaseService != null ? "OK" : "NULL")}");
                    System.Diagnostics.Debug.WriteLine($"[SplashPage] LanguageService: {(_languageService != null ? "OK" : "NULL")}");

                    _isInitialized = true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[SplashPage] ERRO OnHandlerChanged: {ex.Message}");
                }
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            System.Diagnostics.Debug.WriteLine("[SplashPage] OnAppearing chamado");

            // Inicia o processo de carregamento
            await StartLoadingProcess();
        }

        private async Task StartLoadingProcess()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("[SplashPage] Iniciando processo de loading");

                // 1. Aguarda serviços estarem disponíveis
                await EnsureServicesReady();

                // 2. Executa a inicialização do banco de dados
                await InitializeDatabaseAsync();

                // 3. Mostra a imagem por um tempo (experiência visual)
                System.Diagnostics.Debug.WriteLine("[SplashPage] Aguardando 2 segundos para experiência visual");
                await Task.Delay(2000);

                // 4. Navega para próxima página
                await NavigateToNextPage();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SplashPage] ERRO StartLoadingProcess: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[SplashPage] Stack trace: {ex.StackTrace}");

                // Em caso de erro, vai direto para TonguePage como fallback
                await NavigateToNextPage();
            }
        }

        private async Task EnsureServicesReady()
        {
            int attempts = 0;
            const int maxAttempts = 10;

            while (!_isInitialized && attempts < maxAttempts)
            {
                System.Diagnostics.Debug.WriteLine($"[SplashPage] Aguardando serviços... tentativa {attempts + 1}/{maxAttempts}");
                await Task.Delay(200);
                attempts++;
            }

            if (!_isInitialized)
            {
                System.Diagnostics.Debug.WriteLine("[SplashPage] AVISO: Serviços não inicializados após timeout");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("[SplashPage] ✅ Serviços prontos para uso");
            }
        }

        private async Task InitializeDatabaseAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("[SplashPage] === INICIANDO INICIALIZAÇÃO DO BANCO ===");

                if (_databaseService == null)
                {
                    System.Diagnostics.Debug.WriteLine("[SplashPage] ⚠️ DatabaseService não disponível - tentando via contexto direto");
                    await InitializeDatabaseFallback();
                    return;
                }

                // Execução em thread de fundo para não bloquear UI
                await Task.Run(async () =>
                {
                    try
                    {
                        System.Diagnostics.Debug.WriteLine("[SplashPage] Chamando DatabaseService.InitializeDatabaseAsync()");
                        await _databaseService.InitializeDatabaseAsync();
                        System.Diagnostics.Debug.WriteLine("[SplashPage] ✅ DatabaseService.InitializeDatabaseAsync() concluído");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"[SplashPage] ❌ ERRO DatabaseService: {ex.Message}");
                        throw;
                    }
                });

                // Verifica se o banco está realmente disponível
                bool isAvailable = await _databaseService.IsDatabaseAvailableAsync();
                System.Diagnostics.Debug.WriteLine($"[SplashPage] Banco disponível após inicialização: {isAvailable}");

                if (!isAvailable)
                {
                    System.Diagnostics.Debug.WriteLine("[SplashPage] ⚠️ Banco não disponível - tentando fallback");
                    await InitializeDatabaseFallback();
                }

                System.Diagnostics.Debug.WriteLine("[SplashPage] === BANCO INICIALIZADO COM SUCESSO ===");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SplashPage] ❌ ERRO CRÍTICO na inicialização do banco: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[SplashPage] Stack trace: {ex.StackTrace}");

                // Tenta fallback antes de desistir
                System.Diagnostics.Debug.WriteLine("[SplashPage] Tentando fallback de inicialização...");
                await InitializeDatabaseFallback();
            }
        }

        private async Task InitializeDatabaseFallback()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("[SplashPage] Executando fallback de inicialização do banco");

                using var scope = MauiProgram.Services.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                System.Diagnostics.Debug.WriteLine("[SplashPage] Fallback: Executando EnsureCreated");
                await context.Database.EnsureCreatedAsync();

                System.Diagnostics.Debug.WriteLine("[SplashPage] Fallback: Testando conexão");
                bool canConnect = await context.Database.CanConnectAsync();

                if (canConnect)
                {
                    System.Diagnostics.Debug.WriteLine("[SplashPage] ✅ Fallback bem-sucedido");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("[SplashPage] ❌ Fallback falhou - banco não conecta");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SplashPage] ❌ ERRO no fallback: {ex.Message}");
                // Não propaga erro - aplicação continua sem banco se necessário
            }
        }

        private async Task NavigateToNextPage()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("[SplashPage] Navegando para próxima página");

                // Verifica se o idioma já foi selecionado
                bool languageSelected = false;

                try
                {
                    if (_languageService != null)
                    {
                        languageSelected = _languageService.IsLanguageSelected();
                        System.Diagnostics.Debug.WriteLine($"[SplashPage] Idioma selecionado via service: {languageSelected}");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[SplashPage] AVISO: Erro ao verificar idioma via service: {ex.Message}");
                    // Fallback para preferências locais
                    languageSelected = Preferences.ContainsKey("UserLanguage");
                    System.Diagnostics.Debug.WriteLine($"[SplashPage] Idioma selecionado via preferences: {languageSelected}");
                }

                // Navega para a página apropriada
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    try
                    {
                        if (languageSelected)
                        {
                            System.Diagnostics.Debug.WriteLine("[SplashPage] Navegando diretamente para StackPage (idioma já selecionado)");
                            Application.Current.MainPage = new NavigationPage(new StackPage());
                            System.Diagnostics.Debug.WriteLine("[SplashPage] ✅ Navegação para StackPage concluída");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("[SplashPage] Navegando para TonguePage (sem idioma selecionado)");
                            Application.Current.MainPage = new NavigationPage(new TonguePage());
                            System.Diagnostics.Debug.WriteLine("[SplashPage] ✅ Navegação para TonguePage concluída");
                        }
                    }
                    catch (Exception navEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"[SplashPage] ERRO na navegação MainThread: {navEx.Message}");
                        throw;
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SplashPage] ERRO ao navegar da SplashPage: {ex.Message}");

                // Fallback final para TonguePage
                try
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        Application.Current.MainPage = new NavigationPage(new TonguePage());
                        System.Diagnostics.Debug.WriteLine("[SplashPage] ✅ Navegação fallback para TonguePage concluída");
                    });
                }
                catch (Exception fallbackEx)
                {
                    System.Diagnostics.Debug.WriteLine($"[SplashPage] ❌ ERRO CRÍTICO no fallback de navegação: {fallbackEx.Message}");
                }
            }
        }

        // Impede o botão voltar durante o loading
        protected override bool OnBackButtonPressed()
        {
            System.Diagnostics.Debug.WriteLine("[SplashPage] Botão voltar bloqueado durante carregamento");
            return true; // Bloqueia o botão voltar
        }
    }
}