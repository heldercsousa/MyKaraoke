using Microsoft.Maui.Controls;
using MyKaraoke.View.Animations;
using MyKaraoke.View.Behaviors;
using System.Windows.Input;
using System.Linq;

namespace MyKaraoke.View.Components
{
    /// <summary>
    /// ✅ LIMPO: Behavior substitui todas as funcionalidades repetitivas
    /// Mantém apenas funcionalidades específicas do SpecialNavButton
    /// </summary>
    public partial class SpecialNavButtonComponent : ContentView
    {
        #region Bindable Properties - ESPECÍFICAS DO SPECIALNAVBUTTON

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

        #region Properties - ESPECÍFICAS DO SPECIALNAVBUTTON

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

        #region Events - ESPECÍFICOS DO SPECIALNAVBUTTON

        public event EventHandler<SpecialNavButtonEventArgs> ButtonClicked;

        #endregion

        public SpecialNavButtonComponent()
        {
            // ✅ O BEHAVIOR já aplica o estado inicial e cria o AnimationManager
            InitializeComponent();

            // ✅ Aplica apenas propriedades específicas do SpecialNavButton
            MainThread.BeginInvokeOnMainThread(() =>
            {
                ApplyInitialProperties();
            });
        }

        #region Private Methods - ESPECÍFICOS DO SPECIALNAVBUTTON

        private void ApplyInitialProperties()
        {
            try
            {
                UpdateCenterContent();
                UpdateGradientStyle(GradientStyle);
                if (buttonLabel != null && !string.IsNullOrEmpty(Text))
                {
                    buttonLabel.Text = Text;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao aplicar propriedades iniciais: {ex.Message}");
            }
        }

        private void UpdateCenterContent()
        {
            try
            {
                if (contentImage == null || contentLabel == null) return;

                if (!string.IsNullOrEmpty(CenterIconSource))
                {
                    // Usa ícone
                    contentImage.Source = CenterIconSource;
                    contentImage.IsVisible = true;
                    contentLabel.IsVisible = false;
                }
                else
                {
                    // Usa texto/símbolo
                    contentLabel.Text = CenterContent;
                    contentLabel.IsVisible = true;
                    contentImage.IsVisible = false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao atualizar conteúdo central: {ex.Message}");
            }
        }

        private void UpdateGradientStyle(SpecialButtonGradientType gradientType)
        {
            try
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
                    // Fallback se os estilos não estiverem disponíveis
                    System.Diagnostics.Debug.WriteLine($"Estilo {gradientType} não encontrado, usando fallback");
                    return;
                }

                if (targetStyle != null)
                {
                    gradientFrame.Style = targetStyle;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao atualizar estilo do gradiente: {ex.Message}");
            }
        }

        #endregion

        #region Property Changed Handlers - ESPECÍFICOS DO SPECIALNAVBUTTON

        private static void OnTextChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is SpecialNavButtonComponent button && newValue is string text)
            {
                try
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        if (button.buttonLabel != null)
                        {
                            button.buttonLabel.Text = text;
                        }
                    });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Erro ao definir Text: {ex.Message}");
                }
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

        #region Event Handlers - ESPECÍFICOS DO SPECIALNAVBUTTON

        private async void OnButtonTapped(object sender, EventArgs e)
        {
            try
            {
                // ✅ ESPECÍFICO: Para a animação especial quando clicado
                if (IsAnimated)
                {
                    await StopSpecialAnimationAsync();
                }

                // ✅ USA BEHAVIOR: Tap effect via Extension (usando gradientFrame como container)
                if (IsAnimated && gradientFrame != null && HardwareDetector.SupportsAnimations)
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await this.AnimateTapEffect(); // Extension do Behavior
                    });
                }

                // ✅ ESPECÍFICO: Comando do SpecialNavButton
                if (Command?.CanExecute(CommandParameter) == true)
                {
                    Command.Execute(CommandParameter);
                }

                // ✅ ESPECÍFICO: Evento do SpecialNavButton
                ButtonClicked?.Invoke(this, new SpecialNavButtonEventArgs(Text, CenterContent, CenterIconSource, CommandParameter));

                System.Diagnostics.Debug.WriteLine($"SpecialNavButtonComponent '{Text ?? "sem nome"}' clicado");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no tap do SpecialNavButtonComponent '{Text ?? "sem nome"}': {ex.Message}");
            }
        }

        #endregion

        #region Métodos de Animação - DELEGADOS PARA O BEHAVIOR + ESPECÍFICOS

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
        /// ✅ ESPECÍFICO: StartSpecialAnimationAsync para SpecialNavButton (pulse especial)
        /// </summary>
        public async Task StartSpecialAnimationAsync()
        {
            if (!IsAnimated || !HardwareDetector.SupportsAnimations || buttonContainer == null)
                return;

            try
            {
                if (AnimationTypeHelper.HasFlag(AnimationTypes, SpecialButtonAnimationType.Pulse))
                {
                    // ✅ ESPECÍFICO: SpecialNavButton usa pulse especial via Behavior
                    await AnimatedButtonExtensions.StartSpecialAnimationAsync(this);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro na animação especial: {ex.Message}");
            }
        }

        /// <summary>
        /// ✅ ESPECÍFICO: Para apenas a animação especial
        /// </summary>
        public async Task StopSpecialAnimationAsync()
        {
            try
            {
                // ✅ USA BEHAVIOR: StopAllAnimationsAsync já para todas as animações
                await AnimatedButtonExtensions.StopAllAnimationsAsync(this);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao parar animação especial: {ex.Message}");
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

                // ✅ ESPECÍFICO: Atualiza propriedades do SpecialNavButton
                try
                {
                    UpdateCenterContent();
                    UpdateGradientStyle(GradientStyle);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Erro no OnHandlerChanged: {ex.Message}");
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