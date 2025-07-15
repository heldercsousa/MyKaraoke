using Microsoft.Maui.Controls;
using MyKaraoke.View.Animations;
using System.Collections.ObjectModel;
using System.Windows.Input;
using MauiView = Microsoft.Maui.Controls.View;

namespace MyKaraoke.View.Components
{
    public partial class BaseNavBarComponent : ContentView
    {
        #region Bindable Properties

        public static readonly BindableProperty ButtonsProperty =
            BindableProperty.Create(nameof(Buttons), typeof(ObservableCollection<NavButtonConfig>), typeof(BaseNavBarComponent), null, propertyChanged: OnButtonsChanged);

        public static readonly BindableProperty IsAnimatedProperty =
            BindableProperty.Create(nameof(IsAnimated), typeof(bool), typeof(BaseNavBarComponent), true);

        public static readonly BindableProperty ShowAnimationDelayProperty =
            BindableProperty.Create(nameof(ShowAnimationDelay), typeof(int), typeof(BaseNavBarComponent), 100);
        
        // NOVA PROPRIEDADE para aceitar definições de coluna customizadas
        public static readonly BindableProperty CustomColumnDefinitionsProperty =
            BindableProperty.Create(nameof(CustomColumnDefinitions), typeof(ColumnDefinitionCollection), typeof(BaseNavBarComponent), null);

        
        #endregion

        #region Properties

        public ObservableCollection<NavButtonConfig> Buttons
        {
            get => (ObservableCollection<NavButtonConfig>)GetValue(ButtonsProperty);
            set => SetValue(ButtonsProperty, value);
        }

        public bool IsAnimated
        {
            get => (bool)GetValue(IsAnimatedProperty);
            set => SetValue(IsAnimatedProperty, value);
        }

        public int ShowAnimationDelay
        {
            get => (int)GetValue(ShowAnimationDelayProperty);
            set => SetValue(ShowAnimationDelayProperty, value);
        }

        public ColumnDefinitionCollection CustomColumnDefinitions
        {
            get => (ColumnDefinitionCollection)GetValue(CustomColumnDefinitionsProperty);
            set => SetValue(CustomColumnDefinitionsProperty, value);

        }
        #endregion

        #region Events

        public event EventHandler<NavBarButtonClickedEventArgs> ButtonClicked;

        #endregion

        #region Private Fields

        private AnimationManager _animationManager;
        private readonly List<MauiView> _buttonViews = new();
        private bool _isShown = false;
        private bool _isAnimating = false;
        private bool _hasBeenInitialized = false; // ✅ NOVO: Previne recriações desnecessárias

        #endregion

        public BaseNavBarComponent()
        {
            InitializeComponent();
            _animationManager = new AnimationManager($"BaseNavBar_{GetHashCode()}");

            // Inicializa lista vazia se não foi definida
            if (Buttons == null)
            {
                Buttons = new ObservableCollection<NavButtonConfig>();
            }

            // ✅ CORREÇÃO: Aplica estado inicial imediatamente
            ApplyInitialState();

            System.Diagnostics.Debug.WriteLine($"BaseNavBarComponent construtor concluído - Hash: {GetHashCode()}");
        }

        /// <summary>
        /// ✅ NOVO MÉTODO: Aplica estado inicial para toda a navbar (escondida)
        /// </summary>
        private void ApplyInitialState()
        {
            try
            {
                // Estado inicial: navbar visível mas todos os botões começam escondidos
                this.IsVisible = true;
                _isShown = false;
                _isAnimating = false;
                System.Diagnostics.Debug.WriteLine("BaseNavBarComponent: Estado inicial aplicado (navbar visível, botões serão configurados individualmente)");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao aplicar estado inicial da navbar: {ex.Message}");
            }
        }

        #region Property Changed Handlers

