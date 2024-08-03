using ProjectNine.Tooling.Generative;
using System.Text;

namespace HDPlanet
{
    /// <summary>
    /// CLI program usage
    /// </summary>
    public static class HDPlanetCLI
    {
        #region Methods
        public static void Main(string[] args)
        {
            if (args.Length == 1 && args.First() == "--help")
                PrintHelp();
            else if (args.Length == 0)
                BatchRandomGeneration();
            else
                new CommandLineParser().ParseArguments(args).Run();
        }
        #endregion

        #region Routines
        private static void PrintHelp()
            => Console.WriteLine(GetHelp());
        private static void BatchRandomGeneration()
        {
            var width = 512;
            var height = 512;
            var magnification = 5;
            var longitude = 0.0;
            var latitude = 0.0;
            var hGridSize = 10;
            var vGridSize = 20;
            var projection = 'm';

            for (int i = 0; i < 100; i++)
            {
                var seed = new Random().NextDouble();

                var arguments1 = $"-ps -s {seed} -m {magnification} -L {latitude} -l {longitude} -G {hGridSize} -g {vGridSize} -w {width} -h {height} -p {projection} -o output_{i}.bmp -E -b -z";
                var arguments2 = $"-ps -s {seed} -m 3 -L 30 -l 35 -G 10 -g 20 -w 320 -h 256 -o output_{i}.bmp";

                Console.WriteLine(i);
                new CommandLineParser().ParseArguments(arguments1).Run();
            }
        }
        #endregion

        #region Helpers
        public static string GetHelp()
        {
            StringBuilder helpMessage = new();
            helpMessage.AppendLine("Usage: HDPlanet [options]");
            helpMessage.AppendLine("""
                -s seed            Specifies seed as number between 0.0 and 1.0
                -w width           Specifies width in pixels, default = 800
                -h height          Specifies height in pixels, default = 600
                -m magnification   Specifies magnification, default = 1.0
                -o output-file     Specifies output file, default is standard output
                -l longitude       Specifies longitude of centre in degrees, default = 0.0
                -L latitude        Specifies latitude of centre in degrees, default = 0.0
                -g gridsize        Specifies vertical gridsize in degrees, default = 0.0 (no grid)
                -G gridsize        Specifies horisontal gridsize in degrees, default = 0.0 (no grid)
                -i init-alt        Specifies initial altitude (default = -0.02)
                -c                 Colour depends on latitude (cumulative, default: only altitude)
                -n                 Apply non-linear scaling to altitude. This makes land flatter near sea level
                -S                 Make more “wrinkly” maps
                -C file            Read colour definitions from file
                -O                 Produce a black and white outline map
                -E                 Trace the edges of land in black on colour map
                -B                 Use “bumpmap” shading
                -b                 Use “bumpmap” shading on land only
                -d                 Use “daylight” shading
                -a angle           Angle of “light” in bumpmap shading or longitude of sun in daylight shading
                -A latitude        Latitude of sun in daylight shading
                -P                 Use PPM file format (default is BMP)
                -x                 Use XPM file format (default is BMP)
                -X                 Debug mode
                -H                 Output heightfield (default is BMP)
                -M delta mapfile   Read map from mapfile and match new points to map if edge length greater than delta (default = 0.1)
                -V number          Distance contribution to variation (default = 0.035)
                -v number          Altitude contribution to variation (default = -0.45)
                -T lo la           Rotate map so what would otherwise be at latitude la and longitude lo is moved to (0,0). This is different from using -l and -L because this rotation is done before applying gridlines and latitude-based effects.
                -z                 Show biomes
                -pprojection       Specifies projection
                    m : Mercator (default)
                    p : Peters
                    q : Square
                    s : Stereographic
                    o : Orthographic
                    g : Gnomonic
                    a : Area preserving azimuthal
                    c : Conical (conformal)
                    M : Mollweide
                    S : Sinusoidal
                    h : Heightfield (obsolete. Use -H option instead)
                    i : Icosahedral
                """);
            helpMessage.AppendLine("Example: HDPlanet -s <seed> -m <magnification> -L <latitude> -l <longitude> -G <hGridSize> -g <vGridSize> -w <width> -h <height> -p <projection> -M 0.1 <Reference.map> -o <Output.bmp> -E -b -z");
            helpMessage.AppendLine("See (original) Manual.txt for details");
            return helpMessage.ToString().TrimEnd();
        }
        #endregion

        #region CLI Queries
        public static Dictionary<string, string> GetArguments()
        {
            return new()
            {
                { "-s seed", "Specifies seed as number between 0.0 and 1.0" },
                { "-w width", "Specifies width in pixels, default = 800" },
                { "-h height", "Specifies height in pixels, default = 600" },
                { "-m magnification", "Specifies magnification, default = 1.0" },
                { "-o output-file", "Specifies output file, default is standard output" },
                { "-l longitude", "Specifies longitude of centre in degrees, default = 0.0" },
                { "-L latitude", "Specifies latitude of centre in degrees, default = 0.0" },
                { "-g gridsize", "Specifies vertical gridsize in degrees, default = 0.0 (no grid)" },
                { "-G gridsize", "Specifies horisontal gridsize in degrees, default = 0.0 (no grid)" },
                { "-i init-alt", "Specifies initial altitude (default = -0.02)" },
                { "-c", "Colour depends on latitude (cumulative, default: only altitude)" },
                { "-n", "Apply non-linear scaling to altitude. This makes land flatter near sea level" },
                { "-S", "Make more “wrinkly” maps" },
                { "-C file", "Read colour definitions from file" },
                { "-O", "Produce a black and white outline map" },
                { "-E", "Trace the edges of land in black on colour map" },
                { "-B", "Use “bumpmap” shading" },
                { "-b", "Use “bumpmap” shading on land only" },
                { "-d", "Use “daylight” shading" },
                { "-a angle", "Angle of “light” in bumpmap shading or longitude of sun in daylight shading" },
                { "-A latitude", "Latitude of sun in daylight shading" },
                { "-P", "Use PPM file format (default is BMP)" },
                { "-x", "Use XPM file format (default is BMP)" },
                { "-X", "Enable debug mode" },
                { "-H", "Output heightfield (default is BMP)" },
                { "-M delta mapfile", "Read map from mapfile and match new points to map if edge length greater than delta (default = 0.1)" },
                { "-V number", "Distance contribution to variation (default = 0.035)" },
                { "-v number", "Altitude contribution to variation (default = -0.45)" },
                { "-T lo la", "Rotate map so what would otherwise be at latitude la and longitude lo is moved to (0,0). This is different from using -l and -L because this rotation is done before applying gridlines and latitude-based effects." },
                { "-z", "Show biomes" },
                { "-pprojection", "Specifies projection" }
            };
        }
        public static Dictionary<char, string> GetProjections()
        {
            return new ()
            {
                { 'm', "Mercator projection" },
                { 'p', "Peters projection (area preserving cylindrical)" },
                { 'q', "Square projection (equidistant latitudes)" },
                { 'M', "Mollweide projection (area preserving)" },
                { 'S', "Sinusoid projection (area preserving)" },
                { 's', "Stereographic projection" },
                { 'o', "Orthographic projection" },
                { 'g', "Gnomonic projection" },
                { 'i', "Icosahedral projection" },
                { 'a', "Area preserving azimuthal projection" },
                { 'c', "Conical projection (conformal)" },
                { 'h', "heightfield (obsolete)" },
            };
        }
        #endregion
    }
}