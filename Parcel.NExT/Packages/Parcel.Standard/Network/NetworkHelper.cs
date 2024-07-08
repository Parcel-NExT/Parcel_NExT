namespace Parcel.Standard.Network
{
    public static class NetworkHelper
    {
        /// <summary>
        /// Download file from network location to a given local path
        /// </summary>
        public static void DownloadFile(string url, string output)
        {
            using HttpClient client = new();
            using Task<Stream> stream = client.GetStreamAsync(url);
            using FileStream fs = new(output, FileMode.OpenOrCreate);
            stream.Result.CopyTo(fs);
        }
    }
}
