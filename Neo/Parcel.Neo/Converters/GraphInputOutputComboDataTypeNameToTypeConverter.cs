using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace Parcel.Neo.Converters
{
    public class GraphInputOutputComboDataTypeNameToTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == typeof(bool))
                return "Boolean";
            else if (value == typeof(double))
                return "Number";
            else if (value == typeof(string))
                return "String";
            else if (value == typeof(DateTime))
                return "DateTime";
            else if (value == typeof(Parcel.Types.DataGrid))
                return "DataGrid";
            else if (value == typeof(double[])) // TODO: Consolidate with Parcel.Math.Types.Vector type
                return "Vector";
            else
                throw new ArgumentException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string item = (value as ComboBoxItem).Content as string;
            return item switch
            {
                "Boolean" => typeof(bool),
                "Number" => typeof(double),
                "String" => typeof(string),
                "DateTime" => typeof(DateTime),
                "DataGrid" => typeof(Parcel.Types.DataGrid),
                "Vector" => typeof(double[]), // TODO: Consolidate with Parcel.Math.Types.Vector type
                _ => throw new ArgumentException()
            };
        }
    }
}
