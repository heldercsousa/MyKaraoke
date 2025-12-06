using Microsoft.Maui.Controls.Shapes;
using MyVocaList.View.Animations; // Assuming you might use animations here, or standard Maui
using MauiView = Microsoft.Maui.Controls.View;
using System;
using System.Threading.Tasks;

namespace MyVocaList.View.Components
{
    public class GlobalSnackbar
    {
        #region Singleton
        private static readonly Lazy<GlobalSnackbar> _instance =
            new Lazy<GlobalSnackbar>(() => new GlobalSnackbar());
        public static GlobalSnackbar Instance => _instance.Value;
        private GlobalSnackbar() { }
        #endregion

        private Border _currentSnackbar; // Changed from Frame to Border
        private ContentPage _currentPage;

        // Helper to get colors from App.xaml
        private Color GetResourceColor(string key)
        {
            if (Application.Current.Resources.TryGetValue(key, out var value))
                return (Color)value;
            return Colors.Black; // Fallback
        }

        public static async Task ShowSuccessAsync(string message, int durationMs = 3000)
        {
            // Use Theme Color "Success" (or InverseSurface for standard MD3 look)
            await Instance.ShowAsync(message, "Success", durationMs);
        }

        public static async Task ShowErrorAsync(string message, int durationMs = 4000)
        {
            await Instance.ShowAsync(message, "Error", durationMs);
        }

        public static async Task ShowWarningAsync(string message, int durationMs = 3500)
        {
            await Instance.ShowAsync(message, "Warning", durationMs);
        }

        private async Task ShowAsync(string message, string colorKey, int durationMs)
        {
            try
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    var currentPage = GetCurrentPage();
                    if (currentPage == null) return;

                    // 1. Remove existing
                    if (_currentSnackbar != null && _currentPage != null)
                    {
                        RemoveSnackbarFromPage(_currentPage, _currentSnackbar);
                    }

                    // 2. Create & Inject
                    _currentSnackbar = CreateSnackbar(message, colorKey);
                    _currentPage = currentPage;
                    InjectSnackbarIntoPage(currentPage, _currentSnackbar);

                    // 3. Animate In (Slide Up + Fade)
                    _currentSnackbar.TranslationY = 50;
                    _currentSnackbar.Opacity = 0;

                    await Task.WhenAll(
                        _currentSnackbar.TranslateTo(0, 0, 250, Easing.CubicOut),
                        _currentSnackbar.FadeTo(1, 250)
                    );

                    // 4. Wait
                    await Task.Delay(durationMs);

                    // 5. Animate Out (Fade Out - cleaner than sliding down)
                    await _currentSnackbar.FadeTo(0, 200);

                    // 6. Cleanup
                    RemoveSnackbarFromPage(_currentPage, _currentSnackbar);
                    _currentSnackbar = null;
                    _currentPage = null;
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GlobalSnackbar Error: {ex.Message}");
            }
        }

        private Border CreateSnackbar(string message, string colorKey)
        {
            var backgroundColor = GetResourceColor(colorKey);

            // Determine Text Color based on background
            // If background is "Success" (Green), Text should be "OnSuccess" (White)
            var textColor = Colors.White;
            if (colorKey == "Error") textColor = GetResourceColor("OnError");
            if (colorKey == "Success") textColor = GetResourceColor("OnSuccess");
            if (colorKey == "Warning") textColor = GetResourceColor("OnWarning");

            // MD3 COMPLIANCE FIX: 
            // 1. Use Border instead of Frame
            // 2. CornerRadius = 4 (Extra Small)
            // 3. No heavy shadow (optional Stroke)

            var border = new Border
            {
                Stroke = Colors.Transparent,
                StrokeThickness = 0,
                StrokeShape = new RoundRectangle { CornerRadius = 4 }, // MD3 Standard
                BackgroundColor = backgroundColor,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.End,
                Margin = new Thickness(16, 0, 16, 90), // Keeps clearance for FAB/Nav
                Padding = new Thickness(16, 14), // Taller padding for better text breathing
                ZIndex = 10000,
                Shadow = new Shadow // Soft MD3-style shadow
                {
                    Brush = Colors.Black,
                    Offset = new Point(0, 2),
                    Radius = 4,
                    Opacity = 0.25f
                },
                Content = new Label
                {
                    Text = message,
                    TextColor = textColor,
                    FontSize = 14, // Body Medium
                    FontAttributes = FontAttributes.None, // MD3 regular weight
                    VerticalOptions = LayoutOptions.Center,
                    LineBreakMode = LineBreakMode.WordWrap
                }
            };

            return border;
        }

        private void InjectSnackbarIntoPage(ContentPage page, Border snackbar)
        {
            try
            {
                var content = page.Content;
                if (content is Grid grid)
                {
                    Grid.SetRow(snackbar, 0);
                    Grid.SetColumn(snackbar, 0);
                    Grid.SetRowSpan(snackbar, Math.Max(1, grid.RowDefinitions.Count));
                    Grid.SetColumnSpan(snackbar, Math.Max(1, grid.ColumnDefinitions.Count));
                    grid.Children.Add(snackbar);
                }
                else
                {
                    var wrapperGrid = new Grid();
                    page.Content = wrapperGrid;
                    wrapperGrid.Children.Add(content);
                    wrapperGrid.Children.Add(snackbar);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GlobalSnackbar Injection Error: {ex.Message}");
            }
        }

        private void RemoveSnackbarFromPage(ContentPage page, Border snackbar)
        {
            try
            {
                if (page.Content is Grid grid && grid.Children.Contains(snackbar))
                {
                    grid.Children.Remove(snackbar);
                }
                else if (page.Content is Grid wrapper && wrapper.Children.Count == 2 && wrapper.Children.Contains(snackbar))
                {
                    // Unwrap if we created a wrapper
                    var originalContent = wrapper.Children[0] as MauiView;
                    page.Content = originalContent;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GlobalSnackbar Removal Error: {ex.Message}");
            }
        }

        private ContentPage GetCurrentPage()
        {
            if (Application.Current?.MainPage is NavigationPage navPage)
                return navPage.CurrentPage as ContentPage;
            if (Application.Current?.MainPage is ContentPage mainPage)
                return mainPage;
            if (Shell.Current?.CurrentPage is ContentPage shellPage)
                return shellPage;
            return null;
        }
    }
}