namespace MyKaraoke.View.Animations
{
    /// <summary>
    /// Gerenciador centralizado de animações para Views
    /// Elimina duplicação de código e facilita o controle de múltiplas animações
    /// Versão estendida com suporte a Fade e Translate animations
    /// </summary>
    public class AnimationManager : IDisposable
    {
        private readonly Dictionary<string, PulseAnimation> _pulseAnimations = new();
        private readonly Dictionary<string, FadeAnimation> _fadeAnimations = new();
        private readonly Dictionary<string, TranslateAnimation> _translateAnimations = new();
        private readonly string _viewName;
        private bool _disposed = false;

        /// <summary>
        /// Evento disparado quando qualquer animação inicia
        /// </summary>
        public event EventHandler<AnimationEventArgs> AnimationStarted;

        /// <summary>
        /// Evento disparado quando qualquer animação para
        /// </summary>
        public event EventHandler<AnimationEventArgs> AnimationStopped;

        /// <summary>
        /// Cria um novo gerenciador de animações
        /// </summary>
        /// <param name="viewName">Nome da view (para debug)</param>
        public AnimationManager(string viewName = "UnknownView")
        {
            _viewName = viewName;
            System.Diagnostics.Debug.WriteLine($"AnimationManager criado para: {_viewName}");
        }

        #region Pulse Animations

        /// <summary>
        /// Adiciona e inicia uma animação de pulse
        /// </summary>
        /// <param name="animationKey">Chave única para a animação</param>
        /// <param name="target">Elemento a ser animado</param>
        /// <param name="config">Configuração da animação (opcional)</param>
        /// <param name="shouldContinue">Condição para continuar animando (opcional)</param>
        public async Task StartPulseAsync(string animationKey, VisualElement target, AnimationConfig config = null, Func<bool> shouldContinue = null)
        {
            if (_disposed || string.IsNullOrEmpty(animationKey) || target == null)
                return;

            try
            {
                // Para animação existente se houver
                await StopPulseAnimationAsync(animationKey);

                // Cria nova animação
                var animation = new PulseAnimation(
                    target: target,
                    config: config ?? AnimationConfig.CallToAction,
                    shouldContinue: shouldContinue ?? (() => true)
                );

                // Event handlers
                animation.AnimationStarted += (s, e) =>
                {
                    System.Diagnostics.Debug.WriteLine($"[{_viewName}] Pulse '{animationKey}' iniciada");
                    AnimationStarted?.Invoke(this, new AnimationEventArgs(animationKey, target));
                };

                animation.AnimationStopped += (s, e) =>
                {
                    System.Diagnostics.Debug.WriteLine($"[{_viewName}] Pulse '{animationKey}' parada");
                    AnimationStopped?.Invoke(this, new AnimationEventArgs(animationKey, target));
                };

                // Armazena a animação
                _pulseAnimations[animationKey] = animation;

                // Inicia a animação
                await animation.StartAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[{_viewName}] Erro ao iniciar pulse '{animationKey}': {ex.Message}");
            }
        }

        /// <summary>
        /// Para uma animação de pulse específica
        /// </summary>
        /// <param name="animationKey">Chave da animação</param>
        public async Task StopPulseAnimationAsync(string animationKey)
        {
            if (string.IsNullOrEmpty(animationKey) || !_pulseAnimations.ContainsKey(animationKey))
                return;

            try
            {
                var animation = _pulseAnimations[animationKey];
                await animation.StopAsync();
                animation.Dispose();
                _pulseAnimations.Remove(animationKey);

                System.Diagnostics.Debug.WriteLine($"[{_viewName}] Pulse '{animationKey}' removida");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[{_viewName}] Erro ao parar pulse '{animationKey}': {ex.Message}");
            }
        }

        #endregion

        #region Fade Animations

