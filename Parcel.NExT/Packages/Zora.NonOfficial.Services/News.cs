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
        #region Methods
        /// <summary>
        /// Read news as single text
        /// </summary>
        public static string ReadNews(NewsSource source, NewsAdditionalConfiguration? configurations)
        {
            configurations ??= new NewsAdditionalConfiguration();

            return RESTAPILean.Get($"https://newsapi.org/v2/top-headlines?country=us&category=business&apiKey={configurations.NewsAPIKey}");
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

    }
}
