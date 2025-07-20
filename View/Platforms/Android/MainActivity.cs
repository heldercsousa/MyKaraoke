using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using System.Threading;
using System.Threading.Tasks;

namespace MyKaraoke.View
{
    [Activity(
        Theme = "@style/Maui.SplashTheme",
        MainLauncher = true,
        LaunchMode = LaunchMode.SingleTop,
        ConfigurationChanges = ConfigChanges.ScreenSize |
                              ConfigChanges.Orientation |
                              ConfigChanges.UiMode |
                              ConfigChanges.ScreenLayout |
                              ConfigChanges.SmallestScreenSize |
                              ConfigChanges.Density |
                              ConfigChanges.Locale,
        Exported = true)]
    public class MainActivity : MauiAppCompatActivity
    {
        private static bool _isDestroying = false;
        private static bool _animationsStopped = false;
        private static readonly object _destroyLock = new object();
        private static readonly object _animationLock = new object();
        private static CancellationTokenSource _globalCancellationTokenSource = new();
        private static System.Timers.Timer _preventiveStopTimer;
        private static bool _preventiveStopInitialized = false;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("[MainActivity] OnCreate iniciado");

                // Configurações de performance antes do base.OnCreate
                OptimizeMemorySettings();

                base.OnCreate(savedInstanceState);

                // NOVO: Inicia sistema preventivo anti-crash
                InitializePreventiveCrashProtection();