        /// <summary>
        /// Adiciona e inicia uma animação de fade in
        /// </summary>
        /// <param name="animationKey">Chave única para a animação</param>
        /// <param name="target">Elemento a ser animado</param>
        /// <param name="duration">Duração da animação em ms (padrão: 300ms)</param>
        /// <param name="shouldContinue">Condição para continuar animando (opcional)</param>
        public async Task StartFadeInAsync(string animationKey, VisualElement target, uint duration = 300, Func<bool> shouldContinue = null)
        {
            if (_disposed || string.IsNullOrEmpty(animationKey) || target == null)
                return;

            try
            {
                // Para animação existente se houver
                await StopFadeAnimationAsync(animationKey);

                // Cria nova animação
                var animation = FadeAnimation.CreateFadeIn(target, duration, shouldContinue);

                // Event handlers
                animation.AnimationStarted += (s, e) =>
                {
                    System.Diagnostics.Debug.WriteLine($"[{_viewName}] Fade In '{animationKey}' iniciada");
                    AnimationStarted?.Invoke(this, new AnimationEventArgs(animationKey, target));
                };

                animation.AnimationStopped += (s, e) =>
                {
                    System.Diagnostics.Debug.WriteLine($"[{_viewName}] Fade In '{animationKey}' parada");
                    AnimationStopped?.Invoke(this, new AnimationEventArgs(animationKey, target));
                };

                // Armazena a animação
                _fadeAnimations[animationKey] = animation;

                // Inicia a animação
                await animation.StartAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[{_viewName}] Erro ao iniciar fade in '{animationKey}': {ex.Message}");
            }
        }

        /// <summary>
        /// Adiciona e inicia uma animação de fade out
        /// </summary>
        /// <param name="animationKey">Chave única para a animação</param>
        /// <param name="target">Elemento a ser animado</param>
        /// <param name="duration">Duração da animação em ms (padrão: 300ms)</param>
        /// <param name="shouldContinue">Condição para continuar animando (opcional)</param>
        public async Task StartFadeOutAsync(string animationKey, VisualElement target, uint duration = 300, Func<bool> shouldContinue = null)
        {
            if (_disposed || string.IsNullOrEmpty(animationKey) || target == null)
                return;

            try
            {
                // Para animação existente se houver
                await StopFadeAnimationAsync(animationKey);

                // Cria nova animação
                var animation = FadeAnimation.CreateFadeOut(target, duration, shouldContinue);

                // Event handlers
                animation.AnimationStarted += (s, e) =>
                {
                    System.Diagnostics.Debug.WriteLine($"[{_viewName}] Fade Out '{animationKey}' iniciada");
                    AnimationStarted?.Invoke(this, new AnimationEventArgs(animationKey, target));
                };

                animation.AnimationStopped += (s, e) =>
                {
                    System.Diagnostics.Debug.WriteLine($"[{_viewName}] Fade Out '{animationKey}' parada");
                    AnimationStopped?.Invoke(this, new AnimationEventArgs(animationKey, target));
                };

                // Armazena a animação
                _fadeAnimations[animationKey] = animation;

                // Inicia a animação
                await animation.StartAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[{_viewName}] Erro ao iniciar fade out '{animationKey}': {ex.Message}");
            }
        }

        /// <summary>
        /// Para uma animação de fade específica
        /// </summary>
        /// <param name="animationKey">Chave da animação</param>
        public async Task StopFadeAnimationAsync(string animationKey)
        {
            if (string.IsNullOrEmpty(animationKey) || !_fadeAnimations.ContainsKey(animationKey))
                return;

            try
            {
                var animation = _fadeAnimations[animationKey];
                await animation.StopAsync();
                animation.Dispose();
                _fadeAnimations.Remove(animationKey);

                System.Diagnostics.Debug.WriteLine($"[{_viewName}] Fade '{animationKey}' removida");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[{_viewName}] Erro ao parar fade '{animationKey}': {ex.Message}");
            }
        }

        #endregion

        #region Translate Animations

