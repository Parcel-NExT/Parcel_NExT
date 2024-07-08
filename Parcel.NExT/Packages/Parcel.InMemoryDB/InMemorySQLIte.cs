using Microsoft.Data.Sqlite;
using System.Data;
using System.Data.Common;
using Parcel.Types;
using Parcel.CoreEngine.Helpers;

namespace Parcel.Database
{
    public class InMemorySQLIte : IDisposable
    {
        #region Construction
        public SqliteConnection InstanceConnection { get; }
        /// <remarks>
        /// Internal/Cross-module use only, might want to hide from Parcel front-ends.
        /// TODO: (POS) Support hiding members.
        /// </remarks>
        public const string InMemorySQLiteDatabaseConnectionSourceNameToken = ":memory:";

        /// <param name="dataSource">Can be either file path to SQLite DB, a network connection (socket), or :memory:</param>
        public InMemorySQLIte(string dataSource = InMemorySQLiteDatabaseConnectionSourceNameToken)
        {
            InstanceConnection = new($"Data Source={dataSource}"); // Remark: Notice we do not need any other connection string for automatic database creation; Mode=ReadWrite doens't work.
            InstanceConnection.Open();
            // Disposed in disposal routines
        }
        /// <summary>
        /// Provides a semantic endpoint for connecting to file/path/protocol service (socket).
        /// </summary>
        public static InMemorySQLIte Connect(string url)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Disposal
        private bool _disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                    InstanceConnection?.Close();