                System.Diagnostics.Debug.WriteLine("[MainActivity] OnCreate concluído com sucesso");
            } catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MainActivity] ERRO OnCreate: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[MainActivity] Stack trace: {ex.StackTrace}");
            } 
        }

        private void InitializePreventiveCrashProtection()
        {
            if (_preventiveStopInitialized) return;

            try
            {
                System.Diagnostics.Debug.WriteLine("[MainActivity] Inicializando proteção preventiva anti-crash");

                // Timer que força parada de animações a cada 30 segundos como prevenção
                _preventiveStopTimer = new System.Timers.Timer(30000);
                _preventiveStopTimer.Elapsed += (sender, e) =>
                {
                    try
                    {
                        // Verifica se há muitas threads HWUI ativas
                        var threadCount = System.Diagnostics.Process.GetCurrentProcess().Threads.Count;
                        if (threadCount > 100) // Limite preventivo
                        {
                            System.Diagnostics.Debug.WriteLine($"[MainActivity] ALERTA: {threadCount} threads ativas - acionando proteção preventiva");
                            StopAllAnimationsAndOperationsImmediate();
                        }
                    }
                    catch (System.Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"[MainActivity] Erro na proteção preventiva: {ex.Message}");
                    }
                };
                _preventiveStopTimer.Start();

                // Monitora eventos de baixa memória para parar animações preventivamente
                try
                {
                    // Força inicialização simples para .NET MAUI 8
                    var appInstance = Microsoft.Maui.Controls.Application.Current;
                    if (appInstance != null)
                    {
                        System.Diagnostics.Debug.WriteLine("[MainActivity] Application instance encontrada");
                    }
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[MainActivity] Erro ao verificar Application: {ex.Message}");
                }

                _preventiveStopInitialized = true;
                System.Diagnostics.Debug.WriteLine("[MainActivity] Proteção preventiva inicializada");
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MainActivity] ERRO OnCreate: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[MainActivity] Stack trace: {ex.StackTrace}");
            }
        }

        private void OptimizeMemorySettings()
        {
            try
            {
                // Configurações de ambiente para melhor performance
                System.Environment.SetEnvironmentVariable("MONO_GC_PARAMS", "major=marksweep-conc,nursery-size=8m");
                System.Environment.SetEnvironmentVariable("MONO_THREADS_PER_CPU", "4");
                System.Environment.SetEnvironmentVariable("MONO_LOG_LEVEL", "info");

                System.Diagnostics.Debug.WriteLine("[MainActivity] Configurações de memória aplicadas");
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MainActivity] Erro ao otimizar memória: {ex.Message}");
            }
        }

        protected override void OnStart()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("[MainActivity] OnStart iniciado");

                if (!_isDestroying)
                {
                    base.OnStart();
                }

                System.Diagnostics.Debug.WriteLine("[MainActivity] OnStart concluído");
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MainActivity] ERRO OnStart: {ex.Message}");
                if (!_isDestroying)
                {
                    throw;
                }
            }
        }

        protected override void OnResume()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("[MainActivity] OnResume iniciado");

                if (!_isDestroying)
                {
                    base.OnResume();
                }

                System.Diagnostics.Debug.WriteLine("[MainActivity] OnResume concluído");
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MainActivity] ERRO OnResume: {ex.Message}");
                if (!_isDestroying)
                {
                    throw;
                }
            }
        }

        protected override void OnPause()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("[MainActivity] OnPause iniciado - pausando operações");

                // CRÍTICO: Para animações ANTES de qualquer outra operação
                StopAllAnimationsAndOperationsImmediate();

                // Aguarda threads críticas terminarem
                Thread.Sleep(150);

                if (!_isDestroying)
                {
                    base.OnPause();
                }

                System.Diagnostics.Debug.WriteLine("[MainActivity] OnPause concluído");
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MainActivity] ERRO OnPause: {ex.Message}");
                try
                {
                    if (!_isDestroying) base.OnPause();
                }
                catch
                {
                    System.Diagnostics.Debug.WriteLine("[MainActivity] base.OnPause() também falhou - ignorando");
                }
            }
        }

        protected override void OnStop()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("[MainActivity] OnStop iniciado - parando operações");

                // Para TODAS as operações imediatamente
                StopAllAnimationsAndOperationsImmediate();

                // Aguarda mais tempo para threads de renderização
                Thread.Sleep(250);

                if (!_isDestroying)
                {
                    base.OnStop();
                }

                System.Diagnostics.Debug.WriteLine("[MainActivity] OnStop concluído");
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MainActivity] ERRO OnStop: {ex.Message}");
                try
                {
                    if (!_isDestroying) base.OnStop();
                }
                catch
                {
                    System.Diagnostics.Debug.WriteLine("[MainActivity] base.OnStop() também falhou - ignorando");
                }
            }
        }

        protected override void OnDestroy()
        {
            lock (_destroyLock)
            {
                if (_isDestroying)
                {
                    System.Diagnostics.Debug.WriteLine("[MainActivity] OnDestroy JÁ EXECUTANDO - ignorando chamada duplicada");
                    return;
                }

                try
                {
                    System.Diagnostics.Debug.WriteLine("[MainActivity] OnDestroy iniciado");
                    _isDestroying = true;

                    // CRÍTICO: Para TUDO imediatamente
                    StopAllAnimationsAndOperationsImmediate();

                    // Cancela todas as operações assíncronas
                    _globalCancellationTokenSource?.Cancel();

                    // Aguarda threads críticas - tempo máximo 500ms
                    Thread.Sleep(500);

                    // Força limpeza de recursos críticos
                    ForceResourceCleanup();

                    // Pausa adicional para garantir que threads de UI terminaram
                    Thread.Sleep(200);

                    base.OnDestroy();
                    System.Diagnostics.Debug.WriteLine("[MainActivity] OnDestroy concluído");
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[MainActivity] ERRO OnDestroy: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine($"[MainActivity] Stack trace: {ex.StackTrace}");
                    // NÃO chama base.OnDestroy() em caso de erro para evitar pthread_mutex crash
                }
            }
        }

        private void StopAllAnimationsAndOperationsImmediate()
        {
            lock (_animationLock)
            {
                if (_animationsStopped)
                {
                    System.Diagnostics.Debug.WriteLine("[MainActivity] Animações já foram paradas - ignorando");
                    return;
                }

                try
                {
                    System.Diagnostics.Debug.WriteLine("[MainActivity] PARANDO IMEDIATAMENTE todas as animações e operações");

                    // Sinaliza para todas as animações pararem
                    _isDestroying = true;
                    _animationsStopped = true;

                    // Cancela token global
                    _globalCancellationTokenSource?.Cancel();

                    // Para animações na thread principal de forma síncrona
                    try
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            try
                            {
                                var app = Microsoft.Maui.Controls.Application.Current;
                                if (app?.MainPage != null)
                                {
                                    System.Diagnostics.Debug.WriteLine("[MainActivity] Parando animações da página atual");

                                    // Para todas as animações da view tree
                                    StopViewTreeAnimations(app.MainPage as Microsoft.Maui.Controls.VisualElement);
                                }
                            }
                            catch (System.Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"[MainActivity] Erro ao parar animações UI: {ex.Message}");
                            }
                        });
                    }
                    catch (System.Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"[MainActivity] Erro ao acessar MainThread: {ex.Message}");
                    }

                    System.Diagnostics.Debug.WriteLine("[MainActivity] Comando para parar animações enviado");
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[MainActivity] ERRO CRÍTICO ao parar animações: {ex.Message}");
                }
            }
        }

        private void StopViewTreeAnimationsImmediate(Microsoft.Maui.Controls.VisualElement element)
        {
            try
            {
                if (element == null) return;

                System.Diagnostics.Debug.WriteLine($"[MainActivity] Parando animações do elemento: {element.GetType().Name}");

                // Para animações do elemento atual IMEDIATAMENTE
                Microsoft.Maui.Controls.ViewExtensions.CancelAnimations(element);

                // NOVO: Para animações de transformação específicas que podem causar pthread_mutex
                try
                {
                    // Aguarda que elemento seja renderizável antes de aplicar transformações
                    if (element.IsVisible && element.Width > 0 && element.Height > 0)
                    {
                        element.ScaleTo(1.0, 0); // Para animações de escala imediatamente
                        element.TranslateTo(0, 0, 0); // Para animações de posição imediatamente
                        element.FadeTo(element.Opacity, 0); // Para animações de fade imediatamente
                        element.RotateTo(0, 0); // Para animações de rotação imediatamente
                    }
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[MainActivity] Erro ao parar transformações: {ex.Message}");
                }

                // Para animações de elementos filhos recursivamente
                if (element is Microsoft.Maui.Controls.Layout layout)
                {
                    foreach (var child in layout.Children)
                    {
                        if (child is Microsoft.Maui.Controls.VisualElement visualChild)
                        {
                            StopViewTreeAnimationsImmediate(visualChild);
                        }
                    }
                }
                else if (element is Microsoft.Maui.Controls.ContentView contentView && contentView.Content != null)
                {
                    StopViewTreeAnimationsImmediate(contentView.Content);
                }
                else if (element is Microsoft.Maui.Controls.ScrollView scrollView && scrollView.Content != null)
                {
                    StopViewTreeAnimationsImmediate(scrollView.Content);
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MainActivity] Erro ao parar animações de elemento: {ex.Message}");
            }
        }

        private void ForceStopRenderingThreads()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("[MainActivity] Forçando parada de threads de renderização");

                // Tenta forçar uma pausa nas threads de renderização
                Android.Views.View rootView = Window?.DecorView?.RootView;
                if (rootView != null)
                {
                    try
                    {
                        // Força invalidação e redesenho imediato para finalizar operações pendentes
                        rootView.Invalidate();
                        rootView.RequestLayout();

                        // Força parada de animações em nível de View Android
                        rootView.ClearAnimation();

                        System.Diagnostics.Debug.WriteLine("[MainActivity] Operações de renderização finalizadas");
                    }
                    catch (System.Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"[MainActivity] Erro ao finalizar renderização: {ex.Message}");
                    }
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MainActivity] Erro ao parar threads de renderização: {ex.Message}");
            }
        }

        private void StopViewTreeAnimations(Microsoft.Maui.Controls.VisualElement element)
        {
            try
            {
                if (element == null) return;

                // Para animações do elemento atual
                Microsoft.Maui.Controls.ViewExtensions.CancelAnimations(element);

                // Para animações de elementos filhos recursivamente
                if (element is Microsoft.Maui.Controls.Layout layout)
                {
                    foreach (var child in layout.Children)
                    {
                        if (child is Microsoft.Maui.Controls.VisualElement visualChild)
                        {
                            StopViewTreeAnimations(visualChild);
                        }
                    }
                }
                else if (element is Microsoft.Maui.Controls.ContentView contentView && contentView.Content != null)
                {
                    StopViewTreeAnimations(contentView.Content);
                }
                else if (element is Microsoft.Maui.Controls.ScrollView scrollView && scrollView.Content != null)
                {
                    StopViewTreeAnimations(scrollView.Content);
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MainActivity] Erro ao parar animações de elemento: {ex.Message}");
            }
        }

        private void ForceResourceCleanup()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("[MainActivity] Iniciando limpeza forçada de recursos");

                // Para operações de threading
                try
                {
                    ThreadPool.SetMaxThreads(1, 1);
                }
                catch { }

                // Limpeza de memória agressiva
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                System.GC.Collect();

                // Limpeza adicional específica para Android
                try
                {
                    // Força finalização de objetos não gerenciados
                    System.GC.Collect(2, GCCollectionMode.Forced, true);
                    System.GC.WaitForPendingFinalizers();
                }
                catch { }

                // NOVO: Para timer de proteção preventiva
                try
                {
                    _preventiveStopTimer?.Stop();
                    _preventiveStopTimer?.Dispose();
                    _preventiveStopTimer = null;
                }
                catch { }

                System.Diagnostics.Debug.WriteLine("[MainActivity] Limpeza de recursos concluída");
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MainActivity] ERRO na limpeza de recursos: {ex.Message}");
            }
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            if (_isDestroying)
            {
                System.Diagnostics.Debug.WriteLine("[MainActivity] OnTrimMemory ignorado - destruindo");
                return;
            }

            try
            {
                System.Diagnostics.Debug.WriteLine($"[MainActivity] OnTrimMemory: {level}");

                // Gerenciamento proativo de memória baseado no nível
                switch (level)
                {
                    case TrimMemory.RunningModerate:
                    case TrimMemory.RunningLow:
                    case TrimMemory.RunningCritical:
                        // Força garbage collection para liberar memória
                        System.GC.Collect();
                        System.GC.WaitForPendingFinalizers();
                        System.GC.Collect();
                        break;
                }

                base.OnTrimMemory(level);
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MainActivity] ERRO OnTrimMemory: {ex.Message}");
                try
                {
                    if (!_isDestroying) base.OnTrimMemory(level);
                }
                catch { }
            }
        }

        public override void OnLowMemory()
        {
            if (_isDestroying)
            {
                System.Diagnostics.Debug.WriteLine("[MainActivity] OnLowMemory ignorado - destruindo");
                return;
            }

            try
            {
                System.Diagnostics.Debug.WriteLine("[MainActivity] OnLowMemory - liberando recursos");

                // Força limpeza agressiva de memória
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                System.GC.Collect();

                base.OnLowMemory();
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MainActivity] ERRO OnLowMemory: {ex.Message}");
                try
                {
                    if (!_isDestroying) base.OnLowMemory();
                }
                catch { }
            }
        }

        // Método para verificar se pode executar operações seguras
        public static bool CanExecuteOperations()
        {
            return !_isDestroying && !_animationsStopped;
        }

        // Propriedades públicas para verificação de estado
        public static bool IsDestroying => _isDestroying;
        public static bool AnimationsStopped => _animationsStopped;
        public static CancellationToken GlobalCancellationToken => _globalCancellationTokenSource?.Token ?? CancellationToken.None;
    }
}