        /// <summary>
        /// Adiciona e inicia uma animação de slide up
        /// </summary>
        /// <param name="animationKey">Chave única para a animação</param>
        /// <param name="target">Elemento a ser animado</param>
        /// <param name="distance">Distância do movimento (padrão: 20px)</param>
        /// <param name="duration">Duração da animação em ms (padrão: 300ms)</param>
        /// <param name="shouldContinue">Condição para continuar animando (opcional)</param>
        public async Task StartSlideUpAsync(string animationKey, VisualElement target, double distance = 20, uint duration = 300, Func<bool> shouldContinue = null)
        {
            if (_disposed || string.IsNullOrEmpty(animationKey) || target == null)
                return;

            try
            {
                // Para animação existente se houver
                await StopTranslateAnimationAsync(animationKey);

                // Cria nova animação
                var animation = TranslateAnimation.CreateSlideUp(target, distance, duration, shouldContinue);

                // Event handlers
                animation.AnimationStarted += (s, e) =>
                {
                    System.Diagnostics.Debug.WriteLine($"[{_viewName}] Slide Up '{animationKey}' iniciada");
                    AnimationStarted?.Invoke(this, new AnimationEventArgs(animationKey, target));
                };

                animation.AnimationStopped += (s, e) =>
                {
                    System.Diagnostics.Debug.WriteLine($"[{_viewName}] Slide Up '{animationKey}' parada");
                    AnimationStopped?.Invoke(this, new AnimationEventArgs(animationKey, target));
                };

                // Armazena a animação
                _translateAnimations[animationKey] = animation;

                // Inicia a animação
                await animation.StartAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[{_viewName}] Erro ao iniciar slide up '{animationKey}': {ex.Message}");
            }
        }

        /// <summary>
        /// Adiciona e inicia uma animação de slide down
        /// </summary>
        /// <param name="animationKey">Chave única para a animação</param>
        /// <param name="target">Elemento a ser animado</param>
        /// <param name="distance">Distância do movimento (padrão: 20px)</param>
        /// <param name="duration">Duração da animação em ms (padrão: 300ms)</param>
        /// <param name="shouldContinue">Condição para continuar animando (opcional)</param>
        public async Task StartSlideDownAsync(string animationKey, VisualElement target, double distance = 20, uint duration = 300, Func<bool> shouldContinue = null)
        {
            if (_disposed || string.IsNullOrEmpty(animationKey) || target == null)
                return;

            try
            {
                // Para animação existente se houver
                await StopTranslateAnimationAsync(animationKey);

                // Cria nova animação
                var animation = TranslateAnimation.CreateSlideDown(target, distance, duration, shouldContinue);

                // Event handlers
                animation.AnimationStarted += (s, e) =>
                {
                    System.Diagnostics.Debug.WriteLine($"[{_viewName}] Slide Down '{animationKey}' iniciada");
                    AnimationStarted?.Invoke(this, new AnimationEventArgs(animationKey, target));
                };

                animation.AnimationStopped += (s, e) =>
                {
                    System.Diagnostics.Debug.WriteLine($"[{_viewName}] Slide Down '{animationKey}' parada");
                    AnimationStopped?.Invoke(this, new AnimationEventArgs(animationKey, target));
                };

                // Armazena a animação
                _translateAnimations[animationKey] = animation;

                // Inicia a animação
                await animation.StartAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[{_viewName}] Erro ao iniciar slide down '{animationKey}': {ex.Message}");
            }
        }

        /// <summary>
        /// Adiciona e inicia uma animação de shake
        /// </summary>
        /// <param name="animationKey">Chave única para a animação</param>
        /// <param name="target">Elemento a ser animado</param>
        /// <param name="intensity">Intensidade do shake (padrão: 10px)</param>
        /// <param name="duration">Duração da animação em ms (padrão: 500ms)</param>
        /// <param name="shouldContinue">Condição para continuar animando (opcional)</param>
        public async Task StartShakeAsync(string animationKey, VisualElement target, double intensity = 10, uint duration = 500, Func<bool> shouldContinue = null)
        {
            if (_disposed || string.IsNullOrEmpty(animationKey) || target == null)
                return;

            try
            {
                // Para animação existente se houver
                await StopTranslateAnimationAsync(animationKey);

                // Cria nova animação
                var animation = TranslateAnimation.CreateShake(target, intensity, duration, shouldContinue);

                // Event handlers
                animation.AnimationStarted += (s, e) =>
                {
                    System.Diagnostics.Debug.WriteLine($"[{_viewName}] Shake '{animationKey}' iniciada");
                    AnimationStarted?.Invoke(this, new AnimationEventArgs(animationKey, target));
                };

                animation.AnimationStopped += (s, e) =>
                {
                    System.Diagnostics.Debug.WriteLine($"[{_viewName}] Shake '{animationKey}' parada");
                    AnimationStopped?.Invoke(this, new AnimationEventArgs(animationKey, target));
                };

                // Armazena a animação
                _translateAnimations[animationKey] = animation;

                // Inicia a animação
                await animation.StartAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[{_viewName}] Erro ao iniciar shake '{animationKey}': {ex.Message}");
            }
        }

