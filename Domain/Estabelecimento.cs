// ####################################################################################################
// # Arquivo: GaleraNaFila.Domain/Estabelecimento.cs
// # Descrição: Classe de modelo de DOMÍNIO para representar um estabelecimento.
// ####################################################################################################
using System.Collections.Generic;

namespace GaleraNaFila.Domain
{
    public class Estabelecimento
    {
        public int Id { get; set; }
        public string Nome { get; set; }

        public ICollection<Evento> Eventos { get; set; } // Propriedade de navegação
    }
}