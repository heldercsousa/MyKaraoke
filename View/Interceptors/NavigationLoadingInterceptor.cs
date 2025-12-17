using Microsoft.Maui.Controls;
using MyVocaList.View.Components;
using System;
using System.Threading.Tasks;

namespace MyVocaList.View.Interceptors
{
    /// <summary>
    /// ✅ INTERCEPTADOR: Mostra loading automaticamente em todas as navegações
    /// 🎯 AUTOMÁTICO: Sem necessidade de código manual nos behaviors
    /// 🔄 INTELIGENTE: Detecta tipo de navegação (Push, Pop, etc.)
    /// </summary>
    public static class NavigationLoadingInterceptor
    {
        #region Initialization

        private static bool _isInitialized = false;
        private static readonly object _initLock = new object();

        /// <summary>
        /// 🎯 INICIALIZAÇÃO: Chama uma vez no startup da aplicação
        /// </summary>
        public static void Initialize()
        {
            lock (_initLock)
            {
                if (_isInitialized)
                {
                    Console.WriteLine($"✅ NavigationInterceptor: Já inicializado - ignorando");
                    return;
                }

                try
                {
                    // 🎯 INTERCEPTA: Navigation Stack global
                    InterceptApplicationNavigation();

                    _isInitialized = true;
                    Console.WriteLine($"✅ NavigationInterceptor: Inicializado com sucesso");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ NavigationInterceptor: Erro na inicialização: {ex.Message}");
                }
            }
        }

        #endregion

        #region Application-Level Interception

        /// <summary>
        /// 🎯 INTERCEPTA: Navegação global da aplicação
        /// </summary>
        private static void InterceptApplicationNavigation()
        {
            try
            {
                // 🎯 HOOK: Application.Current.MainPage changes
                if (Application.Current != null)
                {
                    Application.Current.PropertyChanged += OnApplicationMainPageChanged;
                    Console.WriteLine($"🎯 NavigationInterceptor: Hook adicionado ao Application.Current");
                }

                Console.WriteLine($"✅ NavigationInterceptor: Hooks de aplicação configurados");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ NavigationInterceptor: Erro ao configurar hooks: {ex.Message}");
            }
        }

