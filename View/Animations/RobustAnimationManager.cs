using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyKaraoke.View.Animations
{
    public class RobustAnimationManager : IDisposable, IAsyncDisposable
    {
        // ✅ CORRIGIDO: Usar VisualElement em vez de Animation interna
        private readonly List<WeakReference<VisualElement>> _activeElements = new();
        private readonly List<CancellationTokenSource> _cancellationTokens = new();
        private volatile bool _isDisposed;

        public async Task StopAllAnimationsCompletely()
        {
            if (_isDisposed) return;

            // 1. Cancelar todos os tokens primeiro
            foreach (var cts in _cancellationTokens.ToList())
            {
                if (!cts.IsCancellationRequested)
                {
                    cts.Cancel();
                }
            }

            // 2. Parar animações MAUI
            await StopMauiAnimations();

            // 3. Força parada de threads nativas (Android específico)
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                await ForceStopNativeAnimations();
            }

            // 4. Limpar referências
            _activeElements.Clear();
            _cancellationTokens.Clear();
        }

        private async Task ForceStopNativeAnimations()
        {
            try
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
#if ANDROID
                    // Acesso direto ao sistema de animação do Android
                    var activity = Platform.CurrentActivity ??
                                  Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
                    if (activity != null)
                    {
                        // ✅ CORRIGIDO: Usar Android.Animation.ValueAnimator corretamente
                        var originalDelay = Android.Animation.ValueAnimator.FrameDelay;
                        Android.Animation.ValueAnimator.FrameDelay = long.MaxValue;

                        // Aguardar um frame para garantir interrupção
                        Task.Delay(50).Wait();

                        // Restaurar delay original
                        Android.Animation.ValueAnimator.FrameDelay = originalDelay;
                    }
#endif
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao parar animações nativas: {ex.Message}");
            }
        }

        // ✅ CORRIGIDO: Usar VisualElement em vez de Animation
        private async Task StopMauiAnimations()
        {
            var stopTasks = _activeElements
                .Where(wr => wr.TryGetTarget(out _))
                .Select(async weakRef =>
                {
                    if (weakRef.TryGetTarget(out var element))
                    {
                        try
                        {
                            if (MainThread.IsMainThread)
                            {
                                // ✅ Para todas as animações do elemento
                                Microsoft.Maui.Controls.ViewExtensions.CancelAnimations(element);
                            }
                            else
                            {
                                await MainThread.InvokeOnMainThreadAsync(() =>
                                    Microsoft.Maui.Controls.ViewExtensions.CancelAnimations(element));
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Erro ao parar animação: {ex.Message}");
                        }
                    }
                });

            await Task.WhenAll(stopTasks);
        }

        // ✅ NOVO: Método para registrar elementos animados
        public void RegisterAnimatedElement(VisualElement element)
        {
            if (element != null && !_isDisposed)
            {
                _activeElements.Add(new WeakReference<VisualElement>(element));
            }
        }

        // ✅ NOVO: Método para registrar tokens de cancelamento
        public void RegisterCancellationToken(CancellationTokenSource cts)
        {
            if (cts != null && !_isDisposed)
            {
                _cancellationTokens.Add(cts);
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_isDisposed) return;
            _isDisposed = true;

            try
            {
                await StopAllAnimationsCompletely();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro durante DisposeAsync: {ex.Message}");
            }
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            try
            {
                // Usar timeout para evitar bloqueios
                StopAllAnimationsCompletely().Wait(TimeSpan.FromSeconds(2));
            }
            catch
            {
                // Disposal deve ser não-blocking
            }
            finally
            {
                _isDisposed = true;
            }
        }
    }
}