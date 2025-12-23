using MyVocaList.Domain;
using MyVocaList.Infra.Data.Config;
using MyVocaList.Infra.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using System.Globalization;
using System.Text;

namespace MyVocaList.Infra.Data;

public class AppDbContext : DbContext
{
    public DbSet<Pessoa> Pessoas { get; set; }
    public DbSet<Estabelecimento> Estabelecimentos { get; set; }
    public DbSet<Evento> Eventos { get; set; }
    public DbSet<ParticipacaoEvento> ParticipacoesEventos { get; set; }
    public DbSet<ConfiguracaoSistema> ConfiguracoesSistema { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        // ✅ COLLATION REGISTRATION: Now handled automatically by CollationInterceptor
        // The interceptor registers NOCASE_NOACCENT on every connection (including migrations)
        // No need for manual registration here anymore
    }

    // Empty constructor for migrations only
    public AppDbContext() { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Design-time/migrations only - use temporary path
            var tempPath = Path.Combine(Path.GetTempPath(), "myvocalist_design.db");

            optionsBuilder.UseSqlite($"Data Source={tempPath}");
        }
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ✅ SET DATABASE-LEVEL DEFAULT COLLATION
        // This applies case and accent insensitive collation to ALL string columns automatically
        // Supports: João = joao = JOAO = jOãO (any case/accent combination)
        SetDatabaseCollation(modelBuilder);

        // Apply entity configurations
        modelBuilder.ApplyConfiguration(new PessoaConfiguration());
        modelBuilder.ApplyConfiguration(new EstabelecimentoConfiguration());
        modelBuilder.ApplyConfiguration(new EventoConfiguration());
        modelBuilder.ApplyConfiguration(new ParticipacaoEventoConfiguration());
        modelBuilder.ApplyConfiguration(new ConfiguracaoSistemaConfiguration());
    }

    /// <summary>
    /// Sets database-level default collation for all string columns.
    /// SQLite: Custom NOCASE_NOACCENT collation (case + accent insensitive)
    ///
    /// REGISTRATION: The collation itself is registered by CollationInterceptor on every connection.
    /// This method only tells EF Core to USE that collation for string comparisons.
    ///
    /// FUTURE: When migrating to SQL Server, use:
    /// - SQL Server: Latin1_General_CI_AI (CI = Case Insensitive, AI = Accent Insensitive)
    /// - PostgreSQL: und-u-ks-level1 (ICU collation, ignores case and accents)
    /// </summary>
    private void SetDatabaseCollation(ModelBuilder modelBuilder)
    {
        // For SQLite: Apply custom NOCASE_NOACCENT collation to all string properties
        // The collation is registered automatically by CollationInterceptor on every connection

        // Apply to all string properties in all entities
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(string))
                {
                    property.SetCollation("NOCASE_NOACCENT");
                }
            }
        }

        Console.WriteLine("✅ Database collation configured: NOCASE_NOACCENT (registered by CollationInterceptor)");
    }

    /// <summary>
    /// Debug method to get the actual database file path
    /// </summary>
    public string GetDatabasePath()
    {
        var connection = Database.GetDbConnection();
        return connection.DataSource;
    }

    /// <summary>
    /// Debug method to log database information
    /// </summary>
    public void LogDatabaseInfo()
    {
        var connection = Database.GetDbConnection();
        Console.WriteLine($"🗃️ Database Path: {connection.DataSource}");
        Console.WriteLine($"🗃️ Connection State: {connection.State}");
        Console.WriteLine($"🗃️ Database Type: {Database.ProviderName}");
    }
}