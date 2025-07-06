using MyKaraoke.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyKaraoke.Infra.Data.Config
{
    public class ConfiguracaoSistemaConfiguration : IEntityTypeConfiguration<ConfiguracaoSistema>
    {
        public void Configure(EntityTypeBuilder<ConfiguracaoSistema> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            builder.Property(p => p.Chave)
                .HasColumnType("varchar(50)")
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.Valor)
                .HasColumnType("varchar(200)")
                .IsRequired()
                .HasMaxLength(200);
        }
    }
}
