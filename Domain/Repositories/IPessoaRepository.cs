namespace MyKaraoke.Domain.Repositories
{
    public interface IPessoaRepository : IBaseRepository<Pessoa>
    {
        Task<Pessoa> GetByNomeCompletoAsync(string nomeCompleto);

        // NOVAS FUNCIONALIDADES: Métodos de busca otimizada
        Task<List<Pessoa>> SearchByNameAsync(string searchTerm, int maxResults = 10);
        Task<List<Pessoa>> SearchByNameStartsWithAsync(string searchTerm, int maxResults = 10);
        Task<List<Pessoa>> SearchByAnyWordAsync(string searchTerm, int maxResults = 10);
        Task<Pessoa> GetByNormalizedNameAsync(string nomeCompleto);
    }
}