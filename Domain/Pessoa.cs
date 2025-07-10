using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace MyKaraoke.Domain
{
    public class Pessoa
    {
        // Constantes para validação híbrida
        public const int MAX_INPUT_LENGTH = 200;  // Limite do input (UX amigável)
        public const int MAX_DB_LENGTH = 250;     // Limite do banco (segurança extra)
        public const int SHOW_COUNTER_AT = 180;   // Quando mostrar contador

        public int Id { get; set; } // Chave primária para o BD
        public string NomeCompleto { get; set; }
        public string NomeCompletoNormalizado { get; set; } // Nova propriedade para busca
        public int Participacoes { get; set; } = 0; // Contador de participações   
        public int Ausencias { get; set; } = 0; // Contador de ausências

        // NOVOS CAMPOS: Diferenciação inteligente
        public string DiaMesAniversario { get; set; } // Formato: "15/03" (obrigatório)
        public string Email { get; set; } // Opcional para marketing/diferenciação

        public Pessoa(string nomeCompleto)
        {
            NomeCompleto = nomeCompleto;
            NomeCompletoNormalizado = NormalizeName(nomeCompleto);
        }

        public Pessoa() { } // Construtor sem argumentos para EF Core

        // VALIDAÇÃO HÍBRIDA: Diferentes níveis de validação
        public static bool ValidarNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome) || nome.Trim().Length < 2)
            {
                return false;
            }
            string[] partes = nome.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (partes.Length < 2)
            {
                return false;
            }
            if (partes[partes.Length - 1].Length < 2)
            {
                return false;
            }
            return true;
        }

        // NOVA VALIDAÇÃO: Para dia/mês aniversário
        public static (bool isValid, string message) ValidarDiaMesAniversario(string diaMes)
        {
            if (string.IsNullOrWhiteSpace(diaMes))
            {
                return (false, "Aniversário é obrigatório");
            }

            // Formato aceito: DD/MM
            var regex = new Regex(@"^(\d{1,2})/(\d{1,2})$");
            var match = regex.Match(diaMes.Trim());

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

        // NOVA VALIDAÇÃO: Para e-mail opcional
        public static (bool isValid, string message) ValidarEmail(string email)
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

        // NOVA VALIDAÇÃO: Para input UX-friendly (200 chars)
        public static (bool isValid, string message) ValidarNomeInput(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
            {
                return (false, "Nome é obrigatório");
            }

            nome = nome.Trim();

            if (nome.Length > MAX_INPUT_LENGTH)
            {
                return (false, $"Nome muito longo. Máximo {MAX_INPUT_LENGTH} caracteres.");
            }

            if (nome.Length < 2)
            {
                return (false, "Nome muito curto. Mínimo 2 caracteres.");
            }

            string[] partes = nome.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
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

        // NOVA VALIDAÇÃO: Para banco super seguro (250 chars)
        public static (bool isValid, string message) ValidarNomeBanco(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
            {
                return (false, "Nome é obrigatório");
            }

            nome = nome.Trim();

            if (nome.Length > MAX_DB_LENGTH)
            {
                return (false, $"Nome excede limite do banco ({MAX_DB_LENGTH} caracteres)");
            }

            // Validação básica aplicada
            return ValidarNome(nome) ? (true, "") : (false, "Nome inválido");
        }

        // FUNCIONALIDADE: Normalização sem chaves duplicadas
        public static string NormalizeName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return string.Empty;

            // Converte para minúsculas primeiro
            name = name.ToLowerInvariant();

            // Mapeamento organizado por categoria, SEM DUPLICATAS
            var accentMap = new Dictionary<char, string>
            {
                // === VOGAIS COM ACENTOS ===
                // A
                {'á', "a"}, {'à', "a"}, {'ã', "a"}, {'â', "a"}, {'ä', "a"}, {'ā', "a"}, {'ă', "a"}, {'ą', "a"}, {'å', "a"},
                
                // E  
                {'é', "e"}, {'è', "e"}, {'ê', "e"}, {'ë', "e"}, {'ē', "e"}, {'ĕ', "e"}, {'ę', "e"}, {'ě', "e"},
                
                // I
                {'í', "i"}, {'ì', "i"}, {'î', "i"}, {'ï', "i"}, {'ī', "i"}, {'ĭ', "i"}, {'į', "i"}, {'ı', "i"},
                
                // O
                {'ó', "o"}, {'ò', "o"}, {'õ', "o"}, {'ô', "o"}, {'ö', "o"}, {'ō', "o"}, {'ŏ', "o"}, {'ø', "o"},
                
                // U
                {'ú', "u"}, {'ù', "u"}, {'û', "u"}, {'ü', "u"}, {'ū', "u"}, {'ŭ', "u"}, {'ų', "u"}, {'ů', "u"},

                // === CONSOANTES ESPECIAIS ===
                {'ç', "c"}, {'ć', "c"}, {'č', "c"},
                {'ñ', "n"}, {'ń', "n"}, {'ň', "n"},
                {'ł', "l"}, {'ľ', "l"},
                {'ś', "s"}, {'š', "s"}, {'ş', "s"},
                {'ź', "z"}, {'ż', "z"}, {'ž', "z"},
                {'ř', "r"}, {'ŕ', "r"},
                {'ť', "t"}, {'ď', "d"}, {'đ', "d"},
                {'ğ', "g"}, {'ý', "y"},

                // === CARACTERES ESPECIAIS ===
                {'ß', "ss"},    // Alemão
                {'œ', "oe"},    // Francês
                {'æ', "ae"},    // Francês/Nórdico
                {'þ', "th"},    // Islandês
                {'ð', "d"}      // Islandês
            };

            var result = new StringBuilder();

            foreach (char c in name)
            {
                if (accentMap.TryGetValue(c, out string replacement))
                {
                    result.Append(replacement);
                }
                else if (char.IsLetter(c) || char.IsDigit(c) || char.IsWhiteSpace(c))
                {
                    // Mantém letras, números e espaços (incluindo scripts não-latinos)
                    result.Append(c);
                }
                // Remove outros caracteres especiais
            }

            // Remove espaços extras e normaliza
            return string.Join(" ", result.ToString()
                .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
        }

        // NOVA FUNCIONALIDADE: Gera identificador único para display
        public string GetDisplayIdentifier()
        {
            // Prioridade: E-mail > Aniversário > Fallback
            if (!string.IsNullOrWhiteSpace(Email))
            {
                return Email.ToLowerInvariant();
            }

            if (!string.IsNullOrWhiteSpace(DiaMesAniversario))
            {
                return $"({DiaMesAniversario})";
            }

            return $"(ID: {Id})"; // Fallback extremo
        }

        // NOVA FUNCIONALIDADE: Formata nome para exibição nas sugestões
        public string GetDisplayName()
        {
            var identifier = GetDisplayIdentifier();

            // Se for e-mail, mostra embaixo do nome
            if (!string.IsNullOrWhiteSpace(Email) && identifier == Email.ToLowerInvariant())
            {
                return NomeCompleto; // E-mail vai numa linha separada
            }

            // Se for aniversário ou ID, mostra junto do nome
            return $"{NomeCompleto} {identifier}";
        }

        // Método para atualizar a normalização quando o nome muda
        public void UpdateNormalizedName()
        {
            NomeCompletoNormalizado = NormalizeName(NomeCompleto);
        }

        // UTILITÁRIO: Verifica se deve mostrar contador de caracteres
        public static bool ShouldShowCharacterCounter(int currentLength)
        {
            return currentLength > SHOW_COUNTER_AT;
        }

        // UTILITÁRIO: Gera texto do contador com cor apropriada
        public static (string text, bool isWarning, bool isError) GetCharacterCounterInfo(int currentLength)
        {
            string text = $"{currentLength}/{MAX_INPUT_LENGTH}";
            bool isWarning = currentLength > 190;
            bool isError = currentLength >= MAX_INPUT_LENGTH;

            return (text, isWarning, isError);
        }
    }
}