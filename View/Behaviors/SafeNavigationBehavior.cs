using MyKaraoke.View.Managers;
using System.Windows.Input;
using MauiView = Microsoft.Maui.Controls.View;

namespace MyKaraoke.View.Behaviors
{
    /// <summary>
    /// ✅ BEHAVIOR INTELIGENTE: Previne navegação duplicada + navegação inteligente por stack
    /// 🛡️ REUTILIZÁVEL: Pode ser aplicado a qualquer elemento que navega
    /// 🎯 CONFIGURÁVEL: Via propriedades bindáveis
    /// 🧠 INTELIGENTE: Determina automaticamente para onde navegar baseado no stack
    /// </summary>
    public class SafeNavigationBehavior : Behavior<VisualElement>
    {
        #region Bindable Properties

        public static readonly BindableProperty TargetPageTypeProperty =
            BindableProperty.Create(nameof(TargetPageType), typeof(Type), typeof(SafeNavigationBehavior));

        public static readonly BindableProperty NavigationCommandProperty =
            BindableProperty.Create(nameof(NavigationCommand), typeof(ICommand), typeof(SafeNavigationBehavior));

        public static readonly BindableProperty DebounceMillisecondsProperty =
            BindableProperty.Create(nameof(DebounceMilliseconds), typeof(int), typeof(SafeNavigationBehavior), 1000);

        public static readonly BindableProperty CreatePageFuncProperty =
            BindableProperty.Create(nameof(CreatePageFunc), typeof(Func<ContentPage>), typeof(SafeNavigationBehavior));

        // ✅ NOVA: Propriedade para habilitar navegação inteligente por stack
        public static readonly BindableProperty EnableSmartStackNavigationProperty =
            BindableProperty.Create(nameof(EnableSmartStackNavigation), typeof(bool), typeof(SafeNavigationBehavior), true);

        #endregion

        #region Properties

        /// <summary>
        /// Tipo da página de destino (para controle de instâncias)
        /// </summary>
        public Type TargetPageType
        {
            get => (Type)GetValue(TargetPageTypeProperty);
            set => SetValue(TargetPageTypeProperty, value);
        }

        /// <summary>
        /// Comando customizado de navegação (opcional)
        /// </summary>
        public ICommand NavigationCommand
        {
            get => (ICommand)GetValue(NavigationCommandProperty);
            set => SetValue(NavigationCommandProperty, value);
        }

        /// <summary>
        /// Tempo de debounce em milissegundos (padrão: 1000ms)
        /// </summary>
        public int DebounceMilliseconds
        {
            get => (int)GetValue(DebounceMillisecondsProperty);
            set => SetValue(DebounceMillisecondsProperty, value);
        }

        /// <summary>
        /// Função para criar a página (permite configuração personalizada)
        /// </summary>
        public Func<ContentPage> CreatePageFunc
        {
            get => (Func<ContentPage>)GetValue(CreatePageFuncProperty);
            set => SetValue(CreatePageFuncProperty, value);
        }

        /// <summary>
        /// ✅ NOVA: Habilita navegação inteligente por stack (padrão: true)
        /// </summary>
        public bool EnableSmartStackNavigation
        {
            get => (bool)GetValue(EnableSmartStackNavigationProperty);
            set => SetValue(EnableSmartStackNavigationProperty, value);
        }

        #endregion

        #region Private Fields

        private VisualElement _associatedElement;
        private readonly PageInstanceManager _instanceManager = PageInstanceManager.Instance;

        #endregion

        #region Behavior Lifecycle

        protected override void OnAttachedTo(VisualElement bindable)
        {
            base.OnAttachedTo(bindable);
            _associatedElement = bindable;

            // 🎯 AUTO-DETECT: Detecta automaticamente o tipo de evento baseado no elemento
            AttachToAppropriateEvent();

            System.Diagnostics.Debug.WriteLine($"✅ SafeNavigationBehavior anexado a {bindable.GetType().Name}");
        }

        protected override void OnDetachingFrom(VisualElement bindable)
        {
            DetachFromEvents();
            _associatedElement = null;
            base.OnDetachingFrom(bindable);
        }

        #endregion

        #region Event Attachment - AUTO-DETECT (PRESERVADO)

        /// <summary>
        /// 🎯 INTELIGENTE: Detecta automaticamente o tipo de elemento e anexa ao evento correto
        /// </summary>
        private void AttachToAppropriateEvent()
        {
            switch (_associatedElement)
            {
                case Button button:
                    button.Clicked += OnElementActivated;
                    break;

                case Frame frame:
                    AttachTapGestureToFrame(frame);
                    break;

                default:
                    if (_associatedElement is MauiView view)
                    {
                        AttachTapGestureToView(view);
                    }
                    break;
            }
        }

        private void AttachTapGestureToFrame(Frame frame)
        {
            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += OnElementActivated;
            frame.GestureRecognizers.Add(tapGesture);
        }

        private void AttachTapGestureToView(MauiView view)
        {
            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += OnElementActivated;
            view.GestureRecognizers.Add(tapGesture);
        }

