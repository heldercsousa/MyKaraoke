using MyVocaList.View.Behaviors;
using Serilog;

namespace MyVocaList.View.Components
{
    /// <summary>
    /// A smart icon component that automatically handles filled/outlined states,
    /// colors, and context-aware sizing.
    /// </summary>
    public partial class StatefulIcon : ContentView
    {
        private static readonly ILogger Logger = Log.ForContext<StatefulIcon>();

        private bool _isSizeSetByUser = false;

        #region Bindable Properties

        public static readonly BindableProperty IconNameProperty =
            BindableProperty.Create(nameof(IconName), typeof(string), typeof(StatefulIcon), propertyChanged: OnStateChanged);

        public static readonly BindableProperty IsSelectedProperty =
            BindableProperty.Create(nameof(IsSelected), typeof(bool), typeof(StatefulIcon), false, propertyChanged: OnStateChanged);

        public static readonly BindableProperty ActiveColorProperty =
            BindableProperty.Create(nameof(ActiveColor), typeof(Color), typeof(StatefulIcon),
            defaultValueCreator: bindable => (Color)Application.Current.Resources["OnBackground"],
            propertyChanged: OnStateChanged);

        public static readonly BindableProperty InactiveColorProperty =
            BindableProperty.Create(nameof(InactiveColor), typeof(Color), typeof(StatefulIcon),
            defaultValueCreator: bindable => (Color)Application.Current.Resources["OnSurfaceVariant"],
            propertyChanged: OnStateChanged); 

        public static readonly BindableProperty SizeProperty =
            BindableProperty.Create(nameof(Size), typeof(StatefulIconSize), typeof(StatefulIcon),
                                    StatefulIconSize.Medium,
                                    propertyChanged: OnSizeChanged);
        #endregion

        #region Public Properties
        public string IconName
        {
            get => (string)GetValue(IconNameProperty);
            set => SetValue(IconNameProperty, value);
        }

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public Color ActiveColor
        {
            get => (Color)GetValue(ActiveColorProperty);
            set => SetValue(ActiveColorProperty, value);
        }

        public Color InactiveColor
        {
            get => (Color)GetValue(InactiveColorProperty);
            set => SetValue(InactiveColorProperty, value);
        }

        public StatefulIconSize Size
        {
            get => (StatefulIconSize)GetValue(SizeProperty);
            set => SetValue(SizeProperty, value);
        }
        #endregion

        public StatefulIcon()
        {
            InitializeComponent();
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();
            Logger.Debug("OnParentSet called - IconName: {IconName}, Parent: {ParentType}", IconName, Parent?.GetType().Name);
            DetectAndApplyContextualSize();
        }

        private static void OnStateChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is StatefulIcon statefulIcon)
            {
                // If IconName changed from null/empty to a value, re-run size detection
                // This handles cases where IconName is set via binding (delayed initialization)
                if (string.IsNullOrEmpty(oldValue as string) && !string.IsNullOrEmpty(newValue as string))
                {
                    Logger.Debug("IconName changed from empty to {NewValue} - Re-running size detection", newValue);
                    statefulIcon.DetectAndApplyContextualSize();
                }

                statefulIcon.UpdateIconState();
            }
        }

        private static void OnSizeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is StatefulIcon statefulIcon)
            {
                var oldSize = oldValue as StatefulIconSize;
                var newSize = newValue as StatefulIconSize;

                Logger.Debug("OnSizeChanged - IconName: {IconName}, Old: {OldSize}, New: {NewSize}, Default: {DefaultSize}",
                    statefulIcon.IconName, oldSize?.Name ?? "null", newSize?.Name ?? "null", (SizeProperty.DefaultValue as StatefulIconSize)?.Name);

                if (newValue != SizeProperty.DefaultValue)
                {
                    statefulIcon._isSizeSetByUser = true;
                    Logger.Debug("Size set by user for {IconName} - Auto-detection will be disabled", statefulIcon.IconName);
                }
                statefulIcon.UpdateIconDimensions();
            }
        }

        private void UpdateIconState()
        {
            if (string.IsNullOrEmpty(IconName)) return;

            string source = IsSelected ? $"{IconName}_filled" : $"{IconName}_outlined";
            Color tintColor = IsSelected ? ActiveColor : InactiveColor;

            Logger.Debug("Setting source: {Source} for {IconName} (Selected={IsSelected})", source, IconName, IsSelected);

            TheIcon.Source = source;

            // Set tint color via the IconTintColorBehavior
            if (tintBehavior != null)
            {
                tintBehavior.TintColor = tintColor;
            }
        }

        private void UpdateIconDimensions()
        {
            double dimension = Size?.Dimension ?? StatefulIconSize.Medium.Dimension;

            // Set size on both the ContentView (this) and the inner Image
            this.WidthRequest = dimension;
            this.HeightRequest = dimension;
            TheIcon.WidthRequest = dimension;
            TheIcon.HeightRequest = dimension;

            Logger.Debug("UpdateIconDimensions - IconName: {IconName}, Size: {SizeName}, Dimension: {Dimension}dp", IconName, Size?.Name ?? "null", dimension);
        }

        private void DetectAndApplyContextualSize()
        {
            if (_isSizeSetByUser)
            {
                Logger.Debug("Skipping auto-detection for {IconName} - Size was set by user", IconName);
                return;
            }

            Element currentParent = this.Parent;
            int depth = 0;

            Logger.Debug("DetectAndApplyContextualSize - IconName: {IconName}, Starting parent: {ParentType}", IconName, currentParent?.GetType().Name);

            while (currentParent != null && depth < 10)
            {
                Logger.Debug("Depth {Depth}: Checking parent {ParentType}", depth, currentParent.GetType().Name);

                if (currentParent is HeaderComponent)
                {
                    Logger.Debug("HeaderComponent found at depth {Depth} - Setting Medium size for icon: {IconName}", depth, IconName);
                    this.Size = StatefulIconSize.Medium;
                    UpdateIconDimensions(); // Force dimension update
                    return;
                }
                if (currentParent is Grid grid && grid.Behaviors.OfType<NavBarBehavior>().Any())
                {
                    Logger.Debug("NavBar Grid found at depth {Depth} - Setting Medium size for icon: {IconName}", depth, IconName);
                    this.Size = StatefulIconSize.Medium;
                    UpdateIconDimensions(); // Force dimension update
                    return;
                }
                if (currentParent is ContentPage page && page.Behaviors.OfType<SmartPageLifecycleBehavior>().Any())
                {
                    Logger.Debug("ContentPage found at depth {Depth} - Setting Large size for icon: {IconName}", depth, IconName);
                    this.Size = StatefulIconSize.Large;
                    UpdateIconDimensions(); // Force dimension update
                    return;
                }
                currentParent = currentParent.Parent;
                depth++;
            }

            Logger.Debug("No context detected for icon: {IconName} - Using default Medium size", IconName);
            UpdateIconDimensions(); // Force dimension update even for default size
        }
    }
}
