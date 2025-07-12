
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

        // Controle de estado da UI
        private bool _isUserSelectedSuggestion = false;
        private string _lastValidFullName = "";
        private bool _isNameInputFocused = false;

        public PersonPage()
        {
            InitializeComponent();

            // Inicializa coleções
            _suggestions = new ObservableCollection<PersonSuggestion>();
            suggestionsCollectionView.ItemsSource = _suggestions;

            // Esconder campos extras inicialmente
            HideExtraFields();

            // Configure eventos
            fullNameEntry.TextChanged += OnNameTextChanged;
            fullNameEntry.Focused += OnNameEntryFocused;
            fullNameEntry.Unfocused += OnNameEntryUnfocused;

            // Eventos para mês e dia
            monthPicker.SelectedIndexChanged += OnMonthChanged;
            dayPicker.SelectedIndexChanged += OnDayChanged;
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
                    _serviceProvider = MyKaraoke.View.ServiceProvider.FromPage(this);
                    _queueService = _serviceProvider?.GetService<IQueueService>();
                    _pessoaService = _serviceProvider?.GetService<IPessoaService>();
                    _textNormalizer = _serviceProvider?.GetService<ITextNormalizer>();

                    // ❌ REMOVER: headerComponent.BackCommand = new Command(OnBackPressed);
                    // ✅ HeaderComponent cuida automaticamente da navegação!

                    SetupDatePickers();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Erro ao inicializar serviços: {ex.Message}");
                }
            }
        }


        private void SetupDatePickers()
        {
            // Configura picker de mês
            var months = new List<string>
            {
                "Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho",
                "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro"
            };

            monthPicker.ItemsSource = months;
            monthPicker.SelectedIndex = -1;

            // Configura picker de dia (inicialmente vazio)
            dayPicker.ItemsSource = new List<string>();
            dayPicker.SelectedIndex = -1;
        }

        private void OnNameEntryFocused(object sender, FocusEventArgs e)
        {
            _isNameInputFocused = true;

            // Se o usuário clica no nome após ter preenchido, esconde campos extras
            if (monthPicker.SelectedIndex >= 0 || !string.IsNullOrWhiteSpace(emailEntry.Text))
            {
                HideExtraFields();
                ClearExtraFields();
            }

            // CORREÇÃO: Remove a lógica que mostrava o botão durante o foco
            // Agora o botão só aparece quando perde o foco E há texto válido
            // var nameText = fullNameEntry.Text?.Trim() ?? "";
            // if (nameText.Length >= 2)
            // {
            //     ShowNextButton();
            // }
        }

        private void ShowNextButton()
        {
            nextButton.IsVisible = true;
        }

        private void HideNextButton()
        {
            nextButton.IsVisible = false;
        }

        private void ShowDateFields()
        {
            monthLabel.IsVisible = true;
            monthPicker.IsVisible = true;
        }

        private void OnNameEntryUnfocused(object sender, FocusEventArgs e)
        {
            _isNameInputFocused = false;

            // Esconde sugestões quando perde o foco
            HideSuggestions();

            // CORREÇÃO: Agora mostra o botão próximo apenas quando perde o foco
            var nameText = fullNameEntry.Text?.Trim() ?? "";
            if (nameText.Length >= 2)
            {
                // Se não há sugestões abertas, mostra o botão próximo
                if (!suggestionsFrame.IsVisible)
                {
                    ShowNextButton();
                }
                // Se há texto válido, avança automaticamente apenas se não há sugestões
                AdvanceToDateFields();
            }
            else
            {
                HideNextButton();
            }
        }

        private void OnNameTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                // Se usuário edita o nome após selecionar sugestão, limpa tudo
                if (_isUserSelectedSuggestion && !string.IsNullOrEmpty(_lastValidFullName))
                {
                    var currentText = e.NewTextValue?.Trim() ?? "";
                    if (currentText != _lastValidFullName)
                    {
                        ClearSelectedSuggestion();
                        HideExtraFields();
                        ClearExtraFields();
                    }
                }

                // Cancela busca anterior
                _searchTimer?.Dispose();

                var searchText = e.NewTextValue?.Trim() ?? "";
                var currentLength = e.NewTextValue?.Length ?? 0;

                // Proteção: Limite de caracteres
                if (currentLength > 200)
                {
                    if (sender is Entry entry)
                    {
                        entry.Text = e.NewTextValue?.Substring(0, 200);
                        return;
                    }
                }

                // Atualiza contador de caracteres
                UpdateCharacterCounter(currentLength);

                // Se texto vazio, esconde tudo
                if (string.IsNullOrWhiteSpace(searchText))
                {
                    HideSuggestions();
                    HideExtraFields();
                    HideNextButton();
                    return;
                }

                // CORREÇÃO: Remove a lógica que mostrava o botão próximo durante a digitação
                // O botão próximo só deve aparecer quando o campo perde o foco
                // if (searchText.Length >= 2 && _isNameInputFocused)
                // {
                //     ShowNextButton();
                // }
                // else
                // {
                //     HideNextButton();
                // }

                // Sempre esconde o botão durante a digitação
                HideNextButton();

                // Se texto muito curto, apenas esconde sugestões
                if (searchText.Length < 2)
                {
                    HideSuggestions();
                    return;
                }

                // Configura timer com debounce para busca
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

                try
                {
                    HideSuggestions();
                }
                catch
                {
                    System.Diagnostics.Debug.WriteLine("Erro crítico no fallback");
                }
            }
        }

        private async Task SearchSuggestionsAsync(string searchText)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchText) ||
                    searchText.Length < 2 ||
                    _pessoaService == null)
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        HideSuggestions();
                    });
                    return;
                }

                var sanitizedSearchText = searchText.Trim();
                if (sanitizedSearchText.Length < 2)
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        HideSuggestions();
                    });
                    return;
                }

                // Busca usando o serviço
                var exactMatches = await _pessoaService.SearchPersonsStartsWithAsync(sanitizedSearchText, 2);
                var containsMatches = await _pessoaService.SearchPersonsAsync(sanitizedSearchText, 3);

                exactMatches = exactMatches ?? new List<Pessoa>();
                containsMatches = containsMatches ?? new List<Pessoa>();

                var allMatches = exactMatches
                    .Concat(containsMatches.Where(c => !exactMatches.Any(e => e.Id == c.Id)))
                    .Take(4)
                    .ToList();

                var suggestions = allMatches.Select(PersonSuggestion.FromPessoa).ToList();

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
                            HideNextButton();
                        }
                        else
                        {
                            HideSuggestions();
                        }
                    }
                    catch (Exception uiEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"Erro na atualização da UI: {uiEx.Message}");
                        HideSuggestions();
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro na busca de sugestões: {ex.Message}");
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    HideSuggestions();
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
                // Verifica se é a opção "Nova pessoa" (mesmo que não exista mais)
                if (suggestion.IsNewPersonOption)
                {
                    HideSuggestions();
                    fullNameEntry.Unfocus();
                    AdvanceToDateFields();
                    return;
                }

                // Marca que usuário selecionou sugestão
                _isUserSelectedSuggestion = true;
                _selectedSuggestion = suggestion;

                // Preenche nome
                fullNameEntry.Text = suggestion.NomeCompleto;
                _lastValidFullName = suggestion.NomeCompleto;

                // Preenche campos de data se disponíveis
                if (!string.IsNullOrWhiteSpace(suggestion.DiaMesAniversario))
                {
                    var parts = suggestion.DiaMesAniversario.Split('/');
                    if (parts.Length == 2 && int.TryParse(parts[1], out int month) && int.TryParse(parts[0], out int day))
                    {
                        monthPicker.SelectedIndex = month - 1;
                        UpdateDayPicker(month);

                        // Agenda seleção do dia para após atualização do picker
                        Device.StartTimer(TimeSpan.FromMilliseconds(100), () =>
                        {
                            if (day > 0 && day <= GetDaysInMonth(month))
                            {
                                dayPicker.SelectedIndex = day - 1;
                            }
                            return false;
                        });
                    }
                }

                // Preenche email se disponível
                if (!string.IsNullOrWhiteSpace(suggestion.Email))
                {
                    emailEntry.Text = suggestion.Email;
                }

                // Esconde sugestões
                HideSuggestions();

                // Remove foco do campo nome
                fullNameEntry.Unfocus();

                // Mostra campos progressivamente
                ShowDateFields();
                if (monthPicker.SelectedIndex >= 0 && dayPicker.SelectedIndex >= 0)
                {
                    ShowEmailAndButton();
                }

                // Limpa seleção do CollectionView
                suggestionsCollectionView.SelectedItem = null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao selecionar sugestão: {ex.Message}");
            }
        }
        private void OnNextButtonClicked(object sender, EventArgs e)
        {
            fullNameEntry.Unfocus();
            AdvanceToDateFields();
        }

        private void AdvanceToDateFields()
        {
            var nameText = fullNameEntry.Text?.Trim() ?? "";
            if (nameText.Length >= 2)
            {
                ShowDateFields();
                HideNextButton();
            }
        }

        private void OnMonthChanged(object sender, EventArgs e)
        {
            if (monthPicker.SelectedIndex >= 0)
            {
                int selectedMonth = monthPicker.SelectedIndex + 1;
                UpdateDayPicker(selectedMonth);

                // Limpa seleção de dia quando mês muda
                dayPicker.SelectedIndex = -1;

                // Esconde email e botão até dia ser selecionado
                HideEmailAndButton();
            }
        }

        private void OnDayChanged(object sender, EventArgs e)
        {
            if (dayPicker.SelectedIndex >= 0 && monthPicker.SelectedIndex >= 0)
            {
                // Verifica se é pessoa existente
                CheckIfExistingPerson();

                // Mostra email e botão
                ShowEmailAndButton();
            }
            else
            {
                HideEmailAndButton();
            }
        }

        private async void CheckIfExistingPerson()
        {
            try
            {
                if (_pessoaService == null || string.IsNullOrWhiteSpace(fullNameEntry.Text) ||
                    monthPicker.SelectedIndex < 0 || dayPicker.SelectedIndex < 0)
                {
                    return;
                }

                var fullName = fullNameEntry.Text.Trim();
                var month = monthPicker.SelectedIndex + 1;
                var day = dayPicker.SelectedIndex + 1;
                var birthdayString = $"{day:00}/{month:00}";

                // Busca pessoa por nome
                var existingPerson = await _pessoaService.GetPersonByNameAsync(fullName);

                if (existingPerson != null && existingPerson.DiaMesAniversario == birthdayString)
                {
                    // Preenche email se disponível
                    if (!string.IsNullOrWhiteSpace(existingPerson.Email))
                    {
                        emailEntry.Text = existingPerson.Email;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao verificar pessoa existente: {ex.Message}");
            }
        }
        private void UpdateDayPicker(int month)
        {
            int daysInMonth = GetDaysInMonth(month);
            var days = Enumerable.Range(1, daysInMonth).Select(d => d.ToString()).ToList();
            dayPicker.ItemsSource = days;
        }

        private int GetDaysInMonth(int month)
        {
            switch (month)
            {
                case 2: return 29; // Fevereiro (considerando ano bissexto)
                case 4:
                case 6:
                case 9:
                case 11: return 30;
                default: return 31;
            }
        }




        private void HideExtraFields()
        {
            monthLabel.IsVisible = false;
            monthPicker.IsVisible = false;
            dayLabel.IsVisible = false;
            dayPicker.IsVisible = false;
            emailLabel.IsVisible = false;
            emailEntry.IsVisible = false;
            addToQueueButton.IsVisible = false;
            HideNextButton();
        }
        private void ShowEmailAndButton()
        {
            dayLabel.IsVisible = true;
            dayPicker.IsVisible = true;
            emailLabel.IsVisible = true;
            emailEntry.IsVisible = true;
            addToQueueButton.IsVisible = true;
        }

        private void HideEmailAndButton()
        {
            emailLabel.IsVisible = false;
            emailEntry.IsVisible = false;
            addToQueueButton.IsVisible = false;
        }

        private void ClearExtraFields()
        {
            monthPicker.SelectedIndex = -1;
            dayPicker.SelectedIndex = -1;
            emailEntry.Text = string.Empty;
        }

        private void ClearSelectedSuggestion()
        {
            _isUserSelectedSuggestion = false;
            _selectedSuggestion = null;
            _lastValidFullName = "";
        }

        private void ShowSuggestions()
        {
            suggestionsFrame.IsVisible = true;
        }

        private void HideSuggestions()
        {
            suggestionsFrame.IsVisible = false;
        }

        private void ClearForm()
        {
            fullNameEntry.Text = string.Empty;
            monthPicker.SelectedIndex = -1;
            dayPicker.SelectedIndex = -1;
            emailEntry.Text = string.Empty;
            HideSuggestions();
            HideExtraFields();
            ClearSelectedSuggestion();
            characterCounterLabel.IsVisible = false;
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
            string birthday = "";
            string email = emailEntry.Text?.Trim();

            // Constrói data a partir dos pickers
            if (monthPicker.SelectedIndex >= 0 && dayPicker.SelectedIndex >= 0)
            {
                int month = monthPicker.SelectedIndex + 1;
                int day = dayPicker.SelectedIndex + 1;
                birthday = $"{day:00}/{month:00}";
            }

            // Validação básica
            if (string.IsNullOrWhiteSpace(fullName))
            {
                validationMessageLabel.Text = "Nome é obrigatório";
                validationMessageLabel.IsVisible = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(birthday))
            {
                validationMessageLabel.Text = "Data de aniversário é obrigatória";
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

        // Métodos de carregamento/salvamento mantidos
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
    }
}