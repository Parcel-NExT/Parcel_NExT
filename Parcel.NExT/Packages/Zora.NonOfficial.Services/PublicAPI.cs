using Parcel.Web;

// TODO: Provide fully OOP unwrapped return values based on common extraction scenarios

namespace Zora.Services
{
    public sealed class PublicAPIConfiguration
    {
        public string APIKey { get; set; }
    }

    /// <summary>
    /// Consolidated Public API interface
    /// </summary>
    public static class PublicAPI
    {
        #region Configuration
        public static PublicAPIConfiguration ConfigureNewsService(string apiKey)
        {
            return new PublicAPIConfiguration()
            {
                APIKey = apiKey
            };
        }
        #endregion

        #region Books
        public static string Bible()
        {
            return RESTAPILean.Get(@"https://www.abibliadigital.com.br/api/verses/nvi/sl/23");
        }
        #endregion

        #region Market
        /// <summary>
        /// Get weather from https://weatherstack.com/
        /// </summary>
        public static string WeatherStack(string location, PublicAPIConfiguration configuration)
        {
            return RESTAPILean.Get($@"https://api.weatherstack.com/current?access_key={configuration.APIKey}&query={location}");
        }
        /// <summary>
        /// Get fx data from https://fixer.io/
        /// </summary>
        public static string FixerAPI(PublicAPIConfiguration configuration)
        {
            return RESTAPILean.Get($@"https://data.fixer.io/api/latest?access_key={configuration.APIKey}");
        }
        #endregion

        #region Weather

        #endregion

        #region Location
        /// <summary>
        /// Get IP location from https://ipstack.com/
        /// </summary>
        public static string IPStackAPI(string ip, PublicAPIConfiguration configuration)
        {
            return RESTAPILean.Get($@"https://api.ipstack.com/{ip}?access_key={configuration.APIKey}");
        }
        #endregion

        #region News

        #endregion
    }
}
