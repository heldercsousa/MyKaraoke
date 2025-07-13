using Microsoft.Maui.Controls;
using MyKaraoke.View.Animations;
using System.Windows.Input;

namespace MyKaraoke.View.Components
{
    public partial class NavButtonComponent : ContentView
    {
        #region Bindable Properties

        public static readonly BindableProperty IconSourceProperty =
            BindableProperty.Create(nameof(IconSource), typeof(string), typeof(NavButtonComponent), string.Empty, propertyChanged: OnIconSourceChanged);

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

        #region Properties

        public string IconSource
        {
            get => (string)GetValue(IconSourceProperty);
            set => SetValue(IconSourceProperty, value);
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

        #region Events

        public event EventHandler<NavButtonEventArgs> ButtonClicked;

        #endregion

        #region Private Fields

        private AnimationManager _animationManager;
        private bool _isShown = false;

        #endregion

        public NavButtonComponent()
        {
            try
            {
                InitializeComponent();
                _animationManager = new AnimationManager($"NavButton_{GetHashCode()}");

                // Aplica propriedades iniciais após a inicialização
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ApplyInitialProperties();
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao inicializar NavButtonComponent: {ex.Message}");
            }
        }

        private void ApplyInitialProperties()
        {
            try
            {
                if (buttonIcon != null && !string.IsNullOrEmpty(IconSource))
                {
                    buttonIcon.Source = IconSource;
                }
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

        #region Property Changed Handlers

        private static void OnIconSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is NavButtonComponent button && newValue is string iconSource)
            {
                try
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        if (button.buttonIcon != null)
                        {
                            button.buttonIcon.Source = iconSource;
                        }
                    });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Erro ao definir IconSource: {ex.Message}");
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

        #region Event Handlers

        private async void OnButtonTapped(object sender, EventArgs e)
        {
            try
            {
                // Animação de tap (press effect)
                if (IsAnimated && buttonContainer != null && HardwareDetector.SupportsAnimations)
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await AnimateTapEffect();
                    });
                }

                // Executa comando se disponível
                if (Command?.CanExecute(CommandParameter) == true)
                {
                    Command.Execute(CommandParameter);
                }

                // Dispara evento personalizado
                ButtonClicked?.Invoke(this, new NavButtonEventArgs(Text, IconSource, CommandParameter));

