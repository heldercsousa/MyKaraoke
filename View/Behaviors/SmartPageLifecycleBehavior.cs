using MyVocaList.View.Components;
using MyVocaList.View.Extensions;
using MyVocaList.View.Interfaces;
using System.Reflection;
using System.Windows.Input;

namespace MyVocaList.View.Behaviors
{
    /// <summary>
    /// ✅ EVOLUÇÃO: PageLifecycleBehavior inteligente que detecta problemas automaticamente
    /// 🛡️ AUTO-BYPASS: Se LoadDataCommand falha, executa fallback local
    /// 🎯 SELF-HEALING: Se navbar não aparece, força exibição direta
    /// 🔄 BACKWARD-COMPATIBLE: Mantém 100% compatibilidade com PageLifecycleBehavior original
    /// 🧹 LIMPO: Removido PageInstanceManager desnecessário - GC gerencia ciclo de vida
    /// </summary>
    public class SmartPageLifecycleBehavior : Behavior<ContentPage>
    {
        #region Bindable Properties

        public static readonly BindableProperty NavBarProperty =
            BindableProperty.Create(nameof(NavBar), typeof(IAnimatableNavBar), typeof(SmartPageLifecycleBehavior));

        public static readonly BindableProperty LoadDataCommandProperty =
            BindableProperty.Create(nameof(LoadDataCommand), typeof(ICommand), typeof(SmartPageLifecycleBehavior));

        // ✅ DEPRECADA: LoadingIndicator não é mais necessário (singleton é usado)
        public static readonly BindableProperty LoadingIndicatorProperty =
            BindableProperty.Create(nameof(LoadingIndicator), typeof(VisualElement), typeof(SmartPageLifecycleBehavior));

        public static readonly BindableProperty MainContentProperty =
            BindableProperty.Create(nameof(MainContent), typeof(VisualElement), typeof(SmartPageLifecycleBehavior));

        public static readonly BindableProperty EnableAutoBypassProperty =
            BindableProperty.Create(nameof(EnableAutoBypass), typeof(bool), typeof(SmartPageLifecycleBehavior), true);

        public static readonly BindableProperty MaxFailuresBeforeBypassProperty =
            BindableProperty.Create(nameof(MaxFailuresBeforeBypass), typeof(int), typeof(SmartPageLifecycleBehavior), 2);

        public static readonly BindableProperty LoadingMessageProperty =
            BindableProperty.Create(nameof(LoadingMessage), typeof(string), typeof(SmartPageLifecycleBehavior), "Carregando...");

        public static readonly BindableProperty UseGlobalLoadingProperty =
            BindableProperty.Create(nameof(UseGlobalLoading), typeof(bool), typeof(SmartPageLifecycleBehavior), true);

        #endregion

        #region Properties

        public IAnimatableNavBar NavBar { get => (IAnimatableNavBar)GetValue(NavBarProperty); set => SetValue(NavBarProperty, value); }
        public ICommand LoadDataCommand { get => (ICommand)GetValue(LoadDataCommandProperty); set => SetValue(LoadDataCommandProperty, value); }

        /// <summary>
        /// ⚠️ DEPRECADA: Use UseGlobalLoading=true para loading automático
        /// </summary>
        public VisualElement LoadingIndicator { get => (VisualElement)GetValue(LoadingIndicatorProperty); set => SetValue(LoadingIndicatorProperty, value); }

        public VisualElement MainContent { get => (VisualElement)GetValue(MainContentProperty); set => SetValue(MainContentProperty, value); }
        public bool EnableAutoBypass { get => (bool)GetValue(EnableAutoBypassProperty); set => SetValue(EnableAutoBypassProperty, value); }
        public int MaxFailuresBeforeBypass { get => (int)GetValue(MaxFailuresBeforeBypassProperty); set => SetValue(MaxFailuresBeforeBypassProperty, value); }
        public string LoadingMessage { get => (string)GetValue(LoadingMessageProperty); set => SetValue(LoadingMessageProperty, value); }
        public bool UseGlobalLoading { get => (bool)GetValue(UseGlobalLoadingProperty); set => SetValue(UseGlobalLoadingProperty, value); }

        #endregion

        #region Private Fields

        private ContentPage _associatedPage;
        private bool _hasExecutedSuccessfully = false;
        private int _failureCount = 0;
        private bool _isProcessing = false;
        private readonly object _lockObject = new object();

        #endregion

        #region Behavior Lifecycle - LIMPO

        protected override void OnAttachedTo(ContentPage page)
        {
            base.OnAttachedTo(page);

            _associatedPage = page;

            // ✅ AUTO-DISCOVER: If NavBar not manually set, find it automatically
            if (NavBar == null)
            {
                NavBar = AutoDiscoverNavBar(page);
            }

            _associatedPage.Appearing += OnPageAppearing;
            _associatedPage.Disappearing += OnPageDisappearing;
        }

