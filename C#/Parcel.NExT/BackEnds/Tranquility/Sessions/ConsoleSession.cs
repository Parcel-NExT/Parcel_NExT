using WebSocketSharp;

namespace Tranquility.Sessions
{
    public class ConsoleSession : BaseSession
    {
        #region Registry
        public static HashSet<ConsoleSession> ConsoleRegistry { get; } = [];
        #endregion

        #region Method
        public static void BroadcastMessages(string message)
        {
            foreach (ConsoleSession session in ConsoleRegistry)
                session.Send(message);
        }
        #endregion

        #region Framework Functions
        protected override void OnOpen()
        {
            base.OnOpen();

            LogInfo("New connection.");
            ConsoleRegistry.Add(this);
        }
        protected override void OnClose(CloseEventArgs e)
        {
            base.OnClose(e);
            LogInfo("Session ended.");
            ConsoleRegistry.Remove(this);
        }
        protected override void OnMessage(MessageEventArgs e)
        {
            string message = e.Data;
            LogInfo($"Invalid message: {message}. Console sessions DO NOT expect incoming messages.");
        }
        #endregion
    }
}
