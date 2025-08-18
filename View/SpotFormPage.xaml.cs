using MyKaraoke.Domain;
using MyKaraoke.Services;
using MyKaraoke.View.Components;
using MyKaraoke.View.Extensions;
using MyKaraoke.View.Behaviors;
using System.ComponentModel;
using System.Windows.Input;

namespace MyKaraoke.View
{
    public partial class SpotFormPage : ContentPage, INotifyPropertyChanged
    {
        private IEstabelecimentoService _estabelecimentoService;

        // Estados da interface
        private bool _isEditing = false;
        private Estabelecimento _editingLocal = null;

        // Propriedade para controlar se deve mostrar botão Salvar na CrudNavBar
        private bool _hasTextToSave;
        public bool HasTextToSave
        {
            get => _hasTextToSave;
            set
            {
                if (_hasTextToSave != value)
                {
                    _hasTextToSave = value;
                    OnPropertyChanged(nameof(HasTextToSave));

                    // 🎯 CORREÇÃO CRÍTICA: Notifica CrudNavBar diretamente sobre mudança
                    NotifyCrudNavBarAboutTextChange(value);
                }
            }
        }

        // Comando que o SmartPageLifecycleBehavior irá executar
        public ICommand LoadDataCommand { get; }

        public SpotFormPage()
        {
            // ✅ CRÍTICO: Inicializa LoadDataCommand PRIMEIRO
            LoadDataCommand = new Command(async () => await InitializeDataAsync());

            InitializeComponent();

            // ✅ CRÍTICO: Define BindingContext DEPOIS do LoadDataCommand
            this.BindingContext = this;

            // ✅ INICIAL: Define HasTextToSave inicial (false = sem botão Salvar inicialmente)
            HasTextToSave = false;

            // 📝 REGISTRO: Auto-registro no PageInstanceManager
            this.RegisterInInstanceManager();
        }

        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();

