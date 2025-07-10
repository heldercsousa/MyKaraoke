using MyKaraoke.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyKaraoke.Infra.Data.Config
{
    public class PessoaConfiguration : IEntityTypeConfiguration<Pessoa>
    {
        public void Configure(EntityTypeBuilder<Pessoa> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                   .ValueGeneratedOnAdd();

            // ABORDAGEM HÍBRIDA: Banco super seguro com TEXT(250)
            builder.Property(p => p.NomeCompleto)
                   .HasColumnType("TEXT")
                   .IsRequired()
                   .HasMaxLength(250); // Super seguro para nomes extremamente longos

            // Coluna normalizada também TEXT(250) para máxima segurança
            builder.Property(p => p.NomeCompletoNormalizado)
                   .HasColumnType("TEXT")
                   .HasMaxLength(250) // Mesma capacidade da coluna original
                   .IsRequired(false); // Pode ser null durante migração

            builder.Property(p => p.Participacoes)
                   .IsRequired()
                   .HasDefaultValue(0);

            builder.Property(p => p.Ausencias)
                   .IsRequired()
                   .HasDefaultValue(0);

            // Índice otimizado para busca rápida por nome normalizado
            builder.HasIndex(p => p.NomeCompletoNormalizado)
                   .HasDatabaseName("IX_Pessoas_NomeCompletoNormalizado");
        }
    }
}