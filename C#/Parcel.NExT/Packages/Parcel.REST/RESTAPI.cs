namespace Parcel.Web
{
    public static class RESTAPILean
    {
        #region GET
        public static string Get(string url)
        {
            throw new NotImplementedException();
        }
        public static string Get(string url, string parameters)
        {
            throw new NotImplementedException();
        }
        public static string Get(string url, string[] parameters)
        {
            throw new NotImplementedException();
        }
        public static string Get(string url, Dictionary<string, string> parameters, string? body = null, Dictionary<string, string>? headers = null)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region POST
        public static string Post(string url)
        {
            throw new NotImplementedException();
        }
        public static string Post(string url, string parameters)
        {
            throw new NotImplementedException();
        }
        public static string Post(string url, string[] parameters)
        {
            throw new NotImplementedException();
        }
        public static string Post(string url, Dictionary<string, string> parameters, string? body = null, Dictionary<string, string>? headers = null)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    public class RESTReponse
    {
        public int ResponseCode { get; set; }
        public int Content { get; set; }
    }
    public static class RESTAPITyped
    {
        #region GET
        public static RESTReponse Get(string url, Dictionary<string, string>? parameters = null, string? body = null, Dictionary<string, string>? headers = null)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
