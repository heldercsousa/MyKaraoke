using MyVocaList.Domain;
using MyVocaList.Services;
using MyVocaList.Services.Configuration;
using MyVocaList.View.Components;
using MyVocaList.View.Extensions;
using MyVocaList.View.Behaviors;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MyVocaList.View.Interfaces;
using MyVocaList.Contracts.DTOs.List;
using MyVocaList.Services.Mappers;
using System.Threading;
using CommunityToolkit.Maui.Views;

namespace MyVocaList.View
{
    public partial class SpotPage : ContentPage, IManipulableDataPage
    {
        private IEstabelecimentoService _estabelecimentoService;
        public ObservableCollection<EstabelecimentoListItemDto> Locais { get; }

        // Pagination state
        private int _currentPage = 1;
        private int _totalCount = 0;
        private bool _hasMoreItems = true;
        private string _currentSearchQuery = null;

        private int _selectionCount;
        public int SelectionCount
        {
            get => _selectionCount;
            set
            {
                if (_selectionCount != value)
                {
                    _selectionCount = value;
                    OnPropertyChanged(nameof(SelectionCount));
                }
            }
        }

        #region IManipulableDataPage Members 

        public ICommand LoadDataCommand { get; private set; }
        public ICommand DeleteSingleCommand { get; private set; } // ✅ NOVO: Comando para Swipe-to-Delete
        public string FriendlyName => "Venues";
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public ICommand ToggleSelectionCommand { get; private set; }
        public ICommand LoadMoreCommand { get; private set; }

        private bool _isLoadingMore;
        public bool IsLoadingMore
        {
            get => _isLoadingMore;
            set
            {
                if (_isLoadingMore != value)
                {
                    _isLoadingMore = value;
                    OnPropertyChanged(nameof(IsLoadingMore));
                }
            }
        }

        public SpotPage()
        {
            LoadDataCommand = new Command(async () => await InitializeAndLoadDataAsync());
            DeleteSingleCommand = new Command<EstabelecimentoListItemDto>(OnDeleteSingleItem);
            OpenItemCommand = new Command<EstabelecimentoListItemDto>(OnOpenItem);
            ToggleSelectionCommand = new Command<EstabelecimentoListItemDto>(OnToggleSelection);
            LoadMoreCommand = new Command(async () => await LoadMoreItemsAsync());

            InitializeComponent();

            Locais = new ObservableCollection<EstabelecimentoListItemDto>();
            locaisCollectionView.ItemsSource = Locais;
            this.BindingContext = this;
            SelectionCount = 0;

            UpdateUIState();
        }

        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();

            if (Handler != null)
            {
                try
                {
                    if (LoadDataCommand == null)
                    {
                        LoadDataCommand = new Command(async () => await InitializeAndLoadDataAsync());
                        OnPropertyChanged(nameof(LoadDataCommand));
                    }

                    var headerComponent = this.FindByName<HeaderComponent>("headerComponent");
                    if (headerComponent != null)
                    {
                        headerComponent.ConfigureSafeBackNavigation(null, 500);
                    }

                    EnsureEstabelecimentoService();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ SpotPage: Erro em OnHandlerChanged: {ex.Message}");
                }
            }
        }

