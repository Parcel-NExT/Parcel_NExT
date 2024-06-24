using Microsoft.Data.Sqlite;
using Parcel.CoreEngine.Helpers;
using Parcel.Database.InMemoryDB.Integration;
using Parcel.Services;
using Parcel.Types;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Text;

namespace Parcel.Database.InMemoryDB
{
    public enum QuerySourceType
    {
        ODBC, // Target should target SQL database (ODBC DSN); Query is SQL
        MSAnalysisService, // Target should target Microsoft Analysis Service databse; Query is MDX
        CSV // Target refers to path to CSV file; Query is empty
    }

    public static class InMemoryDBIntegration
    {
        public static void Load(this ProceduralInMemoryDB connection, string tableName, QuerySourceType type, string target, string query)
        {
            connection.Execute($"DROP TABLE IF EXISTS \"{tableName}\"");
            if (type == QuerySourceType.ODBC)
            {
                var odbcConnection = new OdbcConnection($"DSN={target}");
                odbcConnection.Open();

                DataTable datatable = new();
                datatable.Load(new OdbcCommand(query, odbcConnection).ExecuteReader());
                connection.ImportAsTable(datatable, tableName);
                connection.PerformBookkeepingRoutine(datatable, tableName);
            }
            else if (type == QuerySourceType.MSAnalysisService)
            {
                var datatable = MSAnalysisServiceExtension.ExecuteMDXQuery(target, query);
                connection.ImportAsTable(datatable, tableName);
                connection.PerformBookkeepingRoutine(datatable, tableName);
            }
            else if (type == QuerySourceType.CSV)
            {
                string filePath = target;
                if (File.Exists(filePath))
                {
                    connection.ImportAsTable(CSVHelper.ReadCSVFile(filePath, out string[] headers, true), headers, tableName, out DataTable table);
                    connection.PerformBookkeepingRoutine(table, tableName);
                }
            }
        }
        public enum AutomaticTableTransferTarget
        {
            InMemoryDB,
            ODBC
        }
        /// <summary>
        /// Append content of a dataGrid to target database.
        /// </summary>
        public static void Push(this InMemorySQLIte connection, AutomaticTableTransferTarget target, string tableName, DataGrid dataGrid, string dsn)
        {
            if (target == AutomaticTableTransferTarget.InMemoryDB)
                connection.InstanceConnection.InsertData(tableName, dataGrid);
            else if (target == AutomaticTableTransferTarget.ODBC)
                InsertODBCData(tableName, dataGrid, dsn);
            else
                throw new ArgumentException($"Invalid target: Target should be either `{AutomaticTableTransferTarget.InMemoryDB}` or `{AutomaticTableTransferTarget.ODBC}.");
        }

        #region Helpers
        public static DataGrid FetchFromODBCDatabase(string query, string dsn)
        {
            DataTable dataTable = ODBCServices.FetchFromODBCDatabase(query, dsn, out string[] headers);

            // Convert to CSV
            StringBuilder output = new();
            output.AppendLine(string.Join(",", headers));
            foreach (DataRow row in dataTable.Rows)
            {
                IEnumerable<string> items = row.ItemArray.Select(item => $"\"{item.ToString().Replace("\"", "\"\"")}\"");
                string csvLine = string.Join(",", items);
                output.AppendLine(csvLine);
            }
            string csv = output.ToString();

            // Convert to Datagrid
            return new DataGrid("Unnamed", CSVHelper.ReadCSVFile(csv, out _, true), headers);
        }
        public static void InsertODBCData(string tableName, DataGrid dataGrid, string dsn)
        {
            OdbcConnection odbcConnection = new($"DSN={dsn}");
            odbcConnection.Open();
            odbcConnection.InsertData(tableName, dataGrid);
            odbcConnection.Close();
        }
        public static void InsertData(this OdbcConnection connection, string tableName, DataGrid dataGrid)
        {
            List<Types.DataColumn> columns = dataGrid.Columns;

            DbTransaction transaction = connection.BeginTransaction();
            foreach (IDictionary<string, object> row in dataGrid.Rows.Select(v => (IDictionary<string, object>)v))
            {
                DbCommand command = connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandText = $"INSERT INTO '{tableName}' ({string.Join(',', columns.Select(c => $"\"{c.Header}\""))}) VALUES ({string.Join(',', columns.Select(c => FormatValue(row[c.Header])))})";

                command.ExecuteNonQuery();
            }
            transaction.Commit();

            static string FormatValue(object value)
            {
                var text = value.ToString();
                if (double.TryParse(text, out _))
                    return text;
                else return $"\"{text}\"";
            }
        }
        #endregion
    }
}
