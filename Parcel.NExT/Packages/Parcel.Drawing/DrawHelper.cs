using Parcel.Types;
using SkiaSharp;

namespace Parcel.Graphing
{
    public static class DrawHelper
    {
        public static Image Draw(int width = 800, int height = 400)
        {
            SKImageInfo imageInfo = new(width, height);
            using SKSurface surface = SKSurface.Create(imageInfo);
            SKCanvas canvas = surface.Canvas;
            canvas.DrawColor(SKColors.Red);
            canvas.Clear(SKColors.Red); //same thing but also erases anything else on the canvas first

            using SKPaint paint = new();
            paint.Color = SKColors.Blue;
            paint.IsAntialias = true;
            paint.StrokeWidth = 15;
            paint.Style = SKPaintStyle.Stroke;
            canvas.DrawCircle(50, 50, 30, paint); //arguments are x position, y position, radius, and paint

            using SKImage image = surface.Snapshot();
            using SKData data = image.Encode(SKEncodedImageFormat.Png, 100);
            string tempFile = GetTempImagePath();
            using FileStream file = File.OpenWrite(tempFile);
            data.SaveTo(file);
            file.Dispose();

            return new Image(tempFile);
        }

        #region Procedural Wrappers

        #endregion

        #region Helpers
        private static string GetTempImagePath()
            => Path.GetTempPath() + Guid.NewGuid().ToString() + ".png";
        #endregion
    }
}
