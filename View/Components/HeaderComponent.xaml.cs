using MyVocaList.View.Behaviors;
using Serilog;
using System.Windows.Input;
using MauiView = Microsoft.Maui.Controls.View;

namespace MyVocaList.View.Components
{
    public partial class HeaderComponent : ContentView
    {
        #region private Props
        private static readonly ILogger Logger = Log.ForContext<HeaderComponent>();
        #endregion
        #region Bindable Properties

        public static readonly BindableProperty TitleProperty =
            BindableProperty.Create(nameof(Title), typeof(string), typeof(HeaderComponent), string.Empty);

        public static readonly BindableProperty BackCommandProperty =
            BindableProperty.Create(nameof(BackCommand), typeof(ICommand), typeof(HeaderComponent), null);

        public static readonly BindableProperty ShowCancelButtonProperty =
            BindableProperty.Create(nameof(ShowCancelButton), typeof(bool), typeof(HeaderComponent), false,
            propertyChanged: OnFormModePropertiesChanged);

        public static readonly BindableProperty ShowSaveButtonProperty =
            BindableProperty.Create(nameof(ShowSaveButton), typeof(bool), typeof(HeaderComponent), false,
            propertyChanged: OnFormModePropertiesChanged);

        public static readonly BindableProperty UseCancelIconProperty =
            BindableProperty.Create(nameof(UseCancelIcon), typeof(bool), typeof(HeaderComponent), false,
            propertyChanged: OnFormModePropertiesChanged);

        public static readonly BindableProperty UseSaveIconProperty =
            BindableProperty.Create(nameof(UseSaveIcon), typeof(bool), typeof(HeaderComponent), false,
            propertyChanged: OnFormModePropertiesChanged);

        public static readonly BindableProperty ExitAppProperty =
            BindableProperty.Create(nameof(ExitApp), typeof(bool), typeof(HeaderComponent), false);

        #endregion

        #region Public Properties

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

        public bool ShowCancelButton
        {
            get => (bool)GetValue(ShowCancelButtonProperty);
            set => SetValue(ShowCancelButtonProperty, value);
        }

        public bool ShowSaveButton
        {
            get => (bool)GetValue(ShowSaveButtonProperty);
            set => SetValue(ShowSaveButtonProperty, value);
        }

        public bool UseCancelIcon
        {
            get => (bool)GetValue(UseCancelIconProperty);
            set => SetValue(UseCancelIconProperty, value);
        }

        public bool UseSaveIcon
        {
            get => (bool)GetValue(UseSaveIconProperty);
            set => SetValue(UseSaveIconProperty, value);
        }

        public bool ExitApp
        {
            get => (bool)GetValue(ExitAppProperty);
            set => SetValue(ExitAppProperty, value);
        }

        #endregion

        #region Events

        public event EventHandler BackButtonClicked;
        public event EventHandler CancelClicked;
        public event EventHandler SaveClicked;

        #endregion

        public HeaderComponent()
        {
            InitializeComponent();
        }

        #region Property Changed Handlers

