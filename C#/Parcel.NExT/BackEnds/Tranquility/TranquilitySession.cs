using Parcel.CoreEngine.Helpers;
using Parcel.CoreEngine.Service.LibraryProvider;
using System.Collections;
using System.Reflection;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Tranquility
{
    public class TranquilitySession : WebSocketBehavior
    {
        #region States
        private Dictionary<string, MethodInfo>? _AvailableEndPoints; // TODO: This could be static and global
        private LibraryProviderServices? _ServiceProvider;
        #endregion

        #region Helpers
        public string Identifier => $"Session {ID.Substring(0, 6)}";
        public void LogInfo(string message)
            => Logging.Info($"({Identifier}) {message}");
        #endregion

        #region Framework Functions
        protected override void OnOpen()
        {
            base.OnOpen();

            LogInfo("New connection.");

            _ServiceProvider = new LibraryProviderServices();
            _AvailableEndPoints = _ServiceProvider.GetAvailableServices();
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
            string[] arguments = message.SplitCommandLineArguments();
            string methodName = arguments.First();
            if (methodName == "Echo")
                // Echo
                Send(message);
            else if (_AvailableEndPoints.ContainsKey(methodName))
            {
                var methodInfo = _AvailableEndPoints[methodName];
                
                object? result = null;
                if (methodInfo.GetParameters().Length == 0)
                    result = methodInfo.Invoke(_ServiceProvider, null);
                else
                    result = methodInfo.Invoke(_ServiceProvider, arguments.Skip(1).ToArray());

                if (result != null)
                    Send(SerializeResult(result));
                else
                    Send(string.Empty);
            }
            else
                Send("ERROR: Unknown endpoint.");
        }
        #endregion

        #region Routines
        private string SerializeResult(object result)
        {
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
            // Serialize serializable Parcel-specific types
            // TODO: Serialize Payload, and MetaInstructions
            throw new NotImplementedException("Unrecognized object type.");
        }
        #endregion
    }
}
