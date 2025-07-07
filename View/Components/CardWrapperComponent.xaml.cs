using System.Windows.Input;
using MauiView = Microsoft.Maui.Controls.View;

namespace MyKaraoke.View.Components;

public partial class CardWrapperComponent : ContentView
{
    public static readonly BindableProperty CardContentProperty =
        BindableProperty.Create(nameof(CardContent), typeof(MauiView), typeof(CardWrapperComponent), null);

    public static readonly BindableProperty CardMarginProperty =
        BindableProperty.Create(nameof(CardMargin), typeof(Thickness), typeof(CardWrapperComponent), new Thickness(15, 10, 15, 0));

    public static readonly BindableProperty CardPaddingProperty =
        BindableProperty.Create(nameof(CardPadding), typeof(Thickness), typeof(CardWrapperComponent), new Thickness(20, 30, 20, 20));

    public static readonly BindableProperty VerticalOptionsProperty =
        BindableProperty.Create(nameof(VerticalOptions), typeof(LayoutOptions), typeof(CardWrapperComponent), LayoutOptions.Fill);

    public MauiView CardContent
    {
        get => (MauiView)GetValue(CardContentProperty);
        set => SetValue(CardContentProperty, value);
    }

    public Thickness CardMargin
    {
        get => (Thickness)GetValue(CardMarginProperty);
        set => SetValue(CardMarginProperty, value);
    }

    public Thickness CardPadding
    {
        get => (Thickness)GetValue(CardPaddingProperty);
        set => SetValue(CardPaddingProperty, value);
    }

    public LayoutOptions VerticalOptions
    {
        get => (LayoutOptions)GetValue(VerticalOptionsProperty);
        set => SetValue(VerticalOptionsProperty, value);
    }

    public CardWrapperComponent()
    {
        InitializeComponent();
    }
}