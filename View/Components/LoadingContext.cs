using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVocaList.View.Components
{
    /// <summary>
    /// Tipos de contexto para decisões inteligentes
    /// </summary>
    public enum LoadingContext
    {
        DatabaseOperation,      // Operação de banco de dados
        PageNavigation,         // Navegação entre páginas
        ComponentLoading,       // Carregamento de componentes UI
        UserInteraction         // Interação direta do usuário
    }
}
