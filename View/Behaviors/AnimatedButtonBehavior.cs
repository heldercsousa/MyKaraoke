using MyKaraoke.View.Animations;
using System.Windows.Input;

namespace MyKaraoke.View.Behaviors
{
    /// <summary>
    /// ✅ BEHAVIOR: Adiciona funcionalidades de animação a qualquer ContentView
    /// Elimina completamente duplicação de código entre botões
    /// 🚀 MIGRADO: Usando RobustAnimationManager consistente com NavBarBehavior
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
        // 🚀 MIGRAÇÃO CONSISTENTE: RobustAnimationManager igual ao NavBarBehavior
        private RobustAnimationManager _robustAnimationManager;
        private bool _isShown = false;

        // 🛡️ PROTEÇÃO: Campos Adicionais Anti-Múltiplas Animações
        private bool _isShowInProgress = false;
        private bool _isHideInProgress = false;
        private bool _isSpecialAnimationInProgress = false;
        private readonly object _animationLock = new object();

        #endregion

        #region Behavior Lifecycle

        protected override void OnAttachedTo(ContentView bindable)
        {
            base.OnAttachedTo(bindable);

            _associatedObject = bindable;

            // 🚀 MIGRAÇÃO CONSISTENTE: Usar GlobalAnimationCoordinator como NavBarBehavior
            var pageId = GetPageIdentifier(bindable);
            _robustAnimationManager = GlobalAnimationCoordinator.Instance.GetOrCreateManagerForPage(pageId);

            // 🚀 MIGRAÇÃO CONSISTENTE: Registrar elemento no RobustAnimationManager
            _robustAnimationManager?.RegisterAnimatedElement(bindable);

            // ✅ APLICA ESTADO INICIAL automaticamente
            ApplyInitialState();

            // ✅ ADICIONA MÉTODOS ao objeto
            AddAnimationMethods();

            System.Diagnostics.Debug.WriteLine($"🚀 AnimatedButtonBehavior anexado a {bindable.GetType().Name} com RobustAnimationManager");
        }

        protected override void OnDetachingFrom(ContentView bindable)
        {
            base.OnDetachingFrom(bindable);

            // 🚀 MIGRAÇÃO CONSISTENTE: Dispose via GlobalAnimationCoordinator como NavBarBehavior
            var pageId = GetPageIdentifier(bindable);
            _ = Task.Run(async () =>
            {
                try
                {
                    await GlobalAnimationCoordinator.Instance.DisposeManagerForPage(pageId);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"🛡️ Erro ao disposed RobustAnimationManager: {ex.Message}");
                }
            });

            _associatedObject = null;

