using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVocaList.View.Components
{
    /// <summary>
    /// Prioridades para controle de loading
    /// </summary>
    public enum LoadingPriority
    {
        Database = 1,           // Menor prioridade - operações de banco
        Navigation = 2,         // Prioridade média - navegação entre páginas
        NavBarWait = 3,         // Maior prioridade - aguardando navbar carregar
        UserAction = 4          // Máxima prioridade - ações diretas do usuário
    }
}
