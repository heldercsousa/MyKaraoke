using MyKaraoke.Domain;
using MyKaraoke.Services;

namespace MyKaraoke.View
{
    public partial class SpotFormPage : ContentPage
    {
        private IEstabelecimentoService _estabelecimentoService;
        private MyKaraoke.View.ServiceProvider _serviceProvider;

        // Estados da interface
        private bool _isEditing = false;
        private Estabelecimento _editingLocal = null;

        public SpotFormPage()
        {
            InitializeComponent();
        }

        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();

            if (Handler != null)
            {
                try
                {
                    _serviceProvider = MyKaraoke.View.ServiceProvider.FromPage(this);
                    _estabelecimentoService = _serviceProvider?.GetService<IEstabelecimentoService>();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Erro ao inicializar serviços SpotFormPage: {ex.Message}");
                }
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Foca no campo de entrada quando a página aparece
            nomeLocalEntry.Focus();
        }

        #region Configuração da Página

        /// <summary>
        /// Configura a página para adicionar novo local
        /// </summary>
        public void ConfigureForAdding()
        {
            _isEditing = false;
            _editingLocal = null;

            headerComponent.Title = "Adicionar Local";
            salvarButton.Text = "Salvar Local";
            nomeLocalEntry.Text = string.Empty;

            ClearMessages();
        }

        /// <summary>
        /// Configura a página para editar local existente
        /// </summary>
        public void ConfigureForEditing(Estabelecimento local)
        {
            _isEditing = true;
            _editingLocal = local;

            headerComponent.Title = "Editar Local";
            salvarButton.Text = "Atualizar Local";
            nomeLocalEntry.Text = local.Nome;

            ClearMessages();
        }

        #endregion

        #region Event Handlers

        private void OnNomeLocalTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var currentLength = e.NewTextValue?.Length ?? 0;

                // Atualiza contador de caracteres
                UpdateCharacterCounter(currentLength);

                // Limpa mensagens de erro enquanto digita
                ClearMessages();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro em OnNomeLocalTextChanged: {ex.Message}");
            }
        }

        private async void OnSalvarLocalClicked(object sender, EventArgs e)
        {
            if (_estabelecimentoService == null)
            {
                ShowValidationMessage("Serviços não disponíveis");
                return;
            }

            var nomeLocal = nomeLocalEntry.Text?.Trim();

            // Validação básica
            var validation = _estabelecimentoService.ValidateNameInput(nomeLocal);
            if (!validation.isValid)
            {
                ShowValidationMessage(validation.message);
                return;
            }

            try
            {
                salvarButton.IsEnabled = false;

                if (_isEditing && _editingLocal != null)
                {
                    // Atualizar local existente
                    var result = await _estabelecimentoService.UpdateEstabelecimentoAsync(_editingLocal.Id, nomeLocal);
                    if (result.success)
                    {
                        ShowSuccessMessage(result.message);
                        await Task.Delay(1500); // Mostra mensagem por um tempo
                        await NavigateBackToSpotPage();
                    }
                    else
                    {
                        ShowValidationMessage(result.message);
                    }
                }
                else
                {
                    // Criar novo local
                    var result = await _estabelecimentoService.CreateEstabelecimentoAsync(nomeLocal);
                    if (result.success)
                    {
                        ShowSuccessMessage(result.message);
                        await Task.Delay(1500); // Mostra mensagem por um tempo
                        await NavigateBackToSpotPage();
                    }
                    else
                    {
                        ShowValidationMessage(result.message);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao salvar local: {ex.Message}");
                ShowValidationMessage("Erro interno ao salvar");
            }
            finally
            {
                salvarButton.IsEnabled = true;
            }
        }

        private async void OnCancelarClicked(object sender, EventArgs e)
        {
            await NavigateBackToSpotPage();
        }

        #endregion

        #region Navegação

        private async Task NavigateBackToSpotPage()
        {
            try
            {
                // Volta para a página anterior (SpotPage)
                await Navigation.PopAsync();
                System.Diagnostics.Debug.WriteLine("Voltou para SpotPage");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao voltar para SpotPage: {ex.Message}");
            }
        }

        #endregion

        #region Utilitários de UI

        private void UpdateCharacterCounter(int currentLength)
        {
            try
            {
                if (_estabelecimentoService == null)
                {
                    characterCounterLabel.IsVisible = false;
                    return;
                }

                // Usa o serviço para determinar se deve mostrar contador
                if (_estabelecimentoService.ShouldShowCharacterCounter(currentLength))
                {
                    var (text, isWarning, isError) = _estabelecimentoService.GetCharacterCounterInfo(currentLength);

                    characterCounterLabel.Text = text;
                    characterCounterLabel.IsVisible = true;

                    // Cores baseadas na proximidade do limite
                    if (isError)
                    {
                        characterCounterLabel.TextColor = Color.FromArgb("#ff6b6b");
                    }
                    else if (isWarning)
                    {
                        characterCounterLabel.TextColor = Color.FromArgb("#FF9800");
                    }
                    else
                    {
                        characterCounterLabel.TextColor = Color.FromArgb("#b0a8c7");
                    }
                }
                else
                {
                    characterCounterLabel.IsVisible = false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no contador de caracteres: {ex.Message}");
                characterCounterLabel.IsVisible = false;
            }
        }

        private void ShowValidationMessage(string message)
        {
            validationMessageLabel.Text = message;
            validationMessageLabel.IsVisible = true;
            successMessageLabel.IsVisible = false;
        }

        private void ShowSuccessMessage(string message)
        {
            successMessageLabel.Text = message;
            successMessageLabel.IsVisible = true;
            validationMessageLabel.IsVisible = false;
        }

        private void ClearMessages()
        {
            validationMessageLabel.IsVisible = false;
            successMessageLabel.IsVisible = false;
        }

        #endregion
    }
}