                _disposed = true;
            }
        }
        ~InMemorySQLIte()
            => Dispose(false);
        #endregion

        #region interface
        /// <summary>
        /// Execute arbitrary query. This functions combines query/non-query type execution in a single function.
        /// </summary>
        public DataTable? Execute(string sqlQuery)
        {
            try
            {
                string formattedQuery = sqlQuery.EndsWith(';') ? sqlQuery : sqlQuery + ';';

                // Select queries have returns
                if (sqlQuery.ToLower().Trim().StartsWith("select"))
                {
                    DataTable table = new();
                    table.Load(new SqliteCommand(formattedQuery, InstanceConnection).ExecuteReader()); // TODO: Currently DataTable/Reader has issue dealing with null values in number columns, which SQLite handles it totally fine.
                    return table;
                }
                // Non-select queries have no returns
                else
                {
                    using SqliteCommand command = new(sqlQuery, InstanceConnection);
                    command.ExecuteNonQuery();
                    return null;
                }
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message.Replace(Environment.NewLine, " "));
            }
        }
        /// <summary>
        /// Import string values as table
        /// </summary>
        public void ImportAsTable(IEnumerable<string[]> csvLines, string[] headers, string tableName, out DataTable? dataTable)
        {
            CreateTable(tableName, headers);

            InsertData(InstanceConnection, tableName, csvLines, headers);
            // TODO: Optimize - can directly return dataTable using csv lines instead of fetching from database.
            dataTable = Execute($"select * from {tableName}");
        }
        public void ImportAsTable(DataTable dataTable, string tableName)
        {
            if (dataTable.Rows.Count == 0)
            {
                string[] headers = new string[dataTable.Columns.Count];
                for (int i = 0; i < headers.Length; i++)
                    headers[i] = dataTable.Columns[i].ColumnName;
                CreateTable(tableName, headers);
            }
            else
            {
                // Convert to CSV (we could do better, but this is easy)
                string csv = dataTable.ToCSV();

                // Import as csv
                ImportAsTable(CSVHelper.ParseCSV(csv, out string[]? headers, true), headers, tableName, out _); // TODO: Remark-cz: Why can't we directly import from DataTable?
            }
        }
        /// <summary>
        /// Create empty table.
        /// </summary>
        public void CreateTable(string tableName, string[] headers)
        {
            if (InstanceConnection == null)
                throw new Exception($"Database is not initialized.");

            SqliteCommand cmd = InstanceConnection.CreateCommand();
            cmd.CommandText = $"CREATE TABLE '{tableName}'({string.Join(',', headers.Select(c => $"\"{c}\""))})";
            cmd.ExecuteNonQuery();
        }
        /// <summary>
        /// Append to existing table.
        /// </summary>
        public void AppendToTable(string tableName, DataTable table)
            => InsertData(InstanceConnection, tableName, table);
        /// <summary>
        /// Migrate table from current InMemoryDB to another; If target table already exists, simply append to it.
        /// </summary>
        public void MigrateInto(string tableName, InMemorySQLIte other, string targetTableName)
        {
            if (!TablesAndViews.Contains(tableName))
                throw new ArgumentException($"Table '{tableName}' doesn't exist.");

            if (other.Tables.Contains(targetTableName))
                other.AppendToTable(targetTableName, this[tableName]!);
            else
                other.ImportAsTable(this[tableName]!, targetTableName);
        }
        /// <summary>
        /// Export SQLite database file
        /// </summary>
        public void Export(string path = "Export.sqlite")
        {
            if (InstanceConnection == null)
                throw new Exception($"Database is not initialized.");

            SqliteConnection saveFile = new($"Data Source={path}");
            saveFile.Open();
            InstanceConnection.BackupDatabase(saveFile, "main", "main");
            saveFile.Close();
        }
        #endregion

        #region Accessor
        /// <summary>
        /// Get names of all views
        /// </summary>
        public IEnumerable<string> Views => Execute(@"SELECT name FROM sqlite_schema WHERE type='view' ORDER BY name")!.Columns[0].ExtractValues().Cast<string>();
        /// <summary>
        /// Get names of all tables
        /// </summary>
        public IEnumerable<string> Tables => Execute(@"SELECT name FROM sqlite_schema WHERE type='table' ORDER BY name")!.Columns[0].ExtractValues().Cast<string>();
        /// <summary>
        /// Get names of all tables and views
        /// </summary>
        public IEnumerable<string> TablesAndViews => Execute(@"SELECT name FROM sqlite_schema WHERE type in ('table', 'view') ORDER BY name")!.Columns[0].ExtractValues().Cast<string>();   

        /// <summary>
        /// Get table
        /// </summary>
        public DataTable? this[string name] => GetDataTable(name);
        public DataTable? GetDataTable(string name)
            => Execute(@$"select * from {name}");

        /// <summary>
        /// Check precense of either table or view
        /// </summary>
        public bool ContainsTableOrView(string name) 
            => TablesAndViews.Contains(name);
        public bool ContainsTable(string name)
            => Tables.Contains(name);
        public bool ContainsView(string name)
            => Views.Contains(name);
        #endregion

        #region Helpers
        private static void InsertData(SqliteConnection connection, string tableName, DataTable table)
        {
            DataColumnCollection columns = table.Columns;
            string[] headers = columns.GetHeaders();

            DbTransaction transaction = connection.BeginTransaction();
            foreach (IDictionary<string, object> row in table.Rows.Select(v => (IDictionary<string, object>)v))
                InsertValues(connection, tableName, headers, transaction, headers.Select(c => row[c]));
            transaction.Commit();
        }
        private static void InsertData(SqliteConnection connection, string tableName, IEnumerable<string[]> csvLines, string[] headers)
        {
            DbTransaction transaction = connection.BeginTransaction();
            foreach (string[] row in csvLines)
                InsertValues(connection, tableName, headers, transaction, row);
            transaction.Commit();
        }
        private static void InsertValues(SqliteConnection connection, string tableName, string[] headers, DbTransaction transaction, IEnumerable<object> rowValues)
        {
            DbCommand command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = $"INSERT INTO '{tableName}' ({string.Join(',', headers.Select(h => $"\"{h}\""))}) VALUES ({string.Join(',', rowValues.Select(FormatValue))})";

            command.ExecuteNonQuery();

            static string FormatValue(object value)
            {
                string? text = value.ToString();
                if (double.TryParse(text, out _))
                    return text;
                else return $"\"{text}\"";
            }
        }
        #endregion
    }
}