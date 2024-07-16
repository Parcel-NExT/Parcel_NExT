namespace Parcel.MiniGame
{
    /// <summary>
    /// Mini English puzzle game.
    /// </summary>
    public static class PuzzleGame
    {
        private record Entry(string Question, string Puzzle);

        #region States
        private static string? CorrectAnswer = null;
        private static string? PreviousAnswer = null;
        private static int Attempts = 0;
        private const string NewGameCommand = "New Puzzle";
        private static readonly Entry[] Puzzles = [

        ];
        #endregion

        #region Entrance
        /// <summary>
        /// Start a trivia game. Select topic then enter your reply or "New Game" to start new game.
        /// </summary>
        public static string Puzzle(string reply = NewGameCommand)
        {
            if (reply == NewGameCommand)
                return NewGame();

            // Passed
            if (reply == CorrectAnswer)
            {
                CorrectAnswer = null;
                PreviousAnswer = null;
                Attempts = 0;
                return "You are right!";
            }
            else
            {
                Attempts++;
                PreviousAnswer = reply;
                return "Wrong answer";
            }
        }
        #endregion

        #region Routines
        private static string NewGame()
        {
            Random rnd = new();
            int draw = rnd.Next(Puzzles!.Length);
            var puzzle = Puzzles[draw];
            CorrectAnswer = puzzle.Puzzle;
            return puzzle.Question;
        }
        #endregion
    }
}
