using Microsoft.Maui.Controls;
using MyVocaList.View.Animations;
using MyVocaList.View.Behaviors;
using System.Windows.Input;
using System.Linq;

namespace MyVocaList.View.Components
{
    /// <summary>
    /// ✅ LIMPO: Behavior substitui todas as funcionalidades repetitivas
    /// Mantém apenas funcionalidades específicas do NavButton
    /// </summary>
    public partial class NavButtonComponent : ContentView
    {
        #region Bindable Properties - ESPECÍFICAS DO NAVBUTTON

        public static readonly BindableProperty IconSourceProperty =
            BindableProperty.Create(nameof(IconSource), typeof(string), typeof(NavButtonComponent), string.Empty, propertyChanged: OnIconSourceChanged);

        public static readonly BindableProperty IconNameProperty =
            BindableProperty.Create(nameof(IconName), typeof(string), typeof(NavButtonComponent), string.Empty, propertyChanged: OnIconNameChanged);

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(NavButtonComponent), string.Empty, propertyChanged: OnTextChanged);

        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(NavButtonComponent), null);

        public static readonly BindableProperty CommandParameterProperty =
            BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(NavButtonComponent), null);

        public static readonly BindableProperty IsAnimatedProperty =
            BindableProperty.Create(nameof(IsAnimated), typeof(bool), typeof(NavButtonComponent), true);

        public static readonly BindableProperty AnimationTypesProperty =
            BindableProperty.Create(nameof(AnimationTypes), typeof(NavButtonAnimationType), typeof(NavButtonComponent), NavButtonAnimationType.ShowHide);

        public static readonly BindableProperty ShowDelayProperty =
            BindableProperty.Create(nameof(ShowDelay), typeof(int), typeof(NavButtonComponent), 0);

        #endregion

        #region Properties - ESPECÍFICAS DO NAVBUTTON

        public string IconSource
        {
            get => (string)GetValue(IconSourceProperty);
            set => SetValue(IconSourceProperty, value);
        }

        public string IconName
        {
            get => (string)GetValue(IconNameProperty);
            set => SetValue(IconNameProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public bool IsAnimated
        {
            get => (bool)GetValue(IsAnimatedProperty);
            set => SetValue(IsAnimatedProperty, value);
        }

        public NavButtonAnimationType AnimationTypes
        {
            get => (NavButtonAnimationType)GetValue(AnimationTypesProperty);
            set => SetValue(AnimationTypesProperty, value);
        }

        public int ShowDelay
        {
            get => (int)GetValue(ShowDelayProperty);
            set => SetValue(ShowDelayProperty, value);
        }

        #endregion

        #region Events - ESPECÍFICOS DO NAVBUTTON

        public event EventHandler<NavButtonEventArgs> ButtonClicked;

        #endregion

        public NavButtonComponent()
        {
            // ✅ O BEHAVIOR já aplica o estado inicial e cria o AnimationManager
            InitializeComponent();

            // ✅ Aplica apenas propriedades específicas do NavButton
            MainThread.BeginInvokeOnMainThread(() =>
            {
                ApplyInitialProperties();
            });
        }

        #region Property Changed Handlers - ESPECÍFICOS DO NAVBUTTON

        private static void OnIconSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is NavButtonComponent button && newValue is string iconSource)
            {
                try
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        if (button.buttonIcon != null && button.buttonIconMd3 != null)
                        {
                            // PNG mode: show Image, hide StatefulIcon
                            button.buttonIcon.Source = iconSource;
                            button.buttonIcon.IsVisible = !string.IsNullOrEmpty(iconSource);
                            button.buttonIconMd3.IsVisible = false;
                        }
                    });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Erro ao definir IconSource: {ex.Message}");
                }
            }
        }

        private static void OnIconNameChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is NavButtonComponent button && newValue is string iconName)
            {
                try
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        if (button.buttonIcon != null && button.buttonIconMd3 != null)
                        {
                            // MD3 mode: show StatefulIcon, hide Image
                            button.buttonIconMd3.IconName = iconName;
                            button.buttonIconMd3.IsVisible = !string.IsNullOrEmpty(iconName);
                            button.buttonIcon.IsVisible = false;
                        }
                    });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Erro ao definir IconName: {ex.Message}");
                }
            }
        }

        private static void OnTextChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is NavButtonComponent button && newValue is string text)
            {
                try
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        if (button.buttonLabel != null)
                        {
                            button.buttonLabel.Text = text;
                            button.buttonLabel.IsVisible = true; // ✅ FIX: Ensure label is visible
                            System.Diagnostics.Debug.WriteLine($"✅ NavButtonComponent: Label text set to '{text}', IsVisible={button.buttonLabel.IsVisible}, Opacity={button.buttonLabel.Opacity}, TextColor={button.buttonLabel.TextColor}");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"❌ NavButtonComponent: buttonLabel is NULL when trying to set text '{text}'");
                        }
                    });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Erro ao definir Text: {ex.Message}");
                }
            }
        }

        #endregion

        #region Event Handlers - ESPECÍFICOS DO NAVBUTTON

        private async void OnButtonTapped(object sender, EventArgs e)
        {
            try
            {
                // ✅ USA BEHAVIOR: Tap effect via Extension
                if (IsAnimated && buttonContainer != null && HardwareDetector.SupportsAnimations)
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await this.AnimateTapEffect(); // Extension do Behavior
                    });
                }

                // ✅ ESPECÍFICO: Comando do NavButton
                if (Command?.CanExecute(CommandParameter) == true)
                {
                    Command.Execute(CommandParameter);
                }

                // ✅ ESPECÍFICO: Evento do NavButton
                ButtonClicked?.Invoke(this, new NavButtonEventArgs(Text, IconSource, CommandParameter));

                System.Diagnostics.Debug.WriteLine($"NavButtonComponent '{Text ?? "sem nome"}' clicado");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no tap do NavButtonComponent '{Text ?? "sem nome"}': {ex.Message}");
            }
        }

        #endregion

        #region Private Methods - ESPECÍFICOS DO NAVBUTTON

        private void    ApplyInitialProperties()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🎯 NavButtonComponent: ApplyInitialProperties STARTED - Text='{Text}', IconName='{IconName}', IconSource='{IconSource}'");

                // ✅ FIX: Manually apply the style from App Resources
                if (buttonContainer != null)
                {
                    if (Application.Current.Resources.TryGetValue("BaseNavButtonStyle", out var styleObj) && styleObj is Style navStyle)
                    {
                        buttonContainer.Style = navStyle;
                    }
                    else
                    {
                        // Fallback if style is missing (Safety net)
                        buttonContainer.Orientation = StackOrientation.Vertical;
                        buttonContainer.HorizontalOptions = LayoutOptions.Center;
                        buttonContainer.Spacing = 1;
                    }
                }
                // MD3 icons have priority over PNG
                if (!string.IsNullOrEmpty(IconName) && buttonIconMd3 != null && buttonIcon != null)
                {
                    buttonIconMd3.IconName = IconName;
                    buttonIconMd3.IsVisible = true;
                    buttonIcon.IsVisible = false;
                    System.Diagnostics.Debug.WriteLine($"✅ NavButtonComponent: MD3 icon '{IconName}' configured");
                }
                else if (!string.IsNullOrEmpty(IconSource) && buttonIcon != null && buttonIconMd3 != null)
                {
                    buttonIcon.Source = IconSource;
                    buttonIcon.IsVisible = true;
                    buttonIconMd3.IsVisible = false;
                    System.Diagnostics.Debug.WriteLine($"✅ NavButtonComponent: PNG icon '{IconSource}' configured");
                }

                if (buttonLabel != null && !string.IsNullOrEmpty(Text))
                {
                    buttonLabel.Text = Text;
                    buttonLabel.IsVisible = true; // ✅ FIX: Ensure label is visible
                    System.Diagnostics.Debug.WriteLine($"✅ NavButtonComponent: Label text '{Text}' configured, IsVisible={buttonLabel.IsVisible}, Opacity={buttonLabel.Opacity}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ NavButtonComponent: Label NOT configured - buttonLabel={buttonLabel != null}, Text='{Text}'");
                }

                System.Diagnostics.Debug.WriteLine($"✅ NavButtonComponent: ApplyInitialProperties COMPLETED");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao aplicar propriedades iniciais: {ex.Message}");
            }
        }

        #endregion

        #region Métodos de Animação - DELEGADOS PARA O BEHAVIOR

        /// <summary>
        /// ✅ DELEGADO: ShowAsync via Behavior
        /// </summary>
        public async Task ShowAsync()
        {
            await AnimatedButtonExtensions.ShowAsync(this);
        }

        /// <summary>
        /// ✅ DELEGADO: HideAsync via Behavior
        /// </summary>
        public async Task HideAsync()
        {
            await AnimatedButtonExtensions.HideAsync(this);
        }

        /// <summary>
        /// ✅ ESPECÍFICO: StartSpecialAnimationAsync para NavButton (pulse padrão)
        /// </summary>
        public async Task StartSpecialAnimationAsync()
        {
            if (!IsAnimated || !HardwareDetector.SupportsAnimations)
                return;

            try
            {
                if (AnimationTypeHelper.HasFlag(AnimationTypes, NavButtonAnimationType.Pulse))
                {
                    // ✅ ESPECÍFICO: NavButton usa pulse padrão via Behavior
                    await AnimatedButtonExtensions.StartSpecialAnimationAsync(this);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro na animação especial: {ex.Message}");
            }
        }

        /// <summary>
        /// ✅ DELEGADO: StopAllAnimationsAsync via Behavior
        /// </summary>
        public async Task StopAllAnimationsAsync()
        {
            await AnimatedButtonExtensions.StopAllAnimationsAsync(this);
        }

        #endregion

        #region Lifecycle Methods - DELEGADOS PARA O BEHAVIOR

        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();

            if (Handler == null)
            {
                // ✅ O BEHAVIOR já limpa os recursos
            }
            else
            {
                // ✅ USA BEHAVIOR: Handler changed via Extension
                this.HandleHandlerChanged();

                // ✅ ESPECÍFICO: Atualiza propriedades do NavButton
                try
                {
                    ApplyInitialProperties();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Erro ao atualizar propriedades no OnHandlerChanged: {ex.Message}");
                }
            }
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            // ✅ USA BEHAVIOR: Binding context changed via Extension
            this.HandleBindingContextChanged();
        }

        #endregion
    }
}