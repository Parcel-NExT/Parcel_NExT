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
            return WebServer.ServerAddress;
        }
        private EndpointDefinition[] ServerEndpoints => [
            new(EndpointDefinition.POSTMethod, "/", HandleCommands)
        ];
        #endregion

        #region Handling Routines
        private string HandleCommands(Dictionary<string, string> parameters, string body)
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
