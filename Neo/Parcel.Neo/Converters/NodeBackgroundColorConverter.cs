using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;

namespace Parcel.Neo.Converters
{
    public class NodeBackgroundColorConverter : IValueConverter
    {
        public object Convert(object source, Type targetType, object target, CultureInfo culture)
        {
            if (source is ProcessorNode node)
            {
                return Brushes.MintCream;
            }

            return Brushes.Red;
        }

        public object ConvertBack(object source, Type targetType, object target, CultureInfo culture)
        {
            throw new InvalidOperationException();
        }
    }
}
