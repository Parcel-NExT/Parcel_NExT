using System.Text;
using System.Text.RegularExpressions;

namespace HDPlanet
{
    #region Structs
    public enum FileType
    {
        BMP,
        PPM,
        XPM,
        HeightField
    }
    internal struct Vertex
    {
        public double Height; /* altitude */
        public double Seed; /* seed */
        public double X, Y, Z; /* coordinates */
        public double Shadow; /* approximate rain shadow */
    }
    public class GeneratorConfigurations
    {
        #region Bookkeping
        public string CommandLine { get; internal set; }
        #endregion

        #region Input Output
        public string FileName { get; internal set; }
        public string ColorsName { get; internal set; }
        /// <summary>
        /// Whether write to file or to stdout (deprecated)
        /// </summary>
        public bool WriteToFile { get; internal set; }
        public bool Debug { get; internal set; } = false;
        public string MapFilePath { get; internal set; }
        public bool MatchMap { get; internal set; } = false;
        public double MatchSize { get; internal set; } = 0.1;
        #endregion

        #region Map Type
        public bool NonLinear { get; internal set; }
        public char View { get; internal set; } = 'm';
        public FileType OutputFileType { get; internal set; }
        public double RSeed { get; internal set; } = 0.123;
        public double VGrid { get; internal set; } = 0.0;
        public double HGrid { get; internal set; } = 0.0;
        #endregion

        #region Location
        public double LongiDegrees { get; internal set; } = 0.0;
        public double LatDegrees { get; internal set; } = 0.0;
        public double Scale { get; internal set; } = 1.0;
        public double Rotate2Degrees { get; internal set; }
        public double Rotate1Degrees { get; internal set; }
        #endregion

        #region Resolution
        /// <summary>
        /// Output map size
        /// </summary>
        public int MapWidth { get; internal set; } = 800;
        /// <summary>
        /// Output map size
        /// </summary>
        public int MapHeight { get; internal set; } = 600;
        #endregion

        #region Rendering Style
        /// <summary>
        /// Flag for latitude based colour
        /// </summary>
        public int LatiColor { get; internal set; } = 0;
        /// <summary>
        /// If >0, # of contour lines
        /// </summary>
        public int ContourLines { get; internal set; } = 0;
        /// <summary>
        /// If >0, # of coastal contour lines
        /// </summary>
        public int CoastContourLines { get; internal set; } = 0;
        /// <summary>
        /// If true, draw coastal outline
        /// </summary>
        public bool DoOutline { get; internal set; }
        /// <summary>
        /// If true, reduce map to black outline on white
        /// </summary>
        public bool DoBW { get; internal set; }
        /// <summary>
        /// Default 0
        /// </summary>
        public int DoShade { get; internal set; }
        #endregion

        #region World Characteristics
        /// <summary>
        /// Initial altitude (slightly below sea level)
        /// </summary>
        public double M { get; internal set; } = -0.02;
        /// <summary>
        /// Power for distance function 
        /// </summary>
        public double POW { get; internal set; } = 0.47;
        /// <summary>
        /// Weight for altitude difference
        /// </summary>
        public double DD1 { get; internal set; } = 0.45;
        /// <summary>
        /// Power for altitude difference
        /// </summary>
        public double POWA { get; internal set; } = 1.0;
        /// <summary>
        /// Weight for distance
        /// </summary>
        public double DD2 { get; internal set; } = 0.035;
        #endregion

        #region Additional Styling
        public double ShadeAngle { get; internal set; } = 150.0; /* angle of "light" on bumpmap */
        public double ShadeAngle2 { get; internal set; } = 20.0; /* with daylight shading, these two are longitude/latitude */

        /// <summary>
        /// If true, show temperatures based on latitude and altitude
        /// </summary>
        public bool Temperature { get; internal set; } = false;
        /// <summary>
        /// If true, calculate rainfall based on latitude and temperature
        /// </summary>
        public bool Rainfall { get; internal set; } = false;
        /// <summary>
        /// If true, make biome map
        /// </summary>
        public bool MakeBiomes { get; internal set; } = false;
        #endregion
    }
    #endregion

    public class PlanetGenerator
    {
        #region Constructor
        public GeneratorConfigurations Configurations { get; }
        public PlanetGenerator(GeneratorConfigurations configurations)
            => Configurations = configurations;
        #endregion

        #region Constants
        public const int BLACK = 0;
        public const int WHITE = 1;
        public const int BACK = 2;
        public const int GRID = 3;
        public const int OUTLINE1 = 4;
        public const int OUTLINE2 = 5;
        public const int LOWEST = 6;
        public int SEA { get; private set; } = 7;
        public int LAND { get; private set; } = 8;
        public int HIGHEST { get; private set; } = 9;

