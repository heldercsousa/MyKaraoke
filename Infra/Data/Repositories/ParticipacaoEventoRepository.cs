using MyVocaList.Domain;
using Microsoft.EntityFrameworkCore;
using MyVocaList.Infra.Utils;

namespace MyVocaList.Infra.Data.Repositories
{
    public class ParticipacaoEventoRepository : BaseRepository<ParticipacaoEvento>, IParticipacaoEventoRepository
    {
        public ParticipacaoEventoRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<ParticipacaoEvento>> GetParticipacoesByPessoaIdAndEventoIdAsync(int pessoaId, int eventoId)
        {
            Guard.AgainstNegativeOrZero(pessoaId, nameof(pessoaId));
            Guard.AgainstNegativeOrZero(eventoId, nameof(eventoId));

            return await _dbSet
                .Where(pe => pe.PessoaId == pessoaId && pe.EventoId == eventoId)
                .ToListAsync();
        }
    }
}
