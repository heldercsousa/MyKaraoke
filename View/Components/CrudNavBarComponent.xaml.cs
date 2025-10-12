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

        // 🎯 OTIMIZAÇÃO: Cache para evitar reconstruções desnecessárias
        private int _lastProcessedSelectionCount = -1; // -1 = nunca processado
        private bool _hasProcessedFirstUpdate = false;


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
                { CrudButtonType.Anterior, new NavButtonConfig { Text = "Anterior", IconSource = "prior.png" } },
                { CrudButtonType.Editar, new NavButtonConfig { Text = "Editar", IconSource = "edit.png" } },
                { CrudButtonType.Excluir, new NavButtonConfig { Text = "Apagar", IconSource = "delete.png" } },
                { CrudButtonType.Salvar, new NavButtonConfig { Text = "Salvar", IconSource = "save.png" } }, // ✅ SEM Command
                { CrudButtonType.Proximo, new NavButtonConfig { Text = "Próximo", IconSource = "next.png" } },
            };

            System.Diagnostics.Debug.WriteLine($"🔍 DEBUG: navBarBehavior._isShown: {navBarBehavior.GetType().GetField("_isShown", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(navBarBehavior)}");
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
        /// ✅ CÉREBRO OTIMIZADO: Decide quais botões mostrar baseado na seleção
        /// 🎯 OTIMIZAÇÃO: Só reconstrói quando realmente necessário
        /// </summary>
        private void UpdateLayoutAndButtons()
        {
            try
            {
                var currentCount = SelectionCount;
                System.Diagnostics.Debug.WriteLine($"🔧 CrudNavBarComponent: UpdateLayoutAndButtons iniciado - IsFormMode={IsFormMode}, SelectionCount={currentCount}");

                if (!_isInitialized)
                {
                    if (Handler != null && navBarBehavior != null)
                    {
                        System.Diagnostics.Debug.WriteLine("🎯 CrudNavBarComponent: Handler e navBarBehavior disponíveis - forçando inicialização inline");
                        try
                        {
                            navBarBehavior.ButtonClicked -= OnNavBarButtonClicked;
                            navBarBehavior.ButtonClicked += OnNavBarButtonClicked;
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

                if (navBarBehavior == null)
                {
                    System.Diagnostics.Debug.WriteLine("❌ CrudNavBarComponent: navBarBehavior é NULL!");
                    return;
                }

                if (!ShouldRebuildButtons(currentCount))
                {
                    System.Diagnostics.Debug.WriteLine($"🛡️ CrudNavBarComponent: Reconstrução desnecessária evitada - SelectionCount={currentCount} (último processado: {_lastProcessedSelectionCount})");

                    // ✅ CORREÇÃO PROBLEMA 1 e 4: Mesmo sem reconstruir, atualiza visibilidade
                    UpdateNavBarVisibility(currentCount);
                    return;
                }

                var visibleButtons = new List<NavButtonConfig>();

                if (IsFormMode)
                {
                    System.Diagnostics.Debug.WriteLine("🔧 CrudNavBarComponent: [FORM] Modo formulário - aguardando chamada explícita");
                    return;
                }
                else
                {
                    // 📋 MODO LISTA
                    if (currentCount == 0)
                    {
                        // ✅ CORREÇÃO: Lista VAZIA = NavBar ESCONDIDA
                        System.Diagnostics.Debug.WriteLine("🔧 CrudNavBarComponent: [LIST] SelectionCount=0 - NavBar será ESCONDIDA");
                        visibleButtons.Clear();
                    }
                    else if (currentCount == 1)
                    {
                        visibleButtons.Add(_buttonConfigs[CrudButtonType.Editar]);
                        visibleButtons.Add(_buttonConfigs[CrudButtonType.Excluir]);
                        System.Diagnostics.Debug.WriteLine("🔧 CrudNavBarComponent: [LIST] SelectionCount=1 - Adicionando Editar e Excluir");
                    }
                    else // > 1
                    {
                        visibleButtons.Add(_buttonConfigs[CrudButtonType.Excluir]);
                        System.Diagnostics.Debug.WriteLine($"🔧 CrudNavBarComponent: [LIST] SelectionCount={currentCount} - Adicionando Excluir");
                    }
                }

                System.Diagnostics.Debug.WriteLine($"🔧 CrudNavBarComponent: {visibleButtons.Count} botões preparados para exibição");

                // Cria colunas dinâmicas
                var columnDefinitions = new ColumnDefinitionCollection();
                foreach (var _ in visibleButtons)
                {
                    columnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
                }

                navBarBehavior.CustomColumnDefinitions = columnDefinitions;
                navBarBehavior.Buttons = new ObservableCollection<NavButtonConfig>(visibleButtons);

                _lastProcessedSelectionCount = currentCount;
                _hasProcessedFirstUpdate = true;

                // ✅ CORREÇÃO PROBLEMA 1 e 4: Atualiza visibilidade da NavBar
                UpdateNavBarVisibility(currentCount);

                // Força exibição apenas se há botões
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

        /// <summary>
        /// ✅ NOVO MÉTODO: Atualiza visibilidade da NavBar baseado no SelectionCount
        /// </summary>
        private void UpdateNavBarVisibility(int selectionCount)
        {
            try
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (selectionCount == 0)
                    {
                        // Esconde NavBar quando não há seleção
                        this.IsVisible = false;
                        System.Diagnostics.Debug.WriteLine("🎯 CrudNavBarComponent: NavBar ESCONDIDA (SelectionCount=0)");
                    }
                    else
                    {
                        // Mostra NavBar quando há seleção
                        this.IsVisible = true;
                        System.Diagnostics.Debug.WriteLine($"🎯 CrudNavBarComponent: NavBar VISÍVEL (SelectionCount={selectionCount})");
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ CrudNavBarComponent: Erro ao atualizar visibilidade: {ex.Message}");
            }
        }

        /// <summary>
        /// 🎯 OTIMIZAÇÃO: Determina se precisa reconstruir botões baseado na sua dica
        /// Só reconstrói nas transições: 0→1, 1→2, 2→1, 1→0
        /// </summary>
        private bool ShouldRebuildButtons(int currentCount)
        {
            // Primeira execução - sempre reconstrói
            if (!_hasProcessedFirstUpdate)
            {
                System.Diagnostics.Debug.WriteLine($"🎯 CrudNavBarComponent: Primeira execução - forçando reconstrução");
                return true;
            }

            var lastCount = _lastProcessedSelectionCount;

            // 🎯 TRANSIÇÕES QUE REQUEREM RECONSTRUÇÃO (baseado na sua dica):
            // 0 → 1: Adicionar → Editar+Excluir
            // 1 → 0: Editar+Excluir → Adicionar  
            // 1 → 2: Editar+Excluir → Excluir
            // 2 → 1: Excluir → Editar+Excluir

            bool needsRebuild = (lastCount == 0 && currentCount == 1) ||   // 0 → 1
                                (lastCount == 1 && currentCount == 0) ||   // 1 → 0  
                                (lastCount == 1 && currentCount == 2) ||   // 1 → 2
                                (lastCount == 2 && currentCount == 1);     // 2 → 1

            if (needsRebuild)
            {
                System.Diagnostics.Debug.WriteLine($"🎯 CrudNavBarComponent: Transição detectada {lastCount}→{currentCount} - RECONSTRUINDO");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"🛡️ CrudNavBarComponent: Transição {lastCount}→{currentCount} não requer reconstrução");
            }

            return needsRebuild;
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