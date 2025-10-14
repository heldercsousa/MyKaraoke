using MyVocaList.Domain;

namespace MyVocaList.Infra.Data.Repositories
{
    public interface IParticipacaoEventoRepository : IBaseRepository<ParticipacaoEvento>
    {
        Task<IEnumerable<ParticipacaoEvento>> GetParticipacoesByPessoaIdAndEventoIdAsync(int pessoaId, int eventoId);
    }
}