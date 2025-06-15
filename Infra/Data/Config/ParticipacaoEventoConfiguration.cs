using GaleraNaFila.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaleraNaFila.Infra.Data.Config
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
