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
        private InMemorySQLIte Database { get; }
        private DevelopmentServer? WebServer { get; set; }
        #endregion

        #region Method
        public string Start()
        {
            WebServer = DevelopmentServer.StartServerInNewThread(ServerEndpoints);
            Thread.Sleep(1000); // Wait for socket to bind
            return WebServer.ServerAddress;
        }
        private EndpointDefinition[] ServerEndpoints => [
            new(EndpointDefinition.POSTMethod, "/Welcome", HandleWelcome),
            new(EndpointDefinition.POSTMethod, "/", HandleCommands)
        ];
        #endregion

        #region Handling Routines
        private EndpointResponse HandleWelcome(Dictionary<string, string> parameters, string body)
        {
            return "Welcome!";
        }
        private EndpointResponse HandleCommands(Dictionary<string, string> parameters, string body)
        {
            string command = body;
            System.Data.DataTable? result = Database.Execute(command);
            string? csv = result?.ToCSV();
            string reply = csv ?? string.Empty;
            return reply;
        }
        #endregion
    }
}
