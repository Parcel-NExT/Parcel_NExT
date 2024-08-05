using Parcel.Types;
using Parcel.Web;

namespace Zora.Services
{
    public sealed class News
    {
        public string Title { get; set; }
        public string Source { get; set; }
        public string Abstract { get; set; }
        public DateTime Date { get; set; }
        public string Content { get; set; }
    }
    public enum NewsSource
    {
        Any,
        CBC,
        NewsAPI
    }
    public sealed class NewsAdditionalConfiguration
    {
        public string NewsAPIKey { get; set; }
    }
    public static class NewsReader
    {
        public static NewsAdditionalConfiguration ConfigureNewsService(string apiKey)
        {
            return new NewsAdditionalConfiguration()
            {
                NewsAPIKey = apiKey
            };
        }

        #region Methods
        /// <summary>
        /// Read news as single text
        /// </summary>
        public static string ReadNews(NewsSource source, string topics, NewsAdditionalConfiguration? configurations)
        {
            configurations ??= new NewsAdditionalConfiguration();

            // NewsAPI
            switch (source)
            {
                case NewsSource.Any:
                    break;
                case NewsSource.CBC:
                    return GetCBCRSSFeed(topics);
                case NewsSource.NewsAPI:
                    return RESTAPILean.Get($"https://newsapi.org/v2/top-headlines?country=us&category=business&apiKey={configurations.NewsAPIKey}");
                default:
                    break;
            }
            return "Not available.";
        }
        /// <summary>
        /// Get news as objects
        /// </summary>
        public static News GetNews()
        {
            throw new NotImplementedException();

        }
        /// <summary>
        /// Get news as DataGrid
        /// </summary>
        public static DataGrid DownloadNews()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Routines
        private static string GetCBCRSSFeed(string topics)
        {
            // https://www.cbc.ca/rss/
            switch (topics)
            {
                case "Top Stories":
                    return RESTAPILean.Get(@"https://www.cbc.ca/webfeed/rss/rss-topstories");
                case "World News":
                    return RESTAPILean.Get(@"https://www.cbc.ca/webfeed/rss/rss-world");
                case "Canada News":
                    return RESTAPILean.Get(@"https://www.cbc.ca/webfeed/rss/rss-canada");
                default:
                    break;
            }
            return "Not Available.";
        }
        #endregion
    }
}
