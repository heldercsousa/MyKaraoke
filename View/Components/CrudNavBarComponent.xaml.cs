using MyKaraoke.View.Behaviors;
using MyKaraoke.View.Components;
using System.Collections.ObjectModel;

namespace MyKaraoke.View.Components
{
    public enum CrudButtonType { Anterior, Adicionar, Editar, Excluir, Salvar, Proximo }

    /// <summary>
    /// ✅ SIMPLIFICADO: Usa NavBarBehavior para eliminar duplicação
    /// Reduzido de 150+ linhas para ~50 linhas
    /// </summary>
    public partial class CrudNavBarComponent : ContentView, IAnimatableNavBar
    {
        #region Bindable Properties

        public static readonly BindableProperty SelectionCountProperty =
            BindableProperty.Create(nameof(SelectionCount), typeof(int), typeof(CrudNavBarComponent), 0,
            propertyChanged: OnSelectionCountChanged);

        public int SelectionCount
        {
            get => (int)GetValue(SelectionCountProperty);
            set => SetValue(SelectionCountProperty, value);
        }

        #endregion

        #region Events

        public event EventHandler<CrudButtonType> ButtonClicked;

        #endregion

        #region Private Fields

        private readonly Dictionary<CrudButtonType, NavButtonConfig> _buttonConfigs;
        private bool _isInitialized = false;

        #endregion

        public CrudNavBarComponent()
        {
            InitializeComponent();
            _buttonConfigs = InitializeButtonConfigs();
        }

        #region Initialization

        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();

            if (Handler != null && !_isInitialized)
            {
                // ✅ BEHAVIOR: Subscreve eventos do NavBarBehavior
                navBarBehavior.ButtonClicked += OnNavBarButtonClicked;
                UpdateLayoutAndButtons(); // Configuração inicial
                _isInitialized = true;

                System.Diagnostics.Debug.WriteLine("✅ CrudNavBarComponent inicializado com NavBarBehavior");
            }
        }

        private Dictionary<CrudButtonType, NavButtonConfig> InitializeButtonConfigs()
        {
            return new Dictionary<CrudButtonType, NavButtonConfig>
            {
                { CrudButtonType.Anterior, new NavButtonConfig { Text = "Anterior", IconSource = "prior.png", Command = new Command(() => ButtonClicked?.Invoke(this, CrudButtonType.Anterior)) } },
                { CrudButtonType.Adicionar, new NavButtonConfig { Text = "Adicionar", IconSource = "add.png", Command = new Command(() => ButtonClicked?.Invoke(this, CrudButtonType.Adicionar)) } },
                { CrudButtonType.Editar, new NavButtonConfig { Text = "Editar", IconSource = "edit.png", Command = new Command(() => ButtonClicked?.Invoke(this, CrudButtonType.Editar)) } },
                { CrudButtonType.Excluir, new NavButtonConfig { Text = "Apagar", IconSource = "delete.png", Command = new Command(() => ButtonClicked?.Invoke(this, CrudButtonType.Excluir)) } },
                { CrudButtonType.Salvar, new NavButtonConfig { Text = "Salvar", IconSource = "save.png", Command = new Command(() => ButtonClicked?.Invoke(this, CrudButtonType.Salvar)) } },
                { CrudButtonType.Proximo, new NavButtonConfig { Text = "Próximo", IconSource = "next.png", Command = new Command(() => ButtonClicked?.Invoke(this, CrudButtonType.Proximo)) } },
            };
        }

        #endregion

        #region Button Logic - CÉREBRO DO CRUD

        private static void OnSelectionCountChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is CrudNavBarComponent navBar)
            {
                navBar.UpdateLayoutAndButtons();
            }
        }

        /// <summary>
        /// ✅ CÉREBRO: Decide quais botões mostrar baseado na seleção
        /// </summary>
        private void UpdateLayoutAndButtons()
        {
            // 1. Determina botões baseado na seleção
            var visibleButtons = new List<NavButtonConfig>();

            if (SelectionCount == 0)
            {
                visibleButtons.Add(_buttonConfigs[CrudButtonType.Adicionar]);
            }
            else if (SelectionCount == 1)
            {
                visibleButtons.Add(_buttonConfigs[CrudButtonType.Editar]);
                visibleButtons.Add(_buttonConfigs[CrudButtonType.Excluir]);
            }
            else // > 1
            {
                visibleButtons.Add(_buttonConfigs[CrudButtonType.Excluir]);
            }

            // 2. Cria colunas dinâmicas
            var columnDefinitions = new ColumnDefinitionCollection();
            foreach (var _ in visibleButtons)
            {
                columnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            }

            // 3. ✅ BEHAVIOR: Configura através do NavBarBehavior
            navBarBehavior.CustomColumnDefinitions = columnDefinitions;
            navBarBehavior.Buttons = new ObservableCollection<NavButtonConfig>(visibleButtons);
        }

        #endregion

        #region Event Handlers

        private void OnNavBarButtonClicked(object sender, NavBarButtonClickedEventArgs e)
        {
            try
            {
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
                System.Diagnostics.Debug.WriteLine($"CrudNavBar: Botão {buttonType} clicado");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no clique do botão CRUD: {ex.Message}");
            }
        }

        #endregion

        #region IAnimatableNavBar - DELEGADO PARA BEHAVIOR

        /// <summary>
        /// ✅ DELEGADO: ShowAsync via NavBarBehavior
        /// </summary>
        public async Task ShowAsync()
        {
            this.IsVisible = true;
            if (navBarBehavior != null)
            {
                await navBarBehavior.ShowAsync();
            }
            else
            {
                // ✅ FALLBACK: Usa extensão no navGrid se behavior não disponível
                await NavBarExtensions.ShowAsync(navGrid);
            }
        }

        /// <summary>
        /// ✅ DELEGADO: HideAsync via NavBarBehavior
        /// </summary>
        public async Task HideAsync()
        {
            // ✅ CORREÇÃO: Usa o behavior diretamente em vez do navGrid
            if (navBarBehavior != null)
            {
                await navBarBehavior.HideAsync();
            }
            else
            {
                // ✅ FALLBACK: Usa extensão no navGrid se behavior não disponível
                await NavBarExtensions.HideAsync(navGrid);
            }

            this.IsVisible = false;
        }

        #endregion
    }
}