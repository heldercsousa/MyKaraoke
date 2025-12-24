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
using Serilog;
using Microsoft.Extensions.Options;

namespace MyVocaList.View
{
    public partial class SpotPage : ContentPage, IManipulableDataPage
    {
        private static readonly Serilog.ILogger Logger = Log.ForContext<SpotPage>();

        private IEstabelecimentoService _estabelecimentoService;
        private PaginationSettings _paginationSettings;
        public ObservableCollection<EstabelecimentoListItemDto> Locais { get; }

        // Pagination state
        private int _currentPage = 1;
        private int _totalCount = 0;
        private bool _hasMoreItems = true;
        private string _currentSearchQuery = null;
        private int _firstLoadedItemIndex = 1; // Track the index of the first item in memory (1-based)

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

        private int _totalItemsCount;
        public int TotalItemsCount
        {
            get => _totalItemsCount;
            set
            {
                if (_totalItemsCount != value)
                {
                    _totalItemsCount = value;
                    OnPropertyChanged(nameof(TotalItemsCount));
                }
            }
        }

        private bool _isSearching;
        public bool IsSearching
        {
            get => _isSearching;
            set
            {
                if (_isSearching != value)
                {
                    _isSearching = value;
                    OnPropertyChanged(nameof(IsSearching));
                }
            }
        }

        public string VenuesCountText
        {
            get
            {
                if (TotalItemsCount == 0)
                    return string.Empty;

                if (IsSearching)
                {
                    var count = Locais.Count;
                    return count == 1 ? "1 result" : $"{count} results";
                }

                if (Locais.Count < TotalItemsCount)
                {
                    // Show range when memory cleanup is active (first index > 1)
                    if (_firstLoadedItemIndex > 1)
                    {
                        var lastIndex = _firstLoadedItemIndex + Locais.Count - 1;
                        return $"{_firstLoadedItemIndex} to {lastIndex} of {TotalItemsCount}";
                    }
                    // Show simple count for initial loading
                    return $"{Locais.Count} of {TotalItemsCount}";
                }

                var total = TotalItemsCount;
                return total == 1 ? "1 venue" : $"{total} venues";
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
        public ICommand PerformSearchCommand { get; private set; }

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
            PerformSearchCommand = new Command<string>(async (query) => await PerformSearchAsync(query));

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
                Logger.Debug("Handler initialized");

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
        }

        private void EnsureEstabelecimentoService()
        {
            if (_estabelecimentoService == null && Handler != null)
            {
                Logger.Debug("Initializing EstabelecimentoService");

                var serviceProvider = new ServiceProvider(this.Handler.MauiContext.Services);
                _estabelecimentoService = serviceProvider.GetService<IEstabelecimentoService>();

                // Get pagination settings
                var paginationOptions = serviceProvider.GetService<IOptions<PaginationSettings>>();
                _paginationSettings = paginationOptions?.Value ?? new PaginationSettings();

                Logger.Debug("EstabelecimentoService initialized successfully");
            }
        }

        // ... (Preservado OnAppearingBypass e InitializeAndLoadDataAsync) ...
        private async Task OnAppearingBypass()
        {
            await this.ExecuteListPageBypass();
        }

        private async Task InitializeAndLoadDataAsync()
        {
            Logger.Debug("Starting data initialization and load");

            MainThread.BeginInvokeOnMainThread(() => SelectionCount = 0);
            await EnsureHandlerAndServiceAvailable();
            await LoadLocaisAsync();
            MainThread.BeginInvokeOnMainThread(() => SelectionCount = 0);

            Logger.Debug("Data initialization and load completed");
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
                    var serviceProvider = new ServiceProvider(this.Handler.MauiContext.Services);
                    _estabelecimentoService = serviceProvider.GetService<IEstabelecimentoService>();
                    if (_estabelecimentoService != null) return;
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
            if (_estabelecimentoService == null)
            {
                Logger.Warning("Cannot load venues - EstabelecimentoService is null");
                return;
            }

            Logger.Debug("Loading venues - first page");

            // Reset pagination state
            _currentPage = 1;
            _hasMoreItems = true;
            _currentSearchQuery = null;
            _firstLoadedItemIndex = 1; // Reset to first item

            // Load first page using pagination
            var (items, totalCount) = await _estabelecimentoService.GetPagedEstabelecimentosForListAsync(
                _currentPage,
                _paginationSettings.PageSize,
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

            MainThread.BeginInvokeOnMainThread(() =>
            {
                TotalItemsCount = _totalCount; // Update total items count for binding
                OnPropertyChanged(nameof(VenuesCountText));
                IsSearching = false; // Clear search state when loading all venues
                UpdateUIState();
            });

            Logger.Debug("Loaded {Count} venues out of {TotalCount}", Locais.Count, _totalCount);
        }

        #region Search Logic

        private async Task PerformSearchAsync(string query)
        {
            // Cancel previous search if typing continues
            Interlocked.Exchange(ref _searchCts, new CancellationTokenSource())?.Cancel();
            var cts = _searchCts;

            try
            {
                await Task.Delay(300, cts.Token); // Debounce 300ms

                if (_estabelecimentoService == null)
                {
                    Logger.Warning("Cannot perform search - EstabelecimentoService is null");
                    return;
                }

                // Reset pagination for new search
                _currentPage = 1;
                _currentSearchQuery = query;
                _firstLoadedItemIndex = 1; // Reset to first item

                // Track whether we're actively searching
                var isActiveSearch = !string.IsNullOrWhiteSpace(query);

                Logger.Debug("Performing search with query: {Query}", query ?? "(empty)");

                var (items, totalCount) = await _estabelecimentoService.GetPagedEstabelecimentosForListAsync(
                    _currentPage,
                    _paginationSettings.PageSize,
                    isActiveSearch ? query : null);

                if (cts.Token.IsCancellationRequested) return;

                _totalCount = totalCount;

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    // Update search state BEFORE updating the list
                    IsSearching = isActiveSearch;

                    Locais.Clear();
                    foreach (var item in items)
                    {
                        Locais.Add(item);
                    }

                    // Check if there are more items to load
                    _hasMoreItems = Locais.Count < _totalCount;

                    TotalItemsCount = _totalCount; // Update total items count for binding
                    OnPropertyChanged(nameof(VenuesCountText));

                    UpdateUIState();
                });

                Logger.Debug("Search completed - found {Count} results out of {TotalCount}", Locais.Count, _totalCount);
            }
            catch (OperationCanceledException)
            {
                // Expected when typing fast - user is still typing
                Logger.Debug("Search cancelled - user still typing");
            }
        }

