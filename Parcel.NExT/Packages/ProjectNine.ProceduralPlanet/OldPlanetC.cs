using HDPlanet;
using System.Text;
using System.Text.RegularExpressions;

namespace PlanetCSharp
{
    [Obsolete("Gallery piece. Archive only.")]
    internal class OldPlanetC
    {
        #region Constants
        int BLACK = 0;
        int WHITE = 1;
        int BACK = 2;
        int GRID = 3;
        int OUTLINE1 = 4;
        int OUTLINE2 = 5;
        int LOWEST = 6;
        int SEA = 7;
        int LAND = 8;
        int HIGHEST = 9;

        /// <summary>
        /// Character table for XPM output
        /// </summary>
        char[] letters = new char[64]{
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
        string[] biomes = new string[45]
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
        double PI = 3.14159265358979323846;
        double DEG2RAD = 0.01745329251994329576923; // pi/180
        #endregion

        #region Structs
        private enum ftype
        {
            bmp,
            ppm,
            xpm,
            heightfield
        }
        public struct Vertex
        {
            public double h; /* altitude */
            public double s; /* seed */
            public double x, y, z; /* coordinates */
            public double shadow; /* approximate rain shadow */
        }
        #endregion

        #region Data
        int[] rtable = new int[65536];
        int[] gtable = new int[65536];
        int[] btable = new int[65536];

        ushort[][] col;  /* colour array */
        int[][] heights;         /* heightfield array */
        double[][] xxx, yyy, zzz; /* x,y,z arrays  (used for gridlines */
        int[][] cl0 = new int[60][/*30*/]; /* search map */
        int[] outx, outy;
        ushort[][] shades; /* shade array */

        bool debug = false;
        bool nonLinear = false;
        char view;

        /* For the vertices of the tetrahedron */
        Vertex[] tetra = new Vertex[4];

        int nocols = 65536;
        string cmdLine;
        #endregion

        #region Configurations
        ftype file_type = ftype.bmp;

        /* these three values can be changed to change world characteristics */

        double M = -0.02;   /* initial altitude (slightly below sea level) */
        double dd1 = 0.45;  /* weight for altitude difference */
        double POWA = 1.0; /* power for altitude difference */
        double dd2 = 0.035; /* weight for distance */
        double POW = 0.47;  /* power for distance function */

        int Depth; /* depth of subdivisions */
        double r1, r2, r3, r4; /* seeds */
        double longi, lat, scale;
        double vgrid, hgrid;

        int latic = 0; /* flag for latitude based colour */

        int Width = 800, Height = 600; /* default map size */
        
        bool do_outline = false;  /* if true, draw coastal outline */
        bool do_bw = false;       /* if true, reduce map to black outline on white */
        int contourLines = 0; /* if >0, # of contour lines */
        int coastContourLines = 0; /* if >0, # of coastal contour lines */

        int doshade = 0;
        int shade;
        double shade_angle = 150.0; /* angle of "light" on bumpmap */
        double shade_angle2 = 20.0; /* with daylight shading, these two are longitude/latitude */

        double cla, sla, clo, slo, rseed;

        bool temperature = false; /* if true, show temperatures based on latitude and altitude*/
        double tempMin = 1000.0, tempMax = -1000.0;

        bool rainfall = false; /* if true, calculate rainfall based on latitude and temperature */
        double rainMin = 1000.0, rainMax = -1000.0;

        double rainShadow = 0.0; /* approximate rain shadow */

        bool makeBiomes = false; /* if true, make biome map */

        bool matchMap = false;
        double matchSize = 0.1;
        #endregion

        #region Interface
        public void Run(string[] arguments)
        {
            double rotate1 = 0.0, rotate2 = 0.0;
            double cR1, sR1, cR2, sR2;

            // Main routine
            string filename = "planet-map";
            string colorsname = "Olsson.col";
            bool do_file = false;   // Whether write to file or to stdout
            int tmp = 0;
            double tx, ty, tz;

            /* initialize vertices to slightly irregular tetrahedron */
            tetra[0].x = -Math.Sqrt(3.0) - 0.20;
            tetra[0].y = -Math.Sqrt(3.0) - 0.22;
            tetra[0].z = -Math.Sqrt(3.0) - 0.23;

            tetra[1].x = -Math.Sqrt(3.0) - 0.19;
            tetra[1].y = Math.Sqrt(3.0) + 0.18;
            tetra[1].z = Math.Sqrt(3.0) + 0.17;

            tetra[2].x = Math.Sqrt(3.0) + 0.21;
            tetra[2].y = -Math.Sqrt(3.0) - 0.24;
            tetra[2].z = Math.Sqrt(3.0) + 0.15;

            tetra[3].x = Math.Sqrt(3.0) + 0.24;
            tetra[3].y = Math.Sqrt(3.0) + 0.22;
            tetra[3].z = -Math.Sqrt(3.0) - 0.25;

            longi = 0.0;
            lat = 0.0;
            scale = 1.0;
            rseed = 0.123;
            view = 'm';
            vgrid = hgrid = 0.0;

            cmdLine = string.Join(" ", arguments);
            for (int i = 0; i < arguments.Length; i++)
            {
                if (arguments[i][0] == '-')
                {
                    switch (arguments[i][1])
                    {
                        case 'X':
                            debug = true;
                            break;
                        case 'V':
                            dd2 = double.Parse(arguments[++i]);
                            break;
                        case 'v':
                            dd1 = double.Parse(arguments[++i]);
                            break;
                        case 's':
                            rseed = double.Parse(arguments[++i]);
                            break;
                        case 'w':
                            Width = int.Parse(arguments[++i]);
                            break;
                        case 'h':
                            Height = int.Parse(arguments[++i]);
                            break;
                        case 'm':
                            scale = double.Parse(arguments[++i]);
                            break;
                        case 'o':
                            filename = arguments[++i];
                            do_file = true;
                            break;
                        case 'x':
                            file_type = ftype.xpm;
                            break;
                        case 'C':
                            colorsname = arguments[++i];
                            break;
                        case 'l':
                            longi = double.Parse(arguments[++i]);
                            while (longi < -180) longi += 360;
                            while (longi > 180) longi -= 360;
                            break;
                        case 'L':
                            lat = double.Parse(arguments[++i]);
                            if (lat < -90) lat = -90;
                            if (lat > 90) lat = 90;
                            break;
                        case 'g':
                            vgrid = double.Parse(arguments[++i]);
                            break;
                        case 'G':
                            hgrid = double.Parse(arguments[++i]);
                            break;
                        case 'c':
                            latic += 1;
                            break;
                        case 'S':
                            dd1 /= 2.0; POWA = 0.75;
                            break;
                        case 'n':
                            nonLinear = true;
                            break;
                        case 'O':
                            do_outline = true;
                            do_bw = true;
                            if (arguments[i].Length > 2)
                            {
                                // Format: -O%d
                                tmp = int.Parse(arguments[i].Substring(2));
                                if (tmp < 0) coastContourLines = -tmp;
                                else contourLines = tmp;
                            }
                            break;
                        case 'E':
                            do_outline = true;
                            if (arguments[i].Length > 2)
                            {
                                // Format: -E%d
                                tmp = int.Parse(arguments[i].Substring(2));
                                if (tmp < 0) coastContourLines = -tmp;
                                else contourLines = tmp;
                            }
                            break;
                        case 'B':
                            doshade = 1; /* bump map */
                            break;
                        case 'b':
                            doshade = 2; /* bump map on land only */
                            break;
                        case 'd':
                            doshade = 3; /* daylight shading */
                            break;
                        case 'P':
                            file_type = ftype.ppm;
                            break;
                        case 'H':
                            file_type = ftype.heightfield;
                            break;
                        case 'M':
                            matchMap = true;
                            matchSize = double.Parse(arguments[++i]);
                            break;
                        case 'a':
                            shade_angle = double.Parse(arguments[++i]);
                            break;
                        case 'A':
                            shade_angle2 = double.Parse(arguments[++i]);
                            break;
                        case 'i':
                            M = double.Parse(arguments[++i]);
                            break;
                        case 'T':
                            rotate2 = double.Parse(arguments[++i]);
                            rotate1 = double.Parse(arguments[++i]);
                            while (rotate1 < -180) rotate1 += 360;
                            while (rotate1 > 180) rotate1 -= 360;
                            while (rotate2 < -180) rotate2 += 360;
                            while (rotate2 > 180) rotate2 += 360;
                            break;
                        case 't': temperature = true; break;
                        case 'r': rainfall = true; break;
                        case 'z': makeBiomes = true; break;
                        case 'p':
                            if (arguments[i].Length > 2) view = arguments[i][2];
                            else view = arguments[++i][0];
                            switch (view)
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
                                case 'h': file_type = ftype.heightfield; break;
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
                {
                    Console.WriteLine($"Unknown option: {arguments[i]}");
                }
            }

            readcolors(colorsname);

            // Either write to file or write as binary to STD
            if (do_file && !string.IsNullOrWhiteSpace(filename))
                filename = !filename.Contains('.') ? filename + file_ext(file_type) : filename;
            else
                filename = Path.GetTempFileName();

            longi = longi * DEG2RAD;
            lat = lat * DEG2RAD;

            sla = Math.Sin(lat); cla = Math.Cos(lat);
            slo = Math.Sin(longi); clo = Math.Cos(longi);

            rotate1 = -rotate1 * DEG2RAD;
            rotate2 = -rotate2 * DEG2RAD;

            sR1 = Math.Sin(rotate1); cR1 = Math.Cos(rotate1);
            sR2 = Math.Sin(rotate2); cR2 = Math.Cos(rotate2);

            for (int i = 0; i < 4; i++)
            { /* rotate around y axis */
                tx = tetra[i].x;
                ty = tetra[i].y;
                tz = tetra[i].z;
                tetra[i].x = cR2 * tx + sR2 * tz;
                tetra[i].y = ty;
                tetra[i].z = -sR2 * tx + cR2 * tz;
            }

            for (int i = 0; i < 4; i++)
            { /* rotate around x axis */
                tx = tetra[i].x;
                ty = tetra[i].y;
                tz = tetra[i].z;
                tetra[i].x = tx;
                tetra[i].y = cR1 * ty - sR1 * tz;
                tetra[i].z = sR1 * ty + cR1 * tz;
            }

            if (matchMap) readmap();

            if (file_type == ftype.heightfield)
            {
                heights = new int[Width][];
                for (int i = 0; i < Width; i++)
                    heights[i] = new int[Height];
            }

            col = new ushort[Width][];
            for (int i = 0; i < Width; i++)
                col[i] = new ushort[Height];

            if (doshade > 0)
            {
                shades = new ushort[Width][];
                for (int i = 0; i < Width; i++)
                    shades[i] = new ushort[Height];
            }

            if (vgrid != 0.0)
            {
                xxx = new double[Width][];
                for (int i = 0; i < Width; i++)
                    xxx[i] = new double[Height];

                zzz = new double[Width][];
                for (int i = 0; i < Width; i++)
                    zzz[i] = new double[Height];
            }

            if (hgrid != 0.0 || vgrid != 0.0)
            {
                yyy = new double[Width][];
                for (int i = 0; i < Width; i++)
                    yyy[i] = new double[Height];
            }

            if (view == 'c')
            {
                if (lat == 0) view = 'm';
                /* Conical approaches mercator when lat -> 0 */
                if (Math.Abs(lat) >= PI - 0.000001) view = 's';
                /* Conical approaches stereo when lat -> +/- 90 */
            }

            Depth = 3 * ((int)(log_2(scale * Height))) + 6;

            r1 = rseed;

            r1 = rand2(r1, r1);
            r2 = rand2(r1, r1);
            r3 = rand2(r1, r2);
            r4 = rand2(r2, r3);

            tetra[0].s = r1;
            tetra[1].s = r2;
            tetra[2].s = r3;
            tetra[3].s = r4;

            tetra[0].h = M;
            tetra[1].h = M;
            tetra[2].h = M;
            tetra[3].h = M;

            tetra[0].shadow = 0.0;
            tetra[1].shadow = 0.0;
            tetra[2].shadow = 0.0;
            tetra[3].shadow = 0.0;

            switch (view)
            {
                case 'm': /* Mercator projection */
                    mercator();
                    break;
                case 'p': /* Peters projection (area preserving cylindrical) */
                    peter();
                    break;
                case 'q': /* Square projection (equidistant latitudes) */
                    squarep();
                    break;
                case 'M': /* Mollweide projection (area preserving) */
                    mollweide();
                    break;
                case 'S': /* Sinusoid projection (area preserving) */
                    sinusoid();
                    break;
                case 's': /* Stereographic projection */
                    stereo();
                    break;
                case 'o': /* Orthographic projection */
                    orthographic();
                    break;
                case 'g': /* Gnomonic projection */
                    gnomonic();
                    break;
                case 'i': /* Icosahedral projection */
                    icosahedral();
                    break;
                case 'a': /* Area preserving azimuthal projection */
                    azimuth();
                    break;
                case 'c': /* Conical projection (conformal) */
                    conical();
                    break;
                case 'h': /* heightfield (obsolete) */
                    orthographic();
                    break;
            }

            if (do_outline) makeoutline(do_bw);

            if (vgrid != 0.0)
            { 
                /* draw longitudes */
                int i, j;
                for (i = 0; i < Width - 1; i++)
                    for (j = 0; j < Height - 1; j++)
                    {
                        double t;
                        bool g = false;
                        if (Math.Abs(yyy[i][j]) == 1.0) g = true;
                        else
                        {
                            t = Math.Floor((Math.Atan2(xxx[i][j], zzz[i][j]) * 180 / PI + 360) / vgrid);
                            if (t != Math.Floor((Math.Atan2(xxx[i + 1][j], zzz[i + 1][j]) * 180 / PI + 360) / vgrid))
                                g = true;
                            if (t != Math.Floor((Math.Atan2(xxx[i][j + 1], zzz[i][j + 1]) * 180 / PI + 360) / vgrid))
                                g = true;
                        }
                        if (g)
                        {
                            if (do_bw) col[i][j] = 0; else col[i][j] = (ushort)GRID;
                            if (doshade > 0) shades[i][j] = 255;
                        }
                    }
            }

            if (hgrid != 0.0)
            { /* draw latitudes */
                int i, j;
                for (i = 0; i < Width - 1; i++)
                    for (j = 0; j < Height - 1; j++)
                    {
                        double t;
                        bool g = false;
                        t = Math.Floor((Math.Asin(yyy[i][j]) * 180 / PI + 360) / hgrid);
                        if (t != Math.Floor((Math.Asin(yyy[i + 1][j]) * 180 / PI + 360) / hgrid))
                            g = true;
                        if (t != Math.Floor((Math.Asin(yyy[i][j + 1]) * 180 / PI + 360) / hgrid))
                            g = true;
                        if (g)
                        {
                            if (do_bw) col[i][j] = 0; else col[i][j] = (ushort)GRID;
                            if (doshade > 0) shades[i][j] = 255;
                        }
                    }
            }

            if (doshade > 0) smoothshades();

            /* plot picture */
            switch (file_type)
            {
                case ftype.ppm:
                    if (do_bw) printppmBW(filename);
                    else printppm(filename);
                    break;
                case ftype.xpm:
                    if (do_bw) printxpmBW(filename);
                    else printxpm(filename);
                    break;
                case ftype.bmp:
                    if (do_bw) printbmpBW(filename);
                    else printbmp(filename);
                    break;
                case ftype.heightfield:
                    printheights(filename);
                    break;
            }
        }
        #endregion

        #region Helpers
        void print_error()
        {
            Console.WriteLine("Usage: planet [options]");
            Console.WriteLine("See Manual.txt for details");
        }
        string file_ext(ftype file_type)
        {
            switch (file_type)
            {
                case ftype.bmp:
                    return (".bmp");
                case ftype.ppm:
                    return (".ppm");
                case ftype.xpm:
                    return (".xpm");
                case ftype.heightfield:
                    return (".heightfield");
                default:
                    return ("");
            }
        }
        #endregion

        #region Algorithms
        void smoothshades()
        {
            int i, j;

            for (i = 0; i < Width - 2; i++)
                for (j = 0; j < Height - 2; j++)
                    shades[i][j] = (ushort)((4 * shades[i][j] + 2 * shades[i][j + 1] + 2 * shades[i + 1][j] + shades[i + 1][j + 1] + 4) / 9);
        }

        void peter()
        {
            double y, cos2, theta1, scale1;
            int k, i, j, water, land;

            y = 2.0 * Math.Sin(lat);
            k = (int)(0.5 * y * Width * scale / PI + 0.5);
            water = land = 0;
            for (j = 0; j < Height; j++)
            {
                if (debug && ((j % (Height / 25)) == 0))
                    Console.WriteLine(view);
                y = 0.5 * PI * (2.0 * (j - k) - Height) / Width / scale;
                if (Math.Abs(y) > 1.0)
                    for (i = 0; i < Width; i++)
                    {
                        col[i][j] = (ushort)BACK;
                        if (doshade > 0) shades[i][j] = 255;
                    }
                else
                {
                    cos2 = Math.Sqrt(1.0 - y * y);
                    if (cos2 > 0.0)
                    {
                        scale1 = scale * Width / Height / cos2 / PI;
                        Depth = 3 * ((int)(log_2(scale1 * Height))) + 3;
                        for (i = 0; i < Width; i++)
                        {
                            theta1 = longi - 0.5 * PI + PI * (2.0 * i - Width) / Width / scale;
                            planet0(Math.Cos(theta1) * cos2, y, -Math.Sin(theta1) * cos2, i, j);
                            if (col[i][j] < LAND) water++; else land++;
                        }
                    }
                }
            }
            if (debug)
                Console.WriteLine($"water percentage: {100 * water / (water + land)}\n");
        }

        void squarep()
        {
            double y, scale1, theta1, cos2;
            int k, i, j;

            k = (int)(0.5 * lat * Width * scale / PI + 0.5);
            for (j = 0; j < Height; j++)
            {
                if (debug && ((j % (Height / 25)) == 0))
                    Console.WriteLine(view);
                y = (2.0 * (j - k) - Height) / Width / scale * PI;
                if (Math.Abs(y + y) > PI)
                    for (i = 0; i < Width; i++)
                    {
                        col[i][j] = (ushort)BACK;
                        if (doshade > 0) shades[i][j] = 255;
                    }
                else
                {
                    cos2 = Math.Cos(y);
                    if (cos2 > 0.0)
                    {
                        scale1 = scale * Width / Height / cos2 / PI;
                        Depth = 3 * ((int)(log_2(scale1 * Height))) + 3;
                        for (i = 0; i < Width; i++)
                        {
                            theta1 = longi - 0.5 * PI + PI * (2.0 * i - Width) / Width / scale;
                            planet0(Math.Cos(theta1) * cos2, Math.Sin(y), -Math.Sin(theta1) * cos2, i, j);
                        }
                    }
                }
            }
        }
        void mercator()
        {
            double y, scale1, cos2, theta1;
            int i, j, k;

            y = Math.Sin(lat);
            y = (1.0 + y) / (1.0 - y);
            y = 0.5 * Math.Log(y);
            k = (int)(0.5 * y * Width * scale / PI + 0.5);
            for (j = 0; j < Height; j++)
            {
                if (debug && ((j % (Height / 25)) == 0))
                    Console.WriteLine(view);
                y = PI * (2.0 * (j - k) - Height) / Width / scale;
                y = Math.Exp(2.0 * y);
                y = (y - 1.0) / (y + 1.0);
                scale1 = scale * Width / Height / Math.Sqrt(1.0 - y * y) / PI;
                cos2 = Math.Sqrt(1.0 - y * y);
                Depth = 3 * ((int)(log_2(scale1 * Height))) + 3;
                for (i = 0; i < Width; i++)
                {
                    theta1 = longi - 0.5 * PI + PI * (2.0 * i - Width) / Width / scale;
                    planet0(Math.Cos(theta1) * cos2, y, -Math.Sin(theta1) * cos2, i, j);
                }
            }
        }

        void mollweide()
        {
            double y, y1, zz, scale1, cos2, theta1, theta2;
            int i, j, i1 = 1, k;

            for (j = 0; j < Height; j++)
            {
                if (debug && ((j % (Height / 25)) == 0))
                    Console.WriteLine(view);
                y1 = 2 * (2.0 * j - Height) / Width / scale;
                if (Math.Abs(y1) >= 1.0) for (i = 0; i < Width; i++)
                    {
                        col[i][j] = (ushort)BACK;
                        if (doshade > 0) shades[i][j] = 255;
                    }
                else
                {
                    zz = Math.Sqrt(1.0 - y1 * y1);
                    y = 2.0 / PI * (y1 * zz + Math.Asin(y1));
                    cos2 = Math.Sqrt(1.0 - y * y);
                    if (cos2 > 0.0)
                    {
                        scale1 = scale * Width / Height / cos2 / PI;
                        Depth = 3 * ((int)(log_2(scale1 * Height))) + 3;
                        for (i = 0; i < Width; i++)
                        {
                            theta1 = PI / zz * (2.0 * i - Width) / Width / scale;
                            if (Math.Abs(theta1) > PI)
                            {
                                col[i][j] = (ushort)BACK;
                                if (doshade > 0) shades[i][j] = 255;
                            }
                            else
                            {
                                double x2, y2, z2, x3, y3, z3;
                                theta1 += -0.5 * PI;
                                x2 = Math.Cos(theta1) * cos2;
                                y2 = y;
                                z2 = -Math.Sin(theta1) * cos2;
                                x3 = clo * x2 + slo * sla * y2 + slo * cla * z2;
                                y3 = cla * y2 - sla * z2;
                                z3 = -slo * x2 + clo * sla * y2 + clo * cla * z2;

                                planet0(x3, y3, z3, i, j);
                            }
                        }
                    }
                }
            }
        }

        void sinusoid()
        {
            double y, theta1, theta2, cos2, l1, i1, scale1;
            int k, i, j, l, c;

            k = (int)(lat * Width * scale / PI + 0.5);
            for (j = 0; j < Height; j++)
            {
                if (debug && ((j % (Height / 25)) == 0))
                    Console.WriteLine(view);
                y = (2.0 * (j - k) - Height) / Width / scale * PI;
                if (Math.Abs(y + y) > PI) for (i = 0; i < Width; i++)
                    {
                        col[i][j] = (ushort)BACK;
                        if (doshade > 0) shades[i][j] = 255;
                    }
                else
                {
                    cos2 = Math.Cos(y);
                    if (cos2 > 0.0)
                    {
                        scale1 = scale * Width / Height / cos2 / PI;
                        Depth = 3 * ((int)(log_2(scale1 * Height))) + 3;
                        for (i = 0; i < Width; i++)
                        {
                            l = (int)(i * 12 / Width / scale);
                            l1 = l * Width * scale / 12.0;
                            i1 = i - l1;
                            theta2 = longi - 0.5 * PI + PI * (2.0 * l1 - Width) / Width / scale;
                            theta1 = (PI * (2.0 * i1 - Width * scale / 12.0) / Width / scale) / cos2;
                            if (Math.Abs(theta1) > PI / 12.0)
                            {
                                col[i][j] = (ushort)BACK;
                                if (doshade > 0) shades[i][j] = 255;
                            }
                            else
                            {
                                planet0(Math.Cos(theta1 + theta2) * cos2, Math.Sin(y), -Math.Sin(theta1 + theta2) * cos2,
                                        i, j);
                            }
                        }
                    }
                }
            }
        }

        void stereo()
        {
            double x, y, ymin, ymax, z, zz, x1, y1, z1, theta1, theta2;
            int i, j;

            ymin = 2.0;
            ymax = -2.0;
            for (j = 0; j < Height; j++)
            {
                if (debug && ((j % (Height / 25)) == 0))
                    Console.WriteLine(view);
                for (i = 0; i < Width; i++)
                {
                    x = (2.0 * i - Width) / Height / scale;
                    y = (2.0 * j - Height) / Height / scale;
                    z = x * x + y * y;
                    zz = 0.25 * (4.0 + z);
                    x = x / zz;
                    y = y / zz;
                    z = (1.0 - 0.25 * z) / zz;
                    x1 = clo * x + slo * sla * y + slo * cla * z;
                    y1 = cla * y - sla * z;
                    z1 = -slo * x + clo * sla * y + clo * cla * z;
                    if (y1 < ymin) ymin = y1;
                    if (y1 > ymax) ymax = y1;

                    /* for level-of-detail effect:
                       Depth = 3*((int)(log_2(scale*Height)/(1.0+x1*x1+y1*y1)))+6; */

                    planet0(x1, y1, z1, i, j);
                }
            }
        }

        void orthographic()
        {
            double x, y, z, x1, y1, z1, ymin, ymax, theta1, theta2, zz;
            int i, j;

            ymin = 2.0;
            ymax = -2.0;
            for (j = 0; j < Height; j++)
            {
                if (debug && ((j % (Height / 25)) == 0))
                    Console.WriteLine(view);
                for (i = 0; i < Width; i++)
                {
                    x = (2.0 * i - Width) / Height / scale;
                    y = (2.0 * j - Height) / Height / scale;
                    if (x * x + y * y > 1.0)
                    {
                        col[i][j] = (ushort)BACK;
                        if (doshade > 0) shades[i][j] = 255;
                    }
                    else
                    {
                        z = Math.Sqrt(1.0 - x * x - y * y);
                        x1 = clo * x + slo * sla * y + slo * cla * z;
                        y1 = cla * y - sla * z;
                        z1 = -slo * x + clo * sla * y + clo * cla * z;
                        if (y1 < ymin) ymin = y1;
                        if (y1 > ymax) ymax = y1;
                        planet0(x1, y1, z1, i, j);
                    }
                }
            }
        }

        void icosahedral() /* modified version of gnomonic */
        {
            double x, y, z, x1, y1, z1, zz, theta1, theta2, ymin, ymax;
            int i, j;
            double lat1, longi1, sla, cla, slo, clo, x0, y0, sq3_4, sq3;
            double L1, L2, S;

            ymin = 2.0;
            ymax = -2.0;
            sq3 = Math.Sqrt(3.0);
            L1 = 10.812317; /* theoretically 10.9715145571469; */
            L2 = -52.622632; /* theoretically -48.3100310579607; */
            S = 55.6; /* found by experimentation */
            for (j = 0; j < Height; j++)
            {
                if (debug && ((j % (Height / 25)) == 0))
                    Console.WriteLine(view);
                for (i = 0; i < Width; i++)
                {

                    x0 = 198.0 * (2.0 * i - Width) / Width / scale - 36;
                    y0 = 198.0 * (2.0 * j - Height) / Width / scale - lat / DEG2RAD;

                    longi1 = 0.0;
                    lat1 = 500.0;
                    if (y0 / sq3 <= 18.0 && y0 / sq3 >= -18.0)
                    { /* middle row of triangles */
                        /* upward triangles */
                        if (x0 - y0 / sq3 < 144.0 && x0 + y0 / sq3 >= 108.0)
                        {
                            lat1 = -L1;
                            longi1 = 126.0;
                        }
                        else if (x0 - y0 / sq3 < 72.0 && x0 + y0 / sq3 >= 36.0)
                        {
                            lat1 = -L1;
                            longi1 = 54.0;
                        }
                        else if (x0 - y0 / sq3 < 0.0 && x0 + y0 / sq3 >= -36.0)
                        {
                            lat1 = -L1;
                            longi1 = -18.0;
                        }
                        else if (x0 - y0 / sq3 < -72.0 && x0 + y0 / sq3 >= -108.0)
                        {
                            lat1 = -L1;
                            longi1 = -90.0;
                        }
                        else if (x0 - y0 / sq3 < -144.0 && x0 + y0 / sq3 >= -180.0)
                        {
                            lat1 = -L1;
                            longi1 = -162.0;
                        }

                        /* downward triangles */
                        else if (x0 + y0 / sq3 < 108.0 && x0 - y0 / sq3 >= 72.0)
                        {
                            lat1 = L1;
                            longi1 = 90.0;
                        }
                        else if (x0 + y0 / sq3 < 36.0 && x0 - y0 / sq3 >= 0.0)
                        {
                            lat1 = L1;
                            longi1 = 18.0;
                        }
                        else if (x0 + y0 / sq3 < -36.0 && x0 - y0 / sq3 >= -72.0)
                        {
                            lat1 = L1;
                            longi1 = -54.0;
                        }
                        else if (x0 + y0 / sq3 < -108.0 && x0 - y0 / sq3 >= -144.0)
                        {
                            lat1 = L1;
                            longi1 = -126.0;
                        }
                        else if (x0 + y0 / sq3 < -180.0 && x0 - y0 / sq3 >= -216.0)
                        {
                            lat1 = L1;
                            longi1 = -198.0;
                        }
                    }

                    if (y0 / sq3 > 18.0)
                    { /* bottom row of triangles */
                        if (x0 + y0 / sq3 < 180.0 && x0 - y0 / sq3 >= 72.0)
                        {
                            lat1 = L2;
                            longi1 = 126.0;
                        }
                        else if (x0 + y0 / sq3 < 108.0 && x0 - y0 / sq3 >= 0.0)
                        {
                            lat1 = L2;
                            longi1 = 54.0;
                        }
                        else if (x0 + y0 / sq3 < 36.0 && x0 - y0 / sq3 >= -72.0)
                        {
                            lat1 = L2;
                            longi1 = -18.0;
                        }
                        else if (x0 + y0 / sq3 < -36.0 && x0 - y0 / sq3 >= -144.0)
                        {
                            lat1 = L2;
                            longi1 = -90.0;
                        }
                        else if (x0 + y0 / sq3 < -108.0 && x0 - y0 / sq3 >= -216.0)
                        {
                            lat1 = L2;
                            longi1 = -162.0;
                        }
                    }
                    if (y0 / sq3 < -18.0)
                    { /* top row of triangles */
                        if (x0 - y0 / sq3 < 144.0 && x0 + y0 / sq3 >= 36.0)
                        {
                            lat1 = -L2;
                            longi1 = 90.0;
                        }
                        else if (x0 - y0 / sq3 < 72.0 && x0 + y0 / sq3 >= -36.0)
                        {
                            lat1 = -L2;
                            longi1 = 18.0;
                        }
                        else if (x0 - y0 / sq3 < 0.0 && x0 + y0 / sq3 >= -108.0)
                        {
                            lat1 = -L2;
                            longi1 = -54.0;
                        }
                        else if (x0 - y0 / sq3 < -72.0 && x0 + y0 / sq3 >= -180.0)
                        {
                            lat1 = -L2;
                            longi1 = -126.0;
                        }
                        else if (x0 - y0 / sq3 < -144.0 && x0 + y0 / sq3 >= -252.0)
                        {
                            lat1 = -L2;
                            longi1 = -198.0;
                        }
                    }

                    if (lat1 > 400.0)
                    {
                        col[i][j] = (ushort)BACK;
                        if (doshade > 0) shades[i][j] = 255;
                    }
                    else
                    {
                        x = (x0 - longi1) / S;
                        y = (y0 + lat1) / S;

                        longi1 = longi1 * DEG2RAD - longi;
                        lat1 = lat1 * DEG2RAD;

                        sla = Math.Sin(lat1); cla = Math.Cos(lat1);
                        slo = Math.Sin(longi1); clo = Math.Cos(longi1);

                        zz = Math.Sqrt(1.0 / (1.0 + x * x + y * y));
                        x = x * zz;
                        y = y * zz;
                        z = Math.Sqrt(1.0 - x * x - y * y);
                        x1 = clo * x + slo * sla * y + slo * cla * z;
                        y1 = cla * y - sla * z;
                        z1 = -slo * x + clo * sla * y + clo * cla * z;

                        if (y1 < ymin) ymin = y1;
                        if (y1 > ymax) ymax = y1;
                        planet0(x1, y1, z1, i, j);
                    }
                }
            }
        }

        void gnomonic()
        {
            double x, y, z, x1, y1, z1, zz, theta1, theta2, ymin, ymax;
            int i, j;

            ymin = 2.0;
            ymax = -2.0;
            for (j = 0; j < Height; j++)
            {
                if (debug && ((j % (Height / 25)) == 0))
                    Console.WriteLine(view);
                for (i = 0; i < Width; i++)
                {
                    x = (2.0 * i - Width) / Height / scale;
                    y = (2.0 * j - Height) / Height / scale;
                    zz = Math.Sqrt(1.0 / (1.0 + x * x + y * y));
                    x = x * zz;
                    y = y * zz;
                    z = Math.Sqrt(1.0 - x * x - y * y);
                    x1 = clo * x + slo * sla * y + slo * cla * z;
                    y1 = cla * y - sla * z;
                    z1 = -slo * x + clo * sla * y + clo * cla * z;
                    if (y1 < ymin) ymin = y1;
                    if (y1 > ymax) ymax = y1;
                    planet0(x1, y1, z1, i, j);
                }
            }
        }

        void azimuth()
        {
            double x, y, z, x1, y1, z1, zz, theta1, theta2, ymin, ymax;
            int i, j;

            ymin = 2.0;
            ymax = -2.0;
            for (j = 0; j < Height; j++)
            {
                if (debug && ((j % (Height / 25)) == 0))
                Console.WriteLine(view);
                for (i = 0; i < Width; i++)
                {
                    x = (2.0 * i - Width) / Height / scale;
                    y = (2.0 * j - Height) / Height / scale;
                    zz = x * x + y * y;
                    z = 1.0 - 0.5 * zz;
                    if (z < -1.0)
                    {
                        col[i][j] = (ushort)BACK;
                        if (doshade > 0) shades[i][j] = 255;
                    }
                    else
                    {
                        zz = Math.Sqrt(1.0 - 0.25 * zz);
                        x = x * zz;
                        y = y * zz;
                        x1 = clo * x + slo * sla * y + slo * cla * z;
                        y1 = cla * y - sla * z;
                        z1 = -slo * x + clo * sla * y + clo * cla * z;
                        if (y1 < ymin) ymin = y1;
                        if (y1 > ymax) ymax = y1;
                        planet0(x1, y1, z1, i, j);
                    }
                }
            }
        }

        void conical()
        {
            double k1, c, y2, x, y, zz, x1, y1, z1, theta1, theta2, ymin, ymax, cos2;
            int i, j;

            ymin = 2.0;
            ymax = -2.0;
            if (lat > 0)
            {
                k1 = 1.0 / Math.Sin(lat);
                c = k1 * k1;
                y2 = Math.Sqrt(c * (1.0 - Math.Sin(lat / k1)) / (1.0 + Math.Sin(lat / k1)));
                for (j = 0; j < Height; j++)
                {
                    if (debug && ((j % (Height / 25)) == 0))
                        Console.WriteLine(view);
                    for (i = 0; i < Width; i++)
                    {
                        x = (2.0 * i - Width) / Height / scale;
                        y = (2.0 * j - Height) / Height / scale + y2;
                        zz = x * x + y * y;
                        if (zz == 0.0) theta1 = 0.0; else theta1 = k1 * Math.Atan2(x, y);
                        if (theta1 < -PI || theta1 > PI)
                        {
                            col[i][j] = (ushort)BACK;
                            if (doshade > 0) shades[i][j] = 255;
                        }
                        else
                        {
                            theta1 += longi - 0.5 * PI; /* theta1 is longitude */
                            theta2 = k1 * Math.Asin((zz - c) / (zz + c));
                            /* theta2 is latitude */
                            if (theta2 > 0.5 * PI || theta2 < -0.5 * PI)
                            {
                                col[i][j] = (ushort)BACK;
                                if (doshade > 0) shades[i][j] = 255;
                            }
                            else
                            {
                                cos2 = Math.Cos(theta2);
                                y = Math.Sin(theta2);
                                if (y < ymin) ymin = y;
                                if (y > ymax) ymax = y;
                                planet0(Math.Cos(theta1) * cos2, y, -Math.Sin(theta1) * cos2, i, j);
                            }
                        }
                    }
                }
            }
            else
            {
                k1 = 1.0 / Math.Sin(lat);
                c = k1 * k1;
                y2 = Math.Sqrt(c * (1.0 - Math.Sin(lat / k1)) / (1.0 + Math.Sin(lat / k1)));
                for (j = 0; j < Height; j++)
                {
                    if (debug && ((j % (Height / 25)) == 0))
                    Console.WriteLine(view);
                    for (i = 0; i < Width; i++)
                    {
                        x = (2.0 * i - Width) / Height / scale;
                        y = (2.0 * j - Height) / Height / scale - y2;
                        zz = x * x + y * y;
                        if (zz == 0.0) theta1 = 0.0; else theta1 = -k1 * Math.Atan2(x, -y);
                        if (theta1 < -PI || theta1 > PI)
                        {
                            col[i][j] = (ushort)BACK;
                            if (doshade > 0) shades[i][j] = 255;
                        }
                        else
                        {
                            theta1 += longi - 0.5 * PI; /* theta1 is longitude */
                            theta2 = k1 * Math.Asin((zz - c) / (zz + c));
                            /* theta2 is latitude */
                            if (theta2 > 0.5 * PI || theta2 < -0.5 * PI)
                            {
                                col[i][j] = (ushort)BACK;
                                if (doshade > 0) shades[i][j] = 255;
                            }
                            else
                            {
                                cos2 = Math.Cos(theta2);
                                y = Math.Sin(theta2);
                                if (y < ymin) ymin = y;
                                if (y > ymax) ymax = y;
                                planet0(Math.Cos(theta1) * cos2, y, -Math.Sin(theta1) * cos2, i, j);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// High level function that gets (average) altitude closest to target point and performs colording using procedural rules
        /// </summary>
        void planet0(double x, double y, double z, int i, int j)
        {
            double alt, y2, sun, temp, rain;
            int colour;

            alt = planet1(x, y, z);

            /* calculate temperature based on altitude and latitude */
            /* scale: -0.1 to 0.1 corresponds to -30 to +30 degrees Celsius */
            sun = Math.Sqrt(1.0 - y * y); /* approximate amount of sunlight at
			     latitude ranged from 0.1 to 1.1 */
            if (alt < 0) temp = sun / 8.0 + alt * 0.3; /* deep water colder */
            else temp = sun / 8.0 - alt * 1.2; /* high altitudes colder */

            if (temp < tempMin && alt > 0) tempMin = temp;
            if (temp > tempMax && alt > 0) tempMax = temp;
            if (temperature) alt = temp - 0.05;

            /* calculate rainfall based on temperature and latitude */
            /* rainfall approximately proportional to temperature but reduced
               near horse latitudes (+/- 30 degrees, y=0.5) and reduced for
               rain shadow */
            y2 = Math.Abs(y) - 0.5;
            rain = temp * 0.65 + 0.1 - 0.011 / (y2 * y2 + 0.1);
            rain += 0.03 * rainShadow;
            if (rain < 0.0) rain = 0.0;

            if (rain < rainMin && alt > 0) rainMin = rain;
            if (rain > rainMax && alt > 0) rainMax = rain;

            if (rainfall) alt = rain - 0.02;

            if (nonLinear)
            {
                /* non-linear scaling to make flatter near sea level */
                alt = alt * alt * alt * 300;
            }
            /* store height for heightfield */
            if (file_type == ftype.heightfield) heights[i][j] = (int)(10000000 * alt);

            y2 = y * y; y2 = y2 * y2; y2 = y2 * y2;

            /* calculate colour */

            if (makeBiomes)
            { /* make biome colours */
                int tt = min(44, max(0, (int)(rain * 300.0 - 9)));
                int rr = min(44, max(0, (int)(temp * 300.0 + 10)));
                char bio = biomes[tt][rr];
                if (alt <= 0.0)
                {
                    colour = SEA + (int)((SEA - LOWEST + 1) * (10 * alt));
                    if (colour < LOWEST) colour = LOWEST;
                }
                else colour = bio - 64 + LAND;  /* from LAND+2 to LAND+23 */
            }
            else if (alt <= 0.0)
            { 
                /* if below sea level then */
                if (latic > 0 && y2 + alt >= 1.0 - 0.02 * latic * latic)
                    colour = HIGHEST;  /* icecap if close to poles */
                else
                {
                    colour = SEA + (int)((SEA - LOWEST + 1) * (10 * alt));
                    if (colour < LOWEST) colour = LOWEST;
                }
            }
            else
            {
                if (latic != 0) alt += 0.1 * latic * y2;  /* altitude adjusted with latitude */
                if (alt >= 0.1) /* if high then */
                    colour = HIGHEST;
                else
                {
                    colour = LAND + (int)((HIGHEST - LAND + 1) * (10 * alt));
                    if (colour > HIGHEST) colour = HIGHEST;
                }
            }

            /* store colour */
            col[i][j] = (ushort)colour;

            /* store (x,y,z) coordinates for grid drawing */
            if (vgrid != 0.0)
            {
                xxx[i][j] = x;
                zzz[i][j] = z;
            }
            if (hgrid != 0.0 || vgrid != 0.0) yyy[i][j] = y;

            /* store shading info */
            if (doshade > 0) shades[i][j] = (ushort)shade;

            return;
        }

        Vertex ssa, ssb, ssc, ssd;

        /// <summary>
        /// Performs recursive subdivision and boundary check function; This is the key function as described in the algorithm
        /// </summary>
        double planet(/* tetrahedron vertices */Vertex a, Vertex b, Vertex c, Vertex d, /* goal point */ double x, double y, double z, /* levels to go */ int level)
        {
            Vertex e;
            double lab, lac, lad, lbc, lbd, lcd, maxlength;
            double es1, es2, es3;
            double eax, eay, eaz, epx, epy, epz;
            double ecx, ecy, ecz, edx, edy, edz;
            double x1, y1, z1, x2, y2, z2, l1, tmp;

            if (level > 0)
            {

                /* make sure ab is longest edge */
                lab = MathHelper.dist2(a, b);
                lac = MathHelper.dist2(a, c);
                lad = MathHelper.dist2(a, d);
                lbc = MathHelper.dist2(b, c);
                lbd = MathHelper.dist2(b, d);
                lcd = MathHelper.dist2(c, d);

                maxlength = lab;
                if (lac > maxlength) maxlength = lac;
                if (lad > maxlength) maxlength = lad;
                if (lbc > maxlength) maxlength = lbc;
                if (lbd > maxlength) maxlength = lbd;
                if (lcd > maxlength) maxlength = lcd;

                // This code is doing sorting
                if (lac == maxlength) return (planet(a, c, b, d, x, y, z, level));
                if (lad == maxlength) return (planet(a, d, b, c, x, y, z, level));
                if (lbc == maxlength) return (planet(b, c, a, d, x, y, z, level));
                if (lbd == maxlength) return (planet(b, d, a, c, x, y, z, level));
                if (lcd == maxlength) return (planet(c, d, a, b, x, y, z, level));

                if (level == 11)
                { /* save tetrahedron for caching */
                    ssa = a; ssb = b; ssc = c; ssd = d;
                }

                /* ab is longest, so cut ab */
                e.s = rand2(a.s, b.s);
                es1 = rand2(e.s, e.s);
                es2 = 0.5 + 0.1 * rand2(es1, es1);  /* find cut point */
                es3 = 1.0 - es2;

                if (a.s < b.s)
                {
                    e.x = es2 * a.x + es3 * b.x; e.y = es2 * a.y + es3 * b.y; e.z = es2 * a.z + es3 * b.z;
                }
                else if (a.s > b.s)
                {
                    e.x = es3 * a.x + es2 * b.x; e.y = es3 * a.y + es2 * b.y; e.z = es3 * a.z + es2 * b.z;
                }
                else
                { /* as==bs, very unlikely to ever happen */
                    e.x = 0.5 * a.x + 0.5 * b.x; e.y = 0.5 * a.y + 0.5 * b.y; e.z = 0.5 * a.z + 0.5 * b.z;
                }

                /* new altitude is: */
                if (matchMap && lab > matchSize)
                { /* use map height */
                    double l, xx, yy;
                    l = Math.Sqrt(e.x * e.x + e.y * e.y + e.z * e.z);
                    yy = Math.Asin(e.y / l) * 23 / PI + 11.5;
                    xx = Math.Atan2(e.x, e.z) * 23.5 / PI + 23.5;
                    e.h = cl0[(int)(xx + 0.5)][(int)(yy + 0.5)] * 0.1 / 8.0;
                }
                else
                {
                    if (lab > 1.0) lab = Math.Pow(lab, 0.5);
                    /* decrease contribution for very long distances */
                    e.h = 0.5 * (a.h + b.h) /* average of end points */
                      + e.s * dd1 * Math.Pow(Math.Abs(a.h - b.h), POWA)
                      /* plus contribution for altitude diff */
                      + es1 * dd2 * Math.Pow(lab, POW); /* plus contribution for distance */
                }

                /* calculate approximate rain shadow for new point */
                if (e.h <= 0.0 || !(rainfall || makeBiomes)) e.shadow = 0.0;
                else
                {
                    x1 = 0.5 * (a.x + b.x);
                    x1 = a.h * (x1 - a.x) + b.h * (x1 - b.x);
                    y1 = 0.5 * (a.y + b.y);
                    y1 = a.h * (y1 - a.y) + b.h * (y1 - b.y);
                    z1 = 0.5 * (a.z + b.z);
                    z1 = a.h * (z1 - a.z) + b.h * (z1 - b.z);
                    l1 = Math.Sqrt(x1 * x1 + y1 * y1 + z1 * z1);
                    if (l1 == 0.0) l1 = 1.0;
                    tmp = Math.Sqrt(1.0 - y * y);
                    if (tmp < 0.0001) tmp = 0.0001;
                    x2 = x * x1 + y * y1 + z * z1;
                    z2 = -z / tmp * x1 + x / tmp * z1;
                    if (lab > 0.04)
                        e.shadow = (a.shadow + b.shadow - Math.Cos(PI * shade_angle / 180.0) * z2 / l1) / 3.0;
                    else
                        e.shadow = (a.shadow + b.shadow) / 2.0;
                }

                /* find out in which new tetrahedron target point is */
                eax = a.x - e.x; eay = a.y - e.y; eaz = a.z - e.z;
                ecx = c.x - e.x; ecy = c.y - e.y; ecz = c.z - e.z;
                edx = d.x - e.x; edy = d.y - e.y; edz = d.z - e.z;
                epx = x - e.x; epy = y - e.y; epz = z - e.z;
                if ((eax * ecy * edz + eay * ecz * edx + eaz * ecx * edy
                     - eaz * ecy * edx - eay * ecx * edz - eax * ecz * edy) *
                    (epx * ecy * edz + epy * ecz * edx + epz * ecx * edy
                     - epz * ecy * edx - epy * ecx * edz - epx * ecz * edy) > 0.0)
                {
                    /* point is inside acde */
                    return (planet(c, d, a, e, x, y, z, level - 1));
                }
                else
                {
                    /* point is inside bcde */
                    return (planet(c, d, b, e, x, y, z, level - 1));
                }
            }
            else
            { /* level == 0 */
                if (doshade == 1 || doshade == 2)
                { /* bump map */
                    x1 = 0.25 * (a.x + b.x + c.x + d.x);
                    x1 = a.h * (x1 - a.x) + b.h * (x1 - b.x) + c.h * (x1 - c.x) + d.h * (x1 - d.x);
                    y1 = 0.25 * (a.y + b.y + c.y + d.y);
                    y1 = a.h * (y1 - a.y) + b.h * (y1 - b.y) + c.h * (y1 - c.y) + d.h * (y1 - d.y);
                    z1 = 0.25 * (a.z + b.z + c.z + d.z);
                    z1 = a.h * (z1 - a.z) + b.h * (z1 - b.z) + c.h * (z1 - c.z) + d.h * (z1 - d.z);
                    l1 = Math.Sqrt(x1 * x1 + y1 * y1 + z1 * z1);
                    if (l1 == 0.0) l1 = 1.0;
                    tmp = Math.Sqrt(1.0 - y * y);
                    if (tmp < 0.0001) tmp = 0.0001;
                    x2 = x * x1 + y * y1 + z * z1;
                    y2 = -x * y / tmp * x1 + tmp * y1 - z * y / tmp * z1;
                    z2 = -z / tmp * x1 + x / tmp * z1;
                    shade =
                      (int)((-Math.Sin(PI * shade_angle / 180.0) * y2 - Math.Cos(PI * shade_angle / 180.0) * z2)
                            / l1 * 48.0 + 128.0);
                    if (shade < 10) shade = 10;
                    if (shade > 255) shade = 255;
                    if (doshade == 2 && (a.h + b.h + c.h + d.h) < 0.0) shade = 150;
                }
                else if (doshade == 3)
                { /* daylight shading */
                    double hh = a.h + b.h + c.h + d.h;
                    if (hh <= 0.0)
                    { /* sea */
                        x1 = x; y1 = y; z1 = z; /* (x1,y1,z1) = normal vector */
                    }
                    else
                    { /* add bumbmap effect */
                        x1 = 0.25 * (a.x + b.x + c.x + d.x);
                        x1 = (a.h * (x1 - a.x) + b.h * (x1 - b.x) + c.h * (x1 - c.x) + d.h * (x1 - d.x));
                        y1 = 0.25 * (a.y + b.y + c.y + d.y);
                        y1 = (a.h * (y1 - a.y) + b.h * (y1 - b.y) + c.h * (y1 - c.y) + d.h * (y1 - d.y));
                        z1 = 0.25 * (a.z + b.z + c.z + d.z);
                        z1 = (a.h * (z1 - a.z) + b.h * (z1 - b.z) + c.h * (z1 - c.z) + d.h * (z1 - d.z));
                        l1 = 5.0 * Math.Sqrt(x1 * x1 + y1 * y1 + z1 * z1);
                        x1 += x * l1; y1 += y * l1; z1 += z * l1;
                    }
                    l1 = Math.Sqrt(x1 * x1 + y1 * y1 + z1 * z1);
                    if (l1 == 0.0) l1 = 1.0;
                    x2 = Math.Cos(PI * shade_angle / 180.0 - 0.5 * PI) * Math.Cos(PI * shade_angle2 / 180.0);
                    y2 = -Math.Sin(PI * shade_angle2 / 180.0);
                    z2 = -Math.Sin(PI * shade_angle / 180.0 - 0.5 * PI) * Math.Cos(PI * shade_angle2 / 180.0);
                    shade = (int)((x1 * x2 + y1 * y2 + z1 * z2) / l1 * 170.0 + 10);
                    if (shade < 10) shade = 10;
                    if (shade > 255) shade = 255;
                }
                rainShadow = 0.25 * (a.shadow + b.shadow + c.shadow + d.shadow);
                return 0.25 * (a.h + b.h + c.h + d.h);
            }
        }

        /// <summary>
        /// Cache related function at level 11
        /// </summary>
        double planet1(double x, double y, double z)
        {
            Vertex a, b, c, d;

            double abx, aby, abz, acx, acy, acz, adx, ady, adz, apx, apy, apz;
            double bax, bay, baz, bcx, bcy, bcz, bdx, bdy, bdz, bpx, bpy, bpz;

            /* check if point is inside cached tetrahedron */

            abx = ssb.x - ssa.x; aby = ssb.y - ssa.y; abz = ssb.z - ssa.z;
            acx = ssc.x - ssa.x; acy = ssc.y - ssa.y; acz = ssc.z - ssa.z;
            adx = ssd.x - ssa.x; ady = ssd.y - ssa.y; adz = ssd.z - ssa.z;
            apx = x - ssa.x; apy = y - ssa.y; apz = z - ssa.z;

            if ((adx * aby * acz + ady * abz * acx + adz * abx * acy
                 - adz * aby * acx - ady * abx * acz - adx * abz * acy) *
                (apx * aby * acz + apy * abz * acx + apz * abx * acy
                 - apz * aby * acx - apy * abx * acz - apx * abz * acy) > 0.0)
            {
                /* p is on same side of abc as d */
                if ((acx * aby * adz + acy * abz * adx + acz * abx * ady
                     - acz * aby * adx - acy * abx * adz - acx * abz * ady) *
                    (apx * aby * adz + apy * abz * adx + apz * abx * ady
                     - apz * aby * adx - apy * abx * adz - apx * abz * ady) > 0.0)
                {
                    /* p is on same side of abd as c */
                    if ((abx * ady * acz + aby * adz * acx + abz * adx * acy
                         - abz * ady * acx - aby * adx * acz - abx * adz * acy) *
                        (apx * ady * acz + apy * adz * acx + apz * adx * acy
                         - apz * ady * acx - apy * adx * acz - apx * adz * acy) > 0.0)
                    {
                        /* p is on same side of acd as b */
                        bax = -abx; bay = -aby; baz = -abz;
                        bcx = ssc.x - ssb.x; bcy = ssc.y - ssb.y; bcz = ssc.z - ssb.z;
                        bdx = ssd.x - ssb.x; bdy = ssd.y - ssb.y; bdz = ssd.z - ssb.z;
                        bpx = x - ssb.x; bpy = y - ssb.y; bpz = z - ssb.z;
                        if ((bax * bcy * bdz + bay * bcz * bdx + baz * bcx * bdy
                             - baz * bcy * bdx - bay * bcx * bdz - bax * bcz * bdy) *
                            (bpx * bcy * bdz + bpy * bcz * bdx + bpz * bcx * bdy
                             - bpz * bcy * bdx - bpy * bcx * bdz - bpx * bcz * bdy) > 0.0)
                        {
                            /* p is on same side of bcd as a */
                            /* Hence, p is inside cached tetrahedron */
                            /* so we start from there */
                            return (planet(ssa, ssb, ssc, ssd, x, y, z, 11));
                        }
                    }
                }
            }
            /* otherwise, we start from scratch */

            return (planet(tetra[0], tetra[1], tetra[2], tetra[3],
                          /* vertices of tetrahedron */
                          x, y, z,   /* coordinates of point we want colour of */
                          Depth)); /* subdivision depth */

        }
        #endregion

        #region Routines
        void makeoutline(bool do_bw)
        {
            int i, j, k, t;

            outx = new int[Width * Height];
            outy = new int[Width * Height];
            k = 0;
            for (i = 1; i < Width - 1; i++)
                for (j = 1; j < Height - 1; j++)
                    if ((col[i][j] >= LOWEST && col[i][j] <= SEA) &&
                        (col[i - 1][j] >= LAND || col[i + 1][j] >= LAND ||
                         col[i][j - 1] >= LAND || col[i][j + 1] >= LAND ||
                         col[i - 1][j - 1] >= LAND || col[i - 1][j + 1] >= LAND ||
                         col[i + 1][j - 1] >= LAND || col[i + 1][j + 1] >= LAND))
                    {
                        /* if point is sea and any neighbour is not, add to outline */
                        outx[k] = i; outy[k++] = j;
                    }

            int contourstep = 0;

            if (contourLines > 0)
            {
                contourstep = (HIGHEST - LAND) / (contourLines + 1);
                for (i = 1; i < Width - 1; i++)
                    for (j = 1; j < Height - 1; j++)
                    {
                        t = (col[i][j] - LAND) / contourstep;
                        if (col[i][j] >= LAND &&
                            ((col[i - 1][j] - LAND) / contourstep > t ||
                             (col[i + 1][j] - LAND) / contourstep > t ||
                             (col[i][j - 1] - LAND) / contourstep > t ||
                             (col[i][j + 1] - LAND) / contourstep > t))
                        {
                            /* if point is at contour line and any neighbour is higher */
                            outx[k] = i; outy[k++] = j;
                        }
                    }
            }
            if (coastContourLines > 0)
            {
                contourstep = (LAND - LOWEST) / 20;
                for (i = 1; i < Width - 1; i++)
                    for (j = 1; j < Height - 1; j++)
                    {
                        t = (col[i][j] - LAND) / contourstep;
                        if (col[i][j] <= SEA && t >= -coastContourLines &&
                            ((col[i - 1][j] - LAND) / contourstep > t ||
                             (col[i + 1][j] - LAND) / contourstep > t ||
                             (col[i][j - 1] - LAND) / contourstep > t ||
                             (col[i][j + 1] - LAND) / contourstep > t))
                        {
                            /* if point is at contour line and any neighbour is higher */
                            outx[k] = i; outy[k++] = j;
                        }
                    }
            }
            if (do_bw) /* if outline only, clear colours */
                for (i = 0; i < Width; i++)
                    for (j = 0; j < Height; j++)
                    {
                        if (col[i][j] >= LOWEST)
                            col[i][j] = (ushort)WHITE;
                        else col[i][j] = (ushort)BLACK;
                    }
            /* draw outline (in black if outline only) */
            contourstep = (HIGHEST - LAND) / (contourLines + 1);
            for (j = 0; j < k; j++)
            {
                if (do_bw) t = BLACK;
                else
                {
                    t = col[outx[j]][outy[j]];
                    if (t != OUTLINE1 && t != OUTLINE2)
                    {
                        if (contourLines > 0 && t >= LAND)
                            if (((t - LAND) / contourstep) % 2 == 1)
                                t = OUTLINE1;
                            else t = OUTLINE2;
                        else if (t <= SEA)
                            t = OUTLINE1;
                    }
                }
                col[outx[j]][outy[j]] = (ushort)t;
            }
        }
        #endregion

        #region Math
        int min(int x, int y)
        { return (x < y ? x : y); }

        int max(int x, int y)
        { return (x < y ? y : x); }

        double fmin(double x, double y)
        { return (x < y ? x : y); }

        double fmax(double x, double y)
        { return (x < y ? y : x); }
        double log_2(double x)
        { 
            return Math.Log(x) / Math.Log(2.0); 
        }
        /// <summary>
        /// random number generator taking two seeds; rand2(p,q) = rand2(q,p) is important
        /// </summary>
        double rand2(double p, double q)
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
        /// <summary>
        /// reads in a map for matching
        /// </summary>
        void readmap()
        {
            int i, j;
            double y;
            char c;
            int Width, Height;

            Width = 48; Height = 24;
            for (j = 0; j < Height; j += 2)
            {
                for (i = 0; i < Width; i += 2)
                {
                    c = Console.ReadKey().KeyChar;
                    switch (c)
                    {
                        case '.':
                            cl0[i][j] = -8;
                            break;
                        case ',':
                            cl0[i][j] = -6;
                            break;
                        case ':':
                            cl0[i][j] = -4;
                            break;
                        case ';':
                            cl0[i][j] = -2;
                            break;
                        case '-':
                            cl0[i][j] = 0;
                            break;
                        case '*':
                            cl0[i][j] = 2;
                            break;
                        case 'o':
                            cl0[i][j] = 4;
                            break;
                        case 'O':
                            cl0[i][j] = 6;
                            break;
                        case '@':
                            cl0[i][j] = 8;
                            break;
                        default: 
                            Console.WriteLine($"Wrong map symbol: {c}");
                            break;
                    }
                }
                c = Console.ReadKey().KeyChar; 
                if (c != '\n') 
                    Console.WriteLine($"Wrong map format: {c}");
            }
            /* interpolate */
            for (j = 1; j < Height; j += 2)
                for (i = 0; i < Width; i += 2)
                    cl0[i][j] = (cl0[i][j - 1] + cl0[i][(j + 1)]) / 2;
            for (j = 0; j < Height; j++)
                for (i = 1; i < Width; i += 2)
                    cl0[i][j] = (cl0[i - 1][j] + cl0[(i + 1) % Width][j]) / 2;
        }
        void readcolors(string colorsname)
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
            foreach (var line in ReadResourceLines(colorsname))
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
                    rtable[cNum] = rValue;
                    gtable[cNum] = gValue;
                    btable[cNum] = bValue;
                    /* interpolate colours between oldcNum and cNum */
                    for (i = oldcNum + 1; i < cNum; i++)
                    {
                        rtable[i] = (rtable[oldcNum] * (cNum - i) + rtable[cNum] * (i - oldcNum))
                                    / (cNum - oldcNum + 1);
                        gtable[i] = (gtable[oldcNum] * (cNum - i) + gtable[cNum] * (i - oldcNum))
                                    / (cNum - oldcNum + 1);
                        btable[i] = (btable[oldcNum] * (cNum - i) + btable[cNum] * (i - oldcNum))
                                    / (cNum - oldcNum + 1);
                    }
                }
            }

            nocols = cNum + 1;
            if (nocols < 10) nocols = 10;

            HIGHEST = nocols - 1;
            SEA = (HIGHEST + LOWEST) / 2;
            LAND = SEA + 1;

            for (i = cNum + 1; i < nocols; i++)
            {
                /* fill up rest of colour table with last read colour */
                rtable[i] = rtable[cNum];
                gtable[i] = gtable[cNum];
                btable[i] = btable[cNum];
            }

            if (makeBiomes)
            {
                /* make biome colours */
                rtable['T' - 64 + LAND] = 210;
                gtable['T' - 64 + LAND] = 210;
                btable['T' - 64 + LAND] = 210;
                rtable['G' - 64 + LAND] = 250;
                gtable['G' - 64 + LAND] = 215;
                btable['G' - 64 + LAND] = 165;
                rtable['B' - 64 + LAND] = 105;
                gtable['B' - 64 + LAND] = 155;
                btable['B' - 64 + LAND] = 120;
                rtable['D' - 64 + LAND] = 220;
                gtable['D' - 64 + LAND] = 195;
                btable['D' - 64 + LAND] = 175;
                rtable['S' - 64 + LAND] = 225;
                gtable['S' - 64 + LAND] = 155;
                btable['S' - 64 + LAND] = 100;
                rtable['F' - 64 + LAND] = 155;
                gtable['F' - 64 + LAND] = 215;
                btable['F' - 64 + LAND] = 170;
                rtable['R' - 64 + LAND] = 170;
                gtable['R' - 64 + LAND] = 195;
                btable['R' - 64 + LAND] = 200;
                rtable['W' - 64 + LAND] = 185;
                gtable['W' - 64 + LAND] = 150;
                btable['W' - 64 + LAND] = 160;
                rtable['E' - 64 + LAND] = 130;
                gtable['E' - 64 + LAND] = 190;
                btable['E' - 64 + LAND] = 25;
                rtable['O' - 64 + LAND] = 110;
                gtable['O' - 64 + LAND] = 160;
                btable['O' - 64 + LAND] = 170;
                rtable['I' - 64 + LAND] = 255;
                gtable['I' - 64 + LAND] = 255;
                btable['I' - 64 + LAND] = 255;
            }
        }
        /// <summary>
        /// Prints picture in PPM (portable pixel map) format
        /// </summary>
        void printppm(string outfile)
        {
            int i, j, c, s;

            using FileStream stream = File.Open(outfile, FileMode.Create);
            using BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8, false);
            writer.Write("P6\n");
            writer.Write("#fractal planet image\n");
            writer.Write($"# Command line:\n# {cmdLine}\n");
            writer.Write($"{Width} {Height} 255\n");

            if (doshade != 0)
            {
                for (j = 0; j < Height; j++)
                {
                    for (i = 0; i < Width; i++)
                    {
                        s = shades[i][j];
                        c = s * rtable[col[i][j]] / 150;
                        if (c > 255) c = 255;
                        writer.Write((char)c);
                        c = s * gtable[col[i][j]] / 150;
                        if (c > 255) c = 255;
                        writer.Write((char)c);
                        c = s * btable[col[i][j]] / 150;
                        if (c > 255) c = 255;
                        writer.Write((char)c);
                    }
                }
            }
            else
            {
                for (j = 0; j < Height; j++)
                    for (i = 0; i < Width; i++)
                    {
                        writer.Write((char)rtable[col[i][j]]);
                        writer.Write((char)gtable[col[i][j]]);
                        writer.Write((char)btable[col[i][j]]);
                    }
            }
        }
        /// <summary>
        /// prints picture in b/w PPM format
        /// </summary>
        void printppmBW(string filename)
        {
            int i, j, c;

            using FileStream file = File.Open(filename, FileMode.Create);
            using BinaryWriter writer = new BinaryWriter(file);
            writer.Write("P6\n");
            writer.Write("#fractal planet image\n");
            writer.Write($"# Command line:\n# {cmdLine}\n");
            writer.Write($"{Width} {Height} 1\n");

            for (j = 0; j < Height; j++)
                for (i = 0; i < Width; i++)
                {
                    if (col[i][j] < WHITE)
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
        void printbmp(string filename)
        {
            int i, j, c, s0, s, W1;

            using FileStream file = File.Open(filename, FileMode.Create);
            using BinaryWriter writer = new BinaryWriter(file, Encoding.ASCII, false);

            writer.Write("BM".ToCharArray());

            W1 = (3 * Width + 3);
            W1 -= W1 % 4;
            s0 = (cmdLine.Length + "Command line:\n\n".Length + 3) & 0xffc;
            s = s0 + 54 + W1 * Height; /* file size */
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

            writer.Write((byte)(Width & 255));
            writer.Write((byte)((Width >> 8) & 255));
            writer.Write((byte)((Width >> 16) & 255));
            writer.Write((byte)(Width >> 24));

            writer.Write((byte)(Height & 255));
            writer.Write((byte)((Height >> 8) & 255));
            writer.Write((byte)((Height >> 16) & 255));
            writer.Write((byte)(Height >> 24));

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

            if (doshade != 0)
            {
                for (j = Height - 1; j >= 0; j--)
                {
                    for (i = 0; i < Width; i++)
                    {
                        s = shades[i][j];
                        c = s * btable[col[i][j]] / 150;
                        if (c > 255) c = 255;
                        writer.Write((byte)c);
                        c = s * gtable[col[i][j]] / 150;
                        if (c > 255) c = 255;
                        writer.Write((byte)c);
                        c = s * rtable[col[i][j]] / 150;
                        if (c > 255) c = 255;
                        writer.Write((byte)c);
                    }
                    for (i = 3 * Width; i < W1; i++) writer.Write((byte)0);
                }
            }
            else
            {
                for (j = Height - 1; j >= 0; j--)
                {
                    for (i = 0; i < Width; i++)
                    {
                        writer.Write((byte)btable[col[i][j]]);
                        writer.Write((byte)gtable[col[i][j]]);
                        writer.Write((byte)rtable[col[i][j]]);
                    }
                    for (i = 3 * Width; i < W1; i++) writer.Write((byte)0);
                }
            }

            writer.Write($"Command line:\n{cmdLine}\n".ToCharArray());
        }

        /// <summary>
        /// Prints picture in b/w BMP format
        /// </summary>
        void printbmpBW(string filename)
        {
            int i, j, c, s, s0, W1;

            using FileStream file = File.Open(filename, FileMode.Create);
            using BinaryWriter writer = new BinaryWriter(file);

            writer.Write("BM");

            W1 = (Width + 31);
            W1 -= W1 % 32;
            s0 = (cmdLine.Length + "Command line:\n\n".Length + 3) & 0xffc;
            s = s0 + 62 + (W1 * Height) / 8; /* file size */
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

            writer.Write(Width & 255);
            writer.Write((Width >> 8) & 255);
            writer.Write((Width >> 16) & 255);
            writer.Write(Width >> 24);

            writer.Write(Height & 255);
            writer.Write((Height >> 8) & 255);
            writer.Write((Height >> 16) & 255);
            writer.Write(Height >> 24);

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

            for (j = Height - 1; j >= 0; j--)
                for (i = 0; i < W1; i += 8)
                {
                    if (i < Width && col[i][j] >= WHITE)
                        c = 128;
                    else c = 0;
                    if (i + 1 < Width && col[i + 1][j] >= WHITE)
                        c += 64;
                    if (i + 2 < Width && col[i + 2][j] >= WHITE)
                        c += 32;
                    if (i + 3 < Width && col[i + 3][j] >= WHITE)
                        c += 16;
                    if (i + 4 < Width && col[i + 4][j] >= WHITE)
                        c += 8;
                    if (i + 5 < Width && col[i + 5][j] >= WHITE)
                        c += 4;
                    if (i + 6 < Width && col[i + 6][j] >= WHITE)
                        c += 2;
                    if (i + 7 < Width && col[i + 7][j] >= WHITE)
                        c += 1;
                    writer.Write(c);
                }

            writer.Write("Command line:\n{cmdLine}\n");
        }

        string nletters(int n, int c)
        {
            int i;
            char[] buffer = new char[8];

            buffer[n] = '\0';

            for (i = n - 1; i >= 0; i--)
            {
                buffer[i] = letters[c & 0x001F];
                c >>= 5;
            }

            return new string(buffer);
        }

        /// <summary>
        /// prints picture in XPM (X-windows pixel map) format
        /// </summary>
        void printxpm(string filename)
        {
            int x, y, i, nbytes;

            x = nocols - 1;
            for (nbytes = 0; x != 0; nbytes++)
                x >>= 5;

            using FileStream stream = File.Open(filename, FileMode.Create);
            using BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8, false);
            writer.Write("/* XPM */\n");
            writer.Write($"/* Command line: */\n/* {cmdLine}*/\n");
            writer.Write("static char *xpmdata[] = {\n");
            writer.Write("/* width height ncolors chars_per_pixel */\n");
            writer.Write($"\"{Width} {Height} {nocols} {nbytes}\",\n");
            writer.Write("/* colors */\n");
            for (i = 0; i < nocols; i++)
                //writer.Write($"\"{nletters(nbytes, i)} c #%2.2X%2.2X%2.2X\",\n",
                //        , rtable[i], gtable[i], btable[i]);

            writer.Write("/* pixels */\n");
            for (y = 0; y < Height; y++)
            {
                writer.Write("\"");
                for (x = 0; x < Width; x++)
                    writer.Write(nletters(nbytes, col[x][y]));
                writer.Write("\",\n");
            }
            writer.Write("};\n");
        }

        /// <summary>
        /// prints picture in XPM (X-windows pixel map) format
        /// </summary>
        void printxpmBW(string filename)
        {
            int x, y, nbytes;

            x = nocols - 1;
            nbytes = 1;

            using FileStream stream = File.Open(filename, FileMode.Create);
            using BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8, false);
            writer.Write("/* XPM */\n");
            writer.Write($"/* Command line: */\n/* {cmdLine}*/\n");
            writer.Write("static char *xpmdata[] = {\n");
            writer.Write("/* width height ncolors chars_per_pixel */\n");
            writer.Write($"\"{Width} {Height} {2} {nbytes}\",\n");
            writer.Write("/* colors */\n");

            writer.Write("\". c #FFFFFF\",\n");
            writer.Write("\"X c #000000\",\n");

            writer.Write("/* pixels */\n");
            for (y = 0; y < Height; y++)
            {
                writer.Write("\"");
                for (x = 0; x < Width; x++)
                    writer.Write((col[x][y] < WHITE) ? "X" : ".");
                writer.Write("\",\n");
            }
            writer.Write("};\n");
        }
        /// <summary>
        /// prints heightfield
        /// </summary>
        void printheights(string filename)
        {
            int i, j;

            using StreamWriter writer = new StreamWriter(filename);
            for (j = 0; j < Height; j++)
            {
                for (i = 0; i < Width; i++)
                    writer.Write($"{heights[i][j]} ");
                writer.Write('\n');
            }
        }
        public void Run(string v)
        {
            Run(v.Split(" "));
        }
        #endregion
    }

    internal static class MathHelper
    {
        /// <summary>
        /// Distance squared between vertices
        /// </summary>
        public static double dist2(this OldPlanetC.Vertex a, OldPlanetC.Vertex b)
        {
            double abx, aby, abz;
            abx = a.x - b.x; aby = a.y - b.y; abz = a.z - b.z;
            return abx * abx + aby * aby + abz * abz;
        }
    }
}
