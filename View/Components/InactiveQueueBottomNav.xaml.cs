using MyKaraoke.View.Behaviors;
using MyKaraoke.View.Components;
using System.Collections.ObjectModel;
using MyKaraoke.View.Animations;

namespace MyKaraoke.View.Components
{
    /// <summary>
    /// ✅ SIMPLIFICADO: Usa NavBarBehavior para eliminar duplicação
    /// Reduzido de 200+ linhas para ~60 linhas
    /// </summary>
    public partial class InactiveQueueBottomNav : ContentView, IAnimatableNavBar
    {
        #region Events

        public event EventHandler LocaisClicked, BandokeClicked, NovaFilaClicked, HistoricoClicked, AdministrarClicked;

        #endregion

        #region Private Fields

        private bool _isInitialized = false;

        #endregion

        public InactiveQueueBottomNav()
        {
            InitializeComponent();
        }

        #region Initialization

        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();

            if (Handler != null && !_isInitialized)
            {
                InitializeNavBar();
                _isInitialized = true;
                System.Diagnostics.Debug.WriteLine("✅ InactiveQueueBottomNav inicializado com NavBarBehavior");
            }
        }

        /// <summary>
        /// ✅ CONFIGURAÇÃO: Define todos os botões através do NavBarBehavior
        /// </summary>
        private void InitializeNavBar()
        {
            var buttons = new ObservableCollection<NavButtonConfig>
            {
                // Botões Regulares
                NavButtonConfig.Regular("Locais", "spot.png", new Command(() => LocaisClicked?.Invoke(this, EventArgs.Empty))),
                NavButtonConfig.Regular("Bandokê", "musicos.png", new Command(() => BandokeClicked?.Invoke(this, EventArgs.Empty))),

                // ✅ BOTÃO ESPECIAL: Nova Fila com animação pulse
                new NavButtonConfig
                {
                    Text = "Nova Fila",
                    IsSpecial = true,
                    CenterContent = "+",
                    Command = new Command(() => NovaFilaClicked?.Invoke(this, EventArgs.Empty)),
                    GradientStyle = SpecialButtonGradientType.Yellow,
                    SpecialAnimationTypes = SpecialButtonAnimationType.ShowHide | SpecialButtonAnimationType.Pulse,
                    IsAnimated = true
                },

                // Botões Regulares
                NavButtonConfig.Regular("Histórico", "historico.png", new Command(() => HistoricoClicked?.Invoke(this, EventArgs.Empty))),
                NavButtonConfig.Regular("Administrar", "manage.png", new Command(() => AdministrarClicked?.Invoke(this, EventArgs.Empty)))
            };

            // ✅ BEHAVIOR: Configura botões e eventos
            navBarBehavior.Buttons = buttons;
            navBarBehavior.ButtonClicked += OnNavBarButtonClicked;
        }

        #endregion

        #region Event Handlers

        private void OnNavBarButtonClicked(object sender, NavBarButtonClickedEventArgs e)
        {
            try
            {
                // ✅ DISPATCH: Mapeia cliques para eventos específicos
                switch (e.ButtonConfig.Text)
                {
                    case "Locais":
                        LocaisClicked?.Invoke(this, EventArgs.Empty);
                        break;
                    case "Bandokê":
                        BandokeClicked?.Invoke(this, EventArgs.Empty);
                        break;
                    case "Nova Fila":
                        NovaFilaClicked?.Invoke(this, EventArgs.Empty);
                        break;
                    case "Histórico":
                        HistoricoClicked?.Invoke(this, EventArgs.Empty);
                        break;
                    case "Administrar":
                        AdministrarClicked?.Invoke(this, EventArgs.Empty);
                        break;
                    default:
                        System.Diagnostics.Debug.WriteLine($"Botão desconhecido: {e.ButtonConfig.Text}");
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no clique da navbar: {ex.Message}");
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
            await NavBarExtensions.ShowAsync(navGrid);
        }

        /// <summary>
        /// ✅ DELEGADO: HideAsync via NavBarBehavior
        /// </summary>
        public async Task HideAsync()
        {
            await NavBarExtensions.HideAsync(navGrid);
            this.IsVisible = false;
        }

        #endregion
    }
}