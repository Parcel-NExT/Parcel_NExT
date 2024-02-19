using System.ComponentModel;

namespace Parcel.CoreEngine.Conversion
{
    public static class StringTypeConverter
    {
        #region Methods
        public static object? ConvertType(Type parameterType, string value)
        {
            // Provide explicit conversion of known types
            if (value == null)
                throw new ArgumentException("Value is null.");
            if (string.IsNullOrEmpty(value) || value.ToLower() == "null")
                return null;

            HashSet<Type> primitiveTypes = new HashSet<Type>
            {
                typeof(bool),
                typeof(byte),
                typeof(sbyte),
                typeof(char),
                typeof(decimal),
                typeof(double),
                typeof(float),
                typeof(int),
                typeof(uint),
                typeof(long),
                typeof(ulong),
                typeof(short),
                typeof(ushort),
                typeof(string),
                typeof(DateTime),
                typeof(DateOnly)
            };

            // Handle simple types
            if (primitiveTypes.Contains(parameterType))
                return ConvertSingle(parameterType, value);
            // Handle nullable
            else if (Nullable.GetUnderlyingType(parameterType) != null)
                return ConvertSingle(parameterType.GetGenericArguments()[0], value);
            // Handle arrays
            else if (value.Contains('\n'))
                return ConvertValue(parameterType, value.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));
            // Handle complex types (e.g. Dictionaries)
            // TODO: PENDING (Consider using JSON library to help)

            throw new NotImplementedException("Unrecognized object type.");
        }
        #endregion

        #region Routines
        /// <summary>
        /// Borrowed from Pure CLI library
        /// </summary>
        private static object ConvertValue(Type type, string[] values)
        {
            if (type.IsArray)
            {
                Type elementType = type.GetElementType();
                var array = Array.CreateInstance(elementType, values.Length);
                for (int i = 0; i < values.Length; i++)
                {
                    string valueString = values[i];
                    object value = ConvertSingle(elementType, valueString);
                    array.SetValue(value, i);
                }
                return array;
            }
            else if (type != typeof(string) && !type.IsValueType)
                throw new ArgumentException($"Type must be value type.");
            else
            {
                if (values.Length == 0)
                {
                    if (type == typeof(bool))
                        return true;
                    else
                        throw new ArgumentException($"Invalid number of values for type {type.Name}");
                }
                else if (values.Length == 1)
                {
                    return ConvertSingle(type, values.Single());
                }
                else
                    throw new ArgumentException($"Values \"{string.Join(", ", values)}\" are too many for type {type.Name}.");
            }
        }
        private static object ConvertSingle(Type type, string value)
        {
            TypeConverter typeConverter = TypeDescriptor.GetConverter(type);
            object objValue = typeConverter.ConvertFromString(value);
            return objValue;
        }
        #endregion
    }
}
