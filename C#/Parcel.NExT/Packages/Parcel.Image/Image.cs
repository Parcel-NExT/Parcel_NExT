namespace Parcel.Types
{
    public struct Pixel
    {
        public byte Red {  get; set; }
        public byte Green {  get; set; }
        public byte Blue {  get; set; }
        public byte Alpha {  get; set; }

        public Pixel(){ }
        public Pixel(byte red, byte green, byte blue, byte alpha)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;
        }
        public Pixel(byte[] bytes) 
            : this(bytes[0], bytes[1], bytes[2], bytes[3]) { }
    }
    /// <summary>
    /// Row-major 2D image pixel grid.
    /// </summary>
    public class Image
    {
        #region Data
        public int Width { get; set; }
        public int Height { get; set; }
        /// <remarks>
        /// Row-major, aka. Pixels[Row][Col] or Pixels[Height][Width]
        /// </remarks>
        private Pixel[][] Pixels { get; set; }
        #endregion

        #region Constructors
        public Image()
            => Pixels = AllocatePixles(0, 0);
        public Image(int width, int height)
            => Pixels = AllocatePixles(width, height);
        public Image(Pixel[][] pixels)
            => Pixels = pixels;
        public Image(uint[][] pixels)
            => Pixels = ConvertUintGrids(pixels);
        #endregion

        #region Static Methods
        public static Image LoadImage(string path)
            => throw new NotImplementedException();
        #endregion

        #region Methods
        public void Load(string path)
            => throw new NotImplementedException();
        public void Save(string path)
            => throw new NotImplementedException();
        #endregion

        #region Helpers
        private static Pixel[][] AllocatePixles(int width, int height)
        {
            Pixel[][] pixels = new Pixel[height][];
            for (int row = 0; row < height; row++)
                pixels[row] = new Pixel[width];
            return pixels;
        }
        private Pixel[][] ConvertUintGrids(uint[][] uintPixels)
        {
            int height = uintPixels.Length;
            int width = uintPixels.First().Length;
            Pixel[][] pixels = AllocatePixles(width, height);
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    byte[] bytes = BitConverter.GetBytes(uintPixels[row][col]);
                    pixels[row][col] = new Pixel(bytes);
                }
            }
            return pixels;
        }
        #endregion
    }
}
