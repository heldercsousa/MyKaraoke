using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyKaraoke.View.Animations;  // ✅ Para GlobalAnimationCoordinator

#if ANDROID
using Android.OS;               // ✅ Para Android.OS.Build
using Java.Lang;               // ✅ Para JavaSystem.Exit
#endif

namespace MyKaraoke.View
{
    public class SafeAppLifecycleManager
    {
        private static readonly string[] ProblematicManufacturers =
            { "SAMSUNG", "HUAWEI", "XIAOMI" };

        public static async Task ShutdownSafely()
        {
#if ANDROID
            var manufacturer = Build.Manufacturer?.ToUpperInvariant();

            if (ProblematicManufacturers.Any(m => manufacturer?.Contains(m) == true))
            {
                // Cleanup ordenado antes da saída forçada
                await CleanupAnimationsBeforeExit();

                // Força saída do processo antes da destruição de mutexes
                JavaSystem.Exit(0);
            }
            else
            {
                // Shutdown normal para outros dispositivos
                await CleanupAnimationsBeforeExit();
            }
#else
            // Para outras plataformas, apenas cleanup normal
            await CleanupAnimationsBeforeExit();
#endif
        }

        // ✅ CORRIGIDO: Método agora é público para ser usado no MainActivity
        public static async Task CleanupAnimationsBeforeExit()
        {
            try
            {
                // Parar AnimationManager global
                await GlobalAnimationCoordinator.Instance.DisposeAll();

                // Força garbage collection
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                // Pequeno delay para threads nativas finalizarem
                await Task.Delay(100);
            }
            catch (System.Exception ex)
            {
                // Log error mas continue com shutdown
                System.Diagnostics.Debug.WriteLine($"Erro durante cleanup: {ex.Message}");
            }
        }
    }
}