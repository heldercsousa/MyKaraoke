using MyKaraoke.View.Components;
using System.Windows.Input;

namespace MyKaraoke.View.Behaviors
{
    /// <summary>
    /// Gerencia o ciclo de vida de uma ContentPage, orquestrando o carregamento
    /// de dados, a exibição de um indicador de loading e as animações de uma NavBar.
    /// 🛡️ PROTEÇÃO: Anti-múltiplas execuções centralizada
    /// 🔧 CORRIGIDO: Eliminado recarregamento desnecessário de navbar
    /// 🎯 REFINADO: Detecta mudança real de configuração da navbar
    /// 🚫 BLOQUEIO: Previne instâncias duplicadas de páginas
    /// ✅ RESTAURADO: Funcionalidades críticas da versão anterior
    /// </summary>
    public class PageLifecycleBehavior : Behavior<ContentPage>
    {
        #region 🚫 PROTEÇÃO GLOBAL: Controle de Instâncias Duplicadas (SIMPLIFICADO)

        private static readonly Dictionary<string, DateTime> _globalPageInstances = new Dictionary<string, DateTime>();
        private static readonly object _globalLock = new object();

        #endregion

        #region Bindable Properties

        /// <summary>
        /// A barra de navegação (que implementa IAnimatableNavBar) a ser animada.
        /// </summary>
        public static readonly BindableProperty NavBarProperty =
            BindableProperty.Create(nameof(NavBar), typeof(IAnimatableNavBar), typeof(PageLifecycleBehavior));

        /// <summary>
        /// O comando a ser executado para carregar os dados da página.
        /// </summary>
        public static readonly BindableProperty LoadDataCommandProperty =
            BindableProperty.Create(nameof(LoadDataCommand), typeof(ICommand), typeof(PageLifecycleBehavior));

        /// <summary>
        /// O elemento visual que serve como indicador de carregamento (ex: um overlay).
        /// </summary>
        public static readonly BindableProperty LoadingIndicatorProperty =
            BindableProperty.Create(nameof(LoadingIndicator), typeof(VisualElement), typeof(PageLifecycleBehavior));

        /// <summary>
        /// O container do conteúdo principal da página, que será escondido durante o carregamento.
        /// </summary>
        public static readonly BindableProperty MainContentProperty =
            BindableProperty.Create(nameof(MainContent), typeof(VisualElement), typeof(PageLifecycleBehavior));

        public IAnimatableNavBar NavBar { get => (IAnimatableNavBar)GetValue(NavBarProperty); set => SetValue(NavBarProperty, value); }
        public ICommand LoadDataCommand { get => (ICommand)GetValue(LoadDataCommandProperty); set => SetValue(LoadDataCommandProperty, value); }
        public VisualElement LoadingIndicator { get => (VisualElement)GetValue(LoadingIndicatorProperty); set => SetValue(LoadingIndicatorProperty, value); }
        public VisualElement MainContent { get => (VisualElement)GetValue(MainContentProperty); set => SetValue(MainContentProperty, value); }

        #endregion

        #region 🛡️ PROTEÇÃO: Campos de Controle de Estado

        private ContentPage _associatedPage;

        // 🛡️ PROTEÇÃO: Anti-múltiplas execuções
        private bool _isAppearingInProgress = false;
        private bool _isDisappearingInProgress = false;
        private bool _isLoadingDataInProgress = false;
        private bool _hasInitializedOnce = false;

        // 🛡️ PROTEÇÃO: Controle de navbar
        private bool _isNavBarReadyCheckInProgress = false;
        private DateTime _lastNavBarCheck = DateTime.MinValue;

        // 🎯 RESTAURADO: Controle anti-recarregamento inteligente da versão anterior
        private bool _navBarAlreadyShown = false;
        private string _lastPageId = string.Empty;
        private string _lastNavBarSignature = string.Empty; // RESTAURADO: Detecta mudança de configuração

        // 🛡️ PROTEÇÃO: Logs de debug
        private readonly object _lockObject = new object();

        #endregion