        /// <summary>
        /// Character table for XPM output
        /// </summary>
        private static readonly char[] Letters = new char[64]{
        '@','$','.',',',':',';','-','+','=','#','*','&','A','B','C','D',
        'E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T',
        'U','V','W','X','Y','Z','a','b','c','d','e','f','g','h','i','j',
        'k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z'};
        /// <summary>
        /// 45x45 Whittaker diagram
        /// T = tundra, G = grasslands, B = Taiga / boreal forest, D = desert,
        /// S = savanna, F = temperate forest, R = temperate rainforest,
        /// W = Xeric shrubland and dry forest, E = tropical dry forest,
        /// O = tropical rainforest, I = icecap
        /// </summary>
        public static readonly string[] Biomes = new string[45]
        {
            "IIITTTTTGGGGGGGGDDDDDDDDDDDDDDDDDDDDDDDDDDDDD",
            "IIITTTTTGGGGGGGGDDDDGGDSDDSDDDDDDDDDDDDDDDDDD",
            "IITTTTTTTTTBGGGGGGGGGGGSSSSSSDDDDDDDDDDDDDDDD",
            "IITTTTTTTTBBBBBBGGGGGGGSSSSSSSSSWWWWWWWDDDDDD",
            "IITTTTTTTTBBBBBBGGGGGGGSSSSSSSSSSWWWWWWWWWWDD",
            "IIITTTTTTTBBBBBBFGGGGGGSSSSSSSSSSSWWWWWWWWWWW",
            "IIIITTTTTTBBBBBBFFGGGGGSSSSSSSSSSSWWWWWWWWWWW",
            "IIIIITTTTTBBBBBBFFFFGGGSSSSSSSSSSSWWWWWWWWWWW",
            "IIIIITTTTTBBBBBBBFFFFGGGSSSSSSSSSSSWWWWWWWWWW",
            "IIIIIITTTTBBBBBBBFFFFFFGGGSSSSSSSSWWWWWWWWWWW",
            "IIIIIIITTTBBBBBBBFFFFFFFFGGGSSSSSSWWWWWWWWWWW",
            "IIIIIIIITTBBBBBBBFFFFFFFFFFGGSSSSSWWWWWWWWWWW",
            "IIIIIIIIITBBBBBBBFFFFFFFFFFFFFSSSSWWWWWWWWWWW",
            "IIIIIIIIIITBBBBBBFFFFFFFFFFFFFFFSSEEEWWWWWWWW",
            "IIIIIIIIIITBBBBBBFFFFFFFFFFFFFFFFFFEEEEEEWWWW",
            "IIIIIIIIIIIBBBBBBFFFFFFFFFFFFFFFFFFEEEEEEEEWW",
            "IIIIIIIIIIIBBBBBBRFFFFFFFFFFFFFFFFFEEEEEEEEEE",
            "IIIIIIIIIIIIBBBBBBRFFFFFFFFFFFFFFFFEEEEEEEEEE",
            "IIIIIIIIIIIIIBBBBBRRRFFFFFFFFFFFFFFEEEEEEEEEE",
            "IIIIIIIIIIIIIIIBBBRRRRRFFFFFFFFFFFFEEEEEEEEEE",
            "IIIIIIIIIIIIIIIIIBRRRRRRRFFFFFFFFFFEEEEEEEEEE",
            "IIIIIIIIIIIIIIIIIRRRRRRRRRRFFFFFFFFEEEEEEEEEE",
            "IIIIIIIIIIIIIIIIIIRRRRRRRRRRRRFFFFFEEEEEEEEEE",
            "IIIIIIIIIIIIIIIIIIIRRRRRRRRRRRRRFRREEEEEEEEEE",
            "IIIIIIIIIIIIIIIIIIIIIRRRRRRRRRRRRRRRREEEEEEEE",
            "IIIIIIIIIIIIIIIIIIIIIIIRRRRRRRRRRRRRROOEEEEEE",
            "IIIIIIIIIIIIIIIIIIIIIIIIRRRRRRRRRRRROOOOOEEEE",
            "IIIIIIIIIIIIIIIIIIIIIIIIIIRRRRRRRRRROOOOOOEEE",
            "IIIIIIIIIIIIIIIIIIIIIIIIIIIRRRRRRRRROOOOOOOEE",
            "IIIIIIIIIIIIIIIIIIIIIIIIIIIIRRRRRRRROOOOOOOEE",
            "IIIIIIIIIIIIIIIIIIIIIIIIIIIIIRRRRRRROOOOOOOOE",
            "IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIRRRRROOOOOOOOOO",
            "IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIRROOOOOOOOOOO",
            "IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIROOOOOOOOOOO",
            "IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIROOOOOOOOOOO",
            "IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIOOOOOOOOOOO",
            "IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIOOOOOOOOOO",
            "IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIOOOOOOOOO",
            "IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIOOOOOOOOO",
            "IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIOOOOOOOO",
            "IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIOOOOOOOO",
            "IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIOOOOOOOO",
            "IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIOOOOOOOO",
            "IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIOOOOOOO",
            "IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIOOOOOOO"
        };
        public const double PI = 3.14159265358979323846;
        public const double DEG2RAD = 0.01745329251994329576923; // pi/180
        #endregion

        #region Data
        private int[] RTable = new int[65536];
        private int[] GTable = new int[65536];
        private int[] BTtable = new int[65536];

        private ushort[][] Colors;  // Colour Array
        private int[][] Heightfield;         // Heightfield array
        private double[][] GridXXX, GridYYY, GridZZZ; // x,y,z arrays  (used for gridlines)
        private int[] OutX, OutY;
        private ushort[][] Shades; // shade array
        #endregion

        #region States
        private int NumColumns = 65536;
        private Projector Projector;
        #endregion

