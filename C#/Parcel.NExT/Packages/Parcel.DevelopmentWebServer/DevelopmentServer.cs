using System.Net;
using System.Net.Sockets;
using System.Text;
using static Parcel.Infrastructure.EndpointDefinition;

namespace Parcel.Infrastructure
{
    public class EndpointDefinition(string endpoint, EndpointHandler handler)
    {
        public delegate string EndpointHandler(Dictionary<string, string> Parameters, string Body);

        public string Endpoint { get; } = endpoint;
        public EndpointHandler Handler { get; } = handler;
    }
    public class ServerMetadata
    {
        /// <summary>
        /// Shown in default template as page title
        /// </summary>
        public string? WebsiteName { get; set; }
    }
    public class DevelopmentServer
    {
        #region Internal
        const int bufferSize = 2048;
        #endregion

        #region Construction
        public DevelopmentServer(EndpointDefinition[]? endpoints = null)
        {
            Endpoints = endpoints?.ToDictionary(e => e.Endpoint, e => e.Handler) ?? null;
        }
        #endregion

        #region Properties
        public int Port { get; private set; }
        public string ServerAddress { get; private set; }
        public string[]? EndpointNames => Endpoints?.Keys.ToArray();
        #endregion

        #region Settings
        public Dictionary<string, EndpointHandler>? Endpoints { get; }
        #endregion

        #region Interface Method
        public static DevelopmentServer StartServer(EndpointDefinition[]? endpoints)
        {
            var server = new DevelopmentServer(endpoints);
            server.Start(FindNextFreeTcpPort());
            return server;
        }
        public static DevelopmentServer StartServer(EndpointDefinition[]? endpoints, int port)
        {
            var server = new DevelopmentServer(endpoints);
            server.Start(port);
            return server;
        }
        public static DevelopmentServer StartServerInNewThread(EndpointDefinition[]? endpoints, int port)
        {
            var server = new DevelopmentServer(endpoints);
            new Thread(() => server.Start(port)).Start();
            return server;
        }
        public static DevelopmentServer StartServerInNewThread(EndpointDefinition[]? endpoints)
        {
            var server = new DevelopmentServer(endpoints);
            new Thread(() => server.Start(FindNextFreeTcpPort())).Start();
            return server;
        }

        public void Start(int port)
        {
            TcpListener server = new(IPAddress.Any, port);
            server.Start();

            Port = port;
            ServerAddress = $"http://localhost:{port}";

            // Main server loop
            while (true) // TODO: Remark: At the moment we are handling in disconnected mode
            {
                // Wait for connection
                TcpClient client = server.AcceptTcpClient();
                new Thread(() => HandleClient(client)).Start();
            }
        }

