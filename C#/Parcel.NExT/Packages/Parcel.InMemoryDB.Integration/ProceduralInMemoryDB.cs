using System.Data;
using Parcel.Database.InMemoryDB;
using Parcel.Types;
using Parcel.Database.InMemoryDB.Integration;

namespace Parcel.Database
{
    #region Streamlined Workflow
    /// <remarks>
    /// Provides semantic actions, streamlining pure SQL based data analytics; Attemptive direct importing from DSNs and database servers.
    /// Notice the design of this generally assumes once created, the tables' contents will not be modified - all subsequent actions are achieved through views
    /// </remarks>
    public class ProceduralInMemoryDB : InMemorySQLIte
    {
        #region References
        private Dictionary<string, string> ViewProceduresReference { get; set; }
        private Dictionary<string, DataSourceQuery> SourceQueriesReferences { get; set; }
        #endregion

        #region Properties
        /// <summary>
        /// Cache tables; This is a main feature for this class
        /// </summary>
        private readonly Dictionary<string, DataTable> DataTables = [];
        #endregion

        #region End-User Facing Development
        // Remark: Those properties are helpful because when using InMemoryDB/SQLite often there is need for debuggin especially when developing SQL-driven workflows
        private bool GenerateDebugCSVs { get; set; } = true;
        private bool PrintDebugOutputs { get; set; } = true;
        #endregion

        #region Construction
        public ProceduralInMemoryDB(Dictionary<string, string> views, Dictionary<string, DataSourceQuery> queries, bool generateDebugCSVs = true, bool printDebugOutputs = true)
        {
            ViewProceduresReference = views;
            SourceQueriesReferences = queries;
            GenerateDebugCSVs = generateDebugCSVs;
            PrintDebugOutputs = printDebugOutputs;
        }
        #endregion

        #region Operators
        public new DataTable this[string name] => DataTables[name];
        #endregion

        #region Public Interface Methods
        public void Prep(string tableName, Dictionary<string, object>? parameters = null, string? queryName = null)
        {
            if (Tables.Contains(tableName))
                throw new ArgumentException($"Table {tableName} already exists.");

            queryName ??= tableName;
            DataSourceQuery queryInfo = SourceQueriesReferences[queryName];
            string query = FormatQuery(SourceQueriesReferences[queryName].QueryCommand, parameters);
            this.Load(tableName, new DataSourceQuery(queryInfo.SourceType, queryInfo.ConnectionTarget, query));
        }
        public void View(string viewName, Dictionary<string, object>? parameters = null, string? queryName = null)
        {
            if (Tables.Contains(viewName))
                throw new ArgumentException($"View {viewName} already exists.");

            queryName ??= viewName;
            string query = FormatQuery(ViewProceduresReference[queryName], parameters);

            Transform(viewName, query);
        }
        /// <summary>
        /// Reload a table from query list and potentially rewrite existing table.
        /// </summary>
        public void Rewrite(string tableName, Dictionary<string, object> parameters = null, string queryNameOverride = null)
        {
            string queryName = queryNameOverride != null ? queryNameOverride : tableName;

            DataSourceQuery queryInfo = SourceQueriesReferences[queryName];
            string query = FormatQuery(SourceQueriesReferences[queryName].QueryCommand, parameters);
            this.Load(tableName, new DataSourceQuery(queryInfo.SourceType, queryInfo.ConnectionTarget, query));
        }
        /// <summary>
        /// Import a datagrid directly as in-memory table; If table already exists, raise an exception. Use Import() directly if you wish to rewrite.
        /// </summary>
        public void Ingest(string tableName, DataTable data)
        {
            if (DataTables.ContainsKey(tableName))
                throw new ArgumentException($"Table {tableName} already exists.");

            DataTables[tableName] = data;
            ImportAsTable(data, tableName);
            PerformBookkeepingRoutine(data, tableName);
        }
        public void Transform(string viewName, string query)
        {
            Execute($"DROP VIEW IF EXISTS \"{viewName}\"");
            Execute($"CREATE VIEW \"{viewName}\" AS " + Environment.NewLine + query);

            DataTable datatable = Execute(query);
            PerformBookkeepingRoutine(datatable, viewName);
        }
        #endregion

        #region Routines
        public void PerformBookkeepingRoutine(DataTable dataTable, string tableName)
        {
            DataTables[tableName] = dataTable;

            Console.WriteLine($"# {tableName}");
            if (PrintDebugOutputs) dataTable.Format();
            if (GenerateDebugCSVs) File.WriteAllText($"{tableName}.csv", dataTable.ToCSV());
        }
        #endregion

        #region Helpers
        private static string FormatQuery(string template, Dictionary<string, object>? parameters = null)
        {
            parameters ??= [];

            if (parameters.Any(p => !p.Key.StartsWith('@')))
                throw new ArgumentException("Parameter name must start with \"@\" symbol.");

            return parameters.Aggregate(template, (current, parameter) => current.Replace(parameter.Key, parameter.Value.ToString()));
        }
        #endregion
    }
    #endregion
}
