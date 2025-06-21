namespace MyKaraoke.Domain
{
    public class Pessoa
    {
        public int Id { get; set; } // Chave primária para o BD
        public string NomeCompleto { get; set; }
        public int Participacoes {  get; set; } = 0; // Contador de participações   
        public int Ausencias { get; set; } = 0; // Contador de ausências        

        public Pessoa(string nomeCompleto)
        {
            NomeCompleto = nomeCompleto;
        }

        public Pessoa() { } // Construtor sem argumentos para EF Core

        // Método de validação movido para cá, pois é lógica de domínio
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
    }
}