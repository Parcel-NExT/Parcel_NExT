using System.Reflection;

namespace HDPlanet
{
    public static class Helper
    {
        public static Dictionary<string, string> GetEmbeddedColorResources()
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceNames()
                .Where(r => r.StartsWith(nameof(HDPlanet)))
                .ToDictionary(r => r[$"{nameof(HDPlanet)}.Assets.Colors.".Length..], r => r);
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
