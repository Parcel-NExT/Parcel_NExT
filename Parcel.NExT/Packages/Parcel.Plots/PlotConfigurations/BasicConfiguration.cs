/*Try avoiding complex inheritance hierarchy; Keep it single-level*/

using Parcel.Types;

namespace Parcel.Graphing.PlotConfigurations
{
    /// <summary>
    /// Provides most common configurations for most if not all plot types
    /// </summary>
    /// <remarks>
    /// Notice all those values will be provided on the GUI, likely as primitive inputs, so they cannot be null.
    /// </remarks>
    /// <references>
    /// Default colors: https://scottplot.net/cookbook/4.1/colors/
    /// </references>
    public class BasicConfiguration
    {
        public int ImageWidth { get; set; } = 600;
        public int ImageHeight { get; set; } = 400;
    }

    public sealed class ScatterPlotConfiguration : BasicConfiguration
    {
        public string Title { get; set; } = string.Empty;
        public string XAxis { get; set; } = string.Empty;
        public string YAxis { get; set; } = string.Empty;
        public string Legend { get; set; } = string.Empty;
        public Color Color { get; set; } = Color.Parse("#1F77B4FF");
    }
    public sealed class LinePlotConfiguration : BasicConfiguration
    {
        public string Title { get; set; } = string.Empty;
        public string XAxis { get; set; } = string.Empty;
        public string YAxis { get; set; } = string.Empty;
        public string Legend { get; set; } = string.Empty;
    }
    public sealed class ScatterPlotTwoAxesConfiguration : BasicConfiguration
    {
        public string Title { get; set; } = string.Empty;
        public string XAxis { get; set; } = string.Empty;
        public string YAxis1 { get; set; } = string.Empty;
        public string YAxis2 { get; set; } = string.Empty;
        public string Legend1 { get; set; } = string.Empty;
        public string Legend2 { get; set; } = string.Empty;
    }
    public sealed class HisogramConfiguration : BasicConfiguration
    {
        public string Title { get; set; } = string.Empty;
        public string XAxis { get; set; } = string.Empty;
        public string YAxis { get; set; } = string.Empty;
        public int HisogramBars { get; set; } = 400;
    }
    public sealed class PopulationPyramidConfiguration : BasicConfiguration
    {
        public double BarSize { get; set; } = 0.8;
        public Color[]? AgeGroupColors { get; set; } = null;
        public bool Frameless { get; set; } = false;
    }
}
