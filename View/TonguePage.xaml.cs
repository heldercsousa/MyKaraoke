using Microsoft.Maui.Controls;
using MyVocaList.Contracts.Models;
using MyVocaList.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MyVocaList.View
{
    public partial class TonguePage : ContentPage
    {
        private ObservableCollection<LanguageItem> languages;
        private ILanguageService? _languageService;
        private ServiceProvider? _serviceProvider;
        private string selectedLanguage = "en"; // Idioma padrão

        // Dicionário de traduções para a palavra "Language" em diferentes idiomas
        private readonly Dictionary<string, string> languageTranslations = new Dictionary<string, string>
        {
            { "en", "Language" },
            { "pt", "Língua" },
            { "es", "Idioma" },
            { "fr", "Langue" },
            { "de", "Sprache" },
            { "zh", "语言" },
            { "ja", "言語" },
            { "ko", "언어" },
            { "ar", "اللغة" },
            { "ru", "Язык" },
            { "hi", "भाषा" }
        };

        public TonguePage()
        {
            InitializeComponent();

            // Inicialização da lista de idiomas
            languages = new ObservableCollection<LanguageItem>
            {
                new LanguageItem { Code = "en", Name = "English", Countries = "United States / United Kingdom", Flag = "🇺🇸 🇬🇧", IsSelected = true },
                new LanguageItem { Code = "pt", Name = "Português", Countries = "Brasil / Portugal", Flag = "🇧🇷 🇵🇹" },
                new LanguageItem { Code = "es", Name = "Español", Countries = "España / América Latina", Flag = "🇪🇸 🇲🇽" },
                new LanguageItem { Code = "fr", Name = "Français", Countries = "France / Canada", Flag = "🇫🇷 🇨🇦" },
                new LanguageItem { Code = "de", Name = "Deutsch", Countries = "Deutschland / Österreich", Flag = "🇩🇪 🇦🇹" },
                new LanguageItem { Code = "zh", Name = "简体中文", Countries = "中国大陆 / 新加坡", Flag = "🇨🇳 🇸🇬" },
                new LanguageItem { Code = "ja", Name = "日本語", Countries = "日本", Flag = "🇯🇵" },
                new LanguageItem { Code = "ko", Name = "한국어", Countries = "대한민국", Flag = "🇰🇷" },
                new LanguageItem { Code = "ar", Name = "العربية", Countries = "السعودية / مصر", Flag = "🇸🇦 🇪🇬" },
                new LanguageItem { Code = "ru", Name = "Русский", Countries = "Россия", Flag = "🇷🇺" },
                new LanguageItem { Code = "hi", Name = "हिन्दी", Countries = "भारत", Flag = "🇮🇳" }
            };
        }

        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();

            if (Handler != null)
            {
                try
                {
                    // Inicializa o ServiceProvider quando o Handler estiver disponível
                    _serviceProvider = ServiceProvider.FromPage(this);
                    _languageService = _serviceProvider.GetService<ILanguageService>();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Erro ao inicializar serviços: {ex.Message}");
                }
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Aguarda um tempo para garantir que a UI esteja pronta
            await Task.Delay(50);

            // Carrega os botões de idioma quando a página aparece
            CreateLanguageButtons();

            // Debug para verificar se os botões foram criados
            System.Diagnostics.Debug.WriteLine($"Botões criados: {languagesContainer?.Children.Count ?? 0}");
        }

        private void CreateLanguageButtons()
        {
            try
            {
                // Limpa os botões existentes
                if (languagesContainer != null)
                {
                    languagesContainer.Children.Clear();
                    System.Diagnostics.Debug.WriteLine("Container limpo com sucesso");

                    // Cria os botões de idioma
                    foreach (var language in languages)
                    {
                        // Debug para verificar cada item sendo processado
                        System.Diagnostics.Debug.WriteLine($"Criando botão para: {language.Name}, Bandeira: {language.Flag}");

                        var frame = new Frame
                        {
                            HeightRequest = 55,
                            CornerRadius = 40,
                            Margin = new Thickness(0),
                            Padding = new Thickness(30, 5, 30, 5),
                            BorderColor = language.IsSelected ? Colors.Transparent : Color.FromArgb("#6c4794"),
                            HasShadow = language.IsSelected
                        };

                        // Aplicar o background como SolidColorBrush ou o gradiente
                        if (language.IsSelected)
                        {
                            // Verificamos se o recurso existe antes de tentar acessá-lo
                            object gradientResource = null;
                            if (Application.Current != null && Application.Current.Resources.TryGetValue("SelectedButtonGradient", out gradientResource) && gradientResource is Brush)
                            {
                                frame.Background = gradientResource as Brush;
                            }
                            else
                            {
                                // Fallback se o recurso não existir
                                frame.Background = new SolidColorBrush(Color.FromArgb("#e52067"));
                            }
                        }
                        else
                        {
                            frame.Background = new SolidColorBrush(Color.FromArgb("#4c426f"));
                        }

                        var grid = new Grid
                        {
                            ColumnDefinitions =
                            {
                                new ColumnDefinition { Width = GridLength.Star },
                                new ColumnDefinition { Width = GridLength.Auto }
                            }
                        };

                        // Nome do idioma
                        var nameLabel = new Label
                        {
                            Text = language.Name,
                            FontAttributes = FontAttributes.Bold,
                            FontSize = 18,
                            TextColor = Colors.White,
                            HorizontalOptions = LayoutOptions.Start,
                            VerticalOptions = LayoutOptions.Center
                        };

                        // Para idiomas RTL (árabe), alinhamento à direita
                        if (language.Code == "ar")
                        {
                            nameLabel.HorizontalOptions = LayoutOptions.End;
                            nameLabel.FlowDirection = FlowDirection.RightToLeft;
                        }

                        // Bandeira do idioma
                        var flagLabel = new Label
                        {
                            Text = language.Flag,
                            FontSize = 20,
                            TextColor = Colors.White,
                            HorizontalOptions = LayoutOptions.End,
                            VerticalOptions = LayoutOptions.Center
                        };

                        System.Diagnostics.Debug.WriteLine($"Texto do label: '{nameLabel.Text}', Bandeira: '{flagLabel.Text}'");

                        // Adiciona os elementos ao grid usando a sintaxe correta para .NET MAUI
                        grid.Add(nameLabel, 0, 0);
                        grid.Add(flagLabel, 1, 0);

                        // Configura o frame com o grid
                        frame.Content = grid;

                        // Adicionar tap recognizer
                        var languageCode = language.Code;
                        var tapGesture = new TapGestureRecognizer();

                        tapGesture.Tapped += async (s, e) =>
                        {
                            await SelectLanguage(languageCode);
                        };

                        frame.GestureRecognizers.Add(tapGesture);

                        // Adiciona o frame ao container
                        languagesContainer.Children.Add(frame);
                        System.Diagnostics.Debug.WriteLine($"Botão para {language.Name} adicionado com sucesso");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("ERRO: languagesContainer é null!");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao criar botões de idioma: {ex.Message}\nStack: {ex.StackTrace}");
            }
        }

        private async Task SelectLanguage(string languageCode)
        {
            try
            {
                // Atualiza a seleção de idioma
                foreach (var language in languages)
                {
                    language.IsSelected = (language.Code == languageCode);
                    if (language.IsSelected)
                    {
                        selectedLanguage = language.Code;
                    }
                }

                // Recria os botões para refletir a nova seleção visual
                CreateLanguageButtons();

                // Atualiza o título para mostrar a tradução (sem salvar no banco)
                UpdateLanguageTitle(selectedLanguage);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao selecionar idioma: {ex.Message}");
            }
        }

        private void UpdateLanguageTitle(string languageCode)
        {
            try
            {
                // Atualiza o texto do título com base no idioma selecionado
                if (languageTranslations.TryGetValue(languageCode, out string translation))
                {
                    titleText.Text = translation;

                    // Configurações específicas para RTL (árabe)
                    if (languageCode == "ar")
                    {
                        titleText.HorizontalOptions = LayoutOptions.End;
                        titleText.FlowDirection = FlowDirection.RightToLeft;
                    }
                    else
                    {
                        titleText.HorizontalOptions = LayoutOptions.Start;
                        titleText.FlowDirection = FlowDirection.LeftToRight;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao atualizar título de idioma: {ex.Message}");
            }
        }

        private async Task SaveSelectedLanguageAsync(string languageCode)
        {
            try
            {
                // Salva o idioma nas preferências do aplicativo
                Preferences.Set("UserLanguage", languageCode);

                // Usa o serviço de idioma para persistir a seleção
                if (_languageService != null)
                {
                    await _languageService.SetUserLanguageAsync(languageCode);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Serviço de idioma não disponível");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao salvar idioma: {ex.Message}");
            }
        }

        private void ApplyLanguageToApp(string languageCode)
        {
            // Aplica o idioma ao aplicativo
            System.Diagnostics.Debug.WriteLine($"Idioma {languageCode} aplicado ao app");
        }

        // Botão voltar da UI - corrigido para usar NavigationPage
        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await NavigateToStackPage();
        }

        // Botão físico do Android - corrigido para usar NavigationPage
        protected override bool OnBackButtonPressed()
        {
            MainThread.BeginInvokeOnMainThread(async () => {
                await NavigateToStackPage();
            });

            return true; // Impede o comportamento padrão
        }

        // Método corrigido de navegação usando Application.Current.MainPage com NavigationPage
        private async Task NavigateToStackPage()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("[DEBUG] TonguePage: Navegando para StackPage");

                // Encontra o idioma selecionado
                var selectedItem = languages.FirstOrDefault(l => l.IsSelected);
                if (selectedItem == null) return;

                // Encontra o nome traduzido do idioma
                string languageDisplayName = selectedItem.Name;
                string englishName = GetEnglishNameForLanguage(selectedItem.Code);

                // Exibe diálogo de confirmação sempre em inglês
                bool confirmed = await DisplayAlert(
                    "Confirmation",
                    $"Confirm {englishName} ({languageDisplayName}) language?",
                    "Confirm",
                    "Cancel"
                );

                if (confirmed)
                {
                    // Salva a preferência no banco de dados
                    await SaveSelectedLanguageAsync(selectedLanguage);

                    // Aplica o idioma
                    ApplyLanguageToApp(selectedLanguage);

                    // Usa Application.Current.MainPage com NavigationPage para corrigir navegação
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        Application.Current.MainPage = new NavigationPage(new StackPage());
                        System.Diagnostics.Debug.WriteLine("[SUCCESS] Navegação para StackPage realizada");
                    });
                }
                // Se cancelar, permanece na página atual
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR] Erro ao navegar para StackPage: {ex.Message}");

                // Fallback simples
                try
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        Application.Current.MainPage = new StackPage();
                    });
                }
                catch (Exception fallbackEx)
                {
                    System.Diagnostics.Debug.WriteLine($"[ERROR] Fallback também falhou: {fallbackEx.Message}");
                }
            }
        }

        private string GetEnglishNameForLanguage(string languageCode)
        {
            // Mapeia os códigos de idioma para nomes em inglês
            Dictionary<string, string> englishNames = new Dictionary<string, string>
            {
                { "en", "English" },
                { "pt", "Portuguese" },
                { "es", "Spanish" },
                { "fr", "French" },
                { "de", "German" },
                { "zh", "Chinese" },
                { "ja", "Japanese" },
                { "ko", "Korean" },
                { "ar", "Arabic" },
                { "ru", "Russian" },
                { "hi", "Hindi" }
            };

            if (englishNames.TryGetValue(languageCode, out string name))
                return name;

            return languageCode; // fallback para o código se não encontrar nome
        }
    }

    // Modelo para representar um item de idioma
    public class LanguageItem : INotifyPropertyChanged
    {
        private bool isSelected;

        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Countries { get; set; } = string.Empty;
        public string Flag { get; set; } = string.Empty;

        public bool IsSelected
        {
            get => isSelected;
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}