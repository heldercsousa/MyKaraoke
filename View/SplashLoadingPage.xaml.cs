using MyKaraoke.Services;

namespace MyKaraoke.View
{
    public partial class SplashLoadingPage : ContentPage
    {
        private bool _isNavigating = false;
        private readonly object _navigationLock = new object();

        public SplashLoadingPage()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("[SplashLoadingPage] Iniciando inicialização");
                InitializeComponent();
                System.Diagnostics.Debug.WriteLine("[SplashLoadingPage] InitializeComponent completado");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SplashLoadingPage] ERRO InitializeComponent: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[SplashLoadingPage] Stack trace: {ex.StackTrace}");
                CreateEmergencyUI();
            }
        }

        protected override void OnAppearing()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("[SplashLoadingPage] OnAppearing iniciado");
                base.OnAppearing();

                // Inicia o processo de carregamento automaticamente
                Task.Run(async () => await SimulateLoading());

                System.Diagnostics.Debug.WriteLine("[SplashLoadingPage] OnAppearing concluído");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SplashLoadingPage] ERRO OnAppearing: {ex.Message}");
            }
        }

        public void UpdateStatus(string status, double progressPercentage)
        {
            try
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    try
                    {
                        if (StatusLabel != null)
                        {
                            StatusLabel.Text = status;
                        }
                        if (LoadingProgressBar != null)
                        {
                            LoadingProgressBar.Progress = Math.Min(1.0, Math.Max(0.0, progressPercentage));
                        }
                        System.Diagnostics.Debug.WriteLine($"[SplashLoadingPage] {status} - {progressPercentage:P0}");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"[SplashLoadingPage] ERRO UpdateStatus UI: {ex.Message}");
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SplashLoadingPage] ERRO UpdateStatus: {ex.Message}");
            }
        }

        public async Task SimulateLoading()
        {
            // Etapas de carregamento básicas
            string[] loadingSteps = new string[] {
                "Verificando recursos...",
                "Carregando assemblies...",
                "Inicializando serviços...",
                "Configurando banco de dados...",
                "Preparando interface...",
                "Finalizando inicialização..."
            };

            try
            {
                System.Diagnostics.Debug.WriteLine("[SplashLoadingPage] Iniciando simulação de carregamento");

                for (int i = 0; i < loadingSteps.Length; i++)
                {
                    // Atualiza status e barra de progresso
                    double progress = (double)(i + 1) / loadingSteps.Length;
                    UpdateStatus(loadingSteps[i], progress);

                    // Simula tempo de processamento com variação
                    int delay = i == 0 ? 500 : (200 + (i * 50)); // Primeiro step mais longo
                    await Task.Delay(delay);

                    System.Diagnostics.Debug.WriteLine($"[SplashLoadingPage] Step {i + 1}/{loadingSteps.Length} concluído");
                }

                // Status final
                UpdateStatus("Inicialização concluída!", 1.0);
                await Task.Delay(300);

                // Navega para a próxima página
                await NavigateToNextPage();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SplashLoadingPage] ERRO SimulateLoading: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[SplashLoadingPage] Stack trace: {ex.StackTrace}");

                UpdateStatus("Erro detectado - continuando...", 1.0);
                await Task.Delay(1000);
                await NavigateToNextPage();
            }
        }

        private async Task NavigateToNextPage()
        {
            lock (_navigationLock)
            {
                if (_isNavigating)
                {
                    System.Diagnostics.Debug.WriteLine("[SplashLoadingPage] Navegação já em andamento, ignorando");
                    return;
                }
                _isNavigating = true;
            }

            try
            {
                System.Diagnostics.Debug.WriteLine("[SplashLoadingPage] Iniciando navegação para próxima página");

                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    try
                    {
                        // Tenta navegar para SplashPage primeiro
                        var splashPage = new SplashPage();
                        Application.Current.MainPage = splashPage;
                        System.Diagnostics.Debug.WriteLine("[SplashLoadingPage] Navegação para SplashPage realizada com sucesso");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"[SplashLoadingPage] ERRO ao navegar para SplashPage: {ex.Message}");

                        // Fallback para TonguePage se SplashPage falhar
                        try
                        {
                            var tonguePage = new TonguePage();
                            Application.Current.MainPage = new NavigationPage(tonguePage);
                            System.Diagnostics.Debug.WriteLine("[SplashLoadingPage] Fallback para TonguePage realizado");
                        }
                        catch (Exception fallbackEx)
                        {
                            System.Diagnostics.Debug.WriteLine($"[SplashLoadingPage] ERRO CRÍTICO no fallback: {fallbackEx.Message}");

                            // Último recurso - página de emergência
                            Application.Current.MainPage = CreateEmergencyPage();
                            System.Diagnostics.Debug.WriteLine("[SplashLoadingPage] Página de emergência carregada");
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SplashLoadingPage] ERRO FATAL na navegação: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[SplashLoadingPage] Stack trace: {ex.StackTrace}");
            }
        }

        private void CreateEmergencyUI()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("[SplashLoadingPage] Criando UI de emergência");

                Content = new Grid
                {
                    BackgroundColor = Color.FromHex("#221b3c"),
                    Children =
                    {
                        new StackLayout
                        {
                            VerticalOptions = LayoutOptions.Center,
                            HorizontalOptions = LayoutOptions.Center,
                            Children =
                            {
                                new Label
                                {
                                    Text = "MyKaraoke",
                                    TextColor = Colors.White,
                                    FontSize = 28,
                                    FontAttributes = FontAttributes.Bold,
                                    HorizontalTextAlignment = TextAlignment.Center,
                                    Margin = new Thickness(0, 0, 0, 20)
                                },
                                new ActivityIndicator
                                {
                                    IsRunning = true,
                                    Color = Color.FromHex("#e91e63"),
                                    HeightRequest = 40,
                                    WidthRequest = 40
                                },
                                new Label
                                {
                                    Text = "Carregando...",
                                    TextColor = Color.FromHex("#b0a8c7"),
                                    FontSize = 16,
                                    HorizontalTextAlignment = TextAlignment.Center,
                                    Margin = new Thickness(0, 20, 0, 0)
                                }
                            }
                        }
                    }
                };

                System.Diagnostics.Debug.WriteLine("[SplashLoadingPage] UI de emergência criada com sucesso");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SplashLoadingPage] ERRO CRÍTICO na UI de emergência: {ex.Message}");
            }
        }

        private ContentPage CreateEmergencyPage()
        {
            return new ContentPage
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
                        new Label
                        {
                            Text = "Erro na inicialização",
                            TextColor = Color.FromHex("#ff6b6b"),
                            FontSize = 18,
                            HorizontalTextAlignment = TextAlignment.Center,
                            Margin = new Thickness(0, 0, 0, 20)
                        },
                        new Button
                        {
                            Text = "Tentar Novamente",
                            BackgroundColor = Color.FromHex("#e91e63"),
                            TextColor = Colors.White,
                            CornerRadius = 8,
                            Command = new Command(async () =>
                            {
                                try
                                {
                                    Application.Current.MainPage = new SplashLoadingPage();
                                }
                                catch
                                {
                                    // Se mesmo isso falhar, pelo menos tenta TonguePage
                                    Application.Current.MainPage = new NavigationPage(new TonguePage());
                                }
                            })
                        }
                    }
                }
            };
        }

        // Impede o botão voltar durante o loading
        protected override bool OnBackButtonPressed()
        {
            System.Diagnostics.Debug.WriteLine("[SplashLoadingPage] Botão voltar bloqueado durante carregamento");
            return true; // Bloqueia o botão voltar
        }
    }
}