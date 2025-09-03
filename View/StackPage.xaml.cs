using MyKaraoke.Contracts;
using MyKaraoke.Domain;
using MyKaraoke.Services;
using MyKaraoke.View.Extensions;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.Json;
using System.Windows.Input;

namespace MyKaraoke.View
{
    public partial class StackPage : ContentPage, INotifyPropertyChanged
    {
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

        // Comando que o SmartPageLifecycleBehavior irá executar
        public ICommand LoadDataCommand { get; }

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

            System.Diagnostics.Debug.WriteLine($"StackPage Constructor - bottomNav: {bottomNav != null}");
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
                System.Diagnostics.Debug.WriteLine($"🎯 StackPage: OnAppearingBypass executado");

                // ✅ SIMPLES: Usa extension method padrão
                await this.ExecuteStandardBypass();

                System.Diagnostics.Debug.WriteLine($"✅ StackPage: OnAppearingBypass concluído com sucesso");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ StackPage: Erro no OnAppearingBypass: {ex.Message}");
            }
        }

        // ===== MÉTODOS ORIGINAIS PRESERVADOS =====

        private async Task InitializeAndLoadDataAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("StackPage: InitializeAndLoadDataAsync - Starting");
                LoadActiveQueueState();
                await Task.Delay(100);
                await CheckActiveQueueAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"StackPage: InitializeAndLoadDataAsync - Error: {ex.Message}");
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
            System.Diagnostics.Debug.WriteLine("StackPage: CheckActiveQueueAsync - Starting");

            try
            {
                if (_queueService == null)
                {
                    System.Diagnostics.Debug.WriteLine("StackPage: CheckActiveQueueAsync - QueueService is null, showing empty state");
                    ShowEmptyQueueState();
                    return;
                }

                var activeEvent = await _queueService.GetActiveEventAsync();
                System.Diagnostics.Debug.WriteLine($"StackPage: CheckActiveQueueAsync - ActiveEvent: {activeEvent?.Id}, FilaAtiva: {activeEvent?.FilaAtiva}");

                if (activeEvent == null || !activeEvent.FilaAtiva)
                {
                    System.Diagnostics.Debug.WriteLine("StackPage: CheckActiveQueueAsync - No active event, showing empty state");
                    ShowEmptyQueueState();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("StackPage: CheckActiveQueueAsync - Active event found, showing active state");
                    ShowActiveQueueState();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"StackPage: CheckActiveQueueAsync - Error: {ex.Message}");
                ShowEmptyQueueState();
            }
        }

        private async void ShowEmptyQueueState()
        {
            System.Diagnostics.Debug.WriteLine("StackPage: ShowEmptyQueueState - Starting");

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                try
                {
                    UpdateHeaderTitle(false);

                    if (emptyQueueMessage != null)
                    {
                        emptyQueueMessage.IsVisible = true;
                        System.Diagnostics.Debug.WriteLine("StackPage: ShowEmptyQueueState - emptyQueueMessage set to visible");
                    }

                    if (filaCollectionView != null)
                    {
                        filaCollectionView.IsVisible = false;
                        System.Diagnostics.Debug.WriteLine("StackPage: ShowEmptyQueueState - filaCollectionView set to hidden");
                    }

                    QueueStatusText = "---";
                    System.Diagnostics.Debug.WriteLine("StackPage: ShowEmptyQueueState - QueueStatusText set to ---");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"StackPage: ShowEmptyQueueState - Exception: {ex.Message}");
                }
            });
        }

        private async void ShowActiveQueueState()
        {
            System.Diagnostics.Debug.WriteLine("StackPage: ShowActiveQueueState - Starting");

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                try
                {
                    UpdateHeaderTitle(true);

                    if (emptyQueueMessage != null)
                    {
                        emptyQueueMessage.IsVisible = false;
                        System.Diagnostics.Debug.WriteLine("StackPage: ShowActiveQueueState - emptyQueueMessage set to hidden");
                    }

                    if (filaCollectionView != null)
                    {
                        filaCollectionView.IsVisible = true;
                        System.Diagnostics.Debug.WriteLine("StackPage: ShowActiveQueueState - filaCollectionView set to visible");
                    }

                    if (bottomNav != null)
                    {
                        bottomNav.IsVisible = false;
                        System.Diagnostics.Debug.WriteLine("StackPage: ShowActiveQueueState - bottomNav set to HIDDEN");
                    }

                    int participantCount = _fila?.Count ?? 0;
                    QueueStatusText = participantCount.ToString();
                    System.Diagnostics.Debug.WriteLine($"StackPage: ShowActiveQueueState - QueueStatusText set to {participantCount}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"StackPage: ShowActiveQueueState - Exception: {ex.Message}");
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
                System.Diagnostics.Debug.WriteLine($"StackPage: OnParticipouClicked - Error: {ex.Message}");
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
                System.Diagnostics.Debug.WriteLine($"StackPage: OnAusenteClicked - Error: {ex.Message}");
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
                System.Diagnostics.Debug.WriteLine($"StackPage: OnMoveToBottomClicked - Error: {ex.Message}");
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
                System.Diagnostics.Debug.WriteLine($"StackPage: OnFilaReorderCompleted - Error: {ex.Message}");
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

        private void UpdateHeaderTitle(bool hasActiveQueue)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    if (headerComponent != null)
                    {
                        headerComponent.Title = hasActiveQueue ? "Bandokê, Trend´s, 09 jul" : "My Karaoke";
                        System.Diagnostics.Debug.WriteLine($"StackPage: Header title updated to: {headerComponent.Title}");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"StackPage: UpdateHeaderTitle - Error: {ex.Message}");
                }
            });
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}