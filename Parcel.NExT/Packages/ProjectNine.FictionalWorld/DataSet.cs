using Parcel.Data;
using Parcel.Neo.Base.Toolboxes.Generator;
using Parcel.Types;

namespace ProjectNine.FictionalWorld
{
    /// <summary>
    /// A hybrid real-procedural dataset
    /// </summary>
    public static class DataSet
    {
        public static DataGrid GetPopulationData()
        {
            DataGrid dataGrid = new DataGrid("Population Data", new DataColumn[]
            {
                new DataColumn("Name"),
                new DataColumn("Age"),
                new DataColumn("Date of Birth"),
                new DataColumn("Race"),
            });

            // Seeds
            int count = 100;
            var rand = new Random();
            DateTime currentDate = DateTime.Today;

            // Sources
            string[] races = ["Race A", "Race B", "Undefined"];

            // Pre-generation
            string[] names = RandomNames.GetRandomEnglishNames(count);
            
            // Procedural generation
            for (int i = 0; i < count; i++)
            {
                int age = rand.Next(1, 69);
                dataGrid.AddRow(names[i], age, currentDate.AddYears(-age), RandomChoices.Pick(races));
            }

            return dataGrid;
        }
    }
}
