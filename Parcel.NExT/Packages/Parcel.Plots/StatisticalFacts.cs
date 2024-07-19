using Parcel.Types;

namespace Parcel.Graphing
{
    /// <summary>
    /// Plots statistical facts, used for demonstration purpose
    /// </summary>
    public static class StatisticalFacts
    {
        #region Configurations
        private const int PresetImageWidth = 600;
        private const int PresetImageHeight = 400;
        #endregion

        #region Distributions
        /// <summary>
        /// Plots normal distribution in histogram
        /// </summary>
        public static Image PlotNormalDistribution(out double[] values, int count = 1000, double mean = 0, double standardDeviation = 1)
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
            plot.SavePng(path, PresetImageWidth, PresetImageHeight);
            return new Image(path);
        }
        /// <summary>
        /// Plots uniform distribution in histogram
        /// </summary>
        public static Image PlotUniformDistribution(out double[] values, int count = 1000, double min = 0, double max = 1)
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
            plot.SavePng(path, PresetImageWidth, PresetImageHeight);
            return new Image(path);
        }
        /// <summary>
        /// Plots random walk as scatter plot
        /// </summary>
        public static Image PlotRandomWalk(out double[] values, int count = 1000, double multiply = 1, double offset = 0)
        {
            ScottPlot.Plot plot = new();
            values = ScottPlot.Generate.RandomWalk(count, multiply, offset);
            plot.Add.Scatter(values.Select((v, i) => i).ToArray(), values);

            string path = Plot.GetTempImagePath();
            plot.SavePng(path, PresetImageWidth, PresetImageHeight);
            return new Image(path);
        }
        #endregion

        #region Mathematical Functions
        /// <summary>
        /// Plots sin wave between [-1, 1] as scatter plot
        /// </summary>
        public static Image PlotSinusoidalWave(out double[] values, int count = 100, double multiply = 1, double offset = 0)
        {
            ScottPlot.Plot plot = new();
            values = ScottPlot.Generate.Sin(count, multiply, offset);
            plot.Add.Scatter(values.Select((v, i) => i).ToArray(), values);

            string path = Plot.GetTempImagePath();
            plot.SavePng(path, PresetImageWidth, PresetImageHeight);
            return new Image(path);
        }
        /// <summary>
        /// Plots cos wave between [-1, 1] as scatter plot
        /// </summary>
        public static Image PlotCosineWave(out double[] values, int count = 100, double multiply = 1, double offset = 0)
        {
            ScottPlot.Plot plot = new();
            values = ScottPlot.Generate.Cos(count, multiply, offset);
            plot.Add.Scatter(values.Select((v, i) => i).ToArray(), values);

            string path = Plot.GetTempImagePath();
            plot.SavePng(path, PresetImageWidth, PresetImageHeight);
            return new Image(path);
        }
        #endregion
    }
}
