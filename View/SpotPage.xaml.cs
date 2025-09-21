using MyKaraoke.Domain;
using MyKaraoke.Services;
using MyKaraoke.View.Components;
using MyKaraoke.View.Extensions;
using MyKaraoke.View.Behaviors;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MyKaraoke.View.Interfaces;
using MyKaraoke.Contracts.DTOs.List;
using MyKaraoke.Services.Mappers;

namespace MyKaraoke.View
{
    public partial class SpotPage : ContentPage, IManipulableDataPage
    {
        private IEstabelecimentoService _estabelecimentoService;
        public ObservableCollection<EstabelecimentoListItemDto> Locais { get; }

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

        #region IManipulableDataPage Members 

        public ICommand LoadDataCommand { get; private set; }
        public string FriendlyName => "Locais";
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public SpotPage()
        {
            // ✅ EXATAMENTE como StackPage: LoadDataCommand ANTES do InitializeComponent
            LoadDataCommand = new Command(async () => await InitializeAndLoadDataAsync());

            System.Diagnostics.Debug.WriteLine($"🔧 SpotPage: LoadDataCommand criado ANTES do InitializeComponent: {LoadDataCommand != null}");

            // ✅ Agora o binding encontrará LoadDataCommand disponível
            InitializeComponent();

            // ✅ Resto da inicialização
            Locais = new ObservableCollection<EstabelecimentoListItemDto>();
            locaisCollectionView.ItemsSource = Locais;
            this.BindingContext = this;
            SelectionCount = 0;

            UpdateUIState();

            System.Diagnostics.Debug.WriteLine($"✅ SpotPage: Construtor concluído - LoadDataCommand: {LoadDataCommand != null}");
        }

        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();

            if (Handler != null)
            {
                try
                {
                    // ✅ VERIFICAÇÃO: LoadDataCommand ainda disponível
                    if (LoadDataCommand == null)
                    {
                        LoadDataCommand = new Command(async () => await InitializeAndLoadDataAsync());
                        OnPropertyChanged(nameof(LoadDataCommand));
                        System.Diagnostics.Debug.WriteLine($"🔧 SpotPage: LoadDataCommand recriado em OnHandlerChanged");
                    }

                    // Resto da configuração do HeaderComponent...
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
                    System.Diagnostics.Debug.WriteLine($"🔧 SpotPage: Inicializando EstabelecimentoService...");

                    var serviceProvider = new ServiceProvider(this.Handler.MauiContext.Services);
                    _estabelecimentoService = serviceProvider.GetService<IEstabelecimentoService>();

                    System.Diagnostics.Debug.WriteLine($"✅ SpotPage: EstabelecimentoService inicializado: {_estabelecimentoService != null}");

                    // ✅ FORÇA: LoadDataCommand estar disponível para SmartPageLifecycleBehavior
                    if (LoadDataCommand == null)
                    {
                        LoadDataCommand = new Command(async () => await InitializeAndLoadDataAsync());
                        OnPropertyChanged(nameof(LoadDataCommand));
                        System.Diagnostics.Debug.WriteLine($"✅ SpotPage: LoadDataCommand recriado após Handler disponível");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SpotPage: Erro ao inicializar EstabelecimentoService: {ex.Message}");
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        // ===== MÉTODO DE BYPASS PARA SMARTPAGELIFECYCLEBEHAVIOR =====

        /// <summary>
        /// 🎯 BYPASS: Método que o SmartPageLifecycleBehavior chamará automaticamente
        /// 🛡️ ESPECÍFICO: Lógica específica da SpotPage para contornar problemas
        /// </summary>
        private async Task OnAppearingBypass()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🎯 SpotPage: OnAppearingBypass executado - Hash: {this.GetHashCode()}");

                // ✅ SIMPLES: Usa extension method específico para SpotPage
                await this.ExecuteListPageBypass();

                System.Diagnostics.Debug.WriteLine($"✅ SpotPage: OnAppearingBypass concluído com sucesso");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SpotPage: Erro no OnAppearingBypass: {ex.Message}");

                // 🛡️ FALLBACK: Garante estado mínimo mesmo com erro
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    SelectionCount = 0;
                    OnPropertyChanged(nameof(SelectionCount));
                });
            }
        }

