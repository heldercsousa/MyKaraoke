using MyKaraoke.View.Animations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyKaraoke.View.Behaviors
{
    public abstract class SafeAnimationBehavior<T> : Behavior<T>, IDisposable
    where T : VisualElement
    {
        private RobustAnimationManager? _animationManager;
        private readonly object _disposeLock = new object();
        private volatile bool _isDisposed;

        protected override void OnAttachedTo(T bindable)
        {
            base.OnAttachedTo(bindable);

            lock (_disposeLock)
            {
                if (_isDisposed) return;

                var pageId = GetPageIdentifier(bindable);
                _animationManager = GlobalAnimationCoordinator.Instance.GetOrCreateManagerForPage(pageId);

                // Registrar para cleanup automático quando elemento sai da árvore visual
                bindable.Unloaded += OnElementUnloaded;
            }
        }

        protected override void OnDetachingFrom(T bindable)
        {
            bindable.Unloaded -= OnElementUnloaded;
            Dispose();
            base.OnDetachingFrom(bindable);
        }

        private async void OnElementUnloaded(object? sender, EventArgs e)
        {
            if (sender is T element)
            {
                var pageId = GetPageIdentifier(element);
                await GlobalAnimationCoordinator.Instance.DisposeManagerForPage(pageId);
            }
        }

        private string GetPageIdentifier(VisualElement element)
        {
            // Encontrar página pai para identificação única
            var page = element;
            while (page.Parent != null)
            {
                page = page.Parent as VisualElement ?? page;
                if (page is Page)
                    break;
            }

            return $"{page.GetType().Name}_{page.GetHashCode()}";
        }

        protected async Task StartSafeAnimation(T element, Func<Task> animationTask)
        {
            if (_isDisposed || _animationManager == null) return;

            try
            {
                await animationTask();
            }
            catch (ObjectDisposedException)
            {
                // Elemento já foi disposed
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro na animação: {ex.Message}");
            }
        }

        public void Dispose()
        {
            lock (_disposeLock)
            {
                if (_isDisposed) return;
                _isDisposed = true;
            }

            // AnimationManager será disposed pelo coordenador global
            _animationManager = null;
        }
    }
}
