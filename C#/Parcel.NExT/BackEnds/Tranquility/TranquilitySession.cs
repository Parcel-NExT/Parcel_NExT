using Parcel.CoreEngine.Helpers;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Tranquility
{

    public class TranquilitySession : WebSocketBehavior
    {
        protected override void OnOpen()
        {
            base.OnOpen();

            Logging.Info("New connection.");
        }
        protected override void OnClose(CloseEventArgs e)
        {
            base.OnClose(e);
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            string message = e.Data;
            Logging.Info(message);

            string[] arguments = message.SplitCommandLineArguments();
            switch (arguments.First())
            {
                case "GET":
                    switch (arguments[1])
                    {
                        case "AvailableModules":
                            Send("12"); // Pesudo
                            break;
                        default:
                            Send("ERROR: Unknown endpoint.");
                            break;
                    }
                    break;
                default:
                    // Echo
                    Send(message);
                    break;
            }
        }
    }
}
