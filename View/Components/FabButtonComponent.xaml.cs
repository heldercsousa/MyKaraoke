namespace MyKaraoke.View.Components
{
    /// <summary>
    /// 🎯 FAB (Floating Action Button) - Padrão Material Design
    /// ✅ Componente reutilizável para ação primária flutuante
    /// 📱 Conforme padrões UI/UX Mobile 2025
    /// </summary>
    public partial class FabButtonComponent : ContentView
    {
        #region Bindable Properties

        public static readonly BindableProperty IconTextProperty =
            BindableProperty.Create(nameof(IconText), typeof(string), typeof(FabButtonComponent), "+",
            propertyChanged: OnIconTextChanged);

        #endregion

        #region Properties

        public string IconText
        {
            get => (string)GetValue(IconTextProperty);
            set => SetValue(IconTextProperty, value);
        }

        #endregion

        #region Events

        public event EventHandler<EventArgs> Clicked;

        #endregion

        public FabButtonComponent()
        {
            InitializeComponent();
        }

        #region Property Changed Handlers

        private static void OnIconTextChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is FabButtonComponent fab && newValue is string iconText)
            {
                try
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        if (fab.fabIcon != null)
                        {
                            fab.fabIcon.Text = iconText;
                        }
                    });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ FabButtonComponent: Erro ao definir IconText: {ex.Message}");
                }
            }
        }

        #endregion

        #region Event Handlers

        private async void OnFabTapped(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("========================================");
                System.Diagnostics.Debug.WriteLine("FAB: OnFabTapped CHAMADO");
                System.Diagnostics.Debug.WriteLine($"Sender: {sender?.GetType().Name}");
                System.Diagnostics.Debug.WriteLine("========================================");

                // TESTE: Mostra alert
                await Application.Current.MainPage.DisplayAlert("FAB", "FAB foi clicado!", "OK");

                // Animação de tap
                await AnimateTapEffect();

                System.Diagnostics.Debug.WriteLine("FAB: Disparando evento Clicked...");

                // Dispara evento
                Clicked?.Invoke(this, EventArgs.Empty);

                if (Clicked == null)
                {
                    System.Diagnostics.Debug.WriteLine("ERRO: Evento Clicked é NULL - nenhum subscriber!");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("FAB: Evento Clicked disparado com sucesso");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERRO em OnFabTapped: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
            }
        }

        #endregion

        #region Animations

        private async Task AnimateTapEffect()
        {
            try
            {
                if (fabContainer == null) return;

                await fabContainer.ScaleTo(0.9, 100, Easing.CubicOut);
                await fabContainer.ScaleTo(1.0, 100, Easing.CubicOut);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ FabButtonComponent: Erro na animação de tap: {ex.Message}");
            }
        }

        public async Task ShowAsync()
        {
            try
            {
                if (fabRoot == null) return;

                System.Diagnostics.Debug.WriteLine("🎯 FabButtonComponent: ShowAsync iniciado");

                fabRoot.Opacity = 0;
                fabRoot.Scale = 0.8;
                fabRoot.IsVisible = true;

                await Task.WhenAll(
                    fabRoot.FadeTo(1.0, 250, Easing.CubicOut),
                    fabRoot.ScaleTo(1.0, 250, Easing.CubicOut)
                );

                System.Diagnostics.Debug.WriteLine("✅ FabButtonComponent: ShowAsync concluído");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ FabButtonComponent: Erro em ShowAsync: {ex.Message}");
            }
        }

        public async Task HideAsync()
        {
            try
            {
                if (fabRoot == null) return;

                System.Diagnostics.Debug.WriteLine("🎯 FabButtonComponent: HideAsync iniciado");

                await Task.WhenAll(
                    fabRoot.FadeTo(0, 200, Easing.CubicIn),
                    fabRoot.ScaleTo(0.8, 200, Easing.CubicIn)
                );

                fabRoot.IsVisible = false;

                System.Diagnostics.Debug.WriteLine("✅ FabButtonComponent: HideAsync concluído");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ FabButtonComponent: Erro em HideAsync: {ex.Message}");
            }
        }

        #endregion
    }
}