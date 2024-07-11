namespace Parcel.Framework.WebPages
{
    /// <summary>
    /// Represents a styled `div`
    /// </summary>
    public abstract class WebsiteBlock
    {

    }

    public sealed class HeaderBlock(string header) : WebsiteBlock
    {
        #region Properties
        public string Header { get; } = header;
        #endregion
    }
}
