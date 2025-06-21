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

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadActiveQueueState(); // Chama o método local
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

        private async void OnCallNextParticipantClicked(object sender, EventArgs e)
        {
            if (_fila.Count == 0)
            {
                await DisplayAlert("Fila Vazia", GetString("fila_vazia"), "OK");
                return;
            }

            PessoaListItemDto proximo = _fila[0];

            bool compareceu = await DisplayAlert(
                GetString("call_next_participant"),
                GetString("call_next_participant_confirm", proximo.NomeCompleto) + "\n\n" + GetString("confirm_presence"),
                GetString("present"),
                GetString("absent")
            );

            ParticipacaoStatus status = compareceu ? ParticipacaoStatus.Presente : ParticipacaoStatus.Ausente;

            // Registra a participação/ausência no histórico (DB)
            await _queueService.RecordParticipationAsync(proximo.Id, status); // Passa o ID da Pessoa

            // Atualiza os contadores no DTO em memória
            if (status == ParticipacaoStatus.Presente)
            {
                proximo.IncrementarParticipacoes();
            }
            else
            {
                proximo.IncrementarAusencias();
            }

            // Move a pessoa do DTO da primeira posição para o final da fila ativa
            _fila.RemoveAt(0);
            _fila.Add(proximo);

            // Salva o novo estado da fila (com a nova ordem e contadores atualizados) nas Preferences
            SaveActiveQueueState(_fila.ToList());
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

        private async void OnSwitchToSimpleModeClicked(object sender, EventArgs e)
        {
            Preferences.Set("IsAdminMode", false);
            await Navigation.PopToRootAsync();
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
