using MyKaraoke.Services;

namespace MyKaraoke.View
{
    public partial class App : Application
    {
        private static bool _isInitialized = false;
        private readonly object _initLock = new object();

        public App()
        {
            lock (_initLock)
            {
                if (_isInitialized)
                {
                    System.Diagnostics.Debug.WriteLine("[App] App já foi inicializada, ignorando");
                    return;
                }
                _isInitialized = true;
            }

            try
            {
                System.Diagnostics.Debug.WriteLine("[App] === INICIANDO APLICAÇÃO MYKARAOKE ===");

                // Configurações de ambiente antes da inicialização
                ConfigureEnvironment();

                // Inicializa componentes XAML
                System.Diagnostics.Debug.WriteLine("[App] Inicializando componentes XAML...");
                InitializeComponent();

                // Inicializa serviços essenciais
                InitializeEssentialServices();

                // Define a página inicial
                SetInitialPage();

                System.Diagnostics.Debug.WriteLine("[App] === APLICAÇÃO INICIADA COM SUCESSO ===");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[App] ERRO CRÍTICO na inicialização: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[App] Stack trace: {ex.StackTrace}");
                CreateFallbackPage();
            }
        }

        private void ConfigureEnvironment()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("[App] Configurando ambiente de execução...");

                // Configurações de GC para reduzir problemas de memória
                System.Environment.SetEnvironmentVariable("MONO_GC_PARAMS", "major=marksweep-conc,nursery-size=8m");
                System.Environment.SetEnvironmentVariable("MONO_THREADS_PER_CPU", "4");

                // Configurações para reduzir "failed to load assembly"
                System.Environment.SetEnvironmentVariable("MONO_LOG_LEVEL", "info");
                System.Environment.SetEnvironmentVariable("MONO_LOG_MASK", "asm");

                // Configurações para assemblies
                System.Environment.SetEnvironmentVariable("MONO_DEBUG", "disable_omit_fp");

                // Força coleta de lixo inicial
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();

