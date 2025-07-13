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

            // Debug: Log component initialization
            System.Diagnostics.Debug.WriteLine($"SpotPage Constructor - CrudNavBarComponent: {CrudNavBarComponent != null}");
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

                    // Debug: Log component availability after handler changed
                    System.Diagnostics.Debug.WriteLine($"OnHandlerChanged - CrudNavBarComponent: {CrudNavBarComponent != null}");
                    System.Diagnostics.Debug.WriteLine($"OnHandlerChanged - EstabelecimentoService: {_estabelecimentoService != null}");
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

            // Debug: Log component availability on appearing
            System.Diagnostics.Debug.WriteLine($"OnAppearing - CrudNavBarComponent: {CrudNavBarComponent != null}");
            System.Diagnostics.Debug.WriteLine($"OnAppearing - EstabelecimentoService: {_estabelecimentoService != null}");

            // ✅ CORREÇÃO CRÍTICA: Sempre recarrega dados ao aparecer na tela
            await LoadLocaisAsync();

            // Wait a bit to ensure XAML elements are fully loaded
            await Task.Delay(100);

            // Inicia animações do CrudNavBarComponent se necessário
            await StartCrudNavBarComponentAnimationsIfNeeded();
        }

        #region Carregamento de Dados

        private async Task LoadLocaisAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("LoadLocaisAsync - Iniciando carregamento");

                if (_estabelecimentoService == null)
                {
                    System.Diagnostics.Debug.WriteLine("LoadLocaisAsync - EstabelecimentoService é null, mostrando estado vazio");
                    ShowEmptyState();
                    return;
                }

                var locais = await _estabelecimentoService.GetAllEstabelecimentosAsync();
                System.Diagnostics.Debug.WriteLine($"LoadLocaisAsync - Encontrados {locais?.Count() ?? 0} locais");

                // ✅ CORREÇÃO: Aguarda thread principal para atualizar UI
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    _locais.Clear();

                    if (locais != null)
                    {
                        foreach (var local in locais)
                        {
                            _locais.Add(local);
                            System.Diagnostics.Debug.WriteLine($"LoadLocaisAsync - Adicionado local: {local.Nome}");
                        }
                    }

                    System.Diagnostics.Debug.WriteLine($"LoadLocaisAsync - ObservableCollection agora tem {_locais.Count} itens");

                    // ✅ CORREÇÃO: Força atualização da UI após modificar dados
                    UpdateUIState();
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao carregar locais: {ex.Message}");
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    ShowEmptyState();
                });
            }
        }

        private void UpdateUIState()
        {
            try
            {
                bool hasLocais = _locais.Count > 0;
                System.Diagnostics.Debug.WriteLine($"UpdateUIState - hasLocais: {hasLocais}, count: {_locais.Count}");

                // ✅ CORREÇÃO: Força atualização no MainThread
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    // Mostra/esconde lista vs estado vazio
                    locaisCollectionView.IsVisible = hasLocais;
                    emptyStateFrame.IsVisible = !hasLocais;

                    System.Diagnostics.Debug.WriteLine($"UpdateUIState - locaisCollectionView.IsVisible: {locaisCollectionView.IsVisible}");
                    System.Diagnostics.Debug.WriteLine($"UpdateUIState - emptyStateFrame.IsVisible: {emptyStateFrame.IsVisible}");

                    // ✅ NOVO: Força refresh da CollectionView para garantir renderização
                    if (hasLocais && locaisCollectionView.ItemsSource != null)
                    {
                        // Força atualização da CollectionView
                        var currentSource = locaisCollectionView.ItemsSource;
                        locaisCollectionView.ItemsSource = null;
                        locaisCollectionView.ItemsSource = currentSource;
                        System.Diagnostics.Debug.WriteLine("UpdateUIState - CollectionView ItemsSource refreshed");
                    }

                    // Atualiza visibilidade da navbar baseado no estado
                    UpdateCrudNavBarComponentVisibility();
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro em UpdateUIState: {ex.Message}");
            }
        }

        private void ShowEmptyState()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("ShowEmptyState - Configurando estado vazio");

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    locaisCollectionView.IsVisible = false;
                    emptyStateFrame.IsVisible = true;

                    System.Diagnostics.Debug.WriteLine($"ShowEmptyState - locaisCollectionView.IsVisible: {locaisCollectionView.IsVisible}");
                    System.Diagnostics.Debug.WriteLine($"ShowEmptyState - emptyStateFrame.IsVisible: {emptyStateFrame.IsVisible}");

                    // Sempre mostra o CrudNavBarComponent no estado vazio
                    UpdateCrudNavBarComponentVisibility();
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro em ShowEmptyState: {ex.Message}");
            }
        }

        private void UpdateCrudNavBarComponentVisibility()
        {
            try
            {
                if (CrudNavBarComponent != null)
                {
                    // Sempre visível na SpotPage para permitir adicionar novos locais
                    CrudNavBarComponent.IsVisible = true;
                    System.Diagnostics.Debug.WriteLine("CrudNavBarComponent visibilidade atualizada: sempre visível");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao atualizar visibilidade CrudNavBarComponent: {ex.Message}");
            }
        }

        #endregion

        #region Animation Methods

        /// <summary>
        /// Inicia animações do CrudNavBarComponent se necessário
        /// Similar ao padrão usado na StackPage
        /// </summary>
        private async Task StartCrudNavBarComponentAnimationsIfNeeded()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("StartCrudNavBarComponentAnimationsIfNeeded - Starting");

                if (CrudNavBarComponent != null)
                {
                    try
                    {
                        await Task.Delay(500); // Aguarda UI renderizar
                        await CrudNavBarComponent.StartNovoLocalAnimationAsync();
                        System.Diagnostics.Debug.WriteLine("StartCrudNavBarComponentAnimationsIfNeeded - Animação Novo Local iniciada com sucesso");
                    }
                    catch (Exception animEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"StartCrudNavBarComponentAnimationsIfNeeded - Erro ao iniciar animação: {animEx.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"StartCrudNavBarComponentAnimationsIfNeeded - Exception: {ex.Message}");
            }
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

        #region CrudNavBarComponent Event Handlers

        /// <summary>
        /// Handler para o evento NovoLocalClicked do CrudNavBarComponent
        /// ✅ CORRIGIDO: Threading e navegação otimizados
        /// </summary>
        private async void OnCrudNavBarComponentNovoLocalClicked(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Botão 'Novo Local' clicado via CrudNavBarComponent - navegando para SpotFormPage");

                // ✅ CORREÇÃO: Navegação deve ser no MainThread para evitar threading issues
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    // ✅ CORREÇÃO: Pequeno delay para evitar "Pending Navigations still processing"
                    await Task.Delay(100);

                    await NavigateToSpotFormPageAsync(isEditing: false);
                    System.Diagnostics.Debug.WriteLine("Navegação para SpotFormPage concluída - threading correto");
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao navegar para SpotFormPage via CrudNavBarComponent: {ex.Message}");
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
                // ✅ CORREÇÃO: Usa ServiceProvider no MainThread para evitar threading issues
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
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
                            System.Diagnostics.Debug.WriteLine($"Navegação para SpotFormPage bem-sucedida (isEditing: {isEditing}) - via ServiceProvider");
                            return;
                        }
                        catch (Exception serviceEx)
                        {
                            System.Diagnostics.Debug.WriteLine($"Erro ao usar ServiceProvider (MainThread): {serviceEx.Message}");
                        }
                    }

                    // Fallback: criação direta (também no MainThread)
                    try
                    {
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
                        System.Diagnostics.Debug.WriteLine("Navegação para SpotFormPage via fallback (MainThread)");
                    }
                    catch (Exception fallbackEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"Erro no fallback de navegação: {fallbackEx.Message}");
                    }
                });
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
                    // ✅ CORREÇÃO: Recarrega dados após exclusão
                    await LoadLocaisAsync();
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

        #region Lifecycle Methods

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
            try
            {
                System.Diagnostics.Debug.WriteLine("SpotPage: OnDisappearing - parando animações pois página está saindo");

                // ✅ CORREÇÃO: Para animações quando a página REALMENTE desaparece
                // Isso é correto - a animação deve parar quando o botão não está mais visível
                if (CrudNavBarComponent != null)
                {
                    try
                    {
                        await CrudNavBarComponent.StopNovoLocalAnimationAsync();
                        System.Diagnostics.Debug.WriteLine("OnDisappearing - Animação Novo Local parada (página saindo)");
                    }
                    catch (Exception animEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"OnDisappearing - Erro ao parar animação: {animEx.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"OnDisappearing - Error: {ex.Message}");
            }
        }

        #endregion
    }
}