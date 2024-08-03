using Parcel.Neo.Base.DataTypes;
using Parcel.Types;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Parcel.Neo.Converters
{
    public class SizeToVector2DConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Vector2D vector2D)
                return new Size(vector2D.X, vector2D.Y);
            else if (value == null)
                return new Size();

            throw new ArgumentException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Size size)
                return new Vector2D(size.Width, size.Height);

            throw new ArgumentException();
        }
    }
}
