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

        // Comando que o SmartPageLifecycleBehavior irá executar
        public ICommand LoadDataCommand { get; private set;  }


        public SpotPage()
        {
            // ✅ EXATAMENTE como StackPage: LoadDataCommand ANTES do InitializeComponent
            LoadDataCommand = new Command(async () => await InitializeAndLoadDataAsync());

            System.Diagnostics.Debug.WriteLine($"🔧 SpotPage: LoadDataCommand criado ANTES do InitializeComponent: {LoadDataCommand != null}");

            // ✅ Agora o binding encontrará LoadDataCommand disponível
            InitializeComponent();

            // ✅ Resto da inicialização
            Locais = new ObservableCollection<Estabelecimento>();
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
                var locais = await _estabelecimentoService.GetAllEstabelecimentosAsync();
                System.Diagnostics.Debug.WriteLine($"🔍 LOAD RESULT: {locais?.Count()} locais encontrados");

                Locais.Clear();
                if (locais != null)
                {
                    foreach (var local in locais)
                    {
                        Locais.Add(local);
                        System.Diagnostics.Debug.WriteLine($"🔍 ADDED TO COLLECTION: {local.Id} - '{local.Nome}'");
                    }
                }
                System.Diagnostics.Debug.WriteLine($"🔍 FINAL COLLECTION COUNT: {Locais.Count}");

                // ✅ CRÍTICO: Chama UpdateUIState no MainThread APÓS carregar dados
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    UpdateUIState();
                    System.Diagnostics.Debug.WriteLine($"✅ SpotPage ({this.GetHashCode()}): UpdateUIState chamado após carregar {Locais.Count} locais");
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SpotPage ({this.GetHashCode()}): Erro ao carregar locais: {ex.Message}");

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

        private void OnLocalSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectionCount = e.CurrentSelection.Count;
            System.Diagnostics.Debug.WriteLine($"✅ SpotPage ({this.GetHashCode()}): Seleção mudou - SelectionCount={SelectionCount}");
        }

        private async void OnCrudNavBarButtonClicked(object sender, CrudButtonType buttonType)
        {
            System.Diagnostics.Debug.WriteLine($"✅ SpotPage ({this.GetHashCode()}): Botão CrudNavBar clicado - {buttonType}");

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

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}