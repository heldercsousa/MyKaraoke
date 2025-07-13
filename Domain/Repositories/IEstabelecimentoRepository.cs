using MyKaraoke.Domain;

namespace MyKaraoke.Domain.Repositories
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
    }
}