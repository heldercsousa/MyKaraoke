using MyKaraoke.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyKaraoke.Infra.Data.Config
{
    public class EventoConfiguration : IEntityTypeConfiguration<Evento>
    {
        public void Configure(EntityTypeBuilder<Evento> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.DataEvento)
                   .IsRequired();

            builder.Property(e => e.NomeEvento)
                   .HasMaxLength(200);

            builder.Property(e => e.FilaAtiva)
                   .IsRequired()
                   .HasDefaultValue(false);

            builder.HasOne(e => e.Estabelecimento)
                   .WithMany(est => est.Eventos)
                   .HasForeignKey(e => e.EstabelecimentoId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