        #region Main Interface
        public void Run()
        {
            ReadColors(Configurations.ColorsName);

            if (Configurations.OutputFileType == FileType.HeightField)
                InitializeHeightField();
            InitializeColors();
            if (Configurations.DoShade > 0)
                InitializeShades();

            if (Configurations.VGrid != 0.0)
                InitializeVerticalGrid();
            if (Configurations.HGrid != 0.0 || Configurations.VGrid != 0.0)
                InitializeHorizontalGrid();
            MapProjection();

            if (Configurations.DoOutline)
                MakeOutline(Configurations.DoBW);
            if (Configurations.VGrid != 0.0)
                DrawLongitudes();
            if (Configurations.HGrid != 0.0)
                DrawLatitudes();
            if (Configurations.DoShade > 0)
                Smoothshades();
            PlotPicture();

            #region Local Functions
            void InitializeHeightField()
            {
                Heightfield = new int[Configurations.MapWidth][];
                for (int i = 0; i < Configurations.MapWidth; i++)
                    Heightfield[i] = new int[Configurations.MapHeight];
            }
            void InitializeShades()
            {
                Shades = new ushort[Configurations.MapWidth][];
                for (int i = 0; i < Configurations.MapWidth; i++)
                    Shades[i] = new ushort[Configurations.MapHeight];
            }
            void InitializeColors()
            {
                Colors = new ushort[Configurations.MapWidth][];
                for (int i = 0; i < Configurations.MapWidth; i++)
                    Colors[i] = new ushort[Configurations.MapHeight];
            }
            void InitializeVerticalGrid()
            {
                GridXXX = new double[Configurations.MapWidth][];
                for (int i = 0; i < Configurations.MapWidth; i++)
                    GridXXX[i] = new double[Configurations.MapHeight];

                GridZZZ = new double[Configurations.MapWidth][];
                for (int i = 0; i < Configurations.MapWidth; i++)
                    GridZZZ[i] = new double[Configurations.MapHeight];
            }
            void InitializeHorizontalGrid()
            {
                GridYYY = new double[Configurations.MapWidth][];
                for (int i = 0; i < Configurations.MapWidth; i++)
                    GridYYY[i] = new double[Configurations.MapHeight];
            }
            void MapProjection()
            {
                int[][] baseTerrain = null;
                if (Configurations.MatchMap) baseTerrain = ReadBaseMap(ReadMapFile(Configurations.MapFilePath));

                Projector = InitializeProjector(baseTerrain, Configurations);
                Projector.Initialize();
                Projector.Map();
            }
            void DrawLongitudes()
            {
                int i, j;
                for (i = 0; i < Configurations.MapWidth - 1; i++)
                    for (j = 0; j < Configurations.MapHeight - 1; j++)
                    {
                        double t;
                        bool g = false;
                        if (Math.Abs(GridYYY[i][j]) == 1.0) g = true;
                        else
                        {
                            t = Math.Floor((Math.Atan2(GridXXX[i][j], GridZZZ[i][j]) * 180 / PI + 360) / Configurations.VGrid);
                            if (t != Math.Floor((Math.Atan2(GridXXX[i + 1][j], GridZZZ[i + 1][j]) * 180 / PI + 360) / Configurations.VGrid))
                                g = true;
                            if (t != Math.Floor((Math.Atan2(GridXXX[i][j + 1], GridZZZ[i][j + 1]) * 180 / PI + 360) / Configurations.VGrid))
                                g = true;
                        }
                        if (g)
                        {
                            if (Configurations.DoBW) Colors[i][j] = 0; else Colors[i][j] = (ushort)GRID;
                            if (Configurations.DoShade > 0) Shades[i][j] = 255;
                        }
                    }
            }
            void DrawLatitudes()
            {
                int i, j;
                for (i = 0; i < Configurations.MapWidth - 1; i++)
                    for (j = 0; j < Configurations.MapHeight - 1; j++)
                    {
                        double t;
                        bool g = false;
                        t = Math.Floor((Math.Asin(GridYYY[i][j]) * 180 / PI + 360) / Configurations.HGrid);
                        if (t != Math.Floor((Math.Asin(GridYYY[i + 1][j]) * 180 / PI + 360) / Configurations.HGrid))
                            g = true;
                        if (t != Math.Floor((Math.Asin(GridYYY[i][j + 1]) * 180 / PI + 360) / Configurations.HGrid))
                            g = true;
                        if (g)
                        {
                            if (Configurations.DoBW) Colors[i][j] = 0; else Colors[i][j] = (ushort)GRID;
                            if (Configurations.DoShade > 0) Shades[i][j] = 255;
                        }
                    }
            }
            void PlotPicture()
            {
                switch (Configurations.OutputFileType)
                {
                    case FileType.PPM:
                        if (Configurations.DoBW) WritePPMBW(Configurations.FileName, Configurations.CommandLine);
                        else WritePPM(Configurations.FileName, Configurations.CommandLine);
                        break;
                    case FileType.XPM:
                        if (Configurations.DoBW) WriteXPMBW(Configurations.FileName, Configurations.CommandLine);
                        else WriteXPM(Configurations.FileName, Configurations.CommandLine);
                        break;
                    case FileType.BMP:
                        if (Configurations.DoBW) WriteBMPBW(Configurations.FileName, Configurations.CommandLine);
                        else WriteBMP(Configurations.FileName, Configurations.CommandLine);
                        break;
                    case FileType.HeightField:
                        WriteHeights(Configurations.FileName);
                        break;
                }
            }
            #endregion
        }
        #endregion

        #region Helpers
        private Projector InitializeProjector(int[][] baseTerrain, GeneratorConfigurations configurations)
        {
            char outputView = Configurations.View;
            if (outputView == 'c')
            {
                double latRadians = configurations.LatDegrees * DEG2RAD;
                if (latRadians == 0) outputView = 'm';
                /* Conical approaches mercator when lat -> 0 */
                if (Math.Abs(latRadians) >= PI - 0.000001) outputView = 's';
                /* Conical approaches stereo when lat -> +/- 90 */
            }

            Projector projector = null;
            switch (outputView)
            {
                case 'm': /* Mercator projection */
                    projector = new Mercator();
                    break;
                case 'p': /* Peters projection (area preserving cylindrical) */
                    projector = new Peter();
                    break;
                case 'q': /* Square projection (equidistant latitudes) */
                    projector = new SquareP();
                    break;
                case 'M': /* Mollweide projection (area preserving) */
                    projector = new Mollweide();
                    break;
                case 'S': /* Sinusoid projection (area preserving) */
                    projector = new Sinusoid();
                    break;
                case 's': /* Stereographic projection */
                    projector = new Stereo();
                    break;
                case 'o': /* Orthographic projection */
                    projector = new Orthographic();
                    break;
                case 'g': /* Gnomonic projection */
                    projector = new Gnomonic();
                    break;
                case 'i': /* Icosahedral projection */
                    projector = new Icosahedral();
                    break;
                case 'a': /* Area preserving azimuthal projection */
                    projector = new Azimuth();
                    break;
                case 'c': /* Conical projection (conformal) */
                    projector = new Conical();
                    break;
                case 'h': /* heightfield (obsolete) */
                    projector = new Orthographic();
                    break;
            }
            projector.View = outputView;
            projector.Width = configurations.MapWidth;
            projector.Height = configurations.MapHeight;
            projector.LongiDegrees = configurations.LongiDegrees;
            projector.LatDegrees = configurations.LatDegrees;
            projector.Rotate1Degrees = configurations.Rotate1Degrees;
            projector.Rotate2Degrees = configurations.Rotate2Degrees;
            projector.Scale = configurations.Scale;
            projector.VGrid = configurations.VGrid;
            projector.HGrid = configurations.HGrid;
            projector.BaseTerrain = baseTerrain;
            projector.RSeed = configurations.RSeed;

            projector.M = configurations.M;
            projector.POW = configurations.POW;
            projector.DD1 = configurations.DD1;
            projector.POWA = configurations.POWA;
            projector.DD2 = configurations.DD2;

            projector.DoShade = configurations.DoShade;
            projector.NonLinear = configurations.NonLinear;
            projector.OutputFileType = configurations.OutputFileType;
            projector.ShadeAngle = configurations.ShadeAngle;
            projector.ShadeAngle2 = configurations.ShadeAngle2;
            projector.MatchMap = configurations.MatchMap;
            projector.MatchSize = configurations.MatchSize;
            projector.LatiColor = configurations.LatiColor;

            projector.Temperature = configurations.Temperature;
            projector.Rainfall = configurations.Rainfall;
            projector.MakeBiomes = configurations.MakeBiomes;

            projector.BLACK = BLACK;
            projector.WHITE = WHITE;
            projector.BACK = BACK;
            projector.GRID = GRID;
            projector.OUTLINE1 = OUTLINE1;
            projector.OUTLINE2 = OUTLINE2;
            projector.LOWEST = LOWEST;
            projector.SEA = SEA;
            projector.LAND = LAND;
            projector.HIGHEST = HIGHEST;
            projector.PI = PI;
            projector.DEG2RAD = DEG2RAD;
            projector.Debug = configurations.Debug;

            projector.RTable = RTable;
            projector.GTable = GTable;
            projector.BTtable = BTtable;
            projector.Col = Colors;
            projector.HeightField = Heightfield;
            projector.XXX = GridXXX;
            projector.YYY = GridYYY;
            projector.ZZZ = GridZZZ;
            projector.OutX = OutX;
            projector.OutY = OutY;
            projector.Shades = Shades;

            return projector;
        }
        #endregion

