using Microsoft.Maui.Controls.Compatibility;
using System.Windows.Input;
using MauiView = Microsoft.Maui.Controls.View;
using MauiGrid = Microsoft.Maui.Controls.Grid;
using Serilog;

namespace MyVocaList.View.Components;

public partial class CardWrapperComponent : ContentView
{
    private static readonly Serilog.ILogger Logger = Log.ForContext<CardWrapperComponent>();

    //public static readonly BindableProperty IconPathProperty =
    //    BindableProperty.Create(nameof(IconPath), typeof(string), typeof(CardWrapperComponent), "stack_purple.png");

    public static readonly BindableProperty TitleTextProperty =
        BindableProperty.Create(nameof(TitleText), typeof(string), typeof(CardWrapperComponent), "Card title");

    public static readonly BindableProperty BadgeIsVisibleProperty =
        BindableProperty.Create(nameof(BadgeIsVisible), typeof(bool), typeof(CardWrapperComponent), true);

    public static readonly BindableProperty BadgeTextProperty =
        BindableProperty.Create(nameof(BadgeText), typeof(string), typeof(CardWrapperComponent), "---");

    public static readonly BindableProperty CardContentProperty =
        BindableProperty.Create(nameof(CardContent), typeof(MauiView), typeof(CardWrapperComponent), null, propertyChanged: OnCardContentChanged);

    private MauiGrid _contentContainer;

    //public string IconPath
    //{
    //    get => (string)GetValue(IconPathProperty);
    //    set => SetValue(IconPathProperty, value);
    //}

    public string TitleText
    {
        get => (string)GetValue(TitleTextProperty);
        set => SetValue(TitleTextProperty, value);
    }

    public bool BadgeIsVisible
    {
        get => (bool)GetValue(BadgeIsVisibleProperty);
        set => SetValue(BadgeIsVisibleProperty, value);
    }

    public string BadgeText
    {
        get => (string)GetValue(BadgeTextProperty);
        set => SetValue(BadgeTextProperty, value);
    }

    public MauiView CardContent
    {
        get => (MauiView)GetValue(CardContentProperty);
        set => SetValue(CardContentProperty, value);
    }

    public CardWrapperComponent()
    {
        InitializeComponent();
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        if (Handler != null)
        {
            // Find container after handler is available
            _contentContainer = this.FindByName<MauiGrid>("contentContainer");

            // Update content if already defined
            if (CardContent != null)
            {
                UpdateContent();
            }
        }
    }

    private static void OnCardContentChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is CardWrapperComponent component)
        {
            component.UpdateContent();
        }
    }

    private void UpdateContent()
    {
        if (_contentContainer == null) return;

        // FIX: Remove previous parent if exists
        if (CardContent?.Parent != null)
        {
            if (CardContent.Parent is Layout<MauiView> parentLayout)
            {
                parentLayout.Children.Remove(CardContent);
            }
            else if (CardContent.Parent is ContentView parentContentView)
            {
                parentContentView.Content = null;
            }
        }

        // Clear previous container
        _contentContainer.Children.Clear();

        // Add new content
        if (CardContent != null)
        {
            _contentContainer.Children.Add(CardContent);
        }
    }
}
