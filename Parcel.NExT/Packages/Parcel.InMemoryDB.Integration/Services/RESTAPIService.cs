using Parcel.Infrastructure;
using Parcel.Types;
using System.Diagnostics;

namespace Parcel.Database.InMemoryDB.Services
{
    internal class RESTAPIService
    {
        #region Constructor
        public RESTAPIService(InMemorySQLIte database) 
            => Database = database;
        #endregion

        #region Properties
        protected InMemorySQLIte Database { get; }
        protected DevelopmentServer? WebServer { get; set; }
        #endregion

        #region States
        protected double PreviousCommandExecutionTimeInMs { get; set; }
        protected string? PreviousQueryResult { get; set; }
        #endregion

        #region Method
        public virtual string Start()
        {
            WebServer = DevelopmentServer.StartServerInNewThread(ServerEndpoints);
            Thread.Sleep(1000); // Wait for socket to bind
            return WebServer.ServerAddress;
        }
        private EndpointDefinition[] ServerEndpoints => [
            new(EndpointDefinition.POSTMethod, "/", HandleWelcome),
            new(EndpointDefinition.POSTMethod, "/Query", HandleCommands),

            new(EndpointDefinition.GETMethod, "/Tables", GetTables),
            new(EndpointDefinition.GETMethod, "/Download", GetDownload),
            new(EndpointDefinition.GETMethod, "/Performance", GetPerformanceMeasure)
        ];
        #endregion

        #region Handling Routines
        protected EndpointResponse HandleWelcome(Dictionary<string, string> parameters, string body)
        {
            return "Welcome! To get started, send a POST request to /Query with SQL query as plain body.";
        }
        protected EndpointResponse HandleCommands(Dictionary<string, string> parameters, string body)
        {
            string command = body;
            var timer = new Stopwatch();
            timer.Start();
            System.Data.DataTable? result = Database.Execute(command);
            PreviousQueryResult = result?.ToCSV();
            timer.Stop();
            PreviousCommandExecutionTimeInMs = timer.ElapsedMilliseconds;
            return PreviousQueryResult ?? string.Empty;
        }
        protected EndpointResponse GetTables(Dictionary<string, string> parameters, string body)
        {
            System.Data.DataTable? result = Database.Execute("SELECT name FROM sqlite_schema WHERE type='table' ORDER BY name");
            string? tables = result?.ToCSV();
            return tables ?? string.Empty;
        }
        protected EndpointResponse GetDownload(Dictionary<string, string> parameters, string body)
        {
            string downloadContent = PreviousQueryResult ?? string.Empty;
            return new EndpointResponse(downloadContent, 200, MIMETypeNames.TextCsv);
        }
        protected EndpointResponse GetPerformanceMeasure(Dictionary<string, string> parameters, string body)
        {
            string performance = $"Query finished in {PreviousCommandExecutionTimeInMs:F2}ms";
            return new EndpointResponse(performance, 200, MIMETypeNames.TextPlain);
        }
        #endregion
    }
}
