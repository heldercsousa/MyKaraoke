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
            filaCollectionView.ItemsSource = _fila;

            filaCollectionView.ReorderCompleted += OnFilaReorderCompleted;
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

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            LoadActiveQueueState(); // Chama o método local
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
            try
            {
                if (_queueService == null)
                {
                    // Fallback: mostrar estado vazio se serviço não estiver disponível
                    ShowEmptyQueueState();
                    return;
                }

                var activeEvent = await _queueService.GetActiveEventAsync();

                if (activeEvent == null || !activeEvent.FilaAtiva)
                {
                    ShowEmptyQueueState();
                }
                else
                {
                    ShowActiveQueueState();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao verificar fila ativa: {ex.Message}");
                ShowEmptyQueueState();
            }
        }

        private void ShowEmptyQueueState()
        {
            emptyQueueMessage.IsVisible = true;
            filaCollectionView.IsVisible = false;
            queueStatusLabel.Text = "---";
        }

        private void ShowActiveQueueState()
        {
            emptyQueueMessage.IsVisible = false;
            filaCollectionView.IsVisible = true;

            // Atualizar badge com número de participantes
            int participantCount = _fila?.Count ?? 0;
            queueStatusLabel.Text = participantCount.ToString();
        }

        private async void OnParticipouClicked(object sender, EventArgs e)
        {
            PessoaListItemDto pessoaDto = (PessoaListItemDto)((Button)sender).CommandParameter;
            await _queueService.RecordParticipationAsync(pessoaDto.Id, ParticipacaoStatus.Presente); // Passa o ID
            pessoaDto.IncrementarParticipacoes(); // Atualiza o contador no DTO
            SaveActiveQueueState(_fila.ToList());
        }

        private async void OnAusenteClicked(object sender, EventArgs e)
        {
            PessoaListItemDto pessoaDto = (PessoaListItemDto)((Button)sender).CommandParameter;
            await _queueService.RecordParticipationAsync(pessoaDto.Id, ParticipacaoStatus.Ausente); // Passa o ID
            pessoaDto.IncrementarAusencias(); // Atualiza o contador no DTO
            SaveActiveQueueState(_fila.ToList());
        }

        private void OnMoveToBottomClicked(object sender, EventArgs e)
        {
            PessoaListItemDto pessoaDto = (PessoaListItemDto)((Button)sender).CommandParameter;
            if (_fila.Contains(pessoaDto))
            {
                _fila.Remove(pessoaDto);
                _fila.Add(pessoaDto);
                SaveActiveQueueState(_fila.ToList());
            }
        }

        private void OnFilaReorderCompleted(object sender, EventArgs e)
        {
            SaveActiveQueueState(_fila.ToList());
        }

        // Método para o botão voltar
        private void OnBackButtonClicked(object sender, EventArgs e)
        {
            ExitApplication();
        }

        // Captura o botão voltar do Android
        protected override bool OnBackButtonPressed()
        {
            ExitApplication();
            return true; // Impede o comportamento padrão
        }

        // Método para fechar a aplicação
        private void ExitApplication()
        {
            Application.Current?.Quit();
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
            filaCollectionView.ReorderCompleted -= OnFilaReorderCompleted;
        }
    }
}