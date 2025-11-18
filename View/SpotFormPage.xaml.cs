using MyVocaList.Domain;
using MyVocaList.Services;
using MyVocaList.View.Components;
using MyVocaList.View.Extensions;
using MyVocaList.View.Behaviors;
using System.Windows.Input;

namespace MyVocaList.View
{
    public partial class SpotFormPage : ContentPage
    {
        private IEstabelecimentoService _estabelecimentoService;
        private bool _isEditing = false;
        private Estabelecimento _editingLocal = null;
        private bool _isInitialized = false;

        // Lifecycle Command
        public ICommand LoadDataCommand { get; }

        public SpotFormPage()
        {
            LoadDataCommand = new Command(async () => await InitializeDataAsync());
            InitializeComponent();
            this.BindingContext = this;
        }

        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();
            if (Handler != null && !_isInitialized)
            {
                try
                {
                    var serviceProvider = MyVocaList.View.ServiceProvider.FromPage(this);
                    _estabelecimentoService = serviceProvider?.GetService<IEstabelecimentoService>();

                    if (_estabelecimentoService != null) _isInitialized = true;

                    headerComponent?.ConfigureSafeBackNavigation(null, 500);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"SpotFormPage Error: {ex.Message}");
                }
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Programmatically focus the new component
            nomeLocalInput?.Focus();
        }

        // Called by SmartPageLifecycleBehavior via Command
        private async Task InitializeDataAsync()
        {
            // Data init logic...
            await Task.Delay(50);
        }

        #region Configuration Methods
        public void ConfigureForAdding()
        {
            _isEditing = false;
            _editingLocal = null;
            if (headerComponent != null) headerComponent.Title = "Add Venue";

            if (nomeLocalInput != null)
            {
                nomeLocalInput.Text = string.Empty;
                ResetInputState();
            }
        }

        public void ConfigureForEditing(Estabelecimento local)
        {
            _isEditing = true;
            _editingLocal = local;
            if (headerComponent != null) headerComponent.Title = "Edit Venue";

            if (nomeLocalInput != null)
            {
                nomeLocalInput.Text = local.Nome;
                ResetInputState();
            }
        }
        #endregion

        #region Interaction
        private void OnNomeLocalTextChanged(object sender, TextChangedEventArgs e)
        {
            var currentLength = e.NewTextValue?.Length ?? 0;
            UpdateCharacterCounter(currentLength);

            // Clear error state immediately when typing
            if (nomeLocalInput != null && nomeLocalInput.HasError)
            {
                nomeLocalInput.HasError = false;
                nomeLocalInput.ErrorText = "";
            }
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            await OnSalvarLocalAsyncInternal();
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await NavigateBackToSpotPage();
        }
        #endregion

        #region Saving Logic
        private async Task OnSalvarLocalAsyncInternal()
        {
            if (_estabelecimentoService == null) return;

            // 1. Get Data
            var nomeLocal = nomeLocalInput?.Text?.Trim();

            // 2. Validate
            var validation = _estabelecimentoService.ValidateNameInput(nomeLocal);
            if (!validation.isValid)
            {
                ShowValidationMessage(validation.message);
                return;
            }

            // 3. Show Global Loading
            // Uses the global singleton you provided
            await GlobalLoadingOverlay.ShowLoadingAsync("Saving venue...");

            try
            {
                bool saveSuccess;
                string resultMessage;

                if (_isEditing && _editingLocal != null)
                {
                    var result = await _estabelecimentoService.UpdateEstabelecimentoAsync(_editingLocal.Id, nomeLocal);
                    saveSuccess = result.success;
                    resultMessage = result.message;
                }
                else
                {
                    var result = await _estabelecimentoService.CreateEstabelecimentoAsync(nomeLocal);
                    saveSuccess = result.success;
                    resultMessage = result.message;
                }

                if (saveSuccess)
                {
                    nomeLocalInput.Text = string.Empty;
                    await NavigateBackToSpotPage();
                    await GlobalSnackbar.ShowSuccessAsync(resultMessage);
                }
                else
                {
                    ShowValidationMessage(resultMessage);
                }
            }
            catch (Exception ex)
            {
                ShowValidationMessage($"Error: {ex.Message}");
            }
            finally
            {
                // 4. Hide Global Loading
                await GlobalLoadingOverlay.HideLoadingAsync();
            }
        }
        #endregion

        #region Helpers
        private async Task NavigateBackToSpotPage()
        {
            var backBehavior = this.Behaviors?.OfType<SafeNavigationBehavior>()
                .FirstOrDefault(b => b.TargetPageType == typeof(SpotPage));

            if (backBehavior != null)
            {
                backBehavior.CreatePageFunc = () => new SpotPage();
                await backBehavior.NavigateToPageAsync();
            }
            else
            {
                await Navigation.PopAsync();
            }
        }

        private void UpdateCharacterCounter(int currentLength)
        {
            if (nomeLocalInput == null || _estabelecimentoService == null) return;

            if (_estabelecimentoService.ShouldShowCharacterCounter(currentLength))
            {
                var (text, isWarning, isError) = _estabelecimentoService.GetCharacterCounterInfo(currentLength);

                nomeLocalInput.HelperText = text;
                nomeLocalInput.ShowCounter = true;

                // Optional: Logic to change color can be added to component if needed
            }
            else
            {
                nomeLocalInput.ShowCounter = false;
            }
        }

        private void ShowValidationMessage(string message)
        {
            if (nomeLocalInput != null)
            {
                nomeLocalInput.HasError = true;
                nomeLocalInput.ErrorText = message;
            }
        }

        private void ResetInputState()
        {
            if (nomeLocalInput != null)
            {
                nomeLocalInput.HasError = false;
                nomeLocalInput.ErrorText = "";
                nomeLocalInput.ShowCounter = false;
            }
        }
        #endregion
    }
}