using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;
using Serilog;

namespace MyVocaList.View.Components
{
    /// <summary>
    /// IMPROVED: LoadingOverlay with auto-detection and forced Z-index
    /// FIXES: Visibility and positioning issues
    /// </summary>
    public partial class LoadingOverlayComponent : ContentView
    {
        private static readonly Serilog.ILogger Logger = Log.ForContext<LoadingOverlayComponent>();

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

            // INITIAL: Starts invisible
            this.IsVisible = false;

            // Z-INDEX: Force to front
            this.ZIndex = 9999;

            // LAYOUT: Force to occupy full area
            this.HorizontalOptions = LayoutOptions.Fill;
            this.VerticalOptions = LayoutOptions.Fill;

            Logger.Debug("LoadingOverlayComponent: Constructor - ZIndex={ZIndex}", this.ZIndex);
        }

        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();

            if (Handler != null)
            {
                // FORCE: Correct positioning after Handler is available
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

                Logger.Debug("LoadingOverlayComponent: IsLoading changed to {IsLoading}", isLoading);

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    // VISIBILITY: Controls correctly
                    component.IsVisible = isLoading;

                    if (isLoading)
                    {
                        // FORCE: Positioning to front when showing
                        component.ForceCorrectPositioning();
                        Logger.Debug("LoadingOverlayComponent: SHOWN");
                    }
                    else
                    {
                        Logger.Debug("LoadingOverlayComponent: HIDDEN");
                    }
                });
            }
        }

        /// <summary>
        /// FORCE: Correct positioning of overlay
        /// </summary>
        private void ForceCorrectPositioning()
        {
            // Z-INDEX: Force to front
            this.ZIndex = 9999;

            // LAYOUT: Force to occupy full area
            this.HorizontalOptions = LayoutOptions.Fill;
            this.VerticalOptions = LayoutOptions.Fill;

            // PARENT: If in Layout, force repositioning via removal/addition
            if (this.Parent is Layout parentLayout)
            {
                // MAUI: Remove and add again to force position on top
                var index = parentLayout.Children.IndexOf(this);
                if (index >= 0)
                {
                    parentLayout.Children.RemoveAt(index);
                    parentLayout.Children.Add(this); // Add at end (most to front)
                    Logger.Debug("LoadingOverlayComponent: Repositioned in layout");
                }
            }

            // GRID: If in Grid, force last position
            if (this.Parent is Grid parentGrid)
            {
                Grid.SetRow(this, 0);
                Grid.SetColumn(this, 0);
                Grid.SetRowSpan(this, Math.Max(1, parentGrid.RowDefinitions.Count));
                Grid.SetColumnSpan(this, Math.Max(1, parentGrid.ColumnDefinitions.Count));
                Logger.Debug("LoadingOverlayComponent: Grid spans configured");
            }
        }

        /// <summary>
        /// PUBLIC: Method for manual testing
        /// </summary>
        public async Task ShowTestLoadingAsync(int durationMs = 2000)
        {
            Logger.Debug("LoadingOverlayComponent: TEST started - {Duration}ms", durationMs);

            IsLoading = true;
            await Task.Delay(durationMs);
            IsLoading = false;

            Logger.Debug("LoadingOverlayComponent: TEST completed");
        }
    }
}
