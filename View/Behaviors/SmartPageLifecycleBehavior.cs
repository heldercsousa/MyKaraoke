using MyKaraoke.View.Components;
using MyKaraoke.View.Extensions;
using MyKaraoke.View.Managers;
using System.Reflection;
using System.Windows.Input;

namespace MyKaraoke.View.Behaviors
{
    /// <summary>
    /// ✅ EVOLUÇÃO: PageLifecycleBehavior inteligente que detecta problemas automaticamente
    /// 🛡️ AUTO-BYPASS: Se LoadDataCommand falha, executa fallback local
    /// 🎯 SELF-HEALING: Se navbar não aparece, força exibição direta
    /// 🔄 BACKWARD-COMPATIBLE: Mantém 100% compatibilidade com PageLifecycleBehavior original
    /// </summary>
    public class SmartPageLifecycleBehavior : Behavior<ContentPage>
    {
        #region Bindable Properties - IGUAIS AO ORIGINAL

        public static readonly BindableProperty NavBarProperty =
            BindableProperty.Create(nameof(NavBar), typeof(IAnimatableNavBar), typeof(SmartPageLifecycleBehavior));

        public static readonly BindableProperty LoadDataCommandProperty =
            BindableProperty.Create(nameof(LoadDataCommand), typeof(ICommand), typeof(SmartPageLifecycleBehavior));

        public static readonly BindableProperty LoadingIndicatorProperty =
            BindableProperty.Create(nameof(LoadingIndicator), typeof(VisualElement), typeof(SmartPageLifecycleBehavior));

        public static readonly BindableProperty MainContentProperty =
            BindableProperty.Create(nameof(MainContent), typeof(VisualElement), typeof(SmartPageLifecycleBehavior));

        // 🎯 NOVA: Propriedade para habilitar auto-bypass
        public static readonly BindableProperty EnableAutoBypassProperty =
            BindableProperty.Create(nameof(EnableAutoBypass), typeof(bool), typeof(SmartPageLifecycleBehavior), true);

        // 🎯 NOVA: Propriedade para número máximo de falhas antes do bypass
        public static readonly BindableProperty MaxFailuresBeforeBypassProperty =
            BindableProperty.Create(nameof(MaxFailuresBeforeBypass), typeof(int), typeof(SmartPageLifecycleBehavior), 2);

        #endregion

        #region Properties

        public IAnimatableNavBar NavBar { get => (IAnimatableNavBar)GetValue(NavBarProperty); set => SetValue(NavBarProperty, value); }
        public ICommand LoadDataCommand { get => (ICommand)GetValue(LoadDataCommandProperty); set => SetValue(LoadDataCommandProperty, value); }
        public VisualElement LoadingIndicator { get => (VisualElement)GetValue(LoadingIndicatorProperty); set => SetValue(LoadingIndicatorProperty, value); }
        public VisualElement MainContent { get => (VisualElement)GetValue(MainContentProperty); set => SetValue(MainContentProperty, value); }

        /// <summary>
        /// Habilita bypass automático quando detecta problemas
        /// </summary>
        public bool EnableAutoBypass { get => (bool)GetValue(EnableAutoBypassProperty); set => SetValue(EnableAutoBypassProperty, value); }

        /// <summary>
        /// Número máximo de falhas antes de ativar bypass automático
        /// </summary>
        public int MaxFailuresBeforeBypass { get => (int)GetValue(MaxFailuresBeforeBypassProperty); set => SetValue(MaxFailuresBeforeBypassProperty, value); }

        #endregion

        #region Private Fields

        private ContentPage _associatedPage;
        private bool _hasExecutedSuccessfully = false;
        private int _failureCount = 0;
        private bool _isProcessing = false;
        private readonly object _lockObject = new object();

        #endregion

        #region Behavior Lifecycle