        /// <summary>
        /// Para uma animação de translate específica
        /// </summary>
        /// <param name="animationKey">Chave da animação</param>
        public async Task StopTranslateAnimationAsync(string animationKey)
        {
            if (string.IsNullOrEmpty(animationKey) || !_translateAnimations.ContainsKey(animationKey))
                return;

            try
            {
                var animation = _translateAnimations[animationKey];
                await animation.StopAsync();
                animation.Dispose();
                _translateAnimations.Remove(animationKey);

                System.Diagnostics.Debug.WriteLine($"[{_viewName}] Translate '{animationKey}' removida");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[{_viewName}] Erro ao parar translate '{animationKey}': {ex.Message}");
            }
        }

        #endregion

        #region Combined Animations

        /// <summary>
        /// Inicia animação combinada de fade in + slide up
        /// </summary>
        /// <param name="baseKey">Chave base para as animações (será sufixada com _fade e _slide)</param>
        /// <param name="target">Elemento a ser animado</param>
        /// <param name="duration">Duração das animações em ms (padrão: 300ms)</param>
        /// <param name="distance">Distância do slide (padrão: 20px)</param>
        public async Task StartShowAsync(string baseKey, VisualElement target, uint duration = 300, double distance = 20)
        {
            if (_disposed || string.IsNullOrEmpty(baseKey) || target == null)
                return;

            try
            {
                var fadeTask = StartFadeInAsync($"{baseKey}_fade", target, duration);
                var slideTask = StartSlideUpAsync($"{baseKey}_slide", target, distance, duration);

                await Task.WhenAll(fadeTask, slideTask);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[{_viewName}] Erro na animação combinada show '{baseKey}': {ex.Message}");
            }
        }

        /// <summary>
        /// Inicia animação combinada de fade out + slide down
        /// </summary>
        /// <param name="baseKey">Chave base para as animações (será sufixada com _fade e _slide)</param>
        /// <param name="target">Elemento a ser animado</param>
        /// <param name="duration">Duração das animações em ms (padrão: 300ms)</param>
        /// <param name="distance">Distância do slide (padrão: 20px)</param>
        public async Task StartHideAsync(string baseKey, VisualElement target, uint duration = 300, double distance = 20)
        {
            if (_disposed || string.IsNullOrEmpty(baseKey) || target == null)
                return;

            try
            {
                var fadeTask = StartFadeOutAsync($"{baseKey}_fade", target, duration);
                var slideTask = StartSlideDownAsync($"{baseKey}_slide", target, distance, duration);

                await Task.WhenAll(fadeTask, slideTask);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[{_viewName}] Erro na animação combinada hide '{baseKey}': {ex.Message}");
            }
        }

        #endregion

        #region General Methods