        #endregion

        private void UpdateUIState()
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

            Logger.Debug("UI state updated - HasVenues: {HasVenues}, SelectionCount: {SelectionCount}", hasLocais, SelectionCount);
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
            if (!itemsToDelete.Any())
            {
                Logger.Debug("No items to delete");
                return;
            }

            Logger.Debug("Confirming deletion of {Count} items", itemsToDelete.Count);

            var itemsWithEvents = itemsToDelete.Where(l => l.HasEvents).ToList();
            var itemsWithoutEvents = itemsToDelete.Where(l => !l.HasEvents).ToList();

            string confirmMessage;
            string confirmTitle;

            if (itemsWithEvents.Any())
            {
                var count = itemsWithEvents.Count;
                var infoMessage = $"{(count == 1 ? "The selected venue has" : $"The {count} selected venues have")} registered events and cannot be deleted.";
                Logger.Debug("Deletion blocked - {Count} venues have events", count);
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
            if (!confirmed)
            {
                Logger.Debug("Deletion cancelled by user");
                return;
            }

            await ExecuteDeletionAsync(itemsWithoutEvents);
        }

        #endregion

        #region Navigation Helpers

        private async Task NavigateToSpotFormPageAsync(bool isEditing, Estabelecimento editingLocal = null)
        {
            Logger.Debug("Navigating to SpotFormPage - IsEditing: {IsEditing}", isEditing);

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
                Logger.Debug("SafeNavigationBehavior not found - using fallback navigation");

                // Fallback
                var spotFormPage = new SpotFormPage();
                if (isEditing && editingLocal != null) spotFormPage.ConfigureForEditing(editingLocal);
                else spotFormPage.ConfigureForAdding();
                await Navigation.PushAsync(spotFormPage);
            }

            Logger.Debug("Navigation to SpotFormPage completed");
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
                await Task.Delay(_paginationSettings.LoadMoreDebounceMs, cts.Token);

                if (cts.Token.IsCancellationRequested) return;

                // Load next page
                _currentPage++;

                Logger.Debug("Loading more items - Page {PageNumber}", _currentPage);

                var (items, totalCount) = await _estabelecimentoService.GetPagedEstabelecimentosForListAsync(
                    _currentPage,
                    _paginationSettings.PageSize,
                    _currentSearchQuery);

                if (cts.Token.IsCancellationRequested)
                {
                    _currentPage--; // Rollback page increment
                    Logger.Debug("Load more cancelled - rolling back to page {PageNumber}", _currentPage);
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
                        if (Locais.Count > _paginationSettings.MaxItemsInMemory)
                        {
                            var itemsToRemove = Locais.Count - _paginationSettings.MaxItemsInMemory;
                            for (int i = 0; i < itemsToRemove; i++)
                            {
                                Locais.RemoveAt(0); // Remove from top (oldest)
                            }

                            // Update first loaded item index to reflect removed items
                            _firstLoadedItemIndex += itemsToRemove;

                            Logger.Debug("Trimmed {ItemsRemoved} old items to keep memory under {MaxItems} items. First index now: {FirstIndex}",
                                itemsToRemove, _paginationSettings.MaxItemsInMemory, _firstLoadedItemIndex);
                        }
                    }

                    TotalItemsCount = _totalCount; // Update total items count for binding
                    OnPropertyChanged(nameof(VenuesCountText));

                    UpdateUIState();
                });

                Logger.Debug("Loaded more items - Page {PageNumber} completed", _currentPage);
            }
            catch (OperationCanceledException)
            {
                _currentPage--; // Rollback page increment
                Logger.Debug("Load more cancelled - rolling back to page {PageNumber}", _currentPage);
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