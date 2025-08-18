using MyKaraoke.View.Behaviors;
using MyKaraoke.View.Components;
using System.Collections.ObjectModel;

namespace MyKaraoke.View.Components
{
    public enum CrudButtonType { Anterior, Adicionar, Editar, Excluir, Salvar, Proximo }

    /// <summary>
    /// ✅ SIMPLIFICADO: Usa NavBarBehavior para eliminar duplicação
    /// 🔧 DEBUG MELHORADO: Logs detalhados para identificar problema
    /// </summary>
    public partial class CrudNavBarComponent : ContentView, IAnimatableNavBar
    {
        #region Bindable Properties

        public static readonly BindableProperty SelectionCountProperty =
            BindableProperty.Create(nameof(SelectionCount), typeof(int), typeof(CrudNavBarComponent), 0,
            propertyChanged: OnSelectionCountChanged);

        public static readonly BindableProperty IsFormModeProperty =
            BindableProperty.Create(nameof(IsFormMode), typeof(bool), typeof(CrudNavBarComponent), false,
            propertyChanged: OnModeChanged);

        public int SelectionCount
        {
            get => (int)GetValue(SelectionCountProperty);
            set => SetValue(SelectionCountProperty, value);
        }

        public bool IsFormMode
        {
            get => (bool)GetValue(IsFormModeProperty);
            set => SetValue(IsFormModeProperty, value);
        }

        #endregion

        #region Events

        public event EventHandler<CrudButtonType> ButtonClicked;

        #endregion

        /// <summary>
        /// ✅ CORREÇÃO CS0122: Expõe navBarBehavior (gerado pelo XAML) como propriedade pública
        /// </summary>
        public NavBarBehavior NavBarBehavior => navBarBehavior;

        #region Private Fields

        private readonly Dictionary<CrudButtonType, NavButtonConfig> _buttonConfigs;
        private bool _isInitialized = false;

        #endregion

        public CrudNavBarComponent()
        {
            InitializeComponent();
            _buttonConfigs = InitializeButtonConfigs();

            System.Diagnostics.Debug.WriteLine("🔧 CrudNavBarComponent: Construtor chamado");
        }

        #region Initialization

        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();

            System.Diagnostics.Debug.WriteLine($"🔧 CrudNavBarComponent: OnHandlerChanged - Handler={Handler != null}, Initialized={_isInitialized}");

            if (Handler != null && !_isInitialized)
            {
                try
                {
                    // ✅ BEHAVIOR: Subscreve eventos do NavBarBehavior
                    navBarBehavior.ButtonClicked += OnNavBarButtonClicked;

                    System.Diagnostics.Debug.WriteLine($"🔧 CrudNavBarComponent: Eventos subscritos, SelectionCount={SelectionCount}");

                    UpdateLayoutAndButtons(); // Configuração inicial
                    _isInitialized = true;

                    System.Diagnostics.Debug.WriteLine("✅ CrudNavBarComponent inicializado com NavBarBehavior");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ CrudNavBarComponent: Erro na inicialização: {ex.Message}");
                }
            }
        }

        private Dictionary<CrudButtonType, NavButtonConfig> InitializeButtonConfigs()
        {
            var configs = new Dictionary<CrudButtonType, NavButtonConfig>
            {
                { CrudButtonType.Anterior, new NavButtonConfig { Text = "Anterior", IconSource = "prior.png", Command = new Command(() => ButtonClicked?.Invoke(this, CrudButtonType.Anterior)) } },
                { CrudButtonType.Adicionar, new NavButtonConfig { Text = "Adicionar", IconSource = "add.png", Command = new Command(() => ButtonClicked?.Invoke(this, CrudButtonType.Adicionar)) } },
                { CrudButtonType.Editar, new NavButtonConfig { Text = "Editar", IconSource = "edit.png", Command = new Command(() => ButtonClicked?.Invoke(this, CrudButtonType.Editar)) } },
                { CrudButtonType.Excluir, new NavButtonConfig { Text = "Apagar", IconSource = "delete.png", Command = new Command(() => ButtonClicked?.Invoke(this, CrudButtonType.Excluir)) } },
                { CrudButtonType.Salvar, new NavButtonConfig { Text = "Salvar", IconSource = "save.png", Command = new Command(() => ButtonClicked?.Invoke(this, CrudButtonType.Salvar)) } },
                { CrudButtonType.Proximo, new NavButtonConfig { Text = "Próximo", IconSource = "next.png", Command = new Command(() => ButtonClicked?.Invoke(this, CrudButtonType.Proximo)) } },
            };

            System.Diagnostics.Debug.WriteLine($"🔧 CrudNavBarComponent: {configs.Count} configurações de botão inicializadas");
            return configs;
        }

