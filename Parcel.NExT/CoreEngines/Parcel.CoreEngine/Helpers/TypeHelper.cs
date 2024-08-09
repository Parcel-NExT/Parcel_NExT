using Parcel.Infrastructure;
using System.ComponentModel;

namespace Parcel.CoreEngine.Helpers
{
    public static class TypeHelper
    {
        #region Type Calssification
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
        #endregion


        #region Type Conversion
        public static bool CanConvert(object instance, Type to)
            => CanConvert(instance.GetType(), to);
        public static bool CanConvert(Type from, Type to)
        {
            if (TypeDescriptor.GetConverter(from).CanConvertTo(to)) // Requires IConvertible; This doesn't cover everything
                return true;
            else if (from == typeof(string) && (IsNumericalType(to) || typeof(IParcelSerializable).IsAssignableFrom(to)))
                return true;
            else if (to == typeof(string))
                return true;
            else if (to.GetConstructor([from]) != null) // Check constructor
                return true;
            else if (to.GetMethod("op_Implicit", [from]) != null || from.GetMethod("op_Implicit", [from])?.ReturnType == to) // Check implicitor converter
                return true;
            return false;
        }
        public static bool TryConvert(object instance, Type to, out object? result)
        {
            Type from = instance.GetType();
            if (TypeDescriptor.GetConverter(from).CanConvertTo(to)) // This doesn't cover everything
            {
                result = Convert.ChangeType(instance, to);
                return true;
            }
            // TODO: Handling string to IParcelSerializable conversion
            // TODO: Handle IParcelSerializable to string conversion
            else if (from == typeof(string) && IsNumericalType(to))
            {
                TypeConverter typeConverter = TypeDescriptor.GetConverter(to);
                result = typeConverter.ConvertFromString((string)instance);
                return true;
            }
            else
            {
                // Check constructor
                System.Reflection.ConstructorInfo? constructor = to.GetConstructor([from]);
                if (constructor != null) 
                {
                    result = constructor?.Invoke([instance]);
                    return true;
                }

                // Check to implicit constructor
                System.Reflection.MethodInfo? toImplicit = to.GetMethod("op_Implicit", [from]);
                System.Reflection.MethodInfo? fromImplicit = from.GetMethod("op_Implicit", [from]);
                if (toImplicit != null)
                {
                    result = toImplicit.Invoke(null, [instance]);
                    return true;
                }
                if (fromImplicit != null && fromImplicit.ReturnType == to)
                {
                    result = fromImplicit.Invoke(null, [instance]);
                    return true;
                }
            }

            // No conversion available
            result = null;
            return false;
        }
        #endregion
    }
}
