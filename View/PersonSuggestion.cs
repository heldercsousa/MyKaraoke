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
        public string BirthdayDisplay => HasBirthday ? $"🎂 {DiaMesAniversario}" : string.Empty;

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