using MyVocaList.Domain;
using Microsoft.EntityFrameworkCore;
using MyVocaList.Infra.Utils;
using System.Diagnostics;

namespace MyVocaList.Infra.Data.Repositories
{
    public class EstabelecimentoRepository : BaseRepository<Estabelecimento>, IEstabelecimentoRepository
    {
        public EstabelecimentoRepository(AppDbContext context) : base(context) { }

        /// <summary>
        /// Gets establishment by exact name match.
        /// Trimming handled automatically by DatabaseLoadingInterceptor
        /// </summary>
        public async Task<Estabelecimento?> GetByNomeAsync(string nome) => await ( Guard.IsNullOrWhiteSpace(nome) ? 
            Task.FromResult<Estabelecimento?>(null) : _context.Estabelecimentos.FirstOrDefaultAsync(e => e.Nome == nome));

        /// <summary>
        /// Searches establishments by name starting with the search term (case and accent insensitive).
        /// SQLite-specific: LIKE ignores column collation, so we must explicitly COLLATE both operands.
        /// When migrating to SQL Server: replace with simple StartsWith() which respects column collation.
        /// Trimming handled automatically by DatabaseLoadingInterceptor
        /// </summary>
        public async Task<IEnumerable<Estabelecimento>> SearchByNomeStartsWithAsync(string searchTerm, int maxResults = 10)
        {
            if (Guard.IsNullOrWhiteSpace(searchTerm))
                return new List<Estabelecimento>();

            // SQLite workaround: LIKE ignores collation, so we explicitly COLLATE both sides
            return await _context.Estabelecimentos
                .Where(e => EF.Functions.Like(
                    EF.Functions.Collate(e.Nome, "NOCASE_NOACCENT"),
                    EF.Functions.Collate(searchTerm, "NOCASE_NOACCENT") + "%"))
                .Take(maxResults)
                .OrderBy(e => e.Nome)
                .ToListAsync();
        }

        /// <summary>
        /// Searches establishments by name containing the search term (case and accent insensitive).
        /// SQLite-specific: LIKE ignores column collation, so we must explicitly COLLATE both operands.
        /// When migrating to SQL Server: replace with simple Contains() which respects column collation.
        /// Trimming handled automatically by DatabaseLoadingInterceptor
        /// </summary>
        public async Task<IEnumerable<Estabelecimento>> SearchByNomeContainsAsync(string searchTerm, int maxResults = 10)
        {
            if (Guard.IsNullOrWhiteSpace(searchTerm))
                return new List<Estabelecimento>();

            // SQLite workaround: LIKE ignores collation, so we explicitly COLLATE both sides
            return await _context.Estabelecimentos
                .Where(e => EF.Functions.Like(
                    EF.Functions.Collate(e.Nome, "NOCASE_NOACCENT"),
                    "%" + EF.Functions.Collate(searchTerm, "NOCASE_NOACCENT") + "%"))
                .Take(maxResults)
                .OrderBy(e => e.Nome)
                .ToListAsync();
        }

        /// <summary>
        /// Gets all establishments ordered by name
        /// </summary>
        public override async Task<IEnumerable<Estabelecimento>> GetAllAsync() =>
            await _context.Estabelecimentos
                .OrderBy(e => e.Nome)
                .ToListAsync();

        /// <summary>
        /// Searches establishments with event information (case and accent insensitive).
        /// SQLite-specific: LIKE ignores column collation, so we must explicitly COLLATE both operands.
        /// When migrating to SQL Server: replace with simple Contains() which respects column collation.
        /// Trimming handled automatically by DatabaseLoadingInterceptor
        /// </summary>
        public async Task<IEnumerable<(Estabelecimento estabelecimento, bool hasEvents)>> SearchWithHasEventsAsync(string? query)
        {
            var q = _context.Estabelecimentos.AsQueryable();

            if (!Guard.IsNullOrWhiteSpace(query))
            {
                // SQLite workaround: LIKE ignores collation, so we explicitly COLLATE both sides
                q = q.Where(e => EF.Functions.Like(
                    EF.Functions.Collate(e.Nome, "NOCASE_NOACCENT"),
                    "%" + EF.Functions.Collate(query, "NOCASE_NOACCENT") + "%"));
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

        public async Task<IEnumerable<(Estabelecimento estabelecimento, bool hasEvents)>> GetAllWithHasEventsAsync() =>
            await _context.Estabelecimentos
                .Select(e => new
                {
                    Estabelecimento = e,
                    HasEvents = e.Eventos.Count > 0
                })
                .OrderBy(x => x.Estabelecimento.Nome)
                .Select(x => ValueTuple.Create(x.Estabelecimento, x.HasEvents))
                .ToListAsync();

        public async Task<IEnumerable<(Estabelecimento estabelecimento, bool hasEvents)>> GetByIdsWithHasEventsAsync(IEnumerable<int> ids) => 
            await _context.Estabelecimentos
                .Where(e => ids.Contains(e.Id))
                .Select(e => new
                {
                    Estabelecimento = e,
                    HasEvents = e.Eventos.Count > 0
                })
                .Select(x => ValueTuple.Create(x.Estabelecimento, x.HasEvents))
                .ToListAsync();

        /// <summary>
        /// Gets a paginated list of establishments with event information flag (case and accent insensitive search).
        /// SQLite-specific: LIKE ignores column collation, so we must explicitly COLLATE both operands.
        /// When migrating to SQL Server: replace with simple Contains() which respects column collation.
        /// Trimming handled automatically by DatabaseLoadingInterceptor
        /// </summary>
        public async Task<(IEnumerable<(Estabelecimento estabelecimento, bool hasEvents)> items, int totalCount)> GetPagedWithEventInfoAsync(
            int pageNumber,
            int pageSize,
            string? query = null)
        {
            var q = _context.Estabelecimentos.AsQueryable();

            if (!Guard.IsNullOrWhiteSpace(query))
            {
                // SQLite workaround: LIKE ignores collation, so we explicitly COLLATE both sides
                q = q.Where(e => EF.Functions.Like(
                    EF.Functions.Collate(e.Nome, "NOCASE_NOACCENT"),
                    "%" + EF.Functions.Collate(query, "NOCASE_NOACCENT") + "%"));
            }

            var totalCount = await q.CountAsync();

            var items = await q
                .OrderBy(e => e.Nome)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(e => new
                {
                    Estabelecimento = e,
                    HasEvents = e.Eventos.Count > 0
                })
                .Select(x => ValueTuple.Create(x.Estabelecimento, x.HasEvents))
                .ToListAsync();

            return (items, totalCount);
        }
    }
}