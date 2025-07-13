using Microsoft.Maui.Controls;
using MyKaraoke.View.Animations;
using System.Collections.ObjectModel;
using System.Windows.Input;
using MauiView = Microsoft.Maui.Controls.View;

namespace MyKaraoke.View.Components
{
    public partial class BaseNavBarComponent : ContentView
    {
        #region Bindable Properties

        public static readonly BindableProperty ButtonsProperty =
            BindableProperty.Create(nameof(Buttons), typeof(ObservableCollection<NavButtonConfig>), typeof(BaseNavBarComponent), null, propertyChanged: OnButtonsChanged);

        public static readonly BindableProperty IsAnimatedProperty =
            BindableProperty.Create(nameof(IsAnimated), typeof(bool), typeof(BaseNavBarComponent), true);

        public static readonly BindableProperty ShowAnimationDelayProperty =
            BindableProperty.Create(nameof(ShowAnimationDelay), typeof(int), typeof(BaseNavBarComponent), 100);

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

        #endregion

        #region Events

        public event EventHandler<NavBarButtonClickedEventArgs> ButtonClicked;

        #endregion

        #region Private Fields

        private AnimationManager _animationManager;
        private readonly List<MauiView> _buttonViews = new();
        private bool _isShown = false;

        #endregion

        public BaseNavBarComponent()
        {
            InitializeComponent();
            _animationManager = new AnimationManager($"BaseNavBar_{GetHashCode()}");

            // Inicializa lista vazia se não foi definida
            if (Buttons == null)
            {
                Buttons = new ObservableCollection<NavButtonConfig>();
            }
        }

        #region Property Changed Handlers

