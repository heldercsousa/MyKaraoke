using System.Windows.Input;

namespace MyVocaList.View.Components
{
    public partial class MaterialEntryComponent : ContentView
    {
        public MaterialEntryComponent()
        {
            InitializeComponent();
        }

        // Bindable Properties to expose Entry features
        public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(MaterialEntryComponent), string.Empty);
        public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(MaterialEntryComponent), string.Empty, BindingMode.TwoWay);
        public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(MaterialEntryComponent), string.Empty);
        public static readonly BindableProperty ErrorTextProperty = BindableProperty.Create(nameof(ErrorText), typeof(string), typeof(MaterialEntryComponent), string.Empty);
        public static readonly BindableProperty HasErrorProperty = BindableProperty.Create(nameof(HasError), typeof(bool), typeof(MaterialEntryComponent), false);
        public static readonly BindableProperty HelperTextProperty = BindableProperty.Create(nameof(HelperText), typeof(string), typeof(MaterialEntryComponent), string.Empty);
        public static readonly BindableProperty ShowCounterProperty = BindableProperty.Create(nameof(ShowCounter), typeof(bool), typeof(MaterialEntryComponent), false);
        public static readonly BindableProperty MaxLengthProperty = BindableProperty.Create(nameof(MaxLength), typeof(int), typeof(MaterialEntryComponent), 255);
        public static readonly BindableProperty KeyboardProperty = BindableProperty.Create(nameof(Keyboard), typeof(Keyboard), typeof(MaterialEntryComponent), Keyboard.Default);

        public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }
        public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }
        public string Placeholder { get => (string)GetValue(PlaceholderProperty); set => SetValue(PlaceholderProperty, value); }
        public string ErrorText { get => (string)GetValue(ErrorTextProperty); set => SetValue(ErrorTextProperty, value); }
        public bool HasError { get => (bool)GetValue(HasErrorProperty); set => SetValue(HasErrorProperty, value); }
        public string HelperText { get => (string)GetValue(HelperTextProperty); set => SetValue(HelperTextProperty, value); }
        public bool ShowCounter { get => (bool)GetValue(ShowCounterProperty); set => SetValue(ShowCounterProperty, value); }
        public int MaxLength { get => (int)GetValue(MaxLengthProperty); set => SetValue(MaxLengthProperty, value); }
        public Keyboard Keyboard { get => (Keyboard)GetValue(KeyboardProperty); set => SetValue(KeyboardProperty, value); }

        // Expose TextChanged event for code-behind logic
        public event EventHandler<TextChangedEventArgs> TextChanged;

        private void OnInnerTextChanged(object sender, TextChangedEventArgs e)
        {
            TextChanged?.Invoke(this, e);
        }

        // Helper to programmatically focus the inner entry
        public new void Focus()
        {
            InnerEntry.Focus();
        }

        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();

#if ANDROID
            if (InnerEntry.Handler?.PlatformView is Android.Widget.EditText platformEntry)
            {
                platformEntry.Background = null; // Nuclear option to remove native background drawable
                platformEntry.SetBackgroundColor(Android.Graphics.Color.Transparent); // Ensure transparent background
                platformEntry.SetPadding(0, 0, 0, 0); // Remove native padding
                platformEntry.SetIncludeFontPadding(false); // Remove native font padding
            }
#elif IOS
        if (InnerEntry.Handler?.PlatformView is UIKit.UITextField platformEntry)
        {
            platformEntry.BorderStyle = UIKit.UITextBorderStyle.None; // Remove native border
        }
#endif
        }
    }
}