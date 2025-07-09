using Microsoft.EntityFrameworkCore;
using MyKaraoke.Infra.Data;
using Microsoft.Extensions.Logging;

namespace MyKaraoke.Services
{
    public class DatabaseService : IDatabaseService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DatabaseService> _logger;

        public DatabaseService(AppDbContext context, ILogger<DatabaseService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task InitializeDatabaseAsync()
        {
            try
            {
                var dbPath = Path.Combine(FileSystem.AppDataDirectory, "MyKaraoke.db");
                var directory = Path.GetDirectoryName(dbPath);

                // Garantir que o diretório existe
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                    _logger.LogInformation($"Diretório criado: {directory}");
                }

                // Verificar permissões de escrita
                await VerifyWritePermissionsAsync(directory);

                // **APLICAR MIGRAÇÕES - RESPONSABILIDADE DO DATABASE SERVICE**
                _logger.LogInformation("Iniciando aplicação de migrações...");
                await _context.Database.MigrateAsync();
                _logger.LogInformation("Migrações aplicadas com sucesso");

                _logger.LogInformation($"Banco de dados inicializado com sucesso em: {dbPath}");

                // Testar conexão
                await TestConnectionAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao inicializar banco de dados");
                throw;
            }
        }

        private async Task VerifyWritePermissionsAsync(string directory)
        {
            var testFile = Path.Combine(directory, "test.txt");
            try
            {
                await File.WriteAllTextAsync(testFile, "test");
                File.Delete(testFile);
                _logger.LogInformation("Permissões de escrita verificadas");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro de permissão de escrita no diretório");
                throw;
            }
        }

        private async Task TestConnectionAsync()
        {
            try
            {
                await _context.Database.CanConnectAsync();
                _logger.LogInformation("Conexão com banco testada com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao testar conexão com banco");
                throw;
            }
        }

        public async Task<bool> IsDatabaseAvailableAsync()
        {
            try
            {
                return await _context.Database.CanConnectAsync();
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> HasPendingMigrationsAsync()
        {
            try
            {
                var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();
                return pendingMigrations.Any();
            }
            catch
            {
                return false;
            }
        }
    }
}