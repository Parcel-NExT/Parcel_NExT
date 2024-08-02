using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using Parcel.Neo.Base.Framework;

namespace Parcel.Neo.Converters
{
    public class NodeMessageTypeToVisibilityConverter : MarkupExtension, IValueConverter
    {
        public bool Negate { get; set; } = false;
        public bool IncludeMinorMessage { get; set; } = true;
        public bool MinorMessageOnly { get; set; } = false;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is NodeMessageType messageType)
            {
                bool visible = messageType != NodeMessageType.Empty ? true : false;
                if (IncludeMinorMessage == false && messageType == NodeMessageType.RuntimeStats)
                    visible = false;
                if (MinorMessageOnly && messageType != NodeMessageType.RuntimeStats)
                    visible = false;

                if (Negate) visible = !visible;
                return visible ? Visibility.Visible : Visibility.Collapsed;
            }

            throw new ArgumentException($"Invalid value: {value}");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
