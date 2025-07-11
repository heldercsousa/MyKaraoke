// Adicione estas propriedades à classe PersonSuggestion existente:

namespace MyKaraoke.View
{
    /// <summary>
    /// Model para sugestões de pessoas na UI com propriedades para binding
    /// </summary>
    public class PersonSuggestion
    {
        public int Id { get; set; }
        public string NomeCompleto { get; set; } = string.Empty;
        public int Participacoes { get; set; }
        public int Ausencias { get; set; }
        public string Email { get; set; } = string.Empty;
        public string DiaMesAniversario { get; set; } = string.Empty;

        // 🔥 PROPRIEDADE PRINCIPAL: Para identificar opção "Nova pessoa"
        public bool IsNewPersonOption { get; set; } = false;

        // 🔥 PROPRIEDADES CALCULADAS: Para binding direto no XAML (SEM conversores)
        public bool IsRealPerson => !IsNewPersonOption;
        public bool ShowParticipationStats => !IsNewPersonOption;
        public bool ShowPersonDetails => !IsNewPersonOption;

        // Propriedades calculadas para UI existentes
        public string ParticipationSummary => $"{Participacoes}P {Ausencias}A";
        public bool HasEmail => !string.IsNullOrWhiteSpace(Email);
        public bool HasBirthday => !string.IsNullOrWhiteSpace(DiaMesAniversario);

        // 🔥 PROPRIEDADES OTIMIZADAS UX MOBILE 2025

        /// <summary>
        /// Exibe data ou placeholder "--/--" para manter consistência visual
        /// </summary>
        public string BirthdayDisplayOrPlaceholder => HasBirthday ? DiaMesAniversario : "--/--";

        /// <summary>
        /// E-mail truncado para layout compacto (máx ~25 chars com espaço otimizado)
        /// </summary>
        public string EmailDisplayTruncated
        {
            get
            {
                if (IsNewPersonOption || string.IsNullOrWhiteSpace(Email))
                    return "---";

                return Email.Length > 25 ? Email.Substring(0, 22) + "..." : Email;
            }
        }

        /// <summary>
        /// 🆕 NOVA PROPRIEDADE: Retorna o email se disponível, caso contrário string vazia
        /// Permite sempre mostrar o ícone de e-mail com campo vazio quando não há e-mail
        /// </summary>
        public string EmailDisplay
        {
            get
            {
                if (IsNewPersonOption)
                    return string.Empty;

                return !string.IsNullOrWhiteSpace(Email) ? Email : string.Empty;
            }
        }

        // Factory method para criar a partir de Pessoa
        public static PersonSuggestion FromPessoa(Domain.Pessoa pessoa)
        {
            return new PersonSuggestion
            {
                Id = pessoa.Id,
                NomeCompleto = pessoa.NomeCompleto,
                Participacoes = pessoa.Participacoes,
                Ausencias = pessoa.Ausencias,
                Email = pessoa.Email ?? string.Empty,
                DiaMesAniversario = pessoa.DiaMesAniversario ?? string.Empty,
                IsNewPersonOption = false // 🔥 SEMPRE FALSE para pessoas reais
            };
        }
    }
}