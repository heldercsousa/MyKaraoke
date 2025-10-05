using MyKaraoke.View.Behaviors;
using System.Windows.Input;
using MauiView = Microsoft.Maui.Controls.View;

namespace MyKaraoke.View.Components
{
    public partial class HeaderComponent : ContentView
    {
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
            try
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

                    System.Diagnostics.Debug.WriteLine(
                        $"HeaderComponent: Form Mode - Cancel={ShowCancelButton}(Icon={UseCancelIcon}), Save={ShowSaveButton}(Icon={UseSaveIcon})");
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

                    System.Diagnostics.Debug.WriteLine("HeaderComponent: List Mode activated");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"HeaderComponent: Error updating mode: {ex.Message}");
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
                    System.Diagnostics.Debug.WriteLine("HeaderComponent: Cancel button clicked");
                    CancelClicked?.Invoke(this, EventArgs.Empty);
                    return;
                }

                System.Diagnostics.Debug.WriteLine("HeaderComponent: Back button clicked");

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
                System.Diagnostics.Debug.WriteLine($"HeaderComponent: OnLeftButtonClicked - Error: {ex.Message}");
                await HandleSpecialCaseNavigationAsync();
            }
        }

        private void OnSaveButtonClicked(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("HeaderComponent: Save button clicked");
                SaveClicked?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"HeaderComponent: OnSaveButtonClicked - Error: {ex.Message}");
            }
        }

        #endregion

        #region Navigation Logic

        private async Task DelegateToSafeNavigationBehaviorAsync()
        {
            try
            {
                var currentPage = GetCurrentPage();
                if (currentPage == null)
                {
                    System.Diagnostics.Debug.WriteLine("HeaderComponent: Current page not found");
                    await HandleSpecialCaseNavigationAsync();
                    return;
                }

                var backBehavior = FindBackNavigationBehavior(currentPage);
                if (backBehavior != null)
                {
                    System.Diagnostics.Debug.WriteLine($"HeaderComponent: Delegating to SafeNavigationBehavior");
                    await backBehavior.NavigateToPageAsync();
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"HeaderComponent: No SafeNavigationBehavior found for {currentPage.GetType().Name}");
                await HandleSpecialCaseNavigationAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"HeaderComponent: Delegation error: {ex.Message}");
                await HandleSpecialCaseNavigationAsync();
            }
        }

        private SafeNavigationBehavior FindBackNavigationBehavior(ContentPage currentPage)
        {
            try
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
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"HeaderComponent: Error finding SafeNavigationBehavior: {ex.Message}");
                return null;
            }
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
                    System.Diagnostics.Debug.WriteLine("HeaderComponent: PopAsync fallback executed");
                }
                else
                {
                    await ExitApplicationAsync();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"HeaderComponent: Special case error: {ex.Message}");
                await ExitApplicationAsync();
            }
        }

        private async Task ExitApplicationAsync()
        {
            try
            {
                await Task.Delay(100);
                Application.Current?.Quit();
                System.Diagnostics.Debug.WriteLine("HeaderComponent: Application closed");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"HeaderComponent: Exit error: {ex.Message}");
            }
        }

        private ContentPage GetCurrentPage()
        {
            try
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
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"HeaderComponent: GetCurrentPage - Error: {ex.Message}");
                return null;
            }
        }

        #endregion

        #region Legacy Methods

        public void ConfigureSafeBackNavigation(Type? targetPageType = null, int debounceMs = 500)
        {
            try
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
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"HeaderComponent: Configuration error: {ex.Message}");
            }
        }

        private VisualElement FindBackButton()
        {
            try
            {
                return FindBackButtonInContent(this.Content);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"HeaderComponent: Back button search error: {ex.Message}");
                return null;
            }
        }

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