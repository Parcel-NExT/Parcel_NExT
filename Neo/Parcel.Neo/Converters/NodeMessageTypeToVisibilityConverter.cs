using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Nodify;
using Parcel.Neo.Base.Framework;
using Parcel.Neo.Base.Framework.ViewModels;

namespace Parcel.Neo.Converters
{
    public class NodeMessageTypeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is NodeMessageType messageType)
            {
                return messageType != NodeMessageType.Empty ? Visibility.Visible : Visibility.Collapsed;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                return visibility == Visibility.Collapsed ? NodeMessageType.Empty : NodeMessageType.Normal;
            }

            return value;
        }
    }
}
