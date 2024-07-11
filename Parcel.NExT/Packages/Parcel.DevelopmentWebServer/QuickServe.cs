namespace Parcel.Infrastructure
{
    public static class QuickServe
    {
        /// <summary>
        /// Serve single html text
        /// </summary>
        public static DevelopmentServer ServeHTML(string html, string endPoint = "/")
        {
            EndpointResponse MainPage(Dictionary<string, string> parameters, string body)
            {
                return html;
            }

            EndpointDefinition[] ServerEndpoints = [
                new(EndpointDefinition.GETMethod, "/", MainPage)
            ];

            DevelopmentServer server = DevelopmentServer.StartServerInNewThread(ServerEndpoints);
            return server;
        }
        /// <summary>
        /// Serve loose texts
        /// </summary>
        /// <remarks>
        /// Some predefined media type names can be found in MediaTypeNames class of the System.Net.Mime namesapce
        /// </remarks>
        public static DevelopmentServer ServeTexts(string[] texts, string[] endpoints, string[]? mediaTypeNames)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Serve files with default or custom endpoints
        /// </summary>
        public static DevelopmentServer ServeFiles(string[] paths, string[]? endpointNameOverrides = null)
        {
            throw new NotImplementedException();
        }
    }
}
