/* Parcel.Plots.MakeConfiguration Source Code Generator
Generates codes for the configuration source code by parsing existing source code.
To use this, make Parcel.Graphing.MakeConfigurations empty, build the dll, then run this script.
*/

Import(Parcel.Plots)
Import(Humanizer.Core as Humanizer)
using System.Reflection;
using System.Text;

var assembly = Assembly.GetAssembly(typeof(Parcel.Graphing.Plot));
Type baseType = typeof(Parcel.Graphing.PlotConfigurations.BasicConfiguration);
Type[] configurationTypes = assembly.GetExportedTypes()
    .Where(t => baseType.IsAssignableFrom(t))
    .Where(type => type != baseType)
    .ToArray();
foreach(Type configurationType in configurationTypes)
{
    // Assume plain properties without hierarchies or complex types
    PropertyInfo[] properties = configurationType.GetProperties(); // Remark: Notice property order cannot be guaranteed, but since we have default values for all of them, it should be good.
    
    string typeName = configurationType.Name;
    string plotTypeName = GetPlotTypeName(typeName);

    // Get default values
    object defaultValueInstance = Activator.CreateInstance(configurationType);
    
    WriteLine($$"""
        /// <summary>
        /// Create a configuration for {{plotTypeName}}
        /// </summary>
        public static {{typeName}} Configure{{plotTypeName}}({{string.Join(", ", properties.Select(p => FormArgumentDeclaration(p, defaultValueInstance)))}})
        {
            {{InitializeDefaultColors(properties.Where(p => p.PropertyType == typeof(Parcel.Types.Color)), defaultValueInstance)}}return new {{typeName}}()
            {
                {{string.Join(",\n        ", properties.Select(p => $"{p.Name} = {p.Name.Camelize()}"))}}
            };
        }
        """);
}

// Helpers
private static string InitializeDefaultColors(IEnumerable<PropertyInfo> properties, object sampleInstance)
{
    StringBuilder builder = new();
    foreach(var property in properties)
    {
        object defaultValue = property.GetValue(sampleInstance);
        string defaultValueString = defaultValue?.ToString() ?? "null";    
        builder.AppendLine($"{property.Name.Camelize()} ??= Parcel.Types.Color.Parse(\"{defaultValueString}\");\n    ");
    }
    return $"{builder.ToString().Trim()}\n    ";
}
private static string GetPlotTypeName(string typeName)
{
    return typeName.Replace("Configuration", string.Empty);
}
private static string FormArgumentDeclaration(PropertyInfo property, object sampleInstance)
{
    object defaultValue = property.GetValue(sampleInstance);
    string defaultValueString = defaultValue?.ToString() ?? "null";
    string friendlyTypeName = GetFriendlyTypeName(property.PropertyType, defaultValue == null);
    if (property.PropertyType == typeof(string))
        defaultValueString = $"\"{defaultValue}\"";
    if (property.PropertyType == typeof(Parcel.Types.Color))
    {
        defaultValueString = "null";
        friendlyTypeName = $"{friendlyTypeName}?";
    }
    
    return $"{friendlyTypeName} {property.Name.Camelize()} = {defaultValueString}";
}
private static string GetFriendlyTypeName(Type type, bool nullableReference)
{
    string name = Nullable.GetUnderlyingType(type) != null ? Nullable.GetUnderlyingType(type).Name : type.Name; // Nullable only tests primitives and value types (structs); For nullable reference, it's not reflected on the type itself
    string friendlyName = name switch
    {  
        "Bool" => "bool",
        "Int32" => "int",
        // "Int32" => "uint",
        // BYte
        // Char
        // Decimal
        // Double
        // Float
        // Long
        // sbyte
        // short
        // ulong
        // ushort
        "String" => "string",
        _ => name
    };
    return (Nullable.GetUnderlyingType(type) != null || nullableReference) ? $"{friendlyName}?" : friendlyName;
}