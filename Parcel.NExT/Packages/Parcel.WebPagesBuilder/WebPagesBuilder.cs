using Parcel.Infrastructure;

namespace Parcel.Framework.WebPages
{
    public static class WebPagesBuilder
    {
        #region Build
        public static string Test(WebsiteConfiguration configurations, string? addressPort)
        {
            string html = new HTMLGenerator(configurations).HTML;
            DevelopmentServer webServer = QuickServe.ServeHTML(html);

            Thread.Sleep(1000); // Wait for socket to bind
            return webServer.ServerAddress;
        }
        public static string Test(WebsiteBlock block)
        {
            string html = new HTMLGenerator(block).HTML;
            DevelopmentServer webServer = QuickServe.ServeHTML(html);

            Thread.Sleep(1000); // Wait for socket to bind
            return webServer.ServerAddress;
        }
        public static string Test(params WebsiteBlock[] blocks)
        {
            string html = new HTMLGenerator(blocks).HTML;
            DevelopmentServer webServer = QuickServe.ServeHTML(html);

            Thread.Sleep(1000); // Wait for socket to bind
            return webServer.ServerAddress;
        }
        public static void Build(WebsiteConfiguration configurations, string outputFolder)
        {

        }
        #endregion

        #region Content Generation
        public static HeaderBlock Header1(string header = "Header 1")
            => new(header, 1);
        public static HeaderBlock Header2(string header = "Header 2")
            => new(header, 2);
        public static HeaderBlock Header3(string header = "Header 3")
            => new(header, 3);
        public static HeaderBlock Header4(string header = "Header 4")
            => new(header, 4);
        public static HeaderBlock Header5(string header = "Header 5")
            => new(header, 5);
        public static HeaderBlock Header6(string header = "Header 6")
            => new(header, 6);
        public static HeaderBlock Header7(string header = "Header 7")
            => new(header, 7);
        public static ParagraphBlock Paragraph(string text)
            => new(text);
        #endregion

        #region Template Generation
        public static WebsiteConfiguration MakeSimpleHeaderPlusContent(string siteName = "My Site", string header = "Site Header", string content = "Welcome!")
        {
            return new WebsiteConfiguration(
                siteName,
                HTMLGenerator.DefaultTemplateName, [
                new HeaderBlock(header),
                new ParagraphBlock(content)
            ]);
        }
        #endregion
    }
}
