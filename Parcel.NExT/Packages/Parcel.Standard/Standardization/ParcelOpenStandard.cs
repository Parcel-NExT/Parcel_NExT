using Parcel.Infrastructure;

namespace Parcel.Standard.Standardization
{
    /// <summary>
    /// Provides a hierarchical description structure for POS. This is mostly for demonstration purpose.
    /// </summary>
    /// <remarks>
    /// For simplicity, we are providing contents directly as string literals; Notice ParcelOpenStandard needs Frontend/POS level support for displaying.
    /// TODO: Consider moving into Parcel.Demo or Parcel.Manual etc. package instead since this module doesn't serve practical purposes.
    /// </remarks>
    public class ParcelOpenStandard : IParcelSerializable
    {
        #region Properties
        public ParcelOpenStandard() { }
        public ParcelOpenStandard(string text) => Text = text;
        public static implicit operator ParcelOpenStandard(string text) => new(text);
        public string Text { get; } = string.Empty;
        #endregion

        #region Components
        /// <name>
        /// Parcel Open Standards
        /// </name>
        /// <returns name="Children"></returns>
        public static ParcelOpenStandard POS()
        {
            return """
                POS is a standard for the overall Parcel programming platform and its ecosystem compositions. 
                See children nodes for more.
                """;
        }
        /// <summary>
        /// Provides description for frontends.
        /// </summary>
        /// <returns name="Children"></returns>
        public static ParcelOpenStandard ParcelFrontends()
        {
            return """
                ## Parcel Frontends
                ...
                """;
        }
        #endregion

        #region Display
        public override string ToString()
            => Text;
        #endregion

        #region Serialization
        public IParcelSerializable FromText(string text)
            => new ParcelOpenStandard(text);
        public IParcelSerializable ReadFromStream(BinaryReader reader, out long consumedBytes)
        {
            long original = reader.BaseStream.Position;
            string text = reader.ReadString();
            consumedBytes = reader.BaseStream.Position - original; // TODO: Need more reliable ways to calculate number of bytes consumed
            return new ParcelOpenStandard(text);
        }
        public string ToText() => Text;
        public void WriteToStream(BinaryWriter writer, IParcelSerializable data)
            => writer.Write(Text);
        #endregion
    }
}
