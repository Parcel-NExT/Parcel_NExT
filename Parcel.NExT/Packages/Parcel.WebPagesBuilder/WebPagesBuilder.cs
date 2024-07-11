using Parcel.Infrastructure;

namespace Parcel.Framework.WebPages
{
    public static class WebPagesBuilder
    {
        #region Build
        public static string Test(WebsiteConfiguration configurations, string? addressPort)
        {
            throw new NotImplementedException();
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
        public static HeaderBlock Header1(string header)
            => new(header, 1);
        public static HeaderBlock Header2(string header)
            => new(header, 2);
        public static HeaderBlock Header3(string header)
            => new(header, 3);
        public static HeaderBlock Header4(string header)
            => new(header, 4);
        public static HeaderBlock Header5(string header)
            => new(header, 5);
        public static HeaderBlock Header6(string header)
            => new(header, 6);
        public static HeaderBlock Header7(string header)
            => new(header, 7);
        public static ParagraphBlock Paragraph(string text)
            => new(text);
        #endregion
    }
}
