using MyKaraoke.Domain;

namespace MyKaraoke.Infra.Data.Repositories
{
    public interface IParticipacaoEventoRepository : IBaseRepository<ParticipacaoEvento>
    {
        Task<IEnumerable<ParticipacaoEvento>> GetParticipacoesByPessoaIdAndEventoIdAsync(int pessoaId, int eventoId);
    }
}