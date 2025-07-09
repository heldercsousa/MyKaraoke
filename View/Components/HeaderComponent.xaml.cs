using System.Windows.Input;

namespace MyKaraoke.View.Components;

public partial class HeaderComponent : ContentView
{
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(HeaderComponent), string.Empty);

    //public static readonly BindableProperty HeaderMarginProperty =
    //    BindableProperty.Create(nameof(HeaderMargin), typeof(Thickness), typeof(HeaderComponent), new Thickness(15, 40, 15, 5));

    public static readonly BindableProperty BackCommandProperty =
        BindableProperty.Create(nameof(BackCommand), typeof(ICommand), typeof(HeaderComponent), null);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    //public Thickness HeaderMargin
    //{
    //    get => (Thickness)GetValue(HeaderMarginProperty);
    //    set => SetValue(HeaderMarginProperty, value);
    //}

    public ICommand BackCommand
    {
        get => (ICommand)GetValue(BackCommandProperty);
        set => SetValue(BackCommandProperty, value);
    }

    public HeaderComponent()
    {
        InitializeComponent();
    }

    private void OnBackButtonClicked(object sender, EventArgs e)
    {
        BackCommand?.Execute(null);
    }
}