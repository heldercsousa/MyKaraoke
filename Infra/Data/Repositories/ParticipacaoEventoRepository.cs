using GaleraNaFila.Domain;
using GaleraNaFila.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GaleraNaFila.Infra.Data.Repositories
{
    public class ParticipacaoEventoRepository : BaseRepository<ParticipacaoEvento>, IParticipacaoEventoRepository
    {
        public ParticipacaoEventoRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<ParticipacaoEvento>> GetParticipacoesByPessoaIdAndEventoIdAsync(int pessoaId, int eventoId)
        {
            return await _dbSet
                .Where(pe => pe.PessoaId == pessoaId && pe.EventoId == eventoId)
                .ToListAsync();
        }
    }
}
