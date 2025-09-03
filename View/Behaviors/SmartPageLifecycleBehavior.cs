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
        #region Bindable Properties - MANTIDAS + NOVAS

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

        // 🎯 NOVA: Propriedade para customizar mensagem de loading
        public static readonly BindableProperty LoadingMessageProperty =
            BindableProperty.Create(nameof(LoadingMessage), typeof(string), typeof(SmartPageLifecycleBehavior), "Carregando...");

        // 🎯 NOVA: Propriedade para habilitar loading automático
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

        /// <summary>
        /// 🎯 NOVA: Mensagem customizada do loading
        /// </summary>
        public string LoadingMessage { get => (string)GetValue(LoadingMessageProperty); set => SetValue(LoadingMessageProperty, value); }

        /// <summary>
        /// 🎯 NOVA: Habilita loading singleton automático (padrão: true)
        /// </summary>
        public bool UseGlobalLoading { get => (bool)GetValue(UseGlobalLoadingProperty); set => SetValue(UseGlobalLoadingProperty, value); }

        #endregion

        #region Private Fields - MANTIDOS

        private ContentPage _associatedPage;
        private bool _hasExecutedSuccessfully = false;
        private int _failureCount = 0;
        private bool _isProcessing = false;
        private readonly object _lockObject = new object();

        #endregion

        #region Behavior Lifecycle - MANTIDO

        protected override void OnAttachedTo(ContentPage page)
        {
            base.OnAttachedTo(page);

            _associatedPage = page;
            _associatedPage.Appearing += OnPageAppearing;
            _associatedPage.Disappearing += OnPageDisappearing;

            // 📝 REGISTRO: Mantido para compatibilidade (mesmo que PageInstanceManager seja removido)
            PageInstanceManager.Instance.RegisterPageInstance(page);

            System.Diagnostics.Debug.WriteLine($"✅ SmartPageLifecycleBehavior: Anexado à {page.GetType().Name} (Hash: {page.GetHashCode()}) - UseGlobalLoading: {UseGlobalLoading}");
        }

        protected override void OnDetachingFrom(ContentPage page)
        {
            lock (_lockObject)
            {
                if (_associatedPage != null)
                {
                    _associatedPage.Appearing -= OnPageAppearing;
                    _associatedPage.Disappearing -= OnPageDisappearing;

                    // 🗑️ REMOÇÃO: Mantido para compatibilidade
                    PageInstanceManager.Instance.UnregisterPageInstance(_associatedPage);
                }

                _isProcessing = false;
                _hasExecutedSuccessfully = false;
                _failureCount = 0;
            }

            base.OnDetachingFrom(page);
            _associatedPage = null;

            System.Diagnostics.Debug.WriteLine($"✅ SmartPageLifecycleBehavior: Removido de {page.GetType().Name}");
        }

        #endregion

        #region Smart Page Lifecycle - MELHORADO COM LOADING SINGLETON

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
                    System.Diagnostics.Debug.WriteLine($"🎯 SmartPageLifecycleBehavior: Já processando - ignorando");
                    return;
                }

                // 🎯 BYPASS: Verifica se deve fazer bypass
                if (ShouldBypassBehavior())
                {
                    System.Diagnostics.Debug.WriteLine($"🎯 SmartPageLifecycleBehavior: Bypass detectado - delegando para página");
                    await ExecutePageBypass();
                    return;
                }

                // 🧠 TENTATIVA: Executa ciclo normal com loading singleton
                var success = await TryExecuteNormalCycle();

                if (!success)
                {
                    _failureCount++;
                    System.Diagnostics.Debug.WriteLine($"❌ SmartPageLifecycleBehavior: Falha #{_failureCount} detectada");

                    if (EnableAutoBypass && _failureCount >= MaxFailuresBeforeBypass)
                    {
                        System.Diagnostics.Debug.WriteLine($"🛡️ SmartPageLifecycleBehavior: AUTO-BYPASS ativado após {_failureCount} falhas");
                        await ExecutePageBypass();
                        return;
                    }
                    else
                    {
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
        /// 🧠 TENTATIVA: Executa ciclo normal com loading singleton integrado
        /// </summary>
        private async Task<bool> TryExecuteNormalCycle()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🧠 SmartPageLifecycleBehavior: Tentando ciclo normal - UseGlobalLoading: {UseGlobalLoading}");

                // ETAPA 1: Aguarda navbar estar pronta
                var navBarReady = await WaitForNavBarReady();
                if (!navBarReady)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ SmartPageLifecycleBehavior: NavBar não ficou pronta");
                }

                // ETAPA 2: ✅ LOADING SINGLETON - Mostra loading se habilitado
                if (UseGlobalLoading)
                {
                    await GlobalLoadingOverlay.ShowLoadingAsync(LoadingMessage);
                    System.Diagnostics.Debug.WriteLine($"🔄 SmartPageLifecycleBehavior: Loading singleton EXIBIDO");
                }
                else
                {
                    // 🔄 FALLBACK: Usa LoadingIndicator tradicional se especificado
                    SetLoadingState(true);
                }

                try
                {
                    // ETAPA 3: Executa LoadDataCommand
                    var dataLoaded = await TryExecuteLoadDataCommand();
                    if (!dataLoaded)
                    {
                        System.Diagnostics.Debug.WriteLine($"❌ SmartPageLifecycleBehavior: LoadDataCommand falhou");
                    }

                    // ETAPA 4: Mostra navbar
                    var navBarShown = await TryShowNavBar();
                    if (!navBarShown)
                    {
                        System.Diagnostics.Debug.WriteLine($"❌ SmartPageLifecycleBehavior: ShowNavBar falhou");
                        // 🔧 NÃO retorna false - navbar pode aparecer depois
                    }

                    System.Diagnostics.Debug.WriteLine($"✅ SmartPageLifecycleBehavior: Ciclo normal concluído");
                    return true;
                }
                finally
                {
                    // ETAPA 5: ✅ LOADING SINGLETON - Esconde loading sempre
                    if (UseGlobalLoading)
                    {
                        await GlobalLoadingOverlay.HideLoadingAsync();
                        System.Diagnostics.Debug.WriteLine($"🔄 SmartPageLifecycleBehavior: Loading singleton ESCONDIDO");
                    }
                    else
                    {
                        // 🔄 FALLBACK: Esconde LoadingIndicator tradicional
                        SetLoadingState(false);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SmartPageLifecycleBehavior: Erro no ciclo normal: {ex.Message}");

                // 🔄 CLEANUP: Garante que loading seja escondido mesmo com erro
                if (UseGlobalLoading)
                {
                    await GlobalLoadingOverlay.HideLoadingAsync();
                }
                else
                {
                    SetLoadingState(false);
                }

                return false;
            }
        }

        /// <summary>
        /// 🛡️ BYPASS: Executa bypass com loading singleton
        /// </summary>
        private async Task ExecutePageBypass()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🛡️ SmartPageLifecycleBehavior: Executando bypass para {_associatedPage.GetType().Name}");

                // ✅ LOADING SINGLETON: Mostra durante bypass também
                if (UseGlobalLoading)
                {
                    await GlobalLoadingOverlay.ShowLoadingAsync($"Carregando {_associatedPage.GetType().Name}...");
                }

                try
                {
                    // ✅ CRÍTICO: SEMPRE executa LoadDataCommand primeiro (se disponível)
                    await TryExecuteLoadDataCommand();

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
                        await _associatedPage.ExecuteStandardBypass();
                    }

                    // 🎯 NAVBAR: Sempre chama NavBar.ShowAsync() após bypass
                    await EnsureNavBarIsShownAfterBypass();

                    _hasExecutedSuccessfully = true;
                    _failureCount = 0;
                    System.Diagnostics.Debug.WriteLine($"✅ SmartPageLifecycleBehavior: Bypass executado com sucesso");
                }
                finally
                {
                    // ✅ LOADING SINGLETON: Esconde após bypass
                    if (UseGlobalLoading)
                    {
                        await GlobalLoadingOverlay.HideLoadingAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SmartPageLifecycleBehavior: Erro no bypass: {ex.Message}");

                // 🔄 CLEANUP: Garante que loading seja escondido
                if (UseGlobalLoading)
                {
                    await GlobalLoadingOverlay.HideLoadingAsync();
                }
            }
        }

        #endregion

        #region Métodos Auxiliares - MANTIDOS COM ADAPTAÇÕES

        private bool ShouldBypassBehavior()
        {
            try
            {
                var styleId = _associatedPage.StyleId;
                if (styleId == "BYPASS_PAGELIFECYCLE")
                {
                    System.Diagnostics.Debug.WriteLine($"🎯 SmartPageLifecycleBehavior: Página marcada para bypass via StyleId");
                    return true;
                }

                if (_associatedPage is SpotPage)
                {
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

        private async Task ForceShowNavBarAfterFailure()
        {
            if (NavBar == null) return;

            try
            {
                System.Diagnostics.Debug.WriteLine($"🎯 SmartPageLifecycleBehavior: FORÇA - Chamando NavBar.ShowAsync() após falha");

                var showTask = NavBar.ShowAsync();
                var timeoutTask = Task.Delay(5000);
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

        private async Task<bool> WaitForNavBarReady()
        {
            if (NavBar == null) return true;

            try
            {
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

        private async Task<bool> TryExecuteLoadDataCommand()
        {
            try
            {
                ICommand commandToExecute = LoadDataCommand;

                if (commandToExecute == null && _associatedPage != null)
                {
                    System.Diagnostics.Debug.WriteLine($"🔍 SmartPageLifecycleBehavior: LoadDataCommand NULL - buscando via reflexão");

                    var pageType = _associatedPage.GetType();
                    var loadCommandProperty = pageType.GetProperty("LoadDataCommand");

                    if (loadCommandProperty != null)
                    {
                        commandToExecute = loadCommandProperty.GetValue(_associatedPage) as ICommand;
                        System.Diagnostics.Debug.WriteLine($"🔍 SmartPageLifecycleBehavior: LoadDataCommand encontrado via reflexão: {commandToExecute != null}");
                    }
                }

                if (commandToExecute == null)
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ SmartPageLifecycleBehavior: LoadDataCommand não encontrado - continuando sem erro");
                    return true;
                }

                if (!commandToExecute.CanExecute(null))
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ SmartPageLifecycleBehavior: LoadDataCommand.CanExecute = false - continuando sem erro");
                    return true;
                }

                try
                {
                    System.Diagnostics.Debug.WriteLine($"🎯 SmartPageLifecycleBehavior: Executando LoadDataCommand");
                    commandToExecute.Execute(null);
                    await Task.Delay(500); // Aguarda operações assíncronas internas
                    System.Diagnostics.Debug.WriteLine($"✅ SmartPageLifecycleBehavior: LoadDataCommand executado");
                    return true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ SmartPageLifecycleBehavior: Erro executando LoadDataCommand: {ex.Message}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SmartPageLifecycleBehavior: Erro em TryExecuteLoadDataCommand: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> TryShowNavBar()
        {
            if (NavBar == null) return true;

            try
            {
                System.Diagnostics.Debug.WriteLine($"🎯 SmartPageLifecycleBehavior: Tentando mostrar NavBar");

                var showTask = NavBar.ShowAsync();
                var timeoutTask = Task.Delay(5000);
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

        private async Task EnsureNavBarIsShownAfterBypass()
        {
            if (NavBar == null) return;

            try
            {
                System.Diagnostics.Debug.WriteLine($"🎯 SmartPageLifecycleBehavior: BYPASS - Chamando NavBar.ShowAsync()");

                var showTask = NavBar.ShowAsync();
                var timeoutTask = Task.Delay(5000);
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

        private async void OnPageDisappearing(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🔄 SmartPageLifecycleBehavior: OnPageDisappearing para {_associatedPage.GetType().Name}");

                // ✅ LOADING SINGLETON: Esconde loading se página está saindo
                if (UseGlobalLoading)
                {
                    await GlobalLoadingOverlay.HideLoadingAsync();
                }

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
        /// 🎯 LOADING: Controla estado de loading tradicional (FALLBACK)
        /// ⚠️ USADO: Apenas quando UseGlobalLoading=false
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

        #region Public Methods for Diagnostics - MANTIDOS

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
                { "UseGlobalLoading", UseGlobalLoading } // ✅ NOVO
            };
        }

        #endregion
    }
}