using ConsoleTables;
using Parcel.Types;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;

namespace Parcel.Database.InMemoryDB.Integration
{
    public static class ConsoleFormatting
    {
        public static string Format(this DataTable dataTable) 
        {
            ConsoleTable consoleTable = new();
            string[] columns = Enumerable
                .Range(0, dataTable.Columns.Count)
                .Select(i => dataTable.Columns[i].ColumnName)
                .ToArray();
            consoleTable.AddColumn(columns);

            foreach (DataRow dr in dataTable.Rows)
            {
                string?[] items = dr.ItemArray.Select(i => i.ToString()).ToArray();
                consoleTable.AddRow(items);
            }
            consoleTable.Write();
            return consoleTable.ToString();
        }

        public static void Display(DataGrid dataGrid)
        {
            Console.WriteLine(string.Join(',', dataGrid.Columns.Select(c => c.Header)), Color.Orange);
            // Display with empty trailing line only if there is no data
            string csv = dataGrid.ToCSV(false);
            int lineCount = Regex.Matches(csv, Environment.NewLine).Count;
            if (lineCount == 1) Console.WriteLine(csv);
            else Console.WriteLine(csv.TrimEnd());
        }
    }
}
