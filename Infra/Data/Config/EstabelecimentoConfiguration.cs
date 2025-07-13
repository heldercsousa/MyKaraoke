using MyKaraoke.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyKaraoke.Infra.Data.Config
{
    public class EstabelecimentoConfiguration : IEntityTypeConfiguration<Estabelecimento>
    {
        public void Configure(EntityTypeBuilder<Estabelecimento> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(e => e.Nome)
                   .HasColumnType("TEXT")
                   .IsRequired()
                   .HasMaxLength(30); // Suporte multilíngue: EN, PT, ES, FR, JA, KO
        }
    }
}