using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyKaraoke.View.Components
{
    public class NavButtonEventArgs : EventArgs
    {
        public string ButtonText { get; }
        public string IconSource { get; }
        public object Parameter { get; }

        public NavButtonEventArgs(string buttonText, string iconSource, object parameter)
        {
            ButtonText = buttonText;
            IconSource = iconSource;
            Parameter = parameter;
        }
    }

}
