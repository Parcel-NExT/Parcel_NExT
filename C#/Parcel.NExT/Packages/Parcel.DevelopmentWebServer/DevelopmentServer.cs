using System.Net;
using System.Net.Sockets;
using System.Text;
using static Parcel.Infrastructure.EndpointDefinition;

namespace Parcel.Infrastructure
{
    public record EndpointResponse(string Content, int StatusCode, string ContentType)
    {
        #region Accessor
        public int Length => Content.Length;
        #endregion

        #region Implicit Constructor
        public static implicit operator EndpointResponse(string body) => new(body, 200, DetermineHeuristicType(body));
        #endregion

        #region Helper
        private static string DetermineHeuristicType(string body)
        {
            if (body.StartsWith("<!DOCTYPE html"))
                return MIMETypeNames.TextHtml;
            else return MIMETypeNames.TextPlain;
        }
        #endregion
    }
    public class EndpointDefinition
    {
        #region Constructors
        public EndpointDefinition(string endpoint, EndpointHandler handler)
        {
            Endpoint = endpoint;
            Handler = handler;
        }
        public EndpointDefinition(string method, string endpoint, EndpointHandler handler)
        {
            Method = method;
            Endpoint = endpoint;
            Handler = handler;
        }
        #endregion

        #region Properties
        public string Method { get; } = "Get";
        public string Endpoint { get; }
        public EndpointHandler Handler { get; }
        public string EndpointIdentifier => $"{Method} {Endpoint}";
        #endregion

        #region Constants
        public const string GETMethod = "GET";
        public const string POSTMethod = "POST";
        public const string PUTMethod = "PUT";
        #endregion