        private void EnsureEstabelecimentoService()
        {
            try
            {
                if (_estabelecimentoService == null && Handler != null)
                {
                    var serviceProvider = new ServiceProvider(this.Handler.MauiContext.Services);
                    _estabelecimentoService = serviceProvider.GetService<IEstabelecimentoService>();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SpotPage: Erro ao inicializar EstabelecimentoService: {ex.Message}");
            }
        }

        // ... (Preservado OnAppearingBypass e InitializeAndLoadDataAsync) ...
        private async Task OnAppearingBypass()
        {
            await this.ExecuteListPageBypass();
        }

        private async Task InitializeAndLoadDataAsync()
        {
            try
            {
                MainThread.BeginInvokeOnMainThread(() => SelectionCount = 0);
                await EnsureHandlerAndServiceAvailable();
                await LoadLocaisAsync();
                MainThread.BeginInvokeOnMainThread(() => SelectionCount = 0);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SpotPage: Erro em InitializeAndLoadDataAsync: {ex.Message}");
                MainThread.BeginInvokeOnMainThread(() => UpdateUIState());
            }
        }

        private async Task EnsureHandlerAndServiceAvailable()
        {
            int attempts = 0;
            const int maxAttempts = 20;

            while (attempts < maxAttempts)
            {
                if (Handler != null && _estabelecimentoService != null) return;
                if (Handler != null && _estabelecimentoService == null)
                {
                    try
                    {
                        var serviceProvider = new ServiceProvider(this.Handler.MauiContext.Services);
                        _estabelecimentoService = serviceProvider.GetService<IEstabelecimentoService>();
                        if (_estabelecimentoService != null) return;
                    }
                    catch (Exception) { }
                }
                await Task.Delay(100);
                attempts++;
            }
        }

        private CancellationTokenSource _searchCts;
        private CancellationTokenSource _loadMoreCts;

        // ...

        private async Task LoadLocaisAsync()
        {
            if (_estabelecimentoService == null) return;
            try
            {
                // Reset pagination state
                _currentPage = 1;
                _hasMoreItems = true;
                _currentSearchQuery = null;

                // Load first page using pagination
                var (items, totalCount) = await _estabelecimentoService.GetPagedEstabelecimentosForListAsync(
                    _currentPage,
                    PaginationSettings.PageSize,
                    null);

                _totalCount = totalCount;

                Locais.Clear();

                if (items != null)
                {
                    foreach (var item in items)
                    {
                        Locais.Add(item);
                    }

                    // Check if there are more items to load
                    _hasMoreItems = Locais.Count < _totalCount;
                }

                MainThread.BeginInvokeOnMainThread(() => UpdateUIState());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SpotPage: Error loading venues: {ex.Message}");
                MainThread.BeginInvokeOnMainThread(() => UpdateUIState());
            }
        }

        #region Search Logic

        private async void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            await PerformSearchAsync(e.NewTextValue);
        }

