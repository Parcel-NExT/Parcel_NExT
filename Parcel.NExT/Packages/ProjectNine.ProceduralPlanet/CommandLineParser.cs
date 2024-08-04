using HDPlanet;

namespace ProjectNine.Tooling.Generative
{
    /// <summary>
    /// Provide single-line command line parsing experience; Conforms to original planet.exe usage
    /// </summary>
    internal sealed class CommandLineParser
    {
        #region Methods
        public PlanetGenerator ParseArguments(string commandLine)
            => ParseArguments(commandLine.Split(" "));
        public PlanetGenerator ParseArguments(string[] arguments)
        {
            GeneratorConfigurations configurations = new();
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

                            // Automatic file type detection based on extension
                            string extension = Path.GetExtension(configurations.FileName);
                            if (extension == ".png")
                                configurations.OutputFileType = FileType.PNG;
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
                            configurations.ShadeMode = ShadeMode.BumpMap; /* bump map */
                            break;
                        case 'b':
                            configurations.ShadeMode = ShadeMode.BumpMapLandOnly; /* bump map on land only */
                            break;
                        case 'd':
                            configurations.ShadeMode = ShadeMode.DaylightShading; /* daylight shading */
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
        private static string GetFileExtension(FileType fileType)
        {
            switch (fileType)
            {
                case FileType.BMP:
                    return (".bmp");
                case FileType.PNG:
                    return (".png");
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
