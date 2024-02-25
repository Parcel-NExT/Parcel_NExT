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
    }
}
