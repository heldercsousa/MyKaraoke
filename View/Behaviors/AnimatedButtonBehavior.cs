using MyKaraoke.View.Animations;
using System.Windows.Input;

namespace MyKaraoke.View.Behaviors
{
    /// <summary>
    /// ✅ BEHAVIOR: Adiciona funcionalidades de animação a qualquer ContentView
    /// Elimina completamente duplicação de código entre botões
    /// </summary>
    public class AnimatedButtonBehavior : Behavior<ContentView>
    {
        #region Bindable Properties

        public static readonly BindableProperty IsAnimatedProperty =
            BindableProperty.Create(nameof(IsAnimated), typeof(bool), typeof(AnimatedButtonBehavior), true);

        public static readonly BindableProperty ShowDelayProperty =
            BindableProperty.Create(nameof(ShowDelay), typeof(int), typeof(AnimatedButtonBehavior), 0);

        public static readonly BindableProperty HasFadeAnimationProperty =
            BindableProperty.Create(nameof(HasFadeAnimation), typeof(bool), typeof(AnimatedButtonBehavior), true);

        public static readonly BindableProperty HasTranslateAnimationProperty =
            BindableProperty.Create(nameof(HasTranslateAnimation), typeof(bool), typeof(AnimatedButtonBehavior), true);

        public static readonly BindableProperty HasPulseAnimationProperty =
            BindableProperty.Create(nameof(HasPulseAnimation), typeof(bool), typeof(AnimatedButtonBehavior), false);

        public static readonly BindableProperty PulseTypeProperty =
            BindableProperty.Create(nameof(PulseType), typeof(PulseAnimationType), typeof(AnimatedButtonBehavior), PulseAnimationType.Default);

        public static readonly BindableProperty AnimationContainerProperty =
            BindableProperty.Create(nameof(AnimationContainer), typeof(VisualElement), typeof(AnimatedButtonBehavior), null);

        #endregion

        #region Properties

        public bool IsAnimated
        {
            get => (bool)GetValue(IsAnimatedProperty);
            set => SetValue(IsAnimatedProperty, value);
        }

        public int ShowDelay
        {
            get => (int)GetValue(ShowDelayProperty);
            set => SetValue(ShowDelayProperty, value);
        }

        public bool HasFadeAnimation
        {
            get => (bool)GetValue(HasFadeAnimationProperty);
            set => SetValue(HasFadeAnimationProperty, value);
        }

        public bool HasTranslateAnimation
        {
            get => (bool)GetValue(HasTranslateAnimationProperty);
            set => SetValue(HasTranslateAnimationProperty, value);
        }

        public bool HasPulseAnimation
        {
            get => (bool)GetValue(HasPulseAnimationProperty);
            set => SetValue(HasPulseAnimationProperty, value);
        }

        public PulseAnimationType PulseType
        {
            get => (PulseAnimationType)GetValue(PulseTypeProperty);
            set => SetValue(PulseTypeProperty, value);
        }

        public VisualElement AnimationContainer
        {
            get => (VisualElement)GetValue(AnimationContainerProperty);
            set => SetValue(AnimationContainerProperty, value);
        }

        #endregion

        #region Private Fields

        private ContentView _associatedObject;
        private AnimationManager _animationManager;
        private bool _isShown = false;

        #endregion

        #region Behavior Lifecycle

        protected override void OnAttachedTo(ContentView bindable)
        {
            base.OnAttachedTo(bindable);

            _associatedObject = bindable;
            _animationManager = new AnimationManager($"AnimatedButton_{bindable.GetHashCode()}");

            // ✅ APLICA ESTADO INICIAL automaticamente
            ApplyInitialState();

            // ✅ ADICIONA MÉTODOS ao objeto
            AddAnimationMethods();

            System.Diagnostics.Debug.WriteLine($"AnimatedButtonBehavior anexado a {bindable.GetType().Name}");
        }

        protected override void OnDetachingFrom(ContentView bindable)
        {
            base.OnDetachingFrom(bindable);

            // ✅ LIMPA RECURSOS
            _animationManager?.Dispose();
            _associatedObject = null;

            System.Diagnostics.Debug.WriteLine($"AnimatedButtonBehavior removido de {bindable.GetType().Name}");
        }