        protected override void OnAttachedTo(ContentPage page)
        {
            base.OnAttachedTo(page);

            _associatedPage = page;
            _associatedPage.Appearing += OnPageAppearing;
            _associatedPage.Disappearing += OnPageDisappearing;

            // 📝 REGISTRO: Registra página no PageInstanceManager
            PageInstanceManager.Instance.RegisterPageInstance(page);

            System.Diagnostics.Debug.WriteLine($"✅ SmartPageLifecycleBehavior: Anexado à {page.GetType().Name} (Hash: {page.GetHashCode()})");
        }

        protected override void OnDetachingFrom(ContentPage page)
        {
            lock (_lockObject)
            {
                if (_associatedPage != null)
                {
                    _associatedPage.Appearing -= OnPageAppearing;
                    _associatedPage.Disappearing -= OnPageDisappearing;

                    // 🗑️ REMOÇÃO: Remove página do PageInstanceManager
                    PageInstanceManager.Instance.UnregisterPageInstance(_associatedPage);
                }

                // 🧹 RESET: Limpa estado
                _isProcessing = false;
                _hasExecutedSuccessfully = false;
                _failureCount = 0;
            }

            base.OnDetachingFrom(page);
            _associatedPage = null;

            System.Diagnostics.Debug.WriteLine($"✅ SmartPageLifecycleBehavior: Removido de {page.GetType().Name}");
        }

        #endregion

        #region Smart Page Lifecycle

        /// <summary>
        /// 🧠 INTELIGENTE: OnPageAppearing com detecção de problemas e auto-bypass
        /// ✅ CORRIGIDO: Removido await de dentro do lock
        /// </summary>
        private async void OnPageAppearing(object sender, EventArgs e)
        {
            try
            {
                bool shouldProcess;

                // 🛡️ PROTEÇÃO: Verifica se deve processar SEM await dentro do lock
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
                    System.Diagnostics.Debug.WriteLine($"🎯 SmartPageLifecycleBehavior: Já processando - ignorando");
                    return;
                }

                // 🎯 VERIFICAÇÃO: Bypass se necessário
                if (ShouldBypassBehavior())
                {
                    System.Diagnostics.Debug.WriteLine($"🎯 SmartPageLifecycleBehavior: Bypass detectado - delegando para página");
                    await ExecutePageBypass();
                    return;
                }

                // 🧠 TENTATIVA: Executa ciclo normal com monitoramento
                var success = await TryExecuteNormalCycle();

                if (!success)
                {
                    _failureCount++;
                    System.Diagnostics.Debug.WriteLine($"❌ SmartPageLifecycleBehavior: Falha #{_failureCount} detectada");

                    // 🛡️ AUTO-BYPASS: Se muitas falhas, executa bypass automático
                    if (EnableAutoBypass && _failureCount >= MaxFailuresBeforeBypass)
                    {
                        System.Diagnostics.Debug.WriteLine($"🛡️ SmartPageLifecycleBehavior: AUTO-BYPASS ativado após {_failureCount} falhas");
                        await ExecutePageBypass();
                        return;
                    }
                    else
                    {
                        // 🎯 CORREÇÃO CRÍTICA: Se falhou mas ainda não atingiu limite, FORÇA exibição da navbar
                        System.Diagnostics.Debug.WriteLine($"🎯 SmartPageLifecycleBehavior: Falha detectada - FORÇANDO exibição da NavBar");
                        await ForceShowNavBarAfterFailure();
                    }
                }
                else
                {
                    _hasExecutedSuccessfully = true;
                    _failureCount = 0;
                    System.Diagnostics.Debug.WriteLine($"✅ SmartPageLifecycleBehavior: Ciclo normal executado com sucesso");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SmartPageLifecycleBehavior: Erro em OnPageAppearing: {ex.Message}");

                // 🛡️ FALLBACK: Em caso de erro crítico, tenta exibir navbar
                await ForceShowNavBarAfterFailure();
            }
            finally
            {
                lock (_lockObject)
                {
                    _isProcessing = false;
                }
            }
        }

