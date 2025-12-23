using System.Globalization;
using Microsoft.Maui.Controls;
using Serilog;

namespace MyVocaList.View.Converters
{
    public class BoolToStyleConverter : IValueConverter
    {
        #region private Props
        private static readonly ILogger Logger = Log.ForContext<BoolToStyleConverter>();
        #endregion

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isSelected && parameter is string styleNames)
            {
                
                var styles = styleNames.Split(',');
                if (styles.Length >= 2)
                {
                    var styleName = isSelected ? styles[0].Trim() : styles[1].Trim();
                        
                    if (Application.Current?.Resources.TryGetValue(styleName, out var style) == true)
                    {
                        return style;
                    }
                }
                
            }
            
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToBorderColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isSelected)
            {
                var color = isSelected ? Colors.Transparent : Color.FromArgb("#6c4794");
                return color;
            }
            return Color.FromArgb("#6c4794");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isSelected)
            {
                var color = isSelected ? Color.FromArgb("#d5528a") : Color.FromArgb("#4c426f");
                Console.WriteLine($"BoolToColorConverter: returning {(isSelected ? "#d5528a" : "#4c426f")}");
                return color;
            }
            
            return Color.FromArgb("#4c426f");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class IntToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int count)
                return count > 0; // Returns TRUE if list has items
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}