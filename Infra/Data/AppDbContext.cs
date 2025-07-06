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

    // Este construtor é o que será usado pela Injeção de Dependência
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // CONSTRUTOR SEM OPÇÕES PARA MIGRAÇÕES:
    // EF Core Migrations precisa de um construtor sem parâmetros para ser invocado
    // a partir das ferramentas de linha de comando (`Add-Migration`, `Update-Database`).
    // Ele vai usar o OnConfiguring que será chamado implicitamente pelas ferramentas.
    // No entanto, para MAUI, a configuração de OnConfiguring (com FileSystem)
    // não pode estar aqui. Uma alternativa é criar um IDbContextFactory ou
    // passar a string de conexão via variável de ambiente para as ferramentas.
    // Por enquanto, para evitar o erro, podemos ter um construtor vazio,
    // mas ele não será usado em tempo de execução do app MAUI.
    public AppDbContext() { }

    // No contexto do EF Core, ao configurar o SQLite
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=mykaraoke.db", 
            options => options.CommandTimeout(60)
            .MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new PessoaConfiguration());
        modelBuilder.ApplyConfiguration(new EstabelecimentoConfiguration());
        modelBuilder.ApplyConfiguration(new EventoConfiguration());
        modelBuilder.ApplyConfiguration(new ParticipacaoEventoConfiguration());
        modelBuilder.ApplyConfiguration(new ConfiguracaoSistemaConfiguration());

        // Configure collation para colunas específicas
        modelBuilder.Entity<Pessoa>()
            .Property(e => e.NomeCompleto)
            .UseCollation("NOCASE"); // Para SQLite, Unicode é suportado por padrão

        modelBuilder.Entity<Estabelecimento>()
            .Property(e => e.Nome)
            .UseCollation("NOCASE"); // Para SQLite, Unicode é suportado por padrão

        modelBuilder.Entity<Evento>()
            .Property(e => e.NomeEvento)
            .UseCollation("NOCASE"); // Para SQLite, Unicode é suportado por padrão

        //// Exemplo para especificar precisão em campos decimais, se necessário
        //modelBuilder.Entity<AlgumaEntidade>()
        //    .Property(e => e.ValorDecimal)
        //    .HasPrecision(18, 2);  // 18 dígitos no total, 2 casas decimais
    }
}