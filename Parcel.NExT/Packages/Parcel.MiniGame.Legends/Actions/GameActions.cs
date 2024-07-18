using Parcel.CoreEngine.Helpers;
using Parcel.MiniGame.Legends.Data;
using Parcel.MiniGame.Legends.Subsystems;

namespace Parcel.MiniGame.Legends.Actions
{
    /// <summary>
    /// Main class for all interactive actions that progresses gameplay.
    /// </summary>
    public static class GameActions
    {
        #region Turn
        public static ActionResult StartGame()
        {
            Singleton.GameInstance = new GameInstance();
            Singleton.GameInstance.StartGame();

            return EmbeddedResourceHelper.ReadTextResource("Parcel.MiniGame.Legends.Assets.Scripts.WelcomeMessage.md");
        }
        public static ActionResult CompleteTurn()
        {
            Singleton.GameInstance.ProceedToNextTurn();
            return "Turn completed.";
        }
        public static ActionResult Help()
        {
            return EmbeddedResourceHelper.ReadTextResource("Parcel.MiniGame.Legends.Assets.Scripts.Help.md");
        }
        #endregion
    }
}
