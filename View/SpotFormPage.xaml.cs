using MyKaraoke.Domain;
using MyKaraoke.Services;
using MyKaraoke.View.Components;
using MyKaraoke.View.Extensions;
using MyKaraoke.View.Behaviors;
using System.Windows.Input;

namespace MyKaraoke.View
{
    public partial class SpotFormPage : ContentPage
    {
        private IEstabelecimentoService _estabelecimentoService;

        // Estados da interface
        private bool _isEditing = false;
        private Estabelecimento _editingLocal = null;

        // ✅ CRÍTICO: Flag para controlar inicialização
        private bool _isInitialized = false;

        // Comando que o SmartPageLifecycleBehavior irá executar
        public ICommand LoadDataCommand { get; }

        public SpotFormPage()
        {
            // ✅ CRÍTICO: Inicializa LoadDataCommand PRIMEIRO
            LoadDataCommand = new Command(async () => await InitializeDataAsync());

            InitializeComponent();

            // ✅ CRÍTICO: Define BindingContext DEPOIS do LoadDataCommand
            this.BindingContext = this;
        }

        #region Header Event Handlers

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("SpotFormPage: Cancelar clicado");
                await NavigateBackToSpotPage();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SpotFormPage: Erro ao cancelar: {ex.Message}");
            }
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("SpotFormPage: Salvar clicado via header");
                await OnSalvarLocalAsyncInternal();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SpotFormPage: Erro ao salvar via header: {ex.Message}");
            }
        }

        #endregion

        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();

            if (Handler != null && !_isInitialized)
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine("✅ SpotFormPage: OnHandlerChanged - Inicializando serviços");

                    var serviceProvider = MyKaraoke.View.ServiceProvider.FromPage(this);
                    _estabelecimentoService = serviceProvider?.GetService<IEstabelecimentoService>();

                    if (_estabelecimentoService != null)
                    {
                        System.Diagnostics.Debug.WriteLine("✅ SpotFormPage: EstabelecimentoService inicializado com sucesso");
                        _isInitialized = true;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("⚠️ SpotFormPage: EstabelecimentoService é NULL após inicialização");
                    }

                    // 🎯 CONFIGURAÇÃO: HeaderComponent para navegação segura de volta
                    var headerComponent = this.FindByName<HeaderComponent>("headerComponent");
                    if (headerComponent != null)
                    {
                        headerComponent.ConfigureSafeBackNavigation(null, 500);
                        System.Diagnostics.Debug.WriteLine("✅ SpotFormPage: HeaderComponent configurado");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ SpotFormPage: Erro ao inicializar serviços: {ex.Message}");
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
        }

        // ===== MÉTODO DE BYPASS PARA SMARTPAGELIFECYCLEBEHAVIOR =====

        /// <summary>
        /// 🎯 BYPASS: Método que o SmartPageLifecycleBehavior chamará automaticamente
        /// </summary>
        private async Task OnAppearingBypass()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("🎯 SpotFormPage: OnAppearingBypass executado");

                // ✅ GENÉRICO: Usa extension method reutilizável para FormPages
                await this.ExecuteFormPageBypass();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SpotFormPage: Erro no OnAppearingBypass: {ex.Message}");
            }
        }

        /// <summary>
        /// ✅ CORRIGIDO: Inicialização de dados para SmartPageLifecycleBehavior
        /// Agora aguarda o Handler estar disponível e os serviços inicializados
        /// </summary>
        private async Task InitializeDataAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("🔄 SpotFormPage: InitializeDataAsync INICIADO");

                // ✅ AGUARDA: Handler estar disponível
                var maxAttempts = 10;
                var attempt = 0;
                while (Handler == null && attempt < maxAttempts)
                {
                    System.Diagnostics.Debug.WriteLine($"⏳ SpotFormPage: Aguardando Handler... tentativa {attempt + 1}/{maxAttempts}");
                    await Task.Delay(50);
                    attempt++;
                }

                if (Handler == null)
                {
                    System.Diagnostics.Debug.WriteLine("❌ SpotFormPage: Handler não disponível após espera");
                    return;
                }

                // ✅ AGUARDA: Serviços estarem inicializados
                attempt = 0;
                while (!_isInitialized && attempt < maxAttempts)
                {
                    System.Diagnostics.Debug.WriteLine($"⏳ SpotFormPage: Aguardando inicialização... tentativa {attempt + 1}/{maxAttempts}");
                    await Task.Delay(50);
                    attempt++;
                }

                if (!_isInitialized || _estabelecimentoService == null)
                {
                    System.Diagnostics.Debug.WriteLine("❌ SpotFormPage: Serviços não inicializados após espera");
                    return;
                }

                System.Diagnostics.Debug.WriteLine("✅ SpotFormPage: InitializeDataAsync CONCLUÍDO - Serviços disponíveis");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SpotFormPage: Erro em InitializeDataAsync: {ex.Message}");
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
        }

        #endregion

        #region Event Handlers

        private void OnNomeLocalTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var currentLength = e.NewTextValue?.Length ?? 0;
                var hasText = !string.IsNullOrWhiteSpace(e.NewTextValue);

                // Atualiza contador de caracteres
                UpdateCharacterCounter(currentLength);

                // Limpa mensagens de erro enquanto digita
                ClearMessages();

                System.Diagnostics.Debug.WriteLine($"SpotFormPage: Texto alterado - Length={currentLength}, HasText={hasText}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro em OnNomeLocalTextChanged: {ex.Message}");
            }
        }

        /// <summary>
        /// ✅ MÉTODO PRINCIPAL: Lógica de salvamento
        /// </summary>
        private async Task OnSalvarLocalAsyncInternal()
        {
            System.Diagnostics.Debug.WriteLine("🚀 === OnSalvarLocalAsyncInternal INICIADO ===");

            try
            {
                if (_estabelecimentoService == null)
                {
                    System.Diagnostics.Debug.WriteLine("❌ EstabelecimentoService é NULL!");
                    ShowValidationMessage("Serviços não disponíveis");
                    return;
                }

                var nomeLocalEntry = this.FindByName<Entry>("nomeLocalEntry");
                var nomeLocal = nomeLocalEntry?.Text?.Trim();

                System.Diagnostics.Debug.WriteLine($"📝 Nome do local digitado: '{nomeLocal}'");

                // Validação básica
                var validation = _estabelecimentoService.ValidateNameInput(nomeLocal);
                System.Diagnostics.Debug.WriteLine($"✅ Validação: isValid={validation.isValid}, message='{validation.message}'");

                if (!validation.isValid)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ Validação falhou: {validation.message}");
                    ShowValidationMessage(validation.message);
                    return;
                }

                // 🎯 CORREÇÃO: Inicia loading ANTES de salvar
                SetLoading(true);
                System.Diagnostics.Debug.WriteLine("🔄 Loading ativado");

                try
                {
                    bool saveSuccess = false;
                    string resultMessage = "";

                    if (_isEditing && _editingLocal != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"📝 MODO EDIÇÃO: Editando local ID {_editingLocal.Id}");
                        var result = await _estabelecimentoService.UpdateEstabelecimentoAsync(_editingLocal.Id, nomeLocal);
                        saveSuccess = result.success;
                        resultMessage = result.message;
                        System.Diagnostics.Debug.WriteLine($"📝 Resultado UPDATE: success={result.success}, message='{result.message}'");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"🆕 MODO CRIAÇÃO: Criando novo local");
                        var result = await _estabelecimentoService.CreateEstabelecimentoAsync(nomeLocal);
                        saveSuccess = result.success;
                        resultMessage = result.message;
                        System.Diagnostics.Debug.WriteLine($"🔍 CREATE RESULT: success={result.success}, message='{result.message}'");
                    }

                    if (saveSuccess)
                    {
                        System.Diagnostics.Debug.WriteLine("✅ Operação bem-sucedida!");

                        // ✅ LIMPA campo após sucesso
                        await MainThread.InvokeOnMainThreadAsync(() =>
                        {
                            if (nomeLocalEntry != null)
                            {
                                nomeLocalEntry.Text = string.Empty;
                            }
                        });

                        // 🎯 CRÍTICO: MANTÉM loading durante navegação
                        System.Diagnostics.Debug.WriteLine("🔙 Navegando de volta para SpotPage COM loading...");
                        await NavigateBackToSpotPage();

                        // 🎯 APENAS AGORA desativa loading e mostra snackbar
                        SetLoading(false);
                        System.Diagnostics.Debug.WriteLine("🔄 Loading desativado APÓS navegação");

                        // 🎯 SNACKBAR: Mostra feedback de sucesso
                        await snackbar.ShowSuccessAsync(resultMessage);
                        System.Diagnostics.Debug.WriteLine("✅ Snackbar de sucesso exibido");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"❌ Operação falhou: {resultMessage}");
                        SetLoading(false); // Desativa loading se falhou
                        ShowValidationMessage(resultMessage); // Erro inline
                    }
                }
                catch (Exception serviceEx)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ Erro no serviço: {serviceEx.Message}");
                    SetLoading(false);
                    ShowValidationMessage($"Erro interno: {serviceEx.Message}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erro geral em OnSalvarLocalAsyncInternal: {ex.Message}");
                SetLoading(false);
                ShowValidationMessage("Erro interno ao salvar");
            }
            finally
            {
                System.Diagnostics.Debug.WriteLine("🚀 === OnSalvarLocalAsyncInternal FINALIZADO ===");
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
    }
}