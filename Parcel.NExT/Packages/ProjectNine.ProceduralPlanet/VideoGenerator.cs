using Emgu.CV;

namespace ProjectNine.Tooling.Generative
{
    public static class VideoGenerator
    {
        public static void GenerateVideo(string? directory = null, string outputVideo = "output_video.mp4")
        {
            directory ??= Directory.GetCurrentDirectory();

            var video = new VideoWriter(outputVideo, 1, new System.Drawing.Size(512, 512), true);
            foreach (var file in Directory.EnumerateFiles(directory)
                .Where(f => Path.GetFileName(f).StartsWith("output_") && Path.GetFileName(f).EndsWith(".bmp")))
            {
                Mat img = CvInvoke.Imread(file);
                video.Write(img);
            }
        }
    }
}
