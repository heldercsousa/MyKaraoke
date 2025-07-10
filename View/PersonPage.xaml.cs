using MyKaraoke.Contracts;
using MyKaraoke.Domain;
using MyKaraoke.Services;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MyKaraoke.View
{
    public partial class PersonPage : ContentPage
    {
        private IQueueService _queueService;
        private ServiceProvider _serviceProvider;
        private const string ActiveQueueKey = "ActiveFilaDeCQueue";

        // Coleções para sugestões
        private ObservableCollection<PersonSuggestion> _suggestions;
        private PersonSuggestion _selectedSuggestion;

        // Timer para debounce da busca
        private Timer _searchTimer;
        private const int SearchDelayMs = 300; // 300ms delay para UX otimizada

        public PersonPage()
        {
            InitializeComponent();

            // Inicializa coleções
            _suggestions = new ObservableCollection<PersonSuggestion>();

            // Configura CollectionView
            suggestionsCollectionView.ItemsSource = _suggestions;

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

                // Configura o comando de voltar do HeaderComponent
                headerComponent.BackCommand = new Command(OnBackPressed);
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        // FUNCIONALIDADE HÍBRIDA: Busca + Contador de caracteres
        private void OnNameTextChanged(object sender, TextChangedEventArgs e)
        {
            // Cancela busca anterior
            _searchTimer?.Dispose();

            var searchText = e.NewTextValue?.Trim() ?? "";
            var currentLength = e.NewTextValue?.Length ?? 0;

            // NOVO: Atualiza contador de caracteres
            UpdateCharacterCounter(currentLength);

            // Se texto vazio, esconde sugestões
            if (string.IsNullOrWhiteSpace(searchText))
            {
                HideSuggestions();
                HideIndicators();
                return;
            }

            // Configura timer com debounce para UX otimizada
            _searchTimer = new Timer(async _ =>
            {
                await MainThread.InvokeOnMainThreadAsync(async () => await SearchSuggestionsAsync(searchText));
            }, null, SearchDelayMs, Timeout.Infinite);
        }

        // NOVA FUNCIONALIDADE: Contador inteligente de caracteres
        private void UpdateCharacterCounter(int currentLength)
        {
            try
            {
                // Mostra contador apenas quando próximo do limite (>180 chars)
                if (Pessoa.ShouldShowCharacterCounter(currentLength))
                {
                    var (text, isWarning, isError) = Pessoa.GetCharacterCounterInfo(currentLength);

                    characterCounterLabel.Text = text;
                    characterCounterLabel.IsVisible = true;

                    // Cores baseadas na proximidade do limite
                    if (isError)
                    {
                        characterCounterLabel.TextColor = Color.FromArgb("#ff6b6b"); // Vermelho (limite atingido)
                    }
                    else if (isWarning)
                    {
                        characterCounterLabel.TextColor = Color.FromArgb("#FF9800"); // Laranja (warning)
                    }
                    else
                    {
                        characterCounterLabel.TextColor = Color.FromArgb("#b0a8c7"); // Cinza claro (normal)
                    }
                }
                else
                {
                    characterCounterLabel.IsVisible = false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no contador de caracteres: {ex.Message}");
            }
        }

        // FUNCIONALIDADE SUPER OTIMIZADA: Busca usando índice normalizado
        private async Task SearchSuggestionsAsync(string searchText)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchText) || searchText.Length < 2)
                {
                    HideSuggestions();
                    HideIndicators();
                    return;
                }

                // Busca otimizada usando repository com índice normalizado
                var pessoaRepository = _serviceProvider.GetService<MyKaraoke.Domain.Repositories.IPessoaRepository>();
                if (pessoaRepository == null)
                {
                    System.Diagnostics.Debug.WriteLine("Repository não disponível");
                    return;
                }

                // Estratégia de busca em cascata para melhor UX:
                // 1. Primeiro busca que INICIA com o termo (mais preciso)
                // 2. Depois busca que CONTÉM o termo (mais abrangente)
                var exactMatches = await pessoaRepository.SearchByNameStartsWithAsync(searchText, 2);
                var containsMatches = await pessoaRepository.SearchByNameAsync(searchText, 3);

                // Combina resultados evitando duplicatas
                var allMatches = exactMatches
                    .Concat(containsMatches.Where(c => !exactMatches.Any(e => e.Id == c.Id)))
                    .Take(3) // Máximo 3 sugestões (UX mobile guidelines)
                    .ToList();

                System.Diagnostics.Debug.WriteLine($"Busca '{searchText}': {exactMatches.Count} exatos + {containsMatches.Count} contém = {allMatches.Count} total");

                // Mapeia para sugestões de UI
                var suggestions = allMatches.Select(p => new PersonSuggestion
                {
                    Id = p.Id,
                    NomeCompleto = p.NomeCompleto,
                    Participacoes = p.Participacoes,
                    Ausencias = p.Ausencias,
                    ParticipationSummary = $"{p.Participacoes}P {p.Ausencias}A"
                }).ToList();

                // Atualiza UI no thread principal
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    _suggestions.Clear();
                    foreach (var suggestion in suggestions)
                    {
                        _suggestions.Add(suggestion);
                    }

                    // Mostra/esconde sugestões e indicadores
                    if (_suggestions.Count > 0)
                    {
                        ShowSuggestions();
                        ShowExistingPersonIndicator();
                    }
                    else
                    {
                        HideSuggestions();
                        ShowNewPersonIndicator();
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro na busca de sugestões: {ex.Message}");
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    HideSuggestions();
                    ShowNewPersonIndicator();
                });
            }
        }

        // NOVA FUNCIONALIDADE: Seleção de sugestão
        private void OnSuggestionSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is PersonSuggestion selectedSuggestion)
            {
                SelectSuggestion(selectedSuggestion);
            }
        }

        private void OnSuggestionTapped(object sender, EventArgs e)
        {
            if ((sender as Grid)?.BindingContext is PersonSuggestion suggestion)
            {
                SelectSuggestion(suggestion);
            }
        }

        private void SelectSuggestion(PersonSuggestion suggestion)
        {
            try
            {
                // Preenche o campo com o nome selecionado
                fullNameEntry.Text = suggestion.NomeCompleto;
                _selectedSuggestion = suggestion;

                // Esconde sugestões e mostra indicador
                HideSuggestions();
                ShowExistingPersonIndicator();

                // Limpa seleção do CollectionView
                suggestionsCollectionView.SelectedItem = null;

                System.Diagnostics.Debug.WriteLine($"Sugestão selecionada: {suggestion.NomeCompleto}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao selecionar sugestão: {ex.Message}");
            }
        }

        // NOVA FUNCIONALIDADE: Controle de visibilidade das sugestões
        private void ShowSuggestions()
        {
            suggestionsFrame.IsVisible = true;
        }

        private void HideSuggestions()
        {
            suggestionsFrame.IsVisible = false;
            _selectedSuggestion = null;
        }

        private void ShowNewPersonIndicator()
        {
            newPersonIndicator.IsVisible = true;
            existingPersonIndicator.IsVisible = false;
        }

        private void ShowExistingPersonIndicator()
        {
            existingPersonIndicator.IsVisible = true;
            newPersonIndicator.IsVisible = false;
        }

        private void HideIndicators()
        {
            newPersonIndicator.IsVisible = false;
            existingPersonIndicator.IsVisible = false;
        }

        // Método chamado pelo HeaderComponent e botão físico do Android
        private async void OnBackPressed()
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

        // FUNCIONALIDADE APRIMORADA: Adicionar à fila com validação híbrida
        private async void OnAddToQueueClicked(object sender, EventArgs e)
        {
            string fullName = fullNameEntry.Text?.Trim();

            // NOVA VALIDAÇÃO HÍBRIDA: Primeira validação no input (200 chars)
            var (isInputValid, inputMessage) = Pessoa.ValidarNomeInput(fullName);
            if (!isInputValid)
            {
                validationMessageLabel.Text = inputMessage;
                validationMessageLabel.IsVisible = true;
                return;
            }

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

                Pessoa personToAdd;

                // Se uma sugestão foi selecionada, usa a pessoa existente
                if (_selectedSuggestion != null &&
                    Pessoa.NormalizeName(_selectedSuggestion.NomeCompleto) == Pessoa.NormalizeName(fullName))
                {
                    // Busca a pessoa existente usando repository
                    var pessoaRepository = _serviceProvider.GetService<MyKaraoke.Domain.Repositories.IPessoaRepository>();
                    personToAdd = await pessoaRepository.GetByIdAsync(_selectedSuggestion.Id);

                    if (personToAdd == null)
                    {
                        validationMessageLabel.Text = "Erro: pessoa não encontrada";
                        validationMessageLabel.IsVisible = true;
                        return;
                    }
                }
                else
                {
                    // NOVA VERIFICAÇÃO: Busca por nome normalizado para evitar duplicatas acentuadas
                    var pessoaRepository = _serviceProvider.GetService<MyKaraoke.Domain.Repositories.IPessoaRepository>();
                    var existingPerson = await pessoaRepository.GetByNormalizedNameAsync(fullName);

                    if (existingPerson != null)
                    {
                        // Pessoa já existe com nome similar (sem acentos)
                        validationMessageLabel.Text = $"Pessoa similar já existe: {existingPerson.NomeCompleto}";
                        validationMessageLabel.IsVisible = true;
                        return;
                    }

                    // SEGUNDA VALIDAÇÃO: Para banco (250 chars) - safety net
                    var (isBankValid, bankMessage) = Pessoa.ValidarNomeBanco(fullName);
                    if (!isBankValid)
                    {
                        validationMessageLabel.Text = $"Erro do sistema: {bankMessage}";
                        validationMessageLabel.IsVisible = true;
                        return;
                    }

                    // Adiciona nova pessoa
                    var result = await _queueService.AddPersonAsync(fullName);
                    if (!result.success)
                    {
                        validationMessageLabel.Text = result.message;
                        validationMessageLabel.IsVisible = true;
                        return;
                    }
                    personToAdd = result.addedDomainPerson;
                }

                // Verifica duplicata na fila ativa
                List<PessoaListItemDto> currentQueueDtos = LoadActiveQueueState();
                if (currentQueueDtos.Any(p => p.Id == personToAdd.Id))
                {
                    validationMessageLabel.Text = "Esta pessoa já está na fila";
                    validationMessageLabel.IsVisible = true;
                    return;
                }

                // Adiciona à fila ativa
                var personDtoToAdd = new PessoaListItemDto(personToAdd);
                personDtoToAdd.Participacoes = 0;
                personDtoToAdd.Ausencias = 0;

                currentQueueDtos.Add(personDtoToAdd);
                SaveActiveQueueState(currentQueueDtos);

                // Sucesso
                validationMessageLabel.IsVisible = false;
                string successMessage = $"{personToAdd.NomeCompleto} adicionado(a) à fila!";
                await DisplayAlert("Sucesso", successMessage, "OK");

                // Limpa campos e UI
                fullNameEntry.Text = string.Empty;
                HideSuggestions();
                HideIndicators();
                characterCounterLabel.IsVisible = false; // NOVO: Esconde contador
                _selectedSuggestion = null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao adicionar à fila: {ex.Message}");
                validationMessageLabel.Text = "Erro interno";
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

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            // Cleanup timer
            _searchTimer?.Dispose();
        }
    }

    // CLASSE: Model para sugestões
    public class PersonSuggestion
    {
        public int Id { get; set; }
        public string NomeCompleto { get; set; }
        public int Participacoes { get; set; }
        public int Ausencias { get; set; }
        public string ParticipationSummary { get; set; }
    }
}