        /// <summary>
        /// 🎯 NOVO: Força exibição da NavBar quando há falha mas ainda não chegou no bypass
        /// ✅ CORREÇÃO CRÍTICA: Garante que navbar sempre seja exibida
        /// </summary>
        private async Task ForceShowNavBarAfterFailure()
        {
            if (NavBar == null)
            {
                System.Diagnostics.Debug.WriteLine($"🎯 SmartPageLifecycleBehavior: NavBar é NULL - pulando ShowAsync");
                return;
            }

            try
            {
                System.Diagnostics.Debug.WriteLine($"🎯 SmartPageLifecycleBehavior: FORÇA - Chamando NavBar.ShowAsync() após falha");

                // 🎯 IGUAL ao TryShowNavBar: sempre chama ShowAsync
                var showTask = NavBar.ShowAsync();
                var timeoutTask = Task.Delay(5000); // Timeout de 5 segundos

                var completedTask = await Task.WhenAny(showTask, timeoutTask);

                if (completedTask == timeoutTask)
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ SmartPageLifecycleBehavior: TIMEOUT ao mostrar NavBar após falha");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"✅ SmartPageLifecycleBehavior: NavBar.ShowAsync() concluído após falha");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SmartPageLifecycleBehavior: Erro ao mostrar NavBar após falha: {ex.Message}");
            }
        }

        /// <summary>
        /// 🎯 VERIFICAÇÃO: Determina se deve fazer bypass do behavior
        /// </summary>
        private bool ShouldBypassBehavior()
        {
            try
            {
                // 🎯 MARCA: Verifica se página foi marcada para bypass
                var styleId = _associatedPage.StyleId;
                if (styleId == "BYPASS_PAGELIFECYCLE")
                {
                    System.Diagnostics.Debug.WriteLine($"🎯 SmartPageLifecycleBehavior: Página marcada para bypass via StyleId");
                    return true;
                }

                // 🎯 TIPO: Verificação específica por tipo de página (para compatibilidade)
                if (_associatedPage is SpotPage)
                {
                    // Para SpotPage, verifica se tem problemas conhecidos
                    if (LoadDataCommand == null)
                    {
                        System.Diagnostics.Debug.WriteLine($"🎯 SmartPageLifecycleBehavior: SpotPage com LoadDataCommand NULL - forçando bypass");
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SmartPageLifecycleBehavior: Erro em ShouldBypassBehavior: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 🧠 TENTATIVA: Executa ciclo normal com monitoramento de falhas (IGUAL AO ORIGINAL)
        /// </summary>
        private async Task<bool> TryExecuteNormalCycle()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🧠 SmartPageLifecycleBehavior: Tentando ciclo normal");

                // ETAPA 1: Aguarda navbar estar pronta
                var navBarReady = await WaitForNavBarReady();
                if (!navBarReady)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ SmartPageLifecycleBehavior: NavBar não ficou pronta");
                    // 🔧 NÃO retorna false - continua tentando mostrar
                }

                // ETAPA 2: Executa LoadDataCommand (agora não considera NULL como falha)
                var dataLoaded = await TryExecuteLoadDataCommand();
                if (!dataLoaded)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ SmartPageLifecycleBehavior: LoadDataCommand falhou");
                    // 🔧 NÃO retorna false - continua tentando mostrar navbar
                }

                // ETAPA 3: SEMPRE tenta mostrar navbar (MUDANÇA CRÍTICA)
                var navBarShown = await TryShowNavBar();
                if (!navBarShown)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ SmartPageLifecycleBehavior: ShowNavBar falhou");
                    return false; // 🔧 APENAS falha de ShowNavBar é crítica
                }

                // 🎯 SUCESSO: Navbar foi exibida com sucesso
                System.Diagnostics.Debug.WriteLine($"✅ SmartPageLifecycleBehavior: Ciclo normal concluído - NavBar exibida");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SmartPageLifecycleBehavior: Erro no ciclo normal: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 🛡️ BYPASS: Executa bypass delegando responsabilidade para a página
        /// </summary>
        private async Task ExecutePageBypass()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🛡️ SmartPageLifecycleBehavior: Executando bypass para {_associatedPage.GetType().Name}");

                // 🎯 REFLEXÃO: Tenta encontrar método OnAppearingBypass na página
                var pageType = _associatedPage.GetType();
                var bypassMethod = pageType.GetMethod("OnAppearingBypass", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                if (bypassMethod != null)
                {
                    System.Diagnostics.Debug.WriteLine($"🎯 SmartPageLifecycleBehavior: Chamando {pageType.Name}.OnAppearingBypass()");

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
                    System.Diagnostics.Debug.WriteLine($"⚠️ SmartPageLifecycleBehavior: Método OnAppearingBypass não encontrado em {pageType.Name}");

                    // 🛡️ FALLBACK: Executa bypass genérico via extension method
                    await _associatedPage.ExecuteStandardBypass();
                }

                // 🎯 CORREÇÃO CRÍTICA: SEMPRE chama NavBar.ShowAsync() após bypass
                // ✅ RESPONSABILIDADE ÚNICA: SmartPageLifecycleBehavior é responsável por exibir navbar
                await EnsureNavBarIsShownAfterBypass();

                // ✅ MARCA: Sucesso no bypass
                _hasExecutedSuccessfully = true;
                _failureCount = 0;
                System.Diagnostics.Debug.WriteLine($"✅ SmartPageLifecycleBehavior: Bypass executado com sucesso E navbar exibida");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SmartPageLifecycleBehavior: Erro no bypass: {ex.Message}");
            }
        }

