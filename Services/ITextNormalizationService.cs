using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace MyKaraoke.Services
{
    /// <summary>
    /// Interface para normalização de texto multilíngue
    /// </summary>
    public interface ITextNormalizationService
    {
        /// <summary>
        /// Normaliza nome removendo acentos e caracteres especiais
        /// </summary>
        string NormalizeName(string name);

        /// <summary>
        /// Detecta se o texto contém caracteres árabes (RTL)
        /// </summary>
        bool ContainsArabicText(string text);

        /// <summary>
        /// Detecta se o texto contém caracteres asiáticos (CJK)
        /// </summary>
        bool ContainsAsianText(string text);

        /// <summary>
        /// Remove caracteres especiais de entrada mantendo letras, números e espaços
        /// </summary>
        string SanitizeInput(string input);

        /// <summary>
        /// Normaliza entrada de pesquisa para busca otimizada
        /// </summary>
        string NormalizeSearchTerm(string searchTerm);
    }
}