        /// <summary>
        /// Anexa o behavior à página e se inscreve nos eventos de ciclo de vida.
        /// 🚫 PROTEÇÃO SIMPLIFICADA: Previne instâncias duplicadas
        /// </summary>
        protected override void OnAttachedTo(ContentPage page)
        {
            base.OnAttachedTo(page);

            // 🚫 PROTEÇÃO SIMPLIFICADA: Verifica apenas se há instância muito recente
            var pageType = page.GetType().Name;
            var currentTime = DateTime.Now;

            lock (_globalLock)
            {
                if (_globalPageInstances.ContainsKey(pageType))
                {
                    var lastInstanceTime = _globalPageInstances[pageType];
                    var timeSinceLastInstance = currentTime - lastInstanceTime;

                    // 🎯 REDUZIDO: Cooldown menor (1 segundo) e só bloqueia se muito próximo
                    if (timeSinceLastInstance < TimeSpan.FromMilliseconds(800))
                    {
                        System.Diagnostics.Debug.WriteLine($"🚫 [PageLifecycleBehavior] INSTÂNCIA DUPLICADA: {pageType} - IGNORANDO (gap: {timeSinceLastInstance.TotalMilliseconds}ms)");
                        return; // NÃO anexa events para duplicatas muito próximas
                    }
                }

                // Sempre atualiza o timestamp da última instância
                _globalPageInstances[pageType] = currentTime;
                System.Diagnostics.Debug.WriteLine($"🚫 [PageLifecycleBehavior] Instância REGISTRADA: {pageType} em {currentTime:HH:mm:ss.fff}");
            }

            _associatedPage = page;
            _associatedPage.Appearing += OnPageAppearing;
            _associatedPage.Disappearing += OnPageDisappearing;

            // 🔧 Reset do controle anti-recarregamento para nova página
            var currentPageId = $"{page.GetType().Name}_{page.GetHashCode()}";
            if (_lastPageId != currentPageId)
            {
                _navBarAlreadyShown = false;
                _lastPageId = currentPageId;
                _lastNavBarSignature = string.Empty;
                System.Diagnostics.Debug.WriteLine($"🔧 [PageLifecycleBehavior] Nova página: {currentPageId} - resetando controle navbar");
            }

            System.Diagnostics.Debug.WriteLine($"🛡️ PageLifecycleBehavior: Anexado à {page.GetType().Name}");
        }

        /// <summary>
        /// Desanexa o behavior e cancela a inscrição nos eventos para evitar memory leaks.
        /// 🚫 LIMPEZA: Remove registro de instância global
        /// </summary>
        protected override void OnDetachingFrom(ContentPage page)
        {
            lock (_lockObject)
            {
                if (_associatedPage != null)
                {
                    _associatedPage.Appearing -= OnPageAppearing;
                    _associatedPage.Disappearing -= OnPageDisappearing;
                }

                // 🛡️ RESET: Limpa todos os flags de estado
                _isAppearingInProgress = false;
                _isDisappearingInProgress = false;
                _isLoadingDataInProgress = false;
                _hasInitializedOnce = false;
                _isNavBarReadyCheckInProgress = false;

                // 🔧 RESTAURADO: Reset controle anti-recarregamento
                _navBarAlreadyShown = false;
                _lastNavBarSignature = string.Empty;
            }

            // 🚫 LIMPEZA GLOBAL: Remove registro da instância
            var pageType = page.GetType().Name;
            lock (_globalLock)
            {
                if (_globalPageInstances.ContainsKey(pageType))
                {
                    _globalPageInstances.Remove(pageType);
                    System.Diagnostics.Debug.WriteLine($"🚫 [PageLifecycleBehavior] Registro de instância REMOVIDO: {pageType}");
                }
            }

            base.OnDetachingFrom(page);
            _associatedPage = null;

            System.Diagnostics.Debug.WriteLine($"🛡️ PageLifecycleBehavior: Removido de {page.GetType().Name}");
        }

        /// <summary>
        /// 🛡️ PROTEÇÃO: Orquestra as ações com controle anti-múltiplas execuções
        /// </summary>
        private async void OnPageAppearing(object sender, EventArgs e)
        {
            // 🛡️ PROTEÇÃO 1: Evita múltiplas execuções simultâneas
            lock (_lockObject)
            {
                if (_isAppearingInProgress)
                {
                    System.Diagnostics.Debug.WriteLine("🛡️ [PageLifecycleBehavior] OnPageAppearing IGNORADO - já em progresso");
                    return;
                }
                _isAppearingInProgress = true;
            }

            try
            {
                System.Diagnostics.Debug.WriteLine("[PageLifecycleBehavior] OnPageAppearing iniciado");

                // 🛡️ PROTEÇÃO 2: Aguarda navbar estar pronta PRIMEIRO
                await EnsureNavBarIsReadyWithProtection();

                // 🛡️ PROTEÇÃO 3: Carrega dados com controle de estado
                await ExecuteLoadDataWithProtection();

                // 🎯 RESTAURADO: Anima navbar com verificação inteligente da versão anterior
                await ShowNavBarWithIntelligentProtection();

                _hasInitializedOnce = true;
                System.Diagnostics.Debug.WriteLine("🛡️ [PageLifecycleBehavior] OnPageAppearing concluído com sucesso");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"🛡️ [PageLifecycleBehavior] Erro em OnPageAppearing: {ex.Message}");
            }
            finally
            {
                lock (_lockObject)
                {
                    _isAppearingInProgress = false;
                }
            }
        }

