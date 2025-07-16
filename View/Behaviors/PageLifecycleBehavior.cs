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
        /// Orquestra as ações quando a página aparece.
        /// </summary>
        private async void OnPageAppearing(object sender, EventArgs e)
        {
            // 1. Carrega os dados primeiro, mostrando o indicador de loading.
            await ExecuteLoadDataAsync();

            // 2. Após os dados carregarem e o conteúdo ser exibido, anima a entrada da navbar.
            if (NavBar != null)
            {
                await NavBar.ShowAsync();
            }
        }

        /// <summary>
        /// Orquestra as ações quando a página desaparece.
        /// </summary>
        private async void OnPageDisappearing(object sender, EventArgs e)
        {
            if (NavBar != null)
            {
                await NavBar.HideAsync();
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
                // Executa o comando fornecido pela página.
                await Task.Run(() => LoadDataCommand.Execute(null));
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