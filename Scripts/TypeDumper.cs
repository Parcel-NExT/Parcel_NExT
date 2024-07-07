/* Type Dumper
Version: v0.0.1
Platform: Pure

This script dumps standard C# type members for easier migration/inspection purpose (because C#/Visual Studio's Object Browser is slow and useless).
*/
using System.Reflection;

string typeName = Arguments[0]; // Full name
string referenceDocumentation = Arguments[1]; // Dumped documentation from MSDN. Format is signature + tab + one-line summary

Dictionary<string, string> documentation = File
    .ReadLines(referenceDocumentation)
    .Select(line => line.Split('\t'))
    .ToDictionary(parts => parts.First(), parts => parts.Last());

// Enumerate all types
int memberCount = 0;
foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
{
    foreach (Type type in assembly.GetExportedTypes())  // Gets only public types
    {
        if (type.FullName == typeName)
        {
            foreach(MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)) // Public instance and non-instance methods
            {
                string signature = method.ToString()
                    .Replace("System.IO.", string.Empty)
                    .Replace("System.", string.Empty);
                signature = signature[(signature.IndexOf(' ') + 1)..];
                if (documentation.TryGetValue(signature, out string value))
                    WriteLine($"""
                    /// <summary>
                    /// {value}
                    /// </summary>
                    """);

                string returnType = ShortenTypeName(method.ReturnType.FullName);
                string declaration = $"public static {returnType} {method.Name}({string.Join(", ", method.GetParameters().Select(p => $"{ShortenTypeName(p.ParameterType.FullName)} {p.Name}"))})";
                WriteLine($"{declaration} => {typeName}.{method.Name}({string.Join(", ", method.GetParameters().Select(p => p.Name))});");

                memberCount++;
            }
        }
    }
}
WriteLine($"Member count: {memberCount}");

// Helpers
static string ShortenTypeName(string original)
{
    return original switch
    {
        "System.String" => "string",
        "System.String[]" => "string[]",
        "System.Int32" => "int",
        "System.Boolean" => "bool",
        "System.Object" => "object",
        "System.Void" => "void",
        "System.DateTime" => "DateTime",
        _ => original
    };
}