        private async Task PerformSearchAsync(string query)
        {
            // Cancel previous search if typing continues
            Interlocked.Exchange(ref _searchCts, new CancellationTokenSource())?.Cancel();
            var cts = _searchCts;

            try
            {
                await Task.Delay(300, cts.Token); // Debounce 300ms

                if (_estabelecimentoService == null) return;

                // Reset pagination for new search
                _currentPage = 1;
                _currentSearchQuery = query;

                var (items, totalCount) = await _estabelecimentoService.GetPagedEstabelecimentosForListAsync(
                    _currentPage,
                    PaginationSettings.PageSize,
                    string.IsNullOrWhiteSpace(query) ? null : query);

                if (cts.Token.IsCancellationRequested) return;

                _totalCount = totalCount;

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Locais.Clear();
                    foreach (var item in items)
                    {
                        Locais.Add(item);
                    }

                    // Check if there are more items to load
                    _hasMoreItems = Locais.Count < _totalCount;

                    UpdateUIState();
                });
            }
            catch (OperationCanceledException)
            {
                // Expected when typing fast
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SpotPage: Search Error: {ex.Message}");
            }
        }

        private void OnSearchButtonPressed(object sender, EventArgs e)
        {
            // Opcional: Esconder teclado
            if (sender is SearchBar sb) sb.Unfocus();
        }

        // private void FilterList(string query) ... REMOVED

        #endregion

        private void UpdateUIState()
        {
            try
            {
                VisualElement emptyStateFrame = this.FindByName<VisualElement>("emptyStateFrame");
                VisualElement addFab = this.FindByName<VisualElement>("addFab");

                bool hasLocais = Locais.Any();

                if (emptyStateFrame != null) emptyStateFrame.IsVisible = !hasLocais;
                locaisCollectionView.IsVisible = hasLocais;

                var currentSelection = Locais.Count(x => x.IsSelected);
                if (SelectionCount != currentSelection) SelectionCount = currentSelection;

                // FAB Logic: Visible only when NO selection
                bool shouldShowFab = SelectionCount == 0;
                if (addFab != null) addFab.IsVisible = shouldShowFab;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERRO em UpdateUIState: {ex.Message}");
            }
        }

        #region Interaction Handlers
        
        // Comandos expostos para Binding no XAML
        public ICommand OpenItemCommand { get; private set; }

        private async void OnAddFabClicked(object sender, EventArgs e)
        {
            await NavigateToSpotFormPageAsync(isEditing: false, editingLocal: null);
        }

        private async void OnOpenItem(EstabelecimentoListItemDto item)
        {
            if (item == null) return;

            if (SelectionCount > 0)
            {
                // Se já estiver em modo de seleção, o Tap funciona como Toggle
                OnToggleSelection(item);
            }
            else
            {
                // Navegação normal (Abrir Detalhes/Edição)
                // TODO: Futuramente separar ViewDetails de Edit. Por enquanto, abre o Form.
                var entity = EstabelecimentoMapper.ToEntity(item);
                await NavigateToSpotFormPageAsync(isEditing: true, editingLocal: entity);
            }
        }

        private void OnToggleSelection(EstabelecimentoListItemDto item)
        {
            if (item == null) return;

            try
            {
                HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            }
            catch { /* Ignorar erros de haptics em dispositivos não suportados */ }

            item.IsSelected = !item.IsSelected;
            // Notificar a mudança para a UI (se objeto não implementar INotifyPropertyChanged corretamente, 
            // a CollectionView pode precisar de um refresh manual, mas DTOs costumam ter).
            // No caso do DTO não ser observável, forçamos a atualização da view se necessário.
            
            SelectionCount = Locais.Count(x => x.IsSelected);
            UpdateUIState();
        }

        private async void OnCrudNavBarButtonClicked(object sender, CrudButtonType buttonType)
        {
            var selectedItems = Locais.Where(x => x.IsSelected).ToList();

            switch (buttonType)
            {
                case CrudButtonType.Editar:
                    if (selectedItems.Count == 1)
                    {
                        var entity = EstabelecimentoMapper.ToEntity(selectedItems.First());
                        await NavigateToSpotFormPageAsync(isEditing: true, editingLocal: entity);
                    }
                    break;

                case CrudButtonType.Excluir:
                    await ConfirmAndDeleteAsync(selectedItems);
                    break;
            }
        }

        // ✅ NOVO: Handler para o comando de Swipe (DeleteSingleCommand)
        private async void OnDeleteSingleItem(EstabelecimentoListItemDto item)
        {
            if (item == null) return;

            // 1. Confirmação usando MD3 Popup
            string name = item.Nome;
            var popup = new ConfirmationPopup("Confirm Deletion",
                $"Are you sure you want to delete '{name}'?", "Delete", "Cancel");

            var confirmed = await popup.ShowAsync();
            if (!confirmed) return;

            // 2. Executa a exclusão (reaproveitando a lógica central)
            var list = new List<EstabelecimentoListItemDto> { item };
            await ExecuteDeletionAsync(list);
        }

        #endregion

        #region Deletion Logic (Refactored)

        private async Task ExecuteDeletionAsync(List<EstabelecimentoListItemDto> items)
        {
            await GlobalLoadingOverlay.ShowLoadingAsync("Deleting...");
            try
            {
                var ids = items.Select(x => x.Id);
                var result = await _estabelecimentoService.DeleteEstabelecimentosAsync(ids);

                await LoadLocaisAsync(); // Atualiza a lista

                // Limpa o estado da seleção
                foreach (var item in Locais) item.IsSelected = false;
                SelectionCount = 0;

                await GlobalLoadingOverlay.HideLoadingAsync();
                await GlobalSnackbar.ShowSuccessAsync(result.message);

                MainThread.BeginInvokeOnMainThread(() => UpdateUIState());
            }
            catch (Exception ex)
            {
                await GlobalLoadingOverlay.HideLoadingAsync();
                await GlobalSnackbar.ShowErrorAsync($"Error: {ex.Message}");
            }
        }

        private async Task ConfirmAndDeleteAsync(List<EstabelecimentoListItemDto> itemsToDelete)
        {
            if (!itemsToDelete.Any()) return;

            try
            {
                var itemsWithEvents = itemsToDelete.Where(l => l.HasEvents).ToList();
                var itemsWithoutEvents = itemsToDelete.Where(l => !l.HasEvents).ToList();

                string confirmMessage;
                string confirmTitle;

                if (itemsWithEvents.Any())
                {
                    var count = itemsWithEvents.Count;
                    var infoMessage = $"{(count == 1 ? "The selected venue has" : $"The {count} selected venues have")} registered events and cannot be deleted.";
                    await DisplayAlert("Deletion Blocked", infoMessage, "OK");
                    return;
                }
                else
                {
                    var count = itemsWithoutEvents.Count;
                    confirmTitle = "Confirm Deletion";
                    confirmMessage = count == 1
                        ? "Are you sure you want to delete 1 venue?"
                        : $"Are you sure you want to delete {count} selected venues?";
                }

                var popup = new ConfirmationPopup(confirmTitle, confirmMessage, "Delete", "Cancel");
                var confirmed = await popup.ShowAsync();
                if (!confirmed) return;

                await ExecuteDeletionAsync(itemsWithoutEvents);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error confirming deletion: {ex.Message}", "OK");
            }
        }

        #endregion

        #region Navigation Helpers

        private async Task NavigateToSpotFormPageAsync(bool isEditing, Estabelecimento editingLocal = null)
        {
            try
            {
                var spotFormBehavior = this.Behaviors?.OfType<SafeNavigationBehavior>()
                    .FirstOrDefault(b => b.TargetPageType == typeof(SpotFormPage));

                if (spotFormBehavior != null)
                {
                    spotFormBehavior.CreatePageFunc = () =>
                    {
                        var spotFormPage = new SpotFormPage();
                        if (isEditing && editingLocal != null) spotFormPage.ConfigureForEditing(editingLocal);
                        else spotFormPage.ConfigureForAdding();
                        return spotFormPage;
                    };

                    await spotFormBehavior.NavigateToPageAsync();
                }
                else
                {
                    // Fallback
                    var spotFormPage = new SpotFormPage();
                    if (isEditing && editingLocal != null) spotFormPage.ConfigureForEditing(editingLocal);
                    else spotFormPage.ConfigureForAdding();
                    await Navigation.PushAsync(spotFormPage);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SpotPage: Erro na navegação segura/fallback: {ex.Message}");
                await DisplayAlert("Navigation Error", "Could not open form page.", "OK");
            }
        }

        #endregion

        private async Task LoadMoreItemsAsync()
        {
            // Prevent concurrent load more requests
            if (IsLoadingMore || !_hasMoreItems || _estabelecimentoService == null) return;

            // Cancel any previous load more operation
            Interlocked.Exchange(ref _loadMoreCts, new CancellationTokenSource())?.Cancel();
            var cts = _loadMoreCts;

            try
            {
                IsLoadingMore = true;

                // Small debounce to prevent rapid-fire requests during fast scrolling
                await Task.Delay(PaginationSettings.LoadMoreDebounceMs, cts.Token);

                if (cts.Token.IsCancellationRequested) return;

                // Load next page
                _currentPage++;

                var (items, totalCount) = await _estabelecimentoService.GetPagedEstabelecimentosForListAsync(
                    _currentPage,
                    PaginationSettings.PageSize,
                    _currentSearchQuery);

                if (cts.Token.IsCancellationRequested)
                {
                    _currentPage--; // Rollback page increment
                    return;
                }

                _totalCount = totalCount;

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (items != null)
                    {
                        foreach (var item in items)
                        {
                            Locais.Add(item);
                        }

                        // Check if there are more items
                        _hasMoreItems = Locais.Count < _totalCount;

                        // Memory management: Remove old items if limit exceeded
                        if (Locais.Count > PaginationSettings.MaxItemsInMemory)
                        {
                            var itemsToRemove = Locais.Count - PaginationSettings.MaxItemsInMemory;
                            for (int i = 0; i < itemsToRemove; i++)
                            {
                                Locais.RemoveAt(0); // Remove from top (oldest)
                            }
                            System.Diagnostics.Debug.WriteLine($"✂️ SpotPage: Trimmed {itemsToRemove} old items to keep memory under {PaginationSettings.MaxItemsInMemory} items");
                        }
                    }

                    UpdateUIState();
                });
            }
            catch (OperationCanceledException)
            {
                _currentPage--; // Rollback page increment
            }
            catch (Exception ex)
            {
                _currentPage--; // Rollback page increment
                System.Diagnostics.Debug.WriteLine($"❌ SpotPage: Error loading more items: {ex.Message}");
            }
            finally
            {
                IsLoadingMore = false;
            }
        }

        private void SetLoading(bool isLoading)
        {
            // Note: Loading is primarily handled by GlobalLoadingOverlay, 
            // but this method can be kept for local visual feedback if needed.
            locaisCollectionView.IsVisible = !isLoading && Locais.Any();
            VisualElement emptyStateFrame = this.FindByName<VisualElement>("emptyStateFrame");
            if (emptyStateFrame != null) emptyStateFrame.IsVisible = !isLoading && !Locais.Any();
        }

    }
}