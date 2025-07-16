using MyKaraoke.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyKaraoke.Services
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
    }
}
