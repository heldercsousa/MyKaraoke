using System.Windows.Input;

namespace MyKaraoke.View.Components
{
    public partial class HeaderComponent : ContentView
    {
        public static readonly BindableProperty TitleProperty =
            BindableProperty.Create(nameof(Title), typeof(string), typeof(HeaderComponent), string.Empty);

        public static readonly BindableProperty BackCommandProperty =
            BindableProperty.Create(nameof(BackCommand), typeof(ICommand), typeof(HeaderComponent), null);

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public ICommand BackCommand
        {
            get => (ICommand)GetValue(BackCommandProperty);
            set => SetValue(BackCommandProperty, value);
        }

        /// <summary>
        /// Evento disparado quando o botão voltar é clicado
        /// Permite que a view pai personalize o comportamento
        /// </summary>
        public event EventHandler BackButtonClicked;

        public HeaderComponent()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handler do botão voltar - usa navegação inteligente automática
        /// </summary>
        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            try
            {
                // 1. Primeiro, verifica se há comando customizado
                if (BackCommand != null && BackCommand.CanExecute(null))
                {
                    BackCommand.Execute(null);
                    return;
                }

                // 2. Depois, verifica se há event handler customizado
                if (BackButtonClicked != null)
                {
                    BackButtonClicked.Invoke(this, EventArgs.Empty);
                    return;
                }

                // 3. Navegação automática inteligente
                await HandleAutomaticNavigationAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"OnBackButtonClicked - Error: {ex.Message}");

                // Fallback: tenta sair da aplicação se estiver na página principal
                try
                {
                    await HandleFallbackNavigationAsync();
                }
                catch (Exception fallbackEx)
                {
                    System.Diagnostics.Debug.WriteLine($"Fallback navigation failed: {fallbackEx.Message}");
                }
            }
        }

        /// <summary>
        /// Lógica de navegação automática baseada no contexto da página atual
        /// </summary>
        private async Task HandleAutomaticNavigationAsync()
        {
            try
            {
                // Encontra a página atual
                var currentPage = GetCurrentPage();
                if (currentPage == null)
                {
                    System.Diagnostics.Debug.WriteLine("Não foi possível encontrar a página atual");
                    return;
                }

                var pageType = currentPage.GetType();
                System.Diagnostics.Debug.WriteLine($"Página atual: {pageType.Name}");

                // Lógica específica baseada no tipo da página
                if (pageType.Name == "StackPage")
                {
                    // StackPage = página principal → sair do app
                    await ExitApplicationAsync();
                }
                else if (pageType.Name == "PersonPage")
                {
                    // PersonPage → voltar para StackPage
                    await NavigateToStackPageAsync();
                }
                else if (IsMainPage(currentPage))
                {
                    // Outras páginas principais → sair do app
                    await ExitApplicationAsync();
                }
                else
                {
                    // Páginas secundárias → usar navegação padrão
                    await UseDefaultNavigationAsync();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"HandleAutomaticNavigation - Error: {ex.Message}");
                // Fallback para navegação padrão
                await UseDefaultNavigationAsync();
            }
        }

        /// <summary>
        /// Usa a navegação padrão do MAUI (.NET)
        /// </summary>
        private async Task UseDefaultNavigationAsync()
        {
            try
            {
                var currentPage = GetCurrentPage();
                if (currentPage?.Navigation?.NavigationStack?.Count > 1)
                {
                    // Há páginas na pilha → volta uma página
                    await currentPage.Navigation.PopAsync();
                    System.Diagnostics.Debug.WriteLine("Navegação padrão: PopAsync() executado");
                }
                else
                {
                    // Não há páginas na pilha → sair do app
                    await ExitApplicationAsync();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UseDefaultNavigation - Error: {ex.Message}");
                await ExitApplicationAsync();
            }
        }

        /// <summary>
        /// Navega especificamente para a StackPage
        /// </summary>
        private async Task NavigateToStackPageAsync()
        {
            try
            {
                var currentPage = GetCurrentPage();
                if (currentPage != null)
                {
                    // Tenta voltar pela pilha de navegação primeiro
                    if (currentPage.Navigation?.NavigationStack?.Count > 1)
                    {
                        await currentPage.Navigation.PopAsync();
                        System.Diagnostics.Debug.WriteLine("Voltou para StackPage via PopAsync");
                        return;
                    }

                    // Se não conseguir, cria nova StackPage
                    await currentPage.Navigation.PushAsync(new StackPage());
                    System.Diagnostics.Debug.WriteLine("Navegou para nova StackPage");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"NavigateToStackPage - Error: {ex.Message}");
                await UseDefaultNavigationAsync();
            }
        }

        /// <summary>
        /// Sai da aplicação de forma segura
        /// </summary>
        private async Task ExitApplicationAsync()
        {
            try
            {
                // Pequeno delay para melhor UX
                await Task.Delay(100);

                Application.Current?.Quit();
                System.Diagnostics.Debug.WriteLine("Aplicação fechada via HeaderComponent");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ExitApplication - Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Fallback para casos extremos
        /// </summary>
        private async Task HandleFallbackNavigationAsync()
        {
            try
            {
                // Tenta navegação padrão como último recurso
                await UseDefaultNavigationAsync();
            }
            catch
            {
                // Se tudo falhar, sai da aplicação
                Application.Current?.Quit();
            }
        }

        /// <summary>
        /// Obtém a página atual navegando pela hierarquia
        /// </summary>
        private ContentPage GetCurrentPage()
        {
            try
            {
                // Método 1: Navega pela hierarquia de parent
                var element = this.Parent;
                while (element != null)
                {
                    if (element is ContentPage page)
                        return page;
                    element = element.Parent;
                }

                // Método 2: Via Application.Current
                if (Application.Current?.MainPage is ContentPage mainPage)
                    return mainPage;

                // Método 3: Via Navigation stack
                if (Application.Current?.MainPage?.Navigation?.NavigationStack?.LastOrDefault() is ContentPage lastPage)
                    return lastPage;

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetCurrentPage - Error: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Verifica se é uma página principal (não tem parent na navegação)
        /// </summary>
        private bool IsMainPage(ContentPage page)
        {
            try
            {
                return page?.Navigation?.NavigationStack?.Count <= 1;
            }
            catch
            {
                return true; // Em caso de dúvida, assume que é página principal
            }
        }
    }
}