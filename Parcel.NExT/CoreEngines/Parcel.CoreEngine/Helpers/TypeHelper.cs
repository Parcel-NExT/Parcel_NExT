namespace Parcel.CoreEngine.Helpers
{
    public static class TypeHelper
    {
        /// <summary>
        /// Check whether a type can represent number, which is parcel defaults to double
        /// </summary>
        public static bool IsNumericalType(Type inputType)
        {
            // Remark: Notice TypeDesciptor/TypeConverter cannot be used per https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.typeconverter.canconvertfrom?view=net-8.0 they always return false
            // Aka. TypeDescriptor.GetConverter(output.DataType).CanConvertFrom(typeof(double)) won't work
            // Remark: It seems "CanConvertTo" works

            Type checkType = inputType;
            if (Nullable.GetUnderlyingType(inputType) != null)
                // It's nullable
                checkType = Nullable.GetUnderlyingType(inputType)!;

            switch (Type.GetTypeCode(checkType))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }
    }
}
