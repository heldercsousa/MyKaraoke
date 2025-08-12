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
        private AnimationManager _animationManager;
        private readonly List<MauiView> _buttonViews = new();
        private bool _isShown = false;
        private bool _isAnimating = false;
        private bool _hasBeenInitialized = false;
        private Frame _mainFrame;

        // 🛡️ PROTEÇÃO: Cache para detectar mudanças
        private string _lastButtonsSignature = string.Empty;
        private int _lastColumnCount = 0;
        private bool _isProcessingButtonsChange = false;

        #endregion

        #region Behavior Lifecycle

        protected override void OnAttachedTo(Grid bindable)
        {
            base.OnAttachedTo(bindable);

            _associatedGrid = bindable;
            _animationManager = new AnimationManager($"NavBar_{bindable.GetHashCode()}");

            // ✅ APLICA ESTADO INICIAL automaticamente
            ApplyInitialState();

            // ✅ ADICIONA MÉTODOS ao objeto
            AddNavBarMethods();

            System.Diagnostics.Debug.WriteLine($"NavBarBehavior anexado a {bindable.GetType().Name}");
        }

        protected override void OnDetachingFrom(Grid bindable)
        {
            base.OnDetachingFrom(bindable);

            // ✅ LIMPA RECURSOS
            _animationManager?.Dispose();
            _associatedGrid = null;

            System.Diagnostics.Debug.WriteLine($"NavBarBehavior removido de {bindable.GetType().Name}");
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

                behavior.SmartRebuildButtons();
            }
        }

        /// <summary>
        /// 🛡️ PROTEÇÃO INTELIGENTE: Só reconstrói se realmente mudou
        /// </summary>
        private void SmartRebuildButtons()
        {
            try
            {
                _isProcessingButtonsChange = true;

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

                        // ✅ ESTADO INICIAL para animação
                        buttonView.Opacity = 0.0;
                        buttonView.TranslationY = 60;
                        buttonView.IsVisible = true;
                    }
                }

                _hasBeenInitialized = true;
                System.Diagnostics.Debug.WriteLine($"NavBarBehavior: {_buttonViews.Count} botões criados");
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

        #region Métodos de Animação - MIGRADOS DO BASENAVBARCOMPONENT

        public async Task ShowAsync()
        {
            if (_isShown || _isAnimating || _associatedGrid == null)
            {
                System.Diagnostics.Debug.WriteLine("NavBarBehavior: ShowAsync ignorado");
                return;
            }

            _isAnimating = true;

            try
            {
                _associatedGrid.IsVisible = true;

                // 🎯 CORREÇÃO: Garante que os botões existem antes de animar
                if (_buttonViews.Count == 0 && Buttons != null && Buttons.Any())
                {
                    System.Diagnostics.Debug.WriteLine("NavBarBehavior: Criando botões antes de mostrar");
                    RebuildButtonsForced(); // Força criação mesmo se já inicializado
                }

                if (_buttonViews.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("NavBarBehavior: Nenhum botão para mostrar");
                    _isShown = true;
                    return;
                }

                await EnsureInitialStateForAllButtons();

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

                    await Task.WhenAll(showTasks);
                }
                else
                {
                    foreach (var buttonView in _buttonViews)
                    {
                        buttonView.IsVisible = true;
                        buttonView.Opacity = 1;
                        buttonView.TranslationY = 0;
                    }
                }

                _isShown = true;
                System.Diagnostics.Debug.WriteLine($"NavBarBehavior: ShowAsync concluído com {_buttonViews.Count} botões");
            }
            finally
            {
                _isAnimating = false;
            }
        }

        /// <summary>
        /// 🎯 CORREÇÃO: HideAsync com parada forçada
        /// </summary>
        public async Task HideAsync()
        {
            if (!_isShown || _associatedGrid == null)
                return;

            System.Diagnostics.Debug.WriteLine("🛑 NavBarBehavior: HideAsync - parando animações primeiro");

            try
            {
                // ✅ CORREÇÃO: Para TODAS as animações ANTES de esconder
                await StopAllAnimationsAsync();

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
                System.Diagnostics.Debug.WriteLine("🛑 NavBarBehavior: HideAsync concluído COMPLETAMENTE");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"🛑 Erro ao esconder: {ex.Message}");
                _associatedGrid.IsVisible = false;
                _isShown = false;
            }
        }

        /// <summary>
        /// 🎯 CORREÇÃO: Para TODAS as animações de forma mais robusta
        /// </summary>
        private async Task StopAllAnimationsAsync()
        {
            System.Diagnostics.Debug.WriteLine("🛑 NavBarBehavior: StopAllAnimationsAsync - PARANDO TODAS AS ANIMAÇÕES");

            try
            {
                // ✅ CORREÇÃO 1: Para AnimationManager primeiro
                if (_animationManager != null)
                {
                    await _animationManager.StopAllAnimationsAsync();
                }

                // ✅ CORREÇÃO 2: Para animações especiais dos botões
                await StopSpecialAnimations();

                // ✅ CORREÇÃO 3: Para animações individuais dos botões
                await StopButtonAnimations();

                System.Diagnostics.Debug.WriteLine("🛑 NavBarBehavior: TODAS as animações paradas");
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
        public static async Task ShowAsync(this Grid navGrid)
        {
            var method = (Func<Task>)navGrid.GetValue(NavBarBehavior.ShowAsyncMethodProperty);
            if (method != null)
                await method();
        }

        public static async Task HideAsync(this Grid navGrid)
        {
            var method = (Func<Task>)navGrid.GetValue(NavBarBehavior.HideAsyncMethodProperty);
            if (method != null)
                await method();
        }

        public static async Task StopAllAnimationsAsync(this Grid navGrid)
        {
            var method = (Func<Task>)navGrid.GetValue(NavBarBehavior.StopAllAnimationsAsyncMethodProperty);
            if (method != null)
                await method();
        }
    }

    #endregion
}