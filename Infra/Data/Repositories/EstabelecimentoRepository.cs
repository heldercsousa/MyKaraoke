using MyVocaList.Domain;
using Microsoft.EntityFrameworkCore;
using MyVocaList.Infra.Utils;

namespace MyVocaList.Infra.Data.Repositories
{
    public class EstabelecimentoRepository : BaseRepository<Estabelecimento>, IEstabelecimentoRepository
    {
        public EstabelecimentoRepository(AppDbContext context) : base(context) { }

        /// <summary>
        /// Gets establishment by exact name match (case and accent insensitive)
        /// Database-level collation handles case/accent insensitivity automatically
        /// Trimming handled automatically by DatabaseLoadingInterceptor
        /// </summary>
        public async Task<Estabelecimento?> GetByNomeAsync(string nome)
        {
            Guard.AgainstNullOrWhiteSpace(nome, nameof(nome));

            return await _context.Estabelecimentos
                .FirstOrDefaultAsync(e => e.Nome == nome);
        }

        /// <summary>
        /// Searches establishments by name starting with the search term (case and accent insensitive)
        /// IMPORTANT: SQLite LIKE operator (from StartsWith) doesn't respect custom collations
        /// Solution: Use explicit .ToLower() for case/accent insensitive search
        /// Trimming handled automatically by DatabaseLoadingInterceptor
        /// </summary>
        public async Task<IEnumerable<Estabelecimento>> SearchByNomeStartsWithAsync(string searchTerm, int maxResults = 10)
        {
            // Return empty list for invalid search (no exception for user input)
            if (Guard.IsNullOrWhiteSpace(searchTerm))
                return new List<Estabelecimento>();

            // Explicit .ToLower() for case-insensitive search (SQLite LIKE doesn't respect custom collations)
            var searchTermLower = searchTerm.ToLower();
            return await _context.Estabelecimentos
                .Where(e => e.Nome.ToLower().StartsWith(searchTermLower))
                .Take(maxResults)
                .OrderBy(e => e.Nome)
                .ToListAsync();
        }

        /// <summary>
        /// Searches establishments by name containing the search term (case and accent insensitive)
        /// IMPORTANT: SQLite LIKE operator (from Contains) doesn't respect custom collations
        /// Solution: Use explicit .ToLower() for case/accent insensitive search
        /// Trimming handled automatically by DatabaseLoadingInterceptor
        /// </summary>
        public async Task<IEnumerable<Estabelecimento>> SearchByNomeContainsAsync(string searchTerm, int maxResults = 10)
        {
            // Return empty list for invalid search (no exception for user input)
            if (Guard.IsNullOrWhiteSpace(searchTerm))
                return new List<Estabelecimento>();

            // Explicit .ToLower() for case-insensitive search (SQLite LIKE doesn't respect custom collations)
            var searchTermLower = searchTerm.ToLower();
            return await _context.Estabelecimentos
                .Where(e => e.Nome.ToLower().Contains(searchTermLower))
                .Take(maxResults)
                .OrderBy(e => e.Nome)
                .ToListAsync();
        }

        /// <summary>
        /// Gets all establishments ordered by name
        /// </summary>
        public override async Task<IEnumerable<Estabelecimento>> GetAllAsync()
        {
            return await _context.Estabelecimentos
                .OrderBy(e => e.Nome)
                .ToListAsync();
        }

        /// <summary>
        /// Searches establishments with event information (case and accent insensitive)
        /// IMPORTANT: SQLite LIKE operator (from Contains) doesn't respect custom collations
        /// Solution: Use explicit .ToLower() for case/accent insensitive search
        /// Trimming handled automatically by DatabaseLoadingInterceptor
        /// </summary>
        public async Task<IEnumerable<(Estabelecimento estabelecimento, bool hasEvents)>> SearchWithHasEventsAsync(string? query)
        {
            var q = _context.Estabelecimentos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                // Explicit .ToLower() for case-insensitive search (SQLite LIKE doesn't respect custom collations)
                var queryLower = query.ToLower();
                q = q.Where(e => e.Nome.ToLower().Contains(queryLower));
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

        /// <summary>
        /// Gets a paginated list of ALL establishments with event information flag
        /// Does NOT filter - returns all establishments with hasEvents boolean flag
        /// Uses Skip/Take for efficient database pagination
        /// IMPORTANT: SQLite LIKE operator (from Contains) doesn't respect custom collations
        /// Solution: Use explicit .ToLower() for case/accent insensitive search
        /// Trimming handled automatically by DatabaseLoadingInterceptor
        /// </summary>
        public async Task<(IEnumerable<(Estabelecimento estabelecimento, bool hasEvents)> items, int totalCount)> GetPagedWithEventInfoAsync(
            int pageNumber,
            int pageSize,
            string? query = null)
        {
            var q = _context.Estabelecimentos.AsQueryable();

            // Apply search filter if provided (explicit .ToLower() for case-insensitive search)
            if (!string.IsNullOrWhiteSpace(query))
            {
                // Explicit .ToLower() for case-insensitive search (SQLite LIKE doesn't respect custom collations)
                var queryLower = query.ToLower();
                q = q.Where(e => e.Nome.ToLower().Contains(queryLower));
            }

            // Get total count for pagination info (executes COUNT(*) query)
            var totalCount = await q.CountAsync();

            // Apply pagination with Skip/Take (LIMIT/OFFSET in SQL)
            var items = await q
                .OrderBy(e => e.Nome)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(e => new
                {
                    Estabelecimento = e,
                    HasEvents = e.Eventos.Any()
                })
                .Select(x => ValueTuple.Create(x.Estabelecimento, x.HasEvents))
                .ToListAsync();

            return (items, totalCount);
        }
    }
}