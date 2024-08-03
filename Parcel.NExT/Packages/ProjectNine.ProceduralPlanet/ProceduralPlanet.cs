using Parcel.Types;

namespace ProjectNine.Tooling.Generative
{
    /// <remarks>
    /// This is going to be the main Pure/Parcel friendly entry point
    /// </remarks>
    public static class ProceduralPlanet
    {
        public static Image GeneratePlanet(int width = 512, int height = 512, int magnification = 5, double longitude = 0.0, double latitude = 0.0, int hGridSize = 10, int vGridSize = 20, char projection = 'm')
        {
            double seed = new Random().NextDouble();
            string tempImage = Image.GetTempImagePath();

            string arguments = $"-ps -s {seed} -m {magnification} -L {latitude} -l {longitude} -G {hGridSize} -g {vGridSize} -w {width} -h {height} -p {projection} -o {tempImage} -E -b -z";
            new CommandLineParser().ParseArguments(arguments).Run();

            return new Image(tempImage, true);
        }
    }
}
