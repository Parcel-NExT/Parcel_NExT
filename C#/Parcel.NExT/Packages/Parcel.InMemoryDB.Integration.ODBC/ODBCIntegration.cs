using System.Data;
using System.Data.Odbc;
using Parcel.Services;

namespace Parcel.Database.InMemoryDB.Integration
{
    public static class ODBCIntegration
    {
        /// <summary>
        /// Append content of in-memory table to remote ODBC connection table. 
        /// </summary>
        public static void Transfer(this InMemorySQLIte connection, string tableName, string dsn, string remoteTableName = null)
        {
            remoteTableName ??= tableName;
            DataTable? table = connection[tableName];
            InsertIntoODBC(remoteTableName, table, dsn);
        }
        public static void ImportFromODBC(this InMemorySQLIte connection, string dsn, string query, string destinationTable)
        {
            connection.ImportAsTable(ODBCServices.FetchFromODBCDatabase(query, dsn, out string[] _), destinationTable);
        }
        public static void InsertIntoODBC(string tableName, DataTable table, string dsn)
        {
            OdbcConnection odbcConnection = new($"DSN={dsn}");
            odbcConnection.Open();
            ODBCServices.InsertData(odbcConnection, tableName, table);
            odbcConnection.Close();
        }
    }
}
