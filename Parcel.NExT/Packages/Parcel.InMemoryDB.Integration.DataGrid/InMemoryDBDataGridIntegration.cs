using Microsoft.Data.Sqlite;
using Parcel.Types;
using System.Data;
using System.Data.Common;

namespace Parcel.Database.InMemoryDB.Integration
{
    public static class InMemoryDBDataGridIntegration
    {
        #region Query Extension
        public static DataGrid FetchDataAsDataGrid(this InMemorySQLIte connection, string sqlQuery)
        {
            DataTable? dataTable = connection.Execute(sqlQuery);
            if (dataTable != null)
                return new(dataTable);
            else throw new ApplicationException("Failed to get data.");
        }
        public static DataGrid GetTableAsDataGrid(this InMemorySQLIte connection, string name)
            => FetchDataAsDataGrid(connection, @$"select * from {name}");
        #endregion

        #region Import Extension
        /// <summary>
        /// Import new table; Table mustn't already exist.
        /// </summary>
        public static void ImportAsTable(this InMemorySQLIte connection, DataGrid table, string tableName)
        {
            connection.CreateTable(tableName, table.Columns.Select(c => c.Header).ToArray());
            InsertData(connection.InstanceConnection, tableName, table);
        }
        #endregion

        #region Helper
        public static void InsertData(this SqliteConnection connection, string tableName, DataGrid dataGrid)
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
