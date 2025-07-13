using MyKaraoke.Domain;
using MyKaraoke.Services;
using System.Collections.ObjectModel;

namespace MyKaraoke.View
{
    public partial class SpotPage : ContentPage
    {
        private IEstabelecimentoService _estabelecimentoService;
        private MyKaraoke.View.ServiceProvider _serviceProvider;
        private ObservableCollection<Estabelecimento> _locais;

        // Estados da interface
        private bool _isEditing = false;
        private Estabelecimento _editingLocal = null;

        public SpotPage()
        {
            InitializeComponent();

            // Inicializa coleções
            _locais = new ObservableCollection<Estabelecimento>();
            locaisCollectionView.ItemsSource = _locais;
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
                    System.Diagnostics.Debug.WriteLine($"Erro ao inicializar serviços SpotPage: {ex.Message}");
                }
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadLocaisAsync();
        }

        #region Carregamento de Dados

        private async Task LoadLocaisAsync()
        {
            try
            {
                if (_estabelecimentoService == null)
                {
                    ShowEmptyState();
                    return;
                }

                var locais = await _estabelecimentoService.GetAllEstabelecimentosAsync();

                _locais.Clear();
                foreach (var local in locais)
                {
                    _locais.Add(local);
                }

                UpdateUIState();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao carregar locais: {ex.Message}");
                ShowEmptyState();
            }
        }

        private void UpdateUIState()
        {
            bool hasLocais = _locais.Count > 0;

            // Mostra/esconde lista vs estado vazio
            locaisCollectionView.IsVisible = hasLocais;
            emptyStateFrame.IsVisible = !hasLocais;
        }

        private void ShowEmptyState()
        {
            locaisCollectionView.IsVisible = false;
            emptyStateFrame.IsVisible = true;
        }

        #endregion

        #region Event Handlers da Lista

        private void OnLocalItemTapped(object sender, EventArgs e)
        {
            // Implementar se necessário - por enquanto apenas o botão de ações
        }

        private async void OnHeaderAddButtonClicked(object sender, EventArgs e)
        {
            try
            {
                // Navega para SpotFormPage para adicionar novo local
                await NavigateToSpotFormPageAsync(isEditing: false);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao navegar para SpotFormPage: {ex.Message}");
            }
        }

        private async void OnLocalActionsClicked(object sender, EventArgs e)
        {
            try
            {
                if (sender is Button button && button.CommandParameter is Estabelecimento local)
                {
                    // Mostra Bottom Sheet com opções
                    await ShowLocalActionsBottomSheet(local);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao mostrar ações do local: {ex.Message}");
            }
        }

        #endregion

        #region Bottom Sheet de Ações

        private async Task ShowLocalActionsBottomSheet(Estabelecimento local)
        {
            try
            {
                var action = await DisplayActionSheet(
                    $"Ações para '{local.Nome}'",
                    "Cancelar",
                    null,
                    "✏️ Editar",
                    "🗑️ Excluir"
                );

                switch (action)
                {
                    case "✏️ Editar":
                        await NavigateToSpotFormPageAsync(isEditing: true, editingLocal: local);
                        break;
                    case "🗑️ Excluir":
                        await ConfirmDeleteLocal(local);
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no bottom sheet: {ex.Message}");
            }
        }

        #endregion

        #region Navegação

        private async Task NavigateToSpotFormPageAsync(bool isEditing, Estabelecimento editingLocal = null)
        {
            try
            {
                // Usa ServiceProvider para criar SpotFormPage
                if (_serviceProvider != null)
                {
                    try
                    {
                        var spotFormPage = _serviceProvider.GetService<SpotFormPage>();

                        // Configura página para edição ou criação
                        if (isEditing && editingLocal != null)
                        {
                            spotFormPage.ConfigureForEditing(editingLocal);
                        }
                        else
                        {
                            spotFormPage.ConfigureForAdding();
                        }

                        await Navigation.PushAsync(spotFormPage);
                        System.Diagnostics.Debug.WriteLine($"Navegação para SpotFormPage bem-sucedida (isEditing: {isEditing})");
                        return;
                    }
                    catch (Exception serviceEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"Erro ao usar ServiceProvider: {serviceEx.Message}");
                    }
                }

                // Fallback: criação direta
                var fallbackSpotFormPage = new SpotFormPage();
                if (isEditing && editingLocal != null)
                {
                    fallbackSpotFormPage.ConfigureForEditing(editingLocal);
                }
                else
                {
                    fallbackSpotFormPage.ConfigureForAdding();
                }

                await Navigation.PushAsync(fallbackSpotFormPage);
                System.Diagnostics.Debug.WriteLine("Navegação para SpotFormPage via fallback");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro na navegação para SpotFormPage: {ex.Message}");
            }
        }

        #endregion

        #region Exclusão

        private async Task ConfirmDeleteLocal(Estabelecimento local)
        {
            try
            {
                var confirm = await DisplayAlert(
                    "Confirmar Exclusão",
                    $"Tem certeza que deseja excluir '{local.Nome}'?\n\nEsta ação não pode ser desfeita.",
                    "Excluir",
                    "Cancelar"
                );

                if (confirm)
                {
                    await DeleteLocal(local);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro na confirmação de exclusão: {ex.Message}");
            }
        }

        private async Task DeleteLocal(Estabelecimento local)
        {
            try
            {
                if (_estabelecimentoService == null)
                {
                    await DisplayAlert("Erro", "Serviços não disponíveis", "OK");
                    return;
                }

                var result = await _estabelecimentoService.DeleteEstabelecimentoAsync(local.Id);

                if (result.success)
                {
                    await DisplayAlert("Sucesso", result.message, "OK");
                    await LoadLocaisAsync(); // Recarrega a lista
                }
                else
                {
                    await DisplayAlert("Erro", result.message, "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao excluir local: {ex.Message}");
                await DisplayAlert("Erro", "Erro interno ao excluir local", "OK");
            }
        }

        #endregion
    }
}