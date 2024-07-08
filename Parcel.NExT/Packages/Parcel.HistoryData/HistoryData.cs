using Parcel.Data.Types;

namespace Parcel.Data
{
    /// <summary>
    /// Single-name single-column numerical history data
    /// </summary>
    public static class HistoryData
    {
        #region Single Vectors
        public static DateSample[] GetSPXClosingValues(DateTime? start, DateTime? end)
        {
            HttpClient client = new();
            string csv = client.GetStringAsync(@"https://charles-zhang-investment.github.io/HistoryDataService/TimeSeries/Daily/SPX500.csv").Result;
            return csv
                .Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Skip(1)
                .Select(line => line.Split(','))
                .Select(parts => new DateSample(DateTime.Parse(parts[0]), double.Parse(parts[5])))
                .ToArray();
        }
        #endregion

        #region Specific Names
        public static string SPX500(DateTime? start, DateTime? end)
        {
            HttpClient client = new();
            return client.GetStringAsync(@"https://charles-zhang-investment.github.io/HistoryDataService/TimeSeries/Daily/SPX500.csv").Result;
        }
        #endregion

        #region Strongly Typed

        #endregion
    }
}