        private void HandleClient(TcpClient client)
        {
            // TODO Handle keep-alive connection type

            byte[] bytes = new byte[bufferSize]; // Buffer for reading data

            // Loop to receive all the data sent by the client.
            StringBuilder builder = new();
            int i;
            NetworkStream stream = client.GetStream();
            while (stream.DataAvailable && (i = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                // Translate data bytes to a string.
                string data = Encoding.UTF8.GetString(bytes, 0, i);
                builder.Append(data);
            }

            // Final request message
            string requestMessage = builder.ToString().TrimEnd();

            // Send reply
            if (!string.IsNullOrWhiteSpace(requestMessage)) // Can be empty (e.g. Firefox constantly send empty requests)
            {
                byte[] replyData = MakeReply(requestMessage, out _, out _);
                stream.Write(replyData);
            }

            client.Dispose();
        }
        #endregion

        #region Routines
        private byte[] MakeReply(string requestMessage, out string replyBody, out string replyHeader)
        {
            SplitMessageBody(requestMessage, out string requestHeader, out string requestBody);
            Dictionary<string, string> headerFields = GetHeaders(requestHeader, out string endpoint, out string parametersString);
            Dictionary<string, string> queryParameters = ParseQueries(parametersString);

            string replyMessage = string.Empty;
            if (Endpoints?.TryGetValue(endpoint, out EndpointHandler? handler) ?? false)
                replyMessage = HandleGenericRequest(handler, queryParameters, requestBody, out replyBody, out replyHeader);
            else
                replyMessage = HandleNotFound(out replyBody, out replyHeader);

            byte[] replyData = Encoding.UTF8.GetBytes(replyMessage);
            return replyData;
        }

        private static string HandleGenericRequest(EndpointHandler handler, Dictionary<string, string> queryParameters, string requestBody, out string replyBody, out string replyHeader)
        {
            replyBody = handler(queryParameters, requestBody);
            replyHeader = $"""
                    HTTP/1.1 200 OK
                    Date: {DateTime.Now.ToUniversalTime():r}
                    Connection: close
                    Vary: Origin
                    Cache-Control: public, max-age=0
                    Last-Modified: {DateTime.Now.ToUniversalTime():r}
                    ETag: W/"{Guid.NewGuid().ToString()[..15]}"
                    Content-Type: text/html; charset=UTF-8
                    Content-Length: {replyBody.Length}
                    """;
            return $"""
                   {replyHeader}
                   
                   {replyBody}
                   """;
        }

        private static string HandleNotFound(out string replyBody, out string replyHeader)
        {
            NotFoundReplyHandler.HandleReply(out replyBody, out replyHeader);
            return $"""
                   {replyHeader}
                   
                   {replyBody}
                   """;
        }
        #endregion

        #region Parsing
        private static Dictionary<string, string> ParseQueries(string parametersString)
        {
            if (string.IsNullOrEmpty(parametersString))
                return [];
            return parametersString
                .Split('&')
                .Select(parameter => parameter.Split('='))
                .ToDictionary(pair => pair[0], pair => pair[1]);
        }
        private static Dictionary<string, string> GetHeaders(string requestHeader, out string endpoint, out string parametersString)
        {
            string[] headerLines = requestHeader.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            string firstLine = headerLines[0];
            ParseFirstLine(firstLine, out string method, out endpoint, out parametersString, out string version);

            return headerLines
                .Skip(1)
                .Select(line =>
                {
                    int delimiter = line.IndexOf(':');
                    return (line[..delimiter], line[(delimiter + 1)..]);
                })
                .ToDictionary(p => p.Item1, p => p.Item2);
        }
        private static void ParseFirstLine(string firstLine, out string method, out string endpoint, out string parametersString, out string protocolVersion)
        {
            string[] parts = firstLine.Split(' ');
            method = parts[0];
            string queryUrl = parts[1];
            protocolVersion = parts[2];

            int lastSplitter = queryUrl.LastIndexOf('?');
            if (lastSplitter != -1)
            {
                endpoint = queryUrl[..lastSplitter];
                parametersString = queryUrl[(lastSplitter + 1)..];
            }
            else
            {
                endpoint = queryUrl;
                parametersString = string.Empty;
            }
        }
        private static void SplitMessageBody(string requestMessage, out string requestHeader, out string requestBody)
        {
            int index = requestMessage.IndexOf("\n\n");
            if (index != -1)
            {
                requestHeader = requestMessage[..index];
                requestBody = requestMessage[(index + 1)..];
            }
            else
            {
                requestHeader = requestMessage;
                requestBody = string.Empty;
            }            
        }
        #endregion

        #region Helpers
        public static int FindNextFreeTcpPort()
        {
            TcpListener listener = new(IPAddress.Loopback, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }
        #endregion
    }

    public class NotFoundReplyHandler
    {
        public static void HandleReply(out string body, out string replyHeader)
        {
            body = $"""
                    <!DOCTYPE html>
                    <html lang="en">

                    <head>
                      <meta charset="utf-8">
                      <meta name="viewport" content="width=device-width, initial-scale=1">
                      <title>Page Not Found</title>
                    </head>

                    <body>
                      <h1>Page Doesn't Exist</h1>
                      <p>The page you have requested doesn't exist.</p>
                    </body>

                    </html>
                    """;
            replyHeader = $"""
                    HTTP/1.1 404 Page Not Found
                    Date: {DateTime.Now.ToUniversalTime():r}
                    Connection: close
                    Vary: Origin
                    Cache-Control: public, max-age=0
                    Last-Modified: {DateTime.Now.ToUniversalTime():r}
                    ETag: W/"{Guid.NewGuid().ToString()[..15]}"
                    Content-Type: text/html; charset=UTF-8
                    Content-Length: {body.Length}
                    """;
        }
    }
}
