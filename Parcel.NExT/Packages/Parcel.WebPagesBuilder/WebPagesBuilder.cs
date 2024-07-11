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
        public static void Build(WebsiteConfiguration configurations, string outputFolder)
        {

        }
        #endregion

        #region Content Generation
        public static HeaderBlock Header(string text)
        {
            return new HeaderBlock(text);
        }
        #endregion
    }
}
