namespace Parcel.MiniGame.Legends
{
    /// <summary>
    /// Main class for all queries
    /// </summary>
    public static class Query
    {
        /// <summary>
        /// Provides (multi-paragraph) summary of current game status, world s tatus (as known to the user) and country status.
        /// </summary>
        public static string Summary()
        {
            return $"""
                Country Name: Bian Jing
                Cities: 3
                  Bian Jing
                  Ling
                  Xia
                Heros: 20
                Country Population: 75,000
                """;
        }
    }
}
