using Humanizer;
using Parcel.CoreEngine.Helpers;
using Parcel.CoreEngine.Service;
using Parcel.CoreEngine.Service.Interpretation;
using Parcel.CoreEngine.Service.LibraryProvider;
using Parcel.CoreEngine.Service.Types;
using System.Collections;
using System.Reflection;
using System.Text;
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
        private void HandleMessageJSONStyle(string jsonMessage)
        {
            throw new NotImplementedException();
        }
        private void HandleMessageCLIStyle(string message)
        {
            string[] arguments = message.SplitCommandLineArguments();
            string methodName = arguments.First();
            if (methodName == "Echo")
                // Echo
                Send(message);
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
                    Send(SerializeResult(result));
                else
                    Send(string.Empty);
            }
            else
                Send("ERROR: Unknown endpoint.");
        }
        private string SerializeResult(object result)
        {
            if (result == null)
                return "ERROR: Result is null.";

            var primitiveTypes = new HashSet<Type>
            {
                typeof(bool),
                typeof(byte),
                typeof(sbyte),
                typeof(char),
                typeof(decimal),
                typeof(double),
                typeof(float),
                typeof(int),
                typeof(uint),
                typeof(long),
                typeof(ulong),
                typeof(short),
                typeof(ushort),
                typeof(string)
            };

            // Explicitly handle and serialize everything in pre-defined format: this is the protocol/contract between Tranquility and clients that interface with it
            var resultType = result.GetType();
            // Simply serialize primitives
            if (primitiveTypes.Contains(resultType))
                return result.ToString()!;
            // Serialize collections
            else if (resultType.IsGenericType && resultType.IsAssignableTo(typeof(IEnumerable))
                && resultType.GetGenericArguments().Length == 1 && primitiveTypes.Contains(resultType.GenericTypeArguments.First()))
            {
                var elements = (IEnumerable<object>)result;
                return string.Join("\n", elements.Select(r => r.ToString()));
            }
            else if (resultType.IsArray && primitiveTypes.Contains(resultType.GetElementType()!))
            {
                List<object> elements = [];
                foreach (var e in (Array)result)
                    elements.Add(e);

                return string.Join("\n", elements.Select(r => r.ToString()));
            }
            // Serialize plain string dictionaries
            else if (resultType == typeof(Dictionary<string, string[]>))
                return SerializeFlatStringArrayDictionaryStructure((Dictionary<string, string[]>)result);
            else if (resultType == typeof(Dictionary<string, string>))
                return SerializeFlatStringDictionaryStructure((Dictionary<string, string>)result);
            else if (resultType == typeof(Dictionary<string, SimplexString>))
                return SerializeFlatSimplexStringDictionaryStructure((Dictionary<string, SimplexString>)result);

            // Serialize serializable Parcel-specific types
            // TODO: Serialize Payload, and MetaInstructions
            throw new NotImplementedException("Unrecognized object type.");
        }
        private static string SerializeFlatStringArrayDictionaryStructure(Dictionary<string, string[]> dictionary)
        {
            // Remark: We intentionally don't use JSON libraries for such simple structure to guarantee predictable behaviors, keep code clean and dependancy free
            // Remark: Notice proper JSON convention uses camelCase for keys
            StringBuilder jsonBuilder = new();
            jsonBuilder.Append("{");
            foreach ((string Key, string[] Values) in dictionary)
            {
                jsonBuilder.Append($"\n  \"{Key.Camelize()}\": [");
                foreach (var value in Values)
                    jsonBuilder.Append($"   \"{value}\",");
                jsonBuilder.Length--; // Remove trailing comma
                jsonBuilder.Append($"],");
            }
            jsonBuilder.Length--; // Remove trailing comma
            jsonBuilder.Append("\n}\n");
            return jsonBuilder.ToString().TrimEnd();
        }
        private static string SerializeFlatStringDictionaryStructure(Dictionary<string, string> dictionary)
        {
            // Remark: We intentionally don't use JSON libraries for such simple structure to guarantee predictable behaviors, keep code clean and dependancy free
            // Remark: Notice proper JSON convention uses camelCase for keys
            StringBuilder jsonBuilder = new();
            jsonBuilder.Append("{");
            foreach ((string Key, string Value) in dictionary)
                jsonBuilder.Append($"\n  \"{Key.Camelize()}\": \"{Value}\",");
            jsonBuilder.Length--; // Remove trailing comma
            jsonBuilder.Append("\n}\n");
            return jsonBuilder.ToString().TrimEnd();
        }
        private static string SerializeFlatSimplexStringDictionaryStructure(Dictionary<string, SimplexString> dictionary)
        {
            // Remark: We intentionally don't use JSON libraries for such simple structure to guarantee predictable behaviors, keep code clean and dependancy free
            // Remark: Notice proper JSON convention uses camelCase for keys
            StringBuilder jsonBuilder = new();
            jsonBuilder.Append("{");
            foreach ((string Key, SimplexString Value) in dictionary)
                jsonBuilder.Append($"\n  \"{Key.Camelize()}\": {Value.ToJSONString()},");
            jsonBuilder.Length--; // Remove trailing comma
            jsonBuilder.Append("\n}\n");
            return jsonBuilder.ToString().TrimEnd();
        }
        #endregion
    }
}
