using Microsoft.Maui.Controls;
using MyVocaList.Contracts.Models;
using MyVocaList.Services;
using MyVocaList.View.Components;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Serilog;

namespace MyVocaList.View
{
    public partial class TonguePage : ContentPage
    {
        private static readonly Serilog.ILogger Logger = Log.ForContext<TonguePage>();

        public ObservableCollection<LanguageItem> Languages { get; private set; }

        private ILanguageService? _languageService;
        private ServiceProvider? _serviceProvider;
        private string selectedLanguage = "en"; // Default language
        private bool _isInitialized = false;

        // Lifecycle Command
        public ICommand LoadDataCommand { get; }

        private LanguageItem _selectedLanguageItem;
        public LanguageItem SelectedLanguageItem
        {
            get => _selectedLanguageItem;
            set
            {
                if (_selectedLanguageItem != value)
                {
                    _selectedLanguageItem = value;
                    OnPropertyChanged(nameof(SelectedLanguageItem));
                }
            }
        }

        public ICommand LanguageSelectedCommand { get; }

        public TonguePage()
        {
            LoadDataCommand = new Command(async () => await InitializeDataAsync());
            LanguageSelectedCommand = new Command<LanguageItem>(async (item) => await OnLanguageSelectedAsync(item));
            
            InitializeComponent();
            this.BindingContext = this;

            // Initialize the list of languages - ONLY 6 SUPPORTED LANGUAGES
            Languages = new ObservableCollection<LanguageItem>
            {
                new LanguageItem { Code = "en", Name = "English", Countries = "United States / United Kingdom", Flag = "🇺🇸 🇬🇧", IsSelected = true },
                new LanguageItem { Code = "pt", Name = "Português", Countries = "Brasil / Portugal", Flag = "🇧🇷 🇵🇹" },
                new LanguageItem { Code = "es", Name = "Español", Countries = "España / América Latina", Flag = "🇪🇸 🇲🇽" },
                new LanguageItem { Code = "fr", Name = "Français", Countries = "France / Canada", Flag = "🇫🇷 🇨🇦" },
                new LanguageItem { Code = "ja", Name = "日本語", Countries = "日本", Flag = "🇯🇵" },
                new LanguageItem { Code = "ko", Name = "한국어", Countries = "대한민국", Flag = "🇰🇷" }
            };
        }

        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();
            if (Handler != null && !_isInitialized)
            {
                try
                {
                    var serviceProvider = MyVocaList.View.ServiceProvider.FromPage(this);
                    _languageService = serviceProvider?.GetService<ILanguageService>();

                    if (_languageService != null) _isInitialized = true;
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Error in OnHandlerChanged");
                }
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // No manual UI creation needed anymore
        }

        // Called by SmartPageLifecycleBehavior via Command
        private async Task InitializeDataAsync()
        {
             try
            {
                // Init selection from preferences or service
                var currentCode = Preferences.Get("UserLanguage", "en");
                if (_isInitialized && _languageService != null)
                {
                    // Optionally fetch from service if needed
                }

                selectedLanguage = currentCode;

                foreach (var lang in Languages)
                {
                    lang.IsSelected = (lang.Code == currentCode);
                }
                
                // Trigger update
                OnPropertyChanged(nameof(Languages));
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error initializing data");
            }
            await Task.Delay(50);
        }

        private async Task OnLanguageSelectedAsync(LanguageItem item)
        {
            if (item == null) return;

            try
            {
                // Update selection state
                foreach (var lang in Languages)
                {
                    lang.IsSelected = (lang.Code == item.Code);
                }

                selectedLanguage = item.Code;
                
                // Force UI update if needed (though ObservableCollection + INotifyPropertyChanged handles it)
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error selecting language");
            }
            await Task.Delay(50); // Small delay for visual feedback
        }

        #region Save/Cancel Handlers
        private async void OnSaveClicked(object sender, EventArgs e)
        {
            await ShowSaveConfirmationAsync();
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await ShowExitConfirmationAsync();
        }
        #endregion

        #region Saving Logic
        private async Task ShowSaveConfirmationAsync()
        {
            try
            {
                // Get selected language item
                var selectedItem = Languages.FirstOrDefault(l => l.IsSelected);
                if (selectedItem == null)
                {
                    Logger.Warning("No language selected");
                    return;
                }

                // Show confirmation popup with selected language
                var popup = new ConfirmationPopup(
                    "Confirm Language",
                    $"Confirm {selectedItem.Name} as your preferred language?",
                    "Confirm",
                    "Cancel"
                );

                var confirmed = await popup.ShowAsync();
                if (!confirmed)
                {
                    Logger.Information("Language selection cancelled by user");
                    return;
                }

                // User confirmed - proceed with save
                await SaveLanguageAndNavigateAsync();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error showing save confirmation");
            }
        }

        private async Task SaveLanguageAndNavigateAsync()
        {
            try
            {
                // Show global loading
                await GlobalLoadingOverlay.ShowLoadingAsync("Saving language...");

                // Save the selected language
                await SaveSelectedLanguageAsync(selectedLanguage);

                // Navigate to StackPage
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Application.Current.MainPage = new NavigationPage(new StackPage());
                    Logger.Information("Navigation to StackPage completed");
                });

                // Hide loading
                await GlobalLoadingOverlay.HideLoadingAsync();
            }
            catch (Exception ex)
            {
                await GlobalLoadingOverlay.HideLoadingAsync();
                Logger.Error(ex, "Error saving language");

                // Show error using ConfirmationPopup (info-only, single button)
                var errorPopup = new ConfirmationPopup(
                    "Error",
                    "Failed to save language selection. Please try again.",
                    "OK",
                    "OK"
                );
                await errorPopup.ShowAsync();
            }
        }

        private async Task SaveSelectedLanguageAsync(string languageCode)
        {
            try
            {
                // Save language in app preferences
                Preferences.Set("UserLanguage", languageCode);

                // Use language service to persist selection
                if (_languageService != null)
                {
                    await _languageService.SetUserLanguageAsync(languageCode);
                }
                else
                {
                    Logger.Warning("Language service not available");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error saving language");
                throw;
            }
        }

        private async Task ShowExitConfirmationAsync()
        {
            try
            {
                // Show confirmation popup for exit
                var popup = new ConfirmationPopup(
                    "Exit Application",
                    "Are you sure you want to exit the application?",
                    "Exit",
                    "Cancel"
                );

                var confirmed = await popup.ShowAsync();
                if (confirmed)
                {
                    // Close the application
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error showing exit confirmation");
            }
        }
        #endregion

        #region Hardware Back Button
        protected override bool OnBackButtonPressed()
        {
            // Prevent hardware back button - user must use Cancel to exit
            return true;
        }
        #endregion
    }

    // Modelo para representar um item de idioma
    public class LanguageItem : INotifyPropertyChanged
    {
        private bool isSelected;

        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Countries { get; set; } = string.Empty;
        public string Flag { get; set; } = string.Empty;

        public bool IsSelected
        {
            get => isSelected;
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}