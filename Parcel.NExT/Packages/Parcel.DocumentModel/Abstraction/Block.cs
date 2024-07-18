namespace Parcel.Model.Abstraction
{
    /// <summary>
    /// A block is an abstraction of either content or layout, which is the same under this documenet model
    /// </summary>
    public abstract class Block
    {

    }

    /// <summary>
    /// A run is a single unit of rich text. It's fundamentally string based.
    /// </summary>
    public class Run: Block
    {
        /// <summary>
        /// Rich text in Markdown-like format
        /// </summary>
        public string? RichText { get; set; }

        #region Accessor
        /// <summary>
        /// Plain text without any annotation construct
        /// </summary>
        public string RawText
        {
            get
            {
                // TODO: Pesudoimplementation
                return RichText?.Replace("**", string.Empty) ?? string.Empty;
            }
        }
        #endregion
    }

    /// <summary>
    /// A block that implies certain layout
    /// </summary>
    public abstract class Layout:Block
    {
        public List<Block> Children { get; set; } = [];
    }

    /// <summary>
    /// A document under this model is just a block that is a layout because it can contain other blocks
    /// </summary>
    public abstract class Document:Layout
    {

    }
}
