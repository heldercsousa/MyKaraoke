using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyVocaList.View.Interfaces
{
    /// <summary>
    /// Interface composta para páginas que manipulam dados
    /// Combina funcionalidades comuns: notificação de propriedades e nome amigável
    /// </summary>
    public interface IManipulableDataPage : INotifyPropertyChanged, IFriendlyPageName
    {
        ICommand LoadDataCommand { get; }
    }
}