        private void DetachFromEvents()
        {
            switch (_associatedElement)
            {
                case Button button:
                    button.Clicked -= OnElementActivated;
                    break;
            }
        }

        #endregion

        #region Safe Navigation Logic

        /// <summary>
        /// 🛡️ PROTEÇÃO: Método principal que executa navegação segura
        /// </summary>
        private async void OnElementActivated(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🎯 SafeNavigationBehavior: Navegação solicitada");

                // 🛡️ PROTEÇÃO: Verifica debounce
                if (TargetPageType != null && !_instanceManager.CanNavigateToPage(TargetPageType, DebounceMilliseconds))
                {
                    System.Diagnostics.Debug.WriteLine($"🚫 SafeNavigationBehavior: Navegação BLOQUEADA por debounce");
                    return;
                }

                // 🎯 PRIORIDADE 1: Comando customizado
                if (NavigationCommand != null && NavigationCommand.CanExecute(null))
                {
                    System.Diagnostics.Debug.WriteLine($"🎯 SafeNavigationBehavior: Executando comando customizado");
                    NavigationCommand.Execute(null);
                    return;
                }

                // 🎯 PRIORIDADE 2: Navegação para página específica configurada
                if (TargetPageType != null)
                {
                    await ExecuteTargetPageNavigationAsync();
                    return;
                }

                // 🧠 PRIORIDADE 3: Navegação inteligente por stack (NOVA FUNCIONALIDADE)
                if (EnableSmartStackNavigation)
                {
                    await ExecuteSmartStackNavigationAsync();
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"⚠️ SafeNavigationBehavior: Nenhuma estratégia de navegação aplicável");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SafeNavigationBehavior: Erro na navegação: {ex.Message}");
            }
        }

        /// <summary>
        /// 🎯 ESPECÍFICO: Navegação para página específica configurada (LÓGICA ORIGINAL PRESERVADA)
        /// </summary>
        private async Task ExecuteTargetPageNavigationAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🎯 SafeNavigationBehavior: Navegando para {TargetPageType.Name}");

                // 🏗️ CRIAÇÃO: Usa função customizada ou construtor padrão
                ContentPage targetPage;
                if (CreatePageFunc != null)
                {
                    targetPage = CreatePageFunc();
                }
                else
                {
                    targetPage = (ContentPage)Activator.CreateInstance(TargetPageType);
                }

                // 🎯 MARCAÇÃO: Marca página para bypass de behaviors problemáticos se necessário
                MarkPageForSpecialHandling(targetPage);

                // 🚀 NAVEGAÇÃO: Executa navegação
                await ExecuteSafeNavigation(targetPage);

                System.Diagnostics.Debug.WriteLine($"✅ SafeNavigationBehavior: Navegação concluída para {TargetPageType.Name}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SafeNavigationBehavior: Erro na navegação específica: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 🧠 NOVA: Navegação inteligente baseada no stack de navegação
        /// </summary>
        private async Task ExecuteSmartStackNavigationAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🧠 SafeNavigationBehavior: Executando navegação inteligente por stack");

                var currentPage = GetCurrentPage();
                if (currentPage == null)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ SafeNavigationBehavior: Página atual não encontrada");
                    return;
                }

                var navigation = currentPage.Navigation;
                var navigationStack = navigation?.NavigationStack;

                if (navigationStack == null || navigationStack.Count <= 1)
                {
                    System.Diagnostics.Debug.WriteLine($"🚪 SafeNavigationBehavior: Sem stack de navegação - não há para onde voltar");
                    return;
                }

                // 🔍 ANÁLISE: Encontra posição da página atual no stack
                var currentPageIndex = FindCurrentPageIndexInStack(navigationStack, currentPage);
                if (currentPageIndex <= 0)
                {
                    System.Diagnostics.Debug.WriteLine($"🎯 SafeNavigationBehavior: Página atual é a primeira - fazendo PopAsync simples");
                    await navigation.PopAsync();
                    return;
                }

                // 🎯 INTELIGENTE: Volta para página anterior
                var previousPage = navigationStack[currentPageIndex - 1];
                System.Diagnostics.Debug.WriteLine($"🎯 SafeNavigationBehavior: Voltando para {previousPage.GetType().Name}");

                // 🚀 EXECUÇÃO: Remove página atual do stack
                await RemoveCurrentPageFromStackAsync(navigation, currentPageIndex, navigationStack.Count);

                System.Diagnostics.Debug.WriteLine($"✅ SafeNavigationBehavior: Navegação inteligente concluída");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SafeNavigationBehavior: Erro na navegação inteligente: {ex.Message}");

