namespace Parcel.Graphing
{
    public static class Plot
    {
        public static string Scatter(double[] x, double[] y, int imageWidth, int imageHeight, string title, string xAxis, string yAxis, string legend)
        {
            ScottPlot.Plot plot = new();
            var s = plot.Add.Scatter(x, y);
            if (!string.IsNullOrEmpty(legend))
            {
                plot.Legend.IsVisible = true;
                s.LegendText = legend;
            }

            if (!string.IsNullOrEmpty(title))
                plot.Title(title);
            if (!string.IsNullOrEmpty(xAxis))
                plot.Axes.Left.Label.Text = xAxis;
            if (!string.IsNullOrEmpty(yAxis))
                plot.Axes.Bottom.Label.Text = yAxis;

            string path = GetTempImagePath();
            plot.SavePng(path, imageWidth == 0 ? 400 : imageWidth, imageHeight == 0 ? 300 : imageHeight);
            return $"Image://{path}";
        }
        public static string Line(double[] x, double[] y, int imageWidth, int imageHeight, string title, string xAxis, string yAxis, string legend)
        {
            ScottPlot.Plot plot = new();
            if (!string.IsNullOrEmpty(legend))
            {
                var s = plot.Add.Scatter(x, y);
                s.LegendText = legend;
            }

            if (!string.IsNullOrEmpty(title))
                plot.Title(title);
            if (!string.IsNullOrEmpty(xAxis))
                plot.Axes.Left.Label.Text = xAxis;
            if (!string.IsNullOrEmpty(yAxis))
                plot.Axes.Bottom.Label.Text = yAxis;

            string path = GetTempImagePath();
            plot.SavePng(path, imageWidth == 0 ? 400 : imageWidth, imageHeight == 0 ? 300 : imageHeight);
            return $"Image://{path}";
        }
        public static string Histogram(double[] v, int imageWidth, int imageHeight, string title, string xAxis, string yAxis, int histogramBars)
        {
            ScottPlot.Plot plot = new();
            
            ScottPlot.Statistics.Histogram hist = new(min: v.Min(), max: v.Max(), binCount: histogramBars);
            hist.AddRange(v);
            plot.Add.Bars(values: hist.Counts, positions: hist.Bins);

            if (!string.IsNullOrEmpty(title))
                plot.Title(title);
            if (!string.IsNullOrEmpty(xAxis))
                plot.Axes.Left.Label.Text = xAxis;
            if (!string.IsNullOrEmpty(yAxis))
                plot.Axes.Bottom.Label.Text = yAxis;

            string path = GetTempImagePath();
            plot.SavePng(path, imageWidth == 0 ? 400 : imageWidth, imageHeight == 0 ? 300 : imageHeight);
            return $"Image://{path}";
        }

        private static string GetTempImagePath()
            => Path.GetTempPath() + Guid.NewGuid().ToString() + ".png";
    }
}
