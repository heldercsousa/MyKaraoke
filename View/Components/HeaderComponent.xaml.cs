using MyKaraoke.View.Behaviors;
using System.Windows.Input;
using MauiView = Microsoft.Maui.Controls.View;

namespace MyKaraoke.View.Components
{
    public partial class HeaderComponent : ContentView
    {
        public static readonly BindableProperty TitleProperty =
            BindableProperty.Create(nameof(Title), typeof(string), typeof(HeaderComponent), string.Empty);

        public static readonly BindableProperty BackCommandProperty =
            BindableProperty.Create(nameof(BackCommand), typeof(ICommand), typeof(HeaderComponent), null);

        public static readonly BindableProperty ShowAddButtonProperty =
            BindableProperty.Create(nameof(ShowAddButton), typeof(bool), typeof(HeaderComponent), false);

        public static readonly BindableProperty AddCommandProperty =
            BindableProperty.Create(nameof(AddCommand), typeof(ICommand), typeof(HeaderComponent), null);

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

        public bool ShowAddButton
        {
            get => (bool)GetValue(ShowAddButtonProperty);
            set => SetValue(ShowAddButtonProperty, value);
        }

        public ICommand AddCommand
        {
            get => (ICommand)GetValue(AddCommandProperty);
            set => SetValue(AddCommandProperty, value);
        }

        /// <summary>
        /// Evento disparado quando o botão voltar é clicado
        /// </summary>
        public event EventHandler BackButtonClicked;

        /// <summary>
        /// Evento disparado quando o botão adicionar é clicado
        /// </summary>
        public event EventHandler AddButtonClicked;

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
        /// Handler do botão adicionar
        /// </summary>
        private void OnAddButtonClicked(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Add button clicked");

                // 1. Primeiro, verifica se há comando customizado
                if (AddCommand != null && AddCommand.CanExecute(null))
                {
                    AddCommand.Execute(null);
                    return;
                }

                // 2. Depois, verifica se há event handler customizado
                if (AddButtonClicked != null)
                {
                    AddButtonClicked.Invoke(this, EventArgs.Empty);
                    return;
                }

                System.Diagnostics.Debug.WriteLine("No add command or event handler configured");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"OnAddButtonClicked - Error: {ex.Message}");
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
                else if (pageType.Name == "PersonPage" || pageType.Name == "SpotPage" || pageType.Name == "SpotFormPage")
                {
                    // PersonPage/SpotPage/SpotFormPage → voltar para página anterior
                    await UseDefaultNavigationAsync();
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


        /// <summary>
        /// 🎯 NAVEGAÇÃO SEGURA: Configurar SafeNavigationBehavior para botão voltar
        /// Chame este método no OnHandlerChanged do HeaderComponent
        /// </summary>
        public void ConfigureSafeBackNavigation(Type targetPageType, int debounceMs = 500)
        {
            try
            {
                // 🔍 BUSCA: Botão voltar no HeaderComponent
                var backButton = FindBackButton();
                if (backButton != null)
                {
                    // ✅ CRIA: SafeNavigationBehavior para navegação segura
                    var safeBehavior = new SafeNavigationBehavior
                    {
                        TargetPageType = targetPageType,
                        DebounceMilliseconds = debounceMs
                    };

                    // 🎯 ANEXA: Behavior ao botão voltar
                    backButton.Behaviors.Add(safeBehavior);

                    System.Diagnostics.Debug.WriteLine($"✅ HeaderComponent: SafeNavigationBehavior configurado para {targetPageType.Name}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ HeaderComponent: Botão voltar não encontrado");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ HeaderComponent: Erro ao configurar navegação segura: {ex.Message}");
            }
        }

        /// <summary>
        /// 🔍 BUSCA: Encontra o botão voltar no HeaderComponent
        /// </summary>
        private VisualElement FindBackButton()
        {
            try
            {
                // 🔍 ESTRATÉGIA: Busca por campo nomeado comum
                var backButtonField = this.GetType().GetField("backButton",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                if (backButtonField != null)
                {
                    return backButtonField.GetValue(this) as VisualElement;
                }

                // 🔍 FALLBACK: Busca por Button com texto "←" ou image "setaesquerda.png"
                return FindBackButtonInContent(this.Content);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ HeaderComponent: Erro ao buscar botão voltar: {ex.Message}");
                return null;
            }
        }


        /// <summary>
        /// 🔍 RECURSIVA: Busca botão voltar no conteúdo
        /// </summary>
        private VisualElement FindBackButtonInContent(MauiView content)
        {
            if (content == null) return null;

            // 🎯 BOTÃO: Se é Button com texto "←"
            if (content is Button button && (button.Text == "←" || button.Text == "Voltar"))
            {
                return button;
            }

            // 🎯 IMAGE: Se é Image com source "setaesquerda.png"
            if (content is Image image && image.Source?.ToString().Contains("setaesquerda") == true)
            {
                return image;
            }

            // 🔍 LAYOUT: Busca recursivamente em layouts
            if (content is Layout layout)
            {
                foreach (var child in layout.Children)
                {
                    if (child is MauiView childView)
                    {
                        var found = FindBackButtonInContent(childView);
                        if (found != null) return found;
                    }
                }
            }

            // 🔍 CONTENTVIEW: Busca dentro de ContentView
            if (content is ContentView contentView && contentView.Content != null)
            {
                return FindBackButtonInContent(contentView.Content);
            }

            return null;
        }
    }
}