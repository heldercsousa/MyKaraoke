using System.ComponentModel;
using System.Globalization;
using MyVocaList.View.Components;

namespace MyVocaList.View.Converters
{
    /// <summary>
    /// Type converter to allow XAML to parse StatefulIconSize from strings like "Large", "Medium", "Small"
    /// </summary>
    public class StatefulIconSizeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string stringValue)
            {
                var trimmed = stringValue.Trim();

                // Case-insensitive comparison
                if (trimmed.Equals("Small", StringComparison.OrdinalIgnoreCase))
                    return StatefulIconSize.Small;

                if (trimmed.Equals("Medium", StringComparison.OrdinalIgnoreCase))
                    return StatefulIconSize.Medium;

                if (trimmed.Equals("Large", StringComparison.OrdinalIgnoreCase))
                    return StatefulIconSize.Large;

                throw new ArgumentException($"Cannot convert '{stringValue}' to StatefulIconSize. Valid values: Small, Medium, Large");
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is StatefulIconSize iconSize)
            {
                return iconSize.Name;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
