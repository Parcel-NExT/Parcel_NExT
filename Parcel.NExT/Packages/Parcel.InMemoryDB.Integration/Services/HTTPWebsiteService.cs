using Parcel.CoreEngine.Helpers;
using Parcel.Infrastructure;

namespace Parcel.Database.InMemoryDB.Services
{
    internal class HTTPWebsiteService: RESTAPIService
    {
        #region Constructor
        public HTTPWebsiteService(InMemorySQLIte database) 
            : base(database) { }
        #endregion

        #region Method
        public override string Start()
        {
            WebServer = DevelopmentServer.StartServerInNewThread(ServerEndpoints);
            Thread.Sleep(1000); // Wait for socket to bind
            return WebServer.ServerAddress;
        }
        private EndpointDefinition[] ServerEndpoints => [
            new(EndpointDefinition.GETMethod, "/", Frontpage),
            new(EndpointDefinition.POSTMethod, "/Query", HandleCommands),
            
            new(EndpointDefinition.GETMethod, "/Tables", GetTables),
            new(EndpointDefinition.GETMethod, "/Download", GetDownload),
            new(EndpointDefinition.GETMethod, "/Performance", GetPerformanceMeasure)
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
