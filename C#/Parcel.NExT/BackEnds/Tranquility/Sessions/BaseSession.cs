using System.Reflection;
using WebSocketSharp.Server;

namespace Tranquility.Sessions
{
    public abstract class BaseSession : WebSocketBehavior
    {
        #region Helpers
        private string? _SessionName;
        private string SessionName
        {
            get
            {
                if (_SessionName == null)
                    _SessionName = GetType().Name;
                return _SessionName;
            }
        }

        public string Identifier => $"Session {ID[..6]}";
        public void LogInfo(string message)
            => Logging.Info($"[{SessionName}] ({Identifier}) {message}");
        #endregion

        #region Methods
        protected void SendMultiPartReply(string message, int sizeLimit = short.MaxValue / 2)
        {
            if (message.Length < sizeLimit)
                Send(message);
            else
            {
                int segments = (int)Math.Ceiling(message.Length / (double)sizeLimit);
                for (int i = 0; i < segments; i++)
                {
                    string fragmentHeader = $"MULTIPART:{i + 1} {segments} {message.Length}"; // We make this format simple so for Gospel it's less parsing.
                    string fragment = message.Substring(i * sizeLimit, Math.Min(message.Length - i * sizeLimit, sizeLimit));
                    Send($"{fragmentHeader}\n{fragment}");
                    Thread.Sleep(5); // Remark-cz: (Hack) Give front-end some processing time before buffer fills up. We have tested that on localhost, both 100ms, 10ms, 5ms and 1ms seems to work - though it largely depends on how fast frontend (Gospel) can digest it. If this time is to short, Godot might either simply run out of buffer memory and output error, or just drop packets silently. 1ms works on a fast PC, while 5ms is minimal requirement for a slow laptop. Theoratically speaking, in terms of Godot, it depends on FPS - because process() is called per frame.
                }
            }
        }
        #endregion

        #region Helpers
        protected object[]? MarshalStringArgumentsToMethodInputs(ParameterInfo[] parameterInfos, string[] stringValues)
        {
            return stringValues; // TODO: Implement actual logic
        }
        #endregion
    }
}
