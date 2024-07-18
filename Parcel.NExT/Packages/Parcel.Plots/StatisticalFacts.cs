using Parcel.Types;

namespace Parcel.Graphing
{
    /// <summary>
    /// Plots statistical facts, used for demonstration purpose
    /// </summary>
    public static class StatisticalFacts
    {
        #region Distributions
        /// <summary>
        /// Plots normal distribution in histogram
        /// </summary>
        public static Image NormalDistribution(out double[] values, int count = 1000, double mean = 0, double standardDeviation = 1)
        {
            ScottPlot.Plot plot = new();
            values = ScottPlot.Generate.RandomNormal(count, mean, standardDeviation);

            ScottPlot.Statistics.Histogram hist = new(min: values.Min(), max: values.Max(), binCount: 20, true, false);
            hist.AddRange(values);
            ScottPlot.Plottables.BarPlot barPlot = plot.Add.Bars(values: hist.Counts, positions: hist.BinCenters);
            foreach (var bar in barPlot.Bars)
                bar.Size = hist.BinSize;

            plot.Title("Demo Normal Distribution");

            string path = Plot.GetTempImagePath();
            plot.SavePng(path, 400, 300);
            return new Image(path);
        }
        /// <summary>
        /// Plots uniform distribution in histogram
        /// </summary>
        public static Image UniformDistribution(out double[] values, int count = 1000, double min = 0, double max = 1)
        {
            ScottPlot.Plot plot = new();
            values = ScottPlot.Generate.RandomSample(count, min, max);

            ScottPlot.Statistics.Histogram hist = new(min: values.Min(), max: values.Max(), binCount: 20, true, false);
            hist.AddRange(values);
            ScottPlot.Plottables.BarPlot barPlot = plot.Add.Bars(values: hist.Counts, positions: hist.BinCenters);
            foreach (var bar in barPlot.Bars)
                bar.Size = hist.BinSize;

            plot.Title("Demo Uniform Distribution");

            string path = Plot.GetTempImagePath();
            plot.SavePng(path, 400, 300);
            return new Image(path);
        }
        /// <summary>
        /// Plots random walk as scatter plot
        /// </summary>
        public static Image RandomWalk(out double[] values, int count = 1000, double min = 0, double max = 1)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
