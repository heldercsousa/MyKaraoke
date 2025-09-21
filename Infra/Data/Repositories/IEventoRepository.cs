using MyKaraoke.Domain;

namespace MyKaraoke.Infra.Data.Repositories
{
    public interface IEventoRepository : IBaseRepository<Evento>
    {
        Task<Evento> GetActiveEventAsync();
        Task SetActiveEventAsync(int eventId);
        Task<bool> HasEventsByEstabelecimentoAsync(int estabelecimentoId);
    }
}