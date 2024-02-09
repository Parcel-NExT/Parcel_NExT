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
        }
    }
}
