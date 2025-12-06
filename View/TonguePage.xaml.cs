using Microsoft.Maui.Controls;
using MyVocaList.Contracts.Models;
using MyVocaList.Services;
using MyVocaList.View.Components;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace MyVocaList.View
{
    public partial class TonguePage : ContentPage
    {
        private ObservableCollection<LanguageItem> languages;
        private ILanguageService? _languageService;
        private ServiceProvider? _serviceProvider;
        private string selectedLanguage = "en"; // Default language
        private bool _isInitialized = false;

        // Lifecycle Command
        public ICommand LoadDataCommand { get; }

        public TonguePage()
        {
            LoadDataCommand = new Command(async () => await InitializeDataAsync());
            InitializeComponent();
            this.BindingContext = this;

            // Initialize the list of languages - ONLY 6 SUPPORTED LANGUAGES
            languages = new ObservableCollection<LanguageItem>
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
                    System.Diagnostics.Debug.WriteLine($"TonguePage Error: {ex.Message}");
                }
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Load language buttons when page appears
            CreateLanguageButtons();
        }

        // Called by SmartPageLifecycleBehavior via Command
        private async Task InitializeDataAsync()
        {
            // Data init logic...
            await Task.Delay(50);
        }

        private void CreateLanguageButtons()
        {
            try
            {
                // Clear existing buttons
                if (languagesContainer != null)
                {
                    languagesContainer.Children.Clear();

                    // Create language buttons with Material Design styling
                    foreach (var language in languages)
                    {
                        var frame = new Frame
                        {
                            Margin = new Thickness(0, 4),
                            Padding = new Thickness(16, 12),
                            HasShadow = false,
                            CornerRadius = 12
                        };

                        // Apply Material Design style based on selection
                        if (language.IsSelected)
                        {
                            // Selected state: use SecondaryContainer and Primary border
                            object secondaryContainerResource = null;
                            object primaryResource = null;

                            if (Application.Current?.Resources.TryGetValue("SecondaryContainer", out secondaryContainerResource) == true)
                                frame.BackgroundColor = secondaryContainerResource as Color;

                            if (Application.Current?.Resources.TryGetValue("Primary", out primaryResource) == true)
                                frame.BorderColor = primaryResource as Color;
                            else
                                frame.BorderColor = Colors.Transparent;
                        }
                        else
                        {
                            // Unselected state: use Surface (same as ElevatedCard style)
                            object surfaceResource = null;
                            if (Application.Current?.Resources.TryGetValue("Surface", out surfaceResource) == true)
                                frame.BackgroundColor = surfaceResource as Color;

                            frame.BorderColor = Colors.Transparent;
                        }

                        var grid = new Grid
                        {
                            ColumnDefinitions =
                            {
                                new ColumnDefinition { Width = GridLength.Auto },
                                new ColumnDefinition { Width = GridLength.Star },
                                new ColumnDefinition { Width = GridLength.Auto }
                            },
                            ColumnSpacing = 16
                        };

                        // Flag icon (left)
                        var flagLabel = new Label
                        {
                            Text = language.Flag,
                            FontSize = 24,
                            HorizontalOptions = LayoutOptions.Center,
                            VerticalOptions = LayoutOptions.Center
                        };

                        // Language name (center)
                        var nameLabel = new Label
                        {
                            Text = language.Name,
                            HorizontalOptions = LayoutOptions.Start,
                            VerticalOptions = LayoutOptions.Center,
                            LineBreakMode = LineBreakMode.TailTruncation
                        };

                        // Apply TitleMedium style
                        object titleMediumStyle = null;
                        if (Application.Current?.Resources.TryGetValue("TitleMedium", out titleMediumStyle) == true && titleMediumStyle is Style style)
                            nameLabel.Style = style;

                        // Selection indicator (right) - only shown when selected
                        var checkLabel = new Label
                        {
                            Text = "✓",
                            FontSize = 20,
                            FontAttributes = FontAttributes.Bold,
                            HorizontalOptions = LayoutOptions.Center,
                            VerticalOptions = LayoutOptions.Center,
                            IsVisible = language.IsSelected
                        };

                        // Apply Primary color to check
                        object primaryColor = null;
                        if (Application.Current?.Resources.TryGetValue("Primary", out primaryColor) == true)
                            checkLabel.TextColor = primaryColor as Color;

                        // Add elements to grid
                        grid.Add(flagLabel, 0, 0);
                        grid.Add(nameLabel, 1, 0);
                        grid.Add(checkLabel, 2, 0);

                        // Configure frame with grid
                        frame.Content = grid;

                        // Add tap recognizer
                        var languageCode = language.Code;
                        var tapGesture = new TapGestureRecognizer();
                        tapGesture.Tapped += async (s, e) =>
                        {
                            await SelectLanguage(languageCode);
                        };
                        frame.GestureRecognizers.Add(tapGesture);

                        // Add frame to container
                        languagesContainer.Children.Add(frame);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating language buttons: {ex.Message}\nStack: {ex.StackTrace}");
            }
        }

        private async Task SelectLanguage(string languageCode)
        {
            try
            {
                // Update language selection
                foreach (var language in languages)
                {
                    language.IsSelected = (language.Code == languageCode);
                    if (language.IsSelected)
                    {
                        selectedLanguage = language.Code;
                    }
                }

                // Recreate buttons to reflect the new visual selection
                CreateLanguageButtons();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error selecting language: {ex.Message}");
            }
        }

        #region Save/Cancel Handlers
        private async void OnSaveClicked(object sender, EventArgs e)
        {
            await SaveLanguageAndNavigateAsync();
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await CloseApplicationAsync();
        }
        #endregion

        #region Saving Logic
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
                    System.Diagnostics.Debug.WriteLine("[SUCCESS] Navigation to StackPage completed");
                });

                // Hide loading
                await GlobalLoadingOverlay.HideLoadingAsync();
            }
            catch (Exception ex)
            {
                await GlobalLoadingOverlay.HideLoadingAsync();
                System.Diagnostics.Debug.WriteLine($"[ERROR] Error saving language: {ex.Message}");
                await DisplayAlert("Error", "Failed to save language selection", "OK");
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
                    System.Diagnostics.Debug.WriteLine("Language service not available");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving language: {ex.Message}");
                throw;
            }
        }

        private async Task CloseApplicationAsync()
        {
            try
            {
                bool confirmed = await DisplayAlert(
                    "Exit",
                    "Are you sure you want to exit the application?",
                    "Yes",
                    "No"
                );

                if (confirmed)
                {
                    // Close the application
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error closing application: {ex.Message}");
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