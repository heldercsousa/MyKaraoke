namespace MyKaraoke.View.Animations
{
    /// <summary>
    /// Sistema de animação de pulse reutilizável para qualquer VisualElement
    /// </summary>
    public class PulseAnimation : IDisposable
    {
        private readonly VisualElement _target;
        private readonly AnimationConfig _config;
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
        /// Cria uma nova animação de pulse
        /// </summary>
        /// <param name="target">Elemento a ser animado</param>
        /// <param name="config">Configuração da animação (opcional, usa CallToAction por padrão)</param>
        /// <param name="shouldContinue">Função que determina se deve continuar animando (opcional)</param>
        public PulseAnimation(VisualElement target, AnimationConfig config = null, Func<bool> shouldContinue = null)
        {
            _target = target ?? throw new ArgumentNullException(nameof(target));
            _config = config ?? AnimationConfig.CallToAction;
            _shouldContinue = shouldContinue ?? (() => true);
        }

        /// <summary>
        /// Inicia a animação (com verificação de hardware automática)
        /// </summary>
        public async Task StartAsync()
        {
            if (_disposed || _isRunning)
                return;

            // ✅ Sistema corrigido: só faz BYPASS se hardware for MUITO ruim
            var optimizedConfig = HardwareDetector.GetOptimalConfig(_config);
            if (optimizedConfig == null)
            {
                System.Diagnostics.Debug.WriteLine("🚫 BYPASS ativado - hardware muito limitado, animação desabilitada para economia de recursos");
                return;
            }

            // ✅ Para hardware adequado (Pixel 5, etc.), usa configuração ORIGINAL
            System.Diagnostics.Debug.WriteLine($"✅ Hardware adequado - usando configuração original: ToScale={optimizedConfig.ToScale}");

            _cancellationTokenSource = new CancellationTokenSource();
            _isRunning = true;

            System.Diagnostics.Debug.WriteLine($"Iniciando PulseAnimation no elemento: {_target.GetType().Name}");
            System.Diagnostics.Debug.WriteLine($"🎯 Configuração: FromScale={optimizedConfig.FromScale}, ToScale={optimizedConfig.ToScale}, Duration={optimizedConfig.PulseDuration}ms");

            AnimationStarted?.Invoke(this, EventArgs.Empty);

            try
            {
                // Delay inicial
                await Task.Delay(optimizedConfig.InitialDelay, _cancellationTokenSource.Token);

                // Loop principal de animação
                while (_isRunning && _shouldContinue() && !_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    await PerformPulseCycle(optimizedConfig);

                    if (optimizedConfig.AutoRepeat && _isRunning)
                    {
                        await Task.Delay(optimizedConfig.CycleInterval, _cancellationTokenSource.Token);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Operação cancelada - comportamento normal
                System.Diagnostics.Debug.WriteLine("PulseAnimation cancelada");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro na PulseAnimation: {ex.Message}");
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
        /// Executa um ciclo completo de pulses
        /// </summary>
        private async Task PerformPulseCycle(AnimationConfig config)
        {
            try
            {
                for (int i = 0; i < config.PulseCount && _isRunning; i++)
                {
                    if (_cancellationTokenSource.Token.IsCancellationRequested)
                        break;

                    // Verifica se ainda deve continuar
                    if (!_shouldContinue())
                        break;

                    // 🎯 LOG detalhado do pulse
                    System.Diagnostics.Debug.WriteLine($"🔥 Pulse {i + 1}/{config.PulseCount}: {config.FromScale} → {config.ToScale} em {config.PulseDuration}ms");

                    // Pulse: expand → contract
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        if (_target != null && _isRunning)
                        {
                            // Expansão
                            System.Diagnostics.Debug.WriteLine($"⬆️ Expandindo para {config.ToScale}");
                            await _target.ScaleTo(config.ToScale, config.PulseDuration, config.ExpandEasing);

                            // Contração
                            if (_isRunning) // Verifica novamente após await
                            {
                                System.Diagnostics.Debug.WriteLine($"⬇️ Contraindo para {config.FromScale}");
                                await _target.ScaleTo(config.FromScale, config.PulseDuration, config.ContractEasing);
                            }
                        }
                    });

                    // Pausa entre pulses (exceto no último)
                    if (i < config.PulseCount - 1 && _isRunning)
                    {
                        await Task.Delay(config.PulsePause, _cancellationTokenSource.Token);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Normal quando para a animação
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no ciclo de pulse: {ex.Message}");
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

            // Restaura escala original
            try
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    if (_target != null)
                    {
                        await _target.ScaleTo(_config.FromScale, 200, Easing.CubicOut);
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao restaurar escala: {ex.Message}");
            }

            System.Diagnostics.Debug.WriteLine("PulseAnimation parada");
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

            // Restaura escala original de forma síncrona
            try
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (_target != null)
                    {
                        _target.Scale = _config.FromScale;
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no dispose: {ex.Message}");
            }
        }

        /// <summary>
        /// Factory method para criar animação rápida
        /// </summary>
        public static PulseAnimation CreateCallToAction(VisualElement target, Func<bool> shouldContinue = null)
        {
            return new PulseAnimation(target, AnimationConfig.CallToAction, shouldContinue);
        }

        /// <summary>
        /// Factory method para criar animação sutil
        /// </summary>
        public static PulseAnimation CreateSubtle(VisualElement target, Func<bool> shouldContinue = null)
        {
            return new PulseAnimation(target, AnimationConfig.Subtle, shouldContinue);
        }

        /// <summary>
        /// Factory method para criar animação intensa
        /// </summary>
        public static PulseAnimation CreateIntense(VisualElement target, Func<bool> shouldContinue = null)
        {
            return new PulseAnimation(target, AnimationConfig.Intense, shouldContinue);
        }
    }
}