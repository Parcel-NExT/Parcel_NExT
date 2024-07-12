/*Try avoiding complex inheritance hierarchy; Keep it single-level*/

namespace Parcel.Graphing.PlotConfigurations
{
    /// <summary>
    /// Provides most common configurations for most if not all plot types
    /// </summary>
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
}
