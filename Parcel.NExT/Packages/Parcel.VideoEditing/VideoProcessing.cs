using Parcel.CoreEngine.Helpers;
using Parcel.Standard.Network;
using System.Runtime.InteropServices;

namespace Parcel.Processing
{
    public enum DownloadSource
    {
        BtbN,
        gyan
    }
    public enum FeatureSet
    {
        Essential,
        Full
    }
    public static class VideoProcessing
    {
        #region Dependency Management
        public static string FFMPEGExecutableName { get; } = "ffmpeg";
        /// <summary>
        /// Downloads core dependency (ffmpeg) into target location.
        /// </summary>
        public static void DownloadDependency(string destination, bool overwrite = false, bool addToUserEnv = true, DownloadSource source = DownloadSource.BtbN, FeatureSet featureSet = FeatureSet.Full)
        {
            destination = Path.GetFullPath(destination);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                throw new NotImplementedException();
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                throw new NotImplementedException();
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                string downloadLink = source switch
                {
                    DownloadSource.gyan => featureSet switch { FeatureSet.Full => @"https://www.gyan.dev/ffmpeg/builds/ffmpeg-release-full.7z", FeatureSet.Essential => @"https://www.gyan.dev/ffmpeg/builds/ffmpeg-release-essentials.7z", _ => throw new ArgumentException($"Invalid feature set: {featureSet}") },
                    DownloadSource.BtbN => @"https://github.com/BtbN/FFmpeg-Builds/releases/download/latest/ffmpeg-master-latest-win64-gpl.zip",
                    _ => throw new ArgumentException($"Invalid source: {source}")
                };

                string directory = Path.GetDirectoryName(destination);
                Directory.CreateDirectory(directory);
                if (!File.Exists(destination) || overwrite)
                    NetworkHelper.DownloadFile(downloadLink, destination);

                // Unzip
                // ...

                // Add to env
                // ...
            }
        }
        public static string GetFFMPEGVersion()
        {
            string? ffmpeg = EnvironmentVariableHelper.FindProgram(FFMPEGExecutableName);
            if (ffmpeg != null)
            {
                string outputs = ProcessHelper.GetOutput(ffmpeg, "-version");
                return outputs
                    .Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .First()
                    .Replace("ffmpeg", string.Empty, StringComparison.InvariantCultureIgnoreCase)
                    .Replace("version", string.Empty, StringComparison.InvariantCultureIgnoreCase)
                    .Trim()
                    .Split(' ')
                    .First()
                    .Trim();
            }
            return null;
        }
        #endregion

        #region Helper
        private static void ValidateDependencies()
        {
            // Check ffmpeg availability
            string? ffmpeg = EnvironmentVariableHelper.FindProgram(FFMPEGExecutableName)
                ?? throw new FileNotFoundException($"Cannot find ffmpeg on current computer.");

            // Check ffmpeg version
            string? version = GetFFMPEGVersion();
            string[] validMajorVersions = ["3.10", "3.11", "3.12", "3.13"];
            if (!validMajorVersions.Any(version.Contains))
                throw new InvalidProgramException($"Expects a ffmpeg version in: {string.Join(", ", validMajorVersions)}");
        }
        #endregion
    }
}
