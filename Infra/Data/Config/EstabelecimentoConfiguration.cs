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
    public class EstabelecimentoConfiguration : IEntityTypeConfiguration<Estabelecimento>
    {
        public void Configure(EntityTypeBuilder<Estabelecimento> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Nome)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.HasData(
                new Estabelecimento { Id = 1, Nome = "Salão Principal" },
                new Estabelecimento { Id = 2, Nome = "Área Externa" }
            );
        }
    }

}
