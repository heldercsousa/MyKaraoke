namespace MyKaraoke.Domain.Repositories
{
    public interface IPessoaRepository : IBaseRepository<Pessoa>
    {
        // Métodos básicos
        Task<Pessoa> GetByNomeCompletoAsync(string nomeCompleto);

        // Métodos de busca otimizada
        Task<List<Pessoa>> SearchByNameAsync(string searchTerm, int maxResults = 10);
        Task<List<Pessoa>> SearchByNameStartsWithAsync(string searchTerm, int maxResults = 10);
        Task<List<Pessoa>> SearchByAnyWordAsync(string searchTerm, int maxResults = 10);

        // Método para verificar duplicatas
        Task<Pessoa> GetByNormalizedNameAsync(string nomeCompleto);
    }
}