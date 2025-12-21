using Microsoft.EntityFrameworkCore;
using MyVocaList.Domain;
using MyVocaList.Infra.Utils;

namespace MyVocaList.Infra.Data.Repositories
{
    public class PessoaRepository : BaseRepository<Pessoa>, IPessoaRepository
    {
        private readonly ITextNormalizer _textNormalizer; // 🔄 MUDANÇA: nome atualizado

        public PessoaRepository(AppDbContext context, ITextNormalizer textNormalizer) // 🔄 MUDANÇA
            : base(context)
        {
            _textNormalizer = textNormalizer; // 🔄 MUDANÇA: nome atualizado
        }

        public async Task<Pessoa> GetByNomeCompletoAsync(string nomeCompleto)
        {
            Guard.AgainstNullOrWhiteSpace(nomeCompleto, nameof(nomeCompleto));
            return await _dbSet.FirstOrDefaultAsync(p => p.NomeCompleto == nomeCompleto);
        }

        /// <summary>
        /// Optimized search by normalized name
        /// </summary>
        public async Task<List<Pessoa>> SearchByNameAsync(string searchTerm, int maxResults = 10)
        {
            // Return empty list for invalid search (no exception for user input)
            if (Guard.IsNullOrWhiteSpace(searchTerm))
                return new List<Pessoa>();

            // Normalize search term
            var normalizedSearch = _textNormalizer.NormalizeName(searchTerm);

            Console.WriteLine($"Searching: '{searchTerm}' → normalized: '{normalizedSearch}'");

            // Optimized search using normalized column index
            var results = await _dbSet
                .Where(p => p.NomeCompletoNormalizado.Contains(normalizedSearch))
                .OrderBy(p => p.NomeCompleto)
                .Take(maxResults)
                .ToListAsync();

            Console.WriteLine($"Found {results.Count} results");

            return results;
        }

        // FUNCIONALIDADE: Busca otimizada que inicia com termo
        public async Task<List<Pessoa>> SearchByNameStartsWithAsync(string searchTerm, int maxResults = 10)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<Pessoa>();

            var normalizedSearch = _textNormalizer.NormalizeName(searchTerm); // 🔄 MUDANÇA

            // Busca que INICIA com o termo (mais precisa para autocompletar)
            return await _dbSet
                .Where(p => p.NomeCompletoNormalizado.StartsWith(normalizedSearch))
                .OrderBy(p => p.NomeCompleto)
                .Take(maxResults)
                .ToListAsync();
        }

        // FUNCIONALIDADE: Busca por qualquer palavra no nome
        public async Task<List<Pessoa>> SearchByAnyWordAsync(string searchTerm, int maxResults = 10)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<Pessoa>();

            string normalizedSearch = _textNormalizer.NormalizeName(searchTerm); // 🔄 MUDANÇA
            var searchWords = normalizedSearch.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (searchWords.Length == 0)
                return new List<Pessoa>();

            var query = _dbSet.AsQueryable();

            // Busca pessoas que contenham QUALQUER uma das palavras
            foreach (var word in searchWords)
            {
                var currentWord = word; // Captura para lambda
                query = query.Where(p => p.NomeCompletoNormalizado.Contains(currentWord));
            }

            return await query
                .OrderBy(p => p.NomeCompleto)
                .Take(maxResults)
                .ToListAsync();
        }

        // FUNCIONALIDADE: Verifica se pessoa existe com nome normalizado
        public async Task<Pessoa> GetByNormalizedNameAsync(string nomeCompleto)
        {
            var normalizedName = _textNormalizer.NormalizeName(nomeCompleto); // 🔄 MUDANÇA

            return await _dbSet
                .FirstOrDefaultAsync(p => p.NomeCompletoNormalizado == normalizedName);
        }
    }
}