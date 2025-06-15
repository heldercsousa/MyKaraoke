using GaleraNaFila.Domain;
using GaleraNaFila.Infra.Data.Config;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaleraNaFila.Infra.Data;
public class AppDbContext : DbContext
{
    public DbSet<Pessoa> Pessoas { get; set; }
    public DbSet<Estabelecimento> Estabelecimentos { get; set; }
    public DbSet<Evento> Eventos { get; set; }
    public DbSet<ParticipacaoEvento> ParticipacoesEventos { get; set; }

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


    // --- IMPORTANTE: REMOVA O MÉTODO OnConfiguring DAQUI ---
    // A configuração da string de conexão será feita no MauiProgram.cs.
    // Se você tiver este método aqui, remova-o:
    /*
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // REMOVA ESTE BLOCO:
        if (!optionsBuilder.IsConfigured)
        {
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "FilaDeC.db"); // Isso causava o erro
            optionsBuilder.UseSqlite($"Filename={dbPath}");
        }
    }
    */

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new PessoaConfiguration());
        modelBuilder.ApplyConfiguration(new EstabelecimentoConfiguration());
        modelBuilder.ApplyConfiguration(new EventoConfiguration());
        modelBuilder.ApplyConfiguration(new ParticipacaoEventoConfiguration());
    }
}