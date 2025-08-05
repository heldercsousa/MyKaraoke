using MyKaraoke.View.Components;
using System.Windows.Input;

namespace MyKaraoke.View.Behaviors
{
    /// <summary>
    /// Gerencia o ciclo de vida de uma ContentPage, orquestrando o carregamento
    /// de dados, a exibição de um indicador de loading e as animações de uma NavBar.
    /// </summary>
    public class PageLifecycleBehavior : Behavior<ContentPage>
    {
        #region Bindable Properties

        /// <summary>
        /// A barra de navegação (que implementa IAnimatableNavBar) a ser animada.
        /// </summary>
        public static readonly BindableProperty NavBarProperty =
            BindableProperty.Create(nameof(NavBar), typeof(IAnimatableNavBar), typeof(PageLifecycleBehavior));

        /// <summary>
        /// O comando a ser executado para carregar os dados da página.
        /// </summary>
        public static readonly BindableProperty LoadDataCommandProperty =
            BindableProperty.Create(nameof(LoadDataCommand), typeof(ICommand), typeof(PageLifecycleBehavior));

        /// <summary>
        /// O elemento visual que serve como indicador de carregamento (ex: um overlay).
        /// </summary>
        public static readonly BindableProperty LoadingIndicatorProperty =
            BindableProperty.Create(nameof(LoadingIndicator), typeof(VisualElement), typeof(PageLifecycleBehavior));

        /// <summary>
        /// O container do conteúdo principal da página, que será escondido durante o carregamento.
        /// </summary>
        public static readonly BindableProperty MainContentProperty =
            BindableProperty.Create(nameof(MainContent), typeof(VisualElement), typeof(PageLifecycleBehavior));

        public IAnimatableNavBar NavBar { get => (IAnimatableNavBar)GetValue(NavBarProperty); set => SetValue(NavBarProperty, value); }
        public ICommand LoadDataCommand { get => (ICommand)GetValue(LoadDataCommandProperty); set => SetValue(LoadDataCommandProperty, value); }
        public VisualElement LoadingIndicator { get => (VisualElement)GetValue(LoadingIndicatorProperty); set => SetValue(LoadingIndicatorProperty, value); }
        public VisualElement MainContent { get => (VisualElement)GetValue(MainContentProperty); set => SetValue(MainContentProperty, value); }

        #endregion

        private ContentPage _associatedPage;

        /// <summary>
        /// Anexa o behavior à página e se inscreve nos eventos de ciclo de vida.
        /// </summary>
        protected override void OnAttachedTo(ContentPage page)
        {
            base.OnAttachedTo(page);
            _associatedPage = page;
            _associatedPage.Appearing += OnPageAppearing;
            _associatedPage.Disappearing += OnPageDisappearing;
        }

        /// <summary>
        /// Desanexa o behavior e cancela a inscrição nos eventos para evitar memory leaks.
        /// </summary>
        protected override void OnDetachingFrom(ContentPage page)
        {
            if (_associatedPage != null)
            {
                _associatedPage.Appearing -= OnPageAppearing;
                _associatedPage.Disappearing -= OnPageDisappearing;
            }
            base.OnDetachingFrom(page);
            _associatedPage = null;
        }

        /// <summary>
        /// 🎯 CORREÇÃO: Orquestra as ações aguardando navbar estar pronta
        /// </summary>
        private async void OnPageAppearing(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("[PageLifecycleBehavior] OnPageAppearing iniciado");

            // 🎯 CORREÇÃO 1: Aguarda navbar estar pronta PRIMEIRO
            await EnsureNavBarIsReady();

            // 2. Carrega os dados, mostrando o indicador de loading.
            await ExecuteLoadDataAsync();

            // 3. Após os dados carregarem e o conteúdo ser exibido, anima a entrada da navbar.
            if (NavBar != null)
            {
                System.Diagnostics.Debug.WriteLine("[PageLifecycleBehavior] Chamando NavBar.ShowAsync()");
                await NavBar.ShowAsync();
                System.Diagnostics.Debug.WriteLine("[PageLifecycleBehavior] NavBar.ShowAsync() concluído");
            }
        }