        /// <summary>
        /// 🛡️ PROTEÇÃO: Aguarda navbar com anti-loop e timeout
        /// </summary>
        private async Task EnsureNavBarIsReadyWithProtection()
        {
            if (NavBar == null)
            {
                System.Diagnostics.Debug.WriteLine("[PageLifecycleBehavior] Nenhuma navbar configurada");
                return;
            }

            // 🛡️ PROTEÇÃO: Evita verificações simultâneas
            lock (_lockObject)
            {
                if (_isNavBarReadyCheckInProgress)
                {
                    System.Diagnostics.Debug.WriteLine("🛡️ [PageLifecycleBehavior] EnsureNavBarIsReady IGNORADO - já verificando");
                    return;
                }
                _isNavBarReadyCheckInProgress = true;
            }

            try
            {
                System.Diagnostics.Debug.WriteLine("[PageLifecycleBehavior] Aguardando navbar estar pronta...");

                // 🛡️ PROTEÇÃO: Cache de verificação (não verifica mais que 1x por 500ms)
                if ((DateTime.Now - _lastNavBarCheck).TotalMilliseconds < 500)
                {
                    System.Diagnostics.Debug.WriteLine("🛡️ [PageLifecycleBehavior] NavBar verificada recentemente - pulando");
                    return;
                }

                // Aguarda até 3 segundos para navbar se configurar
                int attempts = 0;
                const int maxAttempts = 30; // 30 x 100ms = 3 segundos

                while (attempts < maxAttempts)
                {
                    await Task.Delay(100);
                    attempts++;

                    // 🛡️ VERIFICAÇÃO: Tenta verificar se navbar tem botões
                    if (await IsNavBarReadyWithProtection())
                    {
                        System.Diagnostics.Debug.WriteLine($"[PageLifecycleBehavior] ✅ Navbar pronta após {attempts} tentativas");
                        _lastNavBarCheck = DateTime.Now;
                        return;
                    }

                    System.Diagnostics.Debug.WriteLine($"[PageLifecycleBehavior] Tentativa {attempts}/{maxAttempts} aguardando navbar");
                }

                System.Diagnostics.Debug.WriteLine($"[PageLifecycleBehavior] ⚠️ Timeout aguardando navbar - continuando mesmo assim");
                _lastNavBarCheck = DateTime.Now;
            }
            finally
            {
                lock (_lockObject)
                {
                    _isNavBarReadyCheckInProgress = false;
                }
            }
        }

