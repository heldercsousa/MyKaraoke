using MyVocaList.Services;
using MyVocaList.View.Interceptors;
using Serilog;

namespace MyVocaList.View
{
    public partial class App : Application
    {
        private static readonly Serilog.ILogger Logger = Log.ForContext<App>();

        private static bool _isInitialized = false;
        private readonly object _initLock = new object();

        public App()
        {
            lock (_initLock)
            {
                if (_isInitialized)
                {
                    Logger.Warning("App already initialized, ignoring");
                    return;
                }
                _isInitialized = true;
            }

            try
            {
                Logger.Information("=== STARTING MyVocaList APPLICATION ===");

                // Configurações de ambiente antes da inicialização
                ConfigureEnvironment();

                // Inicializa componentes XAML
                Logger.Debug("Initializing XAML components");
                InitializeComponent();

                // Inicializa serviços essenciais
                InitializeEssentialServices();

                // Define a página inicial
                SetInitialPage();

                Logger.Information("=== APPLICATION STARTED SUCCESSFULLY ===");
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex, "CRITICAL ERROR during initialization");
                CreateFallbackPage();
            }
        }

        private void ConfigureEnvironment()
        {
            try
            {
                Logger.Debug("Configuring runtime environment");

                // Configurações de GC para reduzir problemas de memória
                System.Environment.SetEnvironmentVariable("MONO_GC_PARAMS", "major=marksweep-conc,nursery-size=8m");
                System.Environment.SetEnvironmentVariable("MONO_THREADS_PER_CPU", "4");

                // Configurações para reduzir "failed to load assembly"
                System.Environment.SetEnvironmentVariable("MONO_LOG_LEVEL", "info");
                System.Environment.SetEnvironmentVariable("MONO_LOG_MASK", "asm");

                // Configurações para assemblies
                System.Environment.SetEnvironmentVariable("MONO_DEBUG", "disable_omit_fp");

                // Força coleta de lixo inicial
                // System.GC.Collect();
                // System.GC.WaitForPendingFinalizers();

                Logger.Debug("Environment configured successfully");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error configuring environment");
            }
        }

        private void InitializeEssentialServices()
        {
            try
            {
                Logger.Debug("Initializing essential services");

                // Verifica se o ServiceProvider está disponível
                if (MauiProgram.Services != null)
                {
                    Logger.Debug("ServiceProvider available");

                    // Inicializa serviços críticos de forma proativa
                    try
                    {
                        var languageService = MauiProgram.Services.GetService<ILanguageService>();
                        if (languageService != null)
                        {
                            Logger.Debug("LanguageService initialized");
                        }
                    }
                    catch (Exception serviceEx)
                    {
                        Logger.Warning(serviceEx, "Error initializing services");
                    }
                }
                else
                {
                    Logger.Warning("ServiceProvider not available yet");
                }

                Logger.Debug("Essential services initialized");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error initializing services");
            }
        }

        private void SetInitialPage()
        {
            try
            {
                Logger.Debug("Setting initial page");

                // Tenta carregar SplashLoadingPage primeiro
                var splashLoadingPage = new SplashLoadingPage();
                MainPage = splashLoadingPage;

                Logger.Debug("SplashLoadingPage set as initial page");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error setting initial page");

                // Fallback para página de emergência
                CreateFallbackPage();
            }
        }

        private void CreateFallbackPage()
        {
            try
            {
                Logger.Warning("Creating fallback page");

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
                                Text = "MyVocaList",
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

                Logger.Debug("Fallback page created successfully");
            }
            catch (Exception criticalEx)
            {
                Logger.Fatal(criticalEx, "CRITICAL ERROR in fallback");

                // Último recurso - página super simples
                try
                {
                    MainPage = new ContentPage
                    {
                        BackgroundColor = Color.FromHex("#221b3c"),
                        Content = new Label
                        {
                            Text = "MyVocaList\nInicializando...",
                            TextColor = Colors.White,
                            FontSize = 24,
                            HorizontalOptions = LayoutOptions.Center,
                            VerticalOptions = LayoutOptions.Center,
                            HorizontalTextAlignment = TextAlignment.Center
                        }
                    };

                    Logger.Debug("Simple emergency page created");
                }
                catch (Exception ultimateEx)
                {
                    Logger.Fatal(ultimateEx, "TOTAL FAILURE");
                }
            }
        }

        private async void OnContinueClicked()
        {
            try
            {
                Logger.Debug("Continue button pressed");

                // Tenta navegar para a aplicação principal
                await AttemptMainNavigation();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error in OnContinueClicked");
            }
        }

        private async Task AttemptMainNavigation()
        {
            try
            {
                Logger.Debug("Attempting main navigation");

                // Aguarda um pouco para garantir que tudo esteja carregado
                await Task.Delay(500);

                // Tenta diferentes opções de navegação
                if (await TryNavigateToSplashPage())
                {
                    Logger.Debug("Navigation to SplashPage successful");
                    return;
                }

                if (await TryNavigateToTonguePage())
                {
                    Logger.Debug("Navigation to TonguePage successful");
                    return;
                }

                Logger.Warning("All navigation attempts failed");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error in AttemptMainNavigation");
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
                Logger.Warning(ex, "Failed to navigate to SplashPage");
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
                Logger.Warning(ex, "Failed to navigate to TonguePage");
                return false;
            }
        }

        protected override void OnStart()
        {
            try
            {
                Logger.Debug("OnStart called");
                base.OnStart();

                //// 🔄 AGORA: Inicializa interceptors com app rodando
                Task.Run(async () =>
                {
                    await Task.Delay(2000); // Aguarda app estabilizar
                    NavigationLoadingInterceptor.Initialize();
                });
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error in OnStart");
            }
        }

        protected override void OnSleep()
        {
            try
            {
                Logger.Debug("OnSleep called - application going to background");
                base.OnSleep();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error in OnSleep");
            }
        }

        protected override void OnResume()
        {
            try
            {
                Logger.Debug("OnResume called - application returning");
                base.OnResume();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error in OnResume");
            }
        }
    }
}