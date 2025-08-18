using System.Collections.Concurrent;

namespace MyKaraoke.View.Managers
{
    /// <summary>
    /// ✅ SINGLETON: Gerencia instâncias de páginas globalmente
    /// 🛡️ THREAD-SAFE: Usa ConcurrentDictionary para proteção
    /// 🎯 CENTRALIZADOR: Todas as proteções anti-duplicata em um lugar
    /// </summary>
    public class PageInstanceManager
    {
        #region Singleton

        private static readonly Lazy<PageInstanceManager> _instance = new Lazy<PageInstanceManager>(() => new PageInstanceManager());
        public static PageInstanceManager Instance => _instance.Value;

        private PageInstanceManager()
        {
            // 🧹 LIMPEZA: Timer para limpeza periódica de referências mortas (a cada 30 segundos)
            _cleanupTimer = new Timer(CleanupTimerCallback, null, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
        }

        #endregion

        #region Private Fields

        // 🛡️ THREAD-SAFE: Controle de navegações por tipo de página
        private readonly ConcurrentDictionary<Type, DateTime> _lastNavigationTimes = new ConcurrentDictionary<Type, DateTime>();

        // 🛡️ THREAD-SAFE: Controle de instâncias ativas por tipo
        private readonly ConcurrentDictionary<Type, List<WeakReference>> _activeInstances = new ConcurrentDictionary<Type, List<WeakReference>>();

        // 🛡️ THREAD-SAFE: Controle de flags de navegação em progresso
        private readonly ConcurrentDictionary<Type, bool> _navigationInProgress = new ConcurrentDictionary<Type, bool>();

        // 🧹 LIMPEZA: Timer para limpeza periódica de referências mortas
        private readonly Timer _cleanupTimer;

        // 🔒 LOCKS: Objeto para sincronização
        private readonly object _lockObject = new object();

        #endregion

        #region Public Methods

        /// <summary>
        /// 🛡️ VERIFICAÇÃO: Determina se pode navegar para um tipo de página
        /// </summary>
        /// <param name="pageType">Tipo da página de destino</param>
        /// <param name="debounceMs">Tempo de debounce em milissegundos</param>
        /// <returns>True se pode navegar, False caso contrário</returns>
        public bool CanNavigateToPage(Type pageType, int debounceMs = 1000)
        {
            try
            {
                lock (_lockObject)
                {
                    var now = DateTime.Now;

                    // 🛡️ PROTEÇÃO 1: Verifica se já está navegando
                    if (_navigationInProgress.GetValueOrDefault(pageType, false))
                    {
                        System.Diagnostics.Debug.WriteLine($"🚫 PageInstanceManager: Navegação para {pageType.Name} BLOQUEADA - já em progresso");
                        return false;
                    }

                    // 🛡️ PROTEÇÃO 2: Verifica debounce
                    if (_lastNavigationTimes.TryGetValue(pageType, out var lastTime))
                    {
                        var timeSinceLastNavigation = now - lastTime;
                        if (timeSinceLastNavigation.TotalMilliseconds < debounceMs)
                        {
                            System.Diagnostics.Debug.WriteLine($"🚫 PageInstanceManager: Navegação para {pageType.Name} BLOQUEADA - debounce (gap: {timeSinceLastNavigation.TotalMilliseconds}ms)");
                            return false;
                        }
                    }

                    // 🛡️ PROTEÇÃO 3: Verifica múltiplas instâncias ativas (opcional)
                    var activeCount = GetActiveInstanceCount(pageType);
                    if (activeCount > 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"⚠️ PageInstanceManager: {activeCount} instâncias ativas de {pageType.Name} - permitindo navegação mas alertando");
                        // Note: Não bloqueia, apenas alerta. Pode ser configurado para bloquear se necessário.
                    }

                    // ✅ AUTORIZAÇÃO: Marca como em progresso e atualiza tempo
                    _navigationInProgress[pageType] = true;
                    _lastNavigationTimes[pageType] = now;

                    // 🕒 LIBERAÇÃO: Agenda liberação do lock após 2 segundos
                    _ = Task.Run(async () =>
                    {
                        await Task.Delay(2000);
                        _navigationInProgress[pageType] = false;
                        System.Diagnostics.Debug.WriteLine($"🔓 PageInstanceManager: Lock de navegação liberado para {pageType.Name}");
                    });

                    System.Diagnostics.Debug.WriteLine($"✅ PageInstanceManager: Navegação AUTORIZADA para {pageType.Name}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ PageInstanceManager: Erro em CanNavigateToPage: {ex.Message}");
                return false; // Por segurança, bloqueia em caso de erro
            }
        }

        /// <summary>
        /// 📝 REGISTRO: Registra uma nova instância de página
        /// </summary>
        public void RegisterPageInstance(ContentPage page)
        {
            try
            {
                var pageType = page.GetType();
                var instances = _activeInstances.GetOrAdd(pageType, _ => new List<WeakReference>());

                lock (instances)
                {
                    instances.Add(new WeakReference(page));
                    System.Diagnostics.Debug.WriteLine($"📝 PageInstanceManager: Instância de {pageType.Name} registrada (Hash: {page.GetHashCode()}, Total: {instances.Count})");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ PageInstanceManager: Erro ao registrar instância: {ex.Message}");
            }
        }

        /// <summary>
        /// 🗑️ REMOÇÃO: Remove registro de uma instância de página
        /// </summary>
        public void UnregisterPageInstance(ContentPage page)
        {
            try
            {
                var pageType = page.GetType();
                if (_activeInstances.TryGetValue(pageType, out var instances))
                {
                    lock (instances)
                    {
                        var beforeCount = instances.Count;
                        instances.RemoveAll(wr => !wr.IsAlive || wr.Target == page);
                        var afterCount = instances.Count;

                        if (beforeCount != afterCount)
                        {
                            System.Diagnostics.Debug.WriteLine($"🗑️ PageInstanceManager: Instância de {pageType.Name} removida (Hash: {page.GetHashCode()})");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ PageInstanceManager: Erro ao remover instância: {ex.Message}");
            }
        }

        /// <summary>
        /// 📊 CONTAGEM: Retorna número de instâncias ativas de um tipo
        /// </summary>
        public int GetActiveInstanceCount(Type pageType)
        {
            try
            {
                if (!_activeInstances.TryGetValue(pageType, out var instances))
                    return 0;

                lock (instances)
                {
                    // Remove referências mortas
                    instances.RemoveAll(wr => !wr.IsAlive);
                    return instances.Count;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ PageInstanceManager: Erro ao contar instâncias: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// 🧹 LIMPEZA: Força limpeza de referências mortas
        /// </summary>
        public void CleanupDeadReferences()
        {
            try
            {
                var totalCleaned = 0;

                foreach (var kvp in _activeInstances.ToList())
                {
                    var instances = kvp.Value;
                    lock (instances)
                    {
                        var beforeCount = instances.Count;
                        instances.RemoveAll(wr => !wr.IsAlive);
                        var afterCount = instances.Count;
                        var cleaned = beforeCount - afterCount;

                        if (cleaned > 0)
                        {
                            totalCleaned += cleaned;
                            System.Diagnostics.Debug.WriteLine($"🧹 PageInstanceManager: {cleaned} referências mortas removidas de {kvp.Key.Name}");
                        }

                        // Remove entrada completamente se não há mais instâncias
                        if (afterCount == 0)
                        {
                            _activeInstances.TryRemove(kvp.Key, out _);
                        }
                    }
                }

                if (totalCleaned > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"🧹 PageInstanceManager: Limpeza concluída - {totalCleaned} referências mortas removidas no total");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ PageInstanceManager: Erro na limpeza: {ex.Message}");
            }
        }

        /// <summary>
        /// 📊 DIAGNÓSTICO: Retorna estatísticas das instâncias ativas
        /// </summary>
        public Dictionary<string, int> GetInstanceStatistics()
        {
            try
            {
                var stats = new Dictionary<string, int>();

                foreach (var kvp in _activeInstances.ToList())
                {
                    var count = GetActiveInstanceCount(kvp.Key);
                    if (count > 0)
                    {
                        stats[kvp.Key.Name] = count;
                    }
                }

                return stats;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ PageInstanceManager: Erro ao obter estatísticas: {ex.Message}");
                return new Dictionary<string, int>();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 🧹 CALLBACK: Método chamado pelo timer de limpeza
        /// </summary>
        private void CleanupTimerCallback(object state)
        {
            CleanupDeadReferences();
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// 🗑️ DISPOSAL: Limpa recursos quando aplicação é fechada
        /// </summary>
        public void Dispose()
        {
            try
            {
                _cleanupTimer?.Dispose();
                _activeInstances.Clear();
                _lastNavigationTimes.Clear();
                _navigationInProgress.Clear();

                System.Diagnostics.Debug.WriteLine($"🗑️ PageInstanceManager: Recursos liberados");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ PageInstanceManager: Erro no dispose: {ex.Message}");
            }
        }

        #endregion
    }
}