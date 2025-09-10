using MyKaraoke.Domain;
using MyKaraoke.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MyKaraoke.Infra.Data.Repositories
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
    }
}