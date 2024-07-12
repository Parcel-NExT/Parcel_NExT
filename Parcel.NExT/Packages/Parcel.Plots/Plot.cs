using Parcel.Graphing.PlotConfigurations;
using Parcel.Types;

namespace Parcel.Graphing
{
    public static class Plot
    {
        public static Image ScatterPLot(double[] x, double[] y, ScatterPlotConfiguration configurations)
        {
            ScottPlot.Plot plot = new();
            var s = plot.Add.Scatter(x, y);
            if (!string.IsNullOrEmpty(configurations.Legend))
            {
                plot.Legend.IsVisible = true;
                s.LegendText = configurations.Legend;
            }

            if (!string.IsNullOrEmpty(configurations.Title))
                plot.Title(configurations.Title);
            if (!string.IsNullOrEmpty(configurations.XAxis))
                plot.Axes.Left.Label.Text = configurations.XAxis;
            if (!string.IsNullOrEmpty(configurations.YAxis))
                plot.Axes.Bottom.Label.Text = configurations.YAxis;

            string path = GetTempImagePath();
            plot.SavePng(path, configurations.ImageWidth == 0 ? 400 : configurations.ImageWidth, configurations.ImageHeight == 0 ? 300 : configurations.ImageHeight);
            return new Image(path);
        }
        public static Image ScatterPlotTwoAxes(double[] x, double[] y1, double[]y2, ScatterPlotTwoAxesConfiguration configurations)
        {
            ScottPlot.Plot plot = new();
            var sig1 = plot.Add.Scatter(x, y1);
            var sig2 = plot.Add.Scatter(x, y2);
            if (!string.IsNullOrEmpty(configurations.Legend1))
            {
                plot.Legend.IsVisible = true;
                sig1.LegendText = configurations.Legend1;
            }
            if (!string.IsNullOrEmpty(configurations.Legend2))
            {
                plot.Legend.IsVisible = true;
                sig2.LegendText = configurations.Legend2;
            }

            if (!string.IsNullOrEmpty(configurations.Title))
                plot.Title(configurations.Title);
            if (!string.IsNullOrEmpty(configurations.XAxis))
                plot.Axes.Bottom.Label.Text = configurations.XAxis;
            if (!string.IsNullOrEmpty(configurations.YAxis1))
            {
                sig1.Axes.YAxis = plot.Axes.Left;
                plot.Axes.Right.Label.Text = configurations.YAxis1;
            }
            if (!string.IsNullOrEmpty(configurations.YAxis2))
            {
                sig2.Axes.YAxis = plot.Axes.Right;
                plot.Axes.Left.Label.Text = configurations.YAxis2;
            }

            string path = GetTempImagePath();
            plot.SavePng(path, configurations.ImageWidth == 0 ? 400 : configurations.ImageWidth, configurations.ImageHeight == 0 ? 300 : configurations.ImageHeight);
            return new Image(path);
        }
        public static Image LinePlot(double[] x, double[] y, LinePlotConfiguration configurations)
        {
            ScottPlot.Plot plot = new();
            if (!string.IsNullOrEmpty(configurations.Legend))
            {
                var s = plot.Add.Scatter(x, y);
                s.LegendText = configurations.Legend;
            }

            if (!string.IsNullOrEmpty(configurations.Title))
                plot.Title(configurations.Title);
            if (!string.IsNullOrEmpty(configurations.XAxis))
                plot.Axes.Bottom.Label.Text = configurations.XAxis;
            if (!string.IsNullOrEmpty(configurations.YAxis))
                plot.Axes.Left.Label.Text = configurations.YAxis;

            string path = GetTempImagePath();
            plot.SavePng(path, configurations.ImageWidth == 0 ? 400 : configurations.ImageWidth, configurations.ImageHeight == 0 ? 300 : configurations.ImageHeight);
            return new Image(path);
        }
        public static Image Histogram(double[] v, HisogramConfiguration configurations)
        {
            ScottPlot.Plot plot = new();
            
            ScottPlot.Statistics.Histogram hist = new(min: v.Min(), max: v.Max(), binCount: configurations.HisogramBars);
            hist.AddRange(v);
            plot.Add.Bars(values: hist.Counts, positions: hist.Bins);

            if (!string.IsNullOrEmpty(configurations.Title))
                plot.Title(configurations.Title);
            if (!string.IsNullOrEmpty(configurations.XAxis))
                plot.Axes.Bottom.Label.Text = configurations.XAxis;
            if (!string.IsNullOrEmpty(configurations.YAxis))
                plot.Axes.Left.Label.Text = configurations.YAxis;

            string path = GetTempImagePath();
            plot.SavePng(path, configurations.ImageWidth == 0 ? 400 : configurations.ImageWidth, configurations.ImageHeight == 0 ? 300 : configurations.ImageHeight);
            return new Image(path);
        }

        #region Helpers
        private static string GetTempImagePath()
            => Path.GetTempPath() + Guid.NewGuid().ToString() + ".png";
        #endregion
    }
}
