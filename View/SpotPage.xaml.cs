using MyKaraoke.Domain;
using MyKaraoke.Services;
using MyKaraoke.View.Components;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace MyKaraoke.View
{
    public partial class SpotPage : ContentPage, INotifyPropertyChanged
    {
        private IEstabelecimentoService _estabelecimentoService;

        public ObservableCollection<Estabelecimento> Locais { get; }

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

        public SpotPage()
        {
            // A primeira linha DEVE ser InitializeComponent() para carregar os elementos do XAML.
            InitializeComponent();

            Locais = new ObservableCollection<Estabelecimento>();
            locaisCollectionView.ItemsSource = Locais; // Associa a coleção à UI
            this.BindingContext = this;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _ = InitializeAndLoadDataAsync();
        }

        private async Task InitializeAndLoadDataAsync()
        {
            SetLoading(true);
            try
            {
                if (_estabelecimentoService == null)
                {
                    var serviceProvider = new ServiceProvider(this.Handler.MauiContext.Services);
                    _estabelecimentoService = serviceProvider.GetService<IEstabelecimentoService>();
                }
                await LoadLocaisAsync();
            }
            finally
            {
                SetLoading(false);
            }
        }

        private async Task LoadLocaisAsync()
        {
            if (_estabelecimentoService == null) return;

            var locais = await _estabelecimentoService.GetAllEstabelecimentosAsync();

            Locais.Clear();
            if (locais != null)
            {
                foreach (var local in locais)
                {
                    Locais.Add(local);
                }
            }

            MainThread.BeginInvokeOnMainThread(UpdateUIState);
        }

        private void UpdateUIState()
        {
            bool hasLocais = Locais.Any();
            emptyStateFrame.IsVisible = !hasLocais;
            locaisCollectionView.IsVisible = hasLocais;
            SelectionCount = 0;
        }

        private void OnLocalSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectionCount = e.CurrentSelection.Count;
        }

        private async void OnCrudNavBarButtonClicked(object sender, CrudButtonType buttonType)
        {
            var selectedItems = locaisCollectionView.SelectedItems.Cast<Estabelecimento>().ToList();

            switch (buttonType)
            {
                case CrudButtonType.Adicionar:
                    await NavigateToSpotFormPageAsync(isEditing: false);
                    break;
                case CrudButtonType.Editar:
                    if (selectedItems.Count == 1)
                    {
                        await NavigateToSpotFormPageAsync(isEditing: true, editingLocal: selectedItems.First());
                    }
                    break;
                case CrudButtonType.Excluir:
                    await ConfirmAndDeleteAsync(selectedItems);
                    break;
            }
        }

        private async Task ConfirmAndDeleteAsync(List<Estabelecimento> itemsToDelete)
        {
            if (!itemsToDelete.Any()) return;

            var confirm = await DisplayAlert("Confirmar Exclusão", $"Tem certeza que deseja excluir {itemsToDelete.Count} local(is)?", "Excluir", "Cancelar");
            if (!confirm) return;

            SetLoading(true);
            try
            {
                var result = await _estabelecimentoService.DeleteEstabelecimentosAsync(itemsToDelete.Select(i => i.Id));
                await DisplayAlert("Resultado da Exclusão", result.message, "OK");
                await LoadLocaisAsync();
            }
            finally
            {
                SetLoading(false);
            }
        }

        private async Task NavigateToSpotFormPageAsync(bool isEditing, Estabelecimento editingLocal = null)
        {
            var spotFormPage = new SpotFormPage();
            if (isEditing && editingLocal != null)
            {
                spotFormPage.ConfigureForEditing(editingLocal);
            }
            else
            {
                spotFormPage.ConfigureForAdding();
            }
            await Navigation.PushAsync(spotFormPage);
        }

        private void SetLoading(bool isLoading)
        {
            loadingOverlay.IsVisible = isLoading;
            locaisCollectionView.IsVisible = !isLoading && Locais.Any();
            emptyStateFrame.IsVisible = !isLoading && !Locais.Any();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}