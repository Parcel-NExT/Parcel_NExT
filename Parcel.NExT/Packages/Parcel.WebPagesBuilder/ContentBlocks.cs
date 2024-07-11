namespace Parcel.Framework.WebPages
{
    /// <summary>
    /// Represents a styled `div`
    /// </summary>
    public abstract class WebsiteBlock
    {
        public abstract string ToHTML(string indentation);
    }

    public sealed class HeaderBlock(string header, int level = 1) : WebsiteBlock
    {
        #region Properties
        public string Header { get; } = header;
        public int Level { get; } = level;
        #endregion

        #region Implementation
        public override string ToHTML(string indentation)
        {
            return $"{indentation}<h{Level}>{Header}</h{Level}>";
        }
        #endregion
    }

    public sealed class ParagraphBlock(string text) : WebsiteBlock
    {
        #region Properties
        public string Text { get; } = text;
        #endregion

        #region Implementation
        public override string ToHTML(string indentation)
        {
            return $"{indentation}<h>{Text}</h>";
        }
        #endregion
    }
}
