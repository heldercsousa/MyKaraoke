using Microsoft.Maui.Controls;
using MyKaraoke.View.Animations;
using System.Windows.Input;

namespace MyKaraoke.View.Components
{
    public partial class SpecialNavButtonComponent : ContentView
    {
        #region Bindable Properties

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

        #region Properties

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

        #region Events

        public event EventHandler<SpecialNavButtonEventArgs> ButtonClicked;

        #endregion

        #region Private Fields

        private AnimationManager _animationManager;
        private bool _isShown = false;

        #endregion

        public SpecialNavButtonComponent()
        {
            try
            {
                // ✅ CORREÇÃO CRÍTICA: Aplica estado inicial ANTES do InitializeComponent para evitar "piscar"
                this.IsVisible = true;
                this.Opacity = 0.0;
                this.TranslationY = 60;

                InitializeComponent();
                _animationManager = new AnimationManager($"SpecialNavButton_{GetHashCode()}");

                // ✅ CORREÇÃO: Aplica estado inicial novamente APÓS InitializeComponent
                ApplyInitialState();

                // Aplica propriedades iniciais após a inicialização
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ApplyInitialProperties();
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao inicializar SpecialNavButtonComponent: {ex.Message}");
            }
        }

        /// <summary>
        /// ✅ NOVO MÉTODO: Aplica estado inicial para animação (escondido na parte inferior e transparente)
        /// </summary>
        private void ApplyInitialState()
        {
            try
            {
                // ✅ ESTADO INICIAL PERFEITO: elemento começa invisível (opacity=0) e embaixo (TranslationY=60)
                this.IsVisible = true;      // Deve estar visível para poder animar
                this.Opacity = 0.0;        // ✅ CORREÇÃO: Começa COMPLETAMENTE transparente (fade in)
                this.TranslationY = 60;     // ✅ CORREÇÃO: Começa 60px abaixo (translate up)

                System.Diagnostics.Debug.WriteLine($"SpecialButton '{Text ?? "sem nome"}': Estado inicial aplicado (Opacity=0.0, TranslationY=60px) - PRONTO PARA FADE+TRANSLATE");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao aplicar estado inicial: {ex.Message}");
            }
        }

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

        #region Property Changed Handlers

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

        #region Event Handlers

        private async void OnButtonTapped(object sender, EventArgs e)
        {
            try
            {
                // Para a animação especial quando clicado
                if (IsAnimated)
                {
                    await StopSpecialAnimationAsync();
                }

                // Animação de tap (press effect)
                if (IsAnimated && gradientFrame != null && HardwareDetector.SupportsAnimations)
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
                ButtonClicked?.Invoke(this, new SpecialNavButtonEventArgs(Text, CenterContent, CenterIconSource, CommandParameter));

                System.Diagnostics.Debug.WriteLine($"SpecialNavButtonComponent '{Text ?? "sem nome"}' clicado");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no tap do SpecialNavButtonComponent '{Text ?? "sem nome"}': {ex.Message}");
            }
        }

        #endregion

        #region Private Methods

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

        #region Animation Methods

        /// <summary>
        /// ✅ CORRIGIDO: Mostra o botão com múltiplas animações simultâneas
        /// Agora com estado inicial forçado e animações síncronas corretas
        /// </summary>
        public async Task ShowAsync()
        {
            if (_isShown)
            {
                System.Diagnostics.Debug.WriteLine($"SpecialButton '{Text ?? "sem nome"}': ShowAsync ignorado - já mostrado");
                return;
            }

            try
            {
                System.Diagnostics.Debug.WriteLine($"SpecialButton '{Text ?? "sem nome"}': Iniciando ShowAsync - AnimationTypes: {AnimationTypes}, Hardware: {HardwareDetector.SupportsAnimations}");

                // ✅ CORREÇÃO 1: Força estado inicial no MainThread ANTES de qualquer delay
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    this.IsVisible = true;
                    this.Opacity = 0.0;        // ✅ GARANTIA: Completamente transparente para fade in
                    this.TranslationY = 60;     // ✅ GARANTIA: 60px abaixo da posição final para translate up
                    System.Diagnostics.Debug.WriteLine($"SpecialButton '{Text ?? "sem nome"}': Estado inicial FORÇADO (Opacity={this.Opacity}, TranslationY={this.TranslationY})");
                });

                // Aplica delay se configurado (APÓS definir estado inicial)
                if (ShowDelay > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"SpecialButton '{Text ?? "sem nome"}': Aguardando delay de {ShowDelay}ms");
                    await Task.Delay(ShowDelay);
                }

