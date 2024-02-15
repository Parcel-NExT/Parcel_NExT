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

        #region Framework Functions
        protected override void OnOpen()
        {
            base.OnOpen();

            Logging.Info("New connection.");

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
            Logging.Info(message);
            HandleMessage(message);
        }
        #endregion

        #region Message Handling
        private void HandleMessage(string message)
        {
            string[] arguments = message.SplitCommandLineArguments();
            string method = arguments.First();
            if (method == "Echo")
                // Echo
                Send(message);
            else if (_AvailableEndPoints.ContainsKey(method))
            {
                object? result = _AvailableEndPoints[method].Invoke(_ServiceProvider, new object[] { arguments.Skip(1).ToArray() });
                Send(result.ToString());
            }
            else
                Send("ERROR: Unknown endpoint.");
        }
        #endregion
    }
}
