using Parcel.MiniGame.Legends.Data;

namespace Parcel.MiniGame.Legends
{
    internal static class Singleton
    {
        #region Access
        private static GameInstance? _gameInstance;
        public static GameInstance GameInstance
        {
            get => _gameInstance ?? throw new InvalidOperationException("Game not started.");
            set
            {
                if (_gameInstance != null)
                    throw new InvalidOperationException("Game already started.");
                _gameInstance = value;
            }
        }
        #endregion
    }
}
