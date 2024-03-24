using Humanizer;
using Parcel.CoreEngine.Document;
using Parcel.CoreEngine.Primitives;
using Parcel.CoreEngine.SemanticTypes;
using System.Collections;
using System.ComponentModel;
using System.Text;

namespace Parcel.CoreEngine.Conversion
{
    public static class StringTypeConverter
    {
        #region Methods
        public static string SerializeResult(object result)
        {
            if (result == null)
                return "ERROR: Result is null.";

            var primitiveTypes = new HashSet<Type>
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
                typeof(string)
            };

            // Explicitly handle and serialize everything in pre-defined format: this is the protocol/contract between Tranquility and clients that interface with it
            var resultType = result.GetType();
            // Simply serialize primitives
            if (primitiveTypes.Contains(resultType))
                return result.ToString()!;
            // Serialize collections
            else if (resultType.IsGenericType && resultType.IsAssignableTo(typeof(IEnumerable))
                && resultType.GetGenericArguments().Length == 1 && primitiveTypes.Contains(resultType.GenericTypeArguments.First()))
            {
                var elements = (IEnumerable<object>)result;
                return string.Join("\n", elements.Select(r => r.ToString()));
            }
            else if (resultType.IsArray && primitiveTypes.Contains(resultType.GetElementType()!))
            {
                List<object> elements = [];
                foreach (var e in (Array)result)
                    elements.Add(e);

                return string.Join("\n", elements.Select(r => r.ToString()));
            }
            // Serialize plain string dictionaries
            else if (resultType == typeof(Dictionary<string, string[]>))
                return SerializeFlatStringArrayDictionaryStructure((Dictionary<string, string[]>)result);
            else if (resultType == typeof(Dictionary<string, string>))
                return SerializeFlatStringDictionaryStructure((Dictionary<string, string>)result);
            else if (resultType == typeof(Dictionary<string, SimplexString>))
                return SerializeFlatSimplexStringDictionaryStructure((Dictionary<string, SimplexString>)result);
            // Serialize other basic dictionares
            else if (resultType == typeof(Dictionary<string, byte>))
                return SerializeFlatStringDictionaryStructure(((Dictionary<string, byte>)result).ToDictionary(r => r.Key, r => r.Value.ToString()));
            else if (resultType == typeof(Dictionary<string, int>))
                return SerializeFlatStringDictionaryStructure(((Dictionary<string, int>)result).ToDictionary(r => r.Key, r => r.Value.ToString()));
            else if (resultType == typeof(Dictionary<string, long>))
                return SerializeFlatStringDictionaryStructure(((Dictionary<string, long>)result).ToDictionary(r => r.Key, r => r.Value.ToString()));
            else if (resultType == typeof(Dictionary<string, float>))
                return SerializeFlatStringDictionaryStructure(((Dictionary<string, float>)result).ToDictionary(r => r.Key, r => r.Value.ToString()));
            else if (resultType == typeof(Dictionary<string, double>))
                return SerializeFlatStringDictionaryStructure(((Dictionary<string, double>)result).ToDictionary(r => r.Key, r => r.Value.ToString()));

            // Serialize serializable Parcel-specific types
            else if (resultType == typeof(DataGrid))
                return SerializaDataGrid((DataGrid)result);
            // TODO: Use standard serialization procedure (as shared with TextSerializer); Deal with non-serializable payloads (which are useful at runtime but cannot be transferred across internet and cannot be saved to document)
            // TODO: Serialize Payload, including MetaInstructions
            else if (resultType == typeof(ParcelPayload))
                return SerializePayload((ParcelPayload)result);
            else
                throw new NotImplementedException("Unrecognized object type.");
        }
        public static object? ConvertType(Type parameterType, object value)
        {
            if (parameterType == value.GetType())
                return value;
            else if (value is string stringValue)
                return ConvertType(parameterType, stringValue);
            else throw new ArgumentException($"Unexpected non-string parameter value: {value}");
        }
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
            // Handle semantic types
            else if (parameterType == typeof(Uri))
                return new Uri(value);
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
                Type elementType = type.GetElementType()!;
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
            object? objValue = typeConverter.ConvertFromString(value);
            if (objValue == null)
                throw new ApplicationException($"Cannot convert {value} to type {type.Name}.");
            return objValue!;
        }
        #endregion

        #region Type Specific Routines
        private static string SerializeFlatStringArrayDictionaryStructure(Dictionary<string, string[]> dictionary)
        {
            // Remark: We intentionally don't use JSON libraries for such simple structure to guarantee predictable behaviors, keep code clean and dependancy free
            // Remark: Notice proper JSON convention uses camelCase for keys
            StringBuilder jsonBuilder = new();
            jsonBuilder.Append("{");
            foreach ((string Key, string[] Values) in dictionary)
            {
                jsonBuilder.Append($"\n  \"{Key.Camelize()}\": [");
                foreach (var value in Values)
                    jsonBuilder.Append($"   \"{value}\",");
                jsonBuilder.Length--; // Remove trailing comma
                jsonBuilder.Append($"],");
            }
            jsonBuilder.Length--; // Remove trailing comma
            jsonBuilder.Append("\n}\n");
            return jsonBuilder.ToString().TrimEnd();
        }
        private static string SerializeFlatStringDictionaryStructure(Dictionary<string, string> dictionary)
        {
            // Remark: We intentionally don't use JSON libraries for such simple structure to guarantee predictable behaviors, keep code clean and dependancy free
            // Remark: Notice proper JSON convention uses camelCase for keys
            StringBuilder jsonBuilder = new();
            jsonBuilder.Append("{");
            foreach ((string Key, string Value) in dictionary)
                jsonBuilder.Append($"\n  \"{Key.Camelize()}\": \"{Value}\",");
            jsonBuilder.Length--; // Remove trailing comma
            jsonBuilder.Append("\n}\n");
            return jsonBuilder.ToString().TrimEnd();
        }
        private static string SerializeFlatSimplexStringDictionaryStructure(Dictionary<string, SimplexString> dictionary)
        {
            // Remark: We intentionally don't use JSON libraries for such simple structure to guarantee predictable behaviors, keep code clean and dependancy free
            // Remark: Notice proper JSON convention uses camelCase for keys
            StringBuilder jsonBuilder = new();
            jsonBuilder.Append("{");
            foreach ((string Key, SimplexString Value) in dictionary)
                jsonBuilder.Append($"\n  \"{Key.Camelize()}\": {Value.ToJSONString()},");
            jsonBuilder.Length--; // Remove trailing comma
            jsonBuilder.Append("\n}\n");
            return jsonBuilder.ToString().TrimEnd();
        }
        private static string SerializaDataGrid(DataGrid result)
        {
            if (result.Raw != null)
                return $"\"{result.Raw.Replace("\n", "\\n")}\"";
            else
                throw new NotImplementedException();
        }
        private static string SerializePayload(ParcelPayload payload)
        {
            // TODO: Differentiate between serializable and non-serializable fields

            // Remark: At the moment we are only serializing simple values
            StringBuilder result = new();
            result.AppendLine("{");
            foreach (KeyValuePair<string, object> item in payload.PayloadData)
            {
                try
                {
                    string valueSerialization = SerializeResult(item.Value);
                    result.AppendLine($"\"{item.Key}\": {valueSerialization}");
                }
                catch (Exception)
                {
                    continue;
                }
            }
            result.AppendLine("}");
            return result.ToString().TrimEnd();
        }
        #endregion
    }
}
