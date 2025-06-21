using MyKaraoke.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyKaraoke.Services
{
    public interface IQueueService
    {
        Task InitializeDatabaseAsync();
        Task<(bool success, string message, Pessoa? addedDomainPerson)> AddPersonAsync(string fullName);
        Task RecordParticipationAsync(int pessoaId, ParticipacaoStatus status);
        Task<Evento?> GetActiveEventAsync();
        Task SetActiveEventAsync(int eventId);
        Task<IEnumerable<Estabelecimento>> GetAllEstablishmentsAsync();
        Task<IEnumerable<Evento>> GetAllEventsAsync();
    }
}
