using ImageMagick;
using Parcel.Types;
using System.Numerics;
using Color = Parcel.Types.Color;

namespace Zora.Graphing
{
    public class ReportFormattingParameters
    {
        #region Auxiliary Elements
        public string? ReportTitle { get; set; } = null;
        public string FontFamily { get; set; } = "Calibri";
        #endregion

        #region Formatting
        public int Margin { get; set; } = 20;
        public double LabelFontSize { get; set; } = 52.0;
        #endregion

        #region Styling
        public Color TextColor { get; set; } = Colors.White;
        public Color BackgroundColor { get; set; } = Colors.White;
        public Color BorderColor { get; set; } = Colors.Black;
        public Color TitleColor { get; internal set; } = Colors.Black;
        public int BorderWidth { get; set; } = 2;
        #endregion
    }

    public sealed class BlockLocationSpecification
    {
        public Image Image { get; set; }
        public Vector2 Position { get; set; }
    }

    public static class StaticReportHelper
    {
        #region Make Configuration
        /// <summary>
        /// Create a configuration for ScatterPlot
        /// </summary>
        public static ReportFormattingParameters ConfigureReport(string? title = null, string fontFamily = "Calibri", int margin = 20, double labelFontSize = 52, Color? textColor = null, Color? borderColor = null, Color? backgroundColor = null, Color? titleColor = null, int borderWidth = 2)
        {
            textColor ??= Colors.White;
            borderColor ??= Colors.Black;
            backgroundColor ??= Colors.White;
            titleColor ??= Colors.Black;
            return new ReportFormattingParameters()
            {
                ReportTitle = string.IsNullOrEmpty(title) ? null : title,
                FontFamily = fontFamily,
                Margin = margin,
                LabelFontSize = labelFontSize,
                TextColor = textColor,
                BorderColor = borderColor,
                BackgroundColor = backgroundColor,
                TitleColor = titleColor,
                BorderWidth = borderWidth
            };
        }
        #endregion

        #region Layout
        public static BlockLocationSpecification PositionChart(Image chart, Vector2 position)
        {
            return new BlockLocationSpecification()
            {
                Image = chart,
                Position = position,
            };
        }
        #endregion

        #region Generation
        public static Image GenerateReport(BlockLocationSpecification[] charts, ReportFormattingParameters? parameters = null)
        {
            parameters ??= new ReportFormattingParameters();

            // Calculate initial parameters
            int width = (int)(charts.Max(c => c.Position.X + c.Image.Width) + parameters.Margin * 2);

            // Add title
            MagickImage? caption = null;
            int titleOffset = 0;
            if (parameters.ReportTitle != null)
            {
                MagickReadSettings captionSettings = new()
                {
                    Font = parameters.FontFamily,
                    FontPointsize = parameters.LabelFontSize,
                    TextGravity = Gravity.Center,
                    BackgroundColor = MagickColors.Transparent,
                    Width = width,
                    FillColor = ConvertColor(parameters.TitleColor),
                };
                caption = new($"caption:{parameters.ReportTitle}", captionSettings); // Notice the `caption` special format; This is just how the library works, per https://github.com/dlemstra/Magick.NET/blob/main/docs/Drawing.md#adding-text-to-existing-image
                titleOffset += caption.Height;
            }

            // Create blank canvas
            int height = (int)(charts.Max(c => c.Position.Y + c.Image.Height) + parameters.Margin * 2 + titleOffset);
            MagickImage composition = new(ConvertColor(parameters.BackgroundColor), width, height);
            // Add title
            if (caption != null)
                composition.Composite(caption, parameters.Margin, parameters.Margin, CompositeOperator.Over);

            for (int i = 0; i < charts.Length; i++)
            {
                BlockLocationSpecification chart = charts[i];
                MagickImage? image = null;
                if (chart.Image.ShouldLoadFileDirectly)
                    image = new(chart.Image.FileReference);
                else
                {
                    var intermediateLocation = Image.GetTempImagePath(); // We are generating a file to save the burden of implementing specific conversion; In the future we will implement in-memory conversion
                    chart.Image.Save(intermediateLocation);
                    image = new(intermediateLocation);
                }
                if (parameters.BorderWidth != 0)
                {
                    new Drawables()
                        // Add a border
                        .StrokeColor(ConvertColor(parameters.BorderColor))
                        .StrokeWidth(parameters.BorderWidth)
                        .FillColor(MagickColors.Transparent)
                        .Rectangle(0, 0, chart.Image.Width, chart.Image.Height)
                        .Draw(image);
                }

                // Composite
                composition.Composite(image, (int)chart.Position.X + parameters.Margin, (int)chart.Position.Y + titleOffset + parameters.Margin, CompositeOperator.Over);
            }

            string tempPath = Image.GetTempImagePath();
            composition.Write(tempPath);
            return new Image(tempPath);
        }
        #endregion

        #region Helpers
        private static string ToHex(Color c)
            => "#" + c.Red.ToString("X2") + c.Green.ToString("X2") + c.Blue.ToString("X2");
        private static MagickColor ConvertColor(Color c)
            => new(c.Red, c.Green, c.Blue, c.Alpha);
        #endregion
    }
}
