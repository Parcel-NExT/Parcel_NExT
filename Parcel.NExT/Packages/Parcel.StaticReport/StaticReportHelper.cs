using ImageMagick;
using Parcel.Types;
using System.Drawing;
using System.Numerics;
using Color = Parcel.Types.Color;

namespace Parcel.StaticReport
{
    public class ReportFormattingParameters
    {
        #region Auxiliary Elements
        public string? ReportTitle { get; set; } = null;
        #endregion

        #region Formatting
        public int Margin { get; set; } = 20;
        public double LabelFontSize = 52.0;
        #endregion

        #region Styling
        public MagickColor TextColor = MagickColors.White;
        #endregion

        #region Placeholder
        // Placeholder; Below stuff should be computed
        public int ImagesPerRow = 2;
        public Color BackgroundColor = Colors.Black;
        public Size Crop = new(1920, 1080);
        public Size CaptionBox = new(680, 250);
        #endregion
    }

    public sealed class BlockLocationSpecification
    {
        public string? Caption { get; set; }
        public Image Image { get; set; }
        public Vector2 Position { get; set; }
    }

    public static class StaticReportHelper
    {
        #region Layout
        public static BlockLocationSpecification PositionChart(Image chart, Vector2 position, string? caption = null)
        {
            return new BlockLocationSpecification()
            {
                Caption = caption,
                Image = chart,
                Position = position,
            };
        }
        #endregion

        #region Generation
        public static Image GenerateReport(BlockLocationSpecification[] charts, ReportFormattingParameters? parameters = null)
        {
            parameters ??= new ReportFormattingParameters();

            int rows = (int)Math.Ceiling((double)charts.Length / parameters.ImagesPerRow);
            var composition = new MagickImage(
                new MagickColor(ToHex(parameters.BackgroundColor)),
                parameters.ImagesPerRow * parameters.Crop.Width + (parameters.ImagesPerRow + 1) * parameters.Margin,
                rows * parameters.Crop.Height + (rows + 1) * parameters.Margin
            );

            for (int i = 0; i < charts.Length; i++)
            {
                BlockLocationSpecification chart = charts[i];
                MagickImage? image = null;
                if (chart.Image.ShouldLoadFileDirectly)
                    image = new(chart.Image.FileReference);
                else
                {
                    var intermediateLocation = GetTempImagePath(); // We are generating a file to save the burden of implementing specific conversion; In the future we will implement in-memory conversion
                    chart.Image.Save(intermediateLocation);
                    image = new(intermediateLocation);
                }
                image.Crop(new MagickGeometry(image.Width / 2 - parameters.Crop.Width / 2, image.Height / 2 - parameters.Crop.Height / 2, parameters.Crop.Width, parameters.Crop.Height));
                new Drawables()
                    // Add a border
                    .StrokeColor(new MagickColor(0, Quantum.Max, 0))
                    .StrokeWidth(2)
                    .FillColor(MagickColors.Transparent)
                    .Rectangle(0, 0, parameters.Crop.Width, parameters.Crop.Height)
                    .Draw(image);

                // Add caption
                var captionSettings = new MagickReadSettings
                {
                    Font = "Calibri",
                    FontPointsize = parameters.LabelFontSize,
                    TextGravity = Gravity.Center,
                    BackgroundColor = MagickColors.Transparent,
                    Height = parameters.CaptionBox.Height, // height of text box
                    Width = parameters.CaptionBox.Width, // width of text box
                    FillColor = parameters.TextColor
                };
                if (!string.IsNullOrEmpty(chart.Caption))
                {
                    MagickImage caption = new(chart.Caption, captionSettings);
                    image.Composite(caption, parameters.Margin, parameters.Margin, CompositeOperator.Over);
                }

                // Composite
                int row = i / parameters.ImagesPerRow;
                int col = i % parameters.ImagesPerRow;
                composition.Composite(image, col * parameters.Crop.Width + (col + 1) * parameters.Margin, row * parameters.Crop.Height + (row + 1) * parameters.Margin, CompositeOperator.Over);
            }

            string tempPath = GetTempImagePath();
            composition.Write(tempPath);
            return new Image(tempPath);
        }
        #endregion

        #region Helpers
        internal static string GetTempImagePath()
            => Path.GetTempPath() + Guid.NewGuid().ToString() + ".png";
        private static string ToHex(Color c)
            => "#" + c.Red.ToString("X2") + c.Green.ToString("X2") + c.Blue.ToString("X2");
        #endregion
    }
}
