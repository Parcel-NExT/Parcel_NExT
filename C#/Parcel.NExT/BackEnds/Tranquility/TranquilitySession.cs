using Parcel.CoreEngine.Helpers;
using Parcel.CoreEngine.Service.LibraryProvider;
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
                    result = methodInfo.Invoke(_ServiceProvider, new object[] { arguments.Skip(1).ToArray() });

                if (result != null)
                    Send(result.ToString());
                else
                    Send(string.Empty);
            }
            else
                Send("ERROR: Unknown endpoint.");
        }
        #endregion
    }
}
