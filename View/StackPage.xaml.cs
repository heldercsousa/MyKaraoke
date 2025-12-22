using MyVocaList.Contracts.Models;
using MyVocaList.Domain;
using MyVocaList.Services;
using MyVocaList.View.Extensions;
using MyVocaList.View.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.Json;
using System.Windows.Input;
using Serilog;

namespace MyVocaList.View
{
    public partial class StackPage : ContentPage, IManipulableDataPage
    {
        private static readonly Serilog.ILogger Logger = Log.ForContext<StackPage>();

        private IQueueService _queueService;
        private ServiceProvider _serviceProvider;
        private ObservableCollection<PessoaListItemDto> _fila;
        private const string ActiveQueueKey = "ActiveFilaDeCQueue";

        // Propriedade para o badge do card
        private string _queueStatusText = "---";
        public string QueueStatusText
        {
            get => _queueStatusText;
            set
            {
                if (_queueStatusText != value)
                {
                    _queueStatusText = value;
                    OnPropertyChanged(nameof(QueueStatusText));
                }
            }
        }

        #region IManipulableDataPage Members

        public ICommand LoadDataCommand { get; private set; }
        public string FriendlyName => "Fila";
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public StackPage()
        {
            InitializeComponent();

            _fila = new ObservableCollection<PessoaListItemDto>();
            LoadDataCommand = new Command(async () => await InitializeAndLoadDataAsync());
            this.BindingContext = this;

            if (filaCollectionView != null)
            {
                filaCollectionView.ItemsSource = _fila;
                filaCollectionView.ReorderCompleted += OnFilaReorderCompleted;
            }

            Logger.Debug("StackPage Constructor - bottomNav: {BottomNavExists}", bottomNav != null);
        }

        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();

            if (Handler != null)
            {
                _serviceProvider = ServiceProvider.FromPage(this);
                _queueService = _serviceProvider.GetService<IQueueService>();

                // ✅ SIMPLIFICADO: Sem evento manual - SafeNavigationBehavior cuida da navegação
                // bottomNav.LocaisClicked -= OnBottomNavLocaisClicked;
                // bottomNav.LocaisClicked += OnBottomNavLocaisClicked;

                if (filaCollectionView != null)
                {
                    filaCollectionView.ReorderCompleted -= OnFilaReorderCompleted;
                    filaCollectionView.ReorderCompleted += OnFilaReorderCompleted;
                }
            }
            else
            {
                if (filaCollectionView != null)
                {
                    filaCollectionView.ReorderCompleted -= OnFilaReorderCompleted;
                }
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        // ===== MÉTODO DE BYPASS PARA SMARTPAGELIFECYCLEBEHAVIOR =====

        /// <summary>
        /// 🎯 BYPASS: Método que o SmartPageLifecycleBehavior chamará se necessário
        /// </summary>
        private async Task OnAppearingBypass()
        {
            try
            {
                Logger.Debug("OnAppearingBypass executed");

                // ✅ SIMPLES: Usa extension method padrão
                await this.ExecuteStandardBypass();

                Logger.Debug("OnAppearingBypass completed successfully");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error in OnAppearingBypass");
            }
        }

        // ===== MÉTODOS ORIGINAIS PRESERVADOS =====

        private async Task InitializeAndLoadDataAsync()
        {
            try
            {
                Logger.Debug("InitializeAndLoadDataAsync - Starting");
                LoadActiveQueueState();
                await Task.Delay(100);
                await CheckActiveQueueAsync();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error in InitializeAndLoadDataAsync");
                ShowEmptyQueueState();
            }
        }

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
                _fila.Clear();
            }
        }

        private void SaveActiveQueueState(List<PessoaListItemDto> fila)
        {
            string filaJson = JsonSerializer.Serialize(fila);
            Preferences.Set(ActiveQueueKey, filaJson);
        }

