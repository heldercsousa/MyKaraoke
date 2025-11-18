using MyVocaList.View.Behaviors;
using MyVocaList.View.Components;
using System.Collections.ObjectModel;

namespace MyVocaList.View.Components
{
    // ✅ CLEANED: Removed 'Adicionar' and 'Salvar'. 
    // This component now focuses solely on List Item Actions (Edit/Delete) and Navigation (Prev/Next).
    public enum CrudButtonType { Anterior, Editar, Excluir, Proximo }

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

        public event EventHandler<CrudButtonType> ButtonClicked;
        public NavBarBehavior NavBarBehavior => navBarBehavior;

        private readonly Dictionary<CrudButtonType, NavButtonConfig> _buttonConfigs;
        private bool _isInitialized = false;

        // Optimization Cache
        private int _lastProcessedSelectionCount = -1;
        private bool _hasProcessedFirstUpdate = false;

        public CrudNavBarComponent()
        {
            InitializeComponent();
            _buttonConfigs = InitializeButtonConfigs();
        }

        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();
            if (Handler != null && !_isInitialized)
            {
                try
                {
                    navBarBehavior.ButtonClicked += OnNavBarButtonClicked;
                    UpdateLayoutAndButtons();
                    _isInitialized = true;
                }
                catch (Exception ex) { System.Diagnostics.Debug.WriteLine(ex.Message); }
            }
        }

        private Dictionary<CrudButtonType, NavButtonConfig> InitializeButtonConfigs()
        {
            // ✅ CLEANED CONFIG: Only Navigation and Selection actions remain.
            var configs = new Dictionary<CrudButtonType, NavButtonConfig>
            {
                { CrudButtonType.Anterior, new NavButtonConfig { Text = "Previous", IconName = "arrow_back" } },
                
                // Removed: Adicionar (Handled by FAB)
                
                { CrudButtonType.Editar, new NavButtonConfig { Text = "Edit", IconName = "edit" } },
                { CrudButtonType.Excluir, new NavButtonConfig { Text = "Delete", IconName = "delete" } },
                
                // Removed: Salvar (Handled by Header)

                { CrudButtonType.Proximo, new NavButtonConfig { Text = "Next", IconName = "arrow_forward" } },
            };
            return configs;
        }

        #region Logic

        private static void OnSelectionCountChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is CrudNavBarComponent navBar) navBar.UpdateLayoutAndButtons();
        }

        private static void OnModeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is CrudNavBarComponent navBar) navBar.UpdateLayoutAndButtons();
        }

        private void UpdateLayoutAndButtons()
        {
            try
            {
                if (navBarBehavior == null) return;

                if (!_isInitialized && Handler != null)
                {
                    navBarBehavior.ButtonClicked -= OnNavBarButtonClicked;
                    navBarBehavior.ButtonClicked += OnNavBarButtonClicked;
                    _isInitialized = true;
                }

                var currentCount = SelectionCount;
                var visibleButtons = new List<NavButtonConfig>();

                // FORM MODE: Hides Bar (Header handles actions)
                if (IsFormMode)
                {
                    visibleButtons.Clear();
                }
                else
                {
                    // LIST MODE
                    if (currentCount == 0)
                    {
                        // 0 Selection -> Hide NavBar (FAB handles Add)
                        visibleButtons.Clear();
                    }
                    else if (currentCount == 1)
                    {
                        // 1 Selection -> Edit + Delete
                        visibleButtons.Add(_buttonConfigs[CrudButtonType.Editar]);
                        visibleButtons.Add(_buttonConfigs[CrudButtonType.Excluir]);
                    }
                    else
                    {
                        // >1 Selection -> Delete Only
                        visibleButtons.Add(_buttonConfigs[CrudButtonType.Excluir]);
                    }
                }

                if (!ShouldRebuildButtons(currentCount) && !IsFormMode)
                {
                    UpdateNavBarVisibility();
                    return;
                }

                var columnDefinitions = new ColumnDefinitionCollection();
                foreach (var _ in visibleButtons) columnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

                navBarBehavior.CustomColumnDefinitions = columnDefinitions;
                navBarBehavior.Buttons = new ObservableCollection<NavButtonConfig>(visibleButtons);

                _lastProcessedSelectionCount = currentCount;
                _hasProcessedFirstUpdate = true;

                UpdateNavBarVisibility();

                if (visibleButtons.Count > 0)
                {
                    _ = Task.Run(async () => await navBarBehavior.ShowAsync());
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error UpdateLayout: {ex.Message}");
            }
        }

        private void UpdateNavBarVisibility()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                bool hasButtons = navBarBehavior?.Buttons?.Count > 0;
                this.IsVisible = hasButtons;
            });
        }

        private bool ShouldRebuildButtons(int currentCount)
        {
            if (!_hasProcessedFirstUpdate) return true;
            int last = _lastProcessedSelectionCount;
            // Preserved optimization for transitions
            return (last == 0 && currentCount == 1) ||
                   (last == 1 && currentCount == 0) ||
                   (last == 1 && currentCount == 2) ||
                   (last == 2 && currentCount == 1);
        }

        private void OnNavBarButtonClicked(object sender, NavBarButtonClickedEventArgs e)
        {
            // ✅ CLEANED SWITCH: Removed Add and Save cases
            var buttonType = e.ButtonConfig.Text switch
            {
                "Previous" => CrudButtonType.Anterior,
                "Edit" => CrudButtonType.Editar,
                "Delete" => CrudButtonType.Excluir,
                "Next" => CrudButtonType.Proximo,
                _ => CrudButtonType.Proximo
            };
            ButtonClicked?.Invoke(this, buttonType);
        }

        #endregion

        public async Task ShowAsync()
        {
            this.IsVisible = true;
            if (navBarBehavior != null) await navBarBehavior.ShowAsync();
        }

        public async Task HideAsync()
        {
            if (navBarBehavior != null) await navBarBehavior.HideAsync();
            this.IsVisible = false;
        }
    }
}