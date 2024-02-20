using System.Text;

namespace Parcel.NExT.Python.Helpers
{
    public static class StringHelper
    {
        public static string JoinAsArguments(this string[] args)
        {
            if (args == null || args.Length == 0)
                return "";
            return string.Join(" ", args.Select(a => a.Contains('"') ? $"\"{a}\"" : a));
        }
        public static string[] SplitCommandLineArguments(this string inputString, char separator = ',', bool includeQuotesInString = false)
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
    }
}
