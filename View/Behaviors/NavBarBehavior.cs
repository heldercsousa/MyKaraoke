using MyKaraoke.View.Animations;
using MyKaraoke.View.Components;
using MauiView = Microsoft.Maui.Controls.View;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MyKaraoke.View.Behaviors
{
    /// <summary>
    /// ✅ BEHAVIOR: Substitui BaseNavBarComponent centralizando toda lógica de navbar
    /// 🛡️ PROTEÇÃO: Anti-dupla inicialização centralizada
    /// 🚀 MIGRADO: Usando RobustAnimationManager para eliminar crash pthread_mutex
    /// </summary>
    public class NavBarBehavior : Behavior<Grid>
    {
        #region Bindable Properties

        public static readonly BindableProperty ButtonsProperty =
            BindableProperty.Create(nameof(Buttons), typeof(ObservableCollection<NavButtonConfig>), typeof(NavBarBehavior), null, propertyChanged: OnButtonsChanged);

        public static readonly BindableProperty IsAnimatedProperty =
            BindableProperty.Create(nameof(IsAnimated), typeof(bool), typeof(NavBarBehavior), true);

        public static readonly BindableProperty ShowAnimationDelayProperty =
            BindableProperty.Create(nameof(ShowAnimationDelay), typeof(int), typeof(NavBarBehavior), 100);

        public static readonly BindableProperty CustomColumnDefinitionsProperty =
            BindableProperty.Create(nameof(CustomColumnDefinitions), typeof(ColumnDefinitionCollection), typeof(NavBarBehavior), null);

        public static readonly BindableProperty StyleKeyProperty =
            BindableProperty.Create(nameof(StyleKey), typeof(string), typeof(NavBarBehavior), "BaseBottomNavBarStyle");

        #endregion

        #region Properties

        public ObservableCollection<NavButtonConfig> Buttons
        {
            get => (ObservableCollection<NavButtonConfig>)GetValue(ButtonsProperty);
            set => SetValue(ButtonsProperty, value);
        }

        public bool IsAnimated
        {
            get => (bool)GetValue(IsAnimatedProperty);
            set => SetValue(IsAnimatedProperty, value);
        }

        public int ShowAnimationDelay
        {
            get => (int)GetValue(ShowAnimationDelayProperty);
            set => SetValue(ShowAnimationDelayProperty, value);
        }

        public ColumnDefinitionCollection CustomColumnDefinitions
        {
            get => (ColumnDefinitionCollection)GetValue(CustomColumnDefinitionsProperty);
            set => SetValue(CustomColumnDefinitionsProperty, value);
        }

        public string StyleKey
        {
            get => (string)GetValue(StyleKeyProperty);
            set => SetValue(StyleKeyProperty, value);
        }

        #endregion

        #region Events

        public event EventHandler<NavBarButtonClickedEventArgs> ButtonClicked;

        #endregion

        #region Private Fields

        private Grid _associatedGrid;
        // 🚀 MIGRAÇÃO: RobustAnimationManager em vez de AnimationManager
        private RobustAnimationManager _robustAnimationManager;
        private readonly List<MauiView> _buttonViews = new();
        private bool _isShown = false;
        private bool _isAnimating = false;
        private bool _hasBeenInitialized = false;
        private Frame _mainFrame;

        // 🛡️ PROTEÇÃO: Cache para detectar mudanças
        private string _lastButtonsSignature = string.Empty;
        private int _lastColumnCount = 0;
        private bool _isProcessingButtonsChange = false;

        // 🎯 CORREÇÃO: Adicionadas variáveis ausentes para controle de páginas
        private readonly object _pageOperationsLock = new object();
        private string _ownerPageId;
        private static readonly Dictionary<string, DateTime> _lastPageOperations = new Dictionary<string, DateTime>();
        private static readonly bool DISABLE_NAVBAR_ANIMATIONS = true;
        #endregion

        #region Behavior Lifecycle

        protected override void OnAttachedTo(Grid bindable)
        {
            base.OnAttachedTo(bindable);

            _associatedGrid = bindable;

            // 🎯 SIMPLES: Usa sempre a página atual ativa
            var pageId = GetPageIdentifier(bindable);
            _ownerPageId = pageId;
            _robustAnimationManager = GlobalAnimationCoordinator.Instance.GetOrCreateManagerForPage(pageId);

            // ✅ APLICA ESTADO INICIAL automaticamente
            ApplyInitialState();

            // ✅ ADICIONA MÉTODOS ao objeto
            AddNavBarMethods();

            System.Diagnostics.Debug.WriteLine($"🚀 NavBarBehavior anexado com ID: {pageId}");
        }

        protected override void OnDetachingFrom(Grid bindable)
        {
            base.OnDetachingFrom(bindable);

            // 🚀 MIGRAÇÃO: Dispose via GlobalAnimationCoordinator
            var pageId = GetPageIdentifier(bindable);
            _ = Task.Run(async () =>
            {
                try
                {
                    await GlobalAnimationCoordinator.Instance.DisposeManagerForPage(pageId);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"🛡️ Erro ao disposed RobustAnimationManager: {ex.Message}");
                }
            });

            // 🎯 LIMPEZA: Remove página do controle de operações
            lock (_pageOperationsLock)
            {
                if (_lastPageOperations.ContainsKey(pageId))
                {
                    _lastPageOperations.Remove(pageId);
                }
            }

            _associatedGrid = null;
            _ownerPageId = null;

            System.Diagnostics.Debug.WriteLine($"🚀 NavBarBehavior removido de {bindable.GetType().Name}");
        }

        /// <summary>
        /// 🎯 SIMPLIFICADO: Sempre usa a página ativa atual do Application.Current
        /// </summary>
        private string GetPageIdentifier(VisualElement element)
        {
            try
            {
                // 🎯 ESTRATÉGIA: Sempre usa a página atual ativa
                var currentPage = Application.Current?.MainPage;

                if (currentPage is NavigationPage navPage && navPage.CurrentPage != null)
                {
                    currentPage = navPage.CurrentPage;
                }
                else if (currentPage is Shell shell && shell.CurrentPage != null)
                {
                    currentPage = shell.CurrentPage;
                }

                if (currentPage != null)
                {
                    var pageId = $"{currentPage.GetType().Name}_{currentPage.GetHashCode()}";
                    System.Diagnostics.Debug.WriteLine($"🎯 NavBarBehavior: GetPageIdentifier (atual) = {pageId}");
                    return pageId;
                }

                // 🛡️ FALLBACK: Se não conseguir obter página atual
                var fallbackId = $"{element.GetType().Name}_{element.GetHashCode()}";
                System.Diagnostics.Debug.WriteLine($"🛡️ NavBarBehavior: GetPageIdentifier FALLBACK = {fallbackId}");
                return fallbackId;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erro ao obter identificador da página: {ex.Message}");
                return $"Error_{DateTime.Now.Ticks}";
            }
        }

        #endregion

        #region Estado Inicial e Estrutura

        private void ApplyInitialState()
        {
            try
            {
                if (_associatedGrid != null)
                {
                    _associatedGrid.IsVisible = true;
                    CreateNavBarStructure();

                    System.Diagnostics.Debug.WriteLine($"NavBarBehavior: Estado inicial aplicado");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao aplicar estado inicial: {ex.Message}");
            }
        }

        /// <summary>
        /// ✅ CRIA ESTRUTURA: Frame + BoxView + Grid igual ao BaseNavBarComponent.xaml
        /// </summary>
        private void CreateNavBarStructure()
        {
            if (_mainFrame != null) return; // Já criado

            // Frame principal
            _mainFrame = new Frame();
            ApplyFrameStyle(_mainFrame);

            // Grid wrapper (linha separadora + grid principal)
            var wrapperGrid = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition { Height = new GridLength(1) },
                    new RowDefinition { Height = GridLength.Star }
                }
            };

            // Linha separadora
            var separator = new BoxView
            {
                BackgroundColor = Color.FromArgb("#533682"),
                HeightRequest = 1,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Start
            };
            Grid.SetRow(separator, 0);

            // Grid principal para botões
            var buttonsGrid = new Grid
            {
                Padding = new Thickness(5, 2, 5, 2),
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill
            };
            Grid.SetRow(buttonsGrid, 1);

            wrapperGrid.Children.Add(separator);
            wrapperGrid.Children.Add(buttonsGrid);
            _mainFrame.Content = wrapperGrid;

            _associatedGrid.Children.Clear();
            _associatedGrid.Children.Add(_mainFrame);
        }

        private void ApplyFrameStyle(Frame frame)
        {
            try
            {
                // Aplica estilo baseado na StyleKey
                if (Application.Current.Resources.TryGetValue(StyleKey, out var styleResource) && styleResource is Style style)
                {
                    frame.Style = style;
                }
                else
                {
                    // Fallback: estilo inline básico
                    frame.BackgroundColor = Colors.Black;
                    frame.BorderColor = Color.FromArgb("#533682");
                    frame.CornerRadius = 0;
                    frame.Padding = 0;
                    frame.HasShadow = false;
                    frame.HeightRequest = 65;
                    frame.VerticalOptions = LayoutOptions.End;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao aplicar estilo do frame: {ex.Message}");
            }
        }

        #endregion

        #region Adicionar Métodos ao Objeto

        /// <summary>
        /// ✅ MAGIA: Adiciona métodos dinâmicos ao Grid
        /// </summary>
        private void AddNavBarMethods()
        {
            if (_associatedGrid == null) return;

            _associatedGrid.SetValue(ShowAsyncMethodProperty, new Func<Task>(ShowAsync));
            _associatedGrid.SetValue(HideAsyncMethodProperty, new Func<Task>(HideAsync));
            _associatedGrid.SetValue(StopAllAnimationsAsyncMethodProperty, new Func<Task>(StopAllAnimationsAsync));
        }

        #endregion

        #region Attached Properties para Métodos

        public static readonly BindableProperty ShowAsyncMethodProperty =
            BindableProperty.CreateAttached("ShowAsyncMethod", typeof(Func<Task>), typeof(NavBarBehavior), null);

        public static readonly BindableProperty HideAsyncMethodProperty =
            BindableProperty.CreateAttached("HideAsyncMethod", typeof(Func<Task>), typeof(NavBarBehavior), null);

        public static readonly BindableProperty StopAllAnimationsAsyncMethodProperty =
            BindableProperty.CreateAttached("StopAllAnimationsAsyncMethod", typeof(Func<Task>), typeof(NavBarBehavior), null);

        #endregion

        #region 🛡️ PROTEÇÃO CENTRALIZADA - Button Management

        private static void OnButtonsChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is NavBarBehavior behavior)
            {
                // 🛡️ PROTEÇÃO: Evita processamento simultâneo
                if (behavior._isProcessingButtonsChange)
                {
                    System.Diagnostics.Debug.WriteLine("🛡️ NavBarBehavior: OnButtonsChanged IGNORADO - já processando");
                    return;
                }

                // 🎯 NOVA PROTEÇÃO: Evita reconstrução durante ShowAsync
                if (behavior._isAnimating)
                {
                    System.Diagnostics.Debug.WriteLine("🎯 NavBarBehavior: OnButtonsChanged IGNORADO - animação em progresso");
                    return;
                }

                behavior.SmartRebuildButtons();
            }
        }

        /// <summary>
        /// 🛡️ PROTEÇÃO INTELIGENTE: Só reconstrói se realmente mudou
        /// 🎯 NOVA PROTEÇÃO: Não reconstrói durante animações
        /// </summary>
        private void SmartRebuildButtons()
        {
            try
            {
                _isProcessingButtonsChange = true;

                // 🎯 PROTEÇÃO ADICIONAL: Não reconstrói durante ShowAsync
                if (_isAnimating)
                {
                    System.Diagnostics.Debug.WriteLine($"🎯 NavBarBehavior: SmartRebuildButtons IGNORADO - animação em progresso");
                    return;
                }

                // 🛡️ PROTEÇÃO 1: Calcula assinatura dos botões atuais
                var currentSignature = CalculateButtonsSignature();
                var currentColumnCount = CustomColumnDefinitions?.Count ?? (Buttons?.Count ?? 0);

                // 🛡️ PROTEÇÃO 2: Compara com cache
                if (_hasBeenInitialized &&
                    _lastButtonsSignature == currentSignature &&
                    _lastColumnCount == currentColumnCount &&
                    _buttonViews.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"🛡️ NavBarBehavior: SmartRebuildButtons IGNORADO - assinatura inalterada ({currentSignature})");
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"🛡️ NavBarBehavior: SmartRebuildButtons EXECUTANDO - nova assinatura ({currentSignature})");

                // 🛡️ PROTEÇÃO 3: Atualiza cache ANTES de reconstruir
                _lastButtonsSignature = currentSignature;
                _lastColumnCount = currentColumnCount;

                // ✅ EXECUTA: Reconstrução real
                RebuildButtonsInternal();
            }
            finally
            {
                _isProcessingButtonsChange = false;
            }
        }

        /// <summary>
        /// 🛡️ ASSINATURA: Cria hash único baseado no conteúdo dos botões
        /// </summary>
        private string CalculateButtonsSignature()
        {
            if (Buttons == null || Buttons.Count == 0)
                return "EMPTY";

            var signature = new System.Text.StringBuilder();

            foreach (var button in Buttons)
            {
                signature.Append($"{button.Text}|{button.IconSource}|{button.IsSpecial}|{button.IsAnimated};");
            }

            // Inclui configuração de colunas na assinatura
            if (CustomColumnDefinitions != null)
            {
                signature.Append($"COLS:{CustomColumnDefinitions.Count}:");
                foreach (var col in CustomColumnDefinitions)
                {
                    signature.Append($"{col.Width.Value}{col.Width.GridUnitType};");
                }
            }

            return signature.ToString();
        }

        /// <summary>
        /// ✅ RECONSTRUÇÃO REAL: Lógica original sem proteções
        /// </summary>
        private void RebuildButtonsInternal()
        {
            try
            {
                ClearButtons();

                if (Buttons == null || Buttons.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("NavBarBehavior: Nenhum botão configurado");
                    return;
                }

                var buttonsGrid = GetButtonsGrid();
                if (buttonsGrid == null) return;

                SetupGridColumns(buttonsGrid, Buttons.Count);

                for (int i = 0; i < Buttons.Count; i++)
                {
                    var buttonConfig = Buttons[i];
                    var buttonView = CreateButtonView(buttonConfig, i);

                    if (buttonView != null)
                    {
                        Grid.SetColumn(buttonView, i);
                        buttonsGrid.Children.Add(buttonView);
                        _buttonViews.Add(buttonView);

                        // 🚀 MIGRAÇÃO: Registrar elemento no RobustAnimationManager
                        if (buttonView is VisualElement visualElement)
                        {
                            _robustAnimationManager?.RegisterAnimatedElement(visualElement);
                        }

                        // ✅ ESTADO INICIAL para animação
                        buttonView.Opacity = 0.0;
                        buttonView.TranslationY = 60;
                        buttonView.IsVisible = true;
                    }
                }

                _hasBeenInitialized = true;
                System.Diagnostics.Debug.WriteLine($"🚀 NavBarBehavior: {_buttonViews.Count} botões criados e registrados no RobustAnimationManager");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao reconstruir botões: {ex.Message}");
            }
        }

        private void ClearButtons()
        {
            var buttonsGrid = GetButtonsGrid();
            if (buttonsGrid != null)
            {
                buttonsGrid.Children.Clear();
                buttonsGrid.ColumnDefinitions.Clear();
            }
            _buttonViews.Clear();
        }

        private void SetupGridColumns(Grid buttonsGrid, int buttonCount)
        {
            buttonsGrid.ColumnDefinitions.Clear();

            if (CustomColumnDefinitions != null && CustomColumnDefinitions.Any())
            {
                foreach (var columnDef in CustomColumnDefinitions)
                {
                    buttonsGrid.ColumnDefinitions.Add(columnDef);
                }
            }
            else
            {
                for (int i = 0; i < buttonCount; i++)
                {
                    buttonsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
                }
            }
        }

        private MauiView CreateButtonView(NavButtonConfig config, int index)
        {
            try
            {
                MauiView buttonView;

                if (config.IsSpecial)
                {
                    buttonView = new SpecialNavButtonComponent
                    {
                        Text = config.Text,
                        CenterContent = config.CenterContent,
                        CenterIconSource = config.CenterIconSource,
                        Command = config.Command,
                        CommandParameter = config.CommandParameter,
                        GradientStyle = config.GradientStyle,
                        IsAnimated = IsAnimated && config.IsAnimated,
                        AnimationTypes = config.SpecialAnimationTypes,
                        ShowDelay = 0
                    };

                    if (buttonView is SpecialNavButtonComponent specialButton)
                    {
                        specialButton.ButtonClicked += (s, e) => OnButtonClicked(config, e.Parameter);
                    }
                }
                else
                {
                    buttonView = new NavButtonComponent
                    {
                        IconSource = config.IconSource,
                        Text = config.Text,
                        Command = config.Command,
                        CommandParameter = config.CommandParameter,
                        IsAnimated = IsAnimated && config.IsAnimated,
                        AnimationTypes = config.AnimationTypes,
                        ShowDelay = 0
                    };

                    if (buttonView is NavButtonComponent regularButton)
                    {
                        regularButton.ButtonClicked += (s, e) => OnButtonClicked(config, e.Parameter);
                    }
                }

                return buttonView;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao criar botão {index}: {ex.Message}");
                return null;
            }
        }

        private void OnButtonClicked(NavButtonConfig config, object parameter)
        {
            try
            {
                ButtonClicked?.Invoke(this, new NavBarButtonClickedEventArgs(config, parameter));
                System.Diagnostics.Debug.WriteLine($"NavBarBehavior: Botão '{config.Text}' clicado");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no evento de clique: {ex.Message}");
            }
        }

        private Grid GetButtonsGrid()
        {
            try
            {
                if (_mainFrame?.Content is Grid wrapperGrid && wrapperGrid.Children.Count > 1)
                {
                    return wrapperGrid.Children[1] as Grid;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao obter grid de botões: {ex.Message}");
            }
            return null;
        }

        #endregion

        #region Métodos de Animação - MIGRADOS PARA ROBUSTANIMATIONMANAGER
        public async Task ShowAsync()
        {
            // 🎯 CORREÇÃO: NavBarBehavior é reutilizável para qualquer página
            var currentPageId = GetCurrentPageId();
            System.Diagnostics.Debug.WriteLine($"🎯 NavBarBehavior: ShowAsync para página {currentPageId}");

            // 🎯 SEMPRE: Atualiza owner para a página atual
            _ownerPageId = currentPageId;
            System.Diagnostics.Debug.WriteLine($"🎯 NavBarBehavior: Owner confirmado como {_ownerPageId}");

            if (_isShown || _isAnimating || _associatedGrid == null)
            {
                System.Diagnostics.Debug.WriteLine($"NavBarBehavior: ShowAsync ignorado - _isShown={_isShown}, _isAnimating={_isAnimating}");
                return;
            }

            _isAnimating = true;

            try
            {
                System.Diagnostics.Debug.WriteLine($"🎯 NavBarBehavior: ShowAsync INICIADO para {_ownerPageId}");

                _associatedGrid.IsVisible = true;

                // 🎯 CORREÇÃO: Garante que os botões existem E estão no estado correto
                if (_buttonViews.Count == 0 && Buttons != null && Buttons.Any())
                {
                    System.Diagnostics.Debug.WriteLine("🎯 NavBarBehavior: Criando botões antes de mostrar");
                    RebuildButtonsForced();
                }

                if (_buttonViews.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("❌ NavBarBehavior: Nenhum botão para mostrar - abortando");
                    _isShown = true;
                    return;
                }

                // ✅ EARLY RETURN: Se animações desabilitadas, apenas torna visível sem animar
                if (DISABLE_NAVBAR_ANIMATIONS)
                {
                    System.Diagnostics.Debug.WriteLine("🚫 NavBarBehavior: Animações desabilitadas - aplicando estado final direto");
                    await ForceVisibleState();
                    _isShown = true;
                    System.Diagnostics.Debug.WriteLine($"🚫 NavBarBehavior: ShowAsync CONCLUÍDO SEM ANIMAÇÕES para {_ownerPageId}");
                    return;
                }

                // 🎯 FORÇA estado inicial correto
                await ForceCorrectInitialState();

                System.Diagnostics.Debug.WriteLine($"🎯 NavBarBehavior: Estado inicial corrigido - iniciando animações");
                System.Diagnostics.Debug.WriteLine($"🔍 HardwareDetector.SupportsAnimations = {HardwareDetector.SupportsAnimations}");
                System.Diagnostics.Debug.WriteLine($"🔍 IsAnimated = {IsAnimated}");
                System.Diagnostics.Debug.WriteLine($"🔍 _buttonViews.Count = {_buttonViews.Count}");

                if (IsAnimated && HardwareDetector.SupportsAnimations && _buttonViews.Any())
                {
                    var showTasks = new List<Task>();
                    for (int i = 0; i < _buttonViews.Count; i++)
                    {
                        var buttonView = _buttonViews[i];
                        var delay = (i + 1) * ShowAnimationDelay;

                        if (buttonView is NavButtonComponent regularButton)
                        {
                            showTasks.Add(DelayedShowButton(regularButton, delay));
                        }
                        else if (buttonView is SpecialNavButtonComponent specialButton)
                        {
                            showTasks.Add(DelayedShowSpecialButton(specialButton, delay));
                        }
                    }

                    // 🎯 AGUARDA todas as animações com timeout
                    var allAnimationsTask = Task.WhenAll(showTasks);
                    var timeoutTask = Task.Delay(3000);

                    var completedTask = await Task.WhenAny(allAnimationsTask, timeoutTask);

                    if (completedTask == timeoutTask)
                    {
                        System.Diagnostics.Debug.WriteLine("⚠️ NavBarBehavior: TIMEOUT nas animações - forçando estado final");
                        await ForceVisibleState();
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"✅ NavBarBehavior: Todas as animações concluídas");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"🎯 NavBarBehavior: Sem animações - aplicando estado final direto");
                    await ForceVisibleState();
                }

                _isShown = true;
                System.Diagnostics.Debug.WriteLine($"🚀 NavBarBehavior: ShowAsync CONCLUÍDO para {_ownerPageId} com {_buttonViews.Count} botões VISÍVEIS");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ NavBarBehavior: ERRO em ShowAsync: {ex.Message}");

                // Fallback: força estado visível mesmo com erro
                await ForceVisibleState();
                _isShown = true;
            }
            finally
            {
                _isAnimating = false;
            }
        }

        /// <summary>
        /// 🎯 NOVO: Força estado final visível dos botões - VERSÃO LIMPA
        /// </summary>
        private async Task ForceVisibleState()
        {
            try
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    foreach (var buttonView in _buttonViews)
                    {
                        buttonView.IsVisible = true;
                        buttonView.Opacity = 1.0;
                        buttonView.TranslationY = 0;

                        System.Diagnostics.Debug.WriteLine($"🎯 Button FINAL: {buttonView.GetType().Name}, Visible={buttonView.IsVisible}, Opacity={buttonView.Opacity}, Y={buttonView.TranslationY}");
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erro ao forçar estado visível: {ex.Message}");
            }
        }

        /// <summary>
        /// 🎯 NOVO: Força estado inicial correto dos botões
        /// </summary>
        private async Task ForceCorrectInitialState()
        {
            try
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    foreach (var buttonView in _buttonViews)
                    {
                        buttonView.IsVisible = true;
                        buttonView.Opacity = 0.0;
                        buttonView.TranslationY = 60;
                        System.Diagnostics.Debug.WriteLine($"🎯 NavBarBehavior: Estado inicial forçado - Opacity=0, TranslationY=60");
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erro ao forçar estado inicial: {ex.Message}");
            }
        }

       

        private string GetCurrentPageId()
        {
            try
            {
                // 🎯 CORREÇÃO: Usa mesma lógica do GetPageIdentifier para consistência
                var currentPage = Application.Current?.MainPage;

                if (currentPage is NavigationPage navPage && navPage.CurrentPage != null)
                {
                    currentPage = navPage.CurrentPage;
                }
                else if (currentPage is Shell shell && shell.CurrentPage != null)
                {
                    currentPage = shell.CurrentPage;
                }

                if (currentPage != null)
                {
                    var pageId = $"{currentPage.GetType().Name}_{currentPage.GetHashCode()}";
                    System.Diagnostics.Debug.WriteLine($"🎯 NavBarBehavior: GetCurrentPageId = {pageId}");
                    return pageId;
                }

                System.Diagnostics.Debug.WriteLine($"🛡️ NavBarBehavior: GetCurrentPageId = Unknown");
                return "Unknown";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erro ao obter página atual: {ex.Message}");
                return "Error";
            }
        }

        /// <summary>
        /// 🚀 MIGRAÇÃO: HideAsync com RobustAnimationManager
        /// </summary>
        public async Task HideAsync()
        {
            if (!_isShown || _associatedGrid == null)
                return;

            // ✅ EARLY RETURN: Se animações desabilitadas, apenas esconde sem animar
            if (DISABLE_NAVBAR_ANIMATIONS)
            {
                System.Diagnostics.Debug.WriteLine("🚫 NavBarBehavior: HideAsync - animações desabilitadas, escondendo direto");
                _associatedGrid.IsVisible = false;
                _isShown = false;
                System.Diagnostics.Debug.WriteLine("🚫 NavBarBehavior: HideAsync concluído SEM ANIMAÇÕES");
                return;
            }

            System.Diagnostics.Debug.WriteLine("🛑 NavBarBehavior: HideAsync - parando animações com RobustAnimationManager");

            try
            {
                // 🚀 MIGRAÇÃO: Para TODAS as animações via RobustAnimationManager
                if (_robustAnimationManager != null)
                {
                    await _robustAnimationManager.StopAllAnimationsCompletely();
                }

                // ✅ Pequeno delay para garantir que parou
                await Task.Delay(50);

                // ✅ ENTÃO esconde sem animações
                if (IsAnimated && HardwareDetector.SupportsAnimations)
                {
                    var hideTasks = new List<Task>();

                    foreach (var buttonView in _buttonViews)
                    {
                        if (buttonView is NavButtonComponent regularButton)
                        {
                            hideTasks.Add(regularButton.HideAsync());
                        }
                        else if (buttonView is SpecialNavButtonComponent specialButton)
                        {
                            hideTasks.Add(specialButton.HideAsync());
                        }
                    }

                    if (hideTasks.Any())
                    {
                        await Task.WhenAll(hideTasks);
                    }
                }

                _associatedGrid.IsVisible = false;
                _isShown = false;
                System.Diagnostics.Debug.WriteLine("🛑 NavBarBehavior: HideAsync concluído COMPLETAMENTE com RobustAnimationManager");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"🛑 Erro ao esconder: {ex.Message}");
                _associatedGrid.IsVisible = false;
                _isShown = false;
            }
        }

        /// <summary>
        /// 🚀 MIGRAÇÃO: StopAllAnimationsAsync via RobustAnimationManager
        /// </summary>
        private async Task StopAllAnimationsAsync()
        {
            // ✅ EARLY RETURN: Se animações desabilitadas, não precisa parar nada
            if (DISABLE_NAVBAR_ANIMATIONS)
            {
                System.Diagnostics.Debug.WriteLine("🚫 NavBarBehavior: StopAllAnimationsAsync - animações já desabilitadas");
                return;
            }

            System.Diagnostics.Debug.WriteLine("🛑 NavBarBehavior: StopAllAnimationsAsync via RobustAnimationManager");

            try
            {
                // 🚀 MIGRAÇÃO: Para RobustAnimationManager primeiro
                if (_robustAnimationManager != null)
                {
                    await _robustAnimationManager.StopAllAnimationsCompletely();
                }

                // ✅ CORREÇÃO 2: Para animações especiais dos botões
                await StopSpecialAnimations();

                // ✅ CORREÇÃO 3: Para animações individuais dos botões
                await StopButtonAnimations();

                System.Diagnostics.Debug.WriteLine("🛑 NavBarBehavior: TODAS as animações paradas via RobustAnimationManager");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"🛑 NavBarBehavior: Erro ao parar animações: {ex.Message}");
            }
        }

        /// <summary>
        /// 🎯 CORREÇÃO: Para animações especiais de forma mais robusta
        /// </summary>
        private async Task StopSpecialAnimations()
        {
            // ✅ EARLY RETURN: Se animações desabilitadas, não precisa parar nada
            if (DISABLE_NAVBAR_ANIMATIONS)
            {
                System.Diagnostics.Debug.WriteLine("🚫 NavBarBehavior: StopSpecialAnimations - animações já desabilitadas");
                return;
            }

            try
            {
                var stopTasks = new List<Task>();

                foreach (var buttonView in _buttonViews.ToList())
                {
                    if (buttonView is SpecialNavButtonComponent specialButton)
                    {
                        stopTasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                // 🎯 CORREÇÃO: Chama diretamente o AnimationManager do behavior
                                var behavior = specialButton.Behaviors?.OfType<AnimatedButtonBehavior>().FirstOrDefault();
                                if (behavior != null)
                                {
                                    // Para via behavior diretamente - acesso direto ao AnimationManager
                                    await behavior.StopAllAnimationsAsync();
                                    System.Diagnostics.Debug.WriteLine($"🛑 SpecialButton '{specialButton.Text}' parado via behavior direto");
                                }
                                else
                                {
                                    // Fallback: tenta parar via métodos tradicionais
                                    await specialButton.StopAllAnimationsAsync();
                                    System.Diagnostics.Debug.WriteLine($"🛑 SpecialButton '{specialButton.Text}' parado via fallback");
                                }
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"🛑 Erro ao parar SpecialButton: {ex.Message}");
                            }
                        }));
                    }
                    else if (buttonView is NavButtonComponent regularButton)
                    {
                        stopTasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                // Para botões regulares, usa método padrão
                                await regularButton.StopAllAnimationsAsync();
                                System.Diagnostics.Debug.WriteLine($"🛑 NavButton '{regularButton.Text}' parado");
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"🛑 Erro ao parar NavButton: {ex.Message}");
                            }
                        }));
                    }
                }

                if (stopTasks.Any())
                {
                    // ✅ Aguarda até 1 segundo para parar todas
                    var timeoutTask = Task.Delay(1000);
                    var stopAllTask = Task.WhenAll(stopTasks);

                    var completedTask = await Task.WhenAny(stopAllTask, timeoutTask);

                    if (completedTask == timeoutTask)
                    {
                        System.Diagnostics.Debug.WriteLine("🛑 NavBarBehavior: Timeout ao parar animações especiais - continuando");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"🛑 NavBarBehavior: {stopTasks.Count} animações especiais paradas");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"🛑 Erro ao parar animações especiais: {ex.Message}");
            }
        }

        /// <summary>
        /// 🎯 NOVO: Para animações individuais dos botões
        /// </summary>
        private async Task StopButtonAnimations()
        {
            // ✅ EARLY RETURN: Se animações desabilitadas, não precisa parar nada
            if (DISABLE_NAVBAR_ANIMATIONS)
            {
                System.Diagnostics.Debug.WriteLine("🚫 NavBarBehavior: StopButtonAnimations - animações já desabilitadas");
                return;
            }

            try
            {
                var stopTasks = new List<Task>();

                foreach (var buttonView in _buttonViews.ToList())
                {
                    stopTasks.Add(Task.Run(async () =>
                    {
                        try
                        {
                            // ✅ Para animações via Behavior Extension
                            await AnimatedButtonExtensions.StopAllAnimationsAsync(buttonView as ContentView);
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"🛑 Erro ao parar animação individual: {ex.Message}");
                        }
                    }));
                }

                if (stopTasks.Any())
                {
                    await Task.WhenAll(stopTasks);
                    System.Diagnostics.Debug.WriteLine($"🛑 NavBarBehavior: {stopTasks.Count} animações individuais paradas");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"🛑 Erro ao parar animações individuais: {ex.Message}");
            }
        }

        private async Task EnsureInitialStateForAllButtons()
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                foreach (var buttonView in _buttonViews)
                {
                    buttonView.Opacity = 0.0;
                    buttonView.TranslationY = 60;
                    buttonView.IsVisible = true;
                }
            });
        }

        private async Task DelayedShowButton(NavButtonComponent button, int delay)
        {
            await Task.Delay(delay);
            await button.ShowAsync();
        }

        private async Task DelayedShowSpecialButton(SpecialNavButtonComponent button, int delay)
        {
            await Task.Delay(delay);
            await button.ShowAsync();
            await button.StartSpecialAnimationAsync();
        }

        /// <summary>
        /// 🎯 NOVO: Força reconstrução de botões mesmo se já inicializado
        /// </summary>
        private void RebuildButtonsForced()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("NavBarBehavior: RebuildButtonsForced - forçando criação");

                // 🛡️ PROTEÇÃO: Atualiza assinatura para forçar reconstrução
                _lastButtonsSignature = string.Empty;

                RebuildButtonsInternal();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao forçar reconstrução de botões: {ex.Message}");
            }
        }

        #endregion
    }

    #region Extension Methods

    /// <summary>
    /// ✅ EXTENSIONS: Para facilitar chamada dos métodos nas páginas
    /// </summary>
    public static class NavBarExtensions
    {
        private static readonly bool DISABLE_NAVBAR_ANIMATIONS = true;
        public static async Task ShowAsync(this Grid navGrid)
        {
            // ✅ EARLY RETURN: Se animações desabilitadas, apenas verifica visibilidade
            if (DISABLE_NAVBAR_ANIMATIONS)
            {
                System.Diagnostics.Debug.WriteLine("🚫 NavBarExtensions: ShowAsync - animações desabilitadas");
                navGrid.IsVisible = true;
                return;
            }

            var method = (Func<Task>)navGrid.GetValue(NavBarBehavior.ShowAsyncMethodProperty);
            if (method != null)
                await method();
        }

        public static async Task HideAsync(this Grid navGrid)
        {
            // ✅ EARLY RETURN: Se animações desabilitadas, apenas esconde
            if (DISABLE_NAVBAR_ANIMATIONS)
            {
                System.Diagnostics.Debug.WriteLine("🚫 NavBarExtensions: HideAsync - animações desabilitadas");
                navGrid.IsVisible = false;
                return;
            }

            var method = (Func<Task>)navGrid.GetValue(NavBarBehavior.HideAsyncMethodProperty);
            if (method != null)
                await method();
        }

        public static async Task StopAllAnimationsAsync(this Grid navGrid)
        {
            // ✅ EARLY RETURN: Se animações desabilitadas, não precisa parar nada
            if (DISABLE_NAVBAR_ANIMATIONS)
            {
                System.Diagnostics.Debug.WriteLine("🚫 NavBarExtensions: StopAllAnimationsAsync - animações já desabilitadas");
                return;
            }

            var method = (Func<Task>)navGrid.GetValue(NavBarBehavior.StopAllAnimationsAsyncMethodProperty);
            if (method != null)
                await method();
        }
    }

    #endregion
}