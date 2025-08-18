using MyKaraoke.View.Managers;
using System.Windows.Input;
using MauiView = Microsoft.Maui.Controls.View;

namespace MyKaraoke.View.Behaviors
{
    /// <summary>
    /// ✅ BEHAVIOR ELEGANTE: Previne navegação duplicada automaticamente
    /// 🛡️ REUTILIZÁVEL: Pode ser aplicado a qualquer elemento que navega
    /// 🎯 CONFIGURÁVEL: Via propriedades bindáveis
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

        #region Event Attachment - AUTO-DETECT

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

                // 🎯 EXTENSÍVEL: Adicione mais tipos conforme necessário
                case Frame frame:
                    AttachTapGestureToFrame(frame);
                    break;

                default:
                    // 🛡️ FALLBACK: Tenta adicionar TapGestureRecognizer se for View
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

                    // Para Frame e View com TapGesture adicionado dinamicamente, 
                    // a limpeza acontece automaticamente quando o elemento é destruído
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
                System.Diagnostics.Debug.WriteLine($"🎯 SafeNavigationBehavior: Navegação solicitada para {TargetPageType?.Name}");

                // 🛡️ VALIDAÇÃO 1: Verifica se tipo da página foi configurado
                if (TargetPageType == null)
                {
                    System.Diagnostics.Debug.WriteLine("❌ SafeNavigationBehavior: TargetPageType não configurado");
                    return;
                }

                // 🛡️ PROTEÇÃO 2: Verifica debounce via PageInstanceManager
                if (!_instanceManager.CanNavigateToPage(TargetPageType, DebounceMilliseconds))
                {
                    System.Diagnostics.Debug.WriteLine($"🚫 SafeNavigationBehavior: Navegação BLOQUEADA por debounce");
                    return;
                }

                // 🎯 EXECUÇÃO: Comando customizado tem prioridade
                if (NavigationCommand != null && NavigationCommand.CanExecute(null))
                {
                    System.Diagnostics.Debug.WriteLine($"🎯 SafeNavigationBehavior: Executando comando customizado");
                    NavigationCommand.Execute(null);
                    return;
                }

                // 🏗️ CRIAÇÃO: Usa função customizada ou construtor padrão
                ContentPage targetPage;
                if (CreatePageFunc != null)
                {
                    System.Diagnostics.Debug.WriteLine($"🏗️ SafeNavigationBehavior: Criando página via função customizada");
                    targetPage = CreatePageFunc();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"🏗️ SafeNavigationBehavior: Criando página via construtor padrão");
                    targetPage = (ContentPage)Activator.CreateInstance(TargetPageType);
                }

                // 🎯 MARCAÇÃO: Marca página para bypass de behaviors problemáticos se necessário
                MarkPageForSpecialHandling(targetPage);

                // 🚀 NAVEGAÇÃO: Executa navegação via Shell ou Navigation
                await ExecuteSafeNavigation(targetPage);

                System.Diagnostics.Debug.WriteLine($"✅ SafeNavigationBehavior: Navegação concluída para {TargetPageType.Name}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SafeNavigationBehavior: Erro na navegação: {ex.Message}");
            }
        }

        /// <summary>
        /// 🎯 MARCAÇÃO: Marca página para tratamento especial se necessário
        /// </summary>
        private void MarkPageForSpecialHandling(ContentPage page)
        {
            // 🎯 ESPECÍFICO: Para SpotPage, marca para bypass do PageLifecycleBehavior
            if (page is SpotPage)
            {
                page.StyleId = "BYPASS_PAGELIFECYCLE";
                System.Diagnostics.Debug.WriteLine($"🎯 SafeNavigationBehavior: SpotPage marcada para bypass");
            }

            // 🎯 EXTENSÍVEL: Adicione outras páginas conforme necessário
            // if (page is PersonPage personPage) { ... }
        }

        /// <summary>
        /// 🚀 NAVEGAÇÃO: Executa navegação segura com fallbacks
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
        /// 🎯 HELPER: Obtém a página atual de forma robusta
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

        #region Public Methods for Manual Usage

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