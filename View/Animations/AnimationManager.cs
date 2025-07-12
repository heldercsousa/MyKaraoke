namespace MyKaraoke.View.Animations
{
    /// <summary>
    /// Gerenciador centralizado de animações para Views
    /// Elimina duplicação de código e facilita o controle de múltiplas animações
    /// </summary>
    public class AnimationManager : IDisposable
    {
        private readonly Dictionary<string, PulseAnimation> _animations = new();
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
                await StopAnimationAsync(animationKey);

                // Cria nova animação
                var animation = new PulseAnimation(
                    target: target,
                    config: config ?? AnimationConfig.CallToAction,
                    shouldContinue: shouldContinue ?? (() => true)
                );

                // Event handlers
                animation.AnimationStarted += (s, e) =>
                {
                    System.Diagnostics.Debug.WriteLine($"[{_viewName}] Animação '{animationKey}' iniciada");
                    AnimationStarted?.Invoke(this, new AnimationEventArgs(animationKey, target));
                };

                animation.AnimationStopped += (s, e) =>
                {
                    System.Diagnostics.Debug.WriteLine($"[{_viewName}] Animação '{animationKey}' parada");
                    AnimationStopped?.Invoke(this, new AnimationEventArgs(animationKey, target));
                };

                // Armazena a animação
                _animations[animationKey] = animation;

                // Inicia a animação
                await animation.StartAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[{_viewName}] Erro ao iniciar animação '{animationKey}': {ex.Message}");
            }
        }

        /// <summary>
        /// Para uma animação específica
        /// </summary>
        /// <param name="animationKey">Chave da animação</param>
        public async Task StopAnimationAsync(string animationKey)
        {
            if (string.IsNullOrEmpty(animationKey) || !_animations.ContainsKey(animationKey))
                return;

            try
            {
                var animation = _animations[animationKey];
                await animation.StopAsync();
                animation.Dispose();
                _animations.Remove(animationKey);

                System.Diagnostics.Debug.WriteLine($"[{_viewName}] Animação '{animationKey}' removida");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[{_viewName}] Erro ao parar animação '{animationKey}': {ex.Message}");
            }
        }

        /// <summary>
        /// Para todas as animações
        /// </summary>
        public async Task StopAllAnimationsAsync()
        {
            var tasks = _animations.Keys.Select(StopAnimationAsync).ToList();
            await Task.WhenAll(tasks);

            System.Diagnostics.Debug.WriteLine($"[{_viewName}] Todas as animações paradas");
        }

        /// <summary>
        /// Verifica se uma animação está rodando
        /// </summary>
        /// <param name="animationKey">Chave da animação</param>
        public bool IsAnimationRunning(string animationKey)
        {
            return _animations.ContainsKey(animationKey) &&
                   _animations[animationKey].IsRunning;
        }

        /// <summary>
        /// Lista todas as animações ativas
        /// </summary>
        public IReadOnlyList<string> ActiveAnimations => _animations.Keys.ToList().AsReadOnly();

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
        /// Libera todos os recursos
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            // Para todas as animações de forma síncrona
            foreach (var animation in _animations.Values)
            {
                try
                {
                    animation.Dispose();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Erro no dispose da animação: {ex.Message}");
                }
            }

            _animations.Clear();
            System.Diagnostics.Debug.WriteLine($"[{_viewName}] AnimationManager disposed");
        }
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