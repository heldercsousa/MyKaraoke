using Microsoft.Maui.Controls;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace MyKaraoke.View.Components
{
    /// <summary>
    /// 🎯 CENTRALIZADO: Loading overlay com sistema de requisições inteligente
    /// 🛡️ PRIORIDADES: Decide automaticamente qual loading mostrar baseado em contexto
    /// 🧠 INTELIGENTE: Resolve conflitos entre diferentes solicitantes
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
        private readonly ConcurrentDictionary<string, LoadingRequest> _activeRequests = new();
        private readonly object _lockObject = new object();
        private ContentView _currentOverlay;
        private ContentPage _currentPage;
        private bool _isPhysicallyShowing = false;
        private LoadingRequest _currentActiveRequest;
        #endregion

        #region Public Methods - Sistema de Requisições

        /// <summary>
        /// 🎯 REQUISITA: Loading com prioridade e contexto
        /// </summary>
        public async Task RequestShowAsync(string requesterId, string message = "Carregando...", LoadingPriority priority = LoadingPriority.Navigation, LoadingContext context = LoadingContext.PageNavigation, bool isPersistent = false, TimeSpan? autoHideAfter = null)
        {
            try
            {
                var request = new LoadingRequest(requesterId, message, priority, context, isPersistent, autoHideAfter);

                lock (_lockObject)
                {
                    // Adiciona/atualiza requisição
                    _activeRequests.AddOrUpdate(requesterId, request, (key, oldValue) => request);

                    System.Diagnostics.Debug.WriteLine($"🎯 LoadingRequest: {requesterId} solicitou loading - Priority: {priority}, Context: {context}, Message: '{message}'");
                }

                // Avalia se deve mostrar este loading
                await EvaluateAndShowLoading();

                // Configura auto-hide se especificado
                if (autoHideAfter.HasValue && !isPersistent)
                {
                    _ = Task.Run(async () =>
                    {
                        await Task.Delay(autoHideAfter.Value);
                        await RequestHideAsync(requesterId);
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ GlobalLoadingOverlay: Erro em RequestShowAsync: {ex.Message}");
            }
        }

        /// <summary>
        /// 🎯 REMOVE: Requisição específica de loading
        /// </summary>
        public async Task RequestHideAsync(string requesterId)
        {
            try
            {
                bool wasRemoved = false;
                LoadingRequest removedRequest = null;

                lock (_lockObject)
                {
                    wasRemoved = _activeRequests.TryRemove(requesterId, out removedRequest);
                }

                if (wasRemoved)
                {
                    System.Diagnostics.Debug.WriteLine($"🎯 LoadingRequest: {requesterId} removeu requisição de loading");

                    // Reavalia se deve continuar mostrando loading
                    await EvaluateAndShowLoading();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ LoadingRequest: {requesterId} tentou remover requisição inexistente");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ GlobalLoadingOverlay: Erro em RequestHideAsync: {ex.Message}");
            }
        }

        /// <summary>
        /// 🎯 FORÇA: Remove todas as requisições de um contexto específico
        /// </summary>
        public async Task ClearContextAsync(LoadingContext context)
        {
            try
            {
                var requestsToRemove = _activeRequests.Values.Where(r => r.Context == context).ToList();

                foreach (var request in requestsToRemove)
                {
                    _activeRequests.TryRemove(request.RequesterId, out _);
                }

                if (requestsToRemove.Any())
                {
                    System.Diagnostics.Debug.WriteLine($"🎯 LoadingRequest: Removidas {requestsToRemove.Count} requisições do contexto {context}");
                    await EvaluateAndShowLoading();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ GlobalLoadingOverlay: Erro em ClearContextAsync: {ex.Message}");
            }
        }

        /// <summary>
        /// 🧹 LIMPEZA: Remove requisições órfãs ou expiradas
        /// </summary>
        public async Task CleanupAsync()
        {
            try
            {
                var now = DateTime.Now;
                var requestsToRemove = _activeRequests.Values
                    .Where(r => r.AutoHideAfter.HasValue &&
                               (now - r.RequestTime) > r.AutoHideAfter.Value)
                    .ToList();

                foreach (var request in requestsToRemove)
                {
                    _activeRequests.TryRemove(request.RequesterId, out _);
                }

                if (requestsToRemove.Any())
                {
                    System.Diagnostics.Debug.WriteLine($"🧹 LoadingRequest: Removidas {requestsToRemove.Count} requisições expiradas");
                    await EvaluateAndShowLoading();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ GlobalLoadingOverlay: Erro em CleanupAsync: {ex.Message}");
            }
        }

        #endregion

        #region Private Methods - Lógica de Decisão

        /// <summary>
        /// 🧠 CÉREBRO: Avalia qual requisição deve ser atendida
        /// </summary>
        private async Task EvaluateAndShowLoading()
        {
            try
            {
                LoadingRequest bestRequest = null;

                lock (_lockObject)
                {
                    if (!_activeRequests.Any())
                    {
                        // Nenhuma requisição ativa - esconde loading
                        _ = Task.Run(async () => await HidePhysicalLoading());
                        return;
                    }

                    // Algoritmo de decisão inteligente
                    bestRequest = SelectBestRequest();
                }

                if (bestRequest != null)
                {
                    await ShowPhysicalLoading(bestRequest);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ GlobalLoadingOverlay: Erro em EvaluateAndShowLoading: {ex.Message}");
            }
        }

        /// <summary>
        /// 🧠 ALGORITMO: Seleciona a melhor requisição para exibir
        /// </summary>
        private LoadingRequest SelectBestRequest()
        {
            var requests = _activeRequests.Values.ToList();

            // Regra 1: Prioridade mais alta
            var maxPriority = requests.Max(r => r.Priority);
            var highPriorityRequests = requests.Where(r => r.Priority == maxPriority).ToList();

            if (highPriorityRequests.Count == 1)
            {
                return highPriorityRequests.First();
            }

            // Regra 2: Se há empate, prioriza por contexto
            var contextPriority = new Dictionary<LoadingContext, int>
            {
                { LoadingContext.UserInteraction, 4 },
                { LoadingContext.ComponentLoading, 3 },
                { LoadingContext.PageNavigation, 2 },
                { LoadingContext.DatabaseOperation, 1 }
            };

            var bestByContext = highPriorityRequests
                .OrderByDescending(r => contextPriority.GetValueOrDefault(r.Context, 0))
                .First();

            // Regra 3: Se ainda há empate, prioriza requisições persistentes
            var persistentRequests = highPriorityRequests.Where(r => r.IsPersistent).ToList();
            if (persistentRequests.Any())
            {
                return persistentRequests.OrderBy(r => r.RequestTime).First(); // Mais antiga
            }

            // Regra 4: Por último, a requisição mais recente
            return highPriorityRequests.OrderByDescending(r => r.RequestTime).First();
        }

        /// <summary>
        /// 🎯 EXIBE: Loading físico na tela
        /// </summary>
        private async Task ShowPhysicalLoading(LoadingRequest request)
        {
            try
            {
                // Se já está mostrando a mesma requisição, não faz nada
                if (_currentActiveRequest?.RequesterId == request.RequesterId && _isPhysicallyShowing)
                {
                    return;
                }

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    var currentPage = GetCurrentPage();
                    if (currentPage == null) return;

                    // Remove loading anterior se existir
                    if (_currentOverlay != null && _currentPage != null)
                    {
                        RemoveOverlayFromPage(_currentPage, _currentOverlay);
                    }

                    // Cria novo overlay
                    _currentOverlay = CreateLoadingOverlay(request.Message);
                    _currentPage = currentPage;
                    _currentActiveRequest = request;

                    // Injeta na página
                    InjectOverlayIntoPage(currentPage, _currentOverlay);
                    _isPhysicallyShowing = true;

                    System.Diagnostics.Debug.WriteLine($"🎯 LoadingOverlay: EXIBINDO '{request.Message}' para {request.RequesterId} na {currentPage.GetType().Name}");
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ GlobalLoadingOverlay: Erro ao mostrar loading físico: {ex.Message}");
            }
        }

        /// <summary>
        /// 🎯 ESCONDE: Loading físico da tela
        /// </summary>
        private async Task HidePhysicalLoading()
        {
            try
            {
                if (!_isPhysicallyShowing) return;

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    if (_currentOverlay != null && _currentPage != null)
                    {
                        RemoveOverlayFromPage(_currentPage, _currentOverlay);
                        System.Diagnostics.Debug.WriteLine($"🎯 LoadingOverlay: ESCONDIDO da {_currentPage.GetType().Name}");

                        _currentOverlay = null;
                        _currentPage = null;
                        _currentActiveRequest = null;
                        _isPhysicallyShowing = false;
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ GlobalLoadingOverlay: Erro ao esconder loading físico: {ex.Message}");
            }
        }

        #endregion

        #region Private Methods - UI Management (Mantidos do código original)

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
                            Color = Color.FromArgb("#e91e63")
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

        private void InjectOverlayIntoPage(ContentPage page, ContentView overlay)
        {
            try
            {
                var content = page.Content;
                if (content is Grid grid)
                {
                    Grid.SetRow(overlay, 0);
                    Grid.SetColumn(overlay, 0);
                    Grid.SetRowSpan(overlay, Math.Max(1, grid.RowDefinitions.Count));
                    Grid.SetColumnSpan(overlay, Math.Max(1, grid.ColumnDefinitions.Count));
                    grid.Children.Add(overlay);
                }
                else
                {
                    var wrapperGrid = new Grid();
                    page.Content = wrapperGrid;
                    wrapperGrid.Children.Add(content);
                    wrapperGrid.Children.Add(overlay);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ GlobalLoadingOverlay: Erro ao injetar overlay: {ex.Message}");
            }
        }

        private void RemoveOverlayFromPage(ContentPage page, ContentView overlay)
        {
            try
            {
                var content = page.Content;
                if (content is Grid grid && grid.Children.Contains(overlay))
                {
                    grid.Children.Remove(overlay);
                }
                else
                {
                    RemoveOverlayRecursive(content, overlay);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ GlobalLoadingOverlay: Erro ao remover overlay: {ex.Message}");
            }
        }

        private void RemoveOverlayRecursive(VisualElement element, ContentView overlay)
        {
            if (element is Layout layout && layout.Children.Contains(overlay))
            {
                layout.Children.Remove(overlay);
            }
            else if (element is Layout parentLayout)
            {
                foreach (var child in parentLayout.Children.OfType<VisualElement>())
                {
                    RemoveOverlayRecursive(child, overlay);
                }
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
                var lastPage = Application.Current?.MainPage?.Navigation?.NavigationStack?.LastOrDefault();
                if (lastPage is ContentPage contentPage && contentPage.Content != null)
                {
                    return contentPage;
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ GlobalLoadingOverlay: Erro ao obter página atual: {ex.Message}");
                return null;
            }
        }

        #endregion

        #region Public Static Wrapper Methods - Compatibilidade com Código Existente

        /// <summary>
        /// 🔄 COMPATIBILIDADE: Método estático simples (usa Navigation como padrão)
        /// </summary>
        public static async Task ShowLoadingAsync(string message = "Carregando...")
        {
            await Instance.RequestShowAsync("Legacy", message, LoadingPriority.Navigation, LoadingContext.PageNavigation);
        }

        /// <summary>
        /// 🔄 COMPATIBILIDADE: Método estático simples
        /// </summary>
        public static async Task HideLoadingAsync()
        {
            await Instance.RequestHideAsync("Legacy");
        }

        // Métodos específicos para diferentes contextos
        public static async Task ShowNavigatingAsync(string requesterId = "Navigation") =>
            await Instance.RequestShowAsync(requesterId, "Navegando...", LoadingPriority.Navigation, LoadingContext.PageNavigation);

        public static async Task ShowLoadingDataAsync(string requesterId = "Database") =>
            await Instance.RequestShowAsync(requesterId, "Carregando dados...", LoadingPriority.Database, LoadingContext.DatabaseOperation);

        public static async Task ShowWaitingNavBarAsync(string requesterId = "NavBarWait") =>
            await Instance.RequestShowAsync(requesterId, "Carregando página...", LoadingPriority.NavBarWait, LoadingContext.ComponentLoading, isPersistent: true);

        public static async Task HideNavigatingAsync(string requesterId = "Navigation") =>
            await Instance.RequestHideAsync(requesterId);

        public static async Task HideLoadingDataAsync(string requesterId = "Database") =>
            await Instance.RequestHideAsync(requesterId);

        public static async Task HideWaitingNavBarAsync(string requesterId = "NavBarWait") =>
            await Instance.RequestHideAsync(requesterId);

        #endregion

        #region Diagnostic Methods

        /// <summary>
        /// 📊 DIAGNÓSTICO: Estado atual das requisições
        /// </summary>
        public Dictionary<string, object> GetDiagnostics()
        {
            lock (_lockObject)
            {
                return new Dictionary<string, object>
                {
                    { "ActiveRequestsCount", _activeRequests.Count },
                    { "IsPhysicallyShowing", _isPhysicallyShowing },
                    { "CurrentActiveRequest", _currentActiveRequest?.RequesterId ?? "None" },
                    { "CurrentMessage", _currentActiveRequest?.Message ?? "None" },
                    { "ActiveRequests", _activeRequests.Keys.ToList() }
                };
            }
        }

        #endregion
    }
}