        /// <summary>
        /// 🎯 HOOK: Detecta mudanças na MainPage
        /// </summary>
        private static void OnApplicationMainPageChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Application.MainPage))
            {
                Console.WriteLine($"🎯 NavigationInterceptor: MainPage mudou - configurando interceptação");
                ConfigurePageInterception(Application.Current?.MainPage);
            }
        }

        #endregion

        #region Page-Level Interception

        /// <summary>
        /// 🎯 CONFIGURA: Interceptação para uma página específica
        /// </summary>
        private static void ConfigurePageInterception(Page page)
        {
            try
            {
                if (page == null) return;

                // 🎯 INTERCEPTA: NavigationPage
                if (page is NavigationPage navPage)
                {
                    InterceptNavigationPage(navPage);
                }

                // 🎯 INTERCEPTA: ContentPage diretamente
                else if (page is ContentPage contentPage)
                {
                    InterceptContentPage(contentPage);
                }

                // 🎯 INTERCEPTA: Shell
                else if (page is Shell shell)
                {
                    InterceptShell(shell);
                }

                Console.WriteLine($"✅ NavigationInterceptor: Configurado para {page.GetType().Name}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ NavigationInterceptor: Erro ao configurar página: {ex.Message}");
            }
        }

        /// <summary>
        /// 🎯 INTERCEPTA: NavigationPage específica
        /// </summary>
        private static void InterceptNavigationPage(NavigationPage navPage)
        {
            try
            {
                // 🎯 HOOK: Eventos de navegação
                navPage.Pushed += OnPagePushed;
                navPage.Popped += OnPagePopped;
                navPage.PoppedToRoot += OnPoppedToRoot;

                Console.WriteLine($"✅ NavigationInterceptor: NavigationPage interceptada");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ NavigationInterceptor: Erro ao interceptar NavigationPage: {ex.Message}");
            }
        }

        /// <summary>
        /// 🎯 INTERCEPTA: ContentPage individual
        /// </summary>
        private static void InterceptContentPage(ContentPage contentPage)
        {
            try
            {
                // 🎯 HOOK: Lifecycle events
                contentPage.Appearing += OnPageAppearing;
                contentPage.Disappearing += OnPageDisappearing;

                Console.WriteLine($"✅ NavigationInterceptor: ContentPage {contentPage.GetType().Name} interceptada");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ NavigationInterceptor: Erro ao interceptar ContentPage: {ex.Message}");
            }
        }

        /// <summary>
        /// 🎯 INTERCEPTA: Shell navigation
        /// </summary>
        private static void InterceptShell(Shell shell)
        {
            try
            {
                // 🎯 HOOK: Shell navigation events
                shell.Navigating += OnShellNavigating;
                shell.Navigated += OnShellNavigated;

                Console.WriteLine($"✅ NavigationInterceptor: Shell interceptado");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ NavigationInterceptor: Erro ao interceptar Shell: {ex.Message}");
            }
        }

        #endregion

        #region Navigation Event Handlers

        /// <summary>
        /// 🚀 PUSH: Page being added to navigation stack
        /// </summary>
        private static async void OnPagePushed(object sender, NavigationEventArgs e)
        {
            try
            {
                var targetPageName = e.Page?.GetType().Name ?? "Unknown";
                var requesterId = $"Navigation_Push_{targetPageName}_{DateTime.Now.Ticks}";

                Console.WriteLine($"🚀 NavigationInterceptor: PUSH to {targetPageName}");

                // ✅ CENTRALIZED SYSTEM: Request navigation loading with auto-hide
                await GlobalLoadingOverlay.Instance.RequestShowAsync(
                    requesterId: requesterId,
                    message: $"Navigating to {GetFriendlyPageName(targetPageName)}...",
                    priority: LoadingPriority.Navigation,
                    context: LoadingContext.PageNavigation,
                    isPersistent: false,
                    autoHideAfter: TimeSpan.FromSeconds(2) // Auto-hide for safety - reduced from 3s
                );

                // ⚡ OPTIMIZED: Minimal delay to prevent UI freeze (reduced from 300ms to 100ms)
                await Task.Delay(100);

                // Remove loading after minimal delay
                await GlobalLoadingOverlay.Instance.RequestHideAsync(requesterId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ NavigationInterceptor: Error in PUSH: {ex.Message}");
            }
        }

        /// <summary>
        /// 🔙 POP: Page being removed from stack
        /// </summary>
        private static async void OnPagePopped(object sender, NavigationEventArgs e)
        {
            try
            {
                var sourcePage = e.Page?.GetType().Name ?? "Unknown";
                var requesterId = $"Navigation_Pop_{sourcePage}_{DateTime.Now.Ticks}";

                Console.WriteLine($"🔙 NavigationInterceptor: POP from {sourcePage}");

                // ✅ CENTRALIZED SYSTEM: Request back navigation loading
                await GlobalLoadingOverlay.Instance.RequestShowAsync(
                    requesterId: requesterId,
                    message: "Going back...",
                    priority: LoadingPriority.Navigation,
                    context: LoadingContext.PageNavigation,
                    isPersistent: false,
                    autoHideAfter: TimeSpan.FromSeconds(1.5)
                );

                // ⚡ OPTIMIZED: Minimal delay (reduced from 200ms to 50ms)
                await Task.Delay(50);

                // Remove loading after minimal delay
                await GlobalLoadingOverlay.Instance.RequestHideAsync(requesterId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ NavigationInterceptor: Error in POP: {ex.Message}");
            }
        }

        /// <summary>
        /// 🏠 ROOT: Returning to root page
        /// </summary>
        private static async void OnPoppedToRoot(object sender, NavigationEventArgs e)
        {
            try
            {
                Console.WriteLine($"🏠 NavigationInterceptor: POP TO ROOT");

                await GlobalLoadingOverlay.ShowLoadingAsync("Returning to start...");
                await Task.Delay(100); // Reduced from 300ms
                await GlobalLoadingOverlay.HideLoadingAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ NavigationInterceptor: Error in POP TO ROOT: {ex.Message}");
                await GlobalLoadingOverlay.HideLoadingAsync();
            }
        }

        /// <summary>
        /// 📄 APPEARING: Página aparecendo (complementar)
        /// </summary>
        private static async void OnPageAppearing(object sender, EventArgs e)
        {
            try
            {
                if (sender is ContentPage page)
                {
                    var pageName = page.GetType().Name;
                    Console.WriteLine($"📄 NavigationInterceptor: APPEARING {pageName}");

                    // 🎯 LOADING: Apenas se não houver loading já ativo
                    // O SmartPageLifecycleBehavior pode já estar gerenciando
                    // Então aqui só interceptamos navegações diretas
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ NavigationInterceptor: Erro no APPEARING: {ex.Message}");
            }
        }

        /// <summary>
        /// 📄 DISAPPEARING: Página desaparecendo
        /// </summary>
        private static void OnPageDisappearing(object sender, EventArgs e)
        {
            try
            {
                if (sender is ContentPage page)
                {
                    var pageName = page.GetType().Name;
                    Console.WriteLine($"📄 NavigationInterceptor: DISAPPEARING {pageName}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ NavigationInterceptor: Erro no DISAPPEARING: {ex.Message}");
            }
        }

        /// <summary>
        /// 🚀 SHELL: Shell navigation starting
        /// </summary>
        private static async void OnShellNavigating(object sender, ShellNavigatingEventArgs e)
        {
            try
            {
                Console.WriteLine($"🚀 NavigationInterceptor: SHELL NAVIGATING to {e.Target}");
                await GlobalLoadingOverlay.ShowLoadingAsync("Navigating...");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ NavigationInterceptor: Error in SHELL NAVIGATING: {ex.Message}");
            }
        }

        /// <summary>
        /// ✅ SHELL: Shell navigation completed
        /// </summary>
        private static async void OnShellNavigated(object sender, ShellNavigatedEventArgs e)
        {
            try
            {
                Console.WriteLine($"✅ NavigationInterceptor: SHELL NAVIGATED to {e.Current}");
                await Task.Delay(50); // Reduced from 200ms
                await GlobalLoadingOverlay.HideLoadingAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ NavigationInterceptor: Error in SHELL NAVIGATED: {ex.Message}");
                await GlobalLoadingOverlay.HideLoadingAsync();
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// 🎯 HELPER: Convert technical page name to friendly name
        /// </summary>
        private static string GetFriendlyPageName(string technicalName)
        {
            return technicalName switch
            {
                "SpotPage" => "Venues",
                "SpotFormPage" => "Form",
                "PersonPage" => "Participants",
                "PersonFormPage" => "Registration",
                "StackPage" => "Queue",
                "TonguePage" => "Languages",
                "ConfigPage" => "Settings",
                "HistoryPage" => "History",
                _ => "page"
            };
        }

        /// <summary>
        /// 🛡️ CLEANUP: Remove todos os hooks (útil para testes)
        /// </summary>
        public static void Cleanup()
        {
            try
            {
                if (Application.Current != null)
                {
                    Application.Current.PropertyChanged -= OnApplicationMainPageChanged;
                }

                _isInitialized = false;
                Console.WriteLine($"🧹 NavigationInterceptor: Cleanup realizado");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ NavigationInterceptor: Erro no cleanup: {ex.Message}");
            }
        }

        #endregion

        #region Public Methods for Manual Control

        /// <summary>
        /// 🎯 MANUAL: Force loading for custom navigation
        /// </summary>
        public static async Task ShowNavigationLoadingAsync(string destinationPageName)
        {
            var requesterId = $"Manual_Navigation_{destinationPageName}_{DateTime.Now.Ticks}";
            var friendlyName = GetFriendlyPageName(destinationPageName);

            await GlobalLoadingOverlay.Instance.RequestShowAsync(
                requesterId: requesterId,
                message: $"Navigating to {friendlyName}...",
                priority: LoadingPriority.Navigation,
                context: LoadingContext.PageNavigation
            );
        }

        /// <summary>
        /// 🎯 MANUAL: Hide navigation loading
        /// </summary>
        public static async Task HideNavigationLoadingAsync()
        {
            // Clear all navigation requests
            await GlobalLoadingOverlay.Instance.ClearContextAsync(LoadingContext.PageNavigation);
        }

        #endregion
    }
}