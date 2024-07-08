namespace Parcel.Processing.Utilities
{
    public static class ParcelFrontendImagePreviewSupport
    {
        /// <summary>
        /// Enable previewing using hard-coded protocol; In the future this will not be necessary when we have better context hints (e.g. through Package metadata)
        /// </summary>
        public static string PreviewImage(string imagePath)
            => $"Image://{imagePath}";
    }
}
