using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyKaraoke.View
{
    public static class CrashPreventionService
    {
        private static readonly HashSet<string> ProblematicDevices = new()
    {
        "SM-G950F", "SM-G960F", "SM-N950F", // Samsung Galaxy
        "LYA-L29", "EML-L29", "VOG-L29"     // Huawei
    };

        public static bool IsProblematicDevice()
        {
            var model = DeviceInfo.Model;
            var manufacturer = DeviceInfo.Manufacturer;

            return ProblematicDevices.Contains(model) ||
                   manufacturer.Contains("samsung", StringComparison.OrdinalIgnoreCase) ||
                   manufacturer.Contains("huawei", StringComparison.OrdinalIgnoreCase);
        }

        public static void LogAnimationLifecycle(string eventName, string context)
        {
            if (IsProblematicDevice())
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[ANIMATION_LIFECYCLE] {DateTime.Now:HH:mm:ss.fff} - {eventName} - {context} - {DeviceInfo.Model}");
            }
        }
    }
}
