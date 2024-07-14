using Parcel.Types;

namespace Parcel.MiniGame.Legends
{
    /// <summary>
    /// Contains result of an action
    /// </summary>
    public class ActionResult
    {
        #region Contents
        public string? Message { get; set; }
        public Image? Image { get; set; }
        #endregion

        #region Construction
        /// <summary>
        /// Construct empty result
        /// </summary>
        public ActionResult() {}
        /// <summary>
        /// Construct result containing text message
        /// </summary>
        public ActionResult(string message, bool error = false)
        {
            Message = message;
            Error = error;
        }
        /// <summary>
        /// Construct result containing image content
        /// </summary>
        public ActionResult(Image image, bool error = false)
        {
            Image = image;
            Error = error;
        }
        /// <summary>
        /// Construct result containing text message and image content
        /// </summary>
        public ActionResult(string message, Image image, bool error = false)
        { 
            // Remark: Usually we do not use this
            Message = message;
            Image = image;
            Error = error;
        }
        /// <summary>
        /// Implicit convert text message to action result
        /// </summary>
        public static implicit operator ActionResult(string message)
            => new(message);
        /// <summary>
        /// Implicit convert image content to action result
        /// </summary>
        public static implicit operator ActionResult(Image image)
            => new(image);
        #endregion

        #region Status
        /// <summary>
        /// Whether the result indicates error
        /// </summary>
        public bool Error { get; set; } = false;
        /// <summary>
        /// Whether the result indicates success
        /// </summary>
        public bool Success => !Error;
        #endregion
    }

    /// <summary>
    /// Main class for all interactive actions that progresses gameplay.
    /// </summary>
    public static class Action
    {
        #region Turn
        public static ActionResult StartGame()
        {
            Singleton.GameInstance = new GameInstance();
            Singleton.GameInstance.StartGame();
            return "Game started.";
        }
        public static ActionResult CompleteTurn()
        {
            Singleton.GameInstance.ProceedToNextTurn();
            return "Turn completed.";
        }
        #endregion
    }
}
