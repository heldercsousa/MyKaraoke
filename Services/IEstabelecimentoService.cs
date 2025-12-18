using MyVocaList.Contracts.DTOs.List;
using MyVocaList.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVocaList.Services
{
    public interface IEstabelecimentoService
    {
        (bool isValid, string message) ValidateNameInput(string name);
        Task<(bool success, string message, Estabelecimento? estabelecimento)> CreateEstabelecimentoAsync(string nome);
        Task<(bool success, string message)> UpdateEstabelecimentoAsync(int id, string novoNome);

        // Método antigo, mantido para compatibilidade e cenários de exclusão única
        Task<(bool success, string message)> DeleteEstabelecimentoAsync(int id);

        // NOVO MÉTODO para exclusão em lote
        Task<(bool success, string message)> DeleteEstabelecimentosAsync(IEnumerable<int> ids);

        Task<IEnumerable<Estabelecimento>> GetAllEstabelecimentosAsync();
        Task<Estabelecimento?> GetEstabelecimentoByIdAsync(int id);
        bool ShouldShowCharacterCounter(int currentLength);
        (string text, bool isWarning, bool isError) GetCharacterCounterInfo(int currentLength);

        Task<IEnumerable<EstabelecimentoListItemDto>> GetAllEstabelecimentosForListAsync();
        Task<IEnumerable<EstabelecimentoListItemDto>> SearchEstabelecimentosForListAsync(string query);

        /// <summary>
        /// Gets a paginated list of establishments for display
        /// </summary>
        /// <param name="pageNumber">Page number (1-based)</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <param name="query">Optional search query</param>
        /// <returns>Tuple with list of DTOs and total count</returns>
        Task<(IEnumerable<EstabelecimentoListItemDto> items, int totalCount)> GetPagedEstabelecimentosForListAsync(
            int pageNumber,
            int pageSize,
            string? query = null);
    }
}
