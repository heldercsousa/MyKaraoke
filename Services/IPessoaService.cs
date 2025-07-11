using MyKaraoke.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyKaraoke.Services
{
    public interface IPessoaService
    {
        // Constantes para validação híbrida
        int MaxInputLength { get; }
        int MaxDatabaseLength { get; }
        int ShowCounterAt { get; }

        // Validação
        (bool isValid, string message) ValidateNameInput(string name);
        (bool isValid, string message) ValidateNameForDatabase(string name);
        (bool isValid, string message) ValidateBirthday(string birthday);
        (bool isValid, string message) ValidateEmail(string email);

        // Operações de cadastro e busca
        Task<(bool success, string message, Pessoa? person)> CreatePersonAsync(string fullName, string birthday = null, string email = null);
        Task<Pessoa?> GetPersonByIdAsync(int id);
        Task<Pessoa?> GetPersonByNameAsync(string name);
        Task<IEnumerable<Pessoa>> SearchPersonsAsync(string searchTerm, int maxResults = 5);
        Task<IEnumerable<Pessoa>> SearchPersonsStartsWithAsync(string searchTerm, int maxResults = 3);

        // Utilitários
        (string text, bool isWarning, bool isError) GetCharacterCounterInfo(int currentLength);
        bool ShouldShowCharacterCounter(int currentLength);
    }
}
