using BigGustave;

namespace Parcel.Types
{
    public struct Pixel
    {
        public byte Red {  get; set; }
        public byte Green {  get; set; }
        public byte Blue {  get; set; }
        /// <summary>
        /// Opacity (Transparency), higher value means more opaque.
        /// </summary>
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
        public Pixel(BigGustave.Pixel pixel) 
            : this(pixel.R, pixel.G, pixel.B, pixel.A) { }
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
        public Pixel[][]? Pixels { get; private set; }
        /// <summary>
        /// Use this to save having to load the file into memory.
        /// </summary>
        public string? FileReference { get; set; }
        /// <summary>
        /// Should load file from disk instead of relying on in-memory data
        /// </summary>
        public bool ShouldLoadFileDirectly => FileReference != null;
        #endregion

        #region Constructors
        public Image()
            => Pixels = AllocatePixles(0, 0);
        public Image(string pngFile, bool doNotLoad = false) // Should be true
        {
            if (doNotLoad)
                FileReference = pngFile;
            else
                Load(pngFile);
        }
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
        {
            Pixels = LoadFile(path, out int width, out int height);
            Width = width;
            Height = height;
        }
        public static Image LoadFrom(string path)
        {
            return new Image(path, false);
        }
        public void Save(string path)
        {
            if (ShouldLoadFileDirectly)
                File.Copy(FileReference!, path);
            else
            {
                PngBuilder builder = ConvertToBigGustave(Pixels, Width, Height);
                FileStream fileStream = File.Create(path);
                builder.Save(fileStream);
                fileStream.Close();
            }
        }
        #endregion

        #region Helpers
        private static Pixel[][] AllocatePixles(int width, int height)
        {
            Pixel[][] pixels = new Pixel[height][];
            for (int row = 0; row < height; row++)
                pixels[row] = new Pixel[width];
            return pixels;
        }
        private static Pixel[][] ConvertUintGrids(uint[][] uintPixels)
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
        private static Pixel[][] LoadFile(string pngFile, out int width, out int height)
        {
            using FileStream stream = File.OpenRead(pngFile);
            Png image = Png.Open(stream);

            return ConvertFromBigGustavePng(out width, out height, image);
        }
        private static PngBuilder ConvertToBigGustave(Pixel[][] pixels, int width, int height)
        {
            PngBuilder builder = PngBuilder.Create(width, height, true);

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    Pixel original = pixels[row][col];
                    BigGustave.Pixel pixel = new(original.Red, original.Green, original.Blue, original.Alpha, false);
                    builder.SetPixel(pixel, col, row);
                }
            }
            return builder;
        }
        private static Png ConvertToBigGustavePng(Pixel[][] pixels, int width, int height)
        {
            PngBuilder builder = ConvertToBigGustave(pixels, width, height);

            using MemoryStream memory = new();
            builder.Save(memory);
            return Png.Open(memory);
        }
        private static Pixel[][] ConvertFromBigGustavePng(out int width, out int height, Png image)
        {
            width = image.Width;
            height = image.Height;
            Pixel[][] pixels = AllocatePixles(width, height);

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    BigGustave.Pixel pixel = image.GetPixel(col, row);
                    pixels[row][col] = new Pixel(pixel);
                }
            }

            return pixels;
        }
        #endregion

        #region Utility
        public static string GetTempImagePath()
            => Path.GetTempPath() + Guid.NewGuid().ToString() + ".png";
        #endregion
    }
}
