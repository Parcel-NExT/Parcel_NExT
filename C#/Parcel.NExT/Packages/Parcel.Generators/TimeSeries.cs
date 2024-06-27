using Parcel.Data;

namespace Parcel.Generators
{
    public static class TimeSeries
    {
        /// <summary>
        /// Uses a random walk and assume normal distribution of returns
        /// </summary>
        public static (DateTime Date, double value)[] GenerateRandomTimeSeries(DateTime? start = null, DateTime? end = null, double startValue = 100, double returnMean = 0.1, double returnStandardDeviation = 15.25)
        {
            start ??= new DateTime(1970, 1, 1);
            end ??= new DateTime(2010, 12, 31);

            int days = (int)(end.Value - start.Value).TotalDays;
            DateTime[] dates = Enumerable.Range(0, days).Select(d => start.Value.AddDays(d)).ToArray();
            double[] returns = Distribution.GenerateNormalDistribution(days - 1, returnMean, returnStandardDeviation); // We have (TotalDays - 1) returns.

            double currentValue = startValue;
            return [(start.Value, currentValue), ..dates.Skip(1).Select((date, i) =>
            {
                double dailyReturn = returns[i];
                currentValue *= (1 + dailyReturn);
                return (date, currentValue);
            })];
        }
    }
}
