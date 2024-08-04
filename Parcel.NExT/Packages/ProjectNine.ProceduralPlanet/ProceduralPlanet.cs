using HDPlanet;
using Parcel.Types;

namespace ProjectNine.Tooling.Generative
{
    /// <remarks>
    /// This is going to be the main Pure/Parcel friendly entry point
    /// </remarks>
    public static class ProceduralPlanet
    {
        #region Methods
        public static Image GeneratePlanet(int width = 512, int height = 512, int magnification = 5, double longitude = 0.0, double latitude = 0.0, int hGridSize = 10, int vGridSize = 20, MapProjection projection = MapProjection.Mercator)
            => GeneratePlanet(new Random().NextDouble(), width, height, magnification, longitude, latitude, hGridSize, vGridSize, projection);
        public static Image GeneratePlanet(double seed = 0, int width = 512, int height = 512, int magnification = 5, double longitude = 0.0, double latitude = 0.0, int hGridSize = 10, int vGridSize = 20, MapProjection projection = MapProjection.Mercator)
        {
            Dictionary<MapProjection, char> mapping = HDPlanetCLI.GetProjectionParameterMapping();
            return GeneratePlanet(seed, width, height, magnification, longitude, latitude, hGridSize, vGridSize, mapping[projection]);
        }
        #endregion

        #region Routine
        private static Image GeneratePlanet(double seed = 0, int width = 512, int height = 512, int magnification = 5, double longitude = 0.0, double latitude = 0.0, int hGridSize = 10, int vGridSize = 20, char projection = 'm')
        {
            string tempImage = Image.GetTempImagePath();

            // TODO: At the moment both BMP saving and PNG saving seems corrupted
            string arguments = $"-ps -s {seed} -m {magnification} -L {latitude} -l {longitude} -G {hGridSize} -g {vGridSize} -w {width} -h {height} -p {projection} -o {tempImage} -E -b -z";
            new CommandLineParser().ParseArguments(arguments).Run();

            return new Image(tempImage, true);
        }
        #endregion
    }
}
