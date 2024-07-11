namespace Parcel.Framework.WebPages
{
    /// <summary>
    /// Provides content, layout and styling configuration of the website
    /// </summary>
    public class WebsiteConfiguration(string siteName, string templateName, WebsiteBlock[] blocks)
    {
        public string SiteName { get; } = siteName;
        public string TemplateName { get; } = templateName; // Including layout
        public WebsiteBlock[] Blocks { get; } = blocks;
    }
}