        #region Types
        public delegate EndpointResponse EndpointHandler(Dictionary<string, string> Parameters, string Body);
        #endregion
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
            Endpoints = endpoints?.ToDictionary(e => e.EndpointIdentifier, e => e.Handler) ?? null;
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
        public static DevelopmentServer StartServer(EndpointDefinition[]? endpoints, int? port = null)
        {
            DevelopmentServer server = new(endpoints);
            if (port == null)
                server.Start(FindNextFreeTcpPort());
            else server.Start(port.Value);
            return server;
        }
        public static DevelopmentServer StartServerInNewThread(EndpointDefinition[]? endpoints, int? port = null)
        {
            DevelopmentServer server = new(endpoints);
            if (port == null)
                new Thread(() => server.Start(FindNextFreeTcpPort())) { IsBackground = true }.Start();
            else new Thread(() => server.Start(port.Value)) { IsBackground = true }.Start();
            return server;
        }
        public void Start(int port)
        {
            TcpListener server = new(IPAddress.Any, port);
            server.Start();

            Port = port;
            string[] addresses = [..GetLocalIPAddresses(), .. Dns.Resolve("localhost").AddressList.Select(addr => addr.MapToIPv4().ToString())];
            ServerAddress = $"{string.Join(";", addresses.Distinct().Select(addr => $"http://{addr}:{port}"))};http://localhost:{port}"; // TODO: Show actual IP address

            // Main server loop
            while (true) // TODO: Remark: At the moment we are handling in disconnected mode
            {
                // Wait for connection
                TcpClient client = server.AcceptTcpClient();
                new Thread(() => HandleClient(client)) { IsBackground = true }.Start();
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
        private byte[] MakeReply(string requestMessage, out EndpointResponse replyContent, out string replyHeader)
        {
            SplitMessageBody(requestMessage, out string requestHeader, out string requestBody);
            Dictionary<string, string> headerFields = GetHeaders(requestHeader, out string endpointIdentifier, out string parametersString);
            Dictionary<string, string> queryParameters = ParseQueries(parametersString);

            string replyMessage = string.Empty;
            if (Endpoints?.TryGetValue(endpointIdentifier, out EndpointHandler? handler) ?? false)
            {
                try
                {
                    replyMessage = HandleGenericRequest(handler, queryParameters, requestBody, out replyContent, out replyHeader);
                }
                catch (Exception e)
                {
                    replyMessage = HandleApplicationException(e, queryParameters, requestBody, out replyContent, out replyHeader);
                }
            }
            else
                replyMessage = HandleNotFound(out replyContent, out replyHeader);

            byte[] replyData = Encoding.UTF8.GetBytes(replyMessage);
            return replyData;
        }
        private static string HandleGenericRequest(EndpointHandler handler, Dictionary<string, string> queryParameters, string requestBody, out EndpointResponse replyContent, out string replyHeader)
        {
            replyContent = handler(queryParameters, requestBody);
            replyHeader = $"""
                    HTTP/1.1 {replyContent.StatusCode} OK
                    Date: {DateTime.Now.ToUniversalTime():r}
                    Connection: close
                    Vary: Origin
                    Cache-Control: public, max-age=0
                    Last-Modified: {DateTime.Now.ToUniversalTime():r}
                    ETag: W/"{Guid.NewGuid().ToString()[..15]}"
                    Content-Type: {replyContent.ContentType}; charset=UTF-8
                    Content-Length: {replyContent.Length}
                    """;
            return $"""
                   {replyHeader}
                   
                   {replyContent.Content}
                   """;
        }
        private string HandleApplicationException(Exception e, Dictionary<string, string> queryParameters, string requestBody, out EndpointResponse replyContent, out string replyHeader)
        {
            ApplicationRuntimeExceptionReplyHandler.HandleReply(e, queryParameters, requestBody, out replyContent, out replyHeader);
            return $"""
                   {replyHeader}
                   
                   {replyContent.Content}
                   """;
        }
        private static string HandleNotFound(out EndpointResponse replyContent, out string replyHeader)
        {
            NotFoundReplyHandler.HandleReply(out replyContent, out replyHeader);
            return $"""
                   {replyHeader}
                   
                   {replyContent.Content}
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
        private static Dictionary<string, string> GetHeaders(string requestHeader, out string endpointIdentifier, out string parametersString)
        {
            string[] headerLines = requestHeader.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            string firstLine = headerLines[0];
            ParseFirstLine(firstLine, out string method, out string endpoint, out parametersString, out string version);
            endpointIdentifier = $"{method} {endpoint}";

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
            int index = requestMessage.IndexOf("\r\n\r\n"); // Remark: Notice the protocol specifically defines \r\n as header line break style, see https://stackoverflow.com/questions/5757290/http-header-line-break-style
            if (index != -1)
            {
                requestHeader = requestMessage[..index];
                requestBody = requestMessage[(index + 1)..].TrimStart();
            }
            else
            {
                requestHeader = requestMessage;
                requestBody = string.Empty;
            }            
        }
        #endregion

        #region Helpers
        private static int FindNextFreeTcpPort()
        {
            TcpListener listener = new(IPAddress.Loopback, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }
        public static IEnumerable<string> GetLocalIPAddresses()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    yield return ip.ToString();
        }
        #endregion
    }

    public class NotFoundReplyHandler
    {
        public static void HandleReply(out EndpointResponse replyContent, out string replyHeader)
        {
            string body = $"""
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
                    Content-Type: {MIMETypeNames.TextHtml}; charset=UTF-8
                    Content-Length: {body.Length}
                    """;
            replyContent = new EndpointResponse(body, 404, MIMETypeNames.TextHtml);
        }
    }
    public class ApplicationRuntimeExceptionReplyHandler
    {
        public static void HandleReply(Exception exception, Dictionary<string, string> queryParameters, string requestBody, out EndpointResponse replyContent, out string replyHeader)
        {
            // TODO: Add richer debug information e.g. http method (GET/POST), request header
            // TODO: Refine context ifnormation regarding query parameters
            string body = $"""
                    <!DOCTYPE html>
                    <html lang="en">

                    <head>
                      <meta charset="utf-8">
                      <meta name="viewport" content="width=device-width, initial-scale=1">
                      <title>Runtime Exception</title>
                    </head>

                    <body>
                      <h1>Application Run Into An Unhandled Exception</h1>
                      <p>
                        The program runs into an unhandled exception: <strong>{exception.Message}<strong/>
                        <br/>
                        This is likely due to the lack of foresight and ommission of exception handling at the summoning site.
                      </p>
                      <code>
                      {exception.StackTrace}
                      </code>
                      <br/>

                      <h3>Context</h3>
                      <p>This happens with the following request:</p>
                      <code>
                      {requestBody}
                      </code>
                      </body>

                    </html>
                    """;
            replyHeader = $"""
                    HTTP/1.1 500 Page Not Found
                    Date: {DateTime.Now.ToUniversalTime():r}
                    Connection: close
                    Vary: Origin
                    Cache-Control: public, max-age=0
                    Last-Modified: {DateTime.Now.ToUniversalTime():r}
                    ETag: W/"{Guid.NewGuid().ToString()[..15]}"
                    Content-Type: {MIMETypeNames.TextHtml}; charset=UTF-8
                    Content-Length: {body.Length}
                    """;
            replyContent = new EndpointResponse(body, 500, MIMETypeNames.TextHtml);
        }
    }
}
