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
            else if (value == null)
                return System.Windows.Media.Color.FromArgb(0, 0, 0, 0);

            throw new ArgumentException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is System.Windows.Media.Color mediaColor)
            {
                string color = mediaColor.ToString();
                return string.IsNullOrEmpty(color.Replace("#", string.Empty).Replace("0", string.Empty)) ? null : Color.ParseARGB(color); // If it's fully transparent color, we just treat it as "null"
            }

            throw new ArgumentException();
        }
    }
}
