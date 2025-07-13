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
        Task<(bool success, string message)> DeleteEstabelecimentoAsync(int id);
        Task<IEnumerable<Estabelecimento>> GetAllEstabelecimentosAsync();
        Task<Estabelecimento?> GetEstabelecimentoByIdAsync(int id);
        bool ShouldShowCharacterCounter(int currentLength);
        (string text, bool isWarning, bool isError) GetCharacterCounterInfo(int currentLength);
    }
}
