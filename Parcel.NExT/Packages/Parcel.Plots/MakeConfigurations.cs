using Parcel.Graphing.PlotConfigurations;
using Parcel.Types;

namespace Parcel.Graphing
{
    /// <remarks>
    /// Codes for this part will be auto-generated based on contents in PLotConfigurations folder.
    /// </remarks>
    public static class MakeConfigurations
    {
        /// <summary>
        /// Create a configuration for ScatterPlot
        /// </summary>
        public static ScatterPlotConfiguration ConfigureScatterPlot(string title = "", string xAxis = "", string yAxis = "", string legend = "", Color? color = null, int imageWidth = Plot.DefaultWidth, int imageHeight = Plot.DefaultHeight)
        {
            color ??= Parcel.Types.Color.Parse("#1F77B4FF");
            return new ScatterPlotConfiguration()
            {
                Title = title,
                XAxis = xAxis,
                YAxis = yAxis,
                Legend = legend,
                Color = color,
                ImageWidth = imageWidth,
                ImageHeight = imageHeight
            };
        }
        /// <summary>
        /// Create a configuration for ScatterPlotMultiSeries
        /// </summary>
        public static ScatterPlotMultiSeriesConfiguration ConfigureScatterPlotMultiSeries(string title = "", string xAxis = "", string yAxis = "", String[]? legends = null, Color[]? colors = null, int imageWidth = Plot.DefaultWidth, int imageHeight = Plot.DefaultHeight)
        {

            return new ScatterPlotMultiSeriesConfiguration()
            {
                Title = title,
                XAxis = xAxis,
                YAxis = yAxis,
                Legends = legends,
                Colors = colors,
                ImageWidth = imageWidth,
                ImageHeight = imageHeight
            };
        }
        /// <summary>
        /// Create a configuration for LinePlot
        /// </summary>
        public static LinePlotConfiguration ConfigureLinePlot(string title = "", string xAxis = "", string yAxis = "", string legend = "", int imageWidth = Plot.DefaultWidth, int imageHeight = Plot.DefaultHeight)
        {

            return new LinePlotConfiguration()
            {
                Title = title,
                XAxis = xAxis,
                YAxis = yAxis,
                Legend = legend,
                ImageWidth = imageWidth,
                ImageHeight = imageHeight
            };
        }
        /// <summary>
        /// Create a configuration for ScatterPlotTwoAxes
        /// </summary>
        public static ScatterPlotTwoAxesConfiguration ConfigureScatterPlotTwoAxes(string title = "", string xAxis = "", string yAxis1 = "", string yAxis2 = "", string legend1 = "", string legend2 = "", int imageWidth = Plot.DefaultWidth, int imageHeight = Plot.DefaultHeight)
        {

            return new ScatterPlotTwoAxesConfiguration()
            {
                Title = title,
                XAxis = xAxis,
                YAxis1 = yAxis1,
                YAxis2 = yAxis2,
                Legend1 = legend1,
                Legend2 = legend2,
                ImageWidth = imageWidth,
                ImageHeight = imageHeight
            };
        }
        /// <summary>
        /// Create a configuration for BarChart
        /// </summary>
        public static BarChartConfiguration ConfigureBarChart(string title = "", string xAxis = "", string yAxis = "", int imageWidth = Plot.DefaultWidth, int imageHeight = Plot.DefaultHeight)
        {

            return new BarChartConfiguration()
            {
                Title = title,
                XAxis = xAxis,
                YAxis = yAxis,
                ImageWidth = imageWidth,
                ImageHeight = imageHeight
            };
        }
        /// <summary>
        /// Create a configuration for Hisogram
        /// </summary>
        public static HisogramConfiguration ConfigureHisogram(string title = "", string xAxis = "", string yAxis = "", int hisogramBars = 10, int imageWidth = Plot.DefaultWidth, int imageHeight = Plot.DefaultHeight)
        {

            return new HisogramConfiguration()
            {
                Title = title,
                XAxis = xAxis,
                YAxis = yAxis,
                HisogramBars = hisogramBars,
                ImageWidth = imageWidth,
                ImageHeight = imageHeight
            };
        }
        /// <summary>
        /// Create a configuration for BubbleChart
        /// </summary>
        public static BubbleChartConfiguration ConfigureBubbleChart(string title = "", string xAxis = "", string yAxis = "", int imageWidth = Plot.DefaultWidth, int imageHeight = Plot.DefaultHeight)
        {
            return new BubbleChartConfiguration()
            {
                Title = title,
                XAxis = xAxis,
                YAxis = yAxis,
                ImageWidth = imageWidth,
                ImageHeight = imageHeight
            };
        }
        /// <summary>
        /// Create a configuration for BubbleChartMultiSeries
        /// </summary>
        public static BubbleChartMultiSeriesConfiguration ConfigureBubbleChartMultiSeries(string title = "", string xAxis = "", string yAxis = "", String[]? legends = null, int imageWidth = Plot.DefaultWidth, int imageHeight = Plot.DefaultHeight)
        {
            return new BubbleChartMultiSeriesConfiguration()
            {
                Title = title,
                XAxis = xAxis,
                YAxis = yAxis,
                Legends = legends,
                ImageWidth = imageWidth,
                ImageHeight = imageHeight
            };
        }
        /// <summary>
        /// Create a configuration for FunnelChart
        /// </summary>
        public static FunnelChartConfiguration ConfigureFunnelChart(string title = "", string xAxis = "", string yAxis = "", String[]? labels = null, int imageWidth = Plot.DefaultWidth, int imageHeight = Plot.DefaultHeight)
        {
            return new FunnelChartConfiguration()
            {
                Title = title,
                XAxis = xAxis,
                YAxis = yAxis,
                Labels = labels,
                ImageWidth = imageWidth,
                ImageHeight = imageHeight
            };
        }
        /// <summary>
        /// Create a configuration for PopulationPyramid
        /// </summary>
        public static PopulationPyramidConfiguration ConfigurePopulationPyramid(string title = "", Double barSize = 0.8, int labelFontSize = 12, Color[]? ageGroupColors = null, Double barGap = 0.1, int imageWidth = Plot.DefaultWidth, int imageHeight = Plot.DefaultHeight)
        {

            return new PopulationPyramidConfiguration()
            {
                Title = title,
                BarSize = barSize,
                LabelFontSize = labelFontSize,
                BarGap = barGap,
                ImageWidth = imageWidth,
                ImageHeight = imageHeight
            };
        }
    }
}
