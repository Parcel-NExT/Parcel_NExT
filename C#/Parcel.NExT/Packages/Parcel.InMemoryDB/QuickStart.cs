using Parcel.CoreEngine.Helpers;

namespace Parcel.Database
{
    public static class QuickStart
    {
        #region Quick Initialization
        public static InMemorySQLIte InitializeFromCSVs(string[] files, string[] tableNames)
        {
            InMemorySQLIte database = new();
            for (int i = 0; i < files.Length; i++)
            {
                string csvFile = files[i];
                IEnumerable<string[]> lines = CSVHelper.ParseCSV(File.ReadAllText(csvFile), out string[] headers, true);
                database.ImportAsTable(lines, headers, CleanTableName(tableNames[i]), out _);
            }
            return database;

            static string CleanTableName(string name)
            {
                char[] invalidCharacters = @"+-*/=\/.,;:".ToCharArray();
                foreach (var c in invalidCharacters)
                    name = name.Replace($"{c}", string.Empty);
                return name;
            }
        }
        public static InMemorySQLIte InitializeFromCSVs(string folderPath)
        {
            string[] csvFiles = Directory.EnumerateFiles(folderPath)
                .Where(f => Path.GetExtension(f).Equals(".csv", StringComparison.InvariantCultureIgnoreCase))
                .ToArray();
            return InitializeFromCSVs(csvFiles, csvFiles.Select(Path.GetFileNameWithoutExtension).ToArray());
        }
        #endregion
    }
}
