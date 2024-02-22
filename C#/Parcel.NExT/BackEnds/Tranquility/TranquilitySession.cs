using Parcel.CoreEngine.Conversion;
using Parcel.CoreEngine.Helpers;
using Parcel.CoreEngine.Service;
using Parcel.CoreEngine.Service.Interpretation;
using Parcel.CoreEngine.Service.LibraryProvider;
using System.Reflection;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Tranquility
{
    public record ServiceEndpoint(ServiceProvider Provider, MethodInfo Method);
    public class TranquilitySession : WebSocketBehavior
    {
        #region States
        private Dictionary<string, ServiceEndpoint>? _AvailableEndPoints; // TODO: This could be static and global
        private List<ServiceProvider>? _ServiceProviders;
        #endregion

        #region Helpers
        public string Identifier => $"Session {ID[..6]}";
        public void LogInfo(string message)
            => Logging.Info($"({Identifier}) {message}");
        #endregion

        #region Framework Functions
        protected override void OnOpen()
        {
            base.OnOpen();

            LogInfo("New connection.");

            _ServiceProviders = [new LibraryProviderServices(), new InterpolationServiceProvider()];
            _AvailableEndPoints = [];
            foreach (var provider in _ServiceProviders)
            {
                string providerName = provider.GetType().Name;
                Dictionary<string, MethodInfo> availableServices = provider.GetAvailableServices();
                foreach (KeyValuePair<string, MethodInfo> service in availableServices)
                    _AvailableEndPoints.TryAdd($"{providerName}.{service.Key}", new(provider, service.Value));
            }
        }
        protected override void OnClose(CloseEventArgs e)
        {
            base.OnClose(e);
            LogInfo("Session ended.");
        }
        protected override void OnMessage(MessageEventArgs e)
        {
            string message = e.Data;
            LogInfo(message);
            HandleMessage(message);
        }
        #endregion

        #region Message Handling
        private void HandleMessage(string message)
        {
            const string jsonToken = "JSON\n";

            // Tranquility handles to kinds of messages: single-line CLI style, and multi-line JSON style
            if (message.StartsWith(jsonToken))
                HandleMessageJSONStyle(message.Substring(jsonToken.Length));
            else
                HandleMessageCLIStyle(message);
        }
        #endregion

        #region Routines
        private void SendMultiPartReply(string message, int sizeLimit = short.MaxValue / 2)
        {
            if (message.Length < sizeLimit)
                Send(message);
            else
            {
                int segments = (int)Math.Ceiling(message.Length / (double)sizeLimit);
                for (int i = 0; i < segments; i++)
                {
                    string fragmentHeader = $"MULTIPART:{i+1} {segments} {message.Length}"; // We make this format simple so for Gospel it's less parsing.
                    string fragment = message.Substring(i * sizeLimit, Math.Min(message.Length - i * sizeLimit, sizeLimit));
                    Send($"{fragmentHeader}\n{fragment}");
                    Thread.Sleep(5); // Remark-cz: (Hack) Give front-end some processing time before buffer fills up. We have tested that on localhost, both 100ms, 10ms, 5ms and 1ms seems to work - though it largely depends on how fast frontend (Gospel) can digest it. If this time is to short, Godot might either simply run out of buffer memory and output error, or just drop packets silently. 1ms works on a fast PC, while 5ms is minimal requirement for a slow laptop.
                }
            }
        }
        private void HandleMessageJSONStyle(string jsonMessage)
        {
            IDictionary<string, object>? json = (IDictionary<string, object>)SimpleJson.SimpleJson.DeserializeObject(jsonMessage);
            string methodName = (string)json["endPoint"];
            if (_AvailableEndPoints!.TryGetValue(methodName, out ServiceEndpoint? endPoint))
            {
                var provider = endPoint.Provider;
                var methodInfo = endPoint.Method;

                object? result;
                if (methodInfo.GetParameters().Length == 0)
                    result = methodInfo.Invoke(provider, null);
                else
                    result = methodInfo.Invoke(provider, methodInfo.GetParameters().Select(p => p.Name).Select(k => json[k]).ToArray());

                if (result != null)
                    SendMultiPartReply(StringTypeConverter.SerializeResult(result));
                else
                    SendMultiPartReply(string.Empty);
            }
            else
                SendMultiPartReply("ERROR: Unknown endpoint.");
        }
        private void HandleMessageCLIStyle(string message)
        {
            string[] arguments = message.SplitCommandLineArguments();
            string methodName = arguments.First();
            if (methodName == "Echo")
                // Echo
                SendMultiPartReply(message);
            else if (_AvailableEndPoints!.TryGetValue(methodName, out ServiceEndpoint? endPoint))
            {
                var provider = endPoint.Provider;
                var methodInfo = endPoint.Method;

                object? result;
                if (methodInfo.GetParameters().Length == 0)
                    result = methodInfo.Invoke(provider, null);
                else
                    result = methodInfo.Invoke(provider, arguments.Skip(1).ToArray());

                if (result != null)
                    SendMultiPartReply(StringTypeConverter.SerializeResult(result));
                else
                    SendMultiPartReply(string.Empty);
            }
            else
                SendMultiPartReply("ERROR: Unknown endpoint.");
        }
        #endregion
    }
}
