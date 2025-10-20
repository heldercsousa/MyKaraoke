using MyVocaList.Domain;
using MyVocaList.Services;
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

namespace MyVocaList.View
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

                    // ✅ AUTO-DISCOVERY: NavBar is automatically discovered by SmartPageLifecycleBehavior
                    // No manual wiring needed! CrudNavBarComponent self-registers via OnParentSet()

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
                System.Diagnostics.Debug.WriteLine($"========================================");
                System.Diagnostics.Debug.WriteLine($"SpotPage: UpdateUIState triggered");

                bool hasLocais = Locais.Any();
                emptyStateFrame.IsVisible = !hasLocais;
                locaisCollectionView.IsVisible = hasLocais;

                if (!hasLocais)
                {
                    System.Diagnostics.Debug.WriteLine($"SpotPage: Sem locais - SelectionCount=0");
                    SelectionCount = 0;
                }
                else
                {
                    var currentSelection = Locais.Count(x => x.IsSelected);
                    System.Diagnostics.Debug.WriteLine($"SpotPage: currentSelection={currentSelection}, SelectionCount atual={SelectionCount}");
                    if (SelectionCount != currentSelection)
                    {
                        SelectionCount = currentSelection;
                    }
                }

                // ✅ CONTROLE DE VISIBILIDADE DO FAB
                bool shouldShowFab = SelectionCount == 0;

                if (addFab != null)
                {
                    addFab.IsVisible = shouldShowFab;
                    System.Diagnostics.Debug.WriteLine($"SpotPage: FAB IsVisible={shouldShowFab} (SelectionCount={SelectionCount})");
                }

                System.Diagnostics.Debug.WriteLine($"========================================");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERRO em UpdateUIState: {ex.Message}");
            }
        }

        private void OnTestFrameTapped(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("========================================");
            System.Diagnostics.Debug.WriteLine("FRAME DE TESTE CLICADO!!!");
            System.Diagnostics.Debug.WriteLine("========================================");
            DisplayAlert("Teste", "Frame funcionou!", "OK");
        }

        private async void OnAddFabClicked(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("🎯 SpotPage: OnAddFabClicked RECEBIDO");
            await OnAddFabClickedAsync();
        }
     
        /// <summary>
        /// 🎯 MÉTODO ESPECÍFICO: Chamado pelo FAB para adicionar novo local
        /// ✅ USA SafeNavigationBehavior igual à CrudNavBar
        /// </summary>
        public async Task OnAddFabClickedAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("========================================");
                System.Diagnostics.Debug.WriteLine("🎯 SpotPage: OnAddFabClickedAsync INICIADO");
                System.Diagnostics.Debug.WriteLine($"🎯 SpotPage: Thread: {Thread.CurrentThread.ManagedThreadId}");
                System.Diagnostics.Debug.WriteLine($"🎯 SpotPage: Navigation: {Navigation != null}");
                System.Diagnostics.Debug.WriteLine($"🎯 SpotPage: Handler: {Handler != null}");
                System.Diagnostics.Debug.WriteLine("========================================");

                System.Diagnostics.Debug.WriteLine("🎯 SpotPage: FAB clicado - navegando para adicionar novo local");

                await NavigateToSpotFormPageAsync(isEditing: false, editingLocal: null);

                System.Diagnostics.Debug.WriteLine("✅ SpotPage: Navegação do FAB concluída");
                System.Diagnostics.Debug.WriteLine("========================================");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SpotPage: Erro na navegação do FAB: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"❌ StackTrace: {ex.StackTrace}");
                System.Diagnostics.Debug.WriteLine("========================================");
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
            System.Diagnostics.Debug.WriteLine($"✅ SpotPage: Botão {buttonType} clicado");

            var selectedItems = Locais.Where(x => x.IsSelected).ToList();
            System.Diagnostics.Debug.WriteLine($"🔍 SpotPage: {selectedItems.Count} itens selecionados");

            switch (buttonType)
            {
                case CrudButtonType.Adicionar:  // ✅ ADICIONAR ESTE CASE
                    System.Diagnostics.Debug.WriteLine($"🔧 SpotPage: Iniciando navegação para adicionar novo local");
                    try
                    {
                        await NavigateToSpotFormPageAsync(isEditing: false, editingLocal: null);
                        System.Diagnostics.Debug.WriteLine($"🔧 SpotPage: Navegação para formulário concluída");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"❌ SpotPage: Erro na navegação: {ex.Message}");
                    }
                    break;
                case CrudButtonType.Editar:
                    if (selectedItems.Count == 1)
                    {
                        System.Diagnostics.Debug.WriteLine($"🔧 SpotPage: Iniciando edição de '{selectedItems.First().Nome}'");
                        try
                        {
                            var entity = EstabelecimentoMapper.ToEntity(selectedItems.First());
                            System.Diagnostics.Debug.WriteLine($"🔧 SpotPage: Entity mapeada: {entity?.Nome}");

                            await NavigateToSpotFormPageAsync(isEditing: true, editingLocal: entity);
                            System.Diagnostics.Debug.WriteLine($"🔧 SpotPage: Navegação concluída");
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"❌ SpotPage: Erro na edição: {ex.Message}");
                        }
                    }
                    break;

                case CrudButtonType.Excluir:
                    System.Diagnostics.Debug.WriteLine($"🔧 SpotPage: Iniciando exclusão de {selectedItems.Count} itens");
                    try
                    {
                        await ConfirmAndDeleteAsync(selectedItems);
                        System.Diagnostics.Debug.WriteLine($"🔧 SpotPage: Exclusão concluída");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"❌ SpotPage: Erro na exclusão: {ex.Message}");
                    }
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

                // ✅ CORREÇÃO: Recarrega lista
                await LoadLocaisAsync();

                // ✅ CORREÇÃO PROBLEMA 4: Limpa seleção E força SelectionCount=0
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    locaisCollectionView.SelectedItems?.Clear();
                    SelectionCount = 0; // FORÇA zero ANTES de UpdateUIState
                    System.Diagnostics.Debug.WriteLine("✅ SpotPage: Seleção limpa e SelectionCount=0 após exclusão");

                    // ✅ Atualiza UI após limpar
                    UpdateUIState();
                });
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