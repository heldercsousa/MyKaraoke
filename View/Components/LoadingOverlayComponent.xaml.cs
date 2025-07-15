using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyKaraoke.View.Components
{
    public partial class LoadingOverlayComponent : ContentView
    {
        public static readonly BindableProperty IsLoadingProperty =
            BindableProperty.Create(nameof(IsLoading), typeof(bool), typeof(LoadingOverlayComponent), false,
            propertyChanged: OnIsLoadingChanged);

        public bool IsLoading
        {
            get => (bool)GetValue(IsLoadingProperty);
            set => SetValue(IsLoadingProperty, value);
        }

        public LoadingOverlayComponent()
        {
            InitializeComponent();
            this.IsVisible = false; // Começa invisível por padrão
        }

        private static void OnIsLoadingChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is LoadingOverlayComponent component)
            {
                // Mostra ou esconde o componente inteiro baseado na propriedade
                component.IsVisible = (bool)newValue;
            }
        }
    }
}
