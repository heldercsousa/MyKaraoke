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

    private static bool _collationRegistered = false;
    private static readonly object _collationLock = new object();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        // Register custom SQLite collation for case and accent insensitive comparisons
        RegisterCustomCollation();
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

    /// <summary>
    /// Registers custom SQLite collation for case and accent insensitive text comparison
    /// This enables searching "João" by typing "joao", "JOAO", "João", etc.
    /// CRITICAL: Connection must be OPEN before CreateCollation() is called
    /// </summary>
    private void RegisterCustomCollation()
    {
        if (Database.IsSqlite())
        {
            lock (_collationLock)
            {
                if (!_collationRegistered)
                {
                    var connection = Database.GetDbConnection() as SqliteConnection;
                    if (connection != null)
                    {
                        // ✅ CRITICAL: Connection must be OPEN before CreateCollation
                        // SQLite requires an active connection to register custom collations
                        if (connection.State != System.Data.ConnectionState.Open)
                        {
                            connection.Open();
                        }

                        // Register NOCASE_NOACCENT collation
                        // This collation removes accents and converts to lowercase for comparison
                        connection.CreateCollation("NOCASE_NOACCENT", (x, y) =>
                        {
                            var normalizedX = NormalizeForCollation(x);
                            var normalizedY = NormalizeForCollation(y);
                            return string.Compare(normalizedX, normalizedY, StringComparison.OrdinalIgnoreCase);
                        });

                        _collationRegistered = true;
                        Console.WriteLine("✅ Custom SQLite collation 'NOCASE_NOACCENT' registered successfully");
                    }
                }
            }
        }
    }

    /// <summary>
    /// Normalizes text for collation: removes accents and converts to lowercase
    /// Supports Portuguese, Spanish, French, and other Latin-based languages
    /// </summary>
    private static string NormalizeForCollation(string text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        // Remove accents using Unicode normalization
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            // Keep only non-spacing marks (accents) removed
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString()
            .Normalize(NormalizationForm.FormC)
            .ToLowerInvariant();
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
    /// Sets database-level default collation for all string columns
    /// SQLite: Custom NOCASE_NOACCENT collation (case + accent insensitive)
    ///
    /// FUTURE: When migrating to SQL Server, use:
    /// - SQL Server: Latin1_General_CI_AI (CI = Case Insensitive, AI = Accent Insensitive)
    /// - PostgreSQL: und-u-ks-level1 (ICU collation, ignores case and accents)
    /// </summary>
    private void SetDatabaseCollation(ModelBuilder modelBuilder)
    {
        // For SQLite: Apply custom NOCASE_NOACCENT collation to all string properties
        // This is registered in RegisterCustomCollation() method

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

        Console.WriteLine("✅ Database default collation set: NOCASE_NOACCENT (case and accent insensitive)");
    }
}