using MyVocaList.Services;
using Serilog;

namespace MyVocaList.View
{
    public partial class SplashLoadingPage : ContentPage
    {
        private static readonly Serilog.ILogger Logger = Log.ForContext<SplashLoadingPage>();
        private bool _isNavigating = false;
        private readonly object _navigationLock = new object();

        public SplashLoadingPage()
        {
            try
            {
                Logger.Information("Starting initialization");
                InitializeComponent();
                Logger.Information("InitializeComponent completed");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error in InitializeComponent");
                CreateEmergencyUI();
            }
        }

        protected override void OnAppearing()
        {
            try
            {
                Logger.Information("OnAppearing started");
                base.OnAppearing();

                Task.Run(async () => await SimulateLoading());

                Logger.Information("OnAppearing completed");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error in OnAppearing");
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
                        Logger.Debug("{Status} - {Progress:P0}", status, progressPercentage);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "Error updating status UI");
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error in UpdateStatus");
            }
        }

        public async Task SimulateLoading()
        {
            string[] loadingSteps = new string[] {
                "Checking resources...",
                "Loading assemblies...",
                "Initializing services...",
                "Configuring database...",
                "Preparing interface...",
                "Finalizing initialization..."
            };

            try
            {
                Logger.Information("Starting loading simulation");

                for (int i = 0; i < loadingSteps.Length; i++)
                {
                    double progress = (double)(i + 1) / loadingSteps.Length;
                    UpdateStatus(loadingSteps[i], progress);

                    int delay = i == 0 ? 500 : (200 + (i * 50));
                    await Task.Delay(delay);

                    Logger.Debug("Step {Step}/{Total} completed", i + 1, loadingSteps.Length);
                }

                UpdateStatus("Initialization completed!", 1.0);
                await Task.Delay(300);

                await NavigateToNextPage();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error in SimulateLoading");

                UpdateStatus("Error detected - continuing...", 1.0);
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
                    Logger.Debug("Navigation already in progress, ignoring");
                    return;
                }
                _isNavigating = true;
            }

            try
            {
                Logger.Information("Starting navigation to next page");

                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    try
                    {
                        var splashPage = new SplashPage();
                        Application.Current.MainPage = splashPage;
                        Logger.Information("Navigation to SplashPage completed successfully");
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "Error navigating to SplashPage");

                        try
                        {
                            var tonguePage = new TonguePage();
                            Application.Current.MainPage = new NavigationPage(tonguePage);
                            Logger.Information("Fallback to TonguePage completed");
                        }
                        catch (Exception fallbackEx)
                        {
                            Logger.Fatal(fallbackEx, "Critical error in fallback");

                            Application.Current.MainPage = CreateEmergencyPage();
                            Logger.Information("Emergency page loaded");
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex, "Fatal error in navigation");
            }
        }

        private void CreateEmergencyUI()
        {
            try
            {
                Logger.Information("Creating emergency UI");

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
                                    Text = "Loading...",
                                    TextColor = Color.FromHex("#b0a8c7"),
                                    FontSize = 16,
                                    HorizontalTextAlignment = TextAlignment.Center,
                                    Margin = new Thickness(0, 20, 0, 0)
                                }
                            }
                        }
                    }
                };

                Logger.Information("Emergency UI created successfully");
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex, "Critical error in emergency UI");
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
                            Text = "Initialization error",
                            TextColor = Color.FromHex("#ff6b6b"),
                            FontSize = 18,
                            HorizontalTextAlignment = TextAlignment.Center,
                            Margin = new Thickness(0, 0, 0, 20)
                        },
                        new Button
                        {
                            Text = "Try Again",
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
                                    Application.Current.MainPage = new NavigationPage(new TonguePage());
                                }
                            })
                        }
                    }
                }
            };
        }

        protected override bool OnBackButtonPressed()
        {
            Logger.Debug("Back button blocked during loading");
            return true;
        }
    }
}