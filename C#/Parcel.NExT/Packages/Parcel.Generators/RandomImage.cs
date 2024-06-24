namespace Parcel.Data
{
    public static class RandomImage
    {
        public static string GetRandomImage(uint? width = null, uint? height = null)
        {
            string uri = GetPicsumPhotosUrl(width, height);

            string outputPath = GetTempImagePath();
            new HttpClient().DownloadFileTaskAsync(new Uri(uri), outputPath).GetAwaiter().GetResult();
            return $"Image://{outputPath}";
            

            static string GetPicsumPhotosUrl(uint? width = null, uint? height = null)
            {
                if (width == null)
                    return "https://picsum.photos/200/300";
                else if (height == null) 
                    return $"https://picsum.photos/{width}";
                else return $"https://picsum.photos/{width}/{height}";
            }
        }

        #region Helpers
        private static string GetTempImagePath()
            => Path.GetTempPath() + Guid.NewGuid().ToString() + ".png";
        #endregion
    }

    public static class HTTPClientHelper
    {
        public static async Task DownloadFileTaskAsync(this HttpClient client, Uri uri, string outputPath)
        {
            // TODO: Not working, program is stuck here.
            HttpResponseMessage response = await client.GetAsync(uri);
            using FileStream fileStream = new(outputPath, FileMode.CreateNew);
            await response.Content.CopyToAsync(fileStream);
        }
    }
}
