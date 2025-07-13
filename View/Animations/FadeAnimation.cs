namespace MyKaraoke.View.Animations
{
    /// <summary>
    /// Sistema de animação de fade (aparece/desaparece) reutilizável para qualquer VisualElement
    /// </summary>
    public class FadeAnimation : IDisposable
    {
        private readonly VisualElement _target;
        private readonly FadeAnimationConfig _config;
        private readonly Func<bool> _shouldContinue;

        private bool _isRunning = false;
        private bool _disposed = false;
        private CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        /// Evento disparado quando a animação inicia
        /// </summary>
        public event EventHandler AnimationStarted;

        /// <summary>
        /// Evento disparado quando a animação para
        /// </summary>
        public event EventHandler AnimationStopped;

        /// <summary>
        /// Indica se a animação está ativa
        /// </summary>
        public bool IsRunning => _isRunning && !_disposed;

        /// <summary>
        /// Cria uma nova animação de fade
        /// </summary>
        /// <param name="target">Elemento a ser animado</param>
        /// <param name="config">Configuração da animação (opcional, usa FadeIn por padrão)</param>
        /// <param name="shouldContinue">Função que determina se deve continuar animando (opcional)</param>
        public FadeAnimation(VisualElement target, FadeAnimationConfig config = null, Func<bool> shouldContinue = null)
        {
            _target = target ?? throw new ArgumentNullException(nameof(target));
            _config = config ?? FadeAnimationConfig.FadeIn;
            _shouldContinue = shouldContinue ?? (() => true);
        }

        /// <summary>
        /// Inicia a animação (com verificação de hardware automática)
        /// </summary>
        public async Task StartAsync()
        {
            if (_disposed || _isRunning)
                return;

            // Verificação de hardware usando o sistema existente
            var optimizedConfig = HardwareDetector.SupportsAnimations ? _config : null;
            if (optimizedConfig == null)
            {
                System.Diagnostics.Debug.WriteLine("🚫 BYPASS ativado - hardware muito limitado, fade animation desabilitada");

                // Para hardware limitado, aplica apenas o estado final sem animação
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    if (_target != null)
                    {
                        _target.Opacity = _config.ToOpacity;
                        _target.IsVisible = _config.ToOpacity > 0;
                    }
                });
                return;
            }

            _cancellationTokenSource = new CancellationTokenSource();
            _isRunning = true;

            System.Diagnostics.Debug.WriteLine($"Iniciando FadeAnimation no elemento: {_target.GetType().Name}");
            System.Diagnostics.Debug.WriteLine($"🎯 Configuração: FromOpacity={optimizedConfig.FromOpacity}, ToOpacity={optimizedConfig.ToOpacity}, Duration={optimizedConfig.Duration}ms");

            AnimationStarted?.Invoke(this, EventArgs.Empty);

            try
            {
                // Delay inicial
                await Task.Delay(optimizedConfig.InitialDelay, _cancellationTokenSource.Token);

                // Executa a animação principal
                await PerformFadeAnimation(optimizedConfig);
            }
            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("FadeAnimation cancelada");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro na FadeAnimation: {ex.Message}");
            }
            finally
            {
                await StopInternal();
            }
        }

        /// <summary>
        /// Para a animação
        /// </summary>
        public async Task StopAsync()
        {
            await StopInternal();
        }

        /// <summary>
        /// Executa a animação de fade
        /// </summary>
        private async Task PerformFadeAnimation(FadeAnimationConfig config)
        {
            try
            {
                if (_cancellationTokenSource.Token.IsCancellationRequested)
                    return;

                // Verifica se ainda deve continuar
                if (!_shouldContinue())
                    return;

                System.Diagnostics.Debug.WriteLine($"🌟 Fade: {config.FromOpacity} → {config.ToOpacity} em {config.Duration}ms");

                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    if (_target != null && _isRunning)
                    {
                        // Define estado inicial
                        _target.Opacity = config.FromOpacity;
                        _target.IsVisible = true; // Sempre visível durante animação

                        // Executa o fade
                        await _target.FadeTo(config.ToOpacity, config.Duration, config.Easing);

                        // Define visibilidade final baseada na opacidade
                        if (_isRunning && config.ToOpacity <= 0)
                        {
                            _target.IsVisible = false;
                        }
                    }
                });
            }
            catch (OperationCanceledException)
            {
                // Normal quando para a animação
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no fade animation: {ex.Message}");
            }
        }

        /// <summary>
        /// Para a animação internamente
        /// </summary>
        private async Task StopInternal()
        {
            if (!_isRunning)
                return;

            _isRunning = false;
            _cancellationTokenSource?.Cancel();

            System.Diagnostics.Debug.WriteLine("FadeAnimation parada");
            AnimationStopped?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Libera recursos
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            _isRunning = false;

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();

            System.Diagnostics.Debug.WriteLine("FadeAnimation disposed");
        }

        /// <summary>
        /// Factory method para fade in
        /// </summary>
        public static FadeAnimation CreateFadeIn(VisualElement target, uint duration = 300, Func<bool> shouldContinue = null)
        {
            return new FadeAnimation(target, FadeAnimationConfig.FadeIn.WithDuration(duration), shouldContinue);
        }

        /// <summary>
        /// Factory method para fade out
        /// </summary>
        public static FadeAnimation CreateFadeOut(VisualElement target, uint duration = 300, Func<bool> shouldContinue = null)
        {
            return new FadeAnimation(target, FadeAnimationConfig.FadeOut.WithDuration(duration), shouldContinue);
        }

        /// <summary>
        /// Factory method para fade customizado
        /// </summary>
        public static FadeAnimation CreateCustomFade(VisualElement target, double fromOpacity, double toOpacity, uint duration = 300, Func<bool> shouldContinue = null)
        {
            var config = new FadeAnimationConfig
            {
                FromOpacity = fromOpacity,
                ToOpacity = toOpacity,
                Duration = duration
            };
            return new FadeAnimation(target, config, shouldContinue);
        }
    }

    /// <summary>
    /// Configurações para animações de fade
    /// </summary>
    public class FadeAnimationConfig
    {
        /// <summary>
        /// Opacidade inicial (padrão: 0.0 para fade in)
        /// </summary>
        public double FromOpacity { get; set; } = 0.0;

        /// <summary>
        /// Opacidade final (padrão: 1.0 para fade in)
        /// </summary>
        public double ToOpacity { get; set; } = 1.0;

        /// <summary>
        /// Duração da animação em ms (padrão: 300ms)
        /// </summary>
        public uint Duration { get; set; } = 300;

        /// <summary>
        /// Delay inicial antes de começar (padrão: 0ms)
        /// </summary>
        public int InitialDelay { get; set; } = 0;

        /// <summary>
        /// Easing da animação (padrão: CubicInOut)
        /// </summary>
        public Easing Easing { get; set; } = Easing.CubicInOut;

        /// <summary>
        /// Configuração padrão para fade in
        /// </summary>
        public static FadeAnimationConfig FadeIn => new FadeAnimationConfig
        {
            FromOpacity = 0.0,
            ToOpacity = 1.0,
            Duration = 300,
            Easing = Easing.CubicOut
        };

        /// <summary>
        /// Configuração padrão para fade out
        /// </summary>
        public static FadeAnimationConfig FadeOut => new FadeAnimationConfig
        {
            FromOpacity = 1.0,
            ToOpacity = 0.0,
            Duration = 300,
            Easing = Easing.CubicIn
        };

        /// <summary>
        /// Configuração rápida para show/hide UI
        /// </summary>
        public static FadeAnimationConfig Quick => new FadeAnimationConfig
        {
            FromOpacity = 0.0,
            ToOpacity = 1.0,
            Duration = 150,
            Easing = Easing.CubicOut
        };

        /// <summary>
        /// Configuração suave para transições elegantes
        /// </summary>
        public static FadeAnimationConfig Smooth => new FadeAnimationConfig
        {
            FromOpacity = 0.0,
            ToOpacity = 1.0,
            Duration = 500,
            Easing = Easing.SinInOut
        };

        /// <summary>
        /// Cria uma nova configuração com duração customizada
        /// </summary>
        public FadeAnimationConfig WithDuration(uint duration)
        {
            return new FadeAnimationConfig
            {
                FromOpacity = this.FromOpacity,
                ToOpacity = this.ToOpacity,
                Duration = duration,
                InitialDelay = this.InitialDelay,
                Easing = this.Easing
            };
        }

        /// <summary>
        /// Cria uma nova configuração com delay customizado
        /// </summary>
        public FadeAnimationConfig WithDelay(int delay)
        {
            return new FadeAnimationConfig
            {
                FromOpacity = this.FromOpacity,
                ToOpacity = this.ToOpacity,
                Duration = this.Duration,
                InitialDelay = delay,
                Easing = this.Easing
            };
        }
    }
}