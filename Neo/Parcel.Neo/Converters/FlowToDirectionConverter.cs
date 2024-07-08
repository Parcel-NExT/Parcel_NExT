using System;
using System.Globalization;
using System.Windows.Data;
using Nodify;
using Parcel.Neo.Base.DataTypes;

namespace Parcel.Neo.Converters
{
    public class FlowToDirectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ConnectorFlowType flow)
            {
                return flow == ConnectorFlowType.Output ? ConnectionDirection.Forward : ConnectionDirection.Backward;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ConnectionDirection dir)
            {
                return dir == ConnectionDirection.Forward ? ConnectorFlowType.Output : ConnectorFlowType.Input;
            }

            return value;
        }
    }
}
