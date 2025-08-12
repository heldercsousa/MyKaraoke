using MyKaraoke.View.Animations;  // ✅ MANTIDO: Para GlobalAnimationCoordinator
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyKaraoke.View
{
    public abstract class BaseAnimatedPage : ContentPage, IAsyncDisposable
    {
        private readonly List<IDisposable> _disposables = new();
        private readonly string _pageId;
        private volatile bool _isDisposed;

        protected BaseAnimatedPage()
        {
            _pageId = $"{GetType().Name}_{GetHashCode()}";
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            InitializeAnimations();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            // ✅ CORRIGIDO: Usar OnDisappearing em vez de OnNavigatedFrom
            // Cleanup quando página desaparece
            _ = Task.Run(async () => await DisposeAsync());
        }

        protected virtual void InitializeAnimations()
        {
            // Override em classes filhas para inicializar animações
        }

        protected void RegisterForDisposal(IDisposable disposable)
        {
            if (!_isDisposed)
            {
                _disposables.Add(disposable);
            }
            else
            {
                disposable.Dispose();
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_isDisposed) return;
            _isDisposed = true;

            try
            {
                // 1. Parar AnimationManager desta página
                await GlobalAnimationCoordinator.Instance.DisposeManagerForPage(_pageId);

                // 2. Cleanup behaviors e outros recursos
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    try
                    {
                        // Parar todas as animações desta página
                        this.AbortAnimation("PageAnimations");

                        // Limpar behaviors com animações
                        foreach (var child in this.GetVisualTreeDescendants().OfType<VisualElement>())
                        {
                            child.CancelAnimations();

                            foreach (var behavior in child.Behaviors.OfType<IDisposable>())
                            {
                                behavior.Dispose();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Erro durante cleanup UI: {ex.Message}");
                    }
                });

                // 3. Dispose recursos registrados
                foreach (var disposable in _disposables.ToList())
                {
                    try
                    {
                        disposable.Dispose();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Erro ao fazer dispose: {ex.Message}");
                    }
                }
                _disposables.Clear();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro geral durante DisposeAsync: {ex.Message}");
            }
        }
    }
}