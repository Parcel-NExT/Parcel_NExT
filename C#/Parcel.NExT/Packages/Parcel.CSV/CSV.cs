namespace Parcel.FileFormats
{
    public static class CSV
    {
        #region Interface
        public static string[][] Parse(string csv, out string[]? headers, bool containsHeader)
        {
            IEnumerable<string[]> lines = ParseCSV(csv, out headers, containsHeader);
            return lines.ToArray();
        }
        #endregion

        #region Helper (Borrowed from Parcel.CoreEngine, hidden from public usage; This is to eliminate dependency)
        private static readonly char[] NewLineSeparators = ['\r', '\n'];
        private static string[] SplitLines(this string text) => text.Split(NewLineSeparators);
        public static string[] SplitLines(this string text, bool removeEmpty) => text.Split(NewLineSeparators, removeEmpty ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
        private static IEnumerable<string[]> ReadCSVFile(string path, out string[]? headers, bool containsHeader = true)
        {
            IEnumerable<string> lines = File.ReadLines(path);
            if (containsHeader)
            {
                string[] allLines = lines.ToArray();

                headers = SplitCSVLine(allLines.First()).ToArray();
                return allLines.Skip(1).Select(line => SplitCSVLine(line).ToArray());
            }
            else
            {
                headers = null;
                return lines.Select(line => SplitCSVLine(line).ToArray());
            }
        }
        private static IEnumerable<string[]> ParseCSV(string text, out string[]? headers, bool containsHeader = true)
        {
            string[] lines = text.SplitLines(true);
            if (containsHeader)
            {
                headers = SplitCSVLine(lines.First()).ToArray();
                return lines.Skip(1).Select(line => SplitCSVLine(line).ToArray());
            }
            else
            {
                headers = null;
                return lines.Select(line => SplitCSVLine(line).ToArray());
            }
        }
        private static IEnumerable<string> SplitCSVLine(string csvline)
            => SplitCommandLine(csvline, ',');
        private static IEnumerable<string> SplitCommandLine(string commandLine, char delimiter = ' ')
        {
            bool inQuotes = false;

            return commandLine.Split(c =>
            {
                if (c == '\"')
                    inQuotes = !inQuotes;

                return !inQuotes && c == delimiter;
            })
                .Select(arg => arg.Trim().TrimMatchingQuotes('\"'))
                .Where(arg => !string.IsNullOrEmpty(arg));
        }
        private static IEnumerable<string> Split(this string str, Func<char, bool> controller)
        {
            int nextPiece = 0;

            for (int c = 0; c < str.Length; c++)
            {
                if (controller(str[c]))
                {
                    yield return str[nextPiece..c];
                    nextPiece = c + 1;
                }
            }

            yield return str[nextPiece..];
        }
        private static string TrimMatchingQuotes(this string input, char quote)
        {
            if ((input.Length >= 2) &&
                (input[0] == quote) && (input[^1] == quote))
                return input[1..^1];

            return input;
        }
        #endregion
    }
}