        // ===== MÉTODOS ORIGINAIS PRESERVADOS =====

        /// <summary>
        /// ✅ ORIGINAL: Este método é chamado pelo SmartPageLifecycleBehavior ou OnAppearingBypass
        /// </summary>

        private async Task InitializeAndLoadDataAsync()
        {
            System.Diagnostics.Debug.WriteLine($"✅ SpotPage ({this.GetHashCode()}): InitializeAndLoadDataAsync INICIADO");

            try
            {
                // ✅ FORÇA SelectionCount=0 NO INÍCIO
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    SelectionCount = 0;
                    OnPropertyChanged(nameof(SelectionCount));
                    System.Diagnostics.Debug.WriteLine($"✅ SpotPage ({this.GetHashCode()}): SelectionCount=0 forçado no INÍCIO");
                });

                // ✅ AGUARDA: Handler estar disponível se ainda não estiver
                await EnsureHandlerAndServiceAvailable();

                // ✅ CARREGA: Dados do banco
                await LoadLocaisAsync();

                // ✅ FORÇA SelectionCount=0 NO FINAL
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    SelectionCount = 0;
                    OnPropertyChanged(nameof(SelectionCount));
                    System.Diagnostics.Debug.WriteLine($"✅ SpotPage ({this.GetHashCode()}): SelectionCount=0 forçado no FINAL");
                });

                System.Diagnostics.Debug.WriteLine($"✅ SpotPage ({this.GetHashCode()}): InitializeAndLoadDataAsync CONCLUÍDO");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SpotPage ({this.GetHashCode()}): Erro em InitializeAndLoadDataAsync: {ex.Message}");

                // ✅ FALLBACK: Mesmo com erro, garante que CrudNavBar tenha botão Adicionar
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    SelectionCount = 0;
                    OnPropertyChanged(nameof(SelectionCount));
                    UpdateUIState();
                    System.Diagnostics.Debug.WriteLine($"✅ SpotPage ({this.GetHashCode()}): Fallback - SelectionCount=0 definido");
                });
            }
        }


        private async Task EnsureHandlerAndServiceAvailable()
        {
            int attempts = 0;
            const int maxAttempts = 20; // 20 x 100ms = 2 segundos

            while (attempts < maxAttempts)
            {
                if (Handler != null && _estabelecimentoService != null)
                {
                    System.Diagnostics.Debug.WriteLine($"✅ SpotPage: Handler e Service disponíveis após {attempts} tentativas");
                    return;
                }

                if (Handler != null && _estabelecimentoService == null)
                {
                    try
                    {
                        var serviceProvider = new ServiceProvider(this.Handler.MauiContext.Services);
                        _estabelecimentoService = serviceProvider.GetService<IEstabelecimentoService>();
                        System.Diagnostics.Debug.WriteLine($"✅ SpotPage: EstabelecimentoService obtido na tentativa {attempts}");

                        if (_estabelecimentoService != null)
                        {
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"❌ SpotPage: Erro ao obter service na tentativa {attempts}: {ex.Message}");
                    }
                }

                System.Diagnostics.Debug.WriteLine($"🔄 SpotPage: Aguardando Handler/Service - tentativa {attempts + 1}/{maxAttempts}");
                await Task.Delay(100);
                attempts++;
            }

            System.Diagnostics.Debug.WriteLine($"⚠️ SpotPage: Timeout aguardando Handler/Service - continuando mesmo assim");
        }

        private async Task LoadLocaisAsync()
        {
            System.Diagnostics.Debug.WriteLine($"✅ SpotPage ({this.GetHashCode()}): LoadLocaisAsync INICIADO");

            if (_estabelecimentoService == null)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SpotPage ({this.GetHashCode()}): EstabelecimentoService é NULL!");
                return;
            }

            try
            {
                var locaisViewModels = await _estabelecimentoService.GetAllEstabelecimentosForListAsync();
                System.Diagnostics.Debug.WriteLine($"🔍 LOAD RESULT: {locaisViewModels?.Count()} locais encontrados");

                Locais.Clear();
                if (locaisViewModels != null)
                {
                    foreach (var localViewModel in locaisViewModels)
                    {
                        Locais.Add(localViewModel);
                        System.Diagnostics.Debug.WriteLine($"🔍 ADDED: {localViewModel.Id} - '{localViewModel.Nome}' (HasEvents: {localViewModel.HasEvents})");
                    }
                }

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    UpdateUIState();
                    System.Diagnostics.Debug.WriteLine($"✅ SpotPage ({this.GetHashCode()}): UpdateUIState chamado após carregar {Locais.Count} locais");
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SpotPage ({this.GetHashCode()}): Erro ao carregar locais: {ex.Message}");
                MainThread.BeginInvokeOnMainThread(() => UpdateUIState());
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

                System.Diagnostics.Debug.WriteLine($"✅ SpotPage ({this.GetHashCode()}): UpdateUIState concluído");
                System.Diagnostics.Debug.WriteLine($"   - HasLocais: {hasLocais}");
                System.Diagnostics.Debug.WriteLine($"   - SelectionCount: {SelectionCount} (era {previousSelection})");
                System.Diagnostics.Debug.WriteLine($"   - emptyStateFrame.IsVisible: {emptyStateFrame.IsVisible}");
                System.Diagnostics.Debug.WriteLine($"   - locaisCollectionView.IsVisible: {locaisCollectionView.IsVisible}");

                // ✅ GARANTE: PropertyChanged sempre dispara (mesmo que valor seja igual)
                OnPropertyChanged(nameof(SelectionCount));
                System.Diagnostics.Debug.WriteLine($"✅ SpotPage ({this.GetHashCode()}): PropertyChanged(SelectionCount) disparado");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SpotPage ({this.GetHashCode()}): Erro em UpdateUIState: {ex.Message}");
            }
        }

        private void OnItemTapped(object sender, EventArgs e)
        {
            if (sender is Frame frame && frame.BindingContext is EstabelecimentoListItemDto item)
            {
                item.IsSelected = !item.IsSelected;
                SelectionCount = Locais.Count(x => x.IsSelected);
            }
        }

        private async void OnCrudNavBarButtonClicked(object sender, CrudButtonType buttonType)
        {
            System.Diagnostics.Debug.WriteLine($"✅ SpotPage ({this.GetHashCode()}): Botão CrudNavBar clicado - {buttonType}");

            var selectedItems = locaisCollectionView.SelectedItems.Cast<EstabelecimentoListItemDto>().ToList();

            switch (buttonType)
            {
                case CrudButtonType.Adicionar:
                    await NavigateToSpotFormPageAsync(isEditing: false);
                    break;
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

        private async Task ConfirmAndDeleteAsync(List<EstabelecimentoListItemDto> itemsToDelete)
        {
            if (!itemsToDelete.Any()) return;

            try
            {
                var itemsWithEvents = itemsToDelete.Where(l => l.HasEvents).ToList();
                var itemsWithoutEvents = itemsToDelete.Where(l => !l.HasEvents).ToList();

                string confirmMessage;
                if (itemsWithEvents.Any() && itemsWithoutEvents.Any())
                {
                    var withEventsNames = string.Join(", ", itemsWithEvents.Select(l => $"'{l.Nome}'"));
                    var withoutEventsNames = string.Join(", ", itemsWithoutEvents.Select(l => $"'{l.Nome}'"));
                    confirmMessage = $"ATENÇÃO:\n\n" +
                                   $"• Serão excluídos: {withoutEventsNames}\n" +
                                   $"• NÃO serão excluídos (possuem eventos): {withEventsNames}\n\n" +
                                   $"Deseja continuar?";
                }
                else if (itemsWithEvents.Any())
                {
                    var names = string.Join(", ", itemsWithEvents.Select(l => $"'{l.Nome}'"));
                    await DisplayAlert("Exclusão Bloqueada",
                        $"Os locais {names} não podem ser excluídos pois possuem eventos registrados.", "OK");
                    return;
                }
                else
                {
                    var names = string.Join(", ", itemsWithoutEvents.Select(l => $"'{l.Nome}'"));
                    confirmMessage = $"Tem certeza que deseja excluir {names}?";
                }

                var confirmed = await DisplayAlert("Confirmar Exclusão", confirmMessage, "Excluir", "Cancelar");
                if (!confirmed) return;

                SetLoading(true);

                var idsToDelete = itemsToDelete.Select(vm => vm.Id);
                var result = await _estabelecimentoService.DeleteEstabelecimentosAsync(idsToDelete);
                await DisplayAlert("Resultado", result.message, "OK");

                await LoadLocaisAsync();

                locaisCollectionView.SelectedItems.Clear();
                SelectionCount = 0;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro ao excluir locais: {ex.Message}", "OK");
            }
            finally
            {
                SetLoading(false);
            }
        }

        // ===== NAVEGAÇÃO SEGURA COM SAFENAVIGATIONBEHAVIOR =====

        /// <summary>
        /// 🎯 NAVEGAÇÃO SEGURA: Substitui NavigateToSpotFormPageAsync usando SafeNavigationBehavior
        /// </summary>
        private async Task NavigateToSpotFormPageAsync(bool isEditing, Estabelecimento editingLocal = null)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🎯 SpotPage: NavigateToSpotFormPageAsync - isEditing: {isEditing}");

                // 🎯 BUSCA: SafeNavigationBehavior no XAML
                var spotFormBehavior = this.Behaviors?.OfType<SafeNavigationBehavior>()
                    .FirstOrDefault(b => b.TargetPageType == typeof(SpotFormPage));

                if (spotFormBehavior != null)
                {
                    // ✅ CONFIGURAÇÃO: Função customizada para SpotFormPage
                    spotFormBehavior.CreatePageFunc = () =>
                    {
                        var spotFormPage = new SpotFormPage();

                        if (isEditing && editingLocal != null)
                        {
                            spotFormPage.ConfigureForEditing(editingLocal);
                            System.Diagnostics.Debug.WriteLine($"✅ SpotPage: SpotFormPage configurada para EDIÇÃO - {editingLocal.Nome}");
                        }
                        else
                        {
                            spotFormPage.ConfigureForAdding();
                            System.Diagnostics.Debug.WriteLine($"✅ SpotPage: SpotFormPage configurada para ADIÇÃO");
                        }

                        return spotFormPage;
                    };

                    // 🚀 NAVEGAÇÃO SEGURA
                    await spotFormBehavior.NavigateToPageAsync();
                    System.Diagnostics.Debug.WriteLine($"✅ SpotPage: Navegação para SpotFormPage via SafeNavigationBehavior concluída");
                }
                else
                {
                    // 🛡️ FALLBACK: Navegação tradicional se behavior não disponível
                    System.Diagnostics.Debug.WriteLine($"⚠️ SpotPage: SafeNavigationBehavior não encontrado - usando navegação tradicional");
                    await NavigateToSpotFormPageFallback(isEditing, editingLocal);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SpotPage: Erro na navegação segura: {ex.Message}");

                // 🛡️ FALLBACK: Tenta navegação tradicional em caso de erro
                try
                {
                    await NavigateToSpotFormPageFallback(isEditing, editingLocal);
                }
                catch (Exception fallbackEx)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ SpotPage: Erro no fallback de navegação: {fallbackEx.Message}");
                }
            }
        }

        /// <summary>
        /// 🛡️ FALLBACK: Navegação tradicional como backup
        /// </summary>
        private async Task NavigateToSpotFormPageFallback(bool isEditing, Estabelecimento editingLocal = null)
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
            locaisCollectionView.IsVisible = !isLoading && Locais.Any();
            emptyStateFrame.IsVisible = !isLoading && !Locais.Any();
        }

        // ===== MÉTODOS DE DIAGNÓSTICO (OPCIONAL) =====

        /// <summary>
        /// 📊 DIAGNÓSTICO: Método para debug e monitoramento específico da SpotPage
        /// </summary>
        public void LogSpotPageDiagnostics()
        {
            try
            {
                var diagnostics = this.GetPageDiagnostics();

                // Adiciona informações específicas da SpotPage
                diagnostics["LocaisCount"] = Locais?.Count ?? 0;
                diagnostics["SelectionCount"] = SelectionCount;
                diagnostics["EstabelecimentoServiceAvailable"] = _estabelecimentoService != null;
                diagnostics["EmptyStateVisible"] = emptyStateFrame?.IsVisible ?? false;
                diagnostics["CollectionViewVisible"] = locaisCollectionView?.IsVisible ?? false;

                System.Diagnostics.Debug.WriteLine($"📊 SpotPage: Diagnósticos específicos da página:");
                foreach (var kvp in diagnostics)
                {
                    System.Diagnostics.Debug.WriteLine($"   {kvp.Key}: {kvp.Value}");
                }

                // Diagnósticos específicos da CrudNavBar
                if (CrudNavBar != null)
                {
                    System.Diagnostics.Debug.WriteLine($"📊 SpotPage: CrudNavBar específico:");
                    System.Diagnostics.Debug.WriteLine($"   CrudNavBar.IsVisible: {CrudNavBar.IsVisible}");
                    System.Diagnostics.Debug.WriteLine($"   CrudNavBar.SelectionCount: {CrudNavBar.SelectionCount}");
                    System.Diagnostics.Debug.WriteLine($"   CrudNavBar.Type: {CrudNavBar.GetType().Name}");

                    if (CrudNavBar.NavBarBehavior != null)
                    {
                        var buttonCount = CrudNavBar.NavBarBehavior.Buttons?.Count ?? 0;
                        System.Diagnostics.Debug.WriteLine($"   CrudNavBar.navBarBehavior.Buttons.Count: {buttonCount}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SpotPage: Erro ao obter diagnósticos específicos: {ex.Message}");
            }
        }

        /// <summary>
        /// 🔧 UTILITÁRIO: Força todas as correções conhecidas da SpotPage
        /// </summary>
        public async Task ApplySpotPageFixes()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🔧 SpotPage: Aplicando correções específicas");

                // 🔧 CORREÇÃO 1: Força SelectionCount = 0
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    SelectionCount = 0;
                    OnPropertyChanged(nameof(SelectionCount));
                });

                // 🔧 CORREÇÃO 2: Verifica e configura CrudNavBar
                if (CrudNavBar != null)
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        CrudNavBar.SelectionCount = 0;
                        CrudNavBar.IsVisible = true;
                    });

                    // Força ShowAsync se possível
                    try
                    {
                        await CrudNavBar.ShowAsync();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"⚠️ SpotPage: Erro ao forçar ShowAsync: {ex.Message}");
                    }
                }

                // 🔧 CORREÇÃO 3: Aplica todas as correções genéricas
                await this.ApplyAllKnownFixes();

                System.Diagnostics.Debug.WriteLine($"✅ SpotPage: Todas as correções específicas aplicadas");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SpotPage: Erro ao aplicar correções específicas: {ex.Message}");
            }
        }

    }
}