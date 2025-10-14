using Microsoft.Maui.Controls;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MyVocaList.View.Components
{
    /// <summary>
    /// Snackbar global singleton que se injeta automaticamente na página atual
    /// </summary>
    public class GlobalSnackbar
    {
        #region Singleton
        private static readonly Lazy<GlobalSnackbar> _instance =
            new Lazy<GlobalSnackbar>(() => new GlobalSnackbar());
        public static GlobalSnackbar Instance => _instance.Value;
        private GlobalSnackbar() { }
        #endregion

        private Frame _currentSnackbar;
        private ContentPage _currentPage;

        /// <summary>
        /// Mostra snackbar de sucesso
        /// </summary>
        public static async Task ShowSuccessAsync(string message, int durationMs = 3000)
        {
            await Instance.ShowAsync(message, "#2E7D32", durationMs);
        }

        /// <summary>
        /// Mostra snackbar de erro
        /// </summary>
        public static async Task ShowErrorAsync(string message, int durationMs = 4000)
        {
            await Instance.ShowAsync(message, "#D32F2F", durationMs);
        }

        /// <summary>
        /// Mostra snackbar de aviso
        /// </summary>
        public static async Task ShowWarningAsync(string message, int durationMs = 3500)
        {
            await Instance.ShowAsync(message, "#F57C00", durationMs);
        }

        private async Task ShowAsync(string message, string backgroundColor, int durationMs)
        {
            try
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    var currentPage = GetCurrentPage();
                    if (currentPage == null)
                    {
                        System.Diagnostics.Debug.WriteLine("GlobalSnackbar: Página atual não encontrada");
                        return;
                    }

                    // Remove snackbar anterior se existir
                    if (_currentSnackbar != null && _currentPage != null)
                    {
                        RemoveSnackbarFromPage(_currentPage, _currentSnackbar);
                    }

                    // Cria novo snackbar
                    _currentSnackbar = CreateSnackbar(message, backgroundColor);
                    _currentPage = currentPage;

                    // Injeta na página
                    InjectSnackbarIntoPage(currentPage, _currentSnackbar);

                    // Anima entrada (slide up)
                    _currentSnackbar.TranslationY = 100;
                    await _currentSnackbar.TranslateTo(0, 0, 250, Easing.CubicOut);

                    // Aguarda duração
                    await Task.Delay(durationMs);

                    // Anima saída (slide down)
                    await _currentSnackbar.TranslateTo(0, 100, 200, Easing.CubicIn);

                    // Remove da página
                    RemoveSnackbarFromPage(_currentPage, _currentSnackbar);
                    _currentSnackbar = null;
                    _currentPage = null;
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GlobalSnackbar: Erro ao exibir: {ex.Message}");
            }
        }

        private Frame CreateSnackbar(string message, string backgroundColor)
        {
            var snackbar = new Frame
            {
                BackgroundColor = Color.FromArgb(backgroundColor),
                CornerRadius = 8,
                Padding = new Thickness(16, 12),
                Margin = new Thickness(16, 0, 16, 90),
                HasShadow = true,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.End,
                ZIndex = 10000,
                Content = new Label
                {
                    Text = message,
                    TextColor = Colors.White,
                    FontSize = 14,
                    VerticalOptions = LayoutOptions.Center,
                    LineBreakMode = LineBreakMode.WordWrap
                }
            };

            return snackbar;
        }

        private void InjectSnackbarIntoPage(ContentPage page, Frame snackbar)
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
                    // Se não é Grid, cria um wrapper
                    var wrapperGrid = new Grid();
                    page.Content = wrapperGrid;
                    wrapperGrid.Children.Add(content);
                    wrapperGrid.Children.Add(snackbar);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GlobalSnackbar: Erro ao injetar: {ex.Message}");
            }
        }

        private void RemoveSnackbarFromPage(ContentPage page, Frame snackbar)
        {
            try
            {
                var content = page.Content;
                if (content is Grid grid && grid.Children.Contains(snackbar))
                {
                    grid.Children.Remove(snackbar);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GlobalSnackbar: Erro ao remover: {ex.Message}");
            }
        }

        private ContentPage GetCurrentPage()
        {
            try
            {
                if (Application.Current?.MainPage is NavigationPage navPage)
                    return navPage.CurrentPage as ContentPage;
                if (Application.Current?.MainPage is ContentPage mainPage)
                    return mainPage;
                if (Shell.Current?.CurrentPage is ContentPage shellPage)
                    return shellPage;
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GlobalSnackbar: Erro ao obter página: {ex.Message}");
                return null;
            }
        }
    }
}