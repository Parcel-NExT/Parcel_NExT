using System.Reflection;

namespace Parcel.CoreEngine.Helpers
{
    public static class EmbeddedResourceHelper
    {
        /// <remarks>
        /// Caller must dispose stream.
        /// </remarks>
        public static Stream ReadBinaryResource(string name)
        {
            // Determine path
            var assembly = Assembly.GetCallingAssembly();
            string resourcePath = name;
            // Format: "{Namespace}.{Folder}.{filename}.{Extension}"
            if (!name.StartsWith(nameof(Parcel)))
            {
                resourcePath = assembly.GetManifestResourceNames()
                    .Single(str => str.EndsWith(name));
            }

            return assembly.GetManifestResourceStream(resourcePath);
        }

        public static string ReadTextResource(string name)
        {
            // Determine path
            var assembly = Assembly.GetCallingAssembly();
            string resourcePath = name;
            // Format: "{Namespace}.{Folder}.{filename}.{Extension}"
            if (!name.StartsWith(nameof(Parcel)))
            {
                resourcePath = assembly.GetManifestResourceNames()
                    .Single(str => str.EndsWith(name));
            }

            using Stream? stream = assembly.GetManifestResourceStream(resourcePath);
            if (stream == null)
                throw new ArgumentException($"Cannot read {name}");
            using StreamReader reader = new(stream);
            return reader.ReadToEnd();
        }
    }
}