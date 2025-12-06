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
        /// 🚀 PUSH: Página sendo adicionada ao stack
        /// </summary>
        private static async void OnPagePushed(object sender, NavigationEventArgs e)
        {
            try
            {
                var targetPageName = e.Page?.GetType().Name ?? "Desconhecida";
                var requesterId = $"Navigation_Push_{targetPageName}_{DateTime.Now.Ticks}";

                Console.WriteLine($"🚀 NavigationInterceptor: PUSH para {targetPageName}");

                // ✅ SISTEMA CENTRALIZADO: Solicita loading de navegação
                await GlobalLoadingOverlay.Instance.RequestShowAsync(
                    requesterId: requesterId,
                    message: $"Navegando para {GetFriendlyPageName(targetPageName)}...",
                    priority: LoadingPriority.Navigation,
                    context: LoadingContext.PageNavigation,
                    isPersistent: false,
                    autoHideAfter: TimeSpan.FromSeconds(3) // Auto-hide por segurança
                );

                // 🕐 DELAY: Pequeno delay para garantir que loading apareça
                await Task.Delay(300);

                // Remove loading após delay
                await GlobalLoadingOverlay.Instance.RequestHideAsync(requesterId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ NavigationInterceptor: Erro no PUSH: {ex.Message}");
            }
        }

        /// <summary>
        /// 🔙 POP: Página sendo removida do stack
        /// </summary>
        private static async void OnPagePopped(object sender, NavigationEventArgs e)
        {
            try
            {
                var sourcePage = e.Page?.GetType().Name ?? "Desconhecida";
                var requesterId = $"Navigation_Pop_{sourcePage}_{DateTime.Now.Ticks}";

                Console.WriteLine($"🔙 NavigationInterceptor: POP de {sourcePage}");

                // ✅ SISTEMA CENTRALIZADO: Solicita loading de volta
                await GlobalLoadingOverlay.Instance.RequestShowAsync(
                    requesterId: requesterId,
                    message: "Voltando...",
                    priority: LoadingPriority.Navigation,
                    context: LoadingContext.PageNavigation,
                    isPersistent: false,
                    autoHideAfter: TimeSpan.FromSeconds(2)
                );

                // 🕐 DELAY: Menor delay para voltar
                await Task.Delay(200);

                // Remove loading após delay
                await GlobalLoadingOverlay.Instance.RequestHideAsync(requesterId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ NavigationInterceptor: Erro no POP: {ex.Message}");
            }
        }

        /// <summary>
        /// 🏠 ROOT: Voltando para página raiz
        /// </summary>
        private static async void OnPoppedToRoot(object sender, NavigationEventArgs e)
        {
            try
            {
                Console.WriteLine($"🏠 NavigationInterceptor: POP TO ROOT");

                await GlobalLoadingOverlay.ShowLoadingAsync("Voltando ao início...");
                await Task.Delay(300);
                await GlobalLoadingOverlay.HideLoadingAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ NavigationInterceptor: Erro no POP TO ROOT: {ex.Message}");
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
        /// 🚀 SHELL: Navegação Shell iniciando
        /// </summary>
        private static async void OnShellNavigating(object sender, ShellNavigatingEventArgs e)
        {
            try
            {
                Console.WriteLine($"🚀 NavigationInterceptor: SHELL NAVIGATING para {e.Target}");
                await GlobalLoadingOverlay.ShowLoadingAsync("Navegando...");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ NavigationInterceptor: Erro no SHELL NAVIGATING: {ex.Message}");
            }
        }

        /// <summary>
        /// ✅ SHELL: Navegação Shell concluída
        /// </summary>
        private static async void OnShellNavigated(object sender, ShellNavigatedEventArgs e)
        {
            try
            {
                Console.WriteLine($"✅ NavigationInterceptor: SHELL NAVIGATED para {e.Current}");
                await Task.Delay(200);
                await GlobalLoadingOverlay.HideLoadingAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ NavigationInterceptor: Erro no SHELL NAVIGATED: {ex.Message}");
                await GlobalLoadingOverlay.HideLoadingAsync();
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// 🎯 HELPER: Converte nome técnico da página em nome amigável
        /// </summary>
        private static string GetFriendlyPageName(string technicalName)
        {
            return technicalName switch
            {
                "SpotPage" => "Locais",
                "SpotFormPage" => "Formulário",
                "PersonPage" => "Participantes",
                "PersonFormPage" => "Cadastro",
                "StackPage" => "Fila",
                "TonguePage" => "Idiomas",
                "ConfigPage" => "Configurações",
                "HistoryPage" => "Histórico",
                _ => "página"
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
        /// 🎯 MANUAL: Força loading para navegação customizada
        /// </summary>
        public static async Task ShowNavigationLoadingAsync(string destinationPageName)
        {
            var requesterId = $"Manual_Navigation_{destinationPageName}_{DateTime.Now.Ticks}";
            var friendlyName = GetFriendlyPageName(destinationPageName);

            await GlobalLoadingOverlay.Instance.RequestShowAsync(
                requesterId: requesterId,
                message: $"Navegando para {friendlyName}...",
                priority: LoadingPriority.Navigation,
                context: LoadingContext.PageNavigation
            );
        }

        /// <summary>
        /// 🎯 MANUAL: Esconde loading de navegação
        /// </summary>
        public static async Task HideNavigationLoadingAsync()
        {
            // Limpa todas as requisições de navegação
            await GlobalLoadingOverlay.Instance.ClearContextAsync(LoadingContext.PageNavigation);
        }

        #endregion
    }
}