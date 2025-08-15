using MyKaraoke.Domain;
using MyKaraoke.Services;
using MyKaraoke.View.Components;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyKaraoke.View
{
    public partial class SpotPage : ContentPage, INotifyPropertyChanged
    {
        private IEstabelecimentoService _estabelecimentoService;
        public ObservableCollection<Estabelecimento> Locais { get; }

        // Propriedade que o CrudNavBarComponent observa
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

        // Comando que o PageLifecycleBehavior irá executar quando a página aparecer
        public ICommand LoadDataCommand { get; }

        public SpotPage()
        {
            // ✅ CRÍTICO: Inicializa LoadDataCommand PRIMEIRO, antes de qualquer coisa
            LoadDataCommand = new Command(async () => await InitializeAndLoadDataAsync());

            System.Diagnostics.Debug.WriteLine($"✅ SpotPage: LoadDataCommand criado PRIMEIRO - CanExecute: {LoadDataCommand?.CanExecute(null)}");

            InitializeComponent();

            Locais = new ObservableCollection<Estabelecimento>();
            locaisCollectionView.ItemsSource = Locais;

            // ✅ CRÍTICO: Define BindingContext DEPOIS do LoadDataCommand
            this.BindingContext = this;

            // ✅ INICIAL: Define SelectionCount inicial (para garantir que CrudNavBar tenha algo para trabalhar)
            SelectionCount = 0;

            System.Diagnostics.Debug.WriteLine($"✅ SpotPage: Construtor concluído");
            System.Diagnostics.Debug.WriteLine($"   - LoadDataCommand: {LoadDataCommand != null}");
            System.Diagnostics.Debug.WriteLine($"   - LoadDataCommand.CanExecute: {LoadDataCommand?.CanExecute(null)}");
            System.Diagnostics.Debug.WriteLine($"   - BindingContext: {this.BindingContext != null}");
            System.Diagnostics.Debug.WriteLine($"   - SelectionCount inicial: {SelectionCount}");

            // ✅ GARANTE estado inicial correto
            UpdateUIState();

            // ✅ VERIFICAÇÃO FINAL: Confirma que comando está disponível
            System.Diagnostics.Debug.WriteLine($"✅ SpotPage: Verificação final - LoadDataCommand executável: {LoadDataCommand?.CanExecute(null)}");
        }

        // ✅ CORRIGIDO: Este método é chamado pelo PageLifecycleBehavior
        private async Task InitializeAndLoadDataAsync()
        {
            System.Diagnostics.Debug.WriteLine("✅ SpotPage: InitializeAndLoadDataAsync INICIADO");

            try
            {
                // ✅ FORÇA SelectionCount=0 NO INÍCIO (garante botão Adicionar)
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    SelectionCount = 0;
                    OnPropertyChanged(nameof(SelectionCount));
                    System.Diagnostics.Debug.WriteLine("✅ SpotPage: SelectionCount=0 forçado no INÍCIO do InitializeAndLoadDataAsync");
                });

                if (_estabelecimentoService == null)
                {
                    var serviceProvider = new ServiceProvider(this.Handler.MauiContext.Services);
                    _estabelecimentoService = serviceProvider.GetService<IEstabelecimentoService>();
                    System.Diagnostics.Debug.WriteLine($"✅ SpotPage: EstabelecimentoService obtido: {_estabelecimentoService != null}");
                }

                await LoadLocaisAsync();

                // ✅ FORÇA SelectionCount=0 NO FINAL (garante que botão permaneça)
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    SelectionCount = 0;
                    OnPropertyChanged(nameof(SelectionCount));
                    System.Diagnostics.Debug.WriteLine("✅ SpotPage: SelectionCount=0 forçado no FINAL do InitializeAndLoadDataAsync");
                });

                System.Diagnostics.Debug.WriteLine("✅ SpotPage: InitializeAndLoadDataAsync CONCLUÍDO");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SpotPage: Erro em InitializeAndLoadDataAsync: {ex.Message}");

                // ✅ FALLBACK: Mesmo com erro, garante que CrudNavBar tenha botão Adicionar
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    SelectionCount = 0;
                    OnPropertyChanged(nameof(SelectionCount));
                    System.Diagnostics.Debug.WriteLine("✅ SpotPage: Fallback - SelectionCount=0 definido");
                });
            }
        }

        private async Task LoadLocaisAsync()
        {
            System.Diagnostics.Debug.WriteLine("✅ SpotPage: LoadLocaisAsync INICIADO");

            if (_estabelecimentoService == null)
            {
                System.Diagnostics.Debug.WriteLine("❌ SpotPage: EstabelecimentoService é NULL!");
                return;
            }

            try
            {
                var locais = await _estabelecimentoService.GetAllEstabelecimentosAsync();
                System.Diagnostics.Debug.WriteLine($"✅ SpotPage: Locais carregados do banco: {locais?.Count() ?? 0}");

                Locais.Clear();
                if (locais != null)
                {
                    foreach (var local in locais)
                    {
                        Locais.Add(local);
                    }
                }

                // ✅ CRÍTICO: Chama UpdateUIState no MainThread APÓS carregar dados
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    UpdateUIState();
                    System.Diagnostics.Debug.WriteLine($"✅ SpotPage: UpdateUIState chamado após carregar {Locais.Count} locais");
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SpotPage: Erro ao carregar locais: {ex.Message}");

                // ✅ FALLBACK: Mesmo com erro de banco, garante botão Adicionar
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    UpdateUIState();
                });
            }
        }

        private void UpdateUIState()
        {
            try
            {
                bool hasLocais = Locais.Any();
                emptyStateFrame.IsVisible = !hasLocais;
                locaisCollectionView.IsVisible = hasLocais;

                // ✅ CRÍTICO: Sempre SelectionCount=0 para mostrar botão "Adicionar"
                var previousSelection = SelectionCount;
                SelectionCount = 0;

                System.Diagnostics.Debug.WriteLine($"✅ SpotPage: UpdateUIState concluído");
                System.Diagnostics.Debug.WriteLine($"   - HasLocais: {hasLocais}");
                System.Diagnostics.Debug.WriteLine($"   - SelectionCount: {SelectionCount} (era {previousSelection})");
                System.Diagnostics.Debug.WriteLine($"   - emptyStateFrame.IsVisible: {emptyStateFrame.IsVisible}");
                System.Diagnostics.Debug.WriteLine($"   - locaisCollectionView.IsVisible: {locaisCollectionView.IsVisible}");

                // ✅ GARANTE: PropertyChanged sempre dispara (mesmo que valor seja igual)
                OnPropertyChanged(nameof(SelectionCount));
                System.Diagnostics.Debug.WriteLine($"✅ SpotPage: PropertyChanged(SelectionCount) disparado");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SpotPage: Erro em UpdateUIState: {ex.Message}");
            }
        }

        private void OnLocalSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectionCount = e.CurrentSelection.Count;
            System.Diagnostics.Debug.WriteLine($"✅ SpotPage: Seleção mudou - SelectionCount={SelectionCount}");
        }

        private async void OnCrudNavBarButtonClicked(object sender, CrudButtonType buttonType)
        {
            System.Diagnostics.Debug.WriteLine($"✅ SpotPage: Botão CrudNavBar clicado - {buttonType}");

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

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}