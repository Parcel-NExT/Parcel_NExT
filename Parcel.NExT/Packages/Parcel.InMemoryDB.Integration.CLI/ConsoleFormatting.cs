using ConsoleTables;
using System.Data;

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
    }
}