            System.Diagnostics.Debug.WriteLine($"🚀 AnimatedButtonBehavior removido de {bindable.GetType().Name}");
        }

        /// <summary>
        /// 🚀 MIGRAÇÃO CONSISTENTE: Obtém identificador único da página igual ao NavBarBehavior
        /// </summary>
        private string GetPageIdentifier(VisualElement element)
        {
            try
            {
                // 🎯 ESTRATÉGIA CONSISTENTE: Sempre usa a página atual ativa como NavBarBehavior
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
                    System.Diagnostics.Debug.WriteLine($"🎯 AnimatedButtonBehavior: GetPageIdentifier (atual) = {pageId}");
                    return pageId;
                }

                // 🛡️ FALLBACK: Se não conseguir obter página atual
                var fallbackId = $"{element.GetType().Name}_{element.GetHashCode()}";
                System.Diagnostics.Debug.WriteLine($"🛡️ AnimatedButtonBehavior: GetPageIdentifier FALLBACK = {fallbackId}");
                return fallbackId;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erro ao obter identificador da página: {ex.Message}");
                return $"Error_{DateTime.Now.Ticks}";
            }
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

        #region Métodos Públicos para Componentes

        public void HandleHandlerChanged()
        {
            try
            {
                if (_associatedObject?.Handler != null)
                {
                    ApplyInitialState();
                    System.Diagnostics.Debug.WriteLine($"AnimatedButtonBehavior: Handler disponível - estado inicial reaplicado");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro em HandleHandlerChanged: {ex.Message}");
            }
        }

        public void HandleBindingContextChanged()
        {
            try
            {
                if (_associatedObject?.BindingContext == null)
                {
                    _ = Task.Run(StopAllAnimationsAsync);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro em HandleBindingContextChanged: {ex.Message}");
            }
        }

        public async Task AnimateTapEffect()
        {
            try
            {
                var target = AnimationContainer ?? _associatedObject;
                if (target != null)
                {
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

        private void AddAnimationMethods()
        {
            if (_associatedObject == null) return;

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

        #region 🚀 MÉTODOS DE ANIMAÇÃO COM ROBUSTANIMATIONMANAGER CONSISTENTE

        public async Task ShowAsync()
        {
            // 🛡️ PROTEÇÃO: Evita múltiplas execuções simultâneas
            lock (_animationLock)
            {
                if (_isShowInProgress || _isShown || _associatedObject == null)
                {
                    System.Diagnostics.Debug.WriteLine("🛡️ AnimatedButtonBehavior: ShowAsync IGNORADO - já em progresso ou mostrado");
                    return;
                }
                _isShowInProgress = true;
            }

            try
            {
                System.Diagnostics.Debug.WriteLine("🚀 AnimatedButtonBehavior: Iniciando ShowAsync com RobustAnimationManager");

                // ✅ FORÇA estado inicial no MainThread ANTES de qualquer delay
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    _associatedObject.IsVisible = true;
                    _associatedObject.Opacity = 0.0;        // GARANTIA: Completamente transparente para fade in
                    _associatedObject.TranslationY = 60;     // GARANTIA: 60px abaixo da posição final para translate up
                    System.Diagnostics.Debug.WriteLine($"AnimatedButtonBehavior: Estado inicial FORÇADO (Opacity={_associatedObject.Opacity}, TranslationY={_associatedObject.TranslationY})");
                });

                // ✅ Aplica delay se configurado
                if (ShowDelay > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"AnimatedButtonBehavior: Aguardando delay de {ShowDelay}ms");
                    await Task.Delay(ShowDelay);
                }

                if (IsAnimated && HardwareDetector.SupportsAnimations)
                {
                    System.Diagnostics.Debug.WriteLine("AnimatedButtonBehavior: Condições atendidas - executando animações");

                    // ✅ Executa múltiplas animações simultaneamente usando Task.WhenAll
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

                    // ✅ Executa todas as animações SIMULTANEAMENTE
                    if (animationTasks.Any())
                    {
                        System.Diagnostics.Debug.WriteLine($"AnimatedButtonBehavior: Executando {animationTasks.Count} animações simultaneamente");

                        // 🛡️ PROTEÇÃO: Timeout para animações (evita travamento)
                        var allAnimationsTask = Task.WhenAll(animationTasks);
                        var timeoutTask = Task.Delay(3000); // 3 segundos máximo

                        var completedTask = await Task.WhenAny(allAnimationsTask, timeoutTask);

                        if (completedTask == timeoutTask)
                        {
                            System.Diagnostics.Debug.WriteLine("🛡️ AnimatedButtonBehavior: TIMEOUT nas animações - aplicando estado final");
                            await MainThread.InvokeOnMainThreadAsync(() =>
                            {
                                _associatedObject.Opacity = 1;
                                _associatedObject.TranslationY = 0;
                            });
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"🚀 AnimatedButtonBehavior: Todas as {animationTasks.Count} animações concluídas com RobustAnimationManager");
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("AnimatedButtonBehavior: Nenhuma animação configurada para execução");
                    }
                }
                else
                {
                    // ✅ Hardware limitado ou animações desabilitadas: apenas aplicar estado final
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        _associatedObject.Opacity = 1;
                        _associatedObject.TranslationY = 0;
                    });
                    System.Diagnostics.Debug.WriteLine("AnimatedButtonBehavior: Hardware limitado ou animações desabilitadas - aplicando estado final direto");
                }

                _isShown = true;
                System.Diagnostics.Debug.WriteLine("🚀 AnimatedButtonBehavior: ShowAsync concluído com sucesso usando RobustAnimationManager");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"🛡️ AnimatedButtonBehavior: Erro em ShowAsync: {ex.Message}");
                // ✅ Fallback
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    _associatedObject.Opacity = 1;
                    _associatedObject.TranslationY = 0;
                    _associatedObject.IsVisible = true;
                });
                _isShown = true;
            }
            finally
            {
                lock (_animationLock)
                {
                    _isShowInProgress = false;
                }
            }
        }

        public async Task HideAsync()
        {
            // 🛡️ PROTEÇÃO: Evita múltiplas execuções simultâneas
            lock (_animationLock)
            {
                if (_isHideInProgress || !_isShown || _associatedObject == null)
                {
                    System.Diagnostics.Debug.WriteLine("🛡️ AnimatedButtonBehavior: HideAsync IGNORADO - já em progresso ou escondido");
                    return;
                }
                _isHideInProgress = true;
            }

            try
            {
                System.Diagnostics.Debug.WriteLine("🚀 AnimatedButtonBehavior: Iniciando HideAsync com RobustAnimationManager");

                // 🚀 MIGRAÇÃO CONSISTENTE: Para TODAS as animações via RobustAnimationManager primeiro
                if (_robustAnimationManager != null)
                {
                    await _robustAnimationManager.StopAllAnimationsCompletely();
                }

                // ✅ Pequeno delay para garantir que parou
                await Task.Delay(50);

                if (IsAnimated && HardwareDetector.SupportsAnimations)
                {
                    // ✅ Executa múltiplas animações simultaneamente baseadas nas flags
                    var animationTasks = new List<Task>();

                    if (HasFadeAnimation)
                    {
                        animationTasks.Add(StartFadeOutAsync());
                    }

                    if (HasTranslateAnimation)
                    {
                        animationTasks.Add(StartSlideDownAsync());
                    }

                    // ✅ Executa todas as animações simultaneamente
                    if (animationTasks.Any())
                    {
                        // 🛡️ PROTEÇÃO: Timeout para animações de saída
                        var allAnimationsTask = Task.WhenAll(animationTasks);
                        var timeoutTask = Task.Delay(2000); // 2 segundos máximo

                        var completedTask = await Task.WhenAny(allAnimationsTask, timeoutTask);

                        if (completedTask == timeoutTask)
                        {
                            System.Diagnostics.Debug.WriteLine("🛡️ AnimatedButtonBehavior: TIMEOUT nas animações de saída - aplicando estado final");
                        }
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
                System.Diagnostics.Debug.WriteLine("🚀 AnimatedButtonBehavior: HideAsync concluído com RobustAnimationManager");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"🛡️ AnimatedButtonBehavior: Erro ao esconder: {ex.Message}");
                _associatedObject.Opacity = 0;
                _associatedObject.IsVisible = false;
                _isShown = false;
            }
            finally
            {
                lock (_animationLock)
                {
                    _isHideInProgress = false;
                }
            }
        }

        public async Task StartSpecialAnimationAsync()
        {
            // 🛡️ PROTEÇÃO: Evita múltiplas animações especiais simultâneas
            lock (_animationLock)
            {
                if (_isSpecialAnimationInProgress || !IsAnimated || !HardwareDetector.SupportsAnimations || !HasPulseAnimation)
                {
                    System.Diagnostics.Debug.WriteLine("🛡️ AnimatedButtonBehavior: StartSpecialAnimationAsync IGNORADO");
                    return;
                }
                _isSpecialAnimationInProgress = true;
            }

            try
            {
                var target = AnimationContainer ?? _associatedObject;
                if (target == null) return;

                System.Diagnostics.Debug.WriteLine("🚀 AnimatedButtonBehavior: Iniciando animação especial com RobustAnimationManager");

                switch (PulseType)
                {
                    case PulseAnimationType.Default:
                        await StartDefaultPulseAsync(target);
                        break;

                    case PulseAnimationType.Special:
                        await StartSpecialPulseAsync(target);
                        break;
                }

                System.Diagnostics.Debug.WriteLine("🚀 AnimatedButtonBehavior: Animação especial concluída com RobustAnimationManager");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"🛡️ AnimatedButtonBehavior: Erro na animação especial: {ex.Message}");
            }
            finally
            {
                lock (_animationLock)
                {
                    _isSpecialAnimationInProgress = false;
                }
            }
        }

        public async Task StopAllAnimationsAsync()
        {
            try
            {
                // 🛡️ RESET: Para flags de controle primeiro
                lock (_animationLock)
                {
                    _isSpecialAnimationInProgress = false;
                }

                System.Diagnostics.Debug.WriteLine("🚀 AnimatedButtonBehavior: Parando todas as animações via RobustAnimationManager");

                // 🚀 MIGRAÇÃO CONSISTENTE: Para RobustAnimationManager primeiro como NavBarBehavior
                if (_robustAnimationManager != null)
                {
                    var stopTask = _robustAnimationManager.StopAllAnimationsCompletely();
                    var timeoutTask = Task.Delay(1000); // 1 segundo máximo

                    var completedTask = await Task.WhenAny(stopTask, timeoutTask);

                    if (completedTask == timeoutTask)
                    {
                        System.Diagnostics.Debug.WriteLine("🛡️ AnimatedButtonBehavior: TIMEOUT ao parar RobustAnimationManager");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("🚀 AnimatedButtonBehavior: RobustAnimationManager parado com sucesso");
                    }
                }

                // ✅ Para animações MAUI também
                if (_associatedObject != null)
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        Microsoft.Maui.Controls.ViewExtensions.CancelAnimations(_associatedObject);
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"🛡️ AnimatedButtonBehavior: Erro ao parar animações: {ex.Message}");
            }
        }

        #endregion

        #region Animações Específicas - MANTIDAS DA VERSÃO ANTERIOR

        /// <summary>
        /// ✅ StartFadeInAsync idêntico da versão anterior
        /// </summary>
        private async Task StartFadeInAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🚀 AnimatedButtonBehavior: Iniciando Fade In PARALELO - Estado atual: Opacity={_associatedObject.Opacity}");

                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    // ✅ Garantia do estado inicial para animação fade
                    _associatedObject.Opacity = 0.0; // Garante que começa completamente transparente

                    // ✅ FADE IN: 0.0 → 1.0 em 500ms (sincronizado com translate)
                    await _associatedObject.FadeTo(1.0, 500, Easing.CubicOut);
                });

                System.Diagnostics.Debug.WriteLine($"🚀 AnimatedButtonBehavior: Fade In PARALELO concluído - Estado final: Opacity={_associatedObject.Opacity}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no Fade In: {ex.Message}");
            }
        }

        /// <summary>
        /// ✅ StartFadeOutAsync idêntico da versão anterior
        /// </summary>
        private async Task StartFadeOutAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🚀 AnimatedButtonBehavior: Iniciando Fade Out PARALELO - Estado atual: Opacity={_associatedObject.Opacity}");

                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    // ✅ FADE OUT: current → 0.0 em 500ms (sincronizado com translate)
                    await _associatedObject.FadeTo(0.0, 500, Easing.CubicIn);
                });

                System.Diagnostics.Debug.WriteLine($"🚀 AnimatedButtonBehavior: Fade Out PARALELO concluído - Estado final: Opacity={_associatedObject.Opacity}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no Fade Out: {ex.Message}");
            }
        }

        /// <summary>
        /// ✅ StartSlideUpAsync idêntico da versão anterior
        /// </summary>
        private async Task StartSlideUpAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🚀 AnimatedButtonBehavior: Iniciando Slide Up PARALELO - Estado atual: TranslationY={_associatedObject.TranslationY}");

                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    // ✅ Garantia do estado inicial para animação translate
                    _associatedObject.TranslationY = 60; // Garante que começa 60px abaixo

                    // ✅ TRANSLATE UP: 60px → 0px em 500ms (sincronizado com fade)
                    await _associatedObject.TranslateTo(0, 0, 500, Easing.CubicOut);
                });

                System.Diagnostics.Debug.WriteLine($"🚀 AnimatedButtonBehavior: Slide Up PARALELO concluído - Estado final: TranslationY={_associatedObject.TranslationY}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no Slide Up: {ex.Message}");
            }
        }

        /// <summary>
        /// ✅ StartSlideDownAsync idêntico da versão anterior
        /// </summary>
        private async Task StartSlideDownAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🚀 AnimatedButtonBehavior: Iniciando Slide Down PARALELO - Estado atual: TranslationY={_associatedObject.TranslationY}");

                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    // ✅ TRANSLATE DOWN: current → 60px em 500ms (sincronizado com fade out)
                    await _associatedObject.TranslateTo(0, 60, 500, Easing.CubicIn); // Move para 60px abaixo
                });

                System.Diagnostics.Debug.WriteLine($"🚀 AnimatedButtonBehavior: Slide Down PARALELO concluído - Estado final: TranslationY={_associatedObject.TranslationY}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no Slide Down: {ex.Message}");
            }
        }

        private async Task StartDefaultPulseAsync(VisualElement target)
        {
            try
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await target.ScaleTo(1.05, 150, Easing.BounceOut);
                    await target.ScaleTo(1.0, 150, Easing.BounceIn);
                });

                System.Diagnostics.Debug.WriteLine("🚀 AnimatedButtonBehavior: Pulse padrão concluído");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no pulse padrão: {ex.Message}");
            }
        }

        private async Task StartSpecialPulseAsync(VisualElement target)
        {
            try
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    for (int i = 0; i < 3; i++)
                    {
                        await target.ScaleTo(1.25, 150, Easing.BounceOut);
                        await target.ScaleTo(1.0, 100, Easing.BounceIn);

                        if (i < 2)
                        {
                            await Task.Delay(100);
                        }
                    }
                });

                System.Diagnostics.Debug.WriteLine("🚀 AnimatedButtonBehavior: Pulse especial concluído");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no pulse especial: {ex.Message}");
            }
        }

        #endregion
    }

    #region Enums

    public enum PulseAnimationType
    {
        Default,
        Special
    }

    #endregion

    #region Extension Methods

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

        public static async Task AnimateTapEffect(this ContentView view)
        {
            var behavior = GetBehavior(view);
            if (behavior != null)
                await behavior.AnimateTapEffect();
            else
                System.Diagnostics.Debug.WriteLine("AnimatedButtonBehavior não encontrado para AnimateTapEffect");
        }

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