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
            BindableProperty.Create(nameof(ActiveColor), typeof(Color), typeof(StatefulIcon), Colors.Black, propertyChanged: OnStateChanged);

        public static readonly BindableProperty InactiveColorProperty =
            BindableProperty.Create(nameof(InactiveColor), typeof(Color), typeof(StatefulIcon), Colors.Grey, propertyChanged: OnStateChanged);

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
            DetectAndApplyContextualSize();
        }

        private static void OnStateChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is StatefulIcon statefulIcon)
            {
                statefulIcon.UpdateIconState();
            }
        }

        private static void OnSizeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is StatefulIcon statefulIcon)
            {
                if (newValue != SizeProperty.DefaultValue)
                {
                    statefulIcon._isSizeSetByUser = true;
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
            TheIcon.WidthRequest = dimension;
            TheIcon.HeightRequest = dimension;
        }

        private void DetectAndApplyContextualSize()
        {
            if (_isSizeSetByUser) return;

            Element currentParent = this.Parent;
            int depth = 0;

            while (currentParent != null && depth < 10)
            {
                if (currentParent is HeaderComponent)
                {
                    this.Size = StatefulIconSize.Medium;
                    return;
                }
                if (currentParent is Grid grid && grid.Behaviors.OfType<NavBarBehavior>().Any())
                {
                    this.Size = StatefulIconSize.Medium;
                    return;
                }
                if (currentParent is ContentPage page && page.Behaviors.OfType<SmartPageLifecycleBehavior>().Any())
                {
                    this.Size = StatefulIconSize.Large;
                    return;
                }
                currentParent = currentParent.Parent;
                depth++;
            }
        }
    }
}
