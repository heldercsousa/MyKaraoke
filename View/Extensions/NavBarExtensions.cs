using MyVocaList.View.Components;
using Serilog;

namespace MyVocaList.View.Extensions
{
    /// <summary>
    /// ✅ SELF-REGISTRATION: Centralized navbar discovery via attached property
    /// 🎯 ZERO CONFIGURATION: NavBars automatically register with their parent page
    /// 🔧 USAGE: Just add navbar to XAML - works automatically!
    /// </summary>
    public static class NavBarExtensions
    {
        private static readonly ILogger Logger = Log.ForContext(typeof(NavBarExtensions));
        /// <summary>
        /// Attached property to store the page's navbar instance
        /// </summary>
        public static readonly BindableProperty PageNavBarProperty =
            BindableProperty.CreateAttached(
                "PageNavBar",
                typeof(IAnimatableNavBar),
                typeof(NavBarExtensions),
                null,
                propertyChanged: OnPageNavBarChanged);

        public static void SetPageNavBar(BindableObject view, IAnimatableNavBar value)
        {
            view.SetValue(PageNavBarProperty, value);
        }

        public static IAnimatableNavBar GetPageNavBar(BindableObject view)
        {
            return (IAnimatableNavBar)view.GetValue(PageNavBarProperty);
        }

        /// <summary>
        /// Called when navbar is registered/unregistered with a page
        /// </summary>
        private static void OnPageNavBarChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is ContentPage page)
            {
                if (newValue is IAnimatableNavBar navBar)
                {
                    Logger.Debug("NavBarExtensions: {NavBarType} registered with {PageType}", navBar.GetType().Name, page.GetType().Name);
                }
                else if (oldValue != null)
                {
                    Logger.Debug("NavBarExtensions: NavBar unregistered from {PageType}", page.GetType().Name);
                }
            }
        }

        /// <summary>
        /// 🔍 HELPER: Find parent of specific type in visual tree
        /// </summary>
        public static T FindParentOfType<T>(this Element element) where T : Element
        {
            var parent = element.Parent;
            while (parent != null)
            {
                if (parent is T typedParent)
                {
                    return typedParent;
                }
                parent = parent.Parent;
            }
            return null;
        }
    }
}