        #endregion

        #region Button Logic - CÉREBRO DO CRUD

        private static void OnSelectionCountChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is CrudNavBarComponent navBar)
            {
                System.Diagnostics.Debug.WriteLine($"🔧 CrudNavBarComponent: SelectionCount mudou de {oldValue} para {newValue}");
                navBar.UpdateLayoutAndButtons();
            }
        }

        /// <summary>
        /// ✅ CÉREBRO: Decide quais botões mostrar baseado na seleção
        /// 🔧 DEBUG MELHORADO: Logs detalhados
        /// 🎯 PROTEÇÃO: Só executa se inicializado ou força inicialização
        /// </summary>
        private void UpdateLayoutAndButtons()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🔧 CrudNavBarComponent: UpdateLayoutAndButtons iniciado - IsFormMode={IsFormMode}, Initialized={_isInitialized}");

                // 🎯 NOVA LÓGICA: Se não está inicializado, tenta forçar
                if (!_isInitialized)
                {
                    if (Handler != null && navBarBehavior != null)
                    {
                        System.Diagnostics.Debug.WriteLine("🎯 CrudNavBarComponent: Handler e navBarBehavior disponíveis - forçando inicialização inline");

                        try
                        {
                            navBarBehavior.ButtonClicked -= OnNavBarButtonClicked; // Remove se já existe
                            navBarBehavior.ButtonClicked += OnNavBarButtonClicked; // Adiciona
                            _isInitialized = true;
                            System.Diagnostics.Debug.WriteLine("🎯 CrudNavBarComponent: Inicialização inline bem-sucedida");
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"🎯 CrudNavBarComponent: Erro na inicialização inline: {ex.Message}");
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"🔧 CrudNavBarComponent: Não inicializado - Handler={Handler != null}, navBarBehavior={navBarBehavior != null}");
                        return;
                    }
                }

                // 🔧 VERIFICAÇÃO: Se navBarBehavior é nulo
                if (navBarBehavior == null)
                {
                    System.Diagnostics.Debug.WriteLine("❌ CrudNavBarComponent: navBarBehavior é NULL!");
                    return;
                }

                var visibleButtons = new List<NavButtonConfig>();

                if (IsFormMode)
                {
                    // 🆕 MODO FORMULÁRIO: Não usa SelectionCount, será controlado pelos métodos ShowSaveButtonAsync/HideSaveButtonAsync
                    System.Diagnostics.Debug.WriteLine("🔧 CrudNavBarComponent: [FORM] Modo formulário - aguardando chamada explícita de ShowSaveButtonAsync/HideSaveButtonAsync");
                    return; // 🎯 IMPORTANTE: Não configura botões automaticamente no modo formulário
                }
                else
                {
                    // 📋 MODO LISTA: Lógica original para listas (usando SelectionCount)
                    if (SelectionCount == 0)
                    {
                        visibleButtons.Add(_buttonConfigs[CrudButtonType.Adicionar]);
                        System.Diagnostics.Debug.WriteLine("🔧 CrudNavBarComponent: [LIST] SelectionCount=0 - Adicionando botão Adicionar");
                    }
                    else if (SelectionCount == 1)
                    {
                        visibleButtons.Add(_buttonConfigs[CrudButtonType.Editar]);
                        visibleButtons.Add(_buttonConfigs[CrudButtonType.Excluir]);
                        System.Diagnostics.Debug.WriteLine("🔧 CrudNavBarComponent: [LIST] SelectionCount=1 - Adicionando botões Editar e Excluir");
                    }
                    else // > 1
                    {
                        visibleButtons.Add(_buttonConfigs[CrudButtonType.Excluir]);
                        System.Diagnostics.Debug.WriteLine($"🔧 CrudNavBarComponent: [LIST] SelectionCount={SelectionCount} - Adicionando botão Excluir");
                    }
                }

                System.Diagnostics.Debug.WriteLine($"🔧 CrudNavBarComponent: {visibleButtons.Count} botões preparados para exibição");

                // 2. Cria colunas dinâmicas
                var columnDefinitions = new ColumnDefinitionCollection();
                foreach (var _ in visibleButtons)
                {
                    columnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
                }

                System.Diagnostics.Debug.WriteLine($"🔧 CrudNavBarComponent: {columnDefinitions.Count} colunas criadas");

                // 3. ✅ BEHAVIOR: Configura através do NavBarBehavior
                navBarBehavior.CustomColumnDefinitions = columnDefinitions;
                navBarBehavior.Buttons = new ObservableCollection<NavButtonConfig>(visibleButtons);

                // 🎯 CORREÇÃO: Força exibição apenas para modo lista
                if (visibleButtons.Count > 0)
                {
                    _ = Task.Run(async () =>
                    {
                        await Task.Delay(200);
                        try
                        {
                            await navBarBehavior.ShowAsync();
                            System.Diagnostics.Debug.WriteLine($"🎯 ShowAsync executado para {visibleButtons.Count} botões");
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"❌ Erro ao executar ShowAsync: {ex.Message}");
                        }
                    });
                }

                System.Diagnostics.Debug.WriteLine($"🔧 CrudNavBarComponent: NavBarBehavior configurado com {visibleButtons.Count} botões");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ CrudNavBarComponent: Erro em UpdateLayoutAndButtons: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"❌ StackTrace: {ex.StackTrace}");
            }
        }

        private static void OnModeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is CrudNavBarComponent navBar)
            {
                System.Diagnostics.Debug.WriteLine($"🔧 CrudNavBarComponent: IsFormMode mudou de {oldValue} para {newValue}");
                navBar.UpdateLayoutAndButtons();
            }
        }

        #endregion

        #region Event Handlers

        private void OnNavBarButtonClicked(object sender, NavBarButtonClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🔧 CrudNavBarComponent: Botão clicado - Texto: '{e.ButtonConfig.Text}'");

                // Mapeia texto do botão para enum
                var buttonType = e.ButtonConfig.Text switch
                {
                    "Anterior" => CrudButtonType.Anterior,
                    "Adicionar" => CrudButtonType.Adicionar,
                    "Editar" => CrudButtonType.Editar,
                    "Apagar" => CrudButtonType.Excluir,
                    "Salvar" => CrudButtonType.Salvar,
                    "Próximo" => CrudButtonType.Proximo,
                    _ => throw new ArgumentException($"Botão desconhecido: {e.ButtonConfig.Text}")
                };

                ButtonClicked?.Invoke(this, buttonType);
                System.Diagnostics.Debug.WriteLine($"CrudNavBar: Evento ButtonClicked disparado para {buttonType}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ CrudNavBarComponent: Erro no clique do botão: {ex.Message}");
            }
        }

        #endregion

        #region IAnimatableNavBar - DELEGADO PARA BEHAVIOR

        /// <summary>
        /// ✅ DELEGADO: ShowAsync via NavBarBehavior
        /// 🎯 CORREÇÃO: Garante configuração antes de mostrar
        /// </summary>
        public async Task ShowAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🔧 CrudNavBarComponent: ShowAsync chamado - IsVisible={this.IsVisible}");

                this.IsVisible = true;

                // 🎯 CORREÇÃO CRÍTICA: Garante inicialização antes de mostrar
                await EnsureProperInitialization();

                if (navBarBehavior != null)
                {
                    System.Diagnostics.Debug.WriteLine($"🔧 CrudNavBarComponent: Chamando navBarBehavior.ShowAsync() - Buttons.Count={navBarBehavior.Buttons?.Count ?? 0}");
                    await navBarBehavior.ShowAsync();
                    System.Diagnostics.Debug.WriteLine($"🔧 CrudNavBarComponent: navBarBehavior.ShowAsync() concluído");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"🔧 CrudNavBarComponent: navBarBehavior é NULL - usando fallback");
                    // ✅ FALLBACK: Usa extensão no navGrid se behavior não disponível
                    await NavBarExtensions.ShowAsync(navGrid);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ CrudNavBarComponent: Erro em ShowAsync: {ex.Message}");
            }
        }

        /// <summary>
        /// 🎯 NOVO: Garante que a inicialização foi feita antes de mostrar
        /// </summary>
        private async Task EnsureProperInitialization()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🎯 CrudNavBarComponent: EnsureProperInitialization - Initialized={_isInitialized}");

                // Se já foi inicializado, não faz nada
                if (_isInitialized)
                {
                    System.Diagnostics.Debug.WriteLine($"🎯 CrudNavBarComponent: Já inicializado - verificando botões");

                    // Verifica se tem botões configurados
                    var buttonCount = navBarBehavior?.Buttons?.Count ?? 0;
                    if (buttonCount == 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"🎯 CrudNavBarComponent: Inicializado mas sem botões - reconfigurando");
                        UpdateLayoutAndButtons();
                    }
                    return;
                }

                // 🎯 AGUARDA até 2 segundos para Handler estar disponível
                int attempts = 0;
                const int maxAttempts = 20; // 20 x 100ms = 2 segundos

                while (attempts < maxAttempts && Handler == null)
                {
                    System.Diagnostics.Debug.WriteLine($"🎯 CrudNavBarComponent: Aguardando Handler - tentativa {attempts + 1}/{maxAttempts}");
                    await Task.Delay(100);
                    attempts++;
                }

                if (Handler == null)
                {
                    System.Diagnostics.Debug.WriteLine($"🎯 CrudNavBarComponent: TIMEOUT aguardando Handler - forçando inicialização");
                    await ForceInitialization();
                    return;
                }

                // 🎯 FORÇA inicialização manual se OnHandlerChanged não foi chamado
                if (!_isInitialized)
                {
                    System.Diagnostics.Debug.WriteLine($"🎯 CrudNavBarComponent: Handler disponível mas não inicializado - forçando");
                    await ForceInitialization();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ CrudNavBarComponent: Erro em EnsureProperInitialization: {ex.Message}");
                // Fallback: força inicialização mesmo com erro
                await ForceInitialization();
            }
        }

        /// <summary>
        /// 🎯 NOVO: Força inicialização manual quando OnHandlerChanged falha/atrasa
        /// </summary>
        private async Task ForceInitialization()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🎯 CrudNavBarComponent: ForceInitialization iniciada");

                // Aguarda um pouco para garantir que navBarBehavior está disponível
                await Task.Delay(50);

                if (navBarBehavior != null)
                {
                    // ✅ BEHAVIOR: Subscreve eventos do NavBarBehavior (se não subscrito)
                    try
                    {
                        navBarBehavior.ButtonClicked -= OnNavBarButtonClicked; // Remove se já existe
                        navBarBehavior.ButtonClicked += OnNavBarButtonClicked; // Adiciona
                        System.Diagnostics.Debug.WriteLine($"🎯 CrudNavBarComponent: Eventos reconfigurados, SelectionCount={SelectionCount}");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"🎯 CrudNavBarComponent: Erro ao configurar eventos: {ex.Message}");
                    }

                    UpdateLayoutAndButtons(); // Configuração forçada
                    _isInitialized = true;

                    System.Diagnostics.Debug.WriteLine("🎯 CrudNavBarComponent: Inicialização FORÇADA concluída");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("❌ CrudNavBarComponent: navBarBehavior ainda NULL após ForceInitialization");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ CrudNavBarComponent: Erro em ForceInitialization: {ex.Message}");
            }
        }

        /// <summary>
        /// ✅ DELEGADO: HideAsync via NavBarBehavior
        /// 🔧 DEBUG MELHORADO: Logs detalhados
        /// </summary>
        public async Task HideAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🔧 CrudNavBarComponent: HideAsync chamado");

                // ✅ CORREÇÃO: Usa o behavior diretamente em vez do navGrid
                if (navBarBehavior != null)
                {
                    System.Diagnostics.Debug.WriteLine($"🔧 CrudNavBarComponent: Chamando navBarBehavior.HideAsync()");
                    await navBarBehavior.HideAsync();
                    System.Diagnostics.Debug.WriteLine($"🔧 CrudNavBarComponent: navBarBehavior.HideAsync() concluído");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"🔧 CrudNavBarComponent: navBarBehavior é NULL - usando fallback");
                    // ✅ FALLBACK: Usa extensão no navGrid se behavior não disponível
                    await NavBarExtensions.HideAsync(navGrid);
                }

                this.IsVisible = false;
                System.Diagnostics.Debug.WriteLine($"🔧 CrudNavBarComponent: HideAsync concluído - IsVisible={this.IsVisible}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ CrudNavBarComponent: Erro em HideAsync: {ex.Message}");
            }
        }

        #endregion



        #region Métodos Específicos para Modo Formulário

        /// <summary>
        /// 🎯 NOVO: Método específico para mostrar botão Salvar (modo formulário)
        /// </summary>
        public async Task ShowSaveButtonAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🎯 CrudNavBarComponent: ShowSaveButtonAsync chamado");

                if (!IsFormMode)
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ CrudNavBarComponent: ShowSaveButtonAsync ignorado - não está em modo formulário");
                    return;
                }

                // 🎯 FORÇA: Cria botão Salvar se não existe
                await ForceCreateSaveButton();

                // 🎯 GARANTE: Botão fica visível
                await ForceShowNavBar();

                System.Diagnostics.Debug.WriteLine($"✅ CrudNavBarComponent: Botão Salvar exibido com sucesso");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ CrudNavBarComponent: Erro em ShowSaveButtonAsync: {ex.Message}");
            }
        }

        /// <summary>
        /// 🎯 NOVO: Método específico para esconder botão Salvar (modo formulário)
        /// </summary>
        public async Task HideSaveButtonAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🎯 CrudNavBarComponent: HideSaveButtonAsync chamado");

                if (!IsFormMode)
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ CrudNavBarComponent: HideSaveButtonAsync ignorado - não está em modo formulário");
                    return;
                }

                // 🎯 FORÇA: Remove botão Salvar
                await ForceRemoveSaveButton();

                System.Diagnostics.Debug.WriteLine($"✅ CrudNavBarComponent: Botão Salvar escondido com sucesso");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ CrudNavBarComponent: Erro em HideSaveButtonAsync: {ex.Message}");
            }
        }

        /// <summary>
        /// 🎯 PRIVADO: Força criação do botão Salvar
        /// </summary>
        private async Task ForceCreateSaveButton()
        {
            try
            {
                var visibleButtons = new List<NavButtonConfig>
        {
            _buttonConfigs[CrudButtonType.Salvar]
        };

                System.Diagnostics.Debug.WriteLine($"🎯 CrudNavBarComponent: Criando botão Salvar forçadamente");

                // Cria colunas dinâmicas
                var columnDefinitions = new ColumnDefinitionCollection();
                foreach (var _ in visibleButtons)
                {
                    columnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
                }

                // ✅ BEHAVIOR: Configura através do NavBarBehavior
                if (navBarBehavior != null)
                {
                    navBarBehavior.CustomColumnDefinitions = columnDefinitions;
                    navBarBehavior.Buttons = new ObservableCollection<NavButtonConfig>(visibleButtons);

                    System.Diagnostics.Debug.WriteLine($"🎯 CrudNavBarComponent: NavBarBehavior configurado com botão Salvar");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ CrudNavBarComponent: Erro em ForceCreateSaveButton: {ex.Message}");
            }
        }

        /// <summary>
        /// 🎯 PRIVADO: Força remoção do botão Salvar
        /// </summary>
        private async Task ForceRemoveSaveButton()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🎯 CrudNavBarComponent: Removendo botão Salvar forçadamente");

                // ✅ BEHAVIOR: Remove botões através do NavBarBehavior
                if (navBarBehavior != null)
                {
                    navBarBehavior.CustomColumnDefinitions = new ColumnDefinitionCollection();
                    navBarBehavior.Buttons = new ObservableCollection<NavButtonConfig>();

                    // 🎯 ESCONDE: NavBar quando não há botões
                    await navBarBehavior.HideAsync();

                    System.Diagnostics.Debug.WriteLine($"🎯 CrudNavBarComponent: NavBarBehavior configurado sem botões");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ CrudNavBarComponent: Erro em ForceRemoveSaveButton: {ex.Message}");
            }
        }

        /// <summary>
        /// 🎯 PRIVADO: Força exibição da NavBar
        /// </summary>
        private async Task ForceShowNavBar()
        {
            try
            {
                if (navBarBehavior != null)
                {
                    // 🎯 CORREÇÃO CRÍTICA: Reseta flags para permitir nova exibição
                    navBarBehavior.GetType().GetField("_isShown", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(navBarBehavior, false);
                    navBarBehavior.GetType().GetField("_isAnimating", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(navBarBehavior, false);

                    await navBarBehavior.ShowAsync();
                    System.Diagnostics.Debug.WriteLine($"🎯 CrudNavBarComponent: ForceShowNavBar concluído");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ CrudNavBarComponent: Erro em ForceShowNavBar: {ex.Message}");
            }
        }

        #endregion
    }
}