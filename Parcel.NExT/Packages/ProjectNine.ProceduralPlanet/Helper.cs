using System.Reflection;

namespace HDPlanet
{
    internal static class Helper
    {
        public static Dictionary<string, string> GetEmbeddedColorResources()
        {
            const string defaultNameSpace = "ProjectNine.Tooling.Generative";
            return Assembly.GetExecutingAssembly().GetManifestResourceNames()
                .Where(r => r.StartsWith(defaultNameSpace))
                .ToDictionary(r => r[$"{defaultNameSpace}.Assets.Colors.".Length..], r => r);
        }
        public static string ReadResource(string name)
        {
            // Determine path
            var assembly = Assembly.GetExecutingAssembly();
            string resourcePath = name;
            // Format: "{Namespace}.{Folder}.{filename}.{Extension}"
            if (!name.StartsWith(nameof(HDPlanet)))
            {
                resourcePath = assembly.GetManifestResourceNames()
                    .Single(str => str.EndsWith(name));
            }

            using Stream stream = assembly.GetManifestResourceStream(resourcePath);
            using StreamReader reader = new(stream);
            return reader.ReadToEnd();
        }
    }
}
