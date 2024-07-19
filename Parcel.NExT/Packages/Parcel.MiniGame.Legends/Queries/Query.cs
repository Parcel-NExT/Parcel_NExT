using Parcel.Graphing;
using Parcel.MiniGame.Legends.Actions;

namespace Parcel.MiniGame.Legends.Queries
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
        /// <summary>
        /// Show current population distribution within the country. Population census are updated every five years.
        /// </summary>
        public static ActionResult ShowPopulation()
        {
            string[] ageGroups = [
                "Age 0-10",
                "Age 11-20",
                "Age 21-30",
                "Age 31-40",
                "Age 41-50",
                "Age 51-60",
                "Age 61-70",
                "Age 71-80",
                "Age 81-90",
                "Age 91-100",
                "Age 101-110",
            ];
            double[] males = [
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
            ];
            double[] females = [
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
            ];
            return Plot.PopulationPyramid(ageGroups, males, females, new Graphing.PlotConfigurations.PopulationPyramidConfiguration()
            {
                BarSize = 0.4,
                BarGap = 0.02
            });
        }
    }
}
