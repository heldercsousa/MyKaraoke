using MyKaraoke.Contracts;
using MyKaraoke.Services;
using System.Text.Json;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MyKaraoke.View
{
    public partial class PersonPage : ContentPage
    {
        private IQueueService _queueService;
        private ServiceProvider _serviceProvider;
        private const string ActiveQueueKey = "ActiveFilaDeCQueue";

        public PersonPage()
        {
            InitializeComponent();
            
            // Configure the entry for international text input
            fullNameEntry.TextChanged += OnTextChanged;
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var entry = sender as Entry;
            if (entry == null || string.IsNullOrEmpty(e.NewTextValue))
                return;

            // Detect Arabic text and adjust flow direction
            if (ContainsArabicText(e.NewTextValue))
            {
                entry.HorizontalTextAlignment = TextAlignment.End;
            }
            else
            {
                entry.HorizontalTextAlignment = TextAlignment.Start;
            }
        }

        private bool ContainsArabicText(string text)
        {
            // Arabic Unicode range: U+0600 to U+06FF
            return Regex.IsMatch(text, @"[\u0600-\u06FF]");
        }

        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();

            if (Handler != null)
            {
                // Inicializa o ServiceProvider quando o Handler estiver disponível
                _serviceProvider = ServiceProvider.FromPage(this);
                _queueService = _serviceProvider.GetService<IQueueService>();
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        // Método para o botão voltar
        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await NavigateToStackPage();
        }

        // Captura o botão voltar do Android
        protected override bool OnBackButtonPressed()
        {
            MainThread.BeginInvokeOnMainThread(async () => {
                await NavigateToStackPage();
            });
            
            return true; // Impede o comportamento padrão
        }

        // Método para navegar para StackPage
        private async Task NavigateToStackPage()
        {
            try
            {
                // Assegura que o ServiceProvider está disponível
                if (_serviceProvider == null)
                {
                    _serviceProvider = ServiceProvider.FromPage(this);
                }
                
                // Obtém a StackPage através do ServiceProvider e navega
                var stackPage = _serviceProvider.GetService<StackPage>();
                if (stackPage != null)
                {
                    await Navigation.PushAsync(stackPage);
                }
                else
                {
                    // Fallback: cria uma nova instância da StackPage se o ServiceProvider falhar
                    await Navigation.PushAsync(new StackPage());
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao navegar para StackPage: {ex.Message}");
                // Fallback: cria uma nova instância da StackPage
                await Navigation.PushAsync(new StackPage());
            }
        }

        private async void OnAddToQueueClicked(object sender, EventArgs e)
        {
            string fullName = fullNameEntry.Text;
            try
            {
                // Verifica se há evento ativo antes de prosseguir
                var activeEvent = await _queueService.GetActiveEventAsync();
                if (activeEvent == null || !activeEvent.FilaAtiva)
                {
                    validationMessageLabel.Text = "não há fila ativa";
                    validationMessageLabel.IsVisible = true;
                    return;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao verificar fila ativa: {ex.Message}");
                validationMessageLabel.Text = "não há fila ativa";
                validationMessageLabel.IsVisible = true;
                return;
            }

            var result = await _queueService.AddPersonAsync(fullName);

            if (result.success)
            {
                validationMessageLabel.Text = "";
                validationMessageLabel.IsVisible = false;

                // Verifica se a pessoa de domínio retornada é nula (embora success=true, é boa prática)
                if (result.addedDomainPerson == null)
                {
                    await DisplayAlert("Erro Interno", "Não foi possível obter os dados da pessoa.", "OK");
                    return;
                }

                // Carrega a fila ativa de DTOs das Preferences
                List<PessoaListItemDto> currentQueueDtos = LoadActiveQueueState();

                // Verifica se o DTO já está na fila ativa (em memória da UI)
                if (currentQueueDtos.Any(p => p.Id == result.addedDomainPerson.Id))
                {
                    await DisplayAlert("Erro", GetString("pessoa_ja_fila"), "OK");
                    return;
                }

                // Mapeia a entidade de domínio (Pessoa) para o DTO de item de lista (PessoaListItemDto) para a UI
                // O DTO é criado a partir da entidade de domínio que o serviço retornou.
                var personDtoToAdd = new PessoaListItemDto(result.addedDomainPerson);
                // As propriedades Participacoes e Ausencias do DTO já são inicializadas como 0 no construtor padrão,
                // mas garantimos que são 0 para o novo evento ativo aqui.
                personDtoToAdd.Participacoes = 0;
                personDtoToAdd.Ausencias = 0;

                currentQueueDtos.Add(personDtoToAdd);
                SaveActiveQueueState(currentQueueDtos); // Salva a fila atualizada nas Preferences

                string successMessage = string.Format(GetString("pessoa_adicionada_sucesso"), personDtoToAdd.NomeCompleto);
                await DisplayAlert("Sucesso", successMessage, "OK");
                fullNameEntry.Text = string.Empty;
            }
            else
            {
                validationMessageLabel.Text = result.message;
                validationMessageLabel.IsVisible = true;
            }
        }

        // --- Métodos de Persistência da Fila Ativa na UI (usando Preferences) ---
        private List<PessoaListItemDto> LoadActiveQueueState()
        {
            string filaJson = Preferences.Get(ActiveQueueKey, string.Empty);
            if (!string.IsNullOrEmpty(filaJson))
            {
                return JsonSerializer.Deserialize<List<PessoaListItemDto>>(filaJson) ?? new List<PessoaListItemDto>();
            }
            return new List<PessoaListItemDto>();
        }

        private void SaveActiveQueueState(List<PessoaListItemDto> fila)
        {
            string filaJson = JsonSerializer.Serialize(fila);
            Preferences.Set(ActiveQueueKey, filaJson);
        }

        // Helper para obter strings do arquivo de recursos (strings.xml / strings.resx)
        private string GetString(string key, params object[] args)
        {
            string value = "";
            switch (key)
            {
                case "pessoa_adicionada_sucesso": value = "%s adicionado(a) à fila!"; break;
                case "pessoa_ja_fila": value = "Esta pessoa já está na fila."; break;
                default: value = key; break;
            }

            if (args != null && args.Length > 0)
            {
                try
                {
                    return string.Format(value, args);
                }
                catch (FormatException)
                {
                    return value;
                }
            }
            return value;
        }
    }
}