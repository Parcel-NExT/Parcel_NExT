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
            return WebServer.ServerAddress;
        }
        private EndpointDefinition[] ServerEndpoints => [
            new(EndpointDefinition.GETMethod, "/", Frontpage)
        ];
        #endregion

        #region Handling Routines
        private string Frontpage(Dictionary<string, string> parameters, string body)
        {
            return """
                <!DOCTYPE html>
                <html lang="en">
                
                <head>
                  <meta charset="utf-8">
                  <meta name="viewport" content="width=device-width, initial-scale=1">
                  <title>In-Memory Database</title>
                </head>
                
                <body>
                  <h1>Welcome!</h1>
                  <p>This is the front-end.</p>
                </body>
                
                </html>
                """;
        }
        #endregion
    }
}