        private static void OnFormModePropertiesChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is HeaderComponent header)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    header.UpdateHeaderMode();
                });
            }
        }

        private void UpdateHeaderMode()
        {
            bool isFormMode = ShowCancelButton || ShowSaveButton;

            if (isFormMode)
            {
                // FORM MODE
                backArrowImage.IsVisible = false;

                // Cancel Button: Icon OR Text
                if (ShowCancelButton)
                {
                    cancelIconImage.IsVisible = UseCancelIcon;
                    cancelTextLabel.IsVisible = !UseCancelIcon;
                }
                else
                {
                    cancelIconImage.IsVisible = false;
                    cancelTextLabel.IsVisible = false;
                }

                // Save Button: Icon OR Text
                if (ShowSaveButton)
                {
                    saveIconImage.IsVisible = UseSaveIcon;
                    saveTextLabel.IsVisible = !UseSaveIcon;
                    rightButtonStack.IsVisible = true;
                    rightSpacer.IsVisible = false;
                }
                else
                {
                    saveIconImage.IsVisible = false;
                    saveTextLabel.IsVisible = false;
                    rightButtonStack.IsVisible = false;
                    rightSpacer.IsVisible = true;
                }
            }
            else
            {
                // LIST MODE
                backArrowImage.IsVisible = true;
                cancelIconImage.IsVisible = false;
                cancelTextLabel.IsVisible = false;

                rightButtonStack.IsVisible = false;
                saveIconImage.IsVisible = false;
                saveTextLabel.IsVisible = false;
                rightSpacer.IsVisible = true;
            }
        }

        #endregion

        #region Event Handlers

        private async void OnLeftButtonClicked(object sender, EventArgs e)
        {
            try
            { 
                if (ShowCancelButton)
                {
                    CancelClicked?.Invoke(this, EventArgs.Empty);
                    return;
                }

                if (BackCommand != null && BackCommand.CanExecute(null))
                {
                    BackCommand.Execute(null);
                    return;
                }

                if (BackButtonClicked != null)
                {
                    BackButtonClicked.Invoke(this, EventArgs.Empty);
                    return;
                }

                if (ExitApp)
                {
                    await ExitApplicationAsync();
                    return;
                }

                await DelegateToSafeNavigationBehaviorAsync();
            }
            catch (Exception ex)
            {
                await HandleSpecialCaseNavigationAsync();
                throw;
            }
        }

        private void OnSaveButtonClicked(object sender, EventArgs e) => SaveClicked?.Invoke(this, EventArgs.Empty);

        #endregion

        #region Navigation Logic

        private async Task DelegateToSafeNavigationBehaviorAsync()
        {
            try
            {
                var currentPage = GetCurrentPage();
                if (currentPage == null)
                {
                    await HandleSpecialCaseNavigationAsync();
                    return;
                }

                var backBehavior = FindBackNavigationBehavior(currentPage);
                if (backBehavior != null)
                {
                    await backBehavior.NavigateToPageAsync();
                    return;
                }

                await HandleSpecialCaseNavigationAsync();
            }
            catch (Exception ex)
            {
                await HandleSpecialCaseNavigationAsync();
                throw;
            }
        }

        private SafeNavigationBehavior FindBackNavigationBehavior(ContentPage currentPage)
        {
            var behaviors = currentPage.Behaviors?.OfType<SafeNavigationBehavior>();
            if (behaviors == null || !behaviors.Any())
            {
                return null;
            }

            var namedBackBehavior = currentPage.FindByName<SafeNavigationBehavior>("BackNavigationBehavior");
            if (namedBackBehavior != null)
            {
                return namedBackBehavior;
            }

            var behaviorsList = behaviors.ToList();
            if (behaviorsList.Count == 1)
            {
                return behaviorsList[0];
            }

            var nonFormBehavior = behaviorsList.FirstOrDefault(b =>
                b.TargetPageType != null && !b.TargetPageType.Name.Contains("Form"));
            if (nonFormBehavior != null)
            {
                return nonFormBehavior;
            }

            return behaviorsList.FirstOrDefault();
    }

        private async Task HandleSpecialCaseNavigationAsync()
        {
            try
            {
                var currentPage = GetCurrentPage();
                if (currentPage == null)
                {
                    return;
                }

                if (currentPage.Navigation?.NavigationStack?.Count > 1)
                {
                    await currentPage.Navigation.PopAsync();
                }
                else
                {
                    await ExitApplicationAsync();
                }
            }
            catch (Exception ex)
            {
                await ExitApplicationAsync();
                throw;
            }
        }

        private async Task ExitApplicationAsync()
        {
           await Task.Delay(100);
           Application.Current?.Quit();
        }

        private ContentPage GetCurrentPage()
        {
            var element = this.Parent;
            while (element != null)
            {
                if (element is ContentPage page)
                    return page;
                element = element.Parent;
            }

            if (Application.Current?.MainPage is NavigationPage navPage)
            {
                return navPage.CurrentPage as ContentPage;
            }

            if (Application.Current?.MainPage is ContentPage mainPage)
                return mainPage;

            if (Application.Current?.MainPage?.Navigation?.NavigationStack?.LastOrDefault() is ContentPage lastPage)
                return lastPage;

            return null;
        }

        #endregion

        #region Legacy Methods

        public void ConfigureSafeBackNavigation(Type? targetPageType = null, int debounceMs = 500)
        {
            var safeBehavior = new SafeNavigationBehavior
            {
                EnableSmartStackNavigation = true,
                DebounceMilliseconds = debounceMs
            };
            var logText = $"HeaderComponent: SafeNavigationBehavior configured for smart navigation";

            var backButton = FindBackButton();
            if (backButton != null && targetPageType != null)
            {
                safeBehavior.TargetPageType = targetPageType;
                logText = $"HeaderComponent: SafeNavigationBehavior configured for {targetPageType.Name}";
            }

            if (backButton != null)
            {
                backButton.Behaviors.Add(safeBehavior);
                System.Diagnostics.Debug.WriteLine(logText);
            }
        }

        private VisualElement FindBackButton() => FindBackButtonInContent(this.Content);
            
        private VisualElement FindBackButtonInContent(MauiView content)
        {
            if (content == null) return null;

            if (content is Button button && (button.Text == "←" || button.Text == "Voltar"))
            {
                return button;
            }

            if (content is Image image && image.Source?.ToString().Contains("setaesquerda") == true)
            {
                return image;
            }

            if (content is StackLayout stackLayout &&
                stackLayout.GestureRecognizers?.Any(g => g is TapGestureRecognizer) == true)
            {
                if (stackLayout.Children?.Any(c => c is Image img &&
                    img.Source?.ToString().Contains("setaesquerda") == true) == true)
                {
                    return stackLayout;
                }
            }

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

            if (content is ContentView contentView && contentView.Content != null)
            {
                return FindBackButtonInContent(contentView.Content);
            }

            return null;
        }

        #endregion
    }
}