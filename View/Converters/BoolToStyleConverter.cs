using System.Globalization;
using Microsoft.Maui.Controls;

namespace MyKaraoke.View.Converters
{
    public class BoolToStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isSelected && parameter is string styleNames)
            {
                try
                {
                    var styles = styleNames.Split(',');
                    if (styles.Length >= 2)
                    {
                        var styleName = isSelected ? styles[0].Trim() : styles[1].Trim();
                        
                        if (Application.Current?.Resources.TryGetValue(styleName, out var style) == true)
                        {
                            System.Diagnostics.Debug.WriteLine($"Style '{styleName}' found and applied");
                            return style;
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"Style '{styleName}' not found in resources");
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Invalid style parameter format: '{styleNames}'. Expected 'trueStyle,falseStyle'");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error in BoolToStyleConverter: {ex.Message}");
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
            try
            {
                if (value is bool isSelected)
                {
                    var color = isSelected ? Colors.Transparent : Color.FromArgb("#6c4794");
                    System.Diagnostics.Debug.WriteLine($"BoolToBorderColorConverter: returning {(isSelected ? "Transparent" : "#6c4794")}");
                    return color;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in BoolToBorderColorConverter: {ex.Message}");
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
            try
            {
                if (value is bool isSelected)
                {
                    var color = isSelected ? Color.FromArgb("#d5528a") : Color.FromArgb("#4c426f");
                    System.Diagnostics.Debug.WriteLine($"BoolToColorConverter: returning {(isSelected ? "#d5528a" : "#4c426f")}");
                    return color;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in BoolToColorConverter: {ex.Message}");
            }
            
            return Color.FromArgb("#4c426f");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}