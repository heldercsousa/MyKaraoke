using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;

namespace MyKaraoke.View.Components
{
    /// <summary>
    /// ✅ MELHORADO: LoadingOverlay com auto-detecção e Z-index forçado
    /// 🎯 CORRIGE: Problemas de visibilidade e posicionamento
    /// </summary>
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

            // ✅ INICIAL: Começa invisível
            this.IsVisible = false;

            // 🎯 Z-INDEX: Força ficar na frente
            this.ZIndex = 9999;

            // 🎯 LAYOUT: Força ocupar toda a área disponível
            this.HorizontalOptions = LayoutOptions.Fill;
            this.VerticalOptions = LayoutOptions.Fill;

            System.Diagnostics.Debug.WriteLine($"🔄 LoadingOverlayComponent: Construtor - ZIndex={this.ZIndex}");
        }

        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();

            if (Handler != null)
            {
                // 🎯 FORÇA: Posicionamento correto após Handler estar disponível
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ForceCorrectPositioning();
                });
            }
        }

        private static void OnIsLoadingChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is LoadingOverlayComponent component)
            {
                var isLoading = (bool)newValue;

                System.Diagnostics.Debug.WriteLine($"🔄 LoadingOverlayComponent: IsLoading mudou para {isLoading}");

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    // ✅ VISIBILIDADE: Controla corretamente
                    component.IsVisible = isLoading;

                    if (isLoading)
                    {
                        // 🎯 FORÇA: Posicionamento na frente quando mostrar
                        component.ForceCorrectPositioning();
                        System.Diagnostics.Debug.WriteLine($"🔄 LoadingOverlayComponent: EXIBIDO");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"🔄 LoadingOverlayComponent: ESCONDIDO");
                    }
                });
            }
        }

        /// <summary>
        /// 🎯 FORÇA: Posicionamento correto do overlay
        /// </summary>
        private void ForceCorrectPositioning()
        {
            try
            {
                // 🎯 Z-INDEX: Força ficar na frente
                this.ZIndex = 9999;

                // 🎯 LAYOUT: Força ocupar toda a área
                this.HorizontalOptions = LayoutOptions.Fill;
                this.VerticalOptions = LayoutOptions.Fill;

                // 🎯 PARENT: Se está em Layout, força subir para frente
                if (this.Parent is Layout parentLayout)
                {
                    parentLayout.RaiseChild(this);
                    System.Diagnostics.Debug.WriteLine($"🎯 LoadingOverlayComponent: RaiseChild executado");
                }

                // 🎯 GRID: Se está em Grid, força última posição
                if (this.Parent is Grid parentGrid)
                {
                    Grid.SetRow(this, 0);
                    Grid.SetColumn(this, 0);
                    Grid.SetRowSpan(this, Math.Max(1, parentGrid.RowDefinitions.Count));
                    Grid.SetColumnSpan(this, Math.Max(1, parentGrid.ColumnDefinitions.Count));
                    System.Diagnostics.Debug.WriteLine($"🎯 LoadingOverlayComponent: Grid spans configurados");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ LoadingOverlayComponent: Erro no posicionamento: {ex.Message}");
            }
        }

        /// <summary>
        /// 🎯 PÚBLICO: Método para teste manual
        /// </summary>
        public async Task ShowTestLoadingAsync(int durationMs = 2000)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🧪 LoadingOverlayComponent: TESTE iniciado - {durationMs}ms");

                IsLoading = true;
                await Task.Delay(durationMs);
                IsLoading = false;

                System.Diagnostics.Debug.WriteLine($"🧪 LoadingOverlayComponent: TESTE concluído");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ LoadingOverlayComponent: Erro no teste: {ex.Message}");
                IsLoading = false;
            }
        }
    }
}