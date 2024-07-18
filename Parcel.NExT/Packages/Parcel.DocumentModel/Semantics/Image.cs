using Parcel.Model.Abstraction;

namespace Parcel.Model.Semantics
{
    /// <summary>
    /// A standalone piece of image by reference
    /// </summary>
    public class Image:Block
    {
        /// <summary>
        /// Display name or title
        /// </summary>
        public string? DisplayName { get; set; }
        /// <summary>
        /// Underlying image file path
        /// </summary>
        public string? FilePath { get; set; }
        /// <summary>
        /// Data encoded in Base64
        /// </summary>
        public string? EncodedData { get; set; }
    }
}
