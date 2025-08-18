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
        /// </summary>
        public event EventHandler BackButtonClicked;

        public HeaderComponent()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handler do botão voltar - MINIMALISTA: apenas delega
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

                // 3. ✅ DELEGAÇÃO: Busca SafeNavigationBehavior na página e delega
                await DelegateToSafeNavigationBehaviorAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"HeaderComponent: OnBackButtonClicked - Error: {ex.Message}");
                await HandleSpecialCaseNavigationAsync();
            }
        }

        /// <summary>
        /// ✅ DELEGAÇÃO: Encontra e usa SafeNavigationBehavior da página
        /// </summary>
        private async Task DelegateToSafeNavigationBehaviorAsync()
        {
            try
            {
                var currentPage = GetCurrentPage();
                if (currentPage == null)
                {
                    System.Diagnostics.Debug.WriteLine("HeaderComponent: Página atual não encontrada");
                    await HandleSpecialCaseNavigationAsync();
                    return;
                }

                // 🔍 BUSCA: SafeNavigationBehavior para navegação de volta
                var backBehavior = FindBackNavigationBehavior(currentPage);
                if (backBehavior != null)
                {
                    System.Diagnostics.Debug.WriteLine($"HeaderComponent: Delegando para SafeNavigationBehavior");
                    await backBehavior.NavigateToPageAsync();
                    return;
                }

                // ⚠️ AVISO: Nenhum SafeNavigationBehavior encontrado
                System.Diagnostics.Debug.WriteLine($"HeaderComponent: Nenhum SafeNavigationBehavior encontrado para {currentPage.GetType().Name}");
                await HandleSpecialCaseNavigationAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"HeaderComponent: Erro na delegação: {ex.Message}");
                await HandleSpecialCaseNavigationAsync();
            }
        }

        /// <summary>
        /// 🔍 BUSCA: Encontra SafeNavigationBehavior de volta na página
        /// </summary>
        private SafeNavigationBehavior FindBackNavigationBehavior(ContentPage currentPage)
        {
            try
            {
                var behaviors = currentPage.Behaviors?.OfType<SafeNavigationBehavior>();
                if (behaviors == null || !behaviors.Any())
                {
                    return null;
                }

                // 🎯 PRIORIDADE 1: Behavior explicitamente nomeado como "BackNavigationBehavior"
                var namedBackBehavior = currentPage.FindByName<SafeNavigationBehavior>("BackNavigationBehavior");
                if (namedBackBehavior != null)
                {
                    return namedBackBehavior;
                }

                // 🎯 PRIORIDADE 2: Se há apenas um SafeNavigationBehavior, assume que é o de volta
                var behaviorsList = behaviors.ToList();
                if (behaviorsList.Count == 1)
                {
                    return behaviorsList[0];
                }

                // 🎯 PRIORIDADE 3: Procura behavior que NÃO é para formulários (não vai para FormPage)
                var nonFormBehavior = behaviorsList.FirstOrDefault(b =>
                    b.TargetPageType != null && !b.TargetPageType.Name.Contains("Form"));
                if (nonFormBehavior != null)
                {
                    return nonFormBehavior;
                }

                // 🎯 ÚLTIMO RECURSO: Pega o primeiro disponível
                return behaviorsList.FirstOrDefault();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"HeaderComponent: Erro ao buscar SafeNavigationBehavior: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 🎯 CASOS ESPECIAIS: Lida com casos onde não há SafeNavigationBehavior
        /// </summary>
        private async Task HandleSpecialCaseNavigationAsync()
        {
            try
            {
                var currentPage = GetCurrentPage();
                if (currentPage == null)
                {
                    return;
                }

                // ✅ ÚNICA EXCEÇÃO: StackPage sai da aplicação
                if (currentPage.GetType().Name == "StackPage")
                {
                    System.Diagnostics.Debug.WriteLine("HeaderComponent: StackPage detectada - saindo da aplicação");
                    await ExitApplicationAsync();
                    return;
                }

                // 🛡️ FALLBACK: Para outras páginas, tenta PopAsync simples
                if (currentPage.Navigation?.NavigationStack?.Count > 1)
                {
                    await currentPage.Navigation.PopAsync();
                    System.Diagnostics.Debug.WriteLine("HeaderComponent: PopAsync simples executado como fallback");
                }
                else
                {
                    // Se não há stack, sai da aplicação
                    await ExitApplicationAsync();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"HeaderComponent: Erro nos casos especiais: {ex.Message}");
                await ExitApplicationAsync();
            }
        }

        /// <summary>
        /// 🚪 SAÍDA: Sai da aplicação
        /// </summary>
        private async Task ExitApplicationAsync()
        {
            try
            {
                await Task.Delay(100);
                Application.Current?.Quit();
                System.Diagnostics.Debug.WriteLine("HeaderComponent: Aplicação fechada");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"HeaderComponent: Erro ao sair da aplicação: {ex.Message}");
            }
        }

        /// <summary>
        /// 🔍 HELPER: Obtém a página atual
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

                // Método 2: Via Application.Current.MainPage
                if (Application.Current?.MainPage is NavigationPage navPage)
                {
                    return navPage.CurrentPage as ContentPage;
                }

                if (Application.Current?.MainPage is ContentPage mainPage)
                    return mainPage;

                // Método 3: Via Navigation stack
                if (Application.Current?.MainPage?.Navigation?.NavigationStack?.LastOrDefault() is ContentPage lastPage)
                    return lastPage;

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"HeaderComponent: GetCurrentPage - Error: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 🎯 CONFIGURAÇÃO: Método utilitário para páginas que precisem configurar programaticamente
        /// (Mantido para compatibilidade, mas não é necessário na maioria dos casos)
        /// </summary>
        public void ConfigureSafeBackNavigation(Type targetPageType, int debounceMs = 500)
        {
            try
            {
                var backButton = FindBackButton();
                if (backButton != null)
                {
                    var safeBehavior = new SafeNavigationBehavior
                    {
                        TargetPageType = targetPageType,
                        DebounceMilliseconds = debounceMs
                    };

                    backButton.Behaviors.Add(safeBehavior);
                    System.Diagnostics.Debug.WriteLine($"HeaderComponent: SafeNavigationBehavior configurado para {targetPageType.Name}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"HeaderComponent: Erro ao configurar navegação: {ex.Message}");
            }
        }

        /// <summary>
        /// 🔍 BUSCA: Encontra o botão voltar no HeaderComponent
        /// </summary>
        private VisualElement FindBackButton()
        {
            try
            {
                return FindBackButtonInContent(this.Content);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"HeaderComponent: Erro ao buscar botão voltar: {ex.Message}");
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

            // 🎯 STACKLAYOUT: Se é StackLayout com TapGestureRecognizer (padrão atual)
            if (content is StackLayout stackLayout &&
                stackLayout.GestureRecognizers?.Any(g => g is TapGestureRecognizer) == true)
            {
                if (stackLayout.Children?.Any(c => c is Image img &&
                    img.Source?.ToString().Contains("setaesquerda") == true) == true)
                {
                    return stackLayout;
                }
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