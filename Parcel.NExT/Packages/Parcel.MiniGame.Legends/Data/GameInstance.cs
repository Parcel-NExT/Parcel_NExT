using Parcel.Database;

namespace Parcel.MiniGame.Legends.Data
{
    internal class GameInstance
    {
        #region Database
        private InMemorySQLIte Database { get; set; }
        #endregion

        #region Methods
        public void StartGame()
        {
            if (Database == null)
                Database = new InMemorySQLIte(GetGameLocation());
            throw new InvalidOperationException("Game is already started!");
        }
        public void ProceedToNextTurn()
        {
            // TODO: Call subsystems to perform all kinds of updates
            throw new NotImplementedException();
        }
        #endregion

        #region Helpers
        private static string GetGameLocation()
        {
            string databaseFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Parcel NExT", "Legends", "GameSession.sqlite3");

            if (!File.Exists(databaseFilePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(databaseFilePath)!);
                File.Create(databaseFilePath);
            }

            return databaseFilePath;
        }
        #endregion
    }
}
