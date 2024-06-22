namespace Parcel.Data
{
    public static class Distribution
    {
        public static double[] GenerateUniformDistribution(int count = 100)
        {
            Random random = new();
            return Enumerable.Range(0, count).Select(i => random.NextDouble()).ToArray();
        }
        public static double[] GenerateNormalDistribution(int count = 100, double mean = 0, double standardDeviation = 1)
            => GenerateGaussianDistribution(count, mean, standardDeviation);
        /// <summary>
        /// Box-Muller transform
        /// </summary>
        public static double[] GenerateGaussianDistribution(int count = 100, double mean = 0, double standardDeviation = 1)
        {
            Random rand = new(); //reuse this if you are generating many
            return Enumerable.Range(0, count).Select(i =>
            {
                double u1 = 1.0 - rand.NextDouble(); //uniform(0,1] random doubles
                double u2 = 1.0 - rand.NextDouble();
                double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
                double randNormal = mean + standardDeviation * randStdNormal; //random normal(mean,stdDev^2)
                return randNormal;
            }).ToArray();
        }
    }
}
