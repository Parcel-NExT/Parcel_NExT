using System.Drawing;
using System.Numerics;

namespace Parcel.Processing.OneShot
{
    public static class OneShotProcessing
    {
        #region Resizing
        public static void ResizeImage(string originalImage, string outputImage, uint newWidth)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Resize the image to new width and crop or fill to new height
        /// </summary>
        public static void ResizeImage(string originalImage, string outputImage, uint newWidth, uint newHeight, Color? fillColor)
        {
            // Default value
            fillColor ??= Color.White;

            throw new NotImplementedException();
        }
        public static void ResizeImage(string originalImage, string outputImage, Size newSize, Color? fillColor)
            => ResizeImage(originalImage, outputImage, (uint)newSize.Width, (uint)newSize.Height, fillColor);
        #endregion

        #region Cropping
        public static void CropImage(string originalImage, string outputImage, uint offsetX, uint offsetY, uint cropWdith, uint cropHeight, Color? fillColor)
        {
            throw new NotImplementedException();
        }
        public static void CropImage(string originalImage, string outputImage, Vector2 offset, Size cropSize, Color? fillColor)
            => CropImage(originalImage, outputImage, (uint)offset.X, (uint)offset.Y, (uint)cropSize.Width, (uint)cropSize.Height, fillColor);
        #endregion

        #region Brightness & Contrast

        #endregion

        #region Color & Saturation

        #endregion

        #region Coloring (Recolor, color selection/filter) and LUT

        #endregion

        #region B&W/Grayscale

        #endregion

        #region HSL

        #endregion

        #region Shadows and Highlights

        #endregion

        #region Add Texts

        #endregion

        #region Color Balance

        #endregion

        #region Invert Color

        #endregion
    }
}
