using MyKaraoke.Domain;

namespace MyKaraoke.Services
{
    /// <summary>
    /// Interface para operações de fila e eventos
    /// Operações de pessoas delegadas para IPessoaService
    /// </summary>
    public interface IQueueService
    {
        // Operações de Fila
        Task<(bool success, string message, Pessoa? addedDomainPerson)> AddPersonToQueueAsync(
            string fullName, string birthday = null, string email = null);
        Task RecordParticipationAsync(int pessoaId, ParticipacaoStatus status);

        // Gerenciamento de Eventos
        Task<Evento?> GetActiveEventAsync();
        Task SetActiveEventAsync(int eventId);
        Task<IEnumerable<Estabelecimento>> GetAllEstablishmentsAsync();
        Task<IEnumerable<Evento>> GetAllEventsAsync();
    }
}