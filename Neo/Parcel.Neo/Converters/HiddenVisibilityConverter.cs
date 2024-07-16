using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Parcel.Neo.Converters
{
    public class HiddenVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isHidden)
            {
                return isHidden ? Visibility.Collapsed : Visibility.Visible;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                return visibility == Visibility.Collapsed ? true : false;
            }

            return value;
        }
    }
}
