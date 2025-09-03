using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;

namespace MyKaraoke.View.Components
{
    /// <summary>
    /// ✅ SINGLETON: Loading overlay global que aparece em qualquer página
    /// 🎯 AUTO-INJECT: Se adiciona automaticamente à página atual
    /// 🔄 UNIVERSAL: Não precisa declarar em XAML
    /// </summary>
    public class GlobalLoadingOverlay
    {
        #region Singleton Pattern

        private static readonly Lazy<GlobalLoadingOverlay> _instance =
            new Lazy<GlobalLoadingOverlay>(() => new GlobalLoadingOverlay());

        public static GlobalLoadingOverlay Instance => _instance.Value;

        private GlobalLoadingOverlay() { }

        #endregion

        #region Private Fields

        private ContentView _currentOverlay;
        private ContentPage _currentPage;
        private bool _isShowing = false;
        private readonly object _lockObject = new object();

        #endregion

        #region Public Methods

        /// <summary>
        /// 🔄 SHOW: Mostra loading na página atual automaticamente
        /// </summary>
        public async Task ShowAsync(string message = "Carregando...")
        {
            try
            {
                lock (_lockObject)
                {
                    if (_isShowing)
                    {
                        System.Diagnostics.Debug.WriteLine($"🔄 GlobalLoadingOverlay: Já está sendo exibido - ignorando");
                        return;
                    }
                    _isShowing = true;
                }

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    var currentPage = GetCurrentPage();
                    if (currentPage == null)
                    {
                        System.Diagnostics.Debug.WriteLine($"❌ GlobalLoadingOverlay: Página atual não encontrada");
                        _isShowing = false;
                        return;
                    }

                    // 🎯 CRIA: Overlay dinamicamente
                    _currentOverlay = CreateLoadingOverlay(message);
                    _currentPage = currentPage;

                    // 🎯 INJETA: Na página atual
                    InjectOverlayIntoPage(currentPage, _currentOverlay);

                    System.Diagnostics.Debug.WriteLine($"🔄 GlobalLoadingOverlay: EXIBIDO na {currentPage.GetType().Name}");
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ GlobalLoadingOverlay: Erro ao mostrar: {ex.Message}");
                _isShowing = false;
            }
        }

