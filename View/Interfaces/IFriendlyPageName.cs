using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyKaraoke.View.Interfaces
{
    /// <summary>
    /// Interface para páginas que possuem nome amigável
    /// </summary>
    public interface IFriendlyPageName
    {
        string FriendlyName { get; }
    }
}
