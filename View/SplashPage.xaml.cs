using MyVocaList.Services;
using Microsoft.EntityFrameworkCore;
using MyVocaList.Infra.Data;
using Serilog;

namespace MyVocaList.View
{
    public partial class SplashPage : ContentPage
    {
        private static readonly Serilog.ILogger Logger = Log.ForContext<SplashPage>();
        private ServiceProvider _serviceProvider;
        private IDatabaseService _databaseService;
        private ILanguageService _languageService;
        private bool _isInitialized = false;

        public SplashPage()
        {
            try
            {
                InitializeComponent();
                Logger.Information("Started successfully");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error in InitializeComponent");
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
                    _serviceProvider = ServiceProvider.FromPage(this);
                    Logger.Information("ServiceProvider initialized");

                    _databaseService = _serviceProvider.GetService<IDatabaseService>();
                    _languageService = _serviceProvider.GetService<ILanguageService>();

                    Logger.Information("DatabaseService: {Status}", _databaseService != null ? "OK" : "NULL");
                    Logger.Information("LanguageService: {Status}", _languageService != null ? "OK" : "NULL");

                    _isInitialized = true;
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Error in OnHandlerChanged");
                }
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            Logger.Information("OnAppearing called");
            await StartLoadingProcess();
        }

        private async Task StartLoadingProcess()
        {
            try
            {
                Logger.Information("Starting loading process");

                await EnsureServicesReady();
                await InitializeDatabaseAsync();

                Logger.Debug("Waiting 2 seconds for visual experience");
                await Task.Delay(2000);

                await NavigateToNextPage();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error in StartLoadingProcess");
                await NavigateToNextPage();
            }
        }

        private async Task EnsureServicesReady()
        {
            int attempts = 0;
            const int maxAttempts = 10;

            while (!_isInitialized && attempts < maxAttempts)
            {
                Logger.Debug("Waiting for services... attempt {Attempt}/{MaxAttempts}", attempts + 1, maxAttempts);
                await Task.Delay(200);
                attempts++;
            }

            if (!_isInitialized)
            {
                Logger.Warning("Services not initialized after timeout");
            }
            else
            {
                Logger.Information("Services ready for use");
            }
        }

        private async Task InitializeDatabaseAsync()
        {
            try
            {
                Logger.Information("Starting database initialization");

                if (_databaseService == null)
                {
                    Logger.Warning("DatabaseService not available - trying direct context");
                    await InitializeDatabaseFallback();
                    return;
                }

                await Task.Run(async () =>
                {
                    Logger.Debug("Calling DatabaseService.InitializeDatabaseAsync()");
                    await _databaseService.InitializeDatabaseAsync();
                    Logger.Information("DatabaseService.InitializeDatabaseAsync() completed");
                });

                bool isAvailable = await _databaseService.IsDatabaseAvailableAsync();
                Logger.Information("Database available after initialization: {IsAvailable}", isAvailable);

                if (!isAvailable)
                {
                    Logger.Warning("Database not available - trying fallback");
                    await InitializeDatabaseFallback();
                }

                Logger.Information("Database initialized successfully");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Critical error in database initialization");
                Logger.Information("Trying initialization fallback");
                await InitializeDatabaseFallback();
            }
        }

        private async Task InitializeDatabaseFallback()
        {
            try
            {
                Logger.Information("Executing database initialization fallback");

                using var scope = MauiProgram.Services.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                Logger.Debug("Fallback: Executing EnsureCreated");
                await context.Database.EnsureCreatedAsync();

                Logger.Debug("Fallback: Testing connection");
                bool canConnect = await context.Database.CanConnectAsync();

                if (canConnect)
                {
                    Logger.Information("Fallback successful");
                }
                else
                {
                    Logger.Error("Fallback failed - database cannot connect");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error in fallback");
            }
        }

        private async Task NavigateToNextPage()
        {
            try
            {
                Logger.Information("Navigating to next page");

                bool languageSelected = false;

                try
                {
                    if (_languageService != null)
                    {
                        languageSelected = _languageService.IsLanguageSelected();
                        Logger.Information("Language selected via service: {LanguageSelected}", languageSelected);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warning(ex, "Error checking language via service");
                    languageSelected = Preferences.ContainsKey("UserLanguage");
                    Logger.Information("Language selected via preferences: {LanguageSelected}", languageSelected);
                }

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    if (languageSelected)
                    {
                        Logger.Information("Navigating directly to StackPage (language already selected)");
                        Application.Current.MainPage = new NavigationPage(new StackPage());
                        Logger.Information("Navigation to StackPage completed");
                    }
                    else
                    {
                        Logger.Information("Navigating to TonguePage (no language selected)");
                        Application.Current.MainPage = new NavigationPage(new TonguePage());
                        Logger.Information("Navigation to TonguePage completed");
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error navigating from SplashPage");

                try
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        Application.Current.MainPage = new NavigationPage(new TonguePage());
                        Logger.Information("Fallback navigation to TonguePage completed");
                    });
                }
                catch (Exception fallbackEx)
                {
                    Logger.Fatal(fallbackEx, "Critical error in navigation fallback");
                }
            }
        }

        protected override bool OnBackButtonPressed()
        {
            Logger.Debug("Back button blocked during loading");
            return true;
        }
    }
}