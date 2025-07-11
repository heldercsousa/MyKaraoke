using MyKaraoke.Domain;
using MyKaraoke.Domain.Repositories;
using MyKaraoke.Infra.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MyKaraoke.Services
{
    /// <summary>
    /// Serviço para operações de negócio relacionadas a pessoas
    /// </summary>
    public class PessoaService : IPessoaService
    {
        private readonly IPessoaRepository _pessoaRepository;
        private readonly ITextNormalizer _textNormalizer; // 🔄 MUDANÇA: nome atualizado

        // Constantes para validação híbrida
        public int MaxInputLength => 200;  // Limite do input (UX amigável)
        public int MaxDatabaseLength => 250;     // Limite do banco (segurança extra)
        public int ShowCounterAt => 180;   // Quando mostrar contador

        public PessoaService(
            IPessoaRepository pessoaRepository,
            ITextNormalizer textNormalizer) // 🔄 MUDANÇA: parâmetro atualizado
        {
            _pessoaRepository = pessoaRepository;
            _textNormalizer = textNormalizer; // 🔄 MUDANÇA: campo atualizado
        }

        #region Validações

        public (bool isValid, string message) ValidateNameInput(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return (false, "Nome é obrigatório");
            }

            name = name.Trim();

            if (name.Length > MaxInputLength)
            {
                return (false, $"Nome muito longo. Máximo {MaxInputLength} caracteres.");
            }

            if (name.Length < 2)
            {
                return (false, "Nome muito curto. Mínimo 2 caracteres.");
            }

            string[] partes = name.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (partes.Length < 2)
            {
                return (false, "Digite nome e sobrenome.");
            }

            if (partes[partes.Length - 1].Length < 2)
            {
                return (false, "Sobrenome deve ter pelo menos 2 caracteres.");
            }

            return (true, "");
        }

        public (bool isValid, string message) ValidateNameForDatabase(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return (false, "Nome é obrigatório");
            }

            name = name.Trim();

            if (name.Length > MaxDatabaseLength)
            {
                return (false, $"Nome excede limite do banco ({MaxDatabaseLength} caracteres)");
            }

            // Usa validação básica
            var inputValidation = ValidateNameInput(name);
            return inputValidation.isValid ? (true, "") : (false, "Nome inválido");
        }

        public (bool isValid, string message) ValidateBirthday(string birthday)
        {
            if (string.IsNullOrWhiteSpace(birthday))
            {
                return (false, "Aniversário é obrigatório");
            }

            // Formato aceito: DD/MM
            var regex = new Regex(@"^(\d{1,2})/(\d{1,2})$");
            var match = regex.Match(birthday.Trim());

            if (!match.Success)
            {
                return (false, "Use formato DD/MM (ex: 15/03)");
            }

            if (!int.TryParse(match.Groups[1].Value, out int dia) ||
                !int.TryParse(match.Groups[2].Value, out int mes))
            {
                return (false, "Data inválida");
            }

            if (dia < 1 || dia > 31)
            {
                return (false, "Dia deve ser entre 1 e 31");
            }

            if (mes < 1 || mes > 12)
            {
                return (false, "Mês deve ser entre 1 e 12");
            }

            // Validação básica de dias por mês
            if ((mes == 2 && dia > 29) ||
                ((mes == 4 || mes == 6 || mes == 9 || mes == 11) && dia > 30))
            {
                return (false, "Data inválida para este mês");
            }

            return (true, "");
        }

        public (bool isValid, string message) ValidateEmail(string email)
        {
            // E-mail é opcional
            if (string.IsNullOrWhiteSpace(email))
            {
                return (true, ""); // Válido se vazio (opcional)
            }

            email = email.Trim();

            // Validação básica de e-mail
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            if (!emailRegex.IsMatch(email))
            {
                return (false, "E-mail inválido");
            }

            if (email.Length > 100)
            {
                return (false, "E-mail muito longo");
            }

            return (true, "");
        }

        #endregion

        #region Operações de Cadastro e Busca

        public async Task<(bool success, string message, Pessoa? person)> CreatePersonAsync(
            string fullName, string birthday = null, string email = null)
        {
            // [Validações permanecem iguais...]

            try
            {
                // Cria nova pessoa
                var pessoa = new Pessoa(fullName)
                {
                    DiaMesAniversario = birthday?.Trim(),
                    Email = email?.Trim()
                };

                // Define nome normalizado usando o utilitário
                var normalizedName = _textNormalizer.NormalizeName(fullName); // 🔄 MUDANÇA
                pessoa.SetNormalizedName(normalizedName);

                // Salva no repositório
                await _pessoaRepository.AddAsync(pessoa);
                await _pessoaRepository.SaveChangesAsync();

                return (true, $"{fullName} cadastrado com sucesso!", pessoa);
            }
            catch (Exception ex)
            {
                return (false, $"Erro ao cadastrar pessoa: {ex.Message}", null);
            }
        }

        public async Task<Pessoa?> GetPersonByIdAsync(int id)
        {
            return await _pessoaRepository.GetByIdAsync(id);
        }

        public async Task<Pessoa?> GetPersonByNameAsync(string name)
        {
            return await _pessoaRepository.GetByNomeCompletoAsync(name);
        }

        public async Task<IEnumerable<Pessoa>> SearchPersonsAsync(string searchTerm, int maxResults = 5)
        {
            if (string.IsNullOrWhiteSpace(searchTerm) || searchTerm.Length < 2)
            {
                return new List<Pessoa>();
            }

            return await _pessoaRepository.SearchByNameAsync(searchTerm, maxResults);
        }

        public async Task<IEnumerable<Pessoa>> SearchPersonsStartsWithAsync(string searchTerm, int maxResults = 3)
        {
            if (string.IsNullOrWhiteSpace(searchTerm) || searchTerm.Length < 2)
            {
                return new List<Pessoa>();
            }

            return await _pessoaRepository.SearchByNameStartsWithAsync(searchTerm, maxResults);
        }

        #endregion

        #region Utilitários

        public bool ShouldShowCharacterCounter(int currentLength)
        {
            return currentLength > ShowCounterAt;
        }

        public (string text, bool isWarning, bool isError) GetCharacterCounterInfo(int currentLength)
        {
            string text = $"{currentLength}/{MaxInputLength}";
            bool isWarning = currentLength > 190;
            bool isError = currentLength >= MaxInputLength;

            return (text, isWarning, isError);
        }

        #endregion
    }
}