        /// <summary>
        /// 🎯 NOVO: Aguarda navbar ter botões configurados antes de prosseguir
        /// </summary>
        private async Task EnsureNavBarIsReady()
        {
            if (NavBar == null)
            {
                System.Diagnostics.Debug.WriteLine("[PageLifecycleBehavior] Nenhuma navbar configurada");
                return;
            }

            System.Diagnostics.Debug.WriteLine("[PageLifecycleBehavior] Aguardando navbar estar pronta...");

            // Aguarda até 3 segundos para navbar se configurar
            int attempts = 0;
            const int maxAttempts = 30; // 30 x 100ms = 3 segundos

            while (attempts < maxAttempts)
            {
                await Task.Delay(100);
                attempts++;

                // 🎯 VERIFICAÇÃO: Tenta verificar se navbar tem botões
                if (await IsNavBarReady())
                {
                    System.Diagnostics.Debug.WriteLine($"[PageLifecycleBehavior] ✅ Navbar pronta após {attempts} tentativas");
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"[PageLifecycleBehavior] Tentativa {attempts}/{maxAttempts} aguardando navbar");
            }

            System.Diagnostics.Debug.WriteLine($"[PageLifecycleBehavior] ⚠️ Timeout aguardando navbar - continuando mesmo assim");
        }

        /// <summary>
        /// 🎯 NOVO: Verifica se navbar está pronta para mostrar botões
        /// </summary>
        private async Task<bool> IsNavBarReady()
        {
            try
            {
                // 🎯 ESTRATÉGIA: Tenta chamar ShowAsync e ver se funciona sem erro
                // Se NavBar tiver botões, ShowAsync não falhará

                // Para componentes que implementam IAnimatableNavBar via NavBarBehavior,
                // podemos verificar se tem conteúdo verificando propriedades do ContentView
                if (NavBar is ContentView navContentView)
                {
                    // Verifica se tem filhos (indica que botões foram criados)
                    return await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        try
                        {
                            // Se é um ContentView com conteúdo, verifica se tem elementos
                            if (navContentView.Content is Grid grid)
                            {
                                return grid.Children.Count > 0;
                            }
                            return navContentView.Content != null;
                        }
                        catch
                        {
                            return false;
                        }
                    });
                }

                // Para outros tipos de navbar, assume que está pronto após um delay
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PageLifecycleBehavior] Erro verificando navbar: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Orquestra as ações quando a página desaparece.
        /// </summary>
        private async void OnPageDisappearing(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("[PageLifecycleBehavior] OnPageDisappearing iniciado");

            if (NavBar != null)
            {
                try
                {
                    // 🎯 CORREÇÃO: Aguarda completamente a animação parar
                    await NavBar.HideAsync();

                    // 🎯 AGUARDA mais tempo para garantir que parou
                    await Task.Delay(100);

                    System.Diagnostics.Debug.WriteLine("[PageLifecycleBehavior] NavBar.HideAsync() concluído completamente");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[PageLifecycleBehavior] Erro ao esconder navbar: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Executa o comando de carregamento de dados de forma segura, gerenciando o estado da UI.
        /// </summary>
        private async Task ExecuteLoadDataAsync()
        {
            if (LoadDataCommand == null || !LoadDataCommand.CanExecute(null)) return;

            SetLoadingState(true);
            try
            {
                System.Diagnostics.Debug.WriteLine("[PageLifecycleBehavior] Executando LoadDataCommand");
                // Executa o comando fornecido pela página.
                await Task.Run(() => LoadDataCommand.Execute(null));
                System.Diagnostics.Debug.WriteLine("[PageLifecycleBehavior] LoadDataCommand concluído");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PageLifecycleBehavior] Erro ao executar LoadDataCommand: {ex.Message}");
            }
            finally
            {
                SetLoadingState(false);
            }
        }

        /// <summary>
        /// Controla a visibilidade do indicador de loading e do conteúdo principal.
        /// </summary>
        private void SetLoadingState(bool isLoading)
        {
            if (LoadingIndicator != null)
                LoadingIndicator.IsVisible = isLoading;

            if (MainContent != null)
                MainContent.IsVisible = !isLoading;
        }
    }
}