using Parcel.CoreEngine.Helpers;
using Parcel.Types;
using Parcel.Web;
using System.Globalization;

namespace Parcel.Services
{
    /// <summary>
    /// Provides helper functions to fetch data using Yahoo Finance web API service.
    /// </summary>
    public static class YahooFinanceHelper
    {
        /// <summary>
        /// Fetch time series data as CSV from Yahoo Finance. This uses https://query1.finance.yahoo.com/v7/finance/download/ API end point.
        /// </summary>
        public static string YahooFinance(string symbol, DateTime startDate, DateTime endDate, string interval)
        {
            static string ConvertTimeFormat(DateTime input)
            {
                input = input.Date; // Clear out time, set to 0
                string timeStamp = (input - new DateTime(1970, 01, 01)).TotalSeconds.ToString(CultureInfo.InvariantCulture);
                return timeStamp;
            }

            Dictionary<string, string> validIntervals = new()
            {
                {"month", "1mo"},
                {"day", "1d"},
                {"week", "1w"},
                {"year", "1y"},
            };
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("Empty symbol.");
            if (string.IsNullOrWhiteSpace(interval))
                throw new ArgumentException("Empty interval.");
            if (startDate > endDate)
                throw new ArgumentException("Wrong input dates: end date is earlier than start date.");
            if (endDate > DateTime.Now)
                throw new ArgumentException("Wrong end date: end date exceed current date.");
            if (symbol.Length > 5)
                throw new ArgumentException($"Invalid symbol: {symbol}; Symbol length must be smaller than 5.");
            if (!validIntervals.Keys.Contains(interval.ToLower()))
                throw new ArgumentException($"Unrecognized interval: {interval}. Valid intervals: {string.Join(", ", validIntervals.Keys)}");

            string startTime = ConvertTimeFormat(startDate);
            string endTime = ConvertTimeFormat(endDate);
            string intervalSymbol = validIntervals[interval.ToLower()];
            string csvUrl = // Remark: In the past the unaccessible entity error was caused by a typo in the url, not caused by UNIX timestamp; The server is able to handle quite generic timestamp
                $"https://query1.finance.yahoo.com/v7/finance/download/{symbol}?period1={startTime}&period2={endTime}&interval={intervalSymbol}&events=history&includeAdjustedClose=true";
            string csvText = RESTAPILean.Get(csvUrl);
            return csvText;
        }
        /// <summary>
        /// Fetch time series data as DataGrid from Yahoo Finance. This uses https://query1.finance.yahoo.com/v7/finance/download/ API end point.
        /// </summary>
        public static DataGrid YahooFinanceDataGrid(string symbol, DateTime startDate, DateTime endDate, string interval)
        {
            string csvText = YahooFinance(symbol, startDate, endDate, interval);
            return new DataGrid(symbol, CSVHelper.ParseCSV(csvText, out string[]? headers, true), headers);
        }
    }
}
