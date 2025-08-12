using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyKaraoke.View.Animations
{
    public class GlobalAnimationCoordinator
    {
        private static readonly Lazy<GlobalAnimationCoordinator> _instance =
            new(() => new GlobalAnimationCoordinator());

        public static GlobalAnimationCoordinator Instance => _instance.Value;

        private readonly ConcurrentDictionary<string, RobustAnimationManager> _pageManagers = new();
        private readonly SemaphoreSlim _disposalSemaphore = new(1, 1);

        public RobustAnimationManager GetOrCreateManagerForPage(string pageId)
        {
            return _pageManagers.GetOrAdd(pageId, _ => new RobustAnimationManager());
        }

        public async Task DisposeManagerForPage(string pageId)
        {
            if (_pageManagers.TryRemove(pageId, out var manager))
            {
                await manager.StopAllAnimationsCompletely();
                manager.Dispose();
            }
        }

        public async Task DisposeAll()
        {
            await _disposalSemaphore.WaitAsync();

            try
            {
                var managers = _pageManagers.Values.ToList();
                _pageManagers.Clear();

                var disposeTasks = managers.Select(async manager =>
                {
                    try
                    {
                        await manager.StopAllAnimationsCompletely();
                        manager.Dispose();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Erro ao fazer dispose de manager: {ex.Message}");
                    }
                });

                await Task.WhenAll(disposeTasks);
            }
            finally
            {
                _disposalSemaphore.Release();
            }
        }
    }
}
