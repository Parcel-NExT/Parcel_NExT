namespace HDPlanet
{
    public static class HDPlanetProgram
    {
        #region Main
        internal static void Main(string[] args)
        {
            if (args.Length == 1 && args.First() == "--help")
                PrintHelp();
            else if (args.Length == 0)
                BatchRandomGeneration();
            else
                new CommandLineParser().ParseArguments(args).Run();
        }
        internal static void PrintHelp()
        {
            Console.WriteLine("Usage: HDPlanet [options]");
            Console.WriteLine("""
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
            Console.WriteLine("Example: HDPlanet -s <seed> -m <magnification> -L <latitude> -l <longitude> -G <hGridSize> -g <vGridSize> -w <width> -h <height> -p <projection> -M 0.1 <Reference.map> -o <Output.bmp> -E -b -z");
            Console.WriteLine("See (original) Manual.txt for details");
        }
        internal static void BatchRandomGeneration()
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

    public sealed class CommandLineParser
    {
        #region Methods
        public PlanetGenerator ParseArguments(string commandLine)
            => ParseArguments(commandLine.Split(" "));
        public PlanetGenerator ParseArguments(string[] arguments)
        {
            GeneratorConfigurations configurations = new GeneratorConfigurations();
            configurations.CommandLine = string.Join(" ", arguments);
            configurations.FileName = "planet-map";
            configurations.ColorsName = "Olsson.col";
            configurations.WriteToFile = false;

            configurations.LongiDegrees = 0.0;
            configurations.LatDegrees = 0.0;
            configurations.Scale = 1.0;
            configurations.RSeed = 0.123;
            configurations.View = 'm';
            configurations.VGrid = configurations.HGrid = 0.0;

            for (int i = 0; i < arguments.Length; i++)
            {
                if (arguments[i][0] == '-')
                {
                    switch (arguments[i][1])
                    {
                        case 'X':
                            configurations.Debug = true;
                            break;
                        case 'V':
                            configurations.DD2 = double.Parse(arguments[++i]);
                            break;
                        case 'v':
                            configurations.DD1 = double.Parse(arguments[++i]);
                            break;
                        case 's':
                            configurations.RSeed = double.Parse(arguments[++i]);
                            break;
                        case 'w':
                            configurations.MapWidth = int.Parse(arguments[++i]);
                            break;
                        case 'h':
                            configurations.MapHeight = int.Parse(arguments[++i]);
                            break;
                        case 'm':
                            configurations.Scale = double.Parse(arguments[++i]);
                            break;
                        case 'o':
                            configurations.FileName = arguments[++i];
                            configurations.WriteToFile = true;
                            break;
                        case 'x':
                            configurations.OutputFileType = FileType.XPM;
                            break;
                        case 'C':
                            configurations.ColorsName = arguments[++i];
                            break;
                        case 'l':
                            configurations.LongiDegrees = double.Parse(arguments[++i]);
                            while (configurations.LongiDegrees < -180) configurations.LongiDegrees += 360;
                            while (configurations.LongiDegrees > 180) configurations.LongiDegrees -= 360;
                            break;
                        case 'L':
                            configurations.LatDegrees = double.Parse(arguments[++i]);
                            if (configurations.LatDegrees < -90) configurations.LatDegrees = -90;
                            if (configurations.LatDegrees > 90) configurations.LatDegrees = 90;
                            break;
                        case 'g':
                            configurations.VGrid = double.Parse(arguments[++i]);
                            break;
                        case 'G':
                            configurations.HGrid = double.Parse(arguments[++i]);
                            break;
                        case 'c':
                            configurations.LatiColor += 1;
                            break;
                        case 'S':
                            configurations.DD1 /= 2.0;
                            configurations.POWA = 0.75;
                            break;
                        case 'n':
                            configurations.NonLinear = true;
                            break;
                        case 'O':
                            configurations.DoOutline = true;
                            configurations.DoBW = true;
                            if (arguments[i].Length > 2)
                            {
                                // Format: -O%d
                                int tmp = int.Parse(arguments[i].Substring(2));
                                if (tmp < 0) configurations.CoastContourLines = -tmp;
                                else configurations.ContourLines = tmp;
                            }
                            break;
                        case 'E':
                            configurations.DoOutline = true;
                            if (arguments[i].Length > 2)
                            {
                                // Format: -E%d
                                int tmp = int.Parse(arguments[i].Substring(2));
                                if (tmp < 0) configurations.CoastContourLines = -tmp;
                                else configurations.ContourLines = tmp;
                            }
                            break;
                        case 'B':
                            configurations.DoShade = 1; /* bump map */
                            break;
                        case 'b':
                            configurations.DoShade = 2; /* bump map on land only */
                            break;
                        case 'd':
                            configurations.DoShade = 3; /* daylight shading */
                            break;
                        case 'P':
                            configurations.OutputFileType = FileType.PPM;
                            break;
                        case 'H':
                            configurations.OutputFileType = FileType.HeightField;
                            break;
                        case 'M':
                            configurations.MatchMap = true;
                            configurations.MatchSize = double.Parse(arguments[++i]);
                            configurations.MapFilePath = arguments[++i];
                            break;
                        case 'a':
                            configurations.ShadeAngle = double.Parse(arguments[++i]);
                            break;
                        case 'A':
                            configurations.ShadeAngle2 = double.Parse(arguments[++i]);
                            break;
                        case 'i':
                            configurations.M = double.Parse(arguments[++i]);
                            break;
                        case 'T':
                            configurations.Rotate2Degrees = double.Parse(arguments[++i]);
                            configurations.Rotate1Degrees = double.Parse(arguments[++i]);
                            while (configurations.Rotate1Degrees < -180) configurations.Rotate1Degrees += 360;
                            while (configurations.Rotate1Degrees > 180) configurations.Rotate1Degrees -= 360;
                            while (configurations.Rotate2Degrees < -180) configurations.Rotate2Degrees += 360;
                            while (configurations.Rotate2Degrees > 180) configurations.Rotate2Degrees += 360;
                            break;
                        case 't': configurations.Temperature = true; break;
                        case 'r': configurations.Rainfall = true; break;
                        case 'z': configurations.MakeBiomes = true; break;
                        case 'p':
                            if (arguments[i].Length > 2) configurations.View = arguments[i][2];
                            else configurations.View = arguments[++i][0];
                            switch (configurations.View)
                            {
                                case 'm':
                                case 'p':
                                case 'q':
                                case 's':
                                case 'o':
                                case 'g':
                                case 'a':
                                case 'c':
                                case 'M':
                                case 'S':
                                case 'i':
                                case 'f': break;
                                case 'h': configurations.OutputFileType = FileType.HeightField; break;
                                default:
                                    Console.WriteLine($"Unknown projection: {arguments[i]}");
                                    break;
                            }
                            break;
                        default:
                            Console.WriteLine($"Unknown option: {arguments[i]}");
                            break;
                    }
                }
                else
                    Console.WriteLine($"Unknown option: {arguments[i]}");
            }

            // Either write to file or write as binary to STD
            if (configurations.WriteToFile && !string.IsNullOrWhiteSpace(configurations.FileName))
                configurations.FileName = !configurations.FileName.Contains('.') ? configurations.FileName + GetFileExtension(configurations.OutputFileType) : configurations.FileName;
            else
                configurations.FileName = Path.GetTempFileName();

            return new PlanetGenerator(configurations);
        }
        private string GetFileExtension(FileType fileType)
        {
            switch (fileType)
            {
                case FileType.BMP:
                    return (".bmp");
                case FileType.PPM:
                    return (".ppm");
                case FileType.XPM:
                    return (".xpm");
                case FileType.HeightField:
                    return (".heightfield");
                default:
                    return ("");
            }
        }
        #endregion
    }
}