        private async Task CheckActiveQueueAsync()
        {
            Logger.Debug("CheckActiveQueueAsync - Starting");

            try
            {
                if (_queueService == null)
                {
                    Logger.Warning("CheckActiveQueueAsync - QueueService is null, showing empty state");
                    ShowEmptyQueueState();
                    return;
                }

                var activeEvent = await _queueService.GetActiveEventAsync();
                Logger.Debug("CheckActiveQueueAsync - ActiveEvent: {EventId}, FilaAtiva: {FilaAtiva}", activeEvent?.Id, activeEvent?.FilaAtiva);

                if (activeEvent == null || !activeEvent.FilaAtiva)
                {
                    Logger.Debug("CheckActiveQueueAsync - No active event, showing empty state");
                    ShowEmptyQueueState();
                }
                else
                {
                    Logger.Debug("CheckActiveQueueAsync - Active event found, showing active state");
                    ShowActiveQueueState();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error in CheckActiveQueueAsync");
                ShowEmptyQueueState();
            }
        }

        private async void ShowEmptyQueueState()
        {
            Logger.Debug("ShowEmptyQueueState - Starting");

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                try
                {
                    if (emptyQueueMessage != null)
                    {
                        emptyQueueMessage.IsVisible = true;
                        Logger.Debug("ShowEmptyQueueState - emptyQueueMessage set to visible");
                    }

                    if (filaCollectionView != null)
                    {
                        filaCollectionView.IsVisible = false;
                        Logger.Debug("ShowEmptyQueueState - filaCollectionView set to hidden");
                    }

                    QueueStatusText = "---";
                    Logger.Debug("ShowEmptyQueueState - QueueStatusText set to ---");
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Exception in ShowEmptyQueueState");
                }
            });
        }

        private async void ShowActiveQueueState()
        {
            Logger.Debug("ShowActiveQueueState - Starting");

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                try
                {
                    if (emptyQueueMessage != null)
                    {
                        emptyQueueMessage.IsVisible = false;
                        Logger.Debug("ShowActiveQueueState - emptyQueueMessage set to hidden");
                    }

                    if (filaCollectionView != null)
                    {
                        filaCollectionView.IsVisible = true;
                        Logger.Debug("ShowActiveQueueState - filaCollectionView set to visible");
                    }

                    if (bottomNav != null)
                    {
                        bottomNav.IsVisible = false;
                        Logger.Debug("ShowActiveQueueState - bottomNav set to HIDDEN");
                    }

                    int participantCount = _fila?.Count ?? 0;
                    QueueStatusText = participantCount.ToString();
                    Logger.Debug("ShowActiveQueueState - QueueStatusText set to {ParticipantCount}", participantCount);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Exception in ShowActiveQueueState");
                }
            });
        }

        // ===== RESTO DOS MÉTODOS ORIGINAIS =====

        private async void OnParticipouClicked(object sender, EventArgs e)
        {
            try
            {
                PessoaListItemDto pessoaDto = (PessoaListItemDto)((Button)sender).CommandParameter;
                await _queueService.RecordParticipationAsync(pessoaDto.Id, ParticipacaoStatus.Presente);
                pessoaDto.IncrementarParticipacoes();
                SaveActiveQueueState(_fila.ToList());
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error in OnParticipouClicked");
            }
        }

        private async void OnAusenteClicked(object sender, EventArgs e)
        {
            try
            {
                PessoaListItemDto pessoaDto = (PessoaListItemDto)((Button)sender).CommandParameter;
                await _queueService.RecordParticipationAsync(pessoaDto.Id, ParticipacaoStatus.Ausente);
                pessoaDto.IncrementarAusencias();
                SaveActiveQueueState(_fila.ToList());
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error in OnAusenteClicked");
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
                Logger.Error(ex, "Error in OnMoveToBottomClicked");
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
                Logger.Error(ex, "Error in OnFilaReorderCompleted");
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

    }
}
