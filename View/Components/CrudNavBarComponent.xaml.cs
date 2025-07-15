using MyKaraoke.Domain; // Para o enum
using System.Collections.ObjectModel;

namespace MyKaraoke.View.Components
{
    public enum CrudButtonType { Anterior, Adicionar, Editar, Excluir, Salvar, Proximo }

    public partial class CrudNavBarComponent : ContentView, IAnimatableNavBar
    {
        // Propriedade que a SpotPage vai definir
        public static readonly BindableProperty SelectionCountProperty =
            BindableProperty.Create(nameof(SelectionCount), typeof(int), typeof(CrudNavBarComponent), 0,
            propertyChanged: OnSelectionCountChanged);

        public int SelectionCount
        {
            get => (int)GetValue(SelectionCountProperty);
            set => SetValue(SelectionCountProperty, value);
        }

        // Evento para a SpotPage saber qual botão foi clicado
        public event EventHandler<CrudButtonType> ButtonClicked;

        // Dicionário privado para guardar as configurações de cada botão
        private readonly Dictionary<CrudButtonType, NavButtonConfig> _buttonConfigs;

        public CrudNavBarComponent()
        {
            InitializeComponent();
            _buttonConfigs = InitializeButtonConfigs();
            UpdateLayoutAndButtons(); // Chamada inicial para configurar o estado padrão
        }

        // Cria a configuração de todos os botões uma única vez
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

        private static void OnSelectionCountChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is CrudNavBarComponent navBar)
            {
                navBar.UpdateLayoutAndButtons();
            }
        }

        // O CÉREBRO: Decide quais botões mostrar e qual layout usar
        private void UpdateLayoutAndButtons()
        {
            // 1. Determina quais botões de ação devem estar visíveis
            var visibleActionButtons = new List<NavButtonConfig>();
            if (SelectionCount == 0)
            {
                visibleActionButtons.Add(_buttonConfigs[CrudButtonType.Adicionar]);
            }
            else if (SelectionCount == 1)
            {
                visibleActionButtons.Add(_buttonConfigs[CrudButtonType.Editar]);
                visibleActionButtons.Add(_buttonConfigs[CrudButtonType.Excluir]);
            }
            else // > 1
            {
                visibleActionButtons.Add(_buttonConfigs[CrudButtonType.Excluir]);
            }

            // 2. Monta a lista final de botões (Anterior + Ações + Próximo)
            var finalButtonList = new ObservableCollection<NavButtonConfig>();
            // (Você pode adicionar lógica aqui para mostrar/esconder os botões Anterior/Proximo se quiser)
            // finalButtonList.Add(_buttonConfigs[CrudButtonType.Anterior]);
            foreach (var btn in visibleActionButtons) finalButtonList.Add(btn);
            // finalButtonList.Add(_buttonConfigs[CrudButtonType.Proximo]);

            // 3. Cria a definição de colunas DINÂMICA
            var columnDefinitions = new ColumnDefinitionCollection();
            foreach (var _ in finalButtonList)
            {
                columnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            }

            // 4. Envia as instruções para o BaseNavBarComponent renderizar
            baseNavBar.CustomColumnDefinitions = columnDefinitions;
            baseNavBar.Buttons = finalButtonList;
        }

        // Delega as chamadas de animação para o BaseNavBarComponent contido nele
        public Task ShowAsync() => baseNavBar.ShowAsync();
        public Task HideAsync() => baseNavBar.HideAsync();
    }
}