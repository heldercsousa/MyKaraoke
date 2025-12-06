namespace MyVocaList.View.Components
{
    /// <summary>
    /// MD3 FAB (Floating Action Button) - Material Design 3 compliant
    /// Reusable component for primary floating actions across CRUD pages
    /// </summary>
    public partial class FabButtonComponent : ContentView
    {
        #region Events

        /// <summary>
        /// Fired when FAB is clicked
        /// </summary>
        public event EventHandler<EventArgs>? Clicked;

        #endregion

        public FabButtonComponent()
        {
            InitializeComponent();
        }

        #region Event Handlers

        private void OnFabClicked(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("[FAB] Button clicked - invoking Clicked event");

                // Invoke the Clicked event for parent page to handle
                Clicked?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FAB] Error in OnFabClicked: {ex.Message}");
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Show FAB with animation
        /// </summary>
        public async Task ShowAsync()
        {
            try
            {
                if (fabRoot == null) return;

                fabRoot.Opacity = 0;
                fabRoot.Scale = 0.8;
                fabRoot.IsVisible = true;

                await Task.WhenAll(
                    fabRoot.FadeTo(1.0, 250, Easing.CubicOut),
                    fabRoot.ScaleTo(1.0, 250, Easing.CubicOut)
                );
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FAB] Error in ShowAsync: {ex.Message}");
            }
        }

        /// <summary>
        /// Hide FAB with animation
        /// </summary>
        public async Task HideAsync()
        {
            try
            {
                if (fabRoot == null) return;

                await Task.WhenAll(
                    fabRoot.FadeTo(0, 200, Easing.CubicIn),
                    fabRoot.ScaleTo(0.8, 200, Easing.CubicIn)
                );

                fabRoot.IsVisible = false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FAB] Error in HideAsync: {ex.Message}");
            }
        }

        #endregion
    }
}