        #region Algorithms
        private void Smoothshades()
        {
            int i, j;

            for (i = 0; i < Configurations.MapWidth - 2; i++)
                for (j = 0; j < Configurations.MapHeight - 2; j++)
                    Shades[i][j] = (ushort)((4 * Shades[i][j] + 2 * Shades[i][j + 1] + 2 * Shades[i + 1][j] + Shades[i + 1][j + 1] + 4) / 9);
        }
        #endregion

        #region Routines
        void MakeOutline(bool doBW)
        {
            int i, j, k, t;

            OutX = new int[Configurations.MapWidth * Configurations.MapHeight];
            OutY = new int[Configurations.MapWidth * Configurations.MapHeight];
            k = 0;
            for (i = 1; i < Configurations.MapWidth - 1; i++)
                for (j = 1; j < Configurations.MapHeight - 1; j++)
                    if ((Colors[i][j] >= LOWEST && Colors[i][j] <= SEA) &&
                        (Colors[i - 1][j] >= LAND || Colors[i + 1][j] >= LAND ||
                         Colors[i][j - 1] >= LAND || Colors[i][j + 1] >= LAND ||
                         Colors[i - 1][j - 1] >= LAND || Colors[i - 1][j + 1] >= LAND ||
                         Colors[i + 1][j - 1] >= LAND || Colors[i + 1][j + 1] >= LAND))
                    {
                        /* if point is sea and any neighbour is not, add to outline */
                        OutX[k] = i; OutY[k++] = j;
                    }

            if (Configurations.ContourLines > 0)
            {
                int contourstep = (HIGHEST - LAND) / (Configurations.ContourLines + 1);
                for (i = 1; i < Configurations.MapWidth - 1; i++)
                    for (j = 1; j < Configurations.MapHeight - 1; j++)
                    {
                        t = (Colors[i][j] - LAND) / contourstep;
                        if (Colors[i][j] >= LAND &&
                            ((Colors[i - 1][j] - LAND) / contourstep > t ||
                             (Colors[i + 1][j] - LAND) / contourstep > t ||
                             (Colors[i][j - 1] - LAND) / contourstep > t ||
                             (Colors[i][j + 1] - LAND) / contourstep > t))
                        {
                            /* if point is at contour line and any neighbour is higher */
                            OutX[k] = i; OutY[k++] = j;
                        }
                    }
            }
            if (Configurations.CoastContourLines > 0)
            {
                int contourstep = (LAND - LOWEST) / 20;
                for (i = 1; i < Configurations.MapWidth - 1; i++)
                    for (j = 1; j < Configurations.MapHeight - 1; j++)
                    {
                        t = (Colors[i][j] - LAND) / contourstep;
                        if (Colors[i][j] <= SEA && t >= -Configurations.CoastContourLines &&
                            ((Colors[i - 1][j] - LAND) / contourstep > t ||
                             (Colors[i + 1][j] - LAND) / contourstep > t ||
                             (Colors[i][j - 1] - LAND) / contourstep > t ||
                             (Colors[i][j + 1] - LAND) / contourstep > t))
                        {
                            /* if point is at contour line and any neighbour is higher */
                            OutX[k] = i; OutY[k++] = j;
                        }
                    }
            }
            if (doBW) /* if outline only, clear colours */
                for (i = 0; i < Configurations.MapWidth; i++)
                    for (j = 0; j < Configurations.MapHeight; j++)
                    {
                        if (Colors[i][j] >= LOWEST)
                            Colors[i][j] = (ushort)WHITE;
                        else Colors[i][j] = (ushort)BLACK;
                    }

            /* draw outline (in black if outline only) */
            {
                int contourstep = (HIGHEST - LAND) / (Configurations.ContourLines + 1);
                for (j = 0; j < k; j++)
                {
                    if (doBW) t = BLACK;
                    else
                    {
                        t = Colors[OutX[j]][OutY[j]];
                        if (t != OUTLINE1 && t != OUTLINE2)
                        {
                            if (Configurations.ContourLines > 0 && t >= LAND)
                                if (((t - LAND) / contourstep) % 2 == 1)
                                    t = OUTLINE1;
                                else t = OUTLINE2;
                            else if (t <= SEA)
                                t = OUTLINE1;
                        }
                    }
                    Colors[OutX[j]][OutY[j]] = (ushort)t;
                }
            }
        }
        #endregion

