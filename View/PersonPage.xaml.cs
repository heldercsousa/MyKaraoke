using MyKaraoke.Contracts;
using MyKaraoke.Domain;
using MyKaraoke.Infra.Utils;
using MyKaraoke.Services;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace MyKaraoke.View
{
    public partial class PersonPage : ContentPage
    {
        private IQueueService _queueService;
        private IPessoaService _pessoaService;
        private ITextNormalizer _textNormalizer;
        private MyKaraoke.View.ServiceProvider _serviceProvider;
        private const string ActiveQueueKey = "ActiveFilaDeCQueue";

        // Coleções para sugestões
        private ObservableCollection<PersonSuggestion> _suggestions;
        private PersonSuggestion _selectedSuggestion;

        // Timer para debounce da busca
        private Timer _searchTimer;
        private const int SearchDelayMs = 300;

        public PersonPage()
        {
            InitializeComponent();

            // Inicializa coleções
            _suggestions = new ObservableCollection<PersonSuggestion>();
            suggestionsCollectionView.ItemsSource = _suggestions;

            // Configure the entry for international text input
            fullNameEntry.TextChanged += OnTextChanged;
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var entry = sender as Entry;
            if (entry == null || string.IsNullOrEmpty(e.NewTextValue))
                return;

            // Usa o serviço de normalização para detectar texto árabe (quando disponível)
            if (_textNormalizer?.ContainsArabicText(e.NewTextValue) == true)
            {
                entry.HorizontalTextAlignment = TextAlignment.End;
            }
            else
            {
                entry.HorizontalTextAlignment = TextAlignment.Start;
            }
        }

        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();

            if (Handler != null)
            {
                try
                {
                    // Inicializa o ServiceProvider quando o Handler estiver disponível
                    _serviceProvider = MyKaraoke.View.ServiceProvider.FromPage(this);
                    _queueService = _serviceProvider?.GetService<IQueueService>();
                    _pessoaService = _serviceProvider?.GetService<IPessoaService>();
                    _textNormalizer = _serviceProvider?.GetService<ITextNormalizer>();

                    // Configura o comando de voltar do HeaderComponent
                    headerComponent.BackCommand = new Command(OnBackPressed);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Erro ao inicializar serviços: {ex.Message}");
                }
            }
        }

        private void OnNameTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                // Cancela busca anterior
                _searchTimer?.Dispose();

                // 🔥 PROTEÇÃO: Validação de entrada
                var searchText = e.NewTextValue?.Trim() ?? "";
                var currentLength = e.NewTextValue?.Length ?? 0;

                // 🔥 PROTEÇÃO: Limite de caracteres
                if (currentLength > 200)
                {
                    // Trunca se exceder o limite
                    if (sender is Entry entry)
                    {
                        entry.Text = e.NewTextValue?.Substring(0, 200);
                        return;
                    }
                }

                // Atualiza contador de caracteres
                UpdateCharacterCounter(currentLength);

                // Se texto vazio, esconde sugestões
                if (string.IsNullOrWhiteSpace(searchText))
                {
                    HideSuggestions();
                    HideIndicators();
                    return;
                }

                // 🔥 PROTEÇÃO: Verifica se texto tem tamanho mínimo
                if (searchText.Length < 2)
                {
                    HideSuggestions();
                    HideIndicators();
                    return;
                }

                // Configura timer com debounce
                _searchTimer = new Timer(async _ =>
                {
                    try
                    {
                        await MainThread.InvokeOnMainThreadAsync(async () =>
                        {
                            await SearchSuggestionsAsync(searchText);
                        });
                    }
                    catch (Exception timerEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"Erro no timer de busca: {timerEx.Message}");
                    }
                }, null, SearchDelayMs, Timeout.Infinite);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro em OnNameTextChanged: {ex.Message}");

                // Fallback seguro
                try
                {
                    HideSuggestions();
                    HideIndicators();
                }
                catch
                {
                    // Se até o fallback falhar, apenas loga
                    System.Diagnostics.Debug.WriteLine("Erro crítico no fallback");
                }
            }
        }

        private void UpdateCharacterCounter(int currentLength)
        {
            try
            {
                if (_pessoaService == null)
                {
                    characterCounterLabel.IsVisible = false;
                    return;
                }

                // Usa o serviço para determinar se deve mostrar contador
                if (_pessoaService.ShouldShowCharacterCounter(currentLength))
                {
                    var (text, isWarning, isError) = _pessoaService.GetCharacterCounterInfo(currentLength);

                    characterCounterLabel.Text = text;
                    characterCounterLabel.IsVisible = true;

                    // Cores baseadas na proximidade do limite
                    if (isError)
                    {
                        characterCounterLabel.TextColor = Color.FromArgb("#ff6b6b");
                    }
                    else if (isWarning)
                    {
                        characterCounterLabel.TextColor = Color.FromArgb("#FF9800");
                    }
                    else
                    {
                        characterCounterLabel.TextColor = Color.FromArgb("#b0a8c7");
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
                characterCounterLabel.IsVisible = false;
            }
        }

        private async Task SearchSuggestionsAsync(string searchText)
        {
            try
            {
                // 🔥 VALIDAÇÃO ROBUSTA
                if (string.IsNullOrWhiteSpace(searchText) ||
                    searchText.Length < 2 ||
                    _pessoaService == null)
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        HideSuggestions();
                        HideIndicators();
                    });
                    return;
                }

                // 🔥 PROTEÇÃO ADICIONAL: Verifica se searchText é válido
                var sanitizedSearchText = searchText.Trim();
                if (sanitizedSearchText.Length < 2)
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        HideSuggestions();
                        HideIndicators();
                    });
                    return;
                }

                // Busca usando o serviço (com try-catch interno)
                var exactMatches = await _pessoaService.SearchPersonsStartsWithAsync(sanitizedSearchText, 2);
                var containsMatches = await _pessoaService.SearchPersonsAsync(sanitizedSearchText, 3);

                // 🔥 PROTEÇÃO: Verifica se listas não são nulas
                exactMatches = exactMatches ?? new List<Pessoa>();
                containsMatches = containsMatches ?? new List<Pessoa>();

                // Combina resultados evitando duplicatas
                var allMatches = exactMatches
                    .Concat(containsMatches.Where(c => !exactMatches.Any(e => e.Id == c.Id)))
                    .Take(3)
                    .ToList();

                // Mapeia para sugestões de UI
                var suggestions = allMatches.Select(PersonSuggestion.FromPessoa).ToList();

                // Atualiza UI no thread principal
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    try
                    {
                        _suggestions.Clear();
                        foreach (var suggestion in suggestions)
                        {
                            _suggestions.Add(suggestion);
                        }

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
                    }
                    catch (Exception uiEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"Erro na atualização da UI: {uiEx.Message}");
                        HideSuggestions();
                        HideIndicators();
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
                // Preenche os campos
                fullNameEntry.Text = suggestion.NomeCompleto;

                if (!string.IsNullOrWhiteSpace(suggestion.DiaMesAniversario))
                {
                    birthdayEntry.Text = suggestion.DiaMesAniversario;
                }

                if (!string.IsNullOrWhiteSpace(suggestion.Email))
                {
                    emailEntry.Text = suggestion.Email;
                }

                _selectedSuggestion = suggestion;

                // Esconde sugestões e mostra indicador
                HideSuggestions();
                ShowExistingPersonIndicator();

                // Limpa seleção do CollectionView
                suggestionsCollectionView.SelectedItem = null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao selecionar sugestão: {ex.Message}");
            }
        }

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

        private async void OnBackPressed()
        {
            await NavigateToStackPage();
        }

        protected override bool OnBackButtonPressed()
        {
            MainThread.BeginInvokeOnMainThread(async () => {
                await NavigateToStackPage();
            });
            return true;
        }

        private async Task NavigateToStackPage()
        {
            try
            {
                if (_serviceProvider == null)
                {
                    _serviceProvider = MyKaraoke.View.ServiceProvider.FromPage(this);
                }

                var stackPage = _serviceProvider?.GetService<StackPage>();
                if (stackPage != null)
                {
                    await Navigation.PushAsync(stackPage);
                }
                else
                {
                    await Navigation.PushAsync(new StackPage());
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao navegar para StackPage: {ex.Message}");
                await Navigation.PushAsync(new StackPage());
            }
        }

        private async void OnAddToQueueClicked(object sender, EventArgs e)
        {
            if (_pessoaService == null || _queueService == null)
            {
                validationMessageLabel.Text = "Serviços não disponíveis";
                validationMessageLabel.IsVisible = true;
                return;
            }

            string fullName = fullNameEntry.Text?.Trim();
            string birthday = birthdayEntry.Text?.Trim();
            string email = emailEntry.Text?.Trim();

            // Validação básica
            if (string.IsNullOrWhiteSpace(fullName))
            {
                validationMessageLabel.Text = "Nome é obrigatório";
                validationMessageLabel.IsVisible = true;
                return;
            }

            try
            {
                // Verifica se há evento ativo
                var activeEvent = await _queueService.GetActiveEventAsync();
                if (activeEvent == null || !activeEvent.FilaAtiva)
                {
                    validationMessageLabel.Text = "Não há fila ativa";
                    validationMessageLabel.IsVisible = true;
                    return;
                }

                // Adiciona pessoa à fila
                var result = await _queueService.AddPersonToQueueAsync(fullName, birthday, email);
                if (!result.success)
                {
                    validationMessageLabel.Text = result.message;
                    validationMessageLabel.IsVisible = true;
                    return;
                }

                // Sucesso
                validationMessageLabel.IsVisible = false;
                await DisplayAlert("Sucesso", $"{result.addedDomainPerson?.NomeCompleto} adicionado à fila!", "OK");

                // Limpa campos
                ClearForm();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao adicionar à fila: {ex.Message}");
                validationMessageLabel.Text = "Erro interno";
                validationMessageLabel.IsVisible = true;
            }
        }

        private void ClearForm()
        {
            fullNameEntry.Text = string.Empty;
            birthdayEntry.Text = string.Empty;
            emailEntry.Text = string.Empty;
            HideSuggestions();
            HideIndicators();
            characterCounterLabel.IsVisible = false;
            _selectedSuggestion = null;
        }

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

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _searchTimer?.Dispose();
        }
    }
}