                System.Diagnostics.Debug.WriteLine($"NavButtonComponent '{Text}' clicado");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no tap do NavButtonComponent '{Text}': {ex.Message}");
            }
        }

        #endregion

        #region Animation Methods

        /// <summary>
        /// Mostra o botão com múltiplas animações simultâneas baseadas no AnimationTypes
        /// Só executa se o hardware suportar animações
        /// </summary>
        public async Task ShowAsync()
        {
            if (_isShown)
            {
                System.Diagnostics.Debug.WriteLine($"⚠️ NavButton '{Text}': ShowAsync ignorado - já mostrado");
                return;
            }

            try
            {
                System.Diagnostics.Debug.WriteLine($"🎬 NavButton '{Text}': Iniciando ShowAsync - AnimationTypes: {AnimationTypes}, Hardware: {HardwareDetector.SupportsAnimations}");

                // Aplica delay se configurado
                if (ShowDelay > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"⏰ NavButton '{Text}': Aguardando delay de {ShowDelay}ms");
                    await Task.Delay(ShowDelay);
                }

                // Estado inicial (escondido) - FORÇA aplicação imediata
                System.Diagnostics.Debug.WriteLine($"📍 NavButton '{Text}': Aplicando estado inicial...");
                this.IsVisible = true;
                this.Opacity = 0;
                this.TranslationY = 20;

                // Força aplicação no MainThread para garantir que seja aplicado
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    this.Opacity = 0;
                    this.TranslationY = 20;
                });

                System.Diagnostics.Debug.WriteLine($"📍 NavButton '{Text}': Estado inicial confirmado (Opacity={this.Opacity}, TranslationY={this.TranslationY})");

                if (IsAnimated && HardwareDetector.SupportsAnimations && AnimationTypes != NavButtonAnimationType.None)
                {
                    System.Diagnostics.Debug.WriteLine($"✅ NavButton '{Text}': Condições atendidas - executando animações");

                    // Executa múltiplas animações simultaneamente baseadas nas flags
                    var animationTasks = new List<Task>();

                    if (AnimationTypeHelper.HasFlag(AnimationTypes, NavButtonAnimationType.Fade))
                    {
                        System.Diagnostics.Debug.WriteLine($"🌟 NavButton '{Text}': Adicionando Fade à lista de animações");
                        animationTasks.Add(StartFadeInAsync());
                    }

                    if (AnimationTypeHelper.HasFlag(AnimationTypes, NavButtonAnimationType.Translate))
                    {
                        System.Diagnostics.Debug.WriteLine($"🚀 NavButton '{Text}': Adicionando Translate à lista de animações");
                        animationTasks.Add(StartSlideUpAsync());
                    }

                    // Executa todas as animações simultaneamente
                    if (animationTasks.Any())
                    {
                        System.Diagnostics.Debug.WriteLine($"🎯 NavButton '{Text}': Executando {animationTasks.Count} animações simultaneamente");
                        await Task.WhenAll(animationTasks);
                        System.Diagnostics.Debug.WriteLine($"🎉 NavButton '{Text}': Todas as {animationTasks.Count} animações concluídas");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"⚠️ NavButton '{Text}': Nenhuma animação configurada para execução");
                    }
                }
                else
                {
                    // Hardware limitado ou animações desabilitadas: apenas aplicar estado final
                    this.Opacity = 1;
                    this.TranslationY = 0;
                    System.Diagnostics.Debug.WriteLine($"🚫 NavButton '{Text}': Hardware limitado ou animações desabilitadas - aplicando estado final direto");
                }

                _isShown = true;
                System.Diagnostics.Debug.WriteLine($"✅ NavButton '{Text}': ShowAsync concluído com sucesso");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ NavButton '{Text}': Erro em ShowAsync: {ex.Message}");
                // Fallback: mostrar sem animação
                this.Opacity = 1;
                this.TranslationY = 0;
                this.IsVisible = true;
                _isShown = true;
            }
        }

        /// <summary>
        /// Esconde o botão com múltiplas animações simultâneas baseadas no AnimationTypes
        /// Só executa se o hardware suportar animações
        /// </summary>
        public async Task HideAsync()
        {
            if (!_isShown)
                return;

            try
            {
                if (IsAnimated && HardwareDetector.SupportsAnimations && AnimationTypes != NavButtonAnimationType.None)
                {
                    // Executa múltiplas animações simultaneamente baseadas nas flags
                    var animationTasks = new List<Task>();

                    if (AnimationTypeHelper.HasFlag(AnimationTypes, NavButtonAnimationType.Fade))
                    {
                        animationTasks.Add(StartFadeOutAsync());
                    }

                    if (AnimationTypeHelper.HasFlag(AnimationTypes, NavButtonAnimationType.Translate))
                    {
                        animationTasks.Add(StartSlideDownAsync());
                    }

                    // Executa todas as animações simultaneamente
                    if (animationTasks.Any())
                    {
                        await Task.WhenAll(animationTasks);
                    }
                }
                else
                {
                    this.Opacity = 0;
                    this.TranslationY = 20;
                }

                this.IsVisible = false;
                _isShown = false;
                System.Diagnostics.Debug.WriteLine($"NavButtonComponent '{Text}' escondido com animações: {AnimationTypes}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao esconder NavButtonComponent '{Text}': {ex.Message}");
                this.Opacity = 0;
                this.IsVisible = false;
                _isShown = false;
            }
        }

        /// <summary>
        /// Inicia animação de Pulse se configurada no AnimationTypes
        /// Só executa se o hardware suportar animações
        /// </summary>
        public async Task StartSpecialAnimationAsync()
        {
            if (!IsAnimated || !HardwareDetector.SupportsAnimations)
                return;

            try
            {
                if (AnimationTypeHelper.HasFlag(AnimationTypes, NavButtonAnimationType.Pulse))
                {
                    await _animationManager.StartCallToActionAsync("nav_pulse", buttonContainer, () => this.IsVisible);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro na animação especial: {ex.Message}");
            }
        }

        /// <summary>
        /// Para todas as animações do botão
        /// </summary>
        public async Task StopAllAnimationsAsync()
        {
            try
            {
                await _animationManager.StopAllAnimationsAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao parar animações: {ex.Message}");
            }
        }

        private async Task StartFadeInAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🌟 NavButton '{Text}': Iniciando Fade In DIRETO - Estado atual: Opacity={this.Opacity}");

                // Executa fade direto usando APIs nativas do MAUI no MainThread
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await this.FadeTo(1.0, 500, Easing.CubicOut); // Aumentei para 500ms para ser mais visível
                });

                System.Diagnostics.Debug.WriteLine($"✅ NavButton '{Text}': Fade In concluído - Estado final: Opacity={this.Opacity}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erro no Fade In: {ex.Message}");
            }
        }

        private async Task StartFadeOutAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🌟 NavButton '{Text}': Iniciando Fade Out DIRETO - Estado atual: Opacity={this.Opacity}");

                // Executa fade direto usando APIs nativas do MAUI no MainThread
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await this.FadeTo(0.0, 500, Easing.CubicIn); // Aumentei para 500ms para ser mais visível
                });

                System.Diagnostics.Debug.WriteLine($"✅ NavButton '{Text}': Fade Out concluído - Estado final: Opacity={this.Opacity}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erro no Fade Out: {ex.Message}");
            }
        }

        private async Task StartSlideUpAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🚀 NavButton '{Text}': Iniciando Slide Up DIRETO - Estado atual: TranslationY={this.TranslationY}");

                // Executa translate direto usando APIs nativas do MAUI no MainThread
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await this.TranslateTo(0, 0, 500, Easing.CubicOut); // Aumentei para 500ms para ser mais visível
                });

                System.Diagnostics.Debug.WriteLine($"✅ NavButton '{Text}': Slide Up concluído - Estado final: TranslationY={this.TranslationY}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erro no Slide Up: {ex.Message}");
            }
        }

        private async Task StartSlideDownAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🚀 NavButton '{Text}': Iniciando Slide Down DIRETO - Estado atual: TranslationY={this.TranslationY}");

                // Executa translate direto usando APIs nativas do MAUI no MainThread
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await this.TranslateTo(0, 20, 500, Easing.CubicIn); // Aumentei para 500ms para ser mais visível
                });

                System.Diagnostics.Debug.WriteLine($"✅ NavButton '{Text}': Slide Down concluído - Estado final: TranslationY={this.TranslationY}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erro no Slide Down: {ex.Message}");
            }
        }

        private async Task AnimateTapEffect()
        {
            try
            {
                if (buttonContainer != null)
                {
                    // Efeito de "press" simples
                    await buttonContainer.ScaleTo(0.95, 100);
                    await buttonContainer.ScaleTo(1.0, 100);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no efeito de tap: {ex.Message}");
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
                // Atualiza propriedades quando o handler estiver disponível
                try
                {
                    if (buttonIcon != null && !string.IsNullOrEmpty(IconSource))
                    {
                        buttonIcon.Source = IconSource;
                    }
                    if (buttonLabel != null && !string.IsNullOrEmpty(Text))
                    {
                        buttonLabel.Text = Text;
                    }
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

            if (BindingContext == null)
            {
                // Para animações quando o contexto muda
                _ = Task.Run(StopAllAnimationsAsync);
            }
        }

        #endregion
    }

    #region Event Args

    public class NavButtonEventArgs : EventArgs
    {
        public string ButtonText { get; }
        public string IconSource { get; }
        public object Parameter { get; }

        public NavButtonEventArgs(string buttonText, string iconSource, object parameter)
        {
            ButtonText = buttonText;
            IconSource = iconSource;
            Parameter = parameter;
        }
    }

    #endregion
}