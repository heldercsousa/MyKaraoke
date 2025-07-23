using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyKaraoke.View.Components
{
    public class SpecialNavButtonEventArgs : EventArgs
    {
        public string ButtonText { get; }
        public string CenterContent { get; }
        public string IconSource { get; }
        public object Parameter { get; }

        public SpecialNavButtonEventArgs(string buttonText, string centerContent, string iconSource, object parameter)
        {
            ButtonText = buttonText;
            CenterContent = centerContent;
            IconSource = iconSource;
            Parameter = parameter;
        }
    }
}
