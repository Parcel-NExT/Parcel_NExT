namespace Parcel.Types
{
    /// <summary>
    /// Expose strongly named functions
    /// </summary>
    public static class DataGridBuilder
    {
        public static DataGrid InitializeDataGridFromCsvText(string tableName, string csvText)
            => new(tableName, MinimalCSV.ParseCSV(csvText, out string[]? headers, true), headers);
    }

    /// <summary>
    /// A private local minimal CSV parser for embedding in packages that wishes to have NO external dependencies
    /// </summary>
    internal static class MinimalCSV
    {
        #region Interface
        public static IEnumerable<string[]> ParseCSV(this string text, out string[]? headers, bool containsHeader = true)
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
        #endregion

        #region Helpers
        private static readonly char[] NewLineSeparators = ['\r', '\n'];
        private static string[] SplitLines(this string text, bool removeEmpty) => text.Split(NewLineSeparators, removeEmpty ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
        private static IEnumerable<string> SplitCSVLine(this string line, char delimiter = ' ')
        {
            bool inQuotes = false;

            return line.Split(c =>
            {
                if (c == '\"')
                    inQuotes = !inQuotes;

                return !inQuotes && c == delimiter;
            })
                .Select(arg => arg.Trim().TrimMatchingQuotes('\"'))
                .Where(arg => !string.IsNullOrEmpty(arg));
        }
        private static string TrimMatchingQuotes(this string input, char quote)
        {
            if ((input.Length >= 2) &&
                (input[0] == quote) && (input[^1] == quote))
                return input[1..^1];

            return input;
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
        #endregion
    }
}