        #region Math Utilities
        public static double Log2(double x)
            => Math.Log(x) / Math.Log(2.0); 
        /// <summary>
        /// Random number generator taking two seeds; rand2(p,q) = rand2(q,p) is important
        /// </summary>
        public static double Rand2(double p, double q)
        {
            double r;
            r = (p + 3.14159265) * (q + 3.14159265);
            return (2.0* (r - (int)r) - 1.0);
        }
        #endregion

        #region File Handling
        private static string[] ReadResourceLines(string resourceName)
        {
            var embeddedAssets = Helper.GetEmbeddedColorResources();
            if (embeddedAssets.ContainsKey(resourceName))
                return Helper.ReadResource(embeddedAssets[resourceName]).Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            return File.ReadAllLines(Path.GetFullPath(resourceName));
        }
        private string[] ReadMapFile(string mapFilePath)
        {
            string path = Path.GetFullPath(mapFilePath);
            return File.ReadAllLines(path);
        }
        /// <summary>
        /// Reads in a map for matching
        /// TODO: This is likely the function we need to modify to enable HD
        /// </summary>
        private static int[][] ReadBaseMap(string[] rows)
        {
            if (rows.Length == 24)
                return ReadBaseMap24x48(rows);
            else 
                return ReadBaseMap12x24(rows);

            static int[][] ReadBaseMap24x48(string[] rows)
            {
                int[][] baseTerrain = new int[60][/*30*/];
                for (int i = 0; i < 60; i++)
                    baseTerrain[i] = new int[30];

                if (rows.Length != 24 || rows.Any(r => r.Length != 48))
                    throw new ArgumentException("Invalid row/column size.");
                for (int row = 0; row < rows.Length; row++)
                {
                    for (int col = 0; col < rows[row].Length; col++)
                    {
                        char symbol = rows[row][col];
                        switch (symbol)
                        {
                            case '.':
                                baseTerrain[col][row] = -8;
                                break;
                            case ',':
                                baseTerrain[col][row] = -6;
                                break;
                            case ':':
                                baseTerrain[col][row] = -4;
                                break;
                            case ';':
                                baseTerrain[col][row] = -2;
                                break;
                            case '-':
                                baseTerrain[col][row] = 0;
                                break;
                            case '*':
                                baseTerrain[col][row] = 2;
                                break;
                            case 'o':
                                baseTerrain[col][row] = 4;
                                break;
                            case 'O':
                                baseTerrain[col][row] = 6;
                                break;
                            case '@':
                                baseTerrain[col][row] = 8;
                                break;
                            default:
                                Console.WriteLine($"Wrong map symbol: {symbol}");
                                break;
                        }
                    }
                }

                return baseTerrain;
            }

            static int[][] ReadBaseMap12x24(string[] rows)
            {
                int inputWidth = 48;    // Actual input is actually 24, the rest is interpolated, and the remaining 12 looks unused? TODO: Make the entire 48 pixels sourced from input
                int inputHeight = 24;    // Actual input is actually 12, the rest is interpolated, and the remaining 8 looks unused? TODO: Make the entire 24 pixels sourced from input

                // "Match base reference text map"; Search map; Defined in colors (heights)
                int[][] baseTerrain = new int[60][/*30*/];
                for (int i = 0; i < 60; i++)
                    baseTerrain[i] = new int[30];

                if (rows.Length != inputHeight / 2 || rows.Any(r => r.Length != inputWidth / 2))
                    throw new ArgumentException("Invalid row/column size.");
                for (int row = 0; row < inputHeight; row += 2)
                {
                    for (int col = 0; col < inputWidth; col += 2)
                    {
                        char symbol = rows[row / 2][col / 2];
                        switch (symbol)
                        {
                            case '.':
                                baseTerrain[col][row] = -8;
                                break;
                            case ',':
                                baseTerrain[col][row] = -6;
                                break;
                            case ':':
                                baseTerrain[col][row] = -4;
                                break;
                            case ';':
                                baseTerrain[col][row] = -2;
                                break;
                            case '-':
                                baseTerrain[col][row] = 0;
                                break;
                            case '*':
                                baseTerrain[col][row] = 2;
                                break;
                            case 'o':
                                baseTerrain[col][row] = 4;
                                break;
                            case 'O':
                                baseTerrain[col][row] = 6;
                                break;
                            case '@':
                                baseTerrain[col][row] = 8;
                                break;
                            default:
                                Console.WriteLine($"Wrong map symbol: {symbol}");
                                break;
                        }
                    }
                }
                /* interpolate */
                for (int j = 1; j < inputHeight; j += 2)    // Interpolate rows
                    for (int i = 0; i < inputWidth; i += 2)
                        baseTerrain[i][j] = (baseTerrain[i][j - 1] + baseTerrain[i][(j + 1)]) / 2;
                for (int j = 0; j < inputHeight; j++)
                    for (int i = 1; i < inputWidth; i += 2)    // Interpolate columns
                        baseTerrain[i][j] = (baseTerrain[i - 1][j] + baseTerrain[(i + 1) % inputWidth][j]) / 2;

                return baseTerrain;
            }
        }
        private void ReadColors(string colorsname)
        {
            /* Format of colour file is a sequence of lines       */
            /* each consisting of four integers:                  */
            /*   colour_number red green blue                     */
            /* where 0 <= colour_number <= 65535                  */
            /* and 0<= red, green, blue <= 255                    */
            /* The colour numbers must be increasing              */
            /* The first colours have special uses:               */
            /* 0 is usually black (0,0,0)                         */
            /* 1 is usually white (255,255,255)                   */
            /* 2 is the background colour                         */
            /* 3 is used for latitude/longitude grid lines        */
            /* 4 and 5 are used for outlines and contour lines    */
            /* 6 upwards are used for altitudes                   */
            /* Halfway between 6 and the max colour is sea level  */
            /* Shallowest sea is (max+6)/2 and land is above this */
            /* With 65536 colours, (max+6)/2 = 32770              */
            /* Colours between specified are interpolated         */

            int crow, cNum = 0, oldcNum, i;
            foreach (string line in ReadResourceLines(colorsname))
            {
                int rValue, gValue, bValue, result = 0;

                oldcNum = cNum;  /* remember last colour number */
                if (Regex.IsMatch(line.Trim(), @"\d+ \d+ \d+ \d+"))
                {
                    var parts = line.Trim().Split(' ');
                    cNum = int.Parse(parts[0]);
                    rValue = int.Parse(parts[1]);
                    gValue = int.Parse(parts[2]);
                    bValue = int.Parse(parts[3]);

                    if (cNum < oldcNum) cNum = oldcNum;
                    if (cNum > 65535) cNum = 65535;
                    RTable[cNum] = rValue;
                    GTable[cNum] = gValue;
                    BTtable[cNum] = bValue;
                    /* interpolate colours between oldcNum and cNum */
                    for (i = oldcNum + 1; i < cNum; i++)
                    {
                        RTable[i] = (RTable[oldcNum] * (cNum - i) + RTable[cNum] * (i - oldcNum))
                                    / (cNum - oldcNum + 1);
                        GTable[i] = (GTable[oldcNum] * (cNum - i) + GTable[cNum] * (i - oldcNum))
                                    / (cNum - oldcNum + 1);
                        BTtable[i] = (BTtable[oldcNum] * (cNum - i) + BTtable[cNum] * (i - oldcNum))
                                    / (cNum - oldcNum + 1);
                    }
                }
            }

            NumColumns = cNum + 1;
            if (NumColumns < 10) NumColumns = 10;

            HIGHEST = NumColumns - 1;
            SEA = (HIGHEST + LOWEST) / 2;
            LAND = SEA + 1;

            for (i = cNum + 1; i < NumColumns; i++)
            {
                /* fill up rest of colour table with last read colour */
                RTable[i] = RTable[cNum];
                GTable[i] = GTable[cNum];
                BTtable[i] = BTtable[cNum];
            }

            if (Configurations.MakeBiomes)
            {
                /* Make biome colours */
                RTable['T' - 64 + LAND] = 210;
                GTable['T' - 64 + LAND] = 210;
                BTtable['T' - 64 + LAND] = 210;
                RTable['G' - 64 + LAND] = 250;
                GTable['G' - 64 + LAND] = 215;
                BTtable['G' - 64 + LAND] = 165;
                RTable['B' - 64 + LAND] = 105;
                GTable['B' - 64 + LAND] = 155;
                BTtable['B' - 64 + LAND] = 120;
                RTable['D' - 64 + LAND] = 220;
                GTable['D' - 64 + LAND] = 195;
                BTtable['D' - 64 + LAND] = 175;
                RTable['S' - 64 + LAND] = 225;
                GTable['S' - 64 + LAND] = 155;
                BTtable['S' - 64 + LAND] = 100;
                RTable['F' - 64 + LAND] = 155;
                GTable['F' - 64 + LAND] = 215;
                BTtable['F' - 64 + LAND] = 170;
                RTable['R' - 64 + LAND] = 170;
                GTable['R' - 64 + LAND] = 195;
                BTtable['R' - 64 + LAND] = 200;
                RTable['W' - 64 + LAND] = 185;
                GTable['W' - 64 + LAND] = 150;
                BTtable['W' - 64 + LAND] = 160;
                RTable['E' - 64 + LAND] = 130;
                GTable['E' - 64 + LAND] = 190;
                BTtable['E' - 64 + LAND] = 25;
                RTable['O' - 64 + LAND] = 110;
                GTable['O' - 64 + LAND] = 160;
                BTtable['O' - 64 + LAND] = 170;
                RTable['I' - 64 + LAND] = 255;
                GTable['I' - 64 + LAND] = 255;
                BTtable['I' - 64 + LAND] = 255;
            }
        }
        /// <summary>
        /// Prints picture in PPM (portable pixel map) format
        /// </summary>
        private void WritePPM(string outfile, string commandLine)
        {
            int i, j, c, s;

            using FileStream stream = File.Open(outfile, FileMode.Create);
            using BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8, false);
            writer.Write("P6\n");
            writer.Write("#fractal planet image\n");
            writer.Write($"# Command line:\n# {commandLine}\n");
            writer.Write($"{Configurations.MapWidth} {Configurations.MapHeight} 255\n");

