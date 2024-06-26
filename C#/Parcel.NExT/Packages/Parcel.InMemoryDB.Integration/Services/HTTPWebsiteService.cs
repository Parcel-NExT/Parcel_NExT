using Parcel.CoreEngine.Helpers;
using Parcel.Infrastructure;

namespace Parcel.Database.InMemoryDB.Services
{
    internal class HTTPWebsiteService
    {
        #region Constructor
        public HTTPWebsiteService(InMemorySQLIte database)
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
            new(EndpointDefinition.GETMethod, "/", Frontpage)
        ];
        #endregion

        #region Handling Routines
        private EndpointResponse Frontpage(Dictionary<string, string> parameters, string body)
        {
            return EmbeddedResourceHelper.ReadTextResource("Parcel.Database.InMemoryDB.Websites.Frontpage.html");
        }
        #endregion
    }
}
