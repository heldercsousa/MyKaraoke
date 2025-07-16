using MyKaraoke.View.Components; // Adicionado para IAnimatableNavBar
using System.Collections.ObjectModel;
using System.Windows.Input;
using MyKaraoke.View.Animations;

namespace MyKaraoke.View.Components
{
    // 1. Implementa a interface IAnimatableNavBar para consistência arquitetural
    public partial class InactiveQueueBottomNav : ContentView, IAnimatableNavBar
    {
        // Eventos e Comandos permanecem os mesmos
        public event EventHandler LocaisClicked, BandokeClicked, NovaFilaClicked, HistoricoClicked, AdministrarClicked;
        public ICommand LocaisCommand { get; private set; }
        public ICommand BandokeCommand { get; private set; }
        public ICommand NovaFilaCommand { get; private set; }
        public ICommand HistoricoCommand { get; private set; }
        public ICommand AdministrarCommand { get; private set; }

        private bool _isInitialized = false;

        public InactiveQueueBottomNav()
        {
            InitializeComponent();
            InitializeCommands();
            // A inicialização dos botões agora é feita de forma segura no OnHandlerChanged.
        }

        // Método único que configura todos os botões e os passa para a base
        private void InitializeNavBar()
        {
            var buttons = new ObservableCollection<NavButtonConfig>
            {
                // Botões Regulares
                NavButtonConfig.Regular("Locais", "spot.png", LocaisCommand),
                NavButtonConfig.Regular("Bandokê", "musicos.png", BandokeCommand),

                // Botão ESPECIAL com configuração própria
                new NavButtonConfig
                {
                    Text = "Nova Fila",
                    IsSpecial = true,
                    CenterContent = "+",
                    Command = new Command(() => NovaFilaClicked?.Invoke(this, EventArgs.Empty)), // Aponta para o evento
                    GradientStyle = SpecialButtonGradientType.Yellow,
                    SpecialAnimationTypes = SpecialButtonAnimationType.ShowHide | SpecialButtonAnimationType.Pulse
                },

                // Botões Regulares
                NavButtonConfig.Regular("Histórico", "historico.png", HistoricoCommand),
                NavButtonConfig.Regular("Administrar", "manage.png", AdministrarCommand)
            };

            // Delega a configuração para o BaseNavBarComponent
            baseNavBar.Buttons = buttons;
            baseNavBar.ButtonClicked += OnBaseNavBarButtonClicked; // Garanta que esta linha exista.
        }

        // 3. Padrão de inicialização robusto no OnHandlerChanged
        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();
            if (Handler != null && !_isInitialized)
            {
                InitializeNavBar();
                _isInitialized = true;
                System.Diagnostics.Debug.WriteLine("✅ InactiveQueueBottomNav inicializado e configurado com sucesso.");
            }
        }

        // --- Implementação da Interface IAnimatableNavBar ---

        // A implementação da interface simplesmente delega para o BaseNavBarComponent
        public Task ShowAsync()
        {
            this.IsVisible = true;
            return baseNavBar.ShowAsync();
        }


        public async Task HideAsync()
        {
            await baseNavBar.HideAsync();
            this.IsVisible = false;
        }


        // O resto do código (InitializeCommands, handlers de clique, etc.) permanece o mesmo.
        #region Initialization and Event Handlers

        private void InitializeCommands()
        {
            LocaisCommand = new Command(() => LocaisClicked?.Invoke(this, EventArgs.Empty));
            BandokeCommand = new Command(() => BandokeClicked?.Invoke(this, EventArgs.Empty));
            NovaFilaCommand = new Command(() => NovaFilaClicked?.Invoke(this, EventArgs.Empty));
            HistoricoCommand = new Command(() => HistoricoClicked?.Invoke(this, EventArgs.Empty));
            AdministrarCommand = new Command(() => AdministrarClicked?.Invoke(this, EventArgs.Empty));
        }

        private void OnBaseNavBarButtonClicked(object sender, NavBarButtonClickedEventArgs e)
        {
            switch (e.ButtonConfig.Text)
            {
                case "Locais": LocaisClicked?.Invoke(this, EventArgs.Empty); break;
                case "Bandokê": BandokeClicked?.Invoke(this, EventArgs.Empty); break;
                case "Nova Fila": NovaFilaClicked?.Invoke(this, EventArgs.Empty); break;
                case "Histórico": HistoricoClicked?.Invoke(this, EventArgs.Empty); break;
                case "Administrar": AdministrarClicked?.Invoke(this, EventArgs.Empty); break;
            }
        }

        private async Task OnLocaisClickedAsync()
        {
            if (this.Parent is Page parentPage)
            {
                await parentPage.Navigation.PushAsync(new SpotPage());
                LocaisClicked?.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}