namespace Parcel.Graphing
{
    public static class Plot
    {
        public static string Scatter(double[] x, double[] y, int imageWidth = 600, int imageHeight = 400, string title = "", string xAxis = "", string yAxis = "", string legend = "")
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
        public static string ScatterTwoAxes(double[] x, double[] y1, double[]y2, int imageWidth = 600, int imageHeight = 400, string title = "", string xAxis = "", string yAxis1 = "", string yAxis2 = "", string legend1 = "", string legend2 = "")
        {
            ScottPlot.Plot plot = new();
            var sig1 = plot.Add.Scatter(x, y1);
            var sig2 = plot.Add.Scatter(x, y2);
            if (!string.IsNullOrEmpty(legend1))
            {
                plot.Legend.IsVisible = true;
                sig1.LegendText = legend1;
            }
            if (!string.IsNullOrEmpty(legend2))
            {
                plot.Legend.IsVisible = true;
                sig2.LegendText = legend2;
            }

            if (!string.IsNullOrEmpty(title))
                plot.Title(title);
            if (!string.IsNullOrEmpty(xAxis))
                plot.Axes.Bottom.Label.Text = xAxis;
            if (!string.IsNullOrEmpty(yAxis1))
            {
                sig1.Axes.YAxis = plot.Axes.Left;
                plot.Axes.Right.Label.Text = yAxis1;
            }
            if (!string.IsNullOrEmpty(yAxis2))
            {
                sig2.Axes.YAxis = plot.Axes.Right;
                plot.Axes.Left.Label.Text = yAxis2;
            }

            string path = GetTempImagePath();
            plot.SavePng(path, imageWidth == 0 ? 400 : imageWidth, imageHeight == 0 ? 300 : imageHeight);
            return $"Image://{path}";
        }
        public static string Line(double[] x, double[] y, int imageWidth = 600, int imageHeight = 400, string title = "", string xAxis = "", string yAxis = "", string legend = "")
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
                plot.Axes.Bottom.Label.Text = xAxis;
            if (!string.IsNullOrEmpty(yAxis))
                plot.Axes.Left.Label.Text = yAxis;

            string path = GetTempImagePath();
            plot.SavePng(path, imageWidth == 0 ? 400 : imageWidth, imageHeight == 0 ? 300 : imageHeight);
            return $"Image://{path}";
        }
        public static string Histogram(double[] v, int imageWidth = 600, int imageHeight = 400, string title = "", string xAxis = "", string yAxis = "", int histogramBars = 400)
        {
            ScottPlot.Plot plot = new();
            
            ScottPlot.Statistics.Histogram hist = new(min: v.Min(), max: v.Max(), binCount: histogramBars);
            hist.AddRange(v);
            plot.Add.Bars(values: hist.Counts, positions: hist.Bins);

            if (!string.IsNullOrEmpty(title))
                plot.Title(title);
            if (!string.IsNullOrEmpty(xAxis))
                plot.Axes.Bottom.Label.Text = xAxis;
            if (!string.IsNullOrEmpty(yAxis))
                plot.Axes.Left.Label.Text = yAxis;

            string path = GetTempImagePath();
            plot.SavePng(path, imageWidth == 0 ? 400 : imageWidth, imageHeight == 0 ? 300 : imageHeight);
            return $"Image://{path}";
        }

        private static string GetTempImagePath()
            => Path.GetTempPath() + Guid.NewGuid().ToString() + ".png";
    }
}
