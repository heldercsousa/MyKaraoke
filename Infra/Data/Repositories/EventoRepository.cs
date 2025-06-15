using GaleraNaFila.Domain;
using GaleraNaFila.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaleraNaFila.Infra.Data.Repositories
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
    }
}
