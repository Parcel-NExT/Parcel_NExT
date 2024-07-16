namespace Parcel.Types
{
    /// <summary>
    /// Parcel-native standard color representation, used mostly by dependent packages (aka. not directly in Parcel.Image)
    /// </summary>
    public class Color
    {
        public byte Red { get; set; }
        public byte Green { get; set; }
        public byte Blue { get; set; }
        /// <summary>
        /// Opacity (Transparency), higher value means more opaque.
        /// </summary>
        public byte Alpha { get; set; }

        public Color() { }
        public Color(byte red, byte green, byte blue, byte alpha)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;
        }
        public Color(byte[] bytes)
            : this(bytes[0], bytes[1], bytes[2], bytes[3]) { }
        public Color(BigGustave.Pixel pixel)
            : this(pixel.R, pixel.G, pixel.B, pixel.A) { }

        #region Parsing
        /// <param name="hex">Format: #RRGGBBAA, where AA is optional</param>
        public static Color Parse(string hex)
        {
            hex = hex.TrimStart('#');
            byte r = (byte)Convert.ToUInt32(hex.Substring(0, 2), 16);
            byte g = (byte)Convert.ToUInt32(hex.Substring(2, 2), 16);
            byte b = (byte)Convert.ToUInt32(hex.Substring(4, 2), 16);
            byte a = 255;
            if (hex.Length > 6)
                a = (byte)Convert.ToUInt32(hex.Substring(6, 2), 16);
            return new Color(r, g, b, a);
        }
        public static Color ParseARGB(string hex)
        {
            hex = hex.TrimStart('#');
            byte a = (byte)Convert.ToUInt32(hex.Substring(0, 2), 16);
            byte r = (byte)Convert.ToUInt32(hex.Substring(2, 2), 16);
            byte g = (byte)Convert.ToUInt32(hex.Substring(4, 2), 16);
            byte b = (byte)Convert.ToUInt32(hex.Substring(6, 2), 16);
            return new Color(r, g, b, a);
        }
        public override string ToString()
        {
            return $"#{BitConverter.ToString([Red, Green, Blue, Alpha]).Replace("-", "")}";
        }
        #endregion
    }
}
