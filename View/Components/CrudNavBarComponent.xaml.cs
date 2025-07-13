using MyKaraoke.View.Animations;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MyKaraoke.View.Components
{
    public partial class CrudNavBarComponent : ContentView
    {
        #region Events

        public event EventHandler NovoLocalClicked;

        #endregion

        #region Commands

        public ICommand NovoLocalCommand { get; private set; }

        #endregion

        #region Private Fields

        private bool _isInitialized = false;
        private bool _isShowing = false; // Proteção contra múltiplas execuções

        #endregion

        public CrudNavBarComponent()
        {
            try
            {
                InitializeComponent();
                InitializeCommands();
                SetupButtons();

                System.Diagnostics.Debug.WriteLine("CrudNavBar inicializado com sucesso");

                // Inicia animações com delay adequado APÓS inicialização completa
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Task.Delay(200); // Delay para garantir que layout esteja completamente pronto
                    await ShowAsync(); // Inicia animações automaticamente
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro na inicialização do CrudNavBar: {ex.Message}");
                throw;
            }
        }

        #region Initialization

        private void InitializeCommands()
        {
            NovoLocalCommand = new Command(async () => await OnNovoLocalClickedAsync());
        }

        private void SetupButtons()
        {
            try
            {
                var buttons = new ObservableCollection<NavButtonConfig>
                {
                    // Novo Local - botão centralizado com animações Fade + Translate
                    new NavButtonConfig
                    {
                        Text = "Novo Local",
                        IconSource = "add.png",
                        Command = NovoLocalCommand,
                        IsSpecial = false,
                        AnimationTypes = HardwareDetector.SupportsAnimations
                            ? (NavButtonAnimationType.Fade | NavButtonAnimationType.Translate)
                            : NavButtonAnimationType.None,
                        IsAnimated = true
                    }
                };

                // Configura navbar com botão único centralizado
                baseNavBar.Buttons = buttons;
                baseNavBar.IsAnimated = true;
                baseNavBar.ShowAnimationDelay = 80; // Delay sutil para animação

                // Conecta evento do componente base
                baseNavBar.ButtonClicked += OnBaseNavBarButtonClicked;

                _isInitialized = true;
                System.Diagnostics.Debug.WriteLine("CrudNavBar: Botão 'Novo Local' configurado com animações FADE+TRANSLATE");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao configurar botão CrudNavBar: {ex.Message}");
            }
        }

        #endregion

        #region Event Handlers

        private void OnBaseNavBarButtonClicked(object sender, NavBarButtonClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"CrudNavBar: Botão '{e.ButtonConfig.Text}' clicado");

                // Os commands já foram executados automaticamente pelo BaseNavBarComponent
                // Este evento é apenas para logging adicional se necessário
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no evento base navbar CrudNavBar: {ex.Message}");
            }
        }

        private async Task OnNovoLocalClickedAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Botão 'Novo Local' clicado no CrudNavBar - animação continua rodando");

                // ✅ COMPORTAMENTO CORRETO: Animação continua rodando durante clique
                // Ela só deve parar quando o botão desaparecer da tela (OnDisappearing da página pai)

                await Task.Run(() =>
                {
                    NovoLocalClicked?.Invoke(this, EventArgs.Empty);
                });

                System.Diagnostics.Debug.WriteLine("Evento NovoLocalClicked disparado - animação ainda ativa");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no clique 'Novo Local': {ex.Message}");
            }
        }

        #endregion

        #region Animation Methods

        /// <summary>
        /// Inicia a animação do botão Novo Local automaticamente
        /// Método público para ser chamado pela view pai (mantém compatibilidade)
        /// ✅ CORRIGIDO: Animação finita que para automaticamente
        /// </summary>
        public async Task StartNovoLocalAnimationAsync()
        {
            try
            {
                if (!_isInitialized)
                {
                    System.Diagnostics.Debug.WriteLine("⚠️ CrudNavBar ainda não inicializado, aguardando...");
                    await Task.Delay(200); // Delay para aguardar inicialização

                    // Verifica novamente após delay
                    if (!_isInitialized)
                    {
                        System.Diagnostics.Debug.WriteLine("❌ CrudNavBar não inicializou a tempo - abortando animação");
                        return;
                    }
                }

                // Só executa se o hardware suportar animações
                if (!HardwareDetector.SupportsAnimations)
                {
                    System.Diagnostics.Debug.WriteLine("🚫 Hardware não suporta animações - Novo Local animation BYPASS ativado");
                    return;
                }

                // Log do hardware para debug
                HardwareDetector.LogHardwareInfo();

                System.Diagnostics.Debug.WriteLine("🎬 Iniciando animação Novo Local com 2 animações (Fade + Translate) via BaseNavBarComponent");

                // Verifica se baseNavBar está disponível
                if (baseNavBar == null)
                {
                    System.Diagnostics.Debug.WriteLine("❌ baseNavBar não disponível - abortando animação");
                    return;
                }

                // ✅ CORREÇÃO: Inicia animações que param automaticamente após conclusão
                await baseNavBar.ShowAsync();

                System.Diagnostics.Debug.WriteLine("✅ Animação Novo Local iniciada com sucesso - animações param automaticamente após conclusão");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erro na animação Novo Local CrudNavBar: {ex.Message}");
            }
        }

        /// <summary>
        /// Para a animação do botão Novo Local
        /// Método público para ser chamado pela view pai (mantém compatibilidade)
        /// </summary>
        public async Task StopNovoLocalAnimationAsync()
        {
            try
            {
                if (baseNavBar != null)
                {
                    await baseNavBar.HideAsync();
                    System.Diagnostics.Debug.WriteLine("✅ Animação Novo Local parada no CrudNavBar");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erro ao parar animação CrudNavBar: {ex.Message}");
            }
        }

        /// <summary>
        /// Verifica se a animação está rodando
        /// Propriedade para compatibilidade com código existente
        /// </summary>
        public bool IsNovoLocalAnimationRunning
        {
            get
            {
                try
                {
                    return _isInitialized &&
                           baseNavBar != null &&
                           baseNavBar.IsVisible &&
                           baseNavBar.IsAnimated &&
                           HardwareDetector.SupportsAnimations;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Mostra a navbar com animação
        /// Agora com proteção correta baseada no estado dos botões
        /// </summary>
        public async Task ShowAsync()
        {
            // Proteção contra múltiplas execuções simultâneas
            if (_isShowing)
            {
                System.Diagnostics.Debug.WriteLine("⚠️ CrudNavBar: ShowAsync IGNORADO - já em execução");
                return;
            }

            _isShowing = true; // Marca como "em execução"

            try
            {
                System.Diagnostics.Debug.WriteLine("CrudNavBar: Iniciando ShowAsync");

                if (baseNavBar == null)
                {
                    System.Diagnostics.Debug.WriteLine("baseNavBar não disponível");
                    return;
                }

                // Força estado inicial da navbar ANTES das animações
                await ForceInitialStateAsync();

                // Só executa animações se o hardware suportar
                if (HardwareDetector.SupportsAnimations)
                {
                    System.Diagnostics.Debug.WriteLine("CrudNavBar: Iniciando animações da navbar - hardware adequado detectado");
                    await baseNavBar.ShowAsync();
                    System.Diagnostics.Debug.WriteLine("CrudNavBar mostrada com animação - botão Novo Local com 2 animações ativas");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("CrudNavBar: Hardware limitado - navbar mostrada sem animações (BYPASS ativo)");
                    baseNavBar.IsVisible = true;
                    // Em hardware limitado, apenas torna visível sem animações
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao mostrar CrudNavBar: {ex.Message}");
                // Fallback: apenas torna visível
                if (baseNavBar != null)
                {
                    baseNavBar.IsVisible = true;
                }
            }
            finally
            {
                _isShowing = false; // Sempre libera o lock
            }
        }

        /// <summary>
        /// Força estado inicial da navbar para prevenir "piscar"
        /// </summary>
        private async Task ForceInitialStateAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("🔧 CrudNavBar: Forçando estado inicial da navbar...");

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    if (baseNavBar != null)
                    {
                        baseNavBar.IsVisible = true;
                        baseNavBar.Opacity = 1; // A navbar em si deve estar visível

                        System.Diagnostics.Debug.WriteLine("🔧 CrudNavBar: Estado inicial da navbar aplicado");
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erro ao forçar estado inicial: {ex.Message}");
            }
        }

        /// <summary>
        /// Esconde a navbar com animação
        /// </summary>
        public async Task HideAsync()
        {
            try
            {
                if (baseNavBar != null)
                {
                    await baseNavBar.HideAsync();
                    System.Diagnostics.Debug.WriteLine("CrudNavBar escondida com animação");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao esconder CrudNavBar: {ex.Message}");
            }
        }

        #endregion

        #region Lifecycle Methods

        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();

            if (Handler == null)
            {
                System.Diagnostics.Debug.WriteLine("CrudNavBar handler removido");
                _isShowing = false; // Reset do estado quando handler é removido
            }
            else if (!_isInitialized)
            {
                // Tenta configurar botões novamente se não foi inicializado
                SetupButtons();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("🔄 CrudNavBar handler disponível - PRONTO");
            }
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (BindingContext == null)
            {
                // Para animações quando o contexto muda
                _ = Task.Run(StopNovoLocalAnimationAsync);
                System.Diagnostics.Debug.WriteLine("Animação parada devido a mudança de BindingContext");
            }
        }

        #endregion
    }
}