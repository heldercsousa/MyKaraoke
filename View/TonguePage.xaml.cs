using MyKaraoke.Services;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace MyKaraoke.View
{
    public partial class TonguePage : ContentPage
    {
        private ILanguageService _languageService;
        private IQueueService _queueService;
        private ServiceProvider _serviceProvider;
        private string selectedLanguage = "en"; // Idioma padrão

        public TonguePage()
        {
            InitializeComponent();
            LoadCurrentLanguage();
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
                    _queueService = _serviceProvider.GetService<IQueueService>();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Erro ao inicializar serviços: {ex.Message}");
                }
            }
        }

        private void LoadCurrentLanguage()
        {
            // Carrega o idioma atual das preferências ou banco de dados
            selectedLanguage = Preferences.Get("AppLanguage", "en");
            UpdateUIForSelectedLanguage(selectedLanguage);
        }

        private async void OnLanguageSelected(object sender, TappedEventArgs e)
        {
            // Obtém o código do idioma do parâmetro do comando
            string languageCode = e.Parameter?.ToString();

            if (string.IsNullOrEmpty(languageCode))
                return;

            selectedLanguage = languageCode;

            // Atualiza a UI para mostrar o idioma selecionado
            UpdateUIForSelectedLanguage(selectedLanguage);

            // Salva o idioma no banco de dados ou preferências
            await SaveSelectedLanguageAsync(selectedLanguage);

            // Aplica o idioma ao aplicativo
            ApplyLanguageToApp(selectedLanguage);
            
            // Removida a navegação automática para StackPage
            // Agora a navegação ocorre apenas pelos botões voltar
        }

        private void UpdateUIForSelectedLanguage(string languageCode)
        {
            // Reseta todos os frames para transparente com borda branca
            foreach (var child in languageContainer.Children)
            {
                if (child is Frame frame)
                {
                    frame.BackgroundColor = Colors.Transparent;
                    frame.BorderColor = Colors.White;
                }
            }

            // Encontra o frame do idioma selecionado e muda sua cor
            var selectedFrame = languageContainer.Children.FirstOrDefault(c =>
            {
                if (c is Frame frame &&
                    frame.GestureRecognizers.FirstOrDefault() is TapGestureRecognizer tap &&
                    tap.CommandParameter?.ToString() == languageCode)
                    return true;
                return false;
            }) as Frame;

            if (selectedFrame != null)
            {
                selectedFrame.BackgroundColor = Color.FromArgb("#d5528a");
                selectedFrame.BorderColor = Colors.Transparent;
            }
        }

        private async Task SaveSelectedLanguageAsync(string languageCode)
        {
            try
            {
                // Salva o idioma nas preferências do aplicativo
                Preferences.Set("AppLanguage", languageCode);

                // Usa o serviço de idioma para persistir a seleção
                if (_languageService != null)
                {
                    await _languageService.SetUserLanguageAsync(languageCode);
                }
                else
                {
                    Console.WriteLine("Serviço de idioma não disponível");
                }
            }
            catch (Exception ex)
            {
                // Log do erro ou notificação ao usuário
                Console.WriteLine($"Erro ao salvar idioma: {ex.Message}");
            }
        }

        private void ApplyLanguageToApp(string languageCode)
        {
            // Aplica o idioma ao aplicativo
            // Isso depende da sua implementação de localização

            // Exemplo:
            // LocalizationResourceManager.Instance.SetLanguage(new CultureInfo(languageCode));
        }

        private void OnBackButtonClicked(object sender, EventArgs e)
        {
            // Navegar para StackPage ao clicar no botão voltar
            if (Application.Current != null)
            {
                Application.Current.MainPage = new NavigationPage(new StackPage());
            }
        }

        // Captura o botão voltar do Android
        protected override bool OnBackButtonPressed()
        {
            // Mesmo comportamento do botão voltar na UI
            if (Application.Current != null)
            {
                Application.Current.MainPage = new NavigationPage(new StackPage());
            }
            return true; // Impede o comportamento padrão de sair do app
        }

        // Este método pode ser removido
        private void OnConfirmLanguage(object sender, EventArgs e)
        {
            // Método não é mais usado após a remoção do botão confirmar
        }
    }
}