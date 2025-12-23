using MyVocaList.View.Animations;
using MyVocaList.View.Components;
using MauiView = Microsoft.Maui.Controls.View;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MyVocaList.View.Behaviors
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

            // ✅ SELF-REGISTRATION: Register navbar with parent page automatically
            RegisterNavBarWithPage(bindable);

            // 🎯 SIMPLES: Usa sempre a página atual ativa
            var pageId = GetPageIdentifier(bindable);
            _ownerPageId = pageId;
            _robustAnimationManager = GlobalAnimationCoordinator.Instance.GetOrCreateManagerForPage(pageId);

            // ✅ APLICA ESTADO INICIAL automaticamente
            ApplyInitialState();

            // ✅ ADICIONA MÉTODOS ao objeto
            AddNavBarMethods();
        }

        protected override void OnDetachingFrom(Grid bindable)
        {
            base.OnDetachingFrom(bindable);

            // 🚀 MIGRAÇÃO: Dispose via GlobalAnimationCoordinator
            var pageId = GetPageIdentifier(bindable);

            _ = Task.Run(async () => await GlobalAnimationCoordinator.Instance.DisposeManagerForPage(pageId));

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
        }

        /// <summary>
        /// 🎯 SIMPLIFICADO: Sempre usa a página ativa atual do Application.Current
        /// </summary>
        private string GetPageIdentifier(VisualElement element)
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
                return pageId;
            }

            // 🛡️ FALLBACK: Se não conseguir obter página atual
            var fallbackId = $"{element.GetType().Name}_{element.GetHashCode()}";
            return fallbackId;
        }

        /// <summary>
        /// ✅ SELF-REGISTRATION: Automatically registers navbar with parent page
        /// 🎯 CENTRALIZED: All navbars get this for FREE via NavBarBehavior!
        /// </summary>
        private void RegisterNavBarWithPage(Grid grid)
        {
            // 🔍 Find parent navbar (ContentView that implements IAnimatableNavBar)
            var navbar = MyVocaList.View.Extensions.NavBarExtensions.FindParentOfType<ContentView>(grid);
            if (navbar is IAnimatableNavBar animatableNavBar)
            {
                // 🔍 Find parent page
                var page = MyVocaList.View.Extensions.NavBarExtensions.FindParentOfType<ContentPage>(navbar);
                if (page != null)
                {
                    MyVocaList.View.Extensions.NavBarExtensions.SetPageNavBar(page, animatableNavBar);
                }
            }
        }

        #endregion

        #region Estado Inicial e Estrutura

        /// <summary>
        /// Gets MD3 color from resources with fallback
        /// </summary>
        private Color GetMD3Color(string resourceKey, string fallbackHex)
        {
            if (Application.Current?.Resources != null &&
                Application.Current.Resources.TryGetValue(resourceKey, out var colorResource) &&
                colorResource is Color color)
            {
                return color;
            }

            return Color.FromArgb(fallbackHex);
        }
        private void ApplyInitialState()
        {
            if (_associatedGrid != null)
            {
                _associatedGrid.IsVisible = true;
                CreateNavBarStructure();
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
                BackgroundColor = GetMD3Color("Primary", "#7F41AC"),
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
            // Aplica estilo baseado na StyleKey
            if (Application.Current.Resources.TryGetValue(StyleKey, out var styleResource) && styleResource is Style style)
            {
                frame.Style = style;
            }
            else
            {
                // Fallback: estilo inline básico usando MD3 cores
                frame.BackgroundColor = Colors.Black;
                frame.BorderColor = GetMD3Color("Primary", "#7F41AC");
                frame.CornerRadius = 0;
                frame.Padding = 0;
                frame.HasShadow = false;
                frame.HeightRequest = 65;
                frame.VerticalOptions = LayoutOptions.End;
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
                    return;
                }

                // 🎯 NOVA PROTEÇÃO: Evita reconstrução durante ShowAsync
                if (behavior._isAnimating)
                {
                    return;
                }

                behavior.SmartRebuildButtons();
            }
        }


        /// <summary>
        /// 🛡️ PROTEÇÃO INTELIGENTE: Só reconstrói se realmente mudou
        /// 🎯 CORREÇÃO: Permite atualização durante animações quando necessário
        /// </summary>
        private void SmartRebuildButtons()
        {
            try
            {
                _isProcessingButtonsChange = true;

                // 🛡️ PROTEÇÃO 1: Calcula assinatura dos botões atuais
                var currentSignature = CalculateButtonsSignature();
                var currentColumnCount = CustomColumnDefinitions?.Count ?? (Buttons?.Count ?? 0);

                // 🎯 CORREÇÃO: Só ignora por animação se a assinatura for REALMENTE igual
                bool signatureChanged = _lastButtonsSignature != currentSignature || _lastColumnCount != currentColumnCount;

                if (_isAnimating && !signatureChanged)
                {
                    Console.WriteLine($"🎯 NavBarBehavior: SmartRebuildButtons IGNORADO - animação em progresso SEM mudança real");
                    return;
                }

                if (!_isAnimating || !signatureChanged)
                {
                    return;
                }

                // 🎯 NOVA VERIFICAÇÃO: Verifica se botões estão invisíveis mesmo com assinatura igual
                bool hasInvisibleButtons = _buttonViews.Any(b => !b.IsVisible || b.Opacity < 1.0);

                // 🛡️ PROTEÇÃO 2: Compara com cache apenas se NÃO há mudança real E botões estão visíveis
                if (_hasBeenInitialized &&
                    !signatureChanged &&
                    !hasInvisibleButtons &&
                    _buttonViews.Count > 0)
                {
                    return;
                }

                if (!signatureChanged && hasInvisibleButtons)
                {
                    _ = Task.Run(async () => await ShowAsync());
                    return;
                }

                // 🛡️ PROTEÇÃO 3: Atualiza cache ANTES de reconstruir
                _lastButtonsSignature = currentSignature;
                _lastColumnCount = currentColumnCount;

                // ✅ EXECUTA: Reconstrução real
                RebuildButtonsInternal();
            }
            catch
            {
                throw;
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
        /// 🎯 CORREÇÃO: Reseta _isShown quando cria novos botões
        /// </summary>
        private void RebuildButtonsInternal()
        {
            ClearButtons();

            if (Buttons == null || Buttons.Count == 0)
            {
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

                    // ... register manager ...

                    // ✅ FIX: Initialize based on the animation flag
                    if (DISABLE_NAVBAR_ANIMATIONS)
                    {
                        buttonView.Opacity = 1.0;
                        buttonView.TranslationY = 0;
                    }
                    else
                    {
                        buttonView.Opacity = 0.0;
                        buttonView.TranslationY = 60;
                    }

                    buttonView.IsVisible = true;
                }
            }

            // 🎯 CORREÇÃO: Quando novos botões são criados, reseta _isShown para forçar ShowAsync
            if (_buttonViews.Count > 0)
            {
                _isShown = false;
            }

			// AFTER the loop, once all buttons are added:

			// ✅ FORCE LAYOUT UPDATE
			if (_associatedGrid != null)
			{
				// This tells MAUI: "My content changed size, please re-calculate everything"
				if (_associatedGrid.Handler != null)
				{
					_associatedGrid.InvalidateMeasureNonVirtual(Microsoft.Maui.Controls.Internals.InvalidationTrigger.MeasureChanged);
				}
			}

			_hasBeenInitialized = true;
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
                    IconName = config.IconName,              // NEW: MD3 SVG icons support
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

        private void OnButtonClicked(NavButtonConfig config, object parameter) => ButtonClicked?.Invoke(this, new NavBarButtonClickedEventArgs(config, parameter));

        private Grid GetButtonsGrid()
        {
            if (_mainFrame?.Content is Grid wrapperGrid && wrapperGrid.Children.Count > 1)
            {
                return wrapperGrid.Children[1] as Grid;
            }
            return null;
        }

        #endregion

        #region Métodos de Animação - MIGRADOS PARA ROBUSTANIMATIONMANAGER
        public async Task ShowAsync()
        {
            var currentPageId = GetCurrentPageId();

            _ownerPageId = currentPageId;

            // 🎯 CORREÇÃO: Verifica se precisa mostrar novos botões mesmo se já "shown"
            bool hasNewButtons = _buttonViews.Count > 0 && _buttonViews.Any(b => b.Opacity < 1.0 || !b.IsVisible);

            if (_isShown && !_isAnimating && !hasNewButtons && _associatedGrid != null)
            {
                return;
            }

            if (_isAnimating || _associatedGrid == null)
            {
                return;
            }

            _isAnimating = true;

            try
            {
                _associatedGrid.IsVisible = true;

                if (_buttonViews.Count == 0 && Buttons != null && Buttons.Any())
                {
                    RebuildButtonsForced();
                }

                if (_buttonViews.Count == 0)
                {
                    _isShown = true;
                    return;
                }

                if (DISABLE_NAVBAR_ANIMATIONS)
                {
                    await ForceVisibleState();
                    _isShown = true;
                    return;
                }

                await ForceCorrectInitialState();

                _isShown = true;
            }
            catch
            {
                await ForceVisibleState();
                _isShown = true;
                throw;
            }
            finally
            {
                _isAnimating = false;
            }
        }

        private async Task ForceVisibleState() => await SetButtonsAsVisible(1,0);
        
        private async Task ForceCorrectInitialState() => await SetButtonsAsVisible(0,60);
      
        private async Task SetButtonsAsVisible(double opacity = 1.0, int translationY = 60)
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                foreach (var buttonView in _buttonViews)
                {
                    buttonView.IsVisible = true;
                    buttonView.Opacity = opacity;
                    buttonView.TranslationY = translationY;
                }
            });
        }

        private string GetCurrentPageId()
        {
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
                return pageId;
            }
            return "Unknown";
        }

        /// <summary>
        /// 🚀 MIGRAÇÃO: HideAsync com RobustAnimationManager
        /// </summary>
        public async Task HideAsync()
        {
            if (!_isShown || _associatedGrid == null)
                return;

            if (DISABLE_NAVBAR_ANIMATIONS)
            {
                _associatedGrid.IsVisible = false;
                _isShown = false;
                return;
            }

            try
            {
                if (_robustAnimationManager != null)
                {
                    await _robustAnimationManager.StopAllAnimationsCompletely();
                }

                await Task.Delay(50);

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
            }
            catch
            {
                _associatedGrid.IsVisible = false;
                _isShown = false;
                throw;
            }
        }

        /// <summary>
        /// 🚀 MIGRAÇÃO: StopAllAnimationsAsync via RobustAnimationManager
        /// </summary>
        private async Task StopAllAnimationsAsync()
        {
            if (DISABLE_NAVBAR_ANIMATIONS)
            {
                return;
            }

            // 🚀 MIGRAÇÃO: Para RobustAnimationManager primeiro
            if (_robustAnimationManager != null)
            {
                await _robustAnimationManager.StopAllAnimationsCompletely();
            }

            // ✅ CORREÇÃO 2: Para animações especiais dos botões
            await StopSpecialAnimations();

            // ✅ CORREÇÃO 3: Para animações individuais dos botões
            await StopButtonAnimations();
             
        }

        /// <summary>
        /// 🎯 CORREÇÃO: Para animações especiais de forma mais robusta
        /// </summary>
        private async Task StopSpecialAnimations()
        {
            if (DISABLE_NAVBAR_ANIMATIONS)
            {
                return;
            }

            var stopTasks = new List<Task>();

            foreach (var buttonView in _buttonViews.ToList())
            {
                if (buttonView is SpecialNavButtonComponent specialButton)
                {
                    stopTasks.Add(Task.Run(async () =>
                    {
                        var behavior = specialButton.Behaviors?.OfType<AnimatedButtonBehavior>().FirstOrDefault();
                        if (behavior != null)
                        {
                            await behavior.StopAllAnimationsAsync();
                        }
                        else
                        {
                            await specialButton.StopAllAnimationsAsync();
                        }
                       
                    }));
                }
                else if (buttonView is NavButtonComponent regularButton)
                {
                    stopTasks.Add(Task.Run(async () => await regularButton.StopAllAnimationsAsync()));
                }
            }

            if (stopTasks.Any())
            {
                var timeoutTask = Task.Delay(1000);
                var stopAllTask = Task.WhenAll(stopTasks);

                var completedTask = await Task.WhenAny(stopAllTask, timeoutTask);
            }
            
        }

        /// <summary>
        /// 🎯 NOVO: Para animações individuais dos botões
        /// </summary>
        private async Task StopButtonAnimations()
        {
            if (DISABLE_NAVBAR_ANIMATIONS)
            {
                return;
            }

            var stopTasks = new List<Task>();

            foreach (var buttonView in _buttonViews.ToList())
            {
                stopTasks.Add(Task.Run(async () => await AnimatedButtonExtensions.StopAllAnimationsAsync(buttonView as ContentView)));
            }

            if (stopTasks.Any())
            {
                await Task.WhenAll(stopTasks);
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
            _lastButtonsSignature = string.Empty;
            RebuildButtonsInternal();
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
            if (DISABLE_NAVBAR_ANIMATIONS)
            {
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
                navGrid.IsVisible = false;
                return;
            }

            var method = (Func<Task>)navGrid.GetValue(NavBarBehavior.HideAsyncMethodProperty);
            if (method != null)
                await method();
        }

        public static async Task StopAllAnimationsAsync(this Grid navGrid)
        {
            if (DISABLE_NAVBAR_ANIMATIONS)
            {
                return;
            }

            var method = (Func<Task>)navGrid.GetValue(NavBarBehavior.StopAllAnimationsAsyncMethodProperty);
            if (method != null)
                await method();
        }
    }

    #endregion
}