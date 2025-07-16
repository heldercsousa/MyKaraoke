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
            System.Diagnostics.Debug.WriteLine($"StackPage Constructor - bottomNav: {bottomNav != null}");
            System.Diagnostics.Debug.WriteLine($"StackPage Constructor - emptyQueueMessage: {emptyQueueMessage != null}");
            System.Diagnostics.Debug.WriteLine($"StackPage Constructor - queueStatusLabel: {queueStatusLabel != null}");
        }

        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();

            if (Handler != null)
            {
                _serviceProvider = ServiceProvider.FromPage(this);
                _queueService = _serviceProvider.GetService<IQueueService>();

                // ✅ CORREÇÃO: Inscreve-se nos eventos da navbar aqui.
                if (bottomNav != null)
                {
                    bottomNav.LocaisClicked -= OnBottomNavLocaisClicked; // Garante que não haja duplicatas
                    bottomNav.LocaisClicked += OnBottomNavLocaisClicked;
                    // (faça o mesmo para outros botões)
                }
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            System.Diagnostics.Debug.WriteLine($"OnAppearing - bottomNav: {bottomNav != null}");
            System.Diagnostics.Debug.WriteLine($"OnAppearing - emptyQueueMessage: {emptyQueueMessage != null}");
            System.Diagnostics.Debug.WriteLine($"OnAppearing - queueStatusLabel: {queueStatusLabel != null}");

            LoadActiveQueueState(); // Chama o método local

            await Task.Delay(100);

            await CheckActiveQueueAsync(); // Nova verificação de fila ativa

            if (bottomNav != null)
            {
                // Chama o método padronizado da interface
                await bottomNav.ShowAsync();
            }
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

        private async void ShowEmptyQueueState()
        {
            System.Diagnostics.Debug.WriteLine("ShowEmptyQueueState - Starting");

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                try
                {
                    // ... (other UI updates)

                    if (bottomNav != null)
                    {
                        System.Diagnostics.Debug.WriteLine("ShowEmptyQueueState - bottomNav visibility NOT set here (controlled by its own StartShowAnimations)");
                    }

                    // ... (rest of the method)
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"ShowEmptyQueueState - Exception: {ex.Message}");
                }
            });
        }

        private async void ShowActiveQueueState()
        {
            System.Diagnostics.Debug.WriteLine("ShowActiveQueueState - Starting");

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                try
                {
                    // 🎯 CORREÇÃO: Para a animação quando há fila ativa
                    if (bottomNav != null)
                    {
                        try
                        {
                            //await bottomNav.StopNovaFilaAnimationAsync();
                            System.Diagnostics.Debug.WriteLine("ShowActiveQueueState - Animação Nova Fila parada");
                        }
                        catch (Exception animEx)
                        {
                            System.Diagnostics.Debug.WriteLine($"ShowActiveQueueState - Erro ao parar animação: {animEx.Message}");
                        }
                    }

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

                    if (bottomNav != null)
                    {
                        bottomNav.IsVisible = false;
                        System.Diagnostics.Debug.WriteLine("ShowActiveQueueState - bottomNav set to HIDDEN");
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

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
            try
            {
                // 🎯 CORREÇÃO: Para animações ao sair da página
                if (bottomNav != null)
                {
                    try
                    {
                        await bottomNav.HideAsync();
                        System.Diagnostics.Debug.WriteLine("OnDisappearing - Animações da InactiveQueueBottomNav paradas e barra escondida.");
                    }
                    catch (Exception animEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"OnDisappearing - Erro ao parar animação: {animEx.Message}");
                    }
                }

                if (filaCollectionView != null)
                    filaCollectionView.ReorderCompleted -= OnFilaReorderCompleted;

                if (bottomNav != null)
                {
                    await bottomNav.HideAsync();
                }
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
                    if (headerComponent != null)
                    {
                        headerComponent.Title = hasActiveQueue ? "Bandokê, Trend´s, 09 jul" : "My Karaoke";
                        System.Diagnostics.Debug.WriteLine($"Header title updated to: {headerComponent.Title}");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"UpdateHeaderTitle - Error: {ex.Message}");
                }
            });
        }

        private async void OnBottomNavLocaisClicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new SpotPage());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Falha ao navegar para SpotPage: {ex.Message}");
            }
        }

    }

}


//public partial class StackPage : ContentPage
//{
//    // ... ICommands para LocaisCommand, BandokeCommand, etc. ...

//    public StackPage()
//    {
//        InitializeComponent();
//        // Vincule os commands aqui. Exemplo:
//        // LocaisCommand = new Command(async () => await NavigateToSpotPage());
//        // this.BindingContext = this; // Para os commands funcionarem
//    }

//    // Um único handler para todos os cliques
//    private void OnBottomNavButtonClicked(object sender, NavBarButtonClickedEventArgs e)
//    {
//        System.Diagnostics.Debug.WriteLine($"Botão clicado: {e.ButtonConfig.Text}");

//        // A lógica de navegação já está nos ICommands vinculados no XAML.
//        // Você poderia ter um switch aqui se preferisse usar o evento em vez de commands.
//        // switch(e.ButtonConfig.Text)
//        // {
//        //     case "Locais": //...
//        //     break;
//        // }
//    }

//    // ... O resto do seu código ...
//    // O Behavior 'NavBarLifecycleBehavior' cuidará de chamar ShowAsync/HideAsync.
//}