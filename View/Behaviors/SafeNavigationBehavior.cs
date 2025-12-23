using System.Windows.Input;
using MauiView = Microsoft.Maui.Controls.View;

namespace MyVocaList.View.Behaviors
{
    /// <summary>
    /// ✅ BEHAVIOR INTELIGENTE: Previne navegação duplicada + navegação inteligente por stack
    /// 🛡️ REUTILIZÁVEL: Pode ser aplicado a qualquer elemento que navega
    /// 🎯 CONFIGURÁVEL: Via propriedades bindáveis
    /// 🧠 INTELIGENTE: Determina automaticamente para onde navegar baseado no stack
    /// 🧹 LIMPO: Sem PageInstanceManager - usa debounce simples e eficaz
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

        public static readonly BindableProperty EnableSmartStackNavigationProperty =
            BindableProperty.Create(nameof(EnableSmartStackNavigation), typeof(bool), typeof(SafeNavigationBehavior), true);

        #endregion

        #region Properties

        public Type TargetPageType
        {
            get => (Type)GetValue(TargetPageTypeProperty);
            set => SetValue(TargetPageTypeProperty, value);
        }

        public ICommand NavigationCommand
        {
            get => (ICommand)GetValue(NavigationCommandProperty);
            set => SetValue(NavigationCommandProperty, value);
        }

        public int DebounceMilliseconds
        {
            get => (int)GetValue(DebounceMillisecondsProperty);
            set => SetValue(DebounceMillisecondsProperty, value);
        }

        public Func<ContentPage> CreatePageFunc
        {
            get => (Func<ContentPage>)GetValue(CreatePageFuncProperty);
            set => SetValue(CreatePageFuncProperty, value);
        }

        public bool EnableSmartStackNavigation
        {
            get => (bool)GetValue(EnableSmartStackNavigationProperty);
            set => SetValue(EnableSmartStackNavigationProperty, value);
        }

        #endregion

        #region Private Fields

        private VisualElement _associatedElement;
        private DateTime _lastNavigationTime = DateTime.MinValue;

        #endregion

        #region Behavior Lifecycle

        protected override void OnAttachedTo(VisualElement bindable)
        {
            base.OnAttachedTo(bindable);
            _associatedElement = bindable;
            AttachToAppropriateEvent();
        }

        protected override void OnDetachingFrom(VisualElement bindable)
        {
            DetachFromEvents();
            _associatedElement = null;
            base.OnDetachingFrom(bindable);
        }

        #endregion

        #region Event Attachment

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

        private async void OnElementActivated(object sender, EventArgs e)
        {
            // 🛡️ DEBOUNCE SIMPLES: Proteção contra cliques múltiplos
            var now = DateTime.Now;
            var timeSinceLastNavigation = now - _lastNavigationTime;

            if (timeSinceLastNavigation.TotalMilliseconds < DebounceMilliseconds)
            {
                return;
            }

            _lastNavigationTime = now;

            // 🎯 PRIORIDADE 1: Comando customizado
            if (NavigationCommand != null && NavigationCommand.CanExecute(null))
            {
                NavigationCommand.Execute(null);
                return;
            }

            // 🎯 PRIORIDADE 2: Navegação para página específica
            if (TargetPageType != null)
            {
                await ExecuteTargetPageNavigationAsync();
                return;
            }

            // 🧠 PRIORIDADE 3: Navegação inteligente por stack
            if (EnableSmartStackNavigation)
            {
                await ExecuteSmartStackNavigationAsync();
                return;
            }
        }

        private async Task ExecuteTargetPageNavigationAsync()
        {
            ContentPage targetPage;
            if (CreatePageFunc != null)
            {
                targetPage = CreatePageFunc();
            }
            else
            {
                targetPage = (ContentPage)Activator.CreateInstance(TargetPageType);
            }

            await ExecuteSafeNavigation(targetPage);
        }

        private async Task ExecuteSmartStackNavigationAsync()
        {
            var currentPage = GetCurrentPage();
            if (currentPage == null)
            {
                return;
            }

            var navigation = currentPage.Navigation;
            var navigationStack = navigation?.NavigationStack;

            if (navigationStack == null || navigationStack.Count <= 1)
            {
                return;
            }

            await navigation.PopAsync();
        }

        private async Task ExecuteSafeNavigation(ContentPage targetPage)
        {
            var currentPage = GetCurrentPage();
            if (currentPage?.Navigation != null)
            {
                await currentPage.Navigation.PushAsync(targetPage);
                return;
            }

            if (Shell.Current != null)
            {
                await Shell.Current.Navigation.PushAsync(targetPage);
                return;
            }

            if (Application.Current?.MainPage is NavigationPage navPage)
            {
                await navPage.PushAsync(targetPage);
                return;
            }

        }

        private ContentPage GetCurrentPage()
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

        #endregion

        #region Public Methods
        public async Task NavigateToPageAsync()
        {
            OnElementActivated(this, EventArgs.Empty);
            await Task.CompletedTask;
        }

        #endregion
    }
}