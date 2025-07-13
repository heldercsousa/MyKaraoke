namespace MyKaraoke.View.Animations
{
    /// <summary>
    /// Sistema de animação de translação (movimento) reutilizável para qualquer VisualElement
    /// </summary>
    public class TranslateAnimation : IDisposable
    {
        private readonly VisualElement _target;
        private readonly TranslateAnimationConfig _config;
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
        /// Cria uma nova animação de translação
        /// </summary>
        /// <param name="target">Elemento a ser animado</param>
        /// <param name="config">Configuração da animação (opcional, usa SlideUp por padrão)</param>
        /// <param name="shouldContinue">Função que determina se deve continuar animando (opcional)</param>
        public TranslateAnimation(VisualElement target, TranslateAnimationConfig config = null, Func<bool> shouldContinue = null)
        {
            _target = target ?? throw new ArgumentNullException(nameof(target));
            _config = config ?? TranslateAnimationConfig.SlideUp;
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
                System.Diagnostics.Debug.WriteLine("🚫 BYPASS ativado - hardware muito limitado, translate animation desabilitada");

                // Para hardware limitado, aplica apenas o estado final sem animação
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    if (_target != null)
                    {
                        _target.TranslationX = _config.ToTranslationX;
                        _target.TranslationY = _config.ToTranslationY;
                    }
                });
                return;
            }

            _cancellationTokenSource = new CancellationTokenSource();
            _isRunning = true;

            System.Diagnostics.Debug.WriteLine($"Iniciando TranslateAnimation no elemento: {_target.GetType().Name}");
            System.Diagnostics.Debug.WriteLine($"🎯 Configuração: X({optimizedConfig.FromTranslationX} → {optimizedConfig.ToTranslationX}), Y({optimizedConfig.FromTranslationY} → {optimizedConfig.ToTranslationY}), Duration={optimizedConfig.Duration}ms");

            AnimationStarted?.Invoke(this, EventArgs.Empty);

            try
            {
                // Delay inicial
                await Task.Delay(optimizedConfig.InitialDelay, _cancellationTokenSource.Token);

                // Executa a animação principal
                await PerformTranslateAnimation(optimizedConfig);
            }
            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("TranslateAnimation cancelada");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro na TranslateAnimation: {ex.Message}");
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
        /// Executa a animação de translação
        /// </summary>
        private async Task PerformTranslateAnimation(TranslateAnimationConfig config)
        {
            try
            {
                if (_cancellationTokenSource.Token.IsCancellationRequested)
                    return;

                // Verifica se ainda deve continuar
                if (!_shouldContinue())
                    return;

                System.Diagnostics.Debug.WriteLine($"🚀 Translate: X({config.FromTranslationX} → {config.ToTranslationX}), Y({config.FromTranslationY} → {config.ToTranslationY}) em {config.Duration}ms");

                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    if (_target != null && _isRunning)
                    {
                        // Define estado inicial
                        _target.TranslationX = config.FromTranslationX;
                        _target.TranslationY = config.FromTranslationY;

                        // Executa a translação
                        await _target.TranslateTo(
                            x: config.ToTranslationX,
                            y: config.ToTranslationY,
                            length: config.Duration,
                            easing: config.Easing
                        );
                    }
                });
            }
            catch (OperationCanceledException)
            {
                // Normal quando para a animação
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no translate animation: {ex.Message}");
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

            // Restaura posição original se necessário
            try
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    if (_target != null && _config.RestoreOriginalPosition)
                    {
                        await _target.TranslateTo(0, 0, 200, Easing.CubicOut);
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao restaurar posição: {ex.Message}");
            }

            System.Diagnostics.Debug.WriteLine("TranslateAnimation parada");
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

            // Restaura posição original de forma síncrona
            try
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (_target != null && _config.RestoreOriginalPosition)
                    {
                        _target.TranslationX = 0;
                        _target.TranslationY = 0;
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no dispose: {ex.Message}");
            }

            System.Diagnostics.Debug.WriteLine("TranslateAnimation disposed");
        }

        /// <summary>
        /// Factory method para slide up (aparece de baixo)
        /// </summary>
        public static TranslateAnimation CreateSlideUp(VisualElement target, double distance = 20, uint duration = 300, Func<bool> shouldContinue = null)
        {
            var config = new TranslateAnimationConfig
            {
                FromTranslationY = distance,
                ToTranslationY = 0,
                Duration = duration,
                Easing = Easing.CubicOut
            };
            return new TranslateAnimation(target, config, shouldContinue);
        }

        /// <summary>
        /// Factory method para slide down (desaparece para baixo)
        /// </summary>
        public static TranslateAnimation CreateSlideDown(VisualElement target, double distance = 20, uint duration = 300, Func<bool> shouldContinue = null)
        {
            var config = new TranslateAnimationConfig
            {
                FromTranslationY = 0,
                ToTranslationY = distance,
                Duration = duration,
                Easing = Easing.CubicIn
            };
            return new TranslateAnimation(target, config, shouldContinue);
        }

        /// <summary>
        /// Factory method para slide left (aparece da direita)
        /// </summary>
        public static TranslateAnimation CreateSlideLeft(VisualElement target, double distance = 50, uint duration = 300, Func<bool> shouldContinue = null)
        {
            var config = new TranslateAnimationConfig
            {
                FromTranslationX = distance,
                ToTranslationX = 0,
                Duration = duration,
                Easing = Easing.CubicOut
            };
            return new TranslateAnimation(target, config, shouldContinue);
        }

        /// <summary>
        /// Factory method para slide right (aparece da esquerda)
        /// </summary>
        public static TranslateAnimation CreateSlideRight(VisualElement target, double distance = 50, uint duration = 300, Func<bool> shouldContinue = null)
        {
            var config = new TranslateAnimationConfig
            {
                FromTranslationX = -distance,
                ToTranslationX = 0,
                Duration = duration,
                Easing = Easing.CubicOut
            };
            return new TranslateAnimation(target, config, shouldContinue);
        }

        /// <summary>
        /// Factory method para shake (tremida horizontal)
        /// </summary>
        public static TranslateAnimation CreateShake(VisualElement target, double intensity = 10, uint duration = 500, Func<bool> shouldContinue = null)
        {
            var config = new TranslateAnimationConfig
            {
                FromTranslationX = 0,
                ToTranslationX = intensity,
                Duration = duration / 4, // Shake rápido
                Easing = Easing.Linear,
                RestoreOriginalPosition = true
            };
            return new TranslateAnimation(target, config, shouldContinue);
        }
    }

    /// <summary>
    /// Configurações para animações de translação
    /// </summary>
    public class TranslateAnimationConfig
    {
        /// <summary>
        /// Translação X inicial (padrão: 0)
        /// </summary>
        public double FromTranslationX { get; set; } = 0;

        /// <summary>
        /// Translação Y inicial (padrão: 20 para slide up)
        /// </summary>
        public double FromTranslationY { get; set; } = 20;

        /// <summary>
        /// Translação X final (padrão: 0)
        /// </summary>
        public double ToTranslationX { get; set; } = 0;

        /// <summary>
        /// Translação Y final (padrão: 0)
        /// </summary>
        public double ToTranslationY { get; set; } = 0;

        /// <summary>
        /// Duração da animação em ms (padrão: 300ms)
        /// </summary>
        public uint Duration { get; set; } = 300;

        /// <summary>
        /// Delay inicial antes de começar (padrão: 0ms)
        /// </summary>
        public int InitialDelay { get; set; } = 0;

        /// <summary>
        /// Easing da animação (padrão: CubicOut)
        /// </summary>
        public Easing Easing { get; set; } = Easing.CubicOut;

        /// <summary>
        /// Se deve restaurar a posição original ao parar (padrão: false)
        /// </summary>
        public bool RestoreOriginalPosition { get; set; } = false;

        /// <summary>
        /// Configuração padrão para slide up (aparece de baixo)
        /// </summary>
        public static TranslateAnimationConfig SlideUp => new TranslateAnimationConfig
        {
            FromTranslationX = 0,
            FromTranslationY = 20,
            ToTranslationX = 0,
            ToTranslationY = 0,
            Duration = 300,
            Easing = Easing.CubicOut
        };

        /// <summary>
        /// Configuração padrão para slide down (desaparece para baixo)
        /// </summary>
        public static TranslateAnimationConfig SlideDown => new TranslateAnimationConfig
        {
            FromTranslationX = 0,
            FromTranslationY = 0,
            ToTranslationX = 0,
            ToTranslationY = 20,
            Duration = 300,
            Easing = Easing.CubicIn
        };

        /// <summary>
        /// Configuração padrão para slide left (aparece da direita)
        /// </summary>
        public static TranslateAnimationConfig SlideLeft => new TranslateAnimationConfig
        {
            FromTranslationX = 50,
            FromTranslationY = 0,
            ToTranslationX = 0,
            ToTranslationY = 0,
            Duration = 300,
            Easing = Easing.CubicOut
        };

        /// <summary>
        /// Configuração padrão para slide right (aparece da esquerda)
        /// </summary>
        public static TranslateAnimationConfig SlideRight => new TranslateAnimationConfig
        {
            FromTranslationX = -50,
            FromTranslationY = 0,
            ToTranslationX = 0,
            ToTranslationY = 0,
            Duration = 300,
            Easing = Easing.CubicOut
        };

        /// <summary>
        /// Configuração para shake (tremida)
        /// </summary>
        public static TranslateAnimationConfig Shake => new TranslateAnimationConfig
        {
            FromTranslationX = 0,
            FromTranslationY = 0,
            ToTranslationX = 10,
            ToTranslationY = 0,
            Duration = 100,
            Easing = Easing.Linear,
            RestoreOriginalPosition = true
        };

        /// <summary>
        /// Configuração rápida para show/hide UI
        /// </summary>
        public static TranslateAnimationConfig Quick => new TranslateAnimationConfig
        {
            FromTranslationX = 0,
            FromTranslationY = 15,
            ToTranslationX = 0,
            ToTranslationY = 0,
            Duration = 150,
            Easing = Easing.CubicOut
        };

        /// <summary>
        /// Configuração suave para transições elegantes
        /// </summary>
        public static TranslateAnimationConfig Smooth => new TranslateAnimationConfig
        {
            FromTranslationX = 0,
            FromTranslationY = 30,
            ToTranslationX = 0,
            ToTranslationY = 0,
            Duration = 500,
            Easing = Easing.SinInOut
        };

        /// <summary>
        /// Cria uma nova configuração com duração customizada
        /// </summary>
        public TranslateAnimationConfig WithDuration(uint duration)
        {
            return new TranslateAnimationConfig
            {
                FromTranslationX = this.FromTranslationX,
                FromTranslationY = this.FromTranslationY,
                ToTranslationX = this.ToTranslationX,
                ToTranslationY = this.ToTranslationY,
                Duration = duration,
                InitialDelay = this.InitialDelay,
                Easing = this.Easing,
                RestoreOriginalPosition = this.RestoreOriginalPosition
            };
        }

        /// <summary>
        /// Cria uma nova configuração com delay customizado
        /// </summary>
        public TranslateAnimationConfig WithDelay(int delay)
        {
            return new TranslateAnimationConfig
            {
                FromTranslationX = this.FromTranslationX,
                FromTranslationY = this.FromTranslationY,
                ToTranslationX = this.ToTranslationX,
                ToTranslationY = this.ToTranslationY,
                Duration = this.Duration,
                InitialDelay = delay,
                Easing = this.Easing,
                RestoreOriginalPosition = this.RestoreOriginalPosition
            };
        }
    }
}