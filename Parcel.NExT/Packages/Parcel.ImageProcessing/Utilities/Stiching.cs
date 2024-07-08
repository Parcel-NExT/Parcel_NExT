using ImageMagick;
using System.Drawing;

namespace Parcel.Processing.Utilities
{
    public static class Stiching
    {
        #region Advanced Configurations
        public class StichImageParameters
        {
            public int Margin = 20;
            public int ImagesPerRow = 2;
            public Color BackgroundColor = Color.Black;
            public Size Crop = new(1920, 1080);

            public double FontSize = 52.0;
            public MagickColor TextColor = MagickColors.White;
            public Size CaptionBox = new(680, 250);
        }
        #endregion

        #region Methods
        public static void StichImages(string[] images, string outputImage, StichImageParameters additionalParameters)
        {
            int rows = (int)Math.Ceiling((double)images.Length / additionalParameters.ImagesPerRow);
            var composition = new MagickImage(
                new MagickColor(ToHex(additionalParameters.BackgroundColor)),
                additionalParameters.ImagesPerRow * additionalParameters.Crop.Width + (additionalParameters.ImagesPerRow + 1) * additionalParameters.Margin,
                rows * additionalParameters.Crop.Height + (rows + 1) * additionalParameters.Margin
            );

            for (int i = 0; i < images.Length; i++)
            {
                string file = images[i];
                MagickImage image = new(file);
                image.Crop(new MagickGeometry(image.Width / 2 - additionalParameters.Crop.Width / 2, image.Height / 2 - additionalParameters.Crop.Height / 2, additionalParameters.Crop.Width, additionalParameters.Crop.Height));
                new Drawables()
                    // Add a border
                    .StrokeColor(new MagickColor(0, Quantum.Max, 0))
                    .StrokeWidth(2)
                    .FillColor(MagickColors.Transparent)
                    .Rectangle(0, 0, additionalParameters.Crop.Width, additionalParameters.Crop.Height)
                    .Draw(image);

                // Add caption
                MagickReadSettings captionSettings = new()
                {
                    Font = "Calibri",
                    FontPointsize = additionalParameters.FontSize,
                    TextGravity = Gravity.Center,
                    BackgroundColor = MagickColors.Transparent,
                    Height = additionalParameters.CaptionBox.Height, // height of text box
                    Width = additionalParameters.CaptionBox.Width, // width of text box
                    FillColor = additionalParameters.TextColor
                };
                MagickImage caption = new($"Caption:{Path.GetFileNameWithoutExtension(file)}", captionSettings);
                image.Composite(caption, additionalParameters.Margin, additionalParameters.Margin, CompositeOperator.Over);

                // Composite
                int row = i / additionalParameters.ImagesPerRow;
                int col = i % additionalParameters.ImagesPerRow;
                composition.Composite(image, col * additionalParameters.Crop.Width + (col + 1) * additionalParameters.Margin, row * additionalParameters.Crop.Height + (row + 1) * additionalParameters.Margin, CompositeOperator.Over);
            }

            composition.Write(outputImage);
        }
        public static void StitchImagesInFolder(string sourceFolder, string outputFilePath, StichImageParameters parameters)
        {
            string[] files = Directory.EnumerateFiles(sourceFolder).ToArray();

            StichImages(files, outputFilePath, parameters);
        }
        #endregion

        #region Helpers
        private static string ToHex(Color c)
            => "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        #endregion
    }
}
