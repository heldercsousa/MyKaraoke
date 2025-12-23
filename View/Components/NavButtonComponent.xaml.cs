using Microsoft.Maui.Controls;
using MyVocaList.View.Animations;
using MyVocaList.View.Behaviors;
using System.Windows.Input;
using System.Linq;
using Serilog;

namespace MyVocaList.View.Components
{
    /// <summary>
    /// CLEAN: Behavior replaces all repetitive functionalities
    /// Maintains only NavButton-specific features
    /// </summary>
    public partial class NavButtonComponent : ContentView
    {
        #region Bindable Properties - NAVBUTTON SPECIFIC

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

        #region Properties - NAVBUTTON SPECIFIC

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

        #region Events - NAVBUTTON SPECIFIC

        public event EventHandler<NavButtonEventArgs> ButtonClicked;

        #endregion

        private static readonly Serilog.ILogger Logger = Log.ForContext<NavButtonComponent>();

        public NavButtonComponent()
        {
            // Behavior already applies initial state and creates AnimationManager
            InitializeComponent();

            // Apply only NavButton-specific properties
            MainThread.BeginInvokeOnMainThread(() =>
            {
                ApplyInitialProperties();
            });
        }

        #region Property Changed Handlers - NAVBUTTON SPECIFIC

        private static void OnIconSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is NavButtonComponent button && newValue is string iconSource)
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
        }

        private static void OnIconNameChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is NavButtonComponent button && newValue is string iconName)
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
        }

        private static void OnTextChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is NavButtonComponent button && newValue is string text)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (button.buttonLabel != null)
                    {
                        button.buttonLabel.Text = text;
                        button.buttonLabel.IsVisible = true;
                        Logger.Debug("NavButtonComponent: Label text set to {Text}, IsVisible={IsVisible}, Opacity={Opacity}, TextColor={TextColor}",
                            text, button.buttonLabel.IsVisible, button.buttonLabel.Opacity, button.buttonLabel.TextColor);
                    }
                    else
                    {
                        Logger.Warning("NavButtonComponent: buttonLabel is NULL when trying to set text {Text}", text);
                    }
                });
            }
        }

        #endregion

        #region Event Handlers - NAVBUTTON SPECIFIC

        private async void OnButtonTapped(object sender, EventArgs e)
        {
            // Tap effect via Behavior Extension
            if (IsAnimated && buttonContainer != null && HardwareDetector.SupportsAnimations)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await this.AnimateTapEffect();
                });
            }

            // NavButton Command
            if (Command?.CanExecute(CommandParameter) == true)
            {
                Command.Execute(CommandParameter);
            }

            // NavButton Event
            ButtonClicked?.Invoke(this, new NavButtonEventArgs(Text, IconSource, CommandParameter));

            Logger.Debug("NavButtonComponent {Text} clicked", Text ?? "unnamed");
        }

        #endregion

        #region Private Methods - NAVBUTTON SPECIFIC

        private void ApplyInitialProperties()
        {
            Logger.Debug("NavButtonComponent: ApplyInitialProperties STARTED - Text={Text}, IconName={IconName}, IconSource={IconSource}",
                Text, IconName, IconSource);

            // Manually apply the style from App Resources
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
                Logger.Debug("NavButtonComponent: MD3 icon {IconName} configured", IconName);
            }
            else if (!string.IsNullOrEmpty(IconSource) && buttonIcon != null && buttonIconMd3 != null)
            {
                buttonIcon.Source = IconSource;
                buttonIcon.IsVisible = true;
                buttonIconMd3.IsVisible = false;
                Logger.Debug("NavButtonComponent: PNG icon {IconSource} configured", IconSource);
            }

            if (buttonLabel != null && !string.IsNullOrEmpty(Text))
            {
                buttonLabel.Text = Text;
                buttonLabel.IsVisible = true;

                // Manually apply style to ensure TextColor is correct (White)
                if (Application.Current.Resources.TryGetValue("NavButtonLabelStyle", out var labelStyleObj) && labelStyleObj is Style labelStyle)
                {
                    buttonLabel.Style = labelStyle;
                }

                // FALLBACK: Force White color if style fails or doesn't set it effectively
                if (buttonLabel.TextColor == null || buttonLabel.TextColor == Colors.Transparent)
                {
                    buttonLabel.TextColor = Colors.White;
                }

                Logger.Debug("NavButtonComponent: Label text {Text} configured, IsVisible={IsVisible}, Opacity={Opacity}, TextColor={TextColor}",
                    Text, buttonLabel.IsVisible, buttonLabel.Opacity, buttonLabel.TextColor);
            }
            else
            {
                Logger.Warning("NavButtonComponent: Label NOT configured - buttonLabel={HasButtonLabel}, Text={Text}",
                    buttonLabel != null, Text);
            }

            Logger.Debug("NavButtonComponent: ApplyInitialProperties COMPLETED");
        }

        #endregion

        #region Animation Methods - DELEGATED TO BEHAVIOR

        /// <summary>
        /// DELEGATED: ShowAsync via Behavior
        /// </summary>
        public async Task ShowAsync()
        {
            await AnimatedButtonExtensions.ShowAsync(this);
        }

        /// <summary>
        /// DELEGATED: HideAsync via Behavior
        /// </summary>
        public async Task HideAsync()
        {
            await AnimatedButtonExtensions.HideAsync(this);
        }

        /// <summary>
        /// SPECIFIC: StartSpecialAnimationAsync for NavButton (default pulse)
        /// </summary>
        public async Task StartSpecialAnimationAsync()
        {
            if (!IsAnimated || !HardwareDetector.SupportsAnimations)
                return;

            if (AnimationTypeHelper.HasFlag(AnimationTypes, NavButtonAnimationType.Pulse))
            {
                // NavButton uses default pulse via Behavior
                await AnimatedButtonExtensions.StartSpecialAnimationAsync(this);
            }
        }

        /// <summary>
        /// DELEGATED: StopAllAnimationsAsync via Behavior
        /// </summary>
        public async Task StopAllAnimationsAsync()
        {
            await AnimatedButtonExtensions.StopAllAnimationsAsync(this);
        }

        #endregion

        #region Lifecycle Methods - DELEGATED TO BEHAVIOR

        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();

            if (Handler == null)
            {
                // Behavior already cleans up resources
            }
            else
            {
                // Handler changed via Extension
                this.HandleHandlerChanged();

                // Update NavButton properties
                ApplyInitialProperties();
            }
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            // Binding context changed via Extension
            this.HandleBindingContextChanged();
        }

        #endregion
    }
}