        private static void OnButtonsChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is BaseNavBarComponent navBar)
            {
                navBar.RebuildButtons();
            }
        }

        #endregion

        #region Button Management

        private void RebuildButtons()
        {
            try
            {
                // ✅ PROTEÇÃO: Evita reconstrução se já foi inicializado E tem botões
                if (_hasBeenInitialized && _buttonViews.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"BaseNavBarComponent: RebuildButtons IGNORADO - já inicializado com {_buttonViews.Count} botões");
                    return;
                }

                // Limpa botões existentes
                ClearButtons();

                if (Buttons == null || Buttons.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("BaseNavBarComponent: Nenhum botão configurado");
                    return;
                }

                // Configura colunas do grid baseado na quantidade de botões
                SetupGridColumns(Buttons.Count);

                // Cria e adiciona botões
                for (int i = 0; i < Buttons.Count; i++)
                {
                    var buttonConfig = Buttons[i];
                    var buttonView = CreateButtonView(buttonConfig, i);

                    if (buttonView != null)
                    {
                        // ✅ CORREÇÃO: Verifica se o elemento já foi adicionado
                        if (buttonView.Parent == null) // Só adiciona se não tem pai
                        {
                            Grid.SetColumn(buttonView, i);
                            buttonsGrid.Children.Add(buttonView);
                            _buttonViews.Add(buttonView);

                            // ✅ DEBUG: Log detalhado do botão criado
                            string buttonText = GetButtonText(buttonView);
                            System.Diagnostics.Debug.WriteLine($"✅ Botão '{buttonText}' criado: Visible={buttonView.IsVisible}, Opacity={buttonView.Opacity}, Parent={buttonView.Parent != null}");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"⚠️ Botão {i} ignorado - já tem pai");
                        }
                    }
                }

                _hasBeenInitialized = true; // ✅ Marca como inicializado
                System.Diagnostics.Debug.WriteLine($"BaseNavBarComponent: {_buttonViews.Count} botões criados com sucesso");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao reconstruir botões: {ex.Message}");
            }
        }

        private void ClearButtons()
        {
            try
            {
                // ✅ PROTEÇÃO: Só limpa se realmente houver botões
                if (_buttonViews.Count == 0 && buttonsGrid.Children.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("BaseNavBarComponent: ClearButtons ignorado - já limpo");
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"BaseNavBarComponent: Limpando {_buttonViews.Count} botões existentes");

                buttonsGrid.Children.Clear();
                buttonsGrid.ColumnDefinitions.Clear();
                _buttonViews.Clear();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao limpar botões: {ex.Message}");
            }
        }

        // MÉTODO MODIFICADO para usar as colunas customizadas se elas existirem
        private void SetupGridColumns(int buttonCount)
        {
            try
            {
                buttonsGrid.ColumnDefinitions.Clear();

                // SE foram passadas colunas customizadas, usa elas.
                if (CustomColumnDefinitions != null && CustomColumnDefinitions.Any())
                {
                    foreach (var columnDef in CustomColumnDefinitions)
                    {
                        buttonsGrid.ColumnDefinitions.Add(columnDef);
                    }
                    System.Diagnostics.Debug.WriteLine($"BaseNavBar: Grid configurado com {CustomColumnDefinitions.Count} colunas customizadas.");
                }
                // SENÃO, mantém o comportamento padrão (fallback para outras navbars)
                else
                {
                    for (int i = 0; i < buttonCount; i++)
                    {
                        buttonsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
                    }
                    System.Diagnostics.Debug.WriteLine($"BaseNavBar: Grid configurado com {buttonCount} colunas padrão (*).");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao configurar colunas do grid: {ex.Message}");
            }
        }

        private MauiView CreateButtonView(NavButtonConfig config, int index)
        {
            try
            {
                MauiView buttonView;

                if (config.IsSpecial)
                {
                    buttonView = CreateSpecialButton(config, index);
                }
                else
                {
                    buttonView = CreateRegularButton(config, index);
                }

                // ✅ DEBUG: Log detalhado da criação
                if (buttonView != null)
                {
                    string buttonText = GetButtonText(buttonView);
                    System.Diagnostics.Debug.WriteLine($"🔧 Criando botão {index}: '{buttonText}' (IsSpecial: {config.IsSpecial})");

                    // ✅ VERIFICAÇÃO: Estado inicial forçado aqui também
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        if (buttonView.Opacity != 0.0 || buttonView.TranslationY != 60)
                        {
                            buttonView.Opacity = 0.0;
                            buttonView.TranslationY = 60;
                            buttonView.IsVisible = true;
                            System.Diagnostics.Debug.WriteLine($"🔧 Estado inicial FORÇADO para '{buttonText}': Opacity=0, TranslationY=60");
                        }
                    });
                }

                return buttonView;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao criar botão {index}: {ex.Message}");
                return null;
            }
        }

        private MauiView CreateRegularButton(NavButtonConfig config, int index)
        {
            var button = new NavButtonComponent
            {
                IconSource = config.IconSource,
                Text = config.Text,
                Command = config.Command,
                CommandParameter = config.CommandParameter,
                IsAnimated = IsAnimated && config.IsAnimated,
                AnimationTypes = config.AnimationTypes,
                ShowDelay = 0
            };

            // Conecta evento
            button.ButtonClicked += (s, e) => OnButtonClicked(config, e.Parameter);

            return button;
        }

        private MauiView CreateSpecialButton(NavButtonConfig config, int index)
        {
            var button = new SpecialNavButtonComponent
            {
                Text = config.Text,
                CenterContent = config.CenterContent,
                CenterIconSource = config.CenterIconSource,
                Command = config.Command,
                CommandParameter = config.CommandParameter,
                GradientStyle = config.GradientStyle,
                IsAnimated = IsAnimated && config.IsAnimated,
                AnimationTypes = config.SpecialAnimationTypes,
                ShowDelay = 0
            };

            // Conecta evento
            button.ButtonClicked += (s, e) => OnButtonClicked(config, e.Parameter);

            return button;
        }

        private void OnButtonClicked(NavButtonConfig config, object parameter)
        {
            try
            {
                // Dispara evento da navbar
                ButtonClicked?.Invoke(this, new NavBarButtonClickedEventArgs(config, parameter));

                System.Diagnostics.Debug.WriteLine($"BaseNavBarComponent: Botão '{config.Text}' clicado");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no evento de clique do botão: {ex.Message}");
            }
        }

        /// <summary>
        /// ✅ NOVO MÉTODO: Extrai o texto do botão para debug
        /// </summary>
        private string GetButtonText(MauiView buttonView)
        {
            if (buttonView is NavButtonComponent nav)
                return nav.Text ?? "nav";
            else if (buttonView is SpecialNavButtonComponent special)
                return special.Text ?? "especial";
            return "desconhecido";
        }

        #endregion

        #region Animation Methods

        /// <summary>
        /// ✅ CORRIGIDO: Mostra toda a navbar com animação escalonada dos botões
        /// Agora com proteção contra múltiplas execuções simultâneas
        /// </summary>
        public async Task ShowAsync()
        {
            // ✅ PROTEÇÃO CRÍTICA: Impede múltiplas execuções simultâneas
            if (_isShown || _isAnimating)
            {
                System.Diagnostics.Debug.WriteLine("⚠️ BaseNavBarComponent: ShowAsync IGNORADO - já mostrado ou animando");
                return;
            }

            _isAnimating = true; // Marca como "animando"

            try
            {
                System.Diagnostics.Debug.WriteLine($"BaseNavBarComponent: Iniciando ShowAsync com {_buttonViews.Count} botões");

                this.IsVisible = true;

                // ✅ VERIFICAÇÃO: Se não há botões, não há o que animar
                if (_buttonViews.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("⚠️ BaseNavBarComponent: Nenhum botão para animar - abortando ShowAsync");
                    _isShown = true;
                    return;
                }

                // ✅ CORREÇÃO CRÍTICA: Força estado inicial em TODOS os botões ANTES das animações
                await EnsureInitialStateForAllButtons();

                if (IsAnimated && HardwareDetector.SupportsAnimations && _buttonViews.Any())
                {
                    System.Diagnostics.Debug.WriteLine("BaseNavBarComponent: Condições atendidas - executando animações escalonadas SUTIS");

                    // ✅ CORREÇÃO: Executa animações sequenciais com delay SUTIL
                    var showTasks = new List<Task>();
                    const int SUBTLE_DELAY = 250; // ✅ 250ms entre cada botão

                    for (int i = 0; i < _buttonViews.Count; i++)
                    {
                        var buttonView = _buttonViews[i];
                        var subtleDelay = (i + 1) * SUBTLE_DELAY;

                        string buttonText = GetButtonText(buttonView);
                        System.Diagnostics.Debug.WriteLine($"BaseNavBarComponent: Programando animação SUTIL do botão {i} ({buttonText}) com delay {subtleDelay}ms");

                        if (buttonView is NavButtonComponent regularButton)
                        {
                            showTasks.Add(DelayedShowButton(regularButton, subtleDelay));
                        }
                        else if (buttonView is SpecialNavButtonComponent specialButton)
                        {
                            showTasks.Add(DelayedShowSpecialButton(specialButton, subtleDelay));
                        }
                    }

                    // ✅ EXECUÇÃO: Todas as animações em paralelo (cada uma com seu próprio delay sutil)
                    await Task.WhenAll(showTasks);

                    System.Diagnostics.Debug.WriteLine("BaseNavBarComponent: Todas as animações de show SUTIS concluídas");

                    // ✅ CORREÇÃO: Inicia animações especiais APÓS todos os botões aparecerem
                    await Task.Delay(100); // Pequeno delay antes das animações especiais
                    await StartSpecialAnimations();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("BaseNavBarComponent: Hardware limitado ou animações desabilitadas - mostrando botões diretamente");
                    // Hardware limitado: apenas torna todos os botões visíveis
                    foreach (var buttonView in _buttonViews)
                    {
                        buttonView.IsVisible = true;
                        buttonView.Opacity = 1;
                    }
                }

                _isShown = true;
                System.Diagnostics.Debug.WriteLine("BaseNavBarComponent: ShowAsync concluído com sucesso");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao mostrar BaseNavBarComponent: {ex.Message}");
                this.IsVisible = true;
                _isShown = true;
            }
            finally
            {
                _isAnimating = false; // ✅ SEMPRE libera o lock de animação
            }
        }

        /// <summary>
        /// ✅ CORRIGIDO: Garante que todos os botões tenham estado inicial correto ANTES das animações
        /// Isso previne o "piscar" onde o botão aparece na posição final e depois anima
        /// </summary>
        private async Task EnsureInitialStateForAllButtons()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("BaseNavBarComponent: Verificando estado inicial de todos os botões...");

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    foreach (var buttonView in _buttonViews)
                    {
                        string buttonText = GetButtonText(buttonView);

                        // ✅ FORÇA estado inicial se necessário
                        if (buttonView.Opacity != 0.0 || buttonView.TranslationY != 60)
                        {
                            buttonView.Opacity = 0.0;
                            buttonView.TranslationY = 60;
                            buttonView.IsVisible = true;
                            System.Diagnostics.Debug.WriteLine($"🔧 RESET estado para '{buttonText}': Opacity=0, TranslationY=60");
                        }

                        System.Diagnostics.Debug.WriteLine($"Estado atual do botão: {buttonText} (Opacity={buttonView.Opacity}, TranslationY={buttonView.TranslationY})");
                    }
                });

                System.Diagnostics.Debug.WriteLine("BaseNavBarComponent: Verificação de estado concluída");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro na verificação de estado: {ex.Message}");
            }
        }

        /// <summary>
        /// ✅ CORRIGIDO: Executa animação de um botão regular com delay SUTIL específico
        /// </summary>
        private async Task DelayedShowButton(NavButtonComponent button, int subtleDelay)
        {
            try
            {
                if (subtleDelay > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"⏰ Aguardando delay SUTIL de {subtleDelay}ms para botão '{button.Text ?? "sem nome"}'");
                    await Task.Delay(subtleDelay);
                }

                // ✅ VERIFICAÇÃO: Confirma estado inicial antes da animação
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    System.Diagnostics.Debug.WriteLine($"🔍 Estado PRÉ-animação '{button.Text ?? "sem nome"}': Opacity={button.Opacity}, TranslationY={button.TranslationY}");
                });

                System.Diagnostics.Debug.WriteLine($"🎯 Iniciando animação FADE+TRANSLATE do botão '{button.Text ?? "sem nome"}' (delay sutil: {subtleDelay}ms)");
                await button.ShowAsync();
                System.Diagnostics.Debug.WriteLine($"✅ Animação FADE+TRANSLATE do botão '{button.Text ?? "sem nome"}' concluída");

                // ✅ VERIFICAÇÃO: Confirma estado final após a animação
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    System.Diagnostics.Debug.WriteLine($"🔍 Estado PÓS-animação '{button.Text ?? "sem nome"}': Opacity={button.Opacity}, TranslationY={button.TranslationY}");
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erro na animação do botão regular: {ex.Message}");
            }
        }

        /// <summary>
        /// ✅ CORRIGIDO: Executa animação de um botão especial com delay SUTIL específico
        /// </summary>
        private async Task DelayedShowSpecialButton(SpecialNavButtonComponent button, int subtleDelay)
        {
            try
            {
                if (subtleDelay > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"⏰ Aguardando delay SUTIL de {subtleDelay}ms para botão especial '{button.Text ?? "sem nome"}'");
                    await Task.Delay(subtleDelay);
                }

                // ✅ VERIFICAÇÃO: Confirma estado inicial antes da animação
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    System.Diagnostics.Debug.WriteLine($"🔍 Estado PRÉ-animação especial '{button.Text ?? "sem nome"}': Opacity={button.Opacity}, TranslationY={button.TranslationY}");
                });

                System.Diagnostics.Debug.WriteLine($"🎯 Iniciando animação FADE+TRANSLATE+PULSE do botão especial '{button.Text ?? "sem nome"}' (delay sutil: {subtleDelay}ms)");
                await button.ShowAsync();
                System.Diagnostics.Debug.WriteLine($"✅ Animação FADE+TRANSLATE+PULSE do botão especial '{button.Text ?? "sem nome"}' concluída");

                // ✅ VERIFICAÇÃO: Confirma estado final após a animação
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    System.Diagnostics.Debug.WriteLine($"🔍 Estado PÓS-animação especial '{button.Text ?? "sem nome"}': Opacity={button.Opacity}, TranslationY={button.TranslationY}");
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erro na animação do botão especial: {ex.Message}");
            }
        }

        /// <summary>
        /// Esconde toda a navbar com animação
        /// </summary>
        public async Task HideAsync()
        {
            if (!_isShown)
                return;

            try
            {
                // Para animações especiais primeiro
                await StopSpecialAnimations();

                if (IsAnimated && HardwareDetector.SupportsAnimations)
                {
                    // Esconde botões simultaneamente
                    var hideTasks = new List<Task>();

                    foreach (var buttonView in _buttonViews)
                    {
                        if (buttonView is NavButtonComponent regularButton)
                        {
                            hideTasks.Add(regularButton.HideAsync());
                        }
                        else if (buttonView is SpecialNavButtonComponent specialButton)
                        {
                            hideTasks.Add(specialButton.HideAsync());
                        }
                    }

                    await Task.WhenAll(hideTasks);
                }

                this.IsVisible = false;
                _isShown = false;
                System.Diagnostics.Debug.WriteLine("BaseNavBarComponent escondida com animação");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao esconder BaseNavBarComponent: {ex.Message}");
                this.IsVisible = false;
                _isShown = false;
            }
        }

        /// <summary>
        /// Inicia animações especiais dos botões configurados
        /// </summary>
        public async Task StartSpecialAnimations()
        {
            if (!HardwareDetector.SupportsAnimations)
            {
                System.Diagnostics.Debug.WriteLine("🚫 BaseNavBarComponent: Hardware não suporta animações especiais - BYPASS ativo");
                return;
            }

            try
            {
                System.Diagnostics.Debug.WriteLine("🌟 BaseNavBarComponent: Iniciando animações especiais...");

                foreach (var buttonView in _buttonViews)
                {
                    if (buttonView is SpecialNavButtonComponent specialButton)
                    {
                        System.Diagnostics.Debug.WriteLine($"🎵 Iniciando animação especial para botão '{specialButton.Text ?? "sem nome"}'");
                        await specialButton.StartSpecialAnimationAsync();
                    }
                    else if (buttonView is NavButtonComponent regularButton)
                    {
                        // Verifica se o botão regular tem animação de pulse configurada
                        if (regularButton.AnimationTypes.HasFlag(NavButtonAnimationType.Pulse))
                        {
                            System.Diagnostics.Debug.WriteLine($"🎵 Iniciando animação especial para botão regular '{regularButton.Text ?? "sem nome"}'");
                            await regularButton.StartSpecialAnimationAsync();
                        }
                    }
                }

                System.Diagnostics.Debug.WriteLine("✅ BaseNavBarComponent: Animações especiais iniciadas");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erro ao iniciar animações especiais: {ex.Message}");
            }
        }

        /// <summary>
        /// Para animações especiais dos botões
        /// </summary>
        public async Task StopSpecialAnimations()
        {
            try
            {
                foreach (var buttonView in _buttonViews)
                {
                    if (buttonView is SpecialNavButtonComponent specialButton)
                    {
                        await specialButton.StopSpecialAnimationAsync();
                    }
                    else if (buttonView is NavButtonComponent regularButton)
                    {
                        await regularButton.StopAllAnimationsAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao parar animações especiais: {ex.Message}");
            }
        }

        /// <summary>
        /// Para todas as animações da navbar
        /// </summary>
        public async Task StopAllAnimationsAsync()
        {
            try
            {
                await _animationManager.StopAllAnimationsAsync();
                await StopSpecialAnimations();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao parar todas as animações: {ex.Message}");
            }
        }

        #endregion

        #region Lifecycle Methods

        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();

            if (Handler == null)
            {
                // Limpa animações quando o handler é removido
                _animationManager?.Dispose();
                System.Diagnostics.Debug.WriteLine("BaseNavBarComponent: Handler removido - limpando recursos");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"BaseNavBarComponent: Handler disponível - HasBeenInitialized: {_hasBeenInitialized}, ButtonCount: {_buttonViews.Count}");

                // ✅ CORREÇÃO CRÍTICA: SÓ reconstrói se REALMENTE não foi inicializado
                if (!_hasBeenInitialized || _buttonViews.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("BaseNavBarComponent: Primeira inicialização ou sem botões - reconstruindo");
                    ApplyInitialState();
                    _isShown = false;
                    _isAnimating = false;
                    RebuildButtons();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("BaseNavBarComponent: Já inicializado - IGNORANDO reconstrução para evitar duplicatas");
                }
            }
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (BindingContext == null)
            {
                // Para animações quando o contexto muda
                _ = Task.Run(StopAllAnimationsAsync);
            }
        }

        #endregion
    }

    #region Configuration Classes

    public class NavButtonConfig
    {
        public string Text { get; set; } = "";
        public string IconSource { get; set; } = "";
        public ICommand Command { get; set; }
        public object CommandParameter { get; set; }
        public bool IsAnimated { get; set; } = true;
        public NavButtonAnimationType AnimationTypes { get; set; } = NavButtonAnimationType.ShowHide;

        // Para botões especiais
        public bool IsSpecial { get; set; } = false;
        public string CenterContent { get; set; } = "+";
        public string CenterIconSource { get; set; } = "";
        public SpecialButtonGradientType GradientStyle { get; set; } = SpecialButtonGradientType.Yellow;
        public SpecialButtonAnimationType SpecialAnimationTypes { get; set; } = SpecialButtonAnimationType.ShowHide;

        // Factory methods para facilitar criação
        public static NavButtonConfig Regular(string text, string iconSource, ICommand command = null)
        {
            return new NavButtonConfig
            {
                Text = text,
                IconSource = iconSource,
                Command = command,
                IsSpecial = false,
                AnimationTypes = HardwareDetector.SupportsAnimations ? NavButtonAnimationType.ShowHide : NavButtonAnimationType.None
            };
        }

        public static NavButtonConfig Special(string text, string centerContent = "+", SpecialButtonGradientType gradientStyle = SpecialButtonGradientType.Yellow, ICommand command = null)
        {
            return new NavButtonConfig
            {
                Text = text,
                CenterContent = centerContent,
                Command = command,
                IsSpecial = true,
                GradientStyle = gradientStyle,
                SpecialAnimationTypes = HardwareDetector.SupportsAnimations
                    ? (SpecialButtonAnimationType.ShowHide | SpecialButtonAnimationType.Pulse)
                    : SpecialButtonAnimationType.None,
                IsAnimated = true
            };
        }
    }

    #endregion

    #region Event Args

    public class NavBarButtonClickedEventArgs : EventArgs
    {
        public NavButtonConfig ButtonConfig { get; }
        public object Parameter { get; }

        public NavBarButtonClickedEventArgs(NavButtonConfig buttonConfig, object parameter)
        {
            ButtonConfig = buttonConfig;
            Parameter = parameter;
        }
    }

    #endregion
}