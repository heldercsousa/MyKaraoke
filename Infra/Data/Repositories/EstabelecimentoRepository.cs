using MyVocaList.Domain;
using Microsoft.EntityFrameworkCore;

namespace MyVocaList.Infra.Data.Repositories
{
    public class EstabelecimentoRepository : BaseRepository<Estabelecimento>, IEstabelecimentoRepository
    {
        public EstabelecimentoRepository(AppDbContext context) : base(context) { }

        /// <summary>
        /// Busca estabelecimento por nome (case-insensitive via NOCASE collation)
        /// </summary>
        public async Task<Estabelecimento?> GetByNomeAsync(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                return null;

            // ✅ CORREÇÃO: Apenas trim - NOCASE já configurado no AppDbContext
            return await _context.Estabelecimentos
                .FirstOrDefaultAsync(e => e.Nome == nome.Trim());
        }

        /// <summary>
        /// Busca estabelecimentos por nome que começam com o termo
        /// </summary>
        public async Task<IEnumerable<Estabelecimento>> SearchByNomeStartsWithAsync(string searchTerm, int maxResults = 10)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<Estabelecimento>();

            return await _context.Estabelecimentos
                .Where(e => e.Nome.StartsWith(searchTerm.Trim()))
                .Take(maxResults)
                .OrderBy(e => e.Nome)
                .ToListAsync();
        }

        /// <summary>
        /// Busca estabelecimentos por nome que contém o termo
        /// </summary>
        public async Task<IEnumerable<Estabelecimento>> SearchByNomeContainsAsync(string searchTerm, int maxResults = 10)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<Estabelecimento>();

            return await _context.Estabelecimentos
                .Where(e => e.Nome.Contains(searchTerm.Trim()))
                .Take(maxResults)
                .OrderBy(e => e.Nome)
                .ToListAsync();
        }

        /// <summary>
        /// Retorna todos os estabelecimentos ordenados por nome
        /// </summary>
        public override async Task<IEnumerable<Estabelecimento>> GetAllAsync()
        {
            return await _context.Estabelecimentos
                .OrderBy(e => e.Nome)
                .ToListAsync();
        }

        public async Task<IEnumerable<(Estabelecimento estabelecimento, bool hasEvents)>> SearchWithHasEventsAsync(string? query)
        {
            var q = _context.Estabelecimentos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                query = query.Trim();
                q = q.Where(e => e.Nome.Contains(query));
            }

            return await q
                .Select(e => new
                {
                    Estabelecimento = e,
                    HasEvents = e.Eventos.Any()
                })
                .OrderBy(x => x.Estabelecimento.Nome)
                .Select(x => ValueTuple.Create(x.Estabelecimento, x.HasEvents))
                .ToListAsync();
        }

        public async Task<IEnumerable<(Estabelecimento estabelecimento, bool hasEvents)>> GetAllWithHasEventsAsync()
        {
            return await _context.Estabelecimentos
                .Select(e => new
                {
                    Estabelecimento = e,
                    HasEvents = e.Eventos.Any()
                })
                .OrderBy(x => x.Estabelecimento.Nome)
                .Select(x => ValueTuple.Create(x.Estabelecimento, x.HasEvents))
                .ToListAsync();
        }

        public async Task<IEnumerable<(Estabelecimento estabelecimento, bool hasEvents)>> GetByIdsWithHasEventsAsync(IEnumerable<int> ids)
        {
            return await _context.Estabelecimentos
                .Where(e => ids.Contains(e.Id))
                .Select(e => new
                {
                    Estabelecimento = e,
                    HasEvents = e.Eventos.Any() // EXISTS otimizado
                })
                .Select(x => ValueTuple.Create(x.Estabelecimento, x.HasEvents))
                .ToListAsync();
        }
    }
}