        protected override void OnDetachingFrom(ContentPage page)
        {
            lock (_lockObject)
            {
                if (_associatedPage != null)
                {
                    _associatedPage.Appearing -= OnPageAppearing;
                    _associatedPage.Disappearing -= OnPageDisappearing;
                }

                _isProcessing = false;
                _hasExecutedSuccessfully = false;
                _failureCount = 0;
            }

            base.OnDetachingFrom(page);
            _associatedPage = null;
        }

        #endregion

        #region Smart Page Lifecycle - OTIMIZADO

        private async void OnPageAppearing(object sender, EventArgs e)
        {
            try
            {
                bool shouldProcess;

                lock (_lockObject)
                {
                    if (_isProcessing)
                    {
                        shouldProcess = false;
                    }
                    else
                    {
                        _isProcessing = true;
                        shouldProcess = true;
                    }
                }

                if (!shouldProcess)
                {
                    return;
                }

                // 🎯 BYPASS: Verifica se deve fazer bypass
                if (ShouldBypassBehavior())
                {
                    await ExecutePageBypass();
                    return;
                }

                // 🧠 TENTATIVA: Executa ciclo normal com loading singleton
                var success = await TryExecuteNormalCycle();

                if (!success)
                {
                    _failureCount++;

                    if (EnableAutoBypass && _failureCount >= MaxFailuresBeforeBypass)
                    {
                        await ExecutePageBypass();
                        return;
                    }
                    else
                    {
                        await ForceShowNavBarAfterFailure();
                    }
                }
                else
                {
                    _hasExecutedSuccessfully = true;
                    _failureCount = 0;
                }
            }
            catch (Exception ex)
            {
                await ForceShowNavBarAfterFailure();

                lock (_lockObject)
                {
                    _isProcessing = false;
                }
                throw;
            }
        }

        private async Task<bool> TryExecuteNormalCycle()
        {
            var pageType = _associatedPage.GetType().Name;
            var requesterId = $"NormalCycle_{pageType}_{_associatedPage.GetHashCode()}";

            // ETAPA 1: Aguarda navbar estar pronta
            var navBarReady = await WaitForNavBarReady();

            // ETAPA 2: ✅ LOADING CENTRALIZADO - Solicita com prioridade de navegação
            if (UseGlobalLoading)
            {
                await GlobalLoadingOverlay.Instance.RequestShowAsync(
                    requesterId: requesterId,
                    message: LoadingMessage,
                    priority: LoadingPriority.Navigation,
                    context: LoadingContext.PageNavigation
                );
            }

            try
            {
                // ETAPA 3: Executa LoadDataCommand
                var dataLoaded = await TryExecuteLoadDataCommand();

                // ETAPA 4: Aguarda navbar estar COMPLETAMENTE pronta após carregamento de dados
                var navBarFullyReady = await WaitForNavBarFullyReady();
                return true;
            }
            catch
            {
                throw;
            }
            finally
            {
                // ETAPA 5: ✅ LOADING CENTRALIZADO - Remove requisição
                if (UseGlobalLoading)
                {
                    await GlobalLoadingOverlay.Instance.RequestHideAsync(requesterId);
                }
            }
        }

        /// <summary>
        /// Gets the friendly page name if the page implements IFriendlyPageName interface.
        /// Returns null if the page doesn't implement the interface.
        /// </summary>
        private string GetFriendlyPageName()
        {
            if (_associatedPage is IFriendlyPageName friendlyPage)
            {
                return friendlyPage.FriendlyName;
            }

            return null;
        }

