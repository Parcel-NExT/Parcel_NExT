using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Parcel.Neo.Converters
{
    public class BooleanToMenuItemCheckedLabelConverter : MarkupExtension, IValueConverter
    {
        public bool Negate { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string stringValue = value?.ToString();
            if (bool.TryParse(stringValue, out var b))
            {
                return (Negate ? !b : b) ? "✔" : "✘";
            }
            else if (double.TryParse(stringValue, out var d))
            {
                return (Negate ? !(d > 0) : (d > 0)) ? "✔" : "✘";
            }

            bool result = value != null;
            return (Negate ? !result : result) ? "✔" : "✘";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => value is string v && v == "✔";

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