        #endregion

        #region Estado Inicial

        private void ApplyInitialState()
        {
            try
            {
                if (_associatedObject != null)
                {
                    _associatedObject.IsVisible = true;
                    _associatedObject.Opacity = 0.0;
                    _associatedObject.TranslationY = 60;

                    System.Diagnostics.Debug.WriteLine($"AnimatedButtonBehavior: Estado inicial aplicado");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao aplicar estado inicial: {ex.Message}");
            }
        }

        #endregion

        #region Métodos Públicos para Componentes - MIGRADOS DOS COMPONENTES

        /// <summary>
        /// ✅ MIGRADO: OnHandlerChanged comum dos dois componentes
        /// </summary>
        public void HandleHandlerChanged()
        {
            try
            {
                if (_associatedObject?.Handler != null)
                {
                    // Re-aplica estado inicial quando handler estiver disponível
                    ApplyInitialState();
                    System.Diagnostics.Debug.WriteLine($"AnimatedButtonBehavior: Handler disponível - estado inicial reaplicado");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro em HandleHandlerChanged: {ex.Message}");
            }
        }

        /// <summary>
        /// ✅ MIGRADO: OnBindingContextChanged comum dos dois componentes
        /// </summary>
        public void HandleBindingContextChanged()
        {
            try
            {
                if (_associatedObject?.BindingContext == null)
                {
                    // Para animações quando o contexto muda
                    _ = Task.Run(StopAllAnimationsAsync);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro em HandleBindingContextChanged: {ex.Message}");
            }
        }

        /// <summary>
        /// ✅ MIGRADO: Tap Effect comum dos dois componentes
        /// </summary>
        public async Task AnimateTapEffect()
        {
            try
            {
                var target = AnimationContainer ?? _associatedObject;
                if (target != null)
                {
                    // Efeito de "press" simples
                    await target.ScaleTo(0.95, 100);
                    await target.ScaleTo(1.0, 100);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no efeito de tap: {ex.Message}");
            }
        }

        #endregion

        #region Adicionar Métodos ao Objeto

        /// <summary>
        /// ✅ MAGIA: Adiciona métodos dinâmicos ao ContentView
        /// </summary>
        private void AddAnimationMethods()
        {
            if (_associatedObject == null) return;

            // ✅ Adiciona ShowAsync como extensão
            _associatedObject.SetValue(ShowAsyncMethodProperty, new Func<Task>(ShowAsync));
            _associatedObject.SetValue(HideAsyncMethodProperty, new Func<Task>(HideAsync));
            _associatedObject.SetValue(StartSpecialAnimationAsyncMethodProperty, new Func<Task>(StartSpecialAnimationAsync));
            _associatedObject.SetValue(StopAllAnimationsAsyncMethodProperty, new Func<Task>(StopAllAnimationsAsync));
        }

        #endregion

        #region Attached Properties para Métodos

        public static readonly BindableProperty ShowAsyncMethodProperty =
            BindableProperty.CreateAttached("ShowAsyncMethod", typeof(Func<Task>), typeof(AnimatedButtonBehavior), null);

        public static readonly BindableProperty HideAsyncMethodProperty =
            BindableProperty.CreateAttached("HideAsyncMethod", typeof(Func<Task>), typeof(AnimatedButtonBehavior), null);

        public static readonly BindableProperty StartSpecialAnimationAsyncMethodProperty =
            BindableProperty.CreateAttached("StartSpecialAnimationAsyncMethod", typeof(Func<Task>), typeof(AnimatedButtonBehavior), null);

        public static readonly BindableProperty StopAllAnimationsAsyncMethodProperty =
            BindableProperty.CreateAttached("StopAllAnimationsAsyncMethod", typeof(Func<Task>), typeof(AnimatedButtonBehavior), null);

        #endregion

        #region Métodos de Animação

        /// <summary>
        /// ✅ MIGRADO: ShowAsync completo dos componentes originais
        /// </summary>
        public async Task ShowAsync()
        {
            if (_isShown || _associatedObject == null)
            {
                System.Diagnostics.Debug.WriteLine("AnimatedButtonBehavior: ShowAsync ignorado - já mostrado");
                return;
            }

            try
            {
                System.Diagnostics.Debug.WriteLine("AnimatedButtonBehavior: Iniciando ShowAsync");

                // ✅ MIGRADO: Força estado inicial no MainThread ANTES de qualquer delay
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    _associatedObject.IsVisible = true;
                    _associatedObject.Opacity = 0.0;        // GARANTIA: Completamente transparente para fade in
                    _associatedObject.TranslationY = 60;     // GARANTIA: 60px abaixo da posição final para translate up
                    System.Diagnostics.Debug.WriteLine($"AnimatedButtonBehavior: Estado inicial FORÇADO (Opacity={_associatedObject.Opacity}, TranslationY={_associatedObject.TranslationY})");
                });

                // ✅ MIGRADO: Aplica delay se configurado
                if (ShowDelay > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"AnimatedButtonBehavior: Aguardando delay de {ShowDelay}ms");
                    await Task.Delay(ShowDelay);
                }

                if (IsAnimated && HardwareDetector.SupportsAnimations)
                {
                    System.Diagnostics.Debug.WriteLine("AnimatedButtonBehavior: Condições atendidas - executando animações");

                    // ✅ MIGRADO: Executa múltiplas animações simultaneamente usando Task.WhenAll
                    var animationTasks = new List<Task>();

                    if (HasFadeAnimation)
                    {
                        System.Diagnostics.Debug.WriteLine("AnimatedButtonBehavior: Adicionando Fade à lista de animações");
                        animationTasks.Add(StartFadeInAsync());
                    }

                    if (HasTranslateAnimation)
                    {
                        System.Diagnostics.Debug.WriteLine("AnimatedButtonBehavior: Adicionando Translate à lista de animações");
                        animationTasks.Add(StartSlideUpAsync());
                    }

                    // ✅ MIGRADO: Executa todas as animações SIMULTANEAMENTE
                    if (animationTasks.Any())
                    {
                        System.Diagnostics.Debug.WriteLine($"AnimatedButtonBehavior: Executando {animationTasks.Count} animações simultaneamente");
                        await Task.WhenAll(animationTasks);
                        System.Diagnostics.Debug.WriteLine($"AnimatedButtonBehavior: Todas as {animationTasks.Count} animações concluídas");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("AnimatedButtonBehavior: Nenhuma animação configurada para execução");
                    }
                }
                else
                {
                    // ✅ MIGRADO: Hardware limitado ou animações desabilitadas: apenas aplicar estado final
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        _associatedObject.Opacity = 1;
                        _associatedObject.TranslationY = 0;
                    });
                    System.Diagnostics.Debug.WriteLine("AnimatedButtonBehavior: Hardware limitado ou animações desabilitadas - aplicando estado final direto");
                }

                _isShown = true;
                System.Diagnostics.Debug.WriteLine("AnimatedButtonBehavior: ShowAsync concluído com sucesso");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AnimatedButtonBehavior: Erro em ShowAsync: {ex.Message}");
                // ✅ MIGRADO: Fallback
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    _associatedObject.Opacity = 1;
                    _associatedObject.TranslationY = 0;
                    _associatedObject.IsVisible = true;
                });
                _isShown = true;
            }
        }

        /// <summary>
        /// ✅ MIGRADO: HideAsync completo dos componentes originais
        /// </summary>
        public async Task HideAsync()
        {
            if (!_isShown || _associatedObject == null)
                return;

            try
            {
                if (IsAnimated && HardwareDetector.SupportsAnimations)
                {
                    // ✅ MIGRADO: Executa múltiplas animações simultaneamente baseadas nas flags
                    var animationTasks = new List<Task>();

                    if (HasFadeAnimation)
                    {
                        animationTasks.Add(StartFadeOutAsync());
                    }

                    if (HasTranslateAnimation)
                    {
                        animationTasks.Add(StartSlideDownAsync());
                    }

                    // ✅ MIGRADO: Executa todas as animações simultaneamente
                    if (animationTasks.Any())
                    {
                        await Task.WhenAll(animationTasks);
                    }
                }
                else
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        _associatedObject.Opacity = 0;
                        _associatedObject.TranslationY = 60;
                    });
                }

                _associatedObject.IsVisible = false;
                _isShown = false;
                System.Diagnostics.Debug.WriteLine("AnimatedButtonBehavior: HideAsync concluído");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao esconder: {ex.Message}");
                _associatedObject.Opacity = 0;
                _associatedObject.IsVisible = false;
                _isShown = false;
            }
        }

        /// <summary>
        /// ✅ PULSE: Baseado no PulseType
        /// </summary>
        public async Task StartSpecialAnimationAsync()
        {
            if (!IsAnimated || !HardwareDetector.SupportsAnimations || !HasPulseAnimation)
                return;

            try
            {
                var target = AnimationContainer ?? _associatedObject;
                if (target == null) return;

                switch (PulseType)
                {
                    case PulseAnimationType.Default:
                        await _animationManager.StartCallToActionAsync("pulse", target, () => _associatedObject.IsVisible);
                        break;

                    case PulseAnimationType.Special:
                        var pulseConfig = new AnimationConfig
                        {
                            FromScale = 1.0,
                            ToScale = 1.25,
                            PulseDuration = 150,
                            PulsePause = 100,
                            PulseCount = 5,
                            InitialDelay = 1000,
                            CycleInterval = 6000,
                            ExpandEasing = Easing.BounceOut,
                            ContractEasing = Easing.BounceIn,
                            AutoRepeat = true
                        };
                        await _animationManager.StartPulseAsync("special_pulse", target, pulseConfig, () => _associatedObject.IsVisible);
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro na animação especial: {ex.Message}");
            }
        }

        /// <summary>
        /// ✅ STOP ALL: Para todas as animações
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

        #endregion

        #region Animações Específicas - MIGRADAS DOS COMPONENTES

        /// <summary>
        /// ✅ MIGRADO: StartFadeInAsync idêntico dos dois componentes
        /// </summary>
        private async Task StartFadeInAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"AnimatedButtonBehavior: Iniciando Fade In PARALELO - Estado atual: Opacity={_associatedObject.Opacity}");

                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    // ✅ MIGRADO: Garantia do estado inicial para animação fade
                    _associatedObject.Opacity = 0.0; // Garante que começa completamente transparente

                    // ✅ MIGRADO: FADE IN: 0.0 → 1.0 em 500ms (sincronizado com translate)
                    await _associatedObject.FadeTo(1.0, 500, Easing.CubicOut);
                });

                System.Diagnostics.Debug.WriteLine($"AnimatedButtonBehavior: Fade In PARALELO concluído - Estado final: Opacity={_associatedObject.Opacity}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no Fade In: {ex.Message}");
            }
        }

        /// <summary>
        /// ✅ MIGRADO: StartFadeOutAsync idêntico dos dois componentes
        /// </summary>
        private async Task StartFadeOutAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"AnimatedButtonBehavior: Iniciando Fade Out PARALELO - Estado atual: Opacity={_associatedObject.Opacity}");

                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    // ✅ MIGRADO: FADE OUT: current → 0.0 em 500ms (sincronizado com translate)
                    await _associatedObject.FadeTo(0.0, 500, Easing.CubicIn);
                });

                System.Diagnostics.Debug.WriteLine($"AnimatedButtonBehavior: Fade Out PARALELO concluído - Estado final: Opacity={_associatedObject.Opacity}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no Fade Out: {ex.Message}");
            }
        }

        /// <summary>
        /// ✅ MIGRADO: StartSlideUpAsync idêntico dos dois componentes
        /// </summary>
        private async Task StartSlideUpAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"AnimatedButtonBehavior: Iniciando Slide Up PARALELO - Estado atual: TranslationY={_associatedObject.TranslationY}");

                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    // ✅ MIGRADO: Garantia do estado inicial para animação translate
                    _associatedObject.TranslationY = 60; // Garante que começa 60px abaixo

                    // ✅ MIGRADO: TRANSLATE UP: 60px → 0px em 500ms (sincronizado com fade)
                    await _associatedObject.TranslateTo(0, 0, 500, Easing.CubicOut);
                });

                System.Diagnostics.Debug.WriteLine($"AnimatedButtonBehavior: Slide Up PARALELO concluído - Estado final: TranslationY={_associatedObject.TranslationY}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no Slide Up: {ex.Message}");
            }
        }

        /// <summary>
        /// ✅ MIGRADO: StartSlideDownAsync idêntico dos dois componentes
        /// </summary>
        private async Task StartSlideDownAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"AnimatedButtonBehavior: Iniciando Slide Down PARALELO - Estado atual: TranslationY={_associatedObject.TranslationY}");

                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    // ✅ MIGRADO: TRANSLATE DOWN: current → 60px em 500ms (sincronizado com fade out)
                    await _associatedObject.TranslateTo(0, 60, 500, Easing.CubicIn); // Move para 60px abaixo
                });

                System.Diagnostics.Debug.WriteLine($"AnimatedButtonBehavior: Slide Down PARALELO concluído - Estado final: TranslationY={_associatedObject.TranslationY}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no Slide Down: {ex.Message}");
            }
        }

        #endregion
    }

    #region Enums

    /// <summary>
    /// Tipos de pulse disponíveis
    /// </summary>
    public enum PulseAnimationType
    {
        Default,  // Pulse padrão (5% maior)
        Special   // Pulse especial (25% maior, bounce)
    }

    #endregion

    #region Extension Methods

    /// <summary>
    /// ✅ EXTENSIONS: Para facilitar chamada dos métodos + NOVOS MÉTODOS MIGRADOS
    /// </summary>
    public static class AnimatedButtonExtensions
    {
        public static async Task ShowAsync(this ContentView view)
        {
            var method = (Func<Task>)view.GetValue(AnimatedButtonBehavior.ShowAsyncMethodProperty);
            if (method != null)
                await method();
        }

        public static async Task HideAsync(this ContentView view)
        {
            var method = (Func<Task>)view.GetValue(AnimatedButtonBehavior.HideAsyncMethodProperty);
            if (method != null)
                await method();
        }

        public static async Task StartSpecialAnimationAsync(this ContentView view)
        {
            var method = (Func<Task>)view.GetValue(AnimatedButtonBehavior.StartSpecialAnimationAsyncMethodProperty);
            if (method != null)
                await method();
        }

        public static async Task StopAllAnimationsAsync(this ContentView view)
        {
            var method = (Func<Task>)view.GetValue(AnimatedButtonBehavior.StopAllAnimationsAsyncMethodProperty);
            if (method != null)
                await method();
        }

        /// <summary>
        /// ✅ NOVO: Tap Effect via Behavior
        /// </summary>
        public static async Task AnimateTapEffect(this ContentView view)
        {
            var behavior = GetBehavior(view);
            if (behavior != null)
                await behavior.AnimateTapEffect();
            else
                System.Diagnostics.Debug.WriteLine("AnimatedButtonBehavior não encontrado para AnimateTapEffect");
        }

        /// <summary>
        /// ✅ NOVO: Handler Changed via Behavior
        /// </summary>
        public static void HandleHandlerChanged(this ContentView view)
        {
            var behavior = GetBehavior(view);
            if (behavior != null)
            {
                behavior.HandleHandlerChanged();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("AnimatedButtonBehavior não encontrado para HandleHandlerChanged");
            }
        }

        /// <summary>
        /// ✅ NOVO: Binding Context Changed via Behavior
        /// </summary>
        public static void HandleBindingContextChanged(this ContentView view)
        {
            var behavior = GetBehavior(view);
            if (behavior != null)
            {
                behavior.HandleBindingContextChanged();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("AnimatedButtonBehavior não encontrado para HandleBindingContextChanged");
            }
        }

        /// <summary>
        /// Helper para obter o Behavior anexado
        /// </summary>
        private static AnimatedButtonBehavior GetBehavior(ContentView view)
        {
            if (view?.Behaviors == null) return null;

            try
            {
                return view.Behaviors.OfType<AnimatedButtonBehavior>().FirstOrDefault();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao obter AnimatedButtonBehavior: {ex.Message}");
                return null;
            }
        }
    }

    #endregion
}