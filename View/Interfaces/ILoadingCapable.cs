using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVocaList.View.Interfaces
{
    /// <summary>
    /// Interface opcional para páginas que possuem estado de loading próprio
    /// </summary>
    public interface ILoadingCapable
    {
        bool IsLoading { get; set; }
        Task ShowLoadingAsync(string message = "Carregando...");
        Task HideLoadingAsync();
    }
}
