using System.Windows.Input;
using Serilog;

namespace MyVocaList.View.Components
{
    public partial class SearchBarComponent : ContentView
    {
        private static readonly Serilog.ILogger Logger = Log.ForContext<SearchBarComponent>();
        #region Bindable Properties

        public static readonly BindableProperty SearchTextProperty =
            BindableProperty.Create(nameof(SearchText), typeof(string), typeof(SearchBarComponent),
                string.Empty, BindingMode.TwoWay);

        public static readonly BindableProperty PlaceholderProperty =
            BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(SearchBarComponent),
                "Search...");

        public static readonly BindableProperty TextChangedCommandProperty =
            BindableProperty.Create(nameof(TextChangedCommand), typeof(ICommand), typeof(SearchBarComponent));

        public static readonly BindableProperty SearchButtonCommandProperty =
            BindableProperty.Create(nameof(SearchButtonCommand), typeof(ICommand), typeof(SearchBarComponent));

        public static readonly BindableProperty TargetCollectionViewProperty =
            BindableProperty.Create(nameof(TargetCollectionView), typeof(CollectionView), typeof(SearchBarComponent),
                propertyChanged: OnTargetCollectionViewChanged);

        public static readonly BindableProperty EnableScrollBehaviorProperty =
            BindableProperty.Create(nameof(EnableScrollBehavior), typeof(bool), typeof(SearchBarComponent),
                true, propertyChanged: OnEnableScrollBehaviorChanged);

        public static readonly BindableProperty ResultCountTextProperty =
            BindableProperty.Create(nameof(ResultCountText), typeof(string), typeof(SearchBarComponent),
                string.Empty);

        public static readonly BindableProperty ShowResultCountProperty =
            BindableProperty.Create(nameof(ShowResultCount), typeof(bool), typeof(SearchBarComponent),
                false);

        #endregion

        #region Properties

        public string SearchText
        {
            get => (string)GetValue(SearchTextProperty);
            set => SetValue(SearchTextProperty, value);
        }

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        public ICommand TextChangedCommand
        {
            get => (ICommand)GetValue(TextChangedCommandProperty);
            set => SetValue(TextChangedCommandProperty, value);
        }

        public ICommand SearchButtonCommand
        {
            get => (ICommand)GetValue(SearchButtonCommandProperty);
            set => SetValue(SearchButtonCommandProperty, value);
        }

        public CollectionView TargetCollectionView
        {
            get => (CollectionView)GetValue(TargetCollectionViewProperty);
            set => SetValue(TargetCollectionViewProperty, value);
        }

        public bool EnableScrollBehavior
        {
            get => (bool)GetValue(EnableScrollBehaviorProperty);
            set => SetValue(EnableScrollBehaviorProperty, value);
        }

        public string ResultCountText
        {
            get => (string)GetValue(ResultCountTextProperty);
            set => SetValue(ResultCountTextProperty, value);
        }

        public bool ShowResultCount
        {
            get => (bool)GetValue(ShowResultCountProperty);
            set => SetValue(ShowResultCountProperty, value);
        }

        #endregion

        #region Events

        public event EventHandler<TextChangedEventArgs> SearchTextChanged;

        #endregion

        #region Private Fields

        private double _lastScrollY = 0;
        private bool _isSearchBarVisible = true;
        private const double SCROLL_THRESHOLD = 50;
        private const uint ANIMATION_DURATION = 200;

        #endregion

        public SearchBarComponent()
        {
            InitializeComponent();
        }

        #region Event Handlers

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            SearchTextChanged?.Invoke(this, e);

            if (TextChangedCommand?.CanExecute(e.NewTextValue) == true)
            {
                TextChangedCommand.Execute(e.NewTextValue);
            }
        }

        private void OnSearchButtonPressed(object sender, EventArgs e)
        {
            if (sender is SearchBar sb)
            {
                sb.Unfocus();
            }

            if (SearchButtonCommand?.CanExecute(SearchText) == true)
            {
                SearchButtonCommand.Execute(SearchText);
            }
        }

        #endregion

        #region Scroll Behavior

        private static void OnTargetCollectionViewChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is SearchBarComponent component)
            {
                if (oldValue is CollectionView oldCollectionView)
                {
                    oldCollectionView.Scrolled -= component.OnCollectionViewScrolled;
                }

                if (newValue is CollectionView newCollectionView && component.EnableScrollBehavior)
                {
                    newCollectionView.Scrolled += component.OnCollectionViewScrolled;
                }
            }
        }

        private static void OnEnableScrollBehaviorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is SearchBarComponent component && component.TargetCollectionView != null)
            {
                bool enable = (bool)newValue;

                if (enable)
                {
                    component.TargetCollectionView.Scrolled += component.OnCollectionViewScrolled;
                }
                else
                {
                    component.TargetCollectionView.Scrolled -= component.OnCollectionViewScrolled;

                    if (!component._isSearchBarVisible)
                    {
                        component.ShowSearchBarAsync();
                    }
                }
            }
        }

        private void OnCollectionViewScrolled(object sender, ItemsViewScrolledEventArgs e)
        {
            if (!EnableScrollBehavior) return;

            var currentScrollY = e.VerticalOffset;
            var scrollDelta = currentScrollY - _lastScrollY;

            // Scrolling down (hide search bar)
            if (scrollDelta > 5 && _isSearchBarVisible && currentScrollY > SCROLL_THRESHOLD)
            {
                HideSearchBarAsync();
            }
            // Scrolling up (show search bar)
            else if (scrollDelta < -5 && !_isSearchBarVisible)
            {
                ShowSearchBarAsync();
            }

            _lastScrollY = currentScrollY;
        }

        private async void HideSearchBarAsync()
        {
            if (!_isSearchBarVisible) return;

            _isSearchBarVisible = false;

            Logger.Debug("Hiding search bar");

            var totalHeight = searchContainer.Height + searchContainer.Margin.Top + searchContainer.Margin.Bottom;
            await searchContainer.TranslateTo(0, -totalHeight, ANIMATION_DURATION, Easing.CubicOut);
        }

        private async void ShowSearchBarAsync()
        {
            if (_isSearchBarVisible) return;

            _isSearchBarVisible = true;

            Logger.Debug("Showing search bar");

            await searchContainer.TranslateTo(0, 0, ANIMATION_DURATION, Easing.CubicOut);
        }

        #endregion

        #region Public Methods

        public new void Focus()
        {
            searchBar?.Focus();
        }

        public void Clear()
        {
            SearchText = string.Empty;
        }

        public void ForceShow()
        {
            if (!_isSearchBarVisible)
            {
                ShowSearchBarAsync();
            }
        }

        public void ForceHide()
        {
            if (_isSearchBarVisible)
            {
                HideSearchBarAsync();
            }
        }

        #endregion
    }
}
