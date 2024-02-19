namespace Demo
{
    /// <summary>
    /// Single-name single-column numerical history data
    /// </summary>
    public static class HistoryData
    {
        public static double[] SPX500(DateTime? start, DateTime? end)
        {
            HttpClient client = new();
            string csv = client.GetStringAsync(@"https://charles-zhang-investment.github.io/HistoryDataService/TimeSeries/Daily/SPX500.csv").Result;
            return csv
                .Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Skip(1)
                .Select(line => line.Split(',')[5])
                .Select(double.Parse)
                .ToArray();
        }
    }
}
