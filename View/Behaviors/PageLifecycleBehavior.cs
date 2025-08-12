using MyKaraoke.View.Components;
using System.Windows.Input;

namespace MyKaraoke.View.Behaviors
{
    /// <summary>
    /// Gerencia o ciclo de vida de uma ContentPage, orquestrando o carregamento
    /// de dados, a exibição de um indicador de loading e as animações de uma NavBar.
    /// 🛡️ PROTEÇÃO: Anti-múltiplas execuções centralizada
    /// </summary>
    public class PageLifecycleBehavior : Behavior<ContentPage>
    {
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

        // 🛡️ PROTEÇÃO: Logs de debug
        private readonly object _lockObject = new object();

        #endregion

        /// <summary>
        /// Anexa o behavior à página e se inscreve nos eventos de ciclo de vida.
        /// </summary>
        protected override void OnAttachedTo(ContentPage page)
        {
            base.OnAttachedTo(page);
            _associatedPage = page;
            _associatedPage.Appearing += OnPageAppearing;
            _associatedPage.Disappearing += OnPageDisappearing;

            System.Diagnostics.Debug.WriteLine($"🛡️ PageLifecycleBehavior: Anexado à {page.GetType().Name}");
        }

        /// <summary>
        /// Desanexa o behavior e cancela a inscrição nos eventos para evitar memory leaks.
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

                // 🛡️ PROTEÇÃO 4: Anima navbar com verificação final
                await ShowNavBarWithProtection();

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
                if (LoadDataCommand == null || !LoadDataCommand.CanExecute(null))
                {
                    System.Diagnostics.Debug.WriteLine("[PageLifecycleBehavior] LoadDataCommand não disponível ou não executável");
                    return;
                }

                SetLoadingState(true);

                try
                {
                    System.Diagnostics.Debug.WriteLine("[PageLifecycleBehavior] Executando LoadDataCommand");
                    // Executa o comando fornecido pela página.
                    await Task.Run(() => LoadDataCommand.Execute(null));
                    System.Diagnostics.Debug.WriteLine("[PageLifecycleBehavior] LoadDataCommand concluído");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[PageLifecycleBehavior] Erro ao executar LoadDataCommand: {ex.Message}");
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
        /// 🛡️ PROTEÇÃO: Mostra navbar com verificações extras
        /// </summary>
        private async Task ShowNavBarWithProtection()
        {
            if (NavBar == null)
            {
                System.Diagnostics.Debug.WriteLine("[PageLifecycleBehavior] NavBar nula - pulando animação");
                return;
            }

            try
            {
                System.Diagnostics.Debug.WriteLine("[PageLifecycleBehavior] Chamando NavBar.ShowAsync()");

                // 🛡️ PROTEÇÃO: Timeout para ShowAsync (evita travamento)
                var showTask = NavBar.ShowAsync();
                var timeoutTask = Task.Delay(5000); // 5 segundos máximo

                var completedTask = await Task.WhenAny(showTask, timeoutTask);

                if (completedTask == timeoutTask)
                {
                    System.Diagnostics.Debug.WriteLine("🛡️ [PageLifecycleBehavior] TIMEOUT ao mostrar navbar - continuando");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("[PageLifecycleBehavior] NavBar.ShowAsync() concluído");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"🛡️ [PageLifecycleBehavior] Erro ao mostrar navbar: {ex.Message}");
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