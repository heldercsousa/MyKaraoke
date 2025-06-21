using MyKaraoke.Infra.Data;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using MyKaraoke.Domain;

namespace MyKaraoke.Services
{
    public class LanguageService : ILanguageService
    {
        private readonly AppDbContext _dbContext;
        private const string PreferenceKey = "UserLanguage";

        public LanguageService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string> GetUserLanguageAsync()
        {
            // Verifica primeiro nas configurações do app
            var language = Preferences.Get(PreferenceKey, string.Empty);
            
            if (!string.IsNullOrEmpty(language))
                return language;

            // Se não encontrar, verifica no banco de dados
            var setting = await _dbContext.ConfiguracoesSistema
                .FirstOrDefaultAsync(c => c.Chave == PreferenceKey);

            return setting?.Valor ?? CultureInfo.CurrentCulture.Name;
        }

        public async Task SetUserLanguageAsync(string languageCode)
        {
            // Salva nas preferences para acesso rápido
            Preferences.Set(PreferenceKey, languageCode);
            
            // Salva no banco de dados para persistência e backup
            var setting = await _dbContext.ConfiguracoesSistema
                .FirstOrDefaultAsync(c => c.Chave == PreferenceKey);

            if (setting == null)
            {
                setting = new ConfiguracaoSistema 
                { 
                    Chave = PreferenceKey, 
                    Valor = languageCode 
                };
                _dbContext.ConfiguracoesSistema.Add(setting);
            }
            else
            {
                setting.Valor = languageCode;
            }

            await _dbContext.SaveChangesAsync();
        }

        public bool IsLanguageSelected()
        {
            // Verifica se o usuário já selecionou um idioma
            return Preferences.ContainsKey(PreferenceKey);
        }
    }
}
