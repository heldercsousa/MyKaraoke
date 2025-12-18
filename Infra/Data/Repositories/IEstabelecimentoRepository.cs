using MyVocaList.Domain;

namespace MyVocaList.Infra.Data.Repositories
{
    public interface IEstabelecimentoRepository : IBaseRepository<Estabelecimento>
    {
        /// <summary>
        /// Busca estabelecimento por nome exato
        /// </summary>
        Task<Estabelecimento?> GetByNomeAsync(string nome);

        /// <summary>
        /// Busca estabelecimentos por nome que começam com o termo
        /// </summary>
        Task<IEnumerable<Estabelecimento>> SearchByNomeStartsWithAsync(string searchTerm, int maxResults = 10);

        /// <summary>
        /// Busca estabelecimentos por nome que contém o termo
        /// </summary>
        Task<IEnumerable<Estabelecimento>> SearchByNomeContainsAsync(string searchTerm, int maxResults = 10);

        Task<IEnumerable<(Estabelecimento estabelecimento, bool hasEvents)>> SearchWithHasEventsAsync(string? query);
        Task<IEnumerable<(Estabelecimento estabelecimento, bool hasEvents)>> GetAllWithHasEventsAsync();
        Task<IEnumerable<(Estabelecimento estabelecimento, bool hasEvents)>> GetByIdsWithHasEventsAsync(IEnumerable<int> ids);

        /// <summary>
        /// Gets a paginated list of establishments with event information
        /// </summary>
        /// <param name="pageNumber">Page number (1-based)</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <param name="query">Optional search query</param>
        /// <returns>Tuple with list of establishments and total count</returns>
        Task<(IEnumerable<(Estabelecimento estabelecimento, bool hasEvents)> items, int totalCount)> GetPagedWithHasEventsAsync(
            int pageNumber,
            int pageSize,
            string? query = null);
    }
}