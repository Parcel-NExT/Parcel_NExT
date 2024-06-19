using Parcel.CoreEngine.Helpers;
using Parcel.CoreEngine.SemanticTypes;

namespace StandardLibrary.ParcelCore
{
    /// <summary>
    /// Single-name single-column numerical history data
    /// </summary>
    public static class HistoryData
    {
        #region Random Samples
        public static double[] TimeSeries(DateTime? start, DateTime? end)
        {
            HttpClient client = new();
            string csv = client.GetStringAsync(@"https://charles-zhang-investment.github.io/HistoryDataService/TimeSeries/Daily/SPX500.csv").Result;
            return csv
                .SplitLines(true)
                .Skip(1)
                .Select(line => line.Split(',')[5])
                .Select(double.Parse)
                .ToArray();
        }
        #endregion

        #region Specific Names
        public static DataGrid SPX500(DateTime? start, DateTime? end)
        {
            HttpClient client = new();
            string csv = client.GetStringAsync(@"https://charles-zhang-investment.github.io/HistoryDataService/TimeSeries/Daily/SPX500.csv").Result;
            return new DataGrid(csv);
        }
        #endregion
    }
}
