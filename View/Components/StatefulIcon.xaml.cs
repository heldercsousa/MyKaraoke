using MyVocaList.View.Behaviors;

namespace MyVocaList.View.Components
{
    /// <summary>
    /// A smart icon component that automatically handles filled/outlined states,
    /// colors, and context-aware sizing.
    /// </summary>
    public partial class StatefulIcon : ContentView
    {
        private bool _isSizeSetByUser = false;

        #region Bindable Properties

        public static readonly BindableProperty IconNameProperty =
            BindableProperty.Create(nameof(IconName), typeof(string), typeof(StatefulIcon), propertyChanged: OnStateChanged);

        public static readonly BindableProperty IsSelectedProperty =
            BindableProperty.Create(nameof(IsSelected), typeof(bool), typeof(StatefulIcon), false, propertyChanged: OnStateChanged);

        public static readonly BindableProperty ActiveColorProperty =
            BindableProperty.Create(nameof(ActiveColor), typeof(Color), typeof(StatefulIcon),
            defaultValueCreator: bindable => (Color)Application.Current.Resources["OnBackground"]);

        public static readonly BindableProperty InactiveColorProperty =
            BindableProperty.Create(nameof(InactiveColor), typeof(Color), typeof(StatefulIcon),
            defaultValueCreator: bindable => (Color)Application.Current.Resources["OnSurfaceVariant"]);

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
            System.Diagnostics.Debug.WriteLine($"[StatefulIcon] OnParentSet called - IconName: {IconName}, Parent: {Parent?.GetType().Name}");
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
                    System.Diagnostics.Debug.WriteLine($"[StatefulIcon] IconName changed from empty to '{newValue}' - Re-running size detection");
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

                System.Diagnostics.Debug.WriteLine($"[StatefulIcon] OnSizeChanged - IconName: '{statefulIcon.IconName}', Old: {oldSize?.Name ?? "null"}, New: {newSize?.Name ?? "null"}, Default: {(SizeProperty.DefaultValue as StatefulIconSize)?.Name}");

                if (newValue != SizeProperty.DefaultValue)
                {
                    statefulIcon._isSizeSetByUser = true;
                    System.Diagnostics.Debug.WriteLine($"[StatefulIcon] Size set by user for '{statefulIcon.IconName}' - Auto-detection will be disabled");
                }
                statefulIcon.UpdateIconDimensions();
            }
        }

        private void UpdateIconState()
        {
            if (string.IsNullOrEmpty(IconName)) return;
            string source = IsSelected ? $"{IconName}_filled.svg" : $"{IconName}_outlined.svg";
            Color tintColor = IsSelected ? ActiveColor : InactiveColor;

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

            System.Diagnostics.Debug.WriteLine($"[StatefulIcon] UpdateIconDimensions - IconName: {IconName}, Size: {Size?.Name ?? "null"}, Dimension: {dimension}dp");
        }

        private void DetectAndApplyContextualSize()
        {
            if (_isSizeSetByUser)
            {
                System.Diagnostics.Debug.WriteLine($"[StatefulIcon] ⚠️ Skipping auto-detection for '{IconName}' - Size was set by user");
                return;
            }

            Element currentParent = this.Parent;
            int depth = 0;

            System.Diagnostics.Debug.WriteLine($"[StatefulIcon] DetectAndApplyContextualSize - IconName: '{IconName}', Starting parent: {currentParent?.GetType().Name}");

            while (currentParent != null && depth < 10)
            {
                System.Diagnostics.Debug.WriteLine($"[StatefulIcon] Depth {depth}: Checking parent {currentParent.GetType().Name}");

                if (currentParent is HeaderComponent)
                {
                    System.Diagnostics.Debug.WriteLine($"[StatefulIcon] ✅ HeaderComponent found at depth {depth} - Setting Medium size for icon: {IconName}");
                    this.Size = StatefulIconSize.Medium;
                    UpdateIconDimensions(); // Force dimension update
                    return;
                }
                if (currentParent is Grid grid && grid.Behaviors.OfType<NavBarBehavior>().Any())
                {
                    System.Diagnostics.Debug.WriteLine($"[StatefulIcon] ✅ NavBar Grid found at depth {depth} - Setting Medium size for icon: {IconName}");
                    this.Size = StatefulIconSize.Medium;
                    UpdateIconDimensions(); // Force dimension update
                    return;
                }
                if (currentParent is ContentPage page && page.Behaviors.OfType<SmartPageLifecycleBehavior>().Any())
                {
                    System.Diagnostics.Debug.WriteLine($"[StatefulIcon] ✅ ContentPage found at depth {depth} - Setting Large size for icon: {IconName}");
                    this.Size = StatefulIconSize.Large;
                    UpdateIconDimensions(); // Force dimension update
                    return;
                }
                currentParent = currentParent.Parent;
                depth++;
            }

            System.Diagnostics.Debug.WriteLine($"[StatefulIcon] ⚠️ No context detected for icon: {IconName} - Using default Medium size");
            UpdateIconDimensions(); // Force dimension update even for default size
        }
    }
}
