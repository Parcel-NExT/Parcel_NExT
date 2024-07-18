using Parcel.Model.Abstraction;

namespace Parcel.Model.Semantics
{
    /// <summary>
    /// Nothing fancy, just a header.
    /// The level of a header is inherited from its section hierarchy.
    /// </summary>
    public class Header:Block
    {
        public string? Text { get; set; }
    }
}
