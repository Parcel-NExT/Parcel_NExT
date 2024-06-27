namespace Parcel.Processing
{
    public class ImageProcessor
    {
        #region Constructor and Static Builder
        public ImageProcessor(string imagePath)
        {
            throw new NotImplementedException();
        }
        public static ImageProcessor LoadImage(string imagePath)
            => new(imagePath);
        #endregion
    }
}
