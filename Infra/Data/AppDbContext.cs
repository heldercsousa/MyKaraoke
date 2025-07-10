using MyKaraoke.Domain;
using MyKaraoke.Infra.Data.Config;
using Microsoft.EntityFrameworkCore;

namespace MyKaraoke.Infra.Data;

public class AppDbContext : DbContext
{
    public DbSet<Pessoa> Pessoas { get; set; }
    public DbSet<Estabelecimento> Estabelecimentos { get; set; }
    public DbSet<Evento> Eventos { get; set; }
    public DbSet<ParticipacaoEvento> ParticipacoesEventos { get; set; }
    public DbSet<ConfiguracaoSistema> ConfiguracoesSistema { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Construtor vazio apenas para migrações
    public AppDbContext() { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Apenas para design-time/migrações - usar caminho temporário
            var tempPath = Path.Combine(Path.GetTempPath(), "mykaraoke_design.db");
            optionsBuilder.UseSqlite($"Data Source={tempPath}");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new PessoaConfiguration());
        modelBuilder.ApplyConfiguration(new EstabelecimentoConfiguration());
        modelBuilder.ApplyConfiguration(new EventoConfiguration());
        modelBuilder.ApplyConfiguration(new ParticipacaoEventoConfiguration());
        modelBuilder.ApplyConfiguration(new ConfiguracaoSistemaConfiguration());

        modelBuilder.Entity<Pessoa>()
            .Property(e => e.NomeCompleto)
            .UseCollation("NOCASE");

        // NOVA CONFIGURAÇÃO: Collation para coluna normalizada
        modelBuilder.Entity<Pessoa>()
            .Property(e => e.NomeCompletoNormalizado)
            .UseCollation("NOCASE");

        modelBuilder.Entity<Estabelecimento>()
            .Property(e => e.Nome)
            .UseCollation("NOCASE");

        modelBuilder.Entity<Evento>()
            .Property(e => e.NomeEvento)
            .UseCollation("NOCASE");
    }

    // NOVA FUNCIONALIDADE: Auto-atualização da coluna normalizada
    public override int SaveChanges()
    {
        UpdateNormalizedNames();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateNormalizedNames();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateNormalizedNames()
    {
        var changedPessoas = ChangeTracker.Entries<Pessoa>()
            .Where(e => e.State == EntityState.Added ||
                       (e.State == EntityState.Modified && e.Property(p => p.NomeCompleto).IsModified))
            .Select(e => e.Entity);

        foreach (var pessoa in changedPessoas)
        {
            // Atualiza automaticamente a versão normalizada
            pessoa.NomeCompletoNormalizado = Pessoa.NormalizeName(pessoa.NomeCompleto);

            System.Diagnostics.Debug.WriteLine($"Normalizando: '{pessoa.NomeCompleto}' → '{pessoa.NomeCompletoNormalizado}'");
        }
    }
}