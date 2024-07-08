using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Parcel.Neo.Base.DataTypes;

namespace Parcel.Neo.Converters
{
    public class NodeLocationConverter : IValueConverter
    {
        public object Convert(object source, Type targetType, object target, CultureInfo culture)
        {
            Vector2D loc = (Vector2D)source;
            return new Point(loc.X, loc.Y);
        }

        public object ConvertBack(object source, Type targetType, object target, CultureInfo culture)
        {
            Point loc = (Point)source;
            return new Vector2D(loc.X, loc.Y);
        }
    }
}
