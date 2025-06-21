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

            builder.Property(p => p.NomeCompleto)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(p => p.Participacoes)
                   .IsRequired()
                   .HasDefaultValue(0);

            builder.Property(p => p.Ausencias)
                   .IsRequired()
                   .HasDefaultValue(0);
        }
    }
}
