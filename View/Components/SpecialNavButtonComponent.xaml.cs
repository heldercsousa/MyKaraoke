using Microsoft.Maui.Controls;
using MyVocaList.View.Animations;
using MyVocaList.View.Behaviors;
using System.Windows.Input;
using System.Linq;
using Serilog;

namespace MyVocaList.View.Components
{
    /// <summary>
    /// ✅ CLEAN: Behavior replaces all repetitive functionalities
    /// Maintains only specific functionalities of SpecialNavButton
    /// </summary>
    public partial class SpecialNavButtonComponent : ContentView
    {
        private static readonly ILogger Logger = Log.ForContext<SpecialNavButtonComponent>();

        #region Bindable Properties - SPECIFIC TO SPECIALNAVBUTTON

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(SpecialNavButtonComponent), string.Empty, propertyChanged: OnTextChanged);

        public static readonly BindableProperty CenterContentProperty =
            BindableProperty.Create(nameof(CenterContent), typeof(string), typeof(SpecialNavButtonComponent), "+", propertyChanged: OnCenterContentChanged);

        public static readonly BindableProperty CenterIconSourceProperty =
            BindableProperty.Create(nameof(CenterIconSource), typeof(string), typeof(SpecialNavButtonComponent), string.Empty, propertyChanged: OnCenterIconSourceChanged);

        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(SpecialNavButtonComponent), null);

        public static readonly BindableProperty CommandParameterProperty =
            BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(SpecialNavButtonComponent), null);

        public static readonly BindableProperty GradientStyleProperty =
            BindableProperty.Create(nameof(GradientStyle), typeof(SpecialButtonGradientType), typeof(SpecialNavButtonComponent), SpecialButtonGradientType.Yellow, propertyChanged: OnGradientStyleChanged);

        public static readonly BindableProperty IsAnimatedProperty =
            BindableProperty.Create(nameof(IsAnimated), typeof(bool), typeof(SpecialNavButtonComponent), true);

        public static readonly BindableProperty AnimationTypesProperty =
            BindableProperty.Create(nameof(AnimationTypes), typeof(SpecialButtonAnimationType), typeof(SpecialNavButtonComponent), SpecialButtonAnimationType.ShowHide);

        public static readonly BindableProperty ShowDelayProperty =
            BindableProperty.Create(nameof(ShowDelay), typeof(int), typeof(SpecialNavButtonComponent), 0);

        #endregion

        #region Properties - SPECIFIC TO SPECIALNAVBUTTON

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public string CenterContent
        {
            get => (string)GetValue(CenterContentProperty);
            set => SetValue(CenterContentProperty, value);
        }

        public string CenterIconSource
        {
            get => (string)GetValue(CenterIconSourceProperty);
            set => SetValue(CenterIconSourceProperty, value);
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

        public SpecialButtonGradientType GradientStyle
        {
            get => (SpecialButtonGradientType)GetValue(GradientStyleProperty);
            set => SetValue(GradientStyleProperty, value);
        }

        public bool IsAnimated
        {
            get => (bool)GetValue(IsAnimatedProperty);
            set => SetValue(IsAnimatedProperty, value);
        }

        public SpecialButtonAnimationType AnimationTypes
        {
            get => (SpecialButtonAnimationType)GetValue(AnimationTypesProperty);
            set => SetValue(AnimationTypesProperty, value);
        }

        public int ShowDelay
        {
            get => (int)GetValue(ShowDelayProperty);
            set => SetValue(ShowDelayProperty, value);
        }

        #endregion

        #region Events - SPECIFIC TO SPECIALNAVBUTTON

        public event EventHandler<SpecialNavButtonEventArgs> ButtonClicked;

        #endregion

        public SpecialNavButtonComponent()
        {
            // ✅ The BEHAVIOR already applies initial state and creates AnimationManager
            InitializeComponent();

            // ✅ Apply only specific properties of SpecialNavButton
            MainThread.BeginInvokeOnMainThread(() =>
            {
                ApplyInitialProperties();
            });
        }

        #region Private Methods - SPECIFIC TO SPECIALNAVBUTTON

        private void ApplyInitialProperties()
        {
            UpdateCenterContent();
            UpdateGradientStyle(GradientStyle);
            if (buttonLabel != null && !string.IsNullOrEmpty(Text))
            {
                buttonLabel.Text = Text;
            }
        }

        private void UpdateCenterContent()
        {
            if (contentImage == null || contentLabel == null) return;

            if (!string.IsNullOrEmpty(CenterIconSource))
            {
                // Use icon
                contentImage.Source = CenterIconSource;
                contentImage.IsVisible = true;
                contentLabel.IsVisible = false;
            }
            else
            {
                // Use text/symbol
                contentLabel.Text = CenterContent;
                contentLabel.IsVisible = true;
                contentImage.IsVisible = false;
            }
        }

        private void UpdateGradientStyle(SpecialButtonGradientType gradientType)
        {
            if (gradientFrame == null) return;

            Style targetStyle = null;
            try
            {
                targetStyle = gradientType switch
                {
                    SpecialButtonGradientType.Yellow => (Style)Application.Current.Resources["YellowGradientFrameStyle"],
                    SpecialButtonGradientType.Purple => (Style)Application.Current.Resources["PurpleGradientFrameStyle"],
                    _ => (Style)Application.Current.Resources["YellowGradientFrameStyle"]
                };
            }
            catch
            {
                // Fallback if styles are not available
                Logger.Debug("Style {GradientType} not found, using fallback", gradientType);
                return;
            }

            if (targetStyle != null)
            {
                gradientFrame.Style = targetStyle;
            }
        }

        #endregion

        #region Property Changed Handlers - SPECIFIC TO SPECIALNAVBUTTON

        private static void OnTextChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is SpecialNavButtonComponent button && newValue is string text)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (button.buttonLabel != null)
                    {
                        button.buttonLabel.Text = text;
                    }
                });
            }
        }

        private static void OnCenterContentChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is SpecialNavButtonComponent button && newValue is string content)
            {
                button.UpdateCenterContent();
            }
        }

        private static void OnCenterIconSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is SpecialNavButtonComponent button)
            {
                button.UpdateCenterContent();
            }
        }

        private static void OnGradientStyleChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is SpecialNavButtonComponent button && newValue is SpecialButtonGradientType gradientType)
            {
                button.UpdateGradientStyle(gradientType);
            }
        }

        #endregion

        #region Event Handlers - SPECIFIC TO SPECIALNAVBUTTON

        private async void OnButtonTapped(object sender, EventArgs e)
        {
            // ✅ SPECIFIC: Stop special animation when clicked
            if (IsAnimated)
            {
                await StopSpecialAnimationAsync();
            }

            // ✅ USE BEHAVIOR: Tap effect via Extension (using gradientFrame as container)
            if (IsAnimated && gradientFrame != null && HardwareDetector.SupportsAnimations)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await this.AnimateTapEffect(); // Extension from Behavior
                });
            }

            // ✅ SPECIFIC: SpecialNavButton command
            if (Command?.CanExecute(CommandParameter) == true)
            {
                Command.Execute(CommandParameter);
            }

            // ✅ SPECIFIC: SpecialNavButton event
            ButtonClicked?.Invoke(this, new SpecialNavButtonEventArgs(Text, CenterContent, CenterIconSource, CommandParameter));

            Logger.Debug("SpecialNavButtonComponent {ButtonText} clicked", Text ?? "unnamed");
        }

        #endregion

        #region Animation Methods - DELEGATED TO BEHAVIOR + SPECIFIC

        /// <summary>
        /// ✅ DELEGATED: ShowAsync via Behavior
        /// </summary>
        public async Task ShowAsync()
        {
            await AnimatedButtonExtensions.ShowAsync(this);
        }

        /// <summary>
        /// ✅ DELEGATED: HideAsync via Behavior
        /// </summary>
        public async Task HideAsync()
        {
            await AnimatedButtonExtensions.HideAsync(this);
        }

        /// <summary>
        /// ✅ SPECIFIC: StartSpecialAnimationAsync for SpecialNavButton (special pulse)
        /// </summary>
        public async Task StartSpecialAnimationAsync()
        {
            if (!IsAnimated || !HardwareDetector.SupportsAnimations || buttonContainer == null)
                return;

            if (AnimationTypeHelper.HasFlag(AnimationTypes, SpecialButtonAnimationType.Pulse))
            {
                // ✅ SPECIFIC: SpecialNavButton uses special pulse via Behavior
                await AnimatedButtonExtensions.StartSpecialAnimationAsync(this);
            }
        }

        /// <summary>
        /// ✅ SPECIFIC: Stop only special animation
        /// </summary>
        public async Task StopSpecialAnimationAsync()
        {
            // ✅ USE BEHAVIOR: StopAllAnimationsAsync already stops all animations
            await AnimatedButtonExtensions.StopAllAnimationsAsync(this);
        }

        /// <summary>
        /// ✅ DELEGATED: StopAllAnimationsAsync via Behavior
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
                // ✅ The BEHAVIOR already cleans up resources
            }
            else
            {
                // ✅ USE BEHAVIOR: Handler changed via Extension
                this.HandleHandlerChanged();

                // ✅ SPECIFIC: Update SpecialNavButton properties
                UpdateCenterContent();
                UpdateGradientStyle(GradientStyle);
            }
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            // ✅ USE BEHAVIOR: Binding context changed via Extension
            this.HandleBindingContextChanged();
        }

        #endregion
    }

}