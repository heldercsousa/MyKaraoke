using MyKaraoke.Contracts;
using MyKaraoke.Domain;
using MyKaraoke.Services;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace MyKaraoke.View
{
    public partial class StackPage : ContentPage
    {
        private IQueueService _queueService;
        private ServiceProvider _serviceProvider;
        private ObservableCollection<PessoaListItemDto> _fila;
        private const string ActiveQueueKey = "ActiveFilaDeCQueue"; // Chave para a fila ativa nas Preferences

        public StackPage()
        {
            InitializeComponent();

            _fila = new ObservableCollection<PessoaListItemDto>();

            // Verify filaCollectionView exists before setting ItemsSource
            if (filaCollectionView != null)
            {
                filaCollectionView.ItemsSource = _fila;
                filaCollectionView.ReorderCompleted += OnFilaReorderCompleted;
            }

            // Debug: Log component initialization
            System.Diagnostics.Debug.WriteLine($"StackPage Constructor - bottomNavBar: {bottomNavBar != null}");
            System.Diagnostics.Debug.WriteLine($"StackPage Constructor - emptyQueueMessage: {emptyQueueMessage != null}");
            System.Diagnostics.Debug.WriteLine($"StackPage Constructor - queueStatusLabel: {queueStatusLabel != null}");
        }

        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();

            if (Handler != null)
            {
                // Inicializa o ServiceProvider quando o Handler estiver disponível
                _serviceProvider = ServiceProvider.FromPage(this);
                _queueService = _serviceProvider.GetService<IQueueService>();

                // Debug: Log component availability after handler changed
                System.Diagnostics.Debug.WriteLine($"OnHandlerChanged - bottomNavBar: {bottomNavBar != null}");
                System.Diagnostics.Debug.WriteLine($"OnHandlerChanged - emptyQueueMessage: {emptyQueueMessage != null}");
                System.Diagnostics.Debug.WriteLine($"OnHandlerChanged - queueStatusLabel: {queueStatusLabel != null}");
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Debug: Log component availability on appearing
            System.Diagnostics.Debug.WriteLine($"OnAppearing - bottomNavBar: {bottomNavBar != null}");
            System.Diagnostics.Debug.WriteLine($"OnAppearing - emptyQueueMessage: {emptyQueueMessage != null}");
            System.Diagnostics.Debug.WriteLine($"OnAppearing - queueStatusLabel: {queueStatusLabel != null}");

            LoadActiveQueueState(); // Chama o método local

            // Wait a bit to ensure XAML elements are fully loaded
            await Task.Delay(100);

            await CheckActiveQueueAsync(); // Nova verificação de fila ativa
        }

        // --- Métodos de Persistência da Fila Ativa na UI (usando Preferences) ---
        private void LoadActiveQueueState()
        {
            string filaJson = Preferences.Get(ActiveQueueKey, string.Empty);
            if (!string.IsNullOrEmpty(filaJson))
            {
                var loadedList = JsonSerializer.Deserialize<List<PessoaListItemDto>>(filaJson);
                _fila.Clear();
                foreach (var pessoaDto in loadedList)
                {
                    _fila.Add(pessoaDto);
                }
            }
            else
            {
                _fila.Clear(); // Garante que a lista está vazia se não houver dados
            }
        }

        private void SaveActiveQueueState(List<PessoaListItemDto> fila)
        {
            string filaJson = JsonSerializer.Serialize(fila);
            Preferences.Set(ActiveQueueKey, filaJson);
        }

        private async Task CheckActiveQueueAsync()
        {
            System.Diagnostics.Debug.WriteLine("CheckActiveQueueAsync - Starting");

            try
            {
                if (_queueService == null)
                {
                    System.Diagnostics.Debug.WriteLine("CheckActiveQueueAsync - QueueService is null, showing empty state");
                    // Fallback: mostrar estado vazio se serviço não estiver disponível
                    ShowEmptyQueueState();
                    return;
                }

                var activeEvent = await _queueService.GetActiveEventAsync();
                System.Diagnostics.Debug.WriteLine($"CheckActiveQueueAsync - ActiveEvent: {activeEvent?.Id}, FilaAtiva: {activeEvent?.FilaAtiva}");

                if (activeEvent == null || !activeEvent.FilaAtiva)
                {
                    System.Diagnostics.Debug.WriteLine("CheckActiveQueueAsync - No active event, showing empty state");
                    ShowEmptyQueueState();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("CheckActiveQueueAsync - Active event found, showing active state");
                    ShowActiveQueueState();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CheckActiveQueueAsync - Error: {ex.Message}");
                ShowEmptyQueueState();
            }
        }

        private void ShowEmptyQueueState()
        {
            System.Diagnostics.Debug.WriteLine("ShowEmptyQueueState - Starting");

            MainThread.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    // Atualizar título para "My Karaoke"
                    UpdateHeaderTitle(false);

                    if (emptyQueueMessage != null)
                    {
                        emptyQueueMessage.IsVisible = true;
                        System.Diagnostics.Debug.WriteLine("ShowEmptyQueueState - emptyQueueMessage set to visible");
                    }

                    if (filaCollectionView != null)
                    {
                        filaCollectionView.IsVisible = false;
                        System.Diagnostics.Debug.WriteLine("ShowEmptyQueueState - filaCollectionView set to hidden");
                    }

                    if (bottomNavBar != null)
                    {
                        bottomNavBar.IsVisible = true;
                        System.Diagnostics.Debug.WriteLine("ShowEmptyQueueState - bottomNavBar set to VISIBLE");
                    }

                    if (queueStatusLabel != null)
                    {
                        queueStatusLabel.Text = "---";
                        System.Diagnostics.Debug.WriteLine("ShowEmptyQueueState - queueStatusLabel set to ---");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"ShowEmptyQueueState - Exception: {ex.Message}");
                }
            });
        }

        private void ShowActiveQueueState()
        {
            System.Diagnostics.Debug.WriteLine("ShowActiveQueueState - Starting");

            MainThread.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    // Atualizar título para "Fila"
                    UpdateHeaderTitle(true);

                    if (emptyQueueMessage != null)
                    {
                        emptyQueueMessage.IsVisible = false;
                        System.Diagnostics.Debug.WriteLine("ShowActiveQueueState - emptyQueueMessage set to hidden");
                    }

                    if (filaCollectionView != null)
                    {
                        filaCollectionView.IsVisible = true;
                        System.Diagnostics.Debug.WriteLine("ShowActiveQueueState - filaCollectionView set to visible");
                    }

                    if (bottomNavBar != null)
                    {
                        bottomNavBar.IsVisible = false;
                        System.Diagnostics.Debug.WriteLine("ShowActiveQueueState - bottomNavBar set to HIDDEN");
                    }

                    int participantCount = _fila?.Count ?? 0;
                    if (queueStatusLabel != null)
                    {
                        queueStatusLabel.Text = participantCount.ToString();
                        System.Diagnostics.Debug.WriteLine($"ShowActiveQueueState - queueStatusLabel set to {participantCount}");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"ShowActiveQueueState - Exception: {ex.Message}");
                }
            });
        }

        // DEBUG METHOD: Force empty queue state for testing
        public void ForceEmptyQueueState()
        {
            System.Diagnostics.Debug.WriteLine("ForceEmptyQueueState - MANUAL TRIGGER");
            MainThread.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    ShowEmptyQueueState();
                    System.Diagnostics.Debug.WriteLine("ForceEmptyQueueState - ShowEmptyQueueState called successfully");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"ForceEmptyQueueState - Exception: {ex.Message}");
                }
            });
        }

        private async void OnParticipouClicked(object sender, EventArgs e)
        {
            try
            {
                PessoaListItemDto pessoaDto = (PessoaListItemDto)((Button)sender).CommandParameter;
                await _queueService.RecordParticipationAsync(pessoaDto.Id, ParticipacaoStatus.Presente); // Passa o ID
                pessoaDto.IncrementarParticipacoes(); // Atualiza o contador no DTO
                SaveActiveQueueState(_fila.ToList());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"OnParticipouClicked - Error: {ex.Message}");
            }
        }

        private async void OnAusenteClicked(object sender, EventArgs e)
        {
            try
            {
                PessoaListItemDto pessoaDto = (PessoaListItemDto)((Button)sender).CommandParameter;
                await _queueService.RecordParticipationAsync(pessoaDto.Id, ParticipacaoStatus.Ausente); // Passa o ID
                pessoaDto.IncrementarAusencias(); // Atualiza o contador no DTO
                SaveActiveQueueState(_fila.ToList());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"OnAusenteClicked - Error: {ex.Message}");
            }
        }

        private void OnMoveToBottomClicked(object sender, EventArgs e)
        {
            try
            {
                PessoaListItemDto pessoaDto = (PessoaListItemDto)((Button)sender).CommandParameter;
                if (_fila.Contains(pessoaDto))
                {
                    _fila.Remove(pessoaDto);
                    _fila.Add(pessoaDto);
                    SaveActiveQueueState(_fila.ToList());
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"OnMoveToBottomClicked - Error: {ex.Message}");
            }
        }

        private void OnFilaReorderCompleted(object sender, EventArgs e)
        {
            try
            {
                SaveActiveQueueState(_fila.ToList());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"OnFilaReorderCompleted - Error: {ex.Message}");
            }
        }

        // Método para o botão voltar
        private void OnBackButtonClicked(object sender, EventArgs e)
        {
            try
            {
                ExitApplication();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"OnBackButtonClicked - Error: {ex.Message}");
            }
        }

        // Captura o botão voltar do Android
        protected override bool OnBackButtonPressed()
        {
            try
            {
                ExitApplication();
                return true; // Impede o comportamento padrão
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"OnBackButtonPressed - Error: {ex.Message}");
                return true;
            }
        }

        // Método para fechar a aplicação
        private void ExitApplication()
        {
            try
            {
                Application.Current?.Quit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ExitApplication - Error: {ex.Message}");
            }
        }

        // NEW: Bottom Navigation Event Handlers
        private void OnBottomNavLocaisClicked(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Locais clicked");
                // TODO: Navigate to Locais page
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"OnBottomNavLocaisClicked - Error: {ex.Message}");
            }
        }

        private async void OnBottomNavCantoresClicked(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Cantores clicked - navigating to PersonPage");

                if (_serviceProvider != null)
                {
                    await Navigation.PushAsync(new PersonPage());
                }
                else
                {
                    // Fallback: criar PersonPage diretamente
                    await Navigation.PushAsync(new PersonPage());
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"OnBottomNavCantoresClicked - Error: {ex.Message}");
                // Fallback final
                try
                {
                    await Navigation.PushAsync(new PersonPage());
                }
                catch
                {
                    System.Diagnostics.Debug.WriteLine("Failed to navigate to PersonPage");
                }
            }
        }

        private void OnBottomNavNovaFilaClicked(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Nova Fila clicked");
                // TODO: Create new queue functionality
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"OnBottomNavNovaFilaClicked - Error: {ex.Message}");
            }
        }

        private void OnBottomNavBandasMusicosClicked(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Bandas/Músicos clicked");
                // TODO: Navigate to Bandas/Músicos page
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"OnBottomNavBandasMusicosClicked - Error: {ex.Message}");
            }
        }

        private void OnBottomNavHistoricoClicked(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Histórico clicked");
                // TODO: Navigate to Histórico page
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"OnBottomNavHistoricoClicked - Error: {ex.Message}");
            }
        }

        private string GetString(string key, params object[] args)
        {
            string value = "";
            switch (key)
            {
                case "fila_vazia": value = "Não há participantes na fila para chamar."; break;
                case "call_next_participant": value = "Chamar Próximo Participante"; break;
                case "confirm_presence": value = "Confirmar Presença?"; break;
                case "present": value = "Presente"; break;
                case "absent": value = "Ausente"; break;
                case "call_next_participant_confirm": value = "Chamar {0}?"; break;
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
            try
            {
                if (filaCollectionView != null)
                    filaCollectionView.ReorderCompleted -= OnFilaReorderCompleted;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"OnDisappearing - Error: {ex.Message}");
            }
        }

        private void UpdateHeaderTitle(bool hasActiveQueue)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    if (headerTitle != null)
                    {
                        headerTitle.Text = hasActiveQueue ? "Fila" : "My Karaoke";
                        System.Diagnostics.Debug.WriteLine($"Header title updated to: {headerTitle.Text}");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"UpdateHeaderTitle - Error: {ex.Message}");
                }
            });
        }
    }
}