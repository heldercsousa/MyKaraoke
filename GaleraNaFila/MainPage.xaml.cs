// ####################################################################################################
// # Arquivo: GaleraNaFila/MainPage.xaml.cs
// # Descrição: Lógica da tela principal (modo participante).
// #            Atualizado para usar o QueueService da camada de Aplicação.
// ####################################################################################################
using GaleraNaFila.Contracts;
using GaleraNaFila.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage; // Para Preferences
using System.Text.Json;
using System;

namespace GaleraNaFila
{
    public partial class MainPage : ContentPage
    {
        private readonly IServiceProvider _serviceProvider; // NOVO: IServiceProvider injetado
        private readonly QueueService _queueService;
        private const string ActiveQueueKey = "ActiveFilaDeCQueue";

        public MainPage(QueueService queueService, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _queueService = queueService;
            _serviceProvider = serviceProvider; // Armazenar o IServiceProvider
        }

        private async void OnAddToQueueClicked(object sender, EventArgs e)
        {
            string fullName = fullNameEntry.Text;

            var result = await _queueService.AddPersonAsync(fullName);

            if (result.success)
            {
                validationMessageLabel.Text = "";

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
            }
        }

        private async void OnSwitchToAdminModeClicked(object sender, EventArgs e)
        {
            Preferences.Set("IsAdminMode", true);
            // CORRIGIDO: Não use App.Current.Services para navegar para uma página já registrada no DI.
            // Apenas instancie e navegue, o MAUI injetará as dependências automaticamente.
            await Navigation.PushAsync(_serviceProvider.GetRequiredService<AdminPage>());
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
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