        /// <summary>
        /// 🔄 HIDE: Esconde loading da página atual
        /// </summary>
        public async Task HideAsync()
        {
            try
            {
                lock (_lockObject)
                {
                    if (!_isShowing)
                    {
                        System.Diagnostics.Debug.WriteLine($"🔄 GlobalLoadingOverlay: Não está sendo exibido - ignorando");
                        return;
                    }
                    _isShowing = false;
                }

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    if (_currentOverlay != null && _currentPage != null)
                    {
                        // 🎯 REMOVE: Overlay da página
                        RemoveOverlayFromPage(_currentPage, _currentOverlay);

                        System.Diagnostics.Debug.WriteLine($"🔄 GlobalLoadingOverlay: ESCONDIDO da {_currentPage.GetType().Name}");

                        _currentOverlay = null;
                        _currentPage = null;
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ GlobalLoadingOverlay: Erro ao esconder: {ex.Message}");
            }
        }

        /// <summary>
        /// 🎯 WRAPPER: Executa ação com loading automático
        /// </summary>
        public async Task ExecuteWithLoadingAsync(Func<Task> action, string message = "Carregando...")
        {
            try
            {
                await ShowAsync(message);
                await action();
            }
            finally
            {
                await HideAsync();
            }
        }

        /// <summary>
        /// 🎯 WRAPPER: Executa ação com loading automático (com resultado)
        /// </summary>
        public async Task<T> ExecuteWithLoadingAsync<T>(Func<Task<T>> action, string message = "Carregando...")
        {
            try
            {
                await ShowAsync(message);
                return await action();
            }
            finally
            {
                await HideAsync();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 🎯 CRIA: Overlay dinamicamente
        /// </summary>
        private ContentView CreateLoadingOverlay(string message)
        {
            var overlay = new ContentView
            {
                BackgroundColor = Color.FromArgb("#80000000"),
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                ZIndex = 9999,
                Content = new VerticalStackLayout
                {
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Spacing = 10,
                    Children =
                    {
                        new ActivityIndicator
                        {
                            IsRunning = true,
                            Color = Color.FromArgb("#e91e63") // PrimaryPink
                        },
                        new Label
                        {
                            Text = message,
                            TextColor = Colors.White,
                            FontAttributes = FontAttributes.Bold
                        }
                    }
                }
            };

            return overlay;
        }

        /// <summary>
        /// 🎯 INJETA: Overlay na página atual
        /// </summary>
        private void InjectOverlayIntoPage(ContentPage page, ContentView overlay)
        {
            try
            {
                var content = page.Content;

                if (content is Grid grid)
                {
                    // 🎯 GRID: Adiciona como último filho (na frente)
                    Grid.SetRow(overlay, 0);
                    Grid.SetColumn(overlay, 0);
                    Grid.SetRowSpan(overlay, Math.Max(1, grid.RowDefinitions.Count));
                    Grid.SetColumnSpan(overlay, Math.Max(1, grid.ColumnDefinitions.Count));
                    grid.Children.Add(overlay);

                    System.Diagnostics.Debug.WriteLine($"🎯 GlobalLoadingOverlay: Injetado em Grid");
                }
                else if (content is StackLayout stackLayout)
                {
                    // 🎯 STACKLAYOUT: Envolve em Grid para sobreposição
                    var wrapperGrid = new Grid();

                    page.Content = wrapperGrid;
                    wrapperGrid.Children.Add(stackLayout);
                    wrapperGrid.Children.Add(overlay);

                    System.Diagnostics.Debug.WriteLine($"🎯 GlobalLoadingOverlay: Injetado via wrapper Grid");
                }
                else
                {
                    // 🎯 OUTROS: Envolve em Grid
                    var wrapperGrid = new Grid();

                    page.Content = wrapperGrid;
                    wrapperGrid.Children.Add(content);
                    wrapperGrid.Children.Add(overlay);

                    System.Diagnostics.Debug.WriteLine($"🎯 GlobalLoadingOverlay: Injetado via wrapper Grid genérico");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ GlobalLoadingOverlay: Erro ao injetar: {ex.Message}");
            }
        }

        /// <summary>
        /// 🎯 REMOVE: Overlay da página
        /// </summary>
        private void RemoveOverlayFromPage(ContentPage page, ContentView overlay)
        {
            try
            {
                var content = page.Content;

                if (content is Grid grid && grid.Children.Contains(overlay))
                {
                    grid.Children.Remove(overlay);
                    System.Diagnostics.Debug.WriteLine($"🎯 GlobalLoadingOverlay: Removido do Grid");
                }
                else
                {
                    // 🎯 PROCURA: Em toda a árvore visual
                    RemoveOverlayRecursive(content, overlay);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ GlobalLoadingOverlay: Erro ao remover: {ex.Message}");
            }
        }

        /// <summary>
        /// 🔍 BUSCA: Remove overlay recursivamente
        /// </summary>
        private void RemoveOverlayRecursive(VisualElement element, ContentView overlay)
        {
            if (element is Layout layout && layout.Children.Contains(overlay))
            {
                layout.Children.Remove(overlay);
                System.Diagnostics.Debug.WriteLine($"🎯 GlobalLoadingOverlay: Removido de {layout.GetType().Name}");
            }
            else if (element is Layout parentLayout)
            {
                foreach (var child in parentLayout.Children.OfType<VisualElement>())
                {
                    RemoveOverlayRecursive(child, overlay);
                }
            }
        }

        /// <summary>
        /// 🔍 HELPER: Obtém a página atual de forma robusta
        /// </summary>
        private ContentPage GetCurrentPage()
        {
            try
            {
                // Método 1: Via Application.Current.MainPage
                if (Application.Current?.MainPage is NavigationPage navPage)
                {
                    return navPage.CurrentPage as ContentPage;
                }

                if (Application.Current?.MainPage is ContentPage mainPage)
                    return mainPage;

                // Método 2: Via Shell
                if (Shell.Current?.CurrentPage is ContentPage shellPage)
                    return shellPage;

                // Método 3: Via Navigation stack
                var lastPage = Application.Current?.MainPage?.Navigation?.NavigationStack?.LastOrDefault();
                return lastPage as ContentPage;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ GlobalLoadingOverlay: Erro ao obter página atual: {ex.Message}");
                return null;
            }
        }

        #endregion

        #region Static Helper Methods

        /// <summary>
        /// 🎯 STATIC: Método estático para facilitar uso
        /// </summary>
        public static async Task ShowLoadingAsync(string message = "Carregando...")
        {
            await Instance.ShowAsync(message);
        }

        /// <summary>
        /// 🎯 STATIC: Método estático para facilitar uso
        /// </summary>
        public static async Task HideLoadingAsync()
        {
            await Instance.HideAsync();
        }

        /// <summary>
        /// 🎯 STATIC: Wrapper estático para ações com loading
        /// </summary>
        public static async Task ExecuteWithLoadingAsync(Func<Task> action, string message = "Carregando...")
        {
            await Instance.ExecuteWithLoadingAsync(action, message);
        }

        /// <summary>
        /// 🎯 STATIC: Wrapper estático para ações com loading (com resultado)
        /// </summary>
        public static async Task<T> ExecuteWithLoadingAsync<T>(Func<Task<T>> action, string message = "Carregando...")
        {
            return await Instance.ExecuteWithLoadingAsync(action, message);
        }

        /// <summary>
        /// 🎯 STATIC: Mensagens contextuais específicas para ações
        /// </summary>
        public static async Task ShowSavingAsync() => await ShowLoadingAsync("Salvando...");
        public static async Task ShowDeletingAsync() => await ShowLoadingAsync("Excluindo...");
        public static async Task ShowNavigatingAsync() => await ShowLoadingAsync("Navegando...");
        public static async Task ShowLoadingDataAsync() => await ShowLoadingAsync("Carregando dados...");
        public static async Task ShowValidatingAsync() => await ShowLoadingAsync("Validando...");

        #endregion
    }
}