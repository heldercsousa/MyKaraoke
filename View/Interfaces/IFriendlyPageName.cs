using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVocaList.View.Interfaces
{
    /// <summary>
    /// Interface for pages that have a friendly title and is auto shown by the pagebehavior.
    /// </summary>
    public interface IFriendlyPageName
    {
        string FriendlyName { get; }
    }
}