                System.Diagnostics.Debug.WriteLine("[App] Ambiente configurado com sucesso");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[App] ERRO ao configurar ambiente: {ex.Message}");
            }
        }

        private void InitializeEssentialServices()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("[App] Inicializando serviços essenciais...");

                // Verifica se o ServiceProvider está disponível
                if (MauiProgram.Services != null)
                {
                    System.Diagnostics.Debug.WriteLine("[App] ServiceProvider disponível");

                    // Inicializa serviços críticos de forma proativa
                    try
                    {
                        var languageService = MauiProgram.Services.GetService<ILanguageService>();
                        if (languageService != null)
                        {
                            System.Diagnostics.Debug.WriteLine("[App] LanguageService inicializado");
                        }
                    }
                    catch (Exception serviceEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"[App] AVISO: Erro ao inicializar serviços: {serviceEx.Message}");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("[App] AVISO: ServiceProvider não disponível ainda");
                }

                System.Diagnostics.Debug.WriteLine("[App] Serviços essenciais inicializados");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[App] ERRO ao inicializar serviços: {ex.Message}");
            }
        }

        private void SetInitialPage()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("[App] Definindo página inicial...");

                // Tenta carregar SplashLoadingPage primeiro
                var splashLoadingPage = new SplashLoadingPage();
                MainPage = splashLoadingPage;

                System.Diagnostics.Debug.WriteLine("[App] SplashLoadingPage definida como página inicial");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[App] ERRO ao definir página inicial: {ex.Message}");

                // Fallback para página de emergência
                CreateFallbackPage();
            }
        }

        private void CreateFallbackPage()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("[App] Criando página de fallback...");

                MainPage = new ContentPage
                {
                    BackgroundColor = Color.FromHex("#221b3c"),
                    Content = new StackLayout
                    {
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center,
                        Padding = new Thickness(40),
                        Children =
                        {
                            new Label
                            {
                                Text = "MyKaraoke",
                                TextColor = Colors.White,
                                FontSize = 32,
                                FontAttributes = FontAttributes.Bold,
                                HorizontalTextAlignment = TextAlignment.Center,
                                Margin = new Thickness(0, 0, 0, 30)
                            },
                            new ActivityIndicator
                            {
                                IsRunning = true,
                                Color = Color.FromHex("#e91e63"),
                                HeightRequest = 50,
                                WidthRequest = 50,
                                Margin = new Thickness(0, 0, 0, 20)
                            },
                            new Label
                            {
                                Text = "Inicializando aplicativo...\nPor favor aguarde.",
                                TextColor = Color.FromHex("#b0a8c7"),
                                FontSize = 16,
                                HorizontalTextAlignment = TextAlignment.Center,
                                Margin = new Thickness(0, 0, 0, 30)
                            },
                            new Button
                            {
                                Text = "Continuar",
                                BackgroundColor = Color.FromHex("#e91e63"),
                                TextColor = Colors.White,
                                CornerRadius = 8,
                                FontSize = 16,
                                Padding = new Thickness(20, 12),
                                Command = new Command(OnContinueClicked)
                            }
                        }
                    }
                };

                System.Diagnostics.Debug.WriteLine("[App] Página de fallback criada com sucesso");
            }
            catch (Exception criticalEx)
            {
                System.Diagnostics.Debug.WriteLine($"[App] ERRO CRÍTICO no fallback: {criticalEx.Message}");

                // Último recurso - página super simples
                try
                {
                    MainPage = new ContentPage
                    {
                        BackgroundColor = Color.FromHex("#221b3c"),
                        Content = new Label
                        {
                            Text = "MyKaraoke\nInicializando...",
                            TextColor = Colors.White,
                            FontSize = 24,
                            HorizontalOptions = LayoutOptions.Center,
                            VerticalOptions = LayoutOptions.Center,
                            HorizontalTextAlignment = TextAlignment.Center
                        }
                    };

                    System.Diagnostics.Debug.WriteLine("[App] Página de emergência simples criada");
                }
                catch (Exception ultimateEx)
                {
                    System.Diagnostics.Debug.WriteLine($"[App] FALHA TOTAL: {ultimateEx.Message}");
                }
            }
        }

        private async void OnContinueClicked()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("[App] Botão Continuar pressionado");

                // Tenta navegar para a aplicação principal
                await AttemptMainNavigation();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[App] ERRO OnContinueClicked: {ex.Message}");
            }
        }

        private async Task AttemptMainNavigation()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("[App] Tentando navegação principal...");

                // Aguarda um pouco para garantir que tudo esteja carregado
                await Task.Delay(500);

                // Tenta diferentes opções de navegação
                if (await TryNavigateToSplashPage())
                {
                    System.Diagnostics.Debug.WriteLine("[App] Navegação para SplashPage bem-sucedida");
                    return;
                }

                if (await TryNavigateToTonguePage())
                {
                    System.Diagnostics.Debug.WriteLine("[App] Navegação para TonguePage bem-sucedida");
                    return;
                }

                System.Diagnostics.Debug.WriteLine("[App] AVISO: Todas as tentativas de navegação falharam");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[App] ERRO AttemptMainNavigation: {ex.Message}");
            }
        }

        private async Task<bool> TryNavigateToSplashPage()
        {
            try
            {
                var splashPage = new SplashPage();
                MainPage = splashPage;
                await Task.Delay(100); // Pequena pausa para verificar se não crashou
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[App] Falha ao navegar para SplashPage: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> TryNavigateToTonguePage()
        {
            try
            {
                var tonguePage = new TonguePage();
                MainPage = new NavigationPage(tonguePage);
                await Task.Delay(100); // Pequena pausa para verificar se não crashou
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[App] Falha ao navegar para TonguePage: {ex.Message}");
                return false;
            }
        }

        protected override void OnStart()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("[App] OnStart chamado");
                base.OnStart();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[App] ERRO OnStart: {ex.Message}");
            }
        }

        protected override void OnSleep()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("[App] OnSleep chamado - aplicação entrando em segundo plano");
                base.OnSleep();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[App] ERRO OnSleep: {ex.Message}");
            }
        }

        protected override void OnResume()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("[App] OnResume chamado - aplicação retornando");
                base.OnResume();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[App] ERRO OnResume: {ex.Message}");
            }
        }
    }
}