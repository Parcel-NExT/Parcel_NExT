namespace Parcel.CoreEngine.Helpers
{
    public static class CSVHelper
    {
        public static IEnumerable<string[]> ReadCSVFile(string path, out string[]? headers, bool containsHeader = true)
        {
            IEnumerable<string> lines = File.ReadLines(path);
            if (containsHeader)
            {
                string[] allLines = lines.ToArray();

                headers = StringHelper.SplitCSVLine(allLines.First()).ToArray();
                return allLines.Skip(1).Select(line => StringHelper.SplitCSVLine(line).ToArray());
            }
            else
            {
                headers = null;
                return lines.Select(line => StringHelper.SplitCSVLine(line).ToArray());
            }
        }

        public static IEnumerable<string[]> ParseCSV(this string text, out string[]? headers, bool containsHeader = true)
        {
            string[] lines = text.SplitLines(true);
            if (containsHeader)
            {
                headers = StringHelper.SplitCSVLine(lines.First()).ToArray();
                return lines.Skip(1).Select(line => StringHelper.SplitCSVLine(line).ToArray());
            }
            else
            {
                headers = null;
                return lines.Select(line => StringHelper.SplitCSVLine(line).ToArray());
            }
        }
    }
}
