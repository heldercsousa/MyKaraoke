using MyKaraoke.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyKaraoke.Infra.Data.Config
{
    public class ParticipacaoEventoConfiguration : IEntityTypeConfiguration<ParticipacaoEvento>
    {
        public void Configure(EntityTypeBuilder<ParticipacaoEvento> builder)
        {
            builder.HasKey(pe => pe.Id);

            builder.Property(pe => pe.Timestamp)
                   .IsRequired();

            builder.Property(pe => pe.Status)
                   .IsRequired();

            builder.HasOne(pe => pe.Pessoa)
                   .WithMany()
                   .HasForeignKey(pe => pe.PessoaId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(pe => pe.Evento)
                   .WithMany(e => e.Participacoes)
                   .HasForeignKey(pe => pe.EventoId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
