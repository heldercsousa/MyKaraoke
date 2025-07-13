using MyKaraoke.Domain;
using MyKaraoke.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MyKaraoke.Infra.Data.Repositories
{
    public class EventoRepository : BaseRepository<Evento>, IEventoRepository
    {
        public EventoRepository(AppDbContext context) : base(context) { }

        public async Task<Evento> GetActiveEventAsync()
        {
            return await _dbSet.Include(e => e.Estabelecimento) // Inclui o Estabelecimento
                               .FirstOrDefaultAsync(e => e.FilaAtiva);
        }

        public async Task SetActiveEventAsync(int eventId)
        {
            var currentActive = await _dbSet.FirstOrDefaultAsync(e => e.FilaAtiva);
            if (currentActive != null && currentActive.Id != eventId) // Evitar desativar se já é o ativo
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
        /// Verifica se há eventos associados a um estabelecimento
        /// Adicionar esta método à interface IEventoRepository e implementação EventoRepository
        /// </summary>
        public async Task<bool> HasEventsByEstabelecimentoAsync(int estabelecimentoId)
        {
            return await _context.Eventos
                .AnyAsync(e => e.EstabelecimentoId == estabelecimentoId);
        }
    }
}
