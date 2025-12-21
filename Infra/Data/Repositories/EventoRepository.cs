using MyVocaList.Domain;
using Microsoft.EntityFrameworkCore;
using MyVocaList.Infra.Utils;

namespace MyVocaList.Infra.Data.Repositories
{
    public class EventoRepository : BaseRepository<Evento>, IEventoRepository
    {
        public EventoRepository(AppDbContext context) : base(context) { }

        public async Task<Evento> GetActiveEventAsync()
        {
            return await _dbSet.Include(e => e.Estabelecimento) // Include the Estabelecimento
                               .FirstOrDefaultAsync(e => e.FilaAtiva);
        }

        public async Task SetActiveEventAsync(int eventId)
        {
            Guard.AgainstNegativeOrZero(eventId, nameof(eventId));

            var currentActive = await _dbSet.FirstOrDefaultAsync(e => e.FilaAtiva);
            if (currentActive != null && currentActive.Id != eventId) // Avoid deactivating if already active
            {
                currentActive.FilaAtiva = false;
                _dbSet.Update(currentActive);
            }

            var newActive = await _dbSet.FindAsync(eventId);
            if (newActive != null)
            {
                newActive.FilaAtiva = true;
                _dbSet.Update(newActive);
            }
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Checks if there are events associated with an establishment
        /// </summary>
        public async Task<bool> HasEventsByEstabelecimentoAsync(int estabelecimentoId)
        {
            Guard.AgainstNegativeOrZero(estabelecimentoId, nameof(estabelecimentoId));

            return await _context.Eventos
                .AnyAsync(e => e.EstabelecimentoId == estabelecimentoId);
        }
    }
}
