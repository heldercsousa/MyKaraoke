using MyVocaList.Services;

namespace MyVocaList.View
{
    public partial class SplashLoadingPage : ContentPage
    {
        private bool _isNavigating = false;
        private readonly object _navigationLock = new object();

        public SplashLoadingPage()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("[SplashLoadingPage] Iniciando inicializa��o");
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

                System.Diagnostics.Debug.WriteLine("[SplashLoadingPage] OnAppearing conclu�do");
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
            // Etapas de carregamento b�sicas
            string[] loadingSteps = new string[] {
                "Verificando recursos...",
                "Carregando assemblies...",
                "Inicializando servi�os...",
                "Configurando banco de dados...",
                "Preparando interface...",
                "Finalizando inicializa��o..."
            };

            try
            {
                System.Diagnostics.Debug.WriteLine("[SplashLoadingPage] Iniciando simula��o de carregamento");

                for (int i = 0; i < loadingSteps.Length; i++)
                {
                    // Atualiza status e barra de progresso
                    double progress = (double)(i + 1) / loadingSteps.Length;
                    UpdateStatus(loadingSteps[i], progress);

                    // Simula tempo de processamento com varia��o
                    int delay = i == 0 ? 500 : (200 + (i * 50)); // Primeiro step mais longo
                    await Task.Delay(delay);

                    System.Diagnostics.Debug.WriteLine($"[SplashLoadingPage] Step {i + 1}/{loadingSteps.Length} conclu�do");
                }

                // Status final
                UpdateStatus("Inicializa��o conclu�da!", 1.0);
                await Task.Delay(300);

                // Navega para a pr�xima p�gina
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
                    System.Diagnostics.Debug.WriteLine("[SplashLoadingPage] Navega��o j� em andamento, ignorando");
                    return;
                }
                _isNavigating = true;
            }

            try
            {
                System.Diagnostics.Debug.WriteLine("[SplashLoadingPage] Iniciando navega��o para pr�xima p�gina");

                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    try
                    {
                        // Tenta navegar para SplashPage primeiro
                        var splashPage = new SplashPage();
                        Application.Current.MainPage = splashPage;
                        System.Diagnostics.Debug.WriteLine("[SplashLoadingPage] Navega��o para SplashPage realizada com sucesso");
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
                            System.Diagnostics.Debug.WriteLine($"[SplashLoadingPage] ERRO CR�TICO no fallback: {fallbackEx.Message}");

                            // �ltimo recurso - p�gina de emerg�ncia
                            Application.Current.MainPage = CreateEmergencyPage();
                            System.Diagnostics.Debug.WriteLine("[SplashLoadingPage] P�gina de emerg�ncia carregada");
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SplashLoadingPage] ERRO FATAL na navega��o: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[SplashLoadingPage] Stack trace: {ex.StackTrace}");
            }
        }

        private void CreateEmergencyUI()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("[SplashLoadingPage] Criando UI de emerg�ncia");

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
                                    Text = "MyVocaList",
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

                System.Diagnostics.Debug.WriteLine("[SplashLoadingPage] UI de emerg�ncia criada com sucesso");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SplashLoadingPage] ERRO CR�TICO na UI de emerg�ncia: {ex.Message}");
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
                            Text = "MyVocaList",
                            TextColor = Colors.White,
                            FontSize = 32,
                            FontAttributes = FontAttributes.Bold,
                            HorizontalTextAlignment = TextAlignment.Center,
                            Margin = new Thickness(0, 0, 0, 30)
                        },
                        new Label
                        {
                            Text = "Erro na inicializa��o",
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

        // Impede o bot�o voltar durante o loading
        protected override bool OnBackButtonPressed()
        {
            System.Diagnostics.Debug.WriteLine("[SplashLoadingPage] Bot�o voltar bloqueado durante carregamento");
            return true; // Bloqueia o bot�o voltar
        }
    }
}