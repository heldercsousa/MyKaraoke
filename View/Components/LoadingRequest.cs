using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyKaraoke.View.Components
{
    /// <summary>
    /// Requisição de loading com metadados completos
    /// </summary>
    public class LoadingRequest
    {
        public string RequesterId { get; set; }
        public string Message { get; set; }
        public LoadingPriority Priority { get; set; }
        public LoadingContext Context { get; set; }
        public DateTime RequestTime { get; set; }
        public bool IsPersistent { get; set; } // Se deve manter loading até explicitamente removido
        public TimeSpan? AutoHideAfter { get; set; } // Auto-hide após tempo especificado

        public LoadingRequest(string requesterId, string message, LoadingPriority priority, LoadingContext context, bool isPersistent = false, TimeSpan? autoHideAfter = null)
        {
            RequesterId = requesterId;
            Message = message;
            Priority = priority;
            Context = context;
            RequestTime = DateTime.Now;
            IsPersistent = isPersistent;
            AutoHideAfter = autoHideAfter;
        }
    }
}
