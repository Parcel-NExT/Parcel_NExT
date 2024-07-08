using System;
using System.Globalization;
using System.Windows.Data;
using Parcel.Neo.Base.Framework;

namespace Parcel.Neo.Converters
{
    public class NodeMessageTypeToBrushColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is NodeMessageType messageType)
            {
                switch (messageType)
                {
                    case NodeMessageType.Empty:
                    case NodeMessageType.Normal:
                    default:
                        return System.Windows.Media.Brushes.LightSlateGray;
                    case NodeMessageType.Error:
                        return System.Windows.Media.Brushes.Red;
                    case NodeMessageType.Documentation:
                        return System.Windows.Media.Brushes.ForestGreen;
                }
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