        private async Task ExecutePageBypass()
        {

            var pageType = _associatedPage.GetType().Name;
            var requesterId = $"PageBypass_{pageType}_{_associatedPage.GetHashCode()}";

            // ✅ Get friendly page name if page implements IFriendlyPageName
            var friendlyPageName = GetFriendlyPageName();
            var loadingMessage = friendlyPageName != null
                ? $"Loading {friendlyPageName}..."
                : LoadingMessage;

            // ✅ LOADING CENTRALIZADO: Solicita loading com alta prioridade até navbar estar pronta
            await GlobalLoadingOverlay.Instance.RequestShowAsync(
                requesterId: requesterId,
                message: loadingMessage,
                priority: LoadingPriority.NavBarWait,
                context: LoadingContext.ComponentLoading,
                isPersistent: true // Mantém até explicitamente removido
            );

            try
            {
                // ✅ CORREÇÃO: Aguarda NavBar estar pronta ANTES de executar qualquer comando
                var navBarReady = await WaitForNavBarReady();

                // ✅ EXECUTA: LoadDataCommand (DatabaseInterceptor pode mostrar seu próprio loading)
                await TryExecuteLoadDataCommand();

                // ✅ MÉTODO REFLEXIVO: Tenta encontrar método OnAppearingBypass na página
                var pageTypeClass = _associatedPage.GetType();
                var bypassMethod = pageTypeClass.GetMethod("OnAppearingBypass", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                if (bypassMethod != null)
                {
                    if (bypassMethod.ReturnType == typeof(Task))
                    {
                        await (Task)bypassMethod.Invoke(_associatedPage, null);
                    }
                    else
                    {
                        bypassMethod.Invoke(_associatedPage, null);
                    }
                }
                else
                {
                    await _associatedPage.ExecuteStandardBypass();
                }

                // ✅ AGUARDA: NavBar estar COMPLETAMENTE pronta
                await EnsureNavBarIsShownAfterBypass();

                _hasExecutedSuccessfully = true;
                _failureCount = 0;
            }
            catch
            {
                throw;
            }
            finally
            {
                // ✅ SEMPRE: Remove a requisição de loading persistente
                await GlobalLoadingOverlay.Instance.RequestHideAsync(requesterId);
            }
        }
           

        private async void OnPageDisappearing(object sender, EventArgs e)
        {
            // ✅ LOADING SINGLETON: Esconde loading se página está saindo
            if (UseGlobalLoading)
            {
                await GlobalLoadingOverlay.HideLoadingAsync();
            }

            if (NavBar != null)
            {
                var hideTask = NavBar.HideAsync();
                var timeoutTask = Task.Delay(3000);
                var completedTask = await Task.WhenAny(hideTask, timeoutTask);
            }
        }

        #endregion

        #region Helper Methods - MANTIDOS E OTIMIZADOS

        private bool ShouldBypassBehavior()
        {
            var styleId = _associatedPage.StyleId;
            if (styleId == "BYPASS_PAGELIFECYCLE")
            {
                return true;
            }

            if (LoadDataCommand == null)
            {
                return true;
            }
                
            return false;
        }

        /// <summary>
        /// 🎯 NOVO: Aguarda a navbar estar COMPLETAMENTE pronta (com botões criados)
        /// </summary>
        private async Task<bool> WaitForNavBarFullyReady()
        {
            if (NavBar == null)
            {
                return true;
            }

            int attempts = 0;
            const int maxAttempts = 30; // 3 segundos

            while (attempts < maxAttempts)
            {
                await Task.Delay(100);
                attempts++;

                var isFullyReady = await CheckIfNavBarIsFullyReady();

                if (isFullyReady)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 🎯 NOVO: Verifica se NavBar específica está completamente pronta
        /// </summary>
        private async Task<bool> CheckIfNavBarIsFullyReady()
        {
            return await MainThread.InvokeOnMainThreadAsync(() =>
            {
                    // 🎯 DETECÇÃO ESPECÍFICA: InactiveQueueBottomNav (StackPage)
                if (NavBar is InactiveQueueBottomNav inactiveNav)
                {
                    // Verifica se está inicializado E tem botões
                    var diagnostics = inactiveNav.GetComponentDiagnostics();
                    var isInitialized = (bool)(diagnostics["IsInitialized"] ?? false);
                    var buttonCount = (int)(diagnostics["ButtonCount"] ?? 0);
                    var hasNavBarBehavior = (bool)(diagnostics["HasNavBarBehavior"] ?? false);

                    var isReady = isInitialized && buttonCount > 0 && hasNavBarBehavior;

                    return isReady;
                }

                // 🎯 DETECÇÃO ESPECÍFICA: CrudNavBarComponent (SpotPage, etc.)
                if (NavBar is CrudNavBarComponent crudNav)
                {
                    // Verifica se NavBarBehavior tem botões criados
                    var navBarBehavior = crudNav.NavBarBehavior;
                    if (navBarBehavior?.Buttons != null && navBarBehavior.Buttons.Count > 0)
                    {
                        return true;
                    }

                    return false;
                }

                // 🎯 DETECÇÃO GENÉRICA: Qualquer ContentView com Grid interno
                if (NavBar is ContentView contentView)
                {
                    if (contentView.Content is Grid grid && grid.Children.Count > 0)
                    {
                        // Verifica se tem estrutura de botões dentro do grid
                        foreach (var child in grid.Children)
                        {
                            if (child is Frame frame && frame.Content is Grid innerGrid && innerGrid.Children.Count > 0)
                            {
                                return true; // Tem estrutura de navbar com botões
                            }
                        }
                    }
                }

                return false;
                    
            });
        }

        private async Task<bool> WaitForNavBarReady()
        {
            if (NavBar == null) return true;
            
            int attempts = 0;
            const int maxAttempts = 30;

            while (attempts < maxAttempts)
            {
                await Task.Delay(100);
                attempts++;

                if (NavBar is ContentView contentView)
                {
                    var hasContent = await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        try
                        {
                            if (contentView.Content is Grid grid)
                            {
                                return grid.Children.Count > 0;
                            }
                            return contentView.Content != null;
                        }
                        catch
                        {
                            return false;
                        }
                    });

                    if (hasContent)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private async Task<bool> TryExecuteLoadDataCommand()
        {
            ICommand commandToExecute = LoadDataCommand;

            if (commandToExecute == null && _associatedPage != null)
            {
                var pageType = _associatedPage.GetType();
                var loadCommandProperty = pageType.GetProperty("LoadDataCommand");

                if (loadCommandProperty != null)
                {
                    commandToExecute = loadCommandProperty.GetValue(_associatedPage) as ICommand;
                }
            }

            if (commandToExecute.CanExecute(null))
            {
                commandToExecute?.Execute(null);
                await Task.Delay(500); // Aguarda operações assíncronas internas
                return true;

            }
            return false;
        }

        private async Task<bool> TryShowNavBar()
        {
            if (NavBar == null) return true;
            
            var showTask = NavBar.ShowAsync();
            var timeoutTask = Task.Delay(5000);
            var completedTask = await Task.WhenAny(showTask, timeoutTask);

            if (completedTask == timeoutTask)
            {
                return false;
            }

            return true;
        }

        private async Task ForceShowNavBarAfterFailure()
        {
            if (NavBar == null) return;

            var showTask = NavBar.ShowAsync();
            var timeoutTask = Task.Delay(5000);
            var completedTask = await Task.WhenAny(showTask, timeoutTask);
        }

        private async Task EnsureNavBarIsShownAfterBypass()
        {
            // 🎯 NOVA CORREÇÃO: Aguarda navbar estar completamente pronta antes de mostrar
            var navBarFullyReady = await WaitForNavBarFullyReady();
            
            if (NavBar == null) return;

            var showTask = NavBar.ShowAsync();
            var timeoutTask = Task.Delay(5000);
            var completedTask = await Task.WhenAny(showTask, timeoutTask);
        }

        private void SetLoadingState(bool isLoading)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (LoadingIndicator != null)
                    LoadingIndicator.IsVisible = isLoading;

                if (MainContent != null)
                    MainContent.IsVisible = !isLoading;
            });
        }

        /// <summary>
        /// ✅ AUTO-DISCOVERY: Finds navbar in page using multiple strategies
        /// 🎯 TIER 1: Self-registered navbar (via attached property) - FAST
        /// 🎯 TIER 2: Visual tree search (fallback) - SLOWER
        /// </summary>
        private IAnimatableNavBar AutoDiscoverNavBar(ContentPage page)
        {
            // ✅ TIER 1: Self-registered navbar (AUTOMATIC via OnParentSet)
            var navBar = MyVocaList.View.Extensions.NavBarExtensions.GetPageNavBar(page);
            if (navBar != null)
            {
                return navBar;
            }

            // ✅ TIER 2: Visual tree search (fallback for edge cases)
            navBar = FindNavBarInVisualTree(page);

            return navBar;
        }

        /// <summary>
        /// 🔍 VISUAL TREE SEARCH: Recursively searches for IAnimatableNavBar in visual tree
        /// </summary>
        private IAnimatableNavBar FindNavBarInVisualTree(Element element)
        {
            if (element is IAnimatableNavBar navbar)
            {
                return navbar;
            }

            if (element is ContentPage page && page.Content != null)
            {
                return FindNavBarInVisualTree(page.Content);
            }

            if (element is Layout layout)
            {
                foreach (var child in layout.Children)
                {
                    if (child is Element childElement)
                    {
                        var found = FindNavBarInVisualTree(childElement);
                        if (found != null) return found;
                    }
                }
            }

            return null;
        }

        #endregion

        #region Public Methods for Diagnostics - LIMPO

        public Dictionary<string, object> GetDiagnostics()
        {
            return new Dictionary<string, object>
            {
                { "HasExecutedSuccessfully", _hasExecutedSuccessfully },
                { "FailureCount", _failureCount },
                { "IsProcessing", _isProcessing },
                { "EnableAutoBypass", EnableAutoBypass },
                { "MaxFailuresBeforeBypass", MaxFailuresBeforeBypass },
                { "PageType", _associatedPage?.GetType().Name ?? "NULL" },
                { "PageHash", _associatedPage?.GetHashCode() ?? 0 },
                { "HasNavBar", NavBar != null },
                { "HasLoadDataCommand", LoadDataCommand != null },
                { "LoadDataCommandCanExecute", LoadDataCommand?.CanExecute(null) ?? false },
                { "UseGlobalLoading", UseGlobalLoading }
            };
        }

        #endregion
    }
}