            if (Configurations.DoShade != 0)
            {
                for (j = 0; j < Configurations.MapHeight; j++)
                {
                    for (i = 0; i < Configurations.MapWidth; i++)
                    {
                        s = Shades[i][j];
                        c = s * RTable[Colors[i][j]] / 150;
                        if (c > 255) c = 255;
                        writer.Write((char)c);
                        c = s * GTable[Colors[i][j]] / 150;
                        if (c > 255) c = 255;
                        writer.Write((char)c);
                        c = s * BTtable[Colors[i][j]] / 150;
                        if (c > 255) c = 255;
                        writer.Write((char)c);
                    }
                }
            }
            else
            {
                for (j = 0; j < Configurations.MapHeight; j++)
                    for (i = 0; i < Configurations.MapWidth; i++)
                    {
                        writer.Write((char)RTable[Colors[i][j]]);
                        writer.Write((char)GTable[Colors[i][j]]);
                        writer.Write((char)BTtable[Colors[i][j]]);
                    }
            }
        }
        /// <summary>
        /// prints picture in b/w PPM format
        /// </summary>
        private void WritePPMBW(string filename, string commandLine)
        {
            int i, j, c;

            using FileStream file = File.Open(filename, FileMode.Create);
            using BinaryWriter writer = new BinaryWriter(file);
            writer.Write("P6\n");
            writer.Write("#fractal planet image\n");
            writer.Write($"# Command line:\n# {commandLine}\n");
            writer.Write($"{Configurations.MapWidth} {Configurations.MapHeight} 1\n");

            for (j = 0; j < Configurations.MapHeight; j++)
                for (i = 0; i < Configurations.MapWidth; i++)
                {
                    if (Colors[i][j] < WHITE)
                        c = 0;
                    else c = 1;
                    writer.Write((byte)c);
                    writer.Write((byte)c);
                    writer.Write((byte)c);
                }
        }
        /// <summary>
        /// Prints picture in BMP format
        /// </summary>
        private void WriteBMP(string filename, string commandLine)
        {
            int i, j, c, s0, s, W1;

            using FileStream file = File.Open(filename, FileMode.Create);
            using BinaryWriter writer = new BinaryWriter(file, Encoding.ASCII, false);

            writer.Write("BM".ToCharArray());

            W1 = (3 * Configurations.MapWidth + 3);
            W1 -= W1 % 4;
            s0 = (commandLine.Length + "Command line:\n\n".Length + 3) & 0xffc;
            s = s0 + 54 + W1 * Configurations.MapHeight; /* file size */
            writer.Write((byte)(s & 255));
            writer.Write((byte)((s >> 8) & 255));
            writer.Write((byte)((s >> 16) & 255));
            writer.Write((byte)(s >> 24));

            writer.Write((byte)0);
            writer.Write((byte)0);
            writer.Write((byte)0);
            writer.Write((byte)0);

            writer.Write((byte)54); /* offset to data */
            writer.Write((byte)0);
            writer.Write((byte)0);
            writer.Write((byte)0);

            writer.Write((byte)40); /* size of infoheader */
            writer.Write((byte)0);
            writer.Write((byte)0);
            writer.Write((byte)0);

            writer.Write((byte)(Configurations.MapWidth & 255));
            writer.Write((byte)((Configurations.MapWidth >> 8) & 255));
            writer.Write((byte)((Configurations.MapWidth >> 16) & 255));
            writer.Write((byte)(Configurations.MapWidth >> 24));

            writer.Write((byte)(Configurations.MapHeight & 255));
            writer.Write((byte)((Configurations.MapHeight >> 8) & 255));
            writer.Write((byte)((Configurations.MapHeight >> 16) & 255));
            writer.Write((byte)(Configurations.MapHeight >> 24));

            writer.Write((byte)1);  /* no. of planes = 1 */
            writer.Write((byte)0);

            writer.Write((byte)24);  /* bpp */
            writer.Write((byte)0);

            writer.Write((byte)0); /* no compression */
            writer.Write((byte)0);
            writer.Write((byte)0);
            writer.Write((byte)0);

            writer.Write((byte)0); /* image size (unspecified) */
            writer.Write((byte)0);
            writer.Write((byte)0);
            writer.Write((byte)0);

            writer.Write((byte)0); /* h. pixels/m */
            writer.Write((byte)32);
            writer.Write((byte)0);
            writer.Write((byte)0);

            writer.Write((byte)0); /* v. pixels/m */
            writer.Write((byte)32);
            writer.Write((byte)0);
            writer.Write((byte)0);

            writer.Write((byte)0); /* colours used (unspecified) */
            writer.Write((byte)0);
            writer.Write((byte)0);
            writer.Write((byte)0);


            writer.Write((byte)0); /* important colours (all) */
            writer.Write((byte)0);
            writer.Write((byte)0);
            writer.Write((byte)0);

            if (Configurations.DoShade != 0)
            {
                for (j = Configurations.MapHeight - 1; j >= 0; j--)
                {
                    for (i = 0; i < Configurations.MapWidth; i++)
                    {
                        s = Shades[i][j];
                        c = s * BTtable[Colors[i][j]] / 150;
                        if (c > 255) c = 255;
                        writer.Write((byte)c);
                        c = s * GTable[Colors[i][j]] / 150;
                        if (c > 255) c = 255;
                        writer.Write((byte)c);
                        c = s * RTable[Colors[i][j]] / 150;
                        if (c > 255) c = 255;
                        writer.Write((byte)c);
                    }
                    for (i = 3 * Configurations.MapWidth; i < W1; i++) writer.Write((byte)0);
                }
            }
            else
            {
                for (j = Configurations.MapHeight - 1; j >= 0; j--)
                {
                    for (i = 0; i < Configurations.MapWidth; i++)
                    {
                        writer.Write((byte)BTtable[Colors[i][j]]);
                        writer.Write((byte)GTable[Colors[i][j]]);
                        writer.Write((byte)RTable[Colors[i][j]]);
                    }
                    for (i = 3 * Configurations.MapWidth; i < W1; i++) writer.Write((byte)0);
                }
            }

            writer.Write($"Command line:\n{commandLine}\n".ToCharArray());
        }
        /// <summary>
        /// Prints picture in b/w BMP format
        /// </summary>
        private void WriteBMPBW(string filename, string commandLine)
        {
            int i, j, c, s, s0, W1;

            using FileStream file = File.Open(filename, FileMode.Create);
            using BinaryWriter writer = new(file);

            writer.Write("BM");

            W1 = (Configurations.MapWidth + 31);
            W1 -= W1 % 32;
            s0 = (commandLine.Length + "Command line:\n\n".Length + 3) & 0xffc;
            s = s0 + 62 + (W1 * Configurations.MapHeight) / 8; /* file size */
            writer.Write(s & 255);
            writer.Write((s >> 8) & 255);
            writer.Write((s >> 16) & 255);
            writer.Write(s >> 24);

            writer.Write(0);
            writer.Write(0);
            writer.Write(0);
            writer.Write(0);

            writer.Write(62); /* offset to data */
            writer.Write(0);
            writer.Write(0);
            writer.Write(0);

            writer.Write(40); /* size of infoheader */
            writer.Write(0);
            writer.Write(0);
            writer.Write(0);

            writer.Write(Configurations.MapWidth & 255);
            writer.Write((Configurations.MapWidth >> 8) & 255);
            writer.Write((Configurations.MapWidth >> 16) & 255);
            writer.Write(Configurations.MapWidth >> 24);

            writer.Write(Configurations.MapHeight & 255);
            writer.Write((Configurations.MapHeight >> 8) & 255);
            writer.Write((Configurations.MapHeight >> 16) & 255);
            writer.Write(Configurations.MapHeight >> 24);

            writer.Write(1);  /* no. of planes = 1 */
            writer.Write(0);

            writer.Write(1);  /* bpp */
            writer.Write(0);

            writer.Write(0); /* no compression */
            writer.Write(0);
            writer.Write(0);
            writer.Write(0);

            writer.Write(0); /* image size (unspecified) */
            writer.Write(0);
            writer.Write(0);
            writer.Write(0);

            writer.Write(0); /* h. pixels/m */
            writer.Write(32);
            writer.Write(0);
            writer.Write(0);

            writer.Write(0); /* v. pixels/m */
            writer.Write(32);
            writer.Write(0);
            writer.Write(0);

            writer.Write(2); /* colours used */
            writer.Write(0);
            writer.Write(0);
            writer.Write(0);


            writer.Write(2); /* important colours (2) */
            writer.Write(0);
            writer.Write(0);
            writer.Write(0);

            writer.Write(0); /* colour 0 = black */
            writer.Write(0);
            writer.Write(0);
            writer.Write(0);

            writer.Write(255); /* colour 1 = white */
            writer.Write(255);
            writer.Write(255);
            writer.Write(255);

            for (j = Configurations.MapHeight - 1; j >= 0; j--)
                for (i = 0; i < W1; i += 8)
                {
                    if (i < Configurations.MapWidth && Colors[i][j] >= WHITE)
                        c = 128;
                    else c = 0;
                    if (i + 1 < Configurations.MapWidth && Colors[i + 1][j] >= WHITE)
                        c += 64;
                    if (i + 2 < Configurations.MapWidth && Colors[i + 2][j] >= WHITE)
                        c += 32;
                    if (i + 3 < Configurations.MapWidth && Colors[i + 3][j] >= WHITE)
                        c += 16;
                    if (i + 4 < Configurations.MapWidth && Colors[i + 4][j] >= WHITE)
                        c += 8;
                    if (i + 5 < Configurations.MapWidth && Colors[i + 5][j] >= WHITE)
                        c += 4;
                    if (i + 6 < Configurations.MapWidth && Colors[i + 6][j] >= WHITE)
                        c += 2;
                    if (i + 7 < Configurations.MapWidth && Colors[i + 7][j] >= WHITE)
                        c += 1;
                    writer.Write(c);
                }

            writer.Write($"Command line:\n{commandLine}\n");
        }
        /// <summary>
        /// prints picture in XPM (X-windows pixel map) format
        /// </summary>
        void WriteXPM(string filename, string commandLine)
        {
            int x, y, i, nbytes;

            x = NumColumns - 1;
            for (nbytes = 0; x != 0; nbytes++)
                x >>= 5;

            using FileStream stream = File.Open(filename, FileMode.Create);
            using BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8, false);
            writer.Write("/* XPM */\n");
            writer.Write($"/* Command line: */\n/* {commandLine}*/\n");
            writer.Write("static char *xpmdata[] = {\n");
            writer.Write("/* width height ncolors chars_per_pixel */\n");
            writer.Write($"\"{Configurations.MapWidth} {Configurations.MapHeight} {NumColumns} {nbytes}\",\n");
            writer.Write("/* colors */\n");
            for (i = 0; i < NumColumns; i++)
                //writer.Write($"\"{nletters(nbytes, i)} c #%2.2X%2.2X%2.2X\",\n",
                //        , rtable[i], gtable[i], btable[i]);