                if (IsAnimated && HardwareDetector.SupportsAnimations && AnimationTypes != SpecialButtonAnimationType.None)
                {
                    System.Diagnostics.Debug.WriteLine($"SpecialButton '{Text ?? "sem nome"}': Condições atendidas - executando animações");

                    // ✅ CORREÇÃO 2: Executa múltiplas animações simultaneamente usando Task.WhenAll
                    var animationTasks = new List<Task>();

                    if (AnimationTypeHelper.HasFlag(AnimationTypes, SpecialButtonAnimationType.Fade))
                    {
                        System.Diagnostics.Debug.WriteLine($"SpecialButton '{Text ?? "sem nome"}': Adicionando Fade à lista de animações");
                        animationTasks.Add(StartFadeInAsync());
                    }

                    if (AnimationTypeHelper.HasFlag(AnimationTypes, SpecialButtonAnimationType.Translate))
                    {
                        System.Diagnostics.Debug.WriteLine($"SpecialButton '{Text ?? "sem nome"}': Adicionando Translate à lista de animações");
                        animationTasks.Add(StartSlideUpAsync());
                    }

                    // ✅ CORREÇÃO 3: Executa todas as animações SIMULTANEAMENTE
                    if (animationTasks.Any())
                    {
                        System.Diagnostics.Debug.WriteLine($"SpecialButton '{Text ?? "sem nome"}': Executando {animationTasks.Count} animações simultaneamente");
                        await Task.WhenAll(animationTasks);
                        System.Diagnostics.Debug.WriteLine($"SpecialButton '{Text ?? "sem nome"}': Todas as {animationTasks.Count} animações concluídas");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"SpecialButton '{Text ?? "sem nome"}': Nenhuma animação configurada para execução");
                    }
                }
                else
                {
                    // Hardware limitado ou animações desabilitadas: apenas aplicar estado final
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        this.Opacity = 1;
                        this.TranslationY = 0;
                    });
                    System.Diagnostics.Debug.WriteLine($"SpecialButton '{Text ?? "sem nome"}': Hardware limitado ou animações desabilitadas - aplicando estado final direto");
                }

                _isShown = true;
                System.Diagnostics.Debug.WriteLine($"SpecialButton '{Text ?? "sem nome"}': ShowAsync concluído com sucesso");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SpecialButton '{Text ?? "sem nome"}': Erro em ShowAsync: {ex.Message}");
                // Fallback: mostrar sem animação
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    this.Opacity = 1;
                    this.TranslationY = 0;
                    this.IsVisible = true;
                });
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
                if (IsAnimated && HardwareDetector.SupportsAnimations && AnimationTypes != SpecialButtonAnimationType.None)
                {
                    // Executa múltiplas animações simultaneamente baseadas nas flags
                    var animationTasks = new List<Task>();

                    if (AnimationTypeHelper.HasFlag(AnimationTypes, SpecialButtonAnimationType.Fade))
                    {
                        animationTasks.Add(StartFadeOutAsync());
                    }

                    if (AnimationTypeHelper.HasFlag(AnimationTypes, SpecialButtonAnimationType.Translate))
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
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        this.Opacity = 0;
                        this.TranslationY = 60;
                    });
                }

                this.IsVisible = false;
                _isShown = false;
                System.Diagnostics.Debug.WriteLine($"SpecialNavButtonComponent '{Text ?? "sem nome"}' escondido com animações: {AnimationTypes}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao esconder SpecialNavButtonComponent '{Text ?? "sem nome"}': {ex.Message}");
                this.Opacity = 0;
                this.IsVisible = false;
                _isShown = false;
            }
        }

        /// <summary>
        /// Inicia animação de Pulse se configurada no AnimationTypes
        /// Usa configuração específica para botão especial (mesmo do Nova Fila original)
        /// Só executa se o hardware suportar animações
        /// </summary>
        public async Task StartSpecialAnimationAsync()
        {
            if (!IsAnimated || !HardwareDetector.SupportsAnimations || buttonContainer == null)
                return;

            try
            {
                if (AnimationTypeHelper.HasFlag(AnimationTypes, SpecialButtonAnimationType.Pulse))
                {
                    // 🎯 Configuração específica para botão especial (mesmo do Nova Fila original)
                    var pulseConfig = new AnimationConfig
                    {
                        FromScale = 1.0,
                        ToScale = 1.25, // 25% maior
                        PulseDuration = 150,
                        PulsePause = 100,
                        PulseCount = 5,
                        InitialDelay = 1000,
                        CycleInterval = 6000,
                        ExpandEasing = Easing.BounceOut,
                        ContractEasing = Easing.BounceIn,
                        AutoRepeat = true
                    };

                    await _animationManager.StartPulseAsync("special_pulse", buttonContainer, pulseConfig, () => this.IsVisible);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro na animação especial: {ex.Message}");
            }
        }

        /// <summary>
        /// Para a animação especial
        /// </summary>
        public async Task StopSpecialAnimationAsync()
        {
            try
            {
                await _animationManager.StopAnimationAsync("special_pulse");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao parar animação especial: {ex.Message}");
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

        /// <summary>
        /// ✅ CORRIGIDO: Fade In usando API nativa do MAUI com duração sincronizada com Translate
        /// EXECUTA EM PARALELO com StartSlideUpAsync() para efeito perfeito
        /// </summary>
        private async Task StartFadeInAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"SpecialButton '{Text ?? "sem nome"}': Iniciando Fade In PARALELO - Estado atual: Opacity={this.Opacity}");

                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    // ✅ GARANTIA: Estado inicial para animação fade
                    this.Opacity = 0.0; // Garante que começa completamente transparente

                    // ✅ FADE IN: 0.0 → 1.0 em 500ms (sincronizado com translate)
                    await this.FadeTo(1.0, 500, Easing.CubicOut);
                });

                System.Diagnostics.Debug.WriteLine($"SpecialButton '{Text ?? "sem nome"}': Fade In PARALELO concluído - Estado final: Opacity={this.Opacity}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no Special Fade In: {ex.Message}");
            }
        }

        private async Task StartFadeOutAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"SpecialButton '{Text ?? "sem nome"}': Iniciando Fade Out PARALELO - Estado atual: Opacity={this.Opacity}");

                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    // ✅ FADE OUT: current → 0.0 em 500ms (sincronizado com translate)
                    await this.FadeTo(0.0, 500, Easing.CubicIn);
                });

                System.Diagnostics.Debug.WriteLine($"SpecialButton '{Text ?? "sem nome"}': Fade Out PARALELO concluído - Estado final: Opacity={this.Opacity}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no Special Fade Out: {ex.Message}");
            }
        }

        /// <summary>
        /// ✅ CORRIGIDO: Slide Up usando API nativa do MAUI sincronizado com Fade
        /// EXECUTA EM PARALELO com StartFadeInAsync() para efeito perfeito
        /// Anima de 60px abaixo (estado inicial) para posição final (0)
        /// </summary>
        private async Task StartSlideUpAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"SpecialButton '{Text ?? "sem nome"}': Iniciando Slide Up PARALELO - Estado atual: TranslationY={this.TranslationY}");

                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    // ✅ GARANTIA: Estado inicial para animação translate
                    this.TranslationY = 60; // Garante que começa 60px abaixo

                    // ✅ TRANSLATE UP: 60px → 0px em 500ms (sincronizado com fade)
                    await this.TranslateTo(0, 0, 500, Easing.CubicOut);
                });

                System.Diagnostics.Debug.WriteLine($"SpecialButton '{Text ?? "sem nome"}': Slide Up PARALELO concluído - Estado final: TranslationY={this.TranslationY}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no Special Slide Up: {ex.Message}");
            }
        }

        /// <summary>
        /// ✅ CORRIGIDO: Slide Down para esconder o botão (sincronizado com fade out)
        /// </summary>
        private async Task StartSlideDownAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"SpecialButton '{Text ?? "sem nome"}': Iniciando Slide Down PARALELO - Estado atual: TranslationY={this.TranslationY}");

                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    // ✅ TRANSLATE DOWN: current → 60px em 500ms (sincronizado com fade out)
                    await this.TranslateTo(0, 60, 500, Easing.CubicIn); // Move para 60px abaixo
                });

                System.Diagnostics.Debug.WriteLine($"SpecialButton '{Text ?? "sem nome"}': Slide Down PARALELO concluído - Estado final: TranslationY={this.TranslationY}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no Special Slide Down: {ex.Message}");
            }
        }

        private async Task AnimateTapEffect()
        {
            try
            {
                if (gradientFrame != null)
                {
                    // Efeito de "press" simples
                    await gradientFrame.ScaleTo(0.95, 100);
                    await gradientFrame.ScaleTo(1.0, 100);
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
                // ✅ CORREÇÃO: Re-aplica estado inicial quando handler estiver disponível
                ApplyInitialState();

                // Atualiza o conteúdo quando o handler estiver disponível
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

            if (BindingContext == null)
            {
                // Para animações quando o contexto muda
                _ = Task.Run(StopAllAnimationsAsync);
            }
        }

        #endregion
    }

    #region Event Args

    public class SpecialNavButtonEventArgs : EventArgs
    {
        public string ButtonText { get; }
        public string CenterContent { get; }
        public string IconSource { get; }
        public object Parameter { get; }

        public SpecialNavButtonEventArgs(string buttonText, string centerContent, string iconSource, object parameter)
        {
            ButtonText = buttonText;
            CenterContent = centerContent;
            IconSource = iconSource;
            Parameter = parameter;
        }
    }

    #endregion
}