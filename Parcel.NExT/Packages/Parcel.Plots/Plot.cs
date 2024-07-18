using Parcel.Graphing.PlotConfigurations;
using Parcel.Types;

namespace Parcel.Graphing
{
    internal class CustomPalette : ScottPlot.IPalette
    {
        public string Name { get; } = "A Custom Palette";
        public string Description { get; } = "Used when customizing plot colors.";

        public ScottPlot.Color[] Colors { get; set; } = new ScottPlot.Palettes.Category10().Colors; // Default

        public ScottPlot.Color GetColor(int index)
            => Colors[index % Colors.Length];
    }

    public static class Plot
    {
        #region Standard Plots
        public static Image ScatterPlot(double[] x, double[] y, ScatterPlotConfiguration? configurations)
        {
            configurations ??= new();

            ScottPlot.Plot plot = new();
            if (configurations.Color != null)
                plot.Add.Palette = new CustomPalette()
                {
                    Colors = [Convert(configurations.Color)]
                };

            ScottPlot.Plottables.Scatter s = plot.Add.Scatter(x, y);
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
        public static Image ScatterPlotTwoAxes(double[] x, double[] y1, double[] y2, ScatterPlotTwoAxesConfiguration? configurations)
        {
            configurations ??= new();

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
        public static Image LinePlot(double[] x, double[] y, LinePlotConfiguration? configurations)
        {
            configurations ??= new();

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
        public static Image Histogram(double[] values, HisogramConfiguration? configurations)
        {
            configurations ??= new();

            ScottPlot.Plot plot = new();

            ScottPlot.Statistics.Histogram hist = new(min: values.Min(), max: values.Max(), binCount: configurations.HisogramBars, true, false);
            hist.AddRange(values);
            ScottPlot.Plottables.BarPlot barPlot = plot.Add.Bars(values: hist.Counts, positions: hist.BinCenters);
            foreach (var bar in barPlot.Bars)
                bar.Size = hist.BinSize;

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
        #endregion

        #region Standard Charts
        /// <alias>Age Population Chart</alias>
        /// <remarks>Notice a population pyramid devides humans into male and female and might not be a gender neural way to represent diversity within population</remarks>
        public static Image PopulationPyramid(string[] ageGroups, double[] maleData, double[] femaleData, PopulationPyramidConfiguration? configurations)
        {
            // TODO: Move age group outside of drawing frame and well aligned on the left side of the drawing area; Might need to use custom text labels for this

            if (ageGroups == null || maleData == null || femaleData == null)
                throw new ArgumentNullException();
            if (ageGroups.Length != maleData.Length || ageGroups.Length != femaleData.Length)
                throw new ArgumentException("Unmatching data length.");
            if (maleData.Any(d => d < 0))
                throw new ArgumentException("Male data contains negative value.");
            if (femaleData.Any(d => d < 0))
                throw new ArgumentException("Male data contains negative value.");
            configurations ??= new();

            // Reverse order
            ageGroups = ageGroups.Reverse().ToArray();
            maleData = maleData.Reverse().ToArray();
            femaleData = femaleData.Reverse().ToArray();

            ScottPlot.Plot plot = new();
            // Male group bar data
            CreateDataBars(maleData, plot, true, configurations);
            // Female group bar data
            CreateDataBars(femaleData, plot, false, configurations);

            // Set display range
            const int fontSize = 12;
            int margin = ageGroups.Select(s => s.Length).Max() * fontSize/6; // Heuristic scale
            double maleMin = maleData.Max();
            plot.Axes.SetLimitsX(-maleMin - margin, +femaleData.Max() + margin / 2); // Include extra margin to account for label // TODO: At the moment there is no reliable way to determine absolute size
            plot.Layout.Frameless();
            plot.HideGrid();
            // Title
            if (!string.IsNullOrEmpty(configurations.Title))
                plot.Title(configurations.Title);
            // Labels
            for (int i = 0; i < ageGroups.Length; i++)
            {
                string item = ageGroups[i];
                ScottPlot.Plottables.Text text = plot.Add.Text(item, -maleMin - fontSize/3*2, i * (configurations.BarSize + configurations.BarGap));
                text.FontSize = fontSize;
                text.LabelBold = true;
                text.LabelFontColor = ScottPlot.Colors.Black;
                text.LabelAlignment = ScottPlot.Alignment.MiddleRight;
            }
            // Draw division line
            plot.Add.VerticalLine(0, 1, ScottPlot.Colors.Black);

            // Save result
            string path = GetTempImagePath();
            plot.SavePng(path, configurations.ImageWidth, configurations.ImageHeight);
            return new Image(path);

            // Helper
            static void CreateDataBars(double[] data, ScottPlot.Plot plot, bool male, PopulationPyramidConfiguration configuration)
            {
                ScottPlot.Plottables.BarPlot barPlot = plot.Add.Bars(male ? data.Select(d => -d).ToArray() : data);
                int index = 0;
                foreach (ScottPlot.Bar bar in barPlot.Bars)
                {
                    // Set position and width
                    bar.Size = configuration.BarSize;
                    bar.Position = index * (configuration.BarSize + configuration.BarGap);

                    // Set label
                    bar.Label = Math.Abs(bar.Value).ToString();
                    index++;
                }
                // Customize label style
                barPlot.ValueLabelStyle.Bold = true;
                barPlot.ValueLabelStyle.FontSize = 18;
                barPlot.Horizontal = true;
                barPlot.LegendText = male ? "Male" : "Female";
            }
        }
        #endregion

        #region Node Graphs
        public static Image NodeGraph()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Helpers
        internal static string GetTempImagePath()
            => Path.GetTempPath() + Guid.NewGuid().ToString() + ".png";
        private static ScottPlot.Color Convert(Parcel.Types.Color color)
            => ScottPlot.Color.FromHex(color.ToString());
        #endregion
    }
}
