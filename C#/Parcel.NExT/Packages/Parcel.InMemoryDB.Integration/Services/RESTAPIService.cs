using Parcel.Infrastructure;
using Parcel.Types;

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

            new(EndpointDefinition.GETMethod, "/Tables", GetTables)
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
            System.Data.DataTable? result = Database.Execute(command);
            string? csv = result?.ToCSV();
            string reply = csv ?? string.Empty;
            return reply;
        }
        protected EndpointResponse GetTables(Dictionary<string, string> parameters, string body)
        {
            System.Data.DataTable? result = Database.Execute("SELECT name FROM sqlite_schema WHERE type='table' ORDER BY name");
            string? csv = result?.ToCSV();
            string reply = csv ?? string.Empty;
            return reply;
        }
        #endregion
    }
}