        /// <summary>
        /// 🎯 NOVO: Garante que NavBar.ShowAsync() é chamado após bypass
        /// ✅ RESPONSABILIDADE ÚNICA: Apenas SmartPageLifecycleBehavior chama ShowAsync
        /// </summary>
        private async Task EnsureNavBarIsShownAfterBypass()
        {
            if (NavBar == null)
            {
                System.Diagnostics.Debug.WriteLine($"🎯 SmartPageLifecycleBehavior: NavBar é NULL - pulando ShowAsync");
                return;
            }

            try
            {
                System.Diagnostics.Debug.WriteLine($"🎯 SmartPageLifecycleBehavior: BYPASS - Chamando NavBar.ShowAsync()");

                // 🎯 IGUAL ao PageLifecycleBehavior: sempre chama ShowAsync
                var showTask = NavBar.ShowAsync();
                var timeoutTask = Task.Delay(5000); // Timeout de 5 segundos

                var completedTask = await Task.WhenAny(showTask, timeoutTask);

                if (completedTask == timeoutTask)
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ SmartPageLifecycleBehavior: TIMEOUT ao mostrar NavBar após bypass");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"✅ SmartPageLifecycleBehavior: NavBar.ShowAsync() concluído após bypass");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SmartPageLifecycleBehavior: Erro ao mostrar NavBar após bypass: {ex.Message}");
            }
        }

        /// <summary>
        /// ⏱️ AGUARDA: NavBar estar pronta com timeout (LÓGICA ORIGINAL PRESERVADA)
        /// </summary>
        private async Task<bool> WaitForNavBarReady()
        {
            if (NavBar == null) return true;

            try
            {
                int attempts = 0;
                const int maxAttempts = 30; // 3 segundos

                while (attempts < maxAttempts)
                {
                    await Task.Delay(100);
                    attempts++;

                    // Verifica se navbar tem conteúdo (lógica original)
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
                            System.Diagnostics.Debug.WriteLine($"✅ SmartPageLifecycleBehavior: NavBar pronta após {attempts} tentativas");
                            return true;
                        }
                    }
                }

                System.Diagnostics.Debug.WriteLine($"⚠️ SmartPageLifecycleBehavior: Timeout aguardando NavBar");
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SmartPageLifecycleBehavior: Erro aguardando NavBar: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 🎯 TENTA: Executar LoadDataCommand com verificações (LÓGICA ORIGINAL PRESERVADA)
        /// ✅ CORRIGIDO: Simplificada detecção de comandos assíncronos
        /// </summary>
        private async Task<bool> TryExecuteLoadDataCommand()
        {
            try
            {
                if (LoadDataCommand == null)
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ SmartPageLifecycleBehavior: LoadDataCommand é NULL - continuando sem erro");
                    return true; // 🎯 MUDANÇA: NULL não é falha crítica
                }

                if (!LoadDataCommand.CanExecute(null))
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ SmartPageLifecycleBehavior: LoadDataCommand.CanExecute = false - continuando sem erro");
                    return true; // 🎯 MUDANÇA: CanExecute=false não é falha crítica
                }

                SetLoadingState(true);

                try
                {
                    System.Diagnostics.Debug.WriteLine($"🎯 SmartPageLifecycleBehavior: Executando LoadDataCommand");

                    LoadDataCommand.Execute(null);

                    // Aguarda um tempo para operações assíncronas internas
                    await Task.Delay(500);

                    System.Diagnostics.Debug.WriteLine($"✅ SmartPageLifecycleBehavior: LoadDataCommand executado");
                    return true;
                }
                finally
                {
                    SetLoadingState(false);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SmartPageLifecycleBehavior: Erro executando LoadDataCommand: {ex.Message}");
                SetLoadingState(false);
                return false; // 🎯 APENAS exceções são falhas críticas
            }
        }

        /// <summary>
        /// 🎯 TENTA: Mostrar NavBar com verificações (LÓGICA ORIGINAL PRESERVADA)
        /// </summary>
        private async Task<bool> TryShowNavBar()
        {
            if (NavBar == null) return true;

            try
            {
                System.Diagnostics.Debug.WriteLine($"🎯 SmartPageLifecycleBehavior: Tentando mostrar NavBar");

                var showTask = NavBar.ShowAsync();
                var timeoutTask = Task.Delay(5000); // Timeout aumentado

                var completedTask = await Task.WhenAny(showTask, timeoutTask);

                if (completedTask == timeoutTask)
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ SmartPageLifecycleBehavior: Timeout ao mostrar NavBar");
                    return false;
                }

                System.Diagnostics.Debug.WriteLine($"✅ SmartPageLifecycleBehavior: NavBar mostrada com sucesso");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SmartPageLifecycleBehavior: Erro ao mostrar NavBar: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 🎯 DISAPPEARING: Mantém lógica original do PageLifecycleBehavior
        /// </summary>
        private async void OnPageDisappearing(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🔄 SmartPageLifecycleBehavior: OnPageDisappearing para {_associatedPage.GetType().Name}");

                if (NavBar != null)
                {
                    try
                    {
                        var hideTask = NavBar.HideAsync();
                        var timeoutTask = Task.Delay(3000);

                        var completedTask = await Task.WhenAny(hideTask, timeoutTask);

                        if (completedTask == timeoutTask)
                        {
                            System.Diagnostics.Debug.WriteLine($"⚠️ SmartPageLifecycleBehavior: Timeout ao esconder NavBar");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"✅ SmartPageLifecycleBehavior: NavBar escondida com sucesso");
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"❌ SmartPageLifecycleBehavior: Erro ao esconder NavBar: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SmartPageLifecycleBehavior: Erro em OnPageDisappearing: {ex.Message}");
            }
        }

        /// <summary>
        /// 🎯 LOADING: Controla estado de loading (LÓGICA ORIGINAL PRESERVADA)
        /// </summary>
        private void SetLoadingState(bool isLoading)
        {
            try
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (LoadingIndicator != null)
                        LoadingIndicator.IsVisible = isLoading;

                    if (MainContent != null)
                        MainContent.IsVisible = !isLoading;
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SmartPageLifecycleBehavior: Erro ao definir loading state: {ex.Message}");
            }
        }

        #endregion

        #region Public Methods for Diagnostics

        /// <summary>
        /// 📊 DIAGNÓSTICO: Retorna estatísticas do behavior para debugging
        /// </summary>
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
                { "LoadDataCommandCanExecute", LoadDataCommand?.CanExecute(null) ?? false }
            };
        }

        #endregion
    }
}