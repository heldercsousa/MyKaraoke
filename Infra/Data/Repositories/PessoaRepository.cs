using MyKaraoke.Domain;
using MyKaraoke.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MyKaraoke.Infra.Data.Repositories
{
    public class PessoaRepository : BaseRepository<Pessoa>, IPessoaRepository
    {
        public PessoaRepository(AppDbContext context) : base(context) { }

        public async Task<Pessoa> GetByNomeCompletoAsync(string nomeCompleto)
        {
            return await _dbSet.FirstOrDefaultAsync(p => p.NomeCompleto == nomeCompleto);
        }

        // NOVA FUNCIONALIDADE: Busca otimizada por nome normalizado
        public async Task<List<Pessoa>> SearchByNameAsync(string searchTerm, int maxResults = 10)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<Pessoa>();

            // Normaliza o termo de busca
            var normalizedSearch = Pessoa.NormalizeName(searchTerm);

            System.Diagnostics.Debug.WriteLine($"Buscando: '{searchTerm}' → normalizado: '{normalizedSearch}'");

            // Busca SUPER otimizada usando índice da coluna normalizada
            var results = await _dbSet
                .Where(p => p.NomeCompletoNormalizado.Contains(normalizedSearch))
                .OrderBy(p => p.NomeCompleto) // Ordena alfabeticamente
                .Take(maxResults)
                .ToListAsync();

            System.Diagnostics.Debug.WriteLine($"Encontrados {results.Count} resultados");

            return results;
        }

        // NOVA FUNCIONALIDADE: Busca otimizada que inicia com termo
        public async Task<List<Pessoa>> SearchByNameStartsWithAsync(string searchTerm, int maxResults = 10)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<Pessoa>();

            var normalizedSearch = Pessoa.NormalizeName(searchTerm);

            // Busca que INICIA com o termo (mais precisa para autocompletar)
            return await _dbSet
                .Where(p => p.NomeCompletoNormalizado.StartsWith(normalizedSearch))
                .OrderBy(p => p.NomeCompleto)
                .Take(maxResults)
                .ToListAsync();
        }

        // NOVA FUNCIONALIDADE: Busca por qualquer palavra no nome
        public async Task<List<Pessoa>> SearchByAnyWordAsync(string searchTerm, int maxResults = 10)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<Pessoa>();

            var normalizedSearch = Pessoa.NormalizeName(searchTerm);
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

        // NOVA FUNCIONALIDADE: Verifica se pessoa existe com nome normalizado
        public async Task<Pessoa> GetByNormalizedNameAsync(string nomeCompleto)
        {
            var normalizedName = Pessoa.NormalizeName(nomeCompleto);

            return await _dbSet
                .FirstOrDefaultAsync(p => p.NomeCompletoNormalizado == normalizedName);
        }
    }
}