                // 🛡️ FALLBACK: PopAsync simples
                try
                {
                    var currentPage = GetCurrentPage();
                    if (currentPage?.Navigation != null)
                    {
                        await currentPage.Navigation.PopAsync();
                    }
                }
                catch (Exception fallbackEx)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ SafeNavigationBehavior: Erro no fallback: {fallbackEx.Message}");
                }
            }
        }

        /// <summary>
        /// 🔍 HELPER: Encontra o índice da página atual no stack
        /// </summary>
        private int FindCurrentPageIndexInStack(IReadOnlyList<Page> navigationStack, ContentPage currentPage)
        {
            try
            {
                // 🎯 BUSCA POR REFERÊNCIA: Mais confiável
                for (int i = 0; i < navigationStack.Count; i++)
                {
                    if (ReferenceEquals(navigationStack[i], currentPage))
                    {
                        return i;
                    }
                }

                // 🎯 BUSCA POR TIPO: Fallback - assume que é a última do mesmo tipo
                for (int i = navigationStack.Count - 1; i >= 0; i--)
                {
                    if (navigationStack[i].GetType() == currentPage.GetType())
                    {
                        return i;
                    }
                }

                // 🎯 ÚLTIMO RECURSO: Assume que é a última página
                return navigationStack.Count - 1;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SafeNavigationBehavior: Erro ao encontrar página no stack: {ex.Message}");
                return navigationStack.Count - 1;
            }
        }

        /// <summary>
        /// 🗑️ LIMPEZA: Remove página atual do stack de forma inteligente
        /// </summary>
        private async Task RemoveCurrentPageFromStackAsync(INavigation navigation, int currentPageIndex, int stackCount)
        {
            try
            {
                // Se a página atual é a última no stack, simplesmente faz PopAsync
                if (currentPageIndex == stackCount - 1)
                {
                    await navigation.PopAsync();
                    return;
                }

                // Se há páginas intermediárias após a atual, remove todas até voltar à anterior
                var pagesToRemove = stackCount - currentPageIndex;
                System.Diagnostics.Debug.WriteLine($"🗑️ SafeNavigationBehavior: Removendo {pagesToRemove} páginas do stack");

                for (int i = 0; i < pagesToRemove; i++)
                {
                    await navigation.PopAsync(false); // false = sem animação para ser mais rápido
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SafeNavigationBehavior: Erro ao remover páginas do stack: {ex.Message}");
                // Fallback: PopAsync simples
                await navigation.PopAsync();
            }
        }

        /// <summary>
        /// 🎯 MARCAÇÃO: Marca página para tratamento especial se necessário (PRESERVADO)
        /// ✅ CORREÇÃO: Não marca SpotPage para bypass quando é navegação de volta
        /// </summary>
        private void MarkPageForSpecialHandling(ContentPage page)
        {
            // ✅ CRÍTICO: Apenas marca para bypass quando é navegação PARA FRENTE
            // Navegação de volta (stack navigation) não deve marcar para bypass
            if (page is SpotPage && !EnableSmartStackNavigation)
            {
                page.StyleId = "BYPASS_PAGELIFECYCLE";
                System.Diagnostics.Debug.WriteLine($"🎯 SafeNavigationBehavior: SpotPage marcada para bypass");
            }
        }

        /// <summary>
        /// 🚀 NAVEGAÇÃO: Executa navegação segura com fallbacks (PRESERVADO)
        /// </summary>
        private async Task ExecuteSafeNavigation(ContentPage targetPage)
        {
            try
            {
                // 🎯 ESTRATÉGIA 1: Tenta via página atual
                var currentPage = GetCurrentPage();
                if (currentPage?.Navigation != null)
                {
                    await currentPage.Navigation.PushAsync(targetPage);
                    return;
                }

                // 🎯 ESTRATÉGIA 2: Tenta via Shell (se disponível)
                if (Shell.Current != null)
                {
                    await Shell.Current.Navigation.PushAsync(targetPage);
                    return;
                }

                // 🎯 ESTRATÉGIA 3: Tenta via Application.MainPage
                if (Application.Current?.MainPage is NavigationPage navPage)
                {
                    await navPage.PushAsync(targetPage);
                    return;
                }

                System.Diagnostics.Debug.WriteLine("❌ SafeNavigationBehavior: Nenhuma estratégia de navegação funcionou");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SafeNavigationBehavior: Erro ao executar navegação: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 🎯 HELPER: Obtém a página atual de forma robusta (PRESERVADO)
        /// </summary>
        private ContentPage GetCurrentPage()
        {
            try
            {
                var mainPage = Application.Current?.MainPage;

                if (mainPage is NavigationPage navPage && navPage.CurrentPage is ContentPage currentContentPage)
                {
                    return currentContentPage;
                }

                if (mainPage is ContentPage directContentPage)
                {
                    return directContentPage;
                }

                if (Shell.Current?.CurrentPage is ContentPage shellContentPage)
                {
                    return shellContentPage;
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SafeNavigationBehavior: Erro ao obter página atual: {ex.Message}");
                return null;
            }
        }

        #endregion

        #region Public Methods for Manual Usage (PRESERVADO)

        /// <summary>
        /// 🎯 PÚBLICO: Permite executar navegação manualmente
        /// </summary>
        public async Task NavigateToPageAsync()
        {
            OnElementActivated(this, EventArgs.Empty);
            await Task.CompletedTask;
        }

        #endregion
    }
}