            writer.Write("/* pixels */\n");
            for (y = 0; y < Configurations.MapHeight; y++)
            {
                writer.Write("\"");
                for (x = 0; x < Configurations.MapWidth; x++)
                    writer.Write(NLetters(nbytes, Colors[x][y]));
                writer.Write("\",\n");
            }
            writer.Write("};\n");
        }

        /// <summary>
        /// prints picture in XPM (X-windows pixel map) format
        /// </summary>
        void WriteXPMBW(string filename, string commandLine)
        {
            int x, y, nbytes;

            x = NumColumns - 1;
            nbytes = 1;

            using FileStream stream = File.Open(filename, FileMode.Create);
            using BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8, false);
            writer.Write("/* XPM */\n");
            writer.Write($"/* Command line: */\n/* {commandLine}*/\n");
            writer.Write("static char *xpmdata[] = {\n");
            writer.Write("/* width height ncolors chars_per_pixel */\n");
            writer.Write($"\"{Configurations.MapWidth} {Configurations.MapHeight} {2} {nbytes}\",\n");
            writer.Write("/* colors */\n");

            writer.Write("\". c #FFFFFF\",\n");
            writer.Write("\"X c #000000\",\n");

            writer.Write("/* pixels */\n");
            for (y = 0; y < Configurations.MapHeight; y++)
            {
                writer.Write("\"");
                for (x = 0; x < Configurations.MapWidth; x++)
                    writer.Write((Colors[x][y] < WHITE) ? "X" : ".");
                writer.Write("\",\n");
            }
            writer.Write("};\n");
        }
        /// <summary>
        /// Writes heightfield
        /// </summary>
        private void WriteHeights(string filename)
        {
            int i, j;

            using StreamWriter writer = new StreamWriter(filename);
            for (j = 0; j < Configurations.MapHeight; j++)
            {
                for (i = 0; i < Configurations.MapWidth; i++)
                    writer.Write($"{Heightfield[i][j]} ");
                writer.Write('\n');
            }
        }
        #endregion

        #region String Utilities
        private string NLetters(int n, int c)
        {
            int i;
            char[] buffer = new char[8];

            buffer[n] = '\0';

            for (i = n - 1; i >= 0; i--)
            {
                buffer[i] = Letters[c & 0x001F];
                c >>= 5;
            }

            return new string(buffer);
        }
        #endregion
    }

    internal static class MathHelper
    {
        /// <summary>
        /// Distance squared between vertices
        /// </summary>
        public static double Dist2(this Vertex a, Vertex b)
        {
            double abx, aby, abz;
            abx = a.X - b.X; aby = a.Y - b.Y; abz = a.Z - b.Z;
            return abx * abx + aby * aby + abz * abz;
        }
    }
}
