using MyVocaList.Domain;

namespace MyVocaList.Infra.Data.Repositories
{
    public interface IEstabelecimentoRepository : IBaseRepository<Estabelecimento>
    {
        /// <summary>
        /// Gets establishment by exact name match
        /// </summary>
        Task<Estabelecimento?> GetByNomeAsync(string nome);

        /// <summary>
        /// Searches establishments by name starting with the search term
        /// </summary>
        Task<IEnumerable<Estabelecimento>> SearchByNomeStartsWithAsync(string searchTerm, int maxResults = 10);

        /// <summary>
        /// Searches establishments by name containing the search term
        /// </summary>
        Task<IEnumerable<Estabelecimento>> SearchByNomeContainsAsync(string searchTerm, int maxResults = 10);

        Task<IEnumerable<(Estabelecimento estabelecimento, bool hasEvents)>> SearchWithHasEventsAsync(string? query);
        Task<IEnumerable<(Estabelecimento estabelecimento, bool hasEvents)>> GetAllWithHasEventsAsync();
        Task<IEnumerable<(Estabelecimento estabelecimento, bool hasEvents)>> GetByIdsWithHasEventsAsync(IEnumerable<int> ids);

        /// <summary>
        /// Gets a paginated list of ALL establishments with event information flag
        /// Does NOT filter - returns all establishments with hasEvents boolean flag
        /// </summary>
        /// <param name="pageNumber">Page number (1-based)</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <param name="query">Optional search query</param>
        /// <returns>Tuple with list of establishments (with event flag) and total count</returns>
        Task<(IEnumerable<(Estabelecimento estabelecimento, bool hasEvents)> items, int totalCount)> GetPagedWithEventInfoAsync(
            int pageNumber,
            int pageSize,
            string? query = null);
    }
}