            if (Handler != null)
            {
                try
                {
                    var serviceProvider = MyKaraoke.View.ServiceProvider.FromPage(this);
                    _estabelecimentoService = serviceProvider?.GetService<IEstabelecimentoService>();

                    // 🎯 CONFIGURAÇÃO: HeaderComponent para navegação segura de volta
                    var headerComponent = this.FindByName<HeaderComponent>("headerComponent");
                    if (headerComponent != null)
                    {
                        headerComponent.ConfigureSafeBackNavigation(typeof(SpotPage), 500);
                    }
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

            try
            {
                var nomeLocalEntry = this.FindByName<Entry>("nomeLocalEntry");
                nomeLocalEntry?.Focus();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao focar campo: {ex.Message}");
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            this.UnregisterFromInstanceManager();
        }

        // ===== MÉTODO DE BYPASS PARA SMARTPAGELIFECYCLEBEHAVIOR =====

        /// <summary>
        /// 🎯 BYPASS: Método que o SmartPageLifecycleBehavior chamará automaticamente
        /// </summary>
        private async Task OnAppearingBypass()
        {
            try
            {
                // ✅ GENÉRICO: Usa extension method reutilizável para FormPages
                await this.ExecuteFormPageBypass();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SpotFormPage: Erro no OnAppearingBypass: {ex.Message}");
                // 🛡️ FALLBACK: Garante estado mínimo mesmo com erro
                MainThread.BeginInvokeOnMainThread(() => HasTextToSave = false);
            }
        }

        /// <summary>
        /// ✅ SIMPLIFICADO: Inicialização de dados para SmartPageLifecycleBehavior
        /// </summary>
        private async Task InitializeDataAsync()
        {
            try
            {
                if (_estabelecimentoService == null)
                {
                    var serviceProvider = new ServiceProvider(this.Handler.MauiContext.Services);
                    _estabelecimentoService = serviceProvider.GetService<IEstabelecimentoService>();
                }

                // ✅ FORÇA: Estado inicial correto
                MainThread.BeginInvokeOnMainThread(() => HasTextToSave = false);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SpotFormPage: Erro em InitializeDataAsync: {ex.Message}");
                MainThread.BeginInvokeOnMainThread(() => HasTextToSave = false);
            }
        }

        #region Configuração da Página

        /// <summary>
        /// Configura a página para adicionar novo local
        /// </summary>
        public void ConfigureForAdding()
        {
            _isEditing = false;
            _editingLocal = null;

            var headerComponent = this.FindByName<HeaderComponent>("headerComponent");
            if (headerComponent != null)
            {
                headerComponent.Title = "Adicionar Local";
            }

            var nomeLocalEntry = this.FindByName<Entry>("nomeLocalEntry");
            if (nomeLocalEntry != null)
            {
                nomeLocalEntry.Text = string.Empty;
            }

            ClearMessages();
            HasTextToSave = false;
        }

        /// <summary>
        /// Configura a página para editar local existente
        /// </summary>
        public void ConfigureForEditing(Estabelecimento local)
        {
            _isEditing = true;
            _editingLocal = local;

            var headerComponent = this.FindByName<HeaderComponent>("headerComponent");
            if (headerComponent != null)
            {
                headerComponent.Title = "Editar Local";
            }

            var nomeLocalEntry = this.FindByName<Entry>("nomeLocalEntry");
            if (nomeLocalEntry != null)
            {
                nomeLocalEntry.Text = local.Nome;
            }

            ClearMessages();
            HasTextToSave = !string.IsNullOrWhiteSpace(local.Nome);
        }

        #endregion

        #region Event Handlers

        private void OnNomeLocalTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var currentLength = e.NewTextValue?.Length ?? 0;
                var hasText = !string.IsNullOrWhiteSpace(e.NewTextValue);

                // ✅ FUNCIONALIDADE 4: Controla exibição do botão Salvar baseado no texto
                HasTextToSave = hasText;

                // 🧪 DEBUG: Logs para identificar o problema
                System.Diagnostics.Debug.WriteLine($"🧪 OnNomeLocalTextChanged: hasText={hasText}, HasTextToSave={HasTextToSave}");

                // 🧪 DEBUG: Verifica se CrudNavBar existe
                var crudNavBar = this.FindByName<VisualElement>("CrudNavBar");
                System.Diagnostics.Debug.WriteLine($"🧪 CrudNavBar encontrada: {crudNavBar != null}, IsVisible: {crudNavBar?.IsVisible}");

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

        // 🎯 NOVO MÉTODO: Notifica CrudNavBar sobre mudança de texto
        private async void NotifyCrudNavBarAboutTextChange(bool hasText)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🎯 SpotFormPage: NotifyCrudNavBarAboutTextChange - hasText={hasText}");

                var crudNavBar = this.FindByName<CrudNavBarComponent>("CrudNavBar");
                if (crudNavBar != null)
                {
                    // 🎯 ESTRATÉGIA: CrudNavBar no modo formulário observa HasTextToSave da página

                    if (hasText)
                    {
                        // 🎯 FORÇA: Mostra botão Salvar quando há texto
                        await crudNavBar.ShowSaveButtonAsync();
                        System.Diagnostics.Debug.WriteLine($"✅ SpotFormPage: Botão Salvar EXIBIDO");
                    }
                    else
                    {
                        // 🎯 FORÇA: Esconde botão Salvar quando não há texto
                        await crudNavBar.HideSaveButtonAsync();
                        System.Diagnostics.Debug.WriteLine($"✅ SpotFormPage: Botão Salvar ESCONDIDO");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"❌ SpotFormPage: CrudNavBar não encontrada");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SpotFormPage: Erro ao notificar CrudNavBar: {ex.Message}");
            }
        }


        /// <summary>
        /// ✅ FUNCIONALIDADE 5: Responde ao clique do botão Salvar da CrudNavBar
        /// </summary>
        private async void OnCrudNavBarButtonClicked(object sender, CrudButtonType buttonType)
        {
            switch (buttonType)
            {
                case CrudButtonType.Salvar:
                    await OnSalvarLocalAsyncInternal();
                    break;
            }
        }

        /// <summary>
        /// 🛡️ COMPATIBILIDADE: Event handler para botão XAML (mantido temporariamente)
        /// </summary>
        private async void OnSalvarLocalClicked(object sender, EventArgs e)
        {
            await OnSalvarLocalAsyncInternal();
        }

        /// <summary>
        /// ✅ MÉTODO PRINCIPAL: Lógica de salvamento (renomeado para evitar conflito)
        /// </summary>
        private async Task OnSalvarLocalAsyncInternal()
        {
            if (_estabelecimentoService == null)
            {
                ShowValidationMessage("Serviços não disponíveis");
                return;
            }

            var nomeLocalEntry = this.FindByName<Entry>("nomeLocalEntry");
            var nomeLocal = nomeLocalEntry?.Text?.Trim();

            // Validação básica
            var validation = _estabelecimentoService.ValidateNameInput(nomeLocal);
            if (!validation.isValid)
            {
                ShowValidationMessage(validation.message);
                return;
            }

            try
            {
                SetLoading(true);

                if (_isEditing && _editingLocal != null)
                {
                    var result = await _estabelecimentoService.UpdateEstabelecimentoAsync(_editingLocal.Id, nomeLocal);
                    if (result.success)
                    {
                        ShowSuccessMessage(result.message);
                        await Task.Delay(1500);
                        await NavigateBackToSpotPage();
                    }
                    else
                    {
                        ShowValidationMessage(result.message);
                    }
                }
                else
                {
                    var result = await _estabelecimentoService.CreateEstabelecimentoAsync(nomeLocal);
                    if (result.success)
                    {
                        ShowSuccessMessage(result.message);
                        await Task.Delay(1500);
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
                SetLoading(false);
            }
        }

        #endregion

        #region Navegação

        private async Task NavigateBackToSpotPage()
        {
            try
            {
                // 🎯 BUSCA: SafeNavigationBehavior no XAML
                var backBehavior = this.Behaviors?.OfType<SafeNavigationBehavior>()
                    .FirstOrDefault(b => b.TargetPageType == typeof(SpotPage));

                if (backBehavior != null)
                {
                    backBehavior.CreatePageFunc = () => new SpotPage();
                    await backBehavior.NavigateToPageAsync();
                }
                else
                {
                    // 🛡️ FALLBACK: Navegação tradicional
                    await Navigation.PopAsync();
                }
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
                var characterCounterLabel = this.FindByName<Label>("characterCounterLabel");
                if (characterCounterLabel == null || _estabelecimentoService == null)
                {
                    if (characterCounterLabel != null)
                    {
                        characterCounterLabel.IsVisible = false;
                    }
                    return;
                }

                if (_estabelecimentoService.ShouldShowCharacterCounter(currentLength))
                {
                    var (text, isWarning, isError) = _estabelecimentoService.GetCharacterCounterInfo(currentLength);

                    characterCounterLabel.Text = text;
                    characterCounterLabel.IsVisible = true;

                    if (isError)
                        characterCounterLabel.TextColor = Color.FromArgb("#ff6b6b");
                    else if (isWarning)
                        characterCounterLabel.TextColor = Color.FromArgb("#FF9800");
                    else
                        characterCounterLabel.TextColor = Color.FromArgb("#b0a8c7");
                }
                else
                {
                    characterCounterLabel.IsVisible = false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no contador de caracteres: {ex.Message}");
            }
        }

        private void ShowValidationMessage(string message)
        {
            var validationMessageLabel = this.FindByName<Label>("validationMessageLabel");
            var successMessageLabel = this.FindByName<Label>("successMessageLabel");

            if (validationMessageLabel != null)
            {
                validationMessageLabel.Text = message;
                validationMessageLabel.IsVisible = true;
            }

            if (successMessageLabel != null)
            {
                successMessageLabel.IsVisible = false;
            }
        }

        private void ShowSuccessMessage(string message)
        {
            var validationMessageLabel = this.FindByName<Label>("validationMessageLabel");
            var successMessageLabel = this.FindByName<Label>("successMessageLabel");

            if (successMessageLabel != null)
            {
                successMessageLabel.Text = message;
                successMessageLabel.IsVisible = true;
            }

            if (validationMessageLabel != null)
            {
                validationMessageLabel.IsVisible = false;
            }
        }

        private void ClearMessages()
        {
            var validationMessageLabel = this.FindByName<Label>("validationMessageLabel");
            var successMessageLabel = this.FindByName<Label>("successMessageLabel");

            if (validationMessageLabel != null)
            {
                validationMessageLabel.IsVisible = false;
            }

            if (successMessageLabel != null)
            {
                successMessageLabel.IsVisible = false;
            }
        }

        private void SetLoading(bool isLoading)
        {
            try
            {
                var loadingOverlay = this.FindByName<VisualElement>("loadingOverlay");
                if (loadingOverlay != null)
                {
                    loadingOverlay.IsVisible = isLoading;
                }

                var crudNavBar = this.FindByName<VisualElement>("CrudNavBar");
                if (crudNavBar != null)
                {
                    crudNavBar.IsVisible = !isLoading;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao configurar loading state: {ex.Message}");
            }
        }

        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}