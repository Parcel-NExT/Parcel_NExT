using FFMpegCore;
using System.IO.Compression;

namespace Parcel.Processing
{
    public static class VideoEditing
    {
        #region Methods
        public static void DownloadDemoVideo(string outputPath)
        {
            DownloadFile("https://test-videos.co.uk/vids/bigbuckbunny/mp4/h264/1080/Big_Buck_Bunny_1080_10s_1MB.mp4", outputPath);
        }
        /// <summary>
        /// Trim/Cut a given video file.
        /// </summary>
        public static void QuickCut(string inputFile, string timeRange, string outputFile)
        {
            InitializeDependency();

            string[] parts = timeRange.Split('-'); // TODO: Error and boundary condition check
            double fromSeconds = ParseSeconds(parts[0]);
            double endSeconds = ParseSeconds(parts[1]);
            FFMpeg.SubVideo(inputFile,
                outputFile,
                TimeSpan.FromSeconds(fromSeconds),
                TimeSpan.FromSeconds(endSeconds)
            );

            static double ParseSeconds(string timeExpression)
            {
                return (TimeOnly.ParseExact(timeExpression, "hh:mm:ss", null) - new TimeOnly(0, 0, 0)).TotalSeconds; // TODO: Refine implementation; This is apparently not super safe because of all the culture stuff and length limit of the hour part
            }
        }
        #endregion

        #region Routines
        private static void InitializeDependency()
        {
            // TODO: Platform dependent downloads

            string utilityFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Parcel NExT", "Tools", "ffmpeg");
            string executablePath = Path.Combine(utilityFolder, "ffmpeg.exe");
            if (!Directory.Exists(utilityFolder) && !File.Exists(executablePath))
            {
                string path = Path.GetTempFileName();
                DownloadFile("https://github.com/ffbinaries/ffbinaries-prebuilt/releases/download/v6.1/ffmpeg-6.1-win-64.zip", path);
                ZipFile.ExtractToDirectory(path, utilityFolder, true);
            }
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Temporary duplicate implementation.
        /// Will use from Parcel.Standard when that package is cleaner.
        /// </summary>
        private static void DownloadFile(string url, string output)
        {
            using HttpClient client = new();
            using Task<Stream> stream = client.GetStreamAsync(url);
            using FileStream fs = new(output, FileMode.OpenOrCreate);
            stream.Result.CopyTo(fs);
        }
        #endregion
    }
}
