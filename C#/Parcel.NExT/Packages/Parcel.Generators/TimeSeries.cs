using Parcel.Data;

namespace Parcel.Generators
{
    public static class TimeSeries
    {
        /// <summary>
        /// Uses a random walk and assume normal distribution of returns
        /// </summary>
        public static (DateTime Date, double value)[] GenerateRandomTimeSeries(DateTime start, DateTime end, double startValue, double returnMean, double returnStandardDeviation)
        {
            int days = (int)(end - start).TotalDays;
            DateTime[] dates = Enumerable.Range(0, days).Select(d => start.AddDays(d)).ToArray();
            double[] returns = Distribution.GenerateNormalDistribution(days - 1, returnMean, returnStandardDeviation); // We have (TotalDays - 1) returns.

            double currentValue = startValue;
            return [(start, currentValue), ..dates.Skip(1).Select((date, i) =>
            {
                double dailyReturn = returns[i];
                currentValue *= (1 + dailyReturn);
                return (date, currentValue);
            })];
        }
    }
}