        /// <summary>
        /// 🛡️ PROTEÇÃO: Verifica navbar com try-catch robusto
        /// </summary>
        private async Task<bool> IsNavBarReadyWithProtection()
        {
            try
            {
                // 🛡️ ESTRATÉGIA: Tenta chamar ShowAsync e ver se funciona sem erro
                // Se NavBar tiver botões, ShowAsync não falhará

                // Para componentes que implementam IAnimatableNavBar via NavBarBehavior,
                // podemos verificar se tem conteúdo verificando propriedades do ContentView
                if (NavBar is ContentView navContentView)
                {
                    // Verifica se tem filhos (indica que botões foram criados)
                    return await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        try
                        {
                            // Se é um ContentView com conteúdo, verifica se tem elementos
                            if (navContentView.Content is Grid grid)
                            {
                                bool hasChildren = grid.Children.Count > 0;
                                if (hasChildren)
                                {
                                    System.Diagnostics.Debug.WriteLine($"🛡️ [PageLifecycleBehavior] NavBar pronta - {grid.Children.Count} filhos encontrados");
                                }
                                return hasChildren;
                            }

                            bool hasContent = navContentView.Content != null;
                            if (hasContent)
                            {
                                System.Diagnostics.Debug.WriteLine($"🛡️ [PageLifecycleBehavior] NavBar pronta - conteúdo {navContentView.Content.GetType().Name} encontrado");
                            }
                            return hasContent;
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"🛡️ [PageLifecycleBehavior] Erro ao verificar navbar: {ex.Message}");
                            return false;
                        }
                    });
                }

                // Para outros tipos de navbar, assume que está pronto após um delay
                System.Diagnostics.Debug.WriteLine("🛡️ [PageLifecycleBehavior] NavBar não é ContentView - assumindo pronta");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"🛡️ [PageLifecycleBehavior] Erro verificando navbar: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 🛡️ PROTEÇÃO: Executa carregamento com controle de estado
        /// </summary>
        private async Task ExecuteLoadDataWithProtection()
        {
            // 🛡️ PROTEÇÃO: Evita carregamento múltiplo
            lock (_lockObject)
            {
                if (_isLoadingDataInProgress)
                {
                    System.Diagnostics.Debug.WriteLine("🛡️ [PageLifecycleBehavior] ExecuteLoadData IGNORADO - já carregando");
                    return;
                }
                _isLoadingDataInProgress = true;
            }

            try
            {
                if (LoadDataCommand == null)
                {
                    System.Diagnostics.Debug.WriteLine("[PageLifecycleBehavior] LoadDataCommand é NULL");
                    return;
                }

                if (!LoadDataCommand.CanExecute(null))
                {
                    System.Diagnostics.Debug.WriteLine("[PageLifecycleBehavior] LoadDataCommand.CanExecute retornou FALSE");
                    return;
                }

                SetLoadingState(true);

                try
                {
                    System.Diagnostics.Debug.WriteLine("[PageLifecycleBehavior] ⚡ EXECUTANDO LoadDataCommand");

                    // 🔧 CORREÇÃO CRÍTICA: Detecta se é Command assíncrono e aguarda adequadamente
                    if (LoadDataCommand is Command asyncCommand)
                    {
                        // Para Command que encapsula operação async, executa diretamente
                        LoadDataCommand.Execute(null);

                        // 🛡️ AGUARDA um tempo para operação assíncrona interna completar
                        await Task.Delay(500);
                        System.Diagnostics.Debug.WriteLine("[PageLifecycleBehavior] ✅ Command executado (com delay para async)");
                    }
                    else
                    {
                        // Para outros tipos de comando, usa abordagem original
                        await Task.Run(() => LoadDataCommand.Execute(null));
                        System.Diagnostics.Debug.WriteLine("[PageLifecycleBehavior] ✅ Command executado (Task.Run)");
                    }

                    System.Diagnostics.Debug.WriteLine("[PageLifecycleBehavior] LoadDataCommand concluído");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[PageLifecycleBehavior] ❌ ERRO ao executar LoadDataCommand: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine($"[PageLifecycleBehavior] StackTrace: {ex.StackTrace}");
                }
                finally
                {
                    SetLoadingState(false);
                }
            }
            finally
            {
                lock (_lockObject)
                {
                    _isLoadingDataInProgress = false;
                }
            }
        }

        /// <summary>
        /// 🎯 RESTAURADO: Sistema inteligente de navbar da versão anterior
        /// </summary>
        private async Task ShowNavBarWithIntelligentProtection()
        {
            if (NavBar == null)
            {
                System.Diagnostics.Debug.WriteLine("[PageLifecycleBehavior] NavBar nula - pulando animação");
                return;
            }

            // 🎯 RESTAURADO: Lógica principal da versão anterior
            var currentNavBarSignature = CalculateNavBarSignature();

            // 🎯 RESTAURADO: Verificação de mudança real de configuração
            bool hasNavBarConfigurationChanged = _lastNavBarSignature != currentNavBarSignature;
            bool isFirstShowOnThisPage = !_navBarAlreadyShown;

            if (!isFirstShowOnThisPage && !hasNavBarConfigurationChanged)
            {
                System.Diagnostics.Debug.WriteLine($"🔧 [PageLifecycleBehavior] ⏭️ NavBar JÁ MOSTRADA - EVITANDO RECARREGAMENTO");
                System.Diagnostics.Debug.WriteLine($"🔧 [PageLifecycleBehavior] Atual: {currentNavBarSignature}");
                System.Diagnostics.Debug.WriteLine($"🔧 [PageLifecycleBehavior] Anterior: {_lastNavBarSignature}");
                return;
            }

            // 🎯 RESTAURADO: Exibição com detecção de mudança
            if (hasNavBarConfigurationChanged)
            {
                System.Diagnostics.Debug.WriteLine($"🎯 [PageLifecycleBehavior] 🔄 MUDANÇA DETECTADA - ShowAsync()");
                System.Diagnostics.Debug.WriteLine($"🎯 [PageLifecycleBehavior] De: {_lastNavBarSignature}");
                System.Diagnostics.Debug.WriteLine($"🎯 [PageLifecycleBehavior] Para: {currentNavBarSignature}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"🔧 [PageLifecycleBehavior] 🎬 PRIMEIRA EXIBIÇÃO - ShowAsync()");
            }

            try
            {
                // 🛡️ EXECUÇÃO: ShowAsync com timeout
                System.Diagnostics.Debug.WriteLine($"🎯 [PageLifecycleBehavior] ⚡ INICIANDO NavBar.ShowAsync()");

                var showTask = NavBar.ShowAsync();
                var timeoutTask = Task.Delay(5000);

                var completedTask = await Task.WhenAny(showTask, timeoutTask);

                if (completedTask == timeoutTask)
                {
                    System.Diagnostics.Debug.WriteLine("🛡️ [PageLifecycleBehavior] ⚠️ TIMEOUT ao mostrar navbar");
                }
                else
                {
                    // 🎯 RESTAURADO: Sucesso - atualiza cache
                    _navBarAlreadyShown = true;
                    _lastNavBarSignature = currentNavBarSignature;
                    System.Diagnostics.Debug.WriteLine($"🔧 [PageLifecycleBehavior] ✅ ShowAsync() CONCLUÍDO - cache atualizado");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"🛡️ [PageLifecycleBehavior] ❌ ERRO ao mostrar navbar: {ex.Message}");
            }
        }

        /// <summary>
        /// 🎯 RESTAURADO: Calcula assinatura única da navbar baseada na configuração atual
        /// </summary>
        private string CalculateNavBarSignature()
        {
            try
            {
                if (NavBar == null) return "NULL";

                // Para CrudNavBarComponent, verifica a configuração específica
                if (NavBar is CrudNavBarComponent crudNav)
                {
                    return $"CRUD_{crudNav.SelectionCount}_{crudNav.GetType().Name}";
                }

                // Para InactiveQueueBottomNav, verifica se tem botões visíveis
                if (NavBar is InactiveQueueBottomNav inactiveNav)
                {
                    return $"INACTIVE_{inactiveNav.IsVisible}_{inactiveNav.GetType().Name}";
                }

                // Para outros tipos, usa tipo + estado básico
                return $"{NavBar.GetType().Name}_{NavBar.GetHashCode()}";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"🛡️ [PageLifecycleBehavior] Erro ao calcular signature: {ex.Message}");
                return $"ERROR_{DateTime.Now.Ticks}";
            }
        }

        /// <summary>
        /// 🛡️ PROTEÇÃO: Orquestra saída com controle anti-múltiplas execuções
        /// </summary>
        private async void OnPageDisappearing(object sender, EventArgs e)
        {
            // 🛡️ PROTEÇÃO 1: Evita múltiplas execuções simultâneas
            lock (_lockObject)
            {
                if (_isDisappearingInProgress)
                {
                    System.Diagnostics.Debug.WriteLine("🛡️ [PageLifecycleBehavior] OnPageDisappearing IGNORADO - já em progresso");
                    return;
                }
                _isDisappearingInProgress = true;
            }

            try
            {
                System.Diagnostics.Debug.WriteLine("[PageLifecycleBehavior] OnPageDisappearing iniciado");

                // 🔧 RESTAURADO: Reset para próxima página da versão anterior
                _navBarAlreadyShown = false;
                _lastNavBarSignature = string.Empty; // Reset signature também
                System.Diagnostics.Debug.WriteLine("🔧 [PageLifecycleBehavior] Reset navbar state para próxima página");

                if (NavBar != null)
                {
                    try
                    {
                        // 🛡️ PROTEÇÃO: Timeout para HideAsync (evita travamento)
                        var hideTask = NavBar.HideAsync();
                        var timeoutTask = Task.Delay(3000); // 3 segundos máximo

                        var completedTask = await Task.WhenAny(hideTask, timeoutTask);

                        if (completedTask == timeoutTask)
                        {
                            System.Diagnostics.Debug.WriteLine("🛡️ [PageLifecycleBehavior] TIMEOUT ao esconder navbar - continuando");
                        }
                        else
                        {
                            // 🛡️ AGUARDA mais tempo para garantir que parou
                            await Task.Delay(100);
                            System.Diagnostics.Debug.WriteLine("[PageLifecycleBehavior] NavBar.HideAsync() concluído completamente");
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"🛡️ [PageLifecycleBehavior] Erro ao esconder navbar: {ex.Message}");
                    }
                }

                System.Diagnostics.Debug.WriteLine("🛡️ [PageLifecycleBehavior] OnPageDisappearing concluído");
            }
            finally
            {
                lock (_lockObject)
                {
                    _isDisappearingInProgress = false;
                }
            }
        }

        /// <summary>
        /// Controla a visibilidade do indicador de loading e do conteúdo principal.
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
                System.Diagnostics.Debug.WriteLine($"🛡️ [PageLifecycleBehavior] Erro ao definir loading state: {ex.Message}");
            }
        }
    }
}