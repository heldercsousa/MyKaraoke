using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVocaList.Services
{
    public interface ILanguageService
    {
        Task<string> GetUserLanguageAsync();
        Task SetUserLanguageAsync(string languageCode);
        bool IsLanguageSelected();
    }
}