        /// <summary>
        /// Para uma animação específica (qualquer tipo)
        /// </summary>
        /// <param name="animationKey">Chave da animação</param>
        public async Task StopAnimationAsync(string animationKey)
        {
            var tasks = new List<Task>();

            if (_pulseAnimations.ContainsKey(animationKey))
                tasks.Add(StopPulseAnimationAsync(animationKey));

            if (_fadeAnimations.ContainsKey(animationKey))
                tasks.Add(StopFadeAnimationAsync(animationKey));

            if (_translateAnimations.ContainsKey(animationKey))
                tasks.Add(StopTranslateAnimationAsync(animationKey));

            if (tasks.Any())
                await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Para todas as animações
        /// </summary>
        public async Task StopAllAnimationsAsync()
        {
            var tasks = new List<Task>();

            // Para todas as animações de pulse
            foreach (var key in _pulseAnimations.Keys.ToList())
                tasks.Add(StopPulseAnimationAsync(key));

            // Para todas as animações de fade
            foreach (var key in _fadeAnimations.Keys.ToList())
                tasks.Add(StopFadeAnimationAsync(key));

            // Para todas as animações de translate
            foreach (var key in _translateAnimations.Keys.ToList())
                tasks.Add(StopTranslateAnimationAsync(key));

            if (tasks.Any())
                await Task.WhenAll(tasks);

            System.Diagnostics.Debug.WriteLine($"[{_viewName}] Todas as animações paradas");
        }

        /// <summary>
        /// Verifica se uma animação está rodando
        /// </summary>
        /// <param name="animationKey">Chave da animação</param>
        public bool IsAnimationRunning(string animationKey)
        {
            return (_pulseAnimations.ContainsKey(animationKey) && _pulseAnimations[animationKey].IsRunning) ||
                   (_fadeAnimations.ContainsKey(animationKey) && _fadeAnimations[animationKey].IsRunning) ||
                   (_translateAnimations.ContainsKey(animationKey) && _translateAnimations[animationKey].IsRunning);
        }

        /// <summary>
        /// Lista todas as animações ativas
        /// </summary>
        public IReadOnlyList<string> ActiveAnimations =>
            _pulseAnimations.Keys
                .Concat(_fadeAnimations.Keys)
                .Concat(_translateAnimations.Keys)
                .ToList()
                .AsReadOnly();

        #endregion

        #region Shortcuts for Common Patterns

        /// <summary>
        /// Shortcut para animação call-to-action
        /// </summary>
        public async Task StartCallToActionAsync(string animationKey, VisualElement target, Func<bool> shouldContinue = null)
        {
            await StartPulseAsync(animationKey, target, AnimationConfig.CallToAction, shouldContinue);
        }

        /// <summary>
        /// Shortcut para animação sutil
        /// </summary>
        public async Task StartSubtleAsync(string animationKey, VisualElement target, Func<bool> shouldContinue = null)
        {
            await StartPulseAsync(animationKey, target, AnimationConfig.Subtle, shouldContinue);
        }

        /// <summary>
        /// Shortcut para animação intensa
        /// </summary>
        public async Task StartIntenseAsync(string animationKey, VisualElement target, Func<bool> shouldContinue = null)
        {
            await StartPulseAsync(animationKey, target, AnimationConfig.Intense, shouldContinue);
        }

        #endregion

        #region Factory Methods

        /// <summary>
        /// Factory method para criar AnimationManager com auto-cleanup
        /// </summary>
        public static AnimationManager CreateForView(ContentPage view)
        {
            var manager = new AnimationManager(view.GetType().Name);

            // Auto-cleanup quando a view desaparecer
            view.Disappearing += async (s, e) =>
            {
                System.Diagnostics.Debug.WriteLine($"Auto-cleanup de animações para {view.GetType().Name}");
                await manager.StopAllAnimationsAsync();
            };

            return manager;
        }

        /// <summary>
        /// Factory method para criar AnimationManager com auto-cleanup para ContentView
        /// </summary>
        public static AnimationManager CreateForComponent(ContentView component)
        {
            var manager = new AnimationManager(component.GetType().Name);

            // Auto-cleanup quando o handler for removido
            component.HandlerChanged += async (s, e) =>
            {
                if (component.Handler == null)
                {
                    System.Diagnostics.Debug.WriteLine($"Auto-cleanup de animações para {component.GetType().Name}");
                    await manager.StopAllAnimationsAsync();
                }
            };

            return manager;
        }

        #endregion

        #region Disposal

        /// <summary>
        /// Libera todos os recursos
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            // Para todas as animações de forma síncrona
            foreach (var animation in _pulseAnimations.Values)
            {
                try { animation.Dispose(); }
                catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Erro no dispose da pulse animation: {ex.Message}"); }
            }

            foreach (var animation in _fadeAnimations.Values)
            {
                try { animation.Dispose(); }
                catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Erro no dispose da fade animation: {ex.Message}"); }
            }

            foreach (var animation in _translateAnimations.Values)
            {
                try { animation.Dispose(); }
                catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Erro no dispose da translate animation: {ex.Message}"); }
            }

            _pulseAnimations.Clear();
            _fadeAnimations.Clear();
            _translateAnimations.Clear();

            System.Diagnostics.Debug.WriteLine($"[{_viewName}] AnimationManager disposed");
        }

        #endregion
    }

    /// <summary>
    /// Argumentos para eventos de animação
    /// </summary>
    public class AnimationEventArgs : EventArgs
    {
        public string AnimationKey { get; }
        public VisualElement Target { get; }

        public AnimationEventArgs(string animationKey, VisualElement target)
        {
            AnimationKey = animationKey;
            Target = target;
        }
    }
}