        private static void OnButtonsChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is BaseNavBarComponent navBar)
            {
                navBar.RebuildButtons();
            }
        }

        #endregion

        #region Button Management

        private void RebuildButtons()
        {
            try
            {
                // Limpa botões existentes
                ClearButtons();

                if (Buttons == null || Buttons.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("BaseNavBarComponent: Nenhum botão configurado");
                    return;
                }

                // Configura colunas do grid baseado na quantidade de botões
                SetupGridColumns(Buttons.Count);

                // Cria e adiciona botões
                for (int i = 0; i < Buttons.Count; i++)
                {
                    var buttonConfig = Buttons[i];
                    var buttonView = CreateButtonView(buttonConfig, i);

                    if (buttonView != null)
                    {
                        Grid.SetColumn(buttonView, i);
                        buttonsGrid.Children.Add(buttonView);
                        _buttonViews.Add(buttonView);
                    }
                }

                System.Diagnostics.Debug.WriteLine($"BaseNavBarComponent: {Buttons.Count} botões criados");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao reconstruir botões: {ex.Message}");
            }
        }

        private void ClearButtons()
        {
            try
            {
                buttonsGrid.Children.Clear();
                buttonsGrid.ColumnDefinitions.Clear();
                _buttonViews.Clear();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao limpar botões: {ex.Message}");
            }
        }

        private void SetupGridColumns(int buttonCount)
        {
            try
            {
                buttonsGrid.ColumnDefinitions.Clear();

                for (int i = 0; i < buttonCount; i++)
                {
                    buttonsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao configurar colunas do grid: {ex.Message}");
            }
        }

        private MauiView CreateButtonView(NavButtonConfig config, int index)
        {
            try
            {
                if (config.IsSpecial)
                {
                    return CreateSpecialButton(config, index);
                }
                else
                {
                    return CreateRegularButton(config, index);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao criar botão {index}: {ex.Message}");
                return null;
            }
        }

        private MauiView CreateRegularButton(NavButtonConfig config, int index)
        {
            var button = new NavButtonComponent
            {
                IconSource = config.IconSource,
                Text = config.Text,
                Command = config.Command,
                CommandParameter = config.CommandParameter,
                IsAnimated = IsAnimated && config.IsAnimated,
                AnimationTypes = config.AnimationTypes,
                ShowDelay = ShowAnimationDelay * index // Delay escalonado
            };

            // Conecta evento
            button.ButtonClicked += (s, e) => OnButtonClicked(config, e.Parameter);

            return button;
        }

        private MauiView CreateSpecialButton(NavButtonConfig config, int index)
        {
            var button = new SpecialNavButtonComponent
            {
                Text = config.Text,
                CenterContent = config.CenterContent,
                CenterIconSource = config.CenterIconSource,
                Command = config.Command,
                CommandParameter = config.CommandParameter,
                GradientStyle = config.GradientStyle,
                IsAnimated = IsAnimated && config.IsAnimated,
                AnimationTypes = config.SpecialAnimationTypes,
                ShowDelay = ShowAnimationDelay * index // Delay escalonado
            };

            // Conecta evento
            button.ButtonClicked += (s, e) => OnButtonClicked(config, e.Parameter);

            return button;
        }

        private void OnButtonClicked(NavButtonConfig config, object parameter)
        {
            try
            {
                // Dispara evento da navbar
                ButtonClicked?.Invoke(this, new NavBarButtonClickedEventArgs(config, parameter));

                System.Diagnostics.Debug.WriteLine($"BaseNavBarComponent: Botão '{config.Text}' clicado");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no evento de clique do botão: {ex.Message}");
            }
        }

        #endregion

        #region Animation Methods

        /// <summary>
        /// Mostra toda a navbar com animação escalonada dos botões
        /// Só executa se o hardware suportar animações
        /// </summary>
        public async Task ShowAsync()
        {
            if (_isShown)
                return;

            try
            {
                this.IsVisible = true;

                if (IsAnimated && HardwareDetector.SupportsAnimations)
                {
                    // Mostra botões com delay escalonado
                    var showTasks = new List<Task>();

                    foreach (var buttonView in _buttonViews)
                    {
                        if (buttonView is NavButtonComponent regularButton)
                        {
                            showTasks.Add(regularButton.ShowAsync());
                        }
                        else if (buttonView is SpecialNavButtonComponent specialButton)
                        {
                            showTasks.Add(specialButton.ShowAsync());
                        }
                    }

                    await Task.WhenAll(showTasks);

                    // Inicia animações especiais após todos os botões aparecerem
                    await StartSpecialAnimations();
                }

                _isShown = true;
                System.Diagnostics.Debug.WriteLine("BaseNavBarComponent mostrada com animação");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao mostrar BaseNavBarComponent: {ex.Message}");
                this.IsVisible = true;
                _isShown = true;
            }
        }

        /// <summary>
        /// Esconde toda a navbar com animação
        /// Só executa se o hardware suportar animações
        /// </summary>
        public async Task HideAsync()
        {
            if (!_isShown)
                return;

            try
            {
                // Para animações especiais primeiro
                await StopSpecialAnimations();

                if (IsAnimated && HardwareDetector.SupportsAnimations)
                {
                    // Esconde botões simultaneamente
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

                    await Task.WhenAll(hideTasks);
                }

                this.IsVisible = false;
                _isShown = false;
                System.Diagnostics.Debug.WriteLine("BaseNavBarComponent escondida com animação");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao esconder BaseNavBarComponent: {ex.Message}");
                this.IsVisible = false;
                _isShown = false;
            }
        }

        /// <summary>
        /// Inicia animações especiais dos botões configurados
        /// Só executa se o hardware suportar animações
        /// </summary>
        public async Task StartSpecialAnimations()
        {
            if (!HardwareDetector.SupportsAnimations)
                return;

            try
            {
                foreach (var buttonView in _buttonViews)
                {
                    if (buttonView is SpecialNavButtonComponent specialButton)
                    {
                        await specialButton.StartSpecialAnimationAsync();
                    }
                    else if (buttonView is NavButtonComponent regularButton)
                    {
                        await regularButton.StartSpecialAnimationAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao iniciar animações especiais: {ex.Message}");
            }
        }

        /// <summary>
        /// Para animações especiais dos botões
        /// </summary>
        public async Task StopSpecialAnimations()
        {
            try
            {
                foreach (var buttonView in _buttonViews)
                {
                    if (buttonView is SpecialNavButtonComponent specialButton)
                    {
                        await specialButton.StopSpecialAnimationAsync();
                    }
                    else if (buttonView is NavButtonComponent regularButton)
                    {
                        await regularButton.StopAllAnimationsAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao parar animações especiais: {ex.Message}");
            }
        }

        /// <summary>
        /// Para todas as animações da navbar
        /// </summary>
        public async Task StopAllAnimationsAsync()
        {
            try
            {
                await _animationManager.StopAllAnimationsAsync();
                await StopSpecialAnimations();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao parar todas as animações: {ex.Message}");
            }
        }

        #endregion

        #region Lifecycle Methods

        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();

            if (Handler == null)
            {
                // Limpa animações quando o handler é removido
                _animationManager?.Dispose();
            }
            else
            {
                // Reconstrói botões quando o handler estiver disponível
                RebuildButtons();
            }
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (BindingContext == null)
            {
                // Para animações quando o contexto muda
                _ = Task.Run(StopAllAnimationsAsync);
            }
        }

        #endregion
    }

    #region Configuration Classes

    public class NavButtonConfig
    {
        public string Text { get; set; } = "";
        public string IconSource { get; set; } = "";
        public ICommand Command { get; set; }
        public object CommandParameter { get; set; }
        public bool IsAnimated { get; set; } = true;
        public NavButtonAnimationType AnimationTypes { get; set; } = NavButtonAnimationType.ShowHide;

        // Para botões especiais
        public bool IsSpecial { get; set; } = false;
        public string CenterContent { get; set; } = "+";
        public string CenterIconSource { get; set; } = "";
        public SpecialButtonGradientType GradientStyle { get; set; } = SpecialButtonGradientType.Yellow;
        public SpecialButtonAnimationType SpecialAnimationTypes { get; set; } = SpecialButtonAnimationType.ShowHide;

        // Factory methods para facilitar criação
        public static NavButtonConfig Regular(string text, string iconSource, ICommand command = null)
        {
            return new NavButtonConfig
            {
                Text = text,
                IconSource = iconSource,
                Command = command,
                IsSpecial = false,
                AnimationTypes = HardwareDetector.SupportsAnimations ? NavButtonAnimationType.ShowHide : NavButtonAnimationType.None
            };
        }

        public static NavButtonConfig Special(string text, string centerContent = "+", SpecialButtonGradientType gradientStyle = SpecialButtonGradientType.Yellow, ICommand command = null)
        {
            return new NavButtonConfig
            {
                Text = text,
                CenterContent = centerContent,
                Command = command,
                IsSpecial = true,
                GradientStyle = gradientStyle,
                SpecialAnimationTypes = HardwareDetector.SupportsAnimations
                    ? (SpecialButtonAnimationType.ShowHide | SpecialButtonAnimationType.Pulse)
                    : SpecialButtonAnimationType.None,
                IsAnimated = true
            };
        }
    }

    #endregion

    #region Event Args

    public class NavBarButtonClickedEventArgs : EventArgs
    {
        public NavButtonConfig ButtonConfig { get; }
        public object Parameter { get; }

        public NavBarButtonClickedEventArgs(NavButtonConfig buttonConfig, object parameter)
        {
            ButtonConfig = buttonConfig;
            Parameter = parameter;
        }
    }

    #endregion
}