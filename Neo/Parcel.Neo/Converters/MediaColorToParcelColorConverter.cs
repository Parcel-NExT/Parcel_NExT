using Parcel.Types;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Parcel.Neo.Converters
{
    public class MediaColorToParcelColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Color parcelColor)
                return System.Windows.Media.Color.FromArgb(parcelColor.Alpha, parcelColor.Red, parcelColor.Green, parcelColor.Blue);

            throw new ArgumentException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is System.Windows.Media.Color mediaColor)
                return Color.ParseARGB(mediaColor.ToString());

            throw new ArgumentException();
        }
    }
}
