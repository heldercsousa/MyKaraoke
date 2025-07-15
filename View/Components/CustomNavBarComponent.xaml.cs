using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MyKaraoke.View.Components
{
    // Este componente agora implementa a interface e delega diretamente para o BaseNavBarComponent.
    public partial class CustomNavBarComponent : ContentView, IAnimatableNavBar
    {
        // 1. Propriedade para RECEBER a lista de botões do XAML.
        public static readonly BindableProperty ButtonsProperty =
            BindableProperty.Create(nameof(Buttons), typeof(ObservableCollection<NavButtonConfig>), typeof(CustomNavBarComponent), null,
            propertyChanged: OnButtonsChanged);

        public ObservableCollection<NavButtonConfig> Buttons
        {
            get => (ObservableCollection<NavButtonConfig>)GetValue(ButtonsProperty);
            set => SetValue(ButtonsProperty, value);
        }

        // 2. Evento GENÉRICO para notificar sobre qualquer clique de botão.
        public event EventHandler<NavBarButtonClickedEventArgs> ButtonClicked;

        public CustomNavBarComponent()
        {
            InitializeComponent();
            baseNavBar.ButtonClicked += (sender, args) => ButtonClicked?.Invoke(this, args);
        }

        // 3. Quando a propriedade Buttons é definida no XAML, repassamos para o BaseNavBarComponent.
        private static void OnButtonsChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is CustomNavBarComponent customNavBar && newValue is ObservableCollection<NavButtonConfig> buttons)
            {
                customNavBar.baseNavBar.Buttons = buttons;
            }
        }

        // 4. Implementação direta da interface, delegando para o BaseNavBarComponent.
        public Task ShowAsync() => baseNavBar.ShowAsync();
        public Task HideAsync() => baseNavBar.HideAsync();
    }
}