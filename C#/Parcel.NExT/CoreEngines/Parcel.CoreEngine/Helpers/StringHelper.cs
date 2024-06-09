using System.Text;

namespace Parcel.CoreEngine.Helpers
{
    public static class StringHelper
    {
        public static int GetDeterministicHashCode(this string str)
        {
            unchecked
            {
                int hash1 = (5381 << 16) + 5381;
                int hash2 = hash1;

                for (int i = 0; i < str.Length; i += 2)
                {
                    hash1 = (hash1 << 5) + hash1 ^ str[i];
                    if (i == str.Length - 1)
                        break;
                    hash2 = (hash2 << 5) + hash2 ^ str[i + 1];
                }

                return hash1 + hash2 * 1566083941;
            }
        }
        public static string[] SplitCommandLineArguments(this string inputString, char separator = ' ', bool includeQuotesInString = false)
        {
            List<string> parameters = [];
            StringBuilder current = new();

            bool inQuotes = false;
            foreach (var c in inputString)
            {
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                    if (includeQuotesInString)
                        current.Append(c);
                }
                else if (c == separator)
                {
                    if (!inQuotes)
                    {
                        parameters.Add(current.ToString());
                        current.Clear();
                    }
                    else
                        current.Append(c);
                }
                else
                {
                    current.Append(c);
                }
            }
            if (current.Length != 0)
                parameters.Add(current.ToString());
            return parameters.ToArray();
        }
        public static string Camelize(this string original)
        {
            return System.Text.Json.JsonNamingPolicy.CamelCase.ConvertName(original);
        }
    }
}
