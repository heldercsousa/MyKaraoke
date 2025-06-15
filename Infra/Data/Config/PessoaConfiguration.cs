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
