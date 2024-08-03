namespace HDPlanet
{
    internal abstract class Projector
    {
        #region Properties
        public char View { get; internal set; }
        public int Width { get; internal set; }
        public int Height { get; internal set; }
        public double LongiDegrees { get; internal set; }
        public double LatDegrees { get; internal set; }
        public double Rotate1Degrees { get; internal set; }
        public double Rotate2Degrees { get; internal set; }
        public double Scale { get; internal set; }
        public double VGrid { get; internal set; }
        public double HGrid { get; internal set; }
        public int[][] BaseTerrain { get; internal set; }
        public double RSeed { get; internal set; }
        #endregion

        #region Context-Sensitive Configurations
        public double M { get; internal set; }
        public double POW { get; internal set; }
        public double DD1 { get; internal set; }
        public double POWA { get; internal set; }
        public double DD2 { get; internal set; }

        public int DoShade { get; internal set; }
        public bool NonLinear { get; internal set; }
        public FileType OutputFileType { get; internal set; }
        public double ShadeAngle { get; internal set; }
        public double ShadeAngle2 { get; internal set; }
        public bool MatchMap { get; internal set; }
        public double MatchSize { get; internal set; }
        public int LatiColor { get; internal set; }

        public bool Temperature { get; internal set; }
        public bool Rainfall { get; internal set; }
        public bool MakeBiomes { get; internal set; }

        public int BLACK { get; internal set; }
        public int WHITE { get; internal set; }
        public int BACK { get; internal set; }
        public int GRID { get; internal set; }
        public int OUTLINE1 { get; internal set; }
        public int OUTLINE2 { get; internal set; }
        public int LOWEST { get; internal set; }  
        public int SEA { get; internal set; }
        public int LAND { get; internal set; }
        public int HIGHEST { get; internal set; }
        public double PI { get; internal set; }
        public double DEG2RAD { get; internal set; }
        #endregion

        #region Additional Settings
        public bool Debug { get; internal set; }
        #endregion

        #region Input/Output Data
        public int[] RTable { get; internal set; }
        public int[] GTable { get; internal set; }
        public int[] BTtable { get; internal set; }

        public ushort[][] Col { get; internal set; }
        public int[][] HeightField { get; internal set; }
        public double[][] XXX { get; internal set; }
        public double[][] YYY { get; internal set; }
        public double[][] ZZZ { get; internal set; }
        public int[] OutX { get; internal set; }
        public int[] OutY { get; internal set; }
        public ushort[][] Shades { get; internal set; }
        #endregion

        #region State Properties
        protected double LongiRadians;
        protected double LatRadians;
        /// <summary>
        /// For the vertices of the tetrahedron
        /// </summary>
        protected Vertex[] Tetra;
        protected int Shade;
        protected int Depth; // Depth of subdivisions
        protected Vertex SSA, SSB, SSC, SSD;
        protected double Cla, Sla, Clo, Slo;

        protected double TempMin = 1000.0;
        protected double TempMax = -1000.0;
        protected double RainMin = 1000.0;
        protected double RainMax = -1000.0;
        protected double RainShadow = 0.0;
        #endregion

        #region Interface
        public void Initialize()
        {
            InitializeParameters();
            InitializeSeeds();
            InitializeTetrahedron();
        }

        public abstract void Map();
        #endregion

        #region Math
        static int Min(int x, int y)
            => (x < y ? x : y);
        static int Max(int x, int y)
            => (x < y ? y : x);
        static double FMin(double x, double y)
            => (x < y ? x : y);
        static double FMax(double x, double y)
            => (x < y ? y : x);
        #endregion

        #region Base Routines
        private void InitializeParameters()
        {
            LongiRadians = LongiDegrees * DEG2RAD;
            LatRadians = LatDegrees * DEG2RAD;

            Depth = 3 * ((int)(PlanetGenerator.Log2(Scale * Height))) + 6;
            Sla = Math.Sin(LatRadians);
            Cla = Math.Cos(LatRadians);
            Slo = Math.Sin(LongiRadians);
            Clo = Math.Cos(LongiRadians);
        }
        private void InitializeSeeds()
        {
            double R1 = PlanetGenerator.Rand2(RSeed, RSeed);
            double R2 = PlanetGenerator.Rand2(R1, R1);
            double R3 = PlanetGenerator.Rand2(R1, R2);
            double R4 = PlanetGenerator.Rand2(R2, R3);

            Tetra = new Vertex[4];

            Tetra[0].Seed = R1;
            Tetra[1].Seed = R2;
            Tetra[2].Seed = R3;
            Tetra[3].Seed = R4;

            Tetra[0].Height = M;
            Tetra[1].Height = M;
            Tetra[2].Height = M;
            Tetra[3].Height = M;

            Tetra[0].Shadow = 0.0;
            Tetra[1].Shadow = 0.0;
            Tetra[2].Shadow = 0.0;
            Tetra[3].Shadow = 0.0;
        }
        private void InitializeTetrahedron()
        {
            double rotate1Radians = -Rotate1Degrees * DEG2RAD;
            double rotate2Radians = -Rotate2Degrees * DEG2RAD;

            double sR1 = Math.Sin(rotate1Radians);
            double cR1 = Math.Cos(rotate1Radians);
            double sR2 = Math.Sin(rotate2Radians);
            double cR2 = Math.Cos(rotate2Radians);

            /* Initialize vertices to slightly irregular tetrahedron */
            Tetra[0].X = -Math.Sqrt(3.0) - 0.20;
            Tetra[0].Y = -Math.Sqrt(3.0) - 0.22;
            Tetra[0].Z = -Math.Sqrt(3.0) - 0.23;

            Tetra[1].X = -Math.Sqrt(3.0) - 0.19;
            Tetra[1].Y = Math.Sqrt(3.0) + 0.18;
            Tetra[1].Z = Math.Sqrt(3.0) + 0.17;

            Tetra[2].X = Math.Sqrt(3.0) + 0.21;
            Tetra[2].Y = -Math.Sqrt(3.0) - 0.24;
            Tetra[2].Z = Math.Sqrt(3.0) + 0.15;

            Tetra[3].X = Math.Sqrt(3.0) + 0.24;
            Tetra[3].Y = Math.Sqrt(3.0) + 0.22;
            Tetra[3].Z = -Math.Sqrt(3.0) - 0.25;

            double tx, ty, tz;
            for (int i = 0; i < 4; i++)
            {
                /* rotate around y axis */
                tx = Tetra[i].X;
                ty = Tetra[i].Y;
                tz = Tetra[i].Z;
                Tetra[i].X = cR2 * tx + sR2 * tz;
                Tetra[i].Y = ty;
                Tetra[i].Z = -sR2 * tx + cR2 * tz;
            }

            for (int i = 0; i < 4; i++)
            {
                /* rotate around x axis */
                tx = Tetra[i].X;
                ty = Tetra[i].Y;
                tz = Tetra[i].Z;
                Tetra[i].X = tx;
                Tetra[i].Y = cR1 * ty - sR1 * tz;
                Tetra[i].Z = sR1 * ty + cR1 * tz;
            }
        }
        /// <summary>
        /// High level function that gets (average) altitude closest to target point and performs colording using procedural rules
        /// </summary>
        protected void Planet0(double x, double y, double z, int i, int j)
        {
            double alt, y2, sun, temp, rain;
            int colour;

            alt = Planet1(x, y, z);

            /* calculate temperature based on altitude and latitude */
            /* scale: -0.1 to 0.1 corresponds to -30 to +30 degrees Celsius */
            sun = Math.Sqrt(1.0 - y * y); /* approximate amount of sunlight at
			     latitude ranged from 0.1 to 1.1 */
            if (alt < 0) temp = sun / 8.0 + alt * 0.3; /* deep water colder */
            else temp = sun / 8.0 - alt * 1.2; /* high altitudes colder */

            if (temp < TempMin && alt > 0) TempMin = temp;
            if (temp > TempMax && alt > 0) TempMax = temp;
            if (Temperature) alt = temp - 0.05;

            /* calculate rainfall based on temperature and latitude */
            /* rainfall approximately proportional to temperature but reduced
               near horse latitudes (+/- 30 degrees, y=0.5) and reduced for
               rain shadow */
            y2 = Math.Abs(y) - 0.5;
            rain = temp * 0.65 + 0.1 - 0.011 / (y2 * y2 + 0.1);
            rain += 0.03 * RainShadow;
            if (rain < 0.0) rain = 0.0;

            if (rain < RainMin && alt > 0) RainMin = rain;
            if (rain > RainMax && alt > 0) RainMax = rain;

            if (Rainfall) alt = rain - 0.02;

            if (NonLinear)
            {
                /* non-linear scaling to make flatter near sea level */
                alt = alt * alt * alt * 300;
            }
            /* store height for heightfield */
            if (OutputFileType == FileType.HeightField) HeightField[i][j] = (int)(10000000 * alt);

            y2 = y * y; y2 = y2 * y2; y2 = y2 * y2;

            /* calculate colour */

            if (MakeBiomes)
            { /* make biome colours */
                int tt = Min(44, Max(0, (int)(rain * 300.0 - 9)));
                int rr = Min(44, Max(0, (int)(temp * 300.0 + 10)));
                char bio = PlanetGenerator.Biomes[tt][rr];
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
                if (LatiColor > 0 && y2 + alt >= 1.0 - 0.02 * LatiColor * LatiColor)
                    colour = HIGHEST;  /* icecap if close to poles */
                else
                {
                    colour = SEA + (int)((SEA - LOWEST + 1) * (10 * alt));
                    if (colour < LOWEST) colour = LOWEST;
                }
            }
            else
            {
                if (LatiColor != 0) alt += 0.1 * LatiColor * y2;  /* altitude adjusted with latitude */
                if (alt >= 0.1) /* if high then */
                    colour = HIGHEST;
                else
                {
                    colour = LAND + (int)((HIGHEST - LAND + 1) * (10 * alt));
                    if (colour > HIGHEST) colour = HIGHEST;
                }
            }

            /* store colour */
            Col[i][j] = (ushort)colour;

            /* store (x,y,z) coordinates for grid drawing */
            if (VGrid != 0.0)
            {
                XXX[i][j] = x;
                ZZZ[i][j] = z;
            }
            if (HGrid != 0.0 || VGrid != 0.0) YYY[i][j] = y;

            /* store shading info */
            if (DoShade > 0) Shades[i][j] = (ushort)Shade;

            return;
        }
        /// <summary>
        /// Performs recursive subdivision and boundary check function; This is the key function as described in the algorithm
        /// </summary>
        protected double Planet(/* tetrahedron vertices */Vertex a, Vertex b, Vertex c, Vertex d, /* goal point */ double x, double y, double z, /* levels to go */ int level)
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
                lab = MathHelper.Dist2(a, b);
                lac = MathHelper.Dist2(a, c);
                lad = MathHelper.Dist2(a, d);
                lbc = MathHelper.Dist2(b, c);
                lbd = MathHelper.Dist2(b, d);
                lcd = MathHelper.Dist2(c, d);

                maxlength = lab;
                if (lac > maxlength) maxlength = lac;
                if (lad > maxlength) maxlength = lad;
                if (lbc > maxlength) maxlength = lbc;
                if (lbd > maxlength) maxlength = lbd;
                if (lcd > maxlength) maxlength = lcd;

                // This code is doing sorting
                if (lac == maxlength) return (Planet(a, c, b, d, x, y, z, level));
                if (lad == maxlength) return (Planet(a, d, b, c, x, y, z, level));
                if (lbc == maxlength) return (Planet(b, c, a, d, x, y, z, level));
                if (lbd == maxlength) return (Planet(b, d, a, c, x, y, z, level));
                if (lcd == maxlength) return (Planet(c, d, a, b, x, y, z, level));

                if (level == 11)
                { /* save tetrahedron for caching */
                    SSA = a; SSB = b; SSC = c; SSD = d;
                }

                /* ab is longest, so cut ab */
                e.Seed = PlanetGenerator.Rand2(a.Seed, b.Seed);
                es1 = PlanetGenerator.Rand2(e.Seed, e.Seed);
                es2 = 0.5 + 0.1 * PlanetGenerator.Rand2(es1, es1);  /* find cut point */
                es3 = 1.0 - es2;

                if (a.Seed < b.Seed)
                {
                    e.X = es2 * a.X + es3 * b.X; e.Y = es2 * a.Y + es3 * b.Y; e.Z = es2 * a.Z + es3 * b.Z;
                }
                else if (a.Seed > b.Seed)
                {
                    e.X = es3 * a.X + es2 * b.X; e.Y = es3 * a.Y + es2 * b.Y; e.Z = es3 * a.Z + es2 * b.Z;
                }
                else
                { /* as==bs, very unlikely to ever happen */
                    e.X = 0.5 * a.X + 0.5 * b.X; e.Y = 0.5 * a.Y + 0.5 * b.Y; e.Z = 0.5 * a.Z + 0.5 * b.Z;
                }

                /* new altitude is: */
                if (MatchMap && lab > MatchSize)
                { 
                    /* Use reference terrain map height */
                    double l, xx, yy;
                    l = Math.Sqrt(e.X * e.X + e.Y * e.Y + e.Z * e.Z);
                    yy = Math.Asin(e.Y / l) * 23 / PI + 11.5;   // Remark-cz: What are the meaning of those hard-coded values?
                    xx = Math.Atan2(e.X, e.Z) * 23.5 / PI + 23.5;   // Remark-cz: What are the meaning of those hard-coded values?
                    e.Height = BaseTerrain[(int)(xx + 0.5)][(int)(yy + 0.5)] * 0.1 / 8.0;   // Remark-cz: What are the meaning of those hard-coded values?
                }
                else
                {
                    if (lab > 1.0) lab = Math.Pow(lab, 0.5);
                    /* decrease contribution for very long distances */
                    e.Height = 0.5 * (a.Height + b.Height) /* average of end points */
                      + e.Seed * DD1 * Math.Pow(Math.Abs(a.Height - b.Height), POWA)
                      /* plus contribution for altitude diff */
                      + es1 * DD2 * Math.Pow(lab, POW); /* plus contribution for distance */
                }

                /* calculate approximate rain shadow for new point */
                if (e.Height <= 0.0 || !(Rainfall || MakeBiomes)) e.Shadow = 0.0;
                else
                {
                    x1 = 0.5 * (a.X + b.X);
                    x1 = a.Height * (x1 - a.X) + b.Height * (x1 - b.X);
                    y1 = 0.5 * (a.Y + b.Y);
                    y1 = a.Height * (y1 - a.Y) + b.Height * (y1 - b.Y);
                    z1 = 0.5 * (a.Z + b.Z);
                    z1 = a.Height * (z1 - a.Z) + b.Height * (z1 - b.Z);
                    l1 = Math.Sqrt(x1 * x1 + y1 * y1 + z1 * z1);
                    if (l1 == 0.0) l1 = 1.0;
                    tmp = Math.Sqrt(1.0 - y * y);
                    if (tmp < 0.0001) tmp = 0.0001;
                    x2 = x * x1 + y * y1 + z * z1;
                    z2 = -z / tmp * x1 + x / tmp * z1;
                    if (lab > 0.04)
                        e.Shadow = (a.Shadow + b.Shadow - Math.Cos(PI * ShadeAngle / 180.0) * z2 / l1) / 3.0;
                    else
                        e.Shadow = (a.Shadow + b.Shadow) / 2.0;
                }

                /* find out in which new tetrahedron target point is */
                eax = a.X - e.X; eay = a.Y - e.Y; eaz = a.Z - e.Z;
                ecx = c.X - e.X; ecy = c.Y - e.Y; ecz = c.Z - e.Z;
                edx = d.X - e.X; edy = d.Y - e.Y; edz = d.Z - e.Z;
                epx = x - e.X; epy = y - e.Y; epz = z - e.Z;
                if ((eax * ecy * edz + eay * ecz * edx + eaz * ecx * edy
                     - eaz * ecy * edx - eay * ecx * edz - eax * ecz * edy) *
                    (epx * ecy * edz + epy * ecz * edx + epz * ecx * edy
                     - epz * ecy * edx - epy * ecx * edz - epx * ecz * edy) > 0.0)
                {
                    /* point is inside acde */
                    return (Planet(c, d, a, e, x, y, z, level - 1));
                }
                else
                {
                    /* point is inside bcde */
                    return (Planet(c, d, b, e, x, y, z, level - 1));
                }
            }
            else
            { /* level == 0 */
                if (DoShade == 1 || DoShade == 2)
                { /* bump map */
                    x1 = 0.25 * (a.X + b.X + c.X + d.X);
                    x1 = a.Height * (x1 - a.X) + b.Height * (x1 - b.X) + c.Height * (x1 - c.X) + d.Height * (x1 - d.X);
                    y1 = 0.25 * (a.Y + b.Y + c.Y + d.Y);
                    y1 = a.Height * (y1 - a.Y) + b.Height * (y1 - b.Y) + c.Height * (y1 - c.Y) + d.Height * (y1 - d.Y);
                    z1 = 0.25 * (a.Z + b.Z + c.Z + d.Z);
                    z1 = a.Height * (z1 - a.Z) + b.Height * (z1 - b.Z) + c.Height * (z1 - c.Z) + d.Height * (z1 - d.Z);
                    l1 = Math.Sqrt(x1 * x1 + y1 * y1 + z1 * z1);
                    if (l1 == 0.0) l1 = 1.0;
                    tmp = Math.Sqrt(1.0 - y * y);
                    if (tmp < 0.0001) tmp = 0.0001;
                    x2 = x * x1 + y * y1 + z * z1;
                    y2 = -x * y / tmp * x1 + tmp * y1 - z * y / tmp * z1;
                    z2 = -z / tmp * x1 + x / tmp * z1;
                    Shade =
                      (int)((-Math.Sin(PI * ShadeAngle / 180.0) * y2 - Math.Cos(PI * ShadeAngle / 180.0) * z2)
                            / l1 * 48.0 + 128.0);
                    if (Shade < 10) Shade = 10;
                    if (Shade > 255) Shade = 255;
                    if (DoShade == 2 && (a.Height + b.Height + c.Height + d.Height) < 0.0) Shade = 150;
                }
                else if (DoShade == 3)
                { /* daylight shading */
                    double hh = a.Height + b.Height + c.Height + d.Height;
                    if (hh <= 0.0)
                    { /* sea */
                        x1 = x; y1 = y; z1 = z; /* (x1,y1,z1) = normal vector */
                    }
                    else
                    { /* add bumbmap effect */
                        x1 = 0.25 * (a.X + b.X + c.X + d.X);
                        x1 = (a.Height * (x1 - a.X) + b.Height * (x1 - b.X) + c.Height * (x1 - c.X) + d.Height * (x1 - d.X));
                        y1 = 0.25 * (a.Y + b.Y + c.Y + d.Y);
                        y1 = (a.Height * (y1 - a.Y) + b.Height * (y1 - b.Y) + c.Height * (y1 - c.Y) + d.Height * (y1 - d.Y));
                        z1 = 0.25 * (a.Z + b.Z + c.Z + d.Z);
                        z1 = (a.Height * (z1 - a.Z) + b.Height * (z1 - b.Z) + c.Height * (z1 - c.Z) + d.Height * (z1 - d.Z));
                        l1 = 5.0 * Math.Sqrt(x1 * x1 + y1 * y1 + z1 * z1);
                        x1 += x * l1; y1 += y * l1; z1 += z * l1;
                    }
                    l1 = Math.Sqrt(x1 * x1 + y1 * y1 + z1 * z1);
                    if (l1 == 0.0) l1 = 1.0;
                    x2 = Math.Cos(PI * ShadeAngle / 180.0 - 0.5 * PI) * Math.Cos(PI * ShadeAngle2 / 180.0);
                    y2 = -Math.Sin(PI * ShadeAngle2 / 180.0);
                    z2 = -Math.Sin(PI * ShadeAngle / 180.0 - 0.5 * PI) * Math.Cos(PI * ShadeAngle2 / 180.0);
                    Shade = (int)((x1 * x2 + y1 * y2 + z1 * z2) / l1 * 170.0 + 10);
                    if (Shade < 10) Shade = 10;
                    if (Shade > 255) Shade = 255;
                }
                RainShadow = 0.25 * (a.Shadow + b.Shadow + c.Shadow + d.Shadow);
                return 0.25 * (a.Height + b.Height + c.Height + d.Height);
            }
        }

        /// <summary>
        /// Cache related function at level 11
        /// </summary>
        protected double Planet1(double x, double y, double z)
        {
            Vertex a, b, c, d;

            double abx, aby, abz, acx, acy, acz, adx, ady, adz, apx, apy, apz;
            double bax, bay, baz, bcx, bcy, bcz, bdx, bdy, bdz, bpx, bpy, bpz;

            /* check if point is inside cached tetrahedron */

            abx = SSB.X - SSA.X; aby = SSB.Y - SSA.Y; abz = SSB.Z - SSA.Z;
            acx = SSC.X - SSA.X; acy = SSC.Y - SSA.Y; acz = SSC.Z - SSA.Z;
            adx = SSD.X - SSA.X; ady = SSD.Y - SSA.Y; adz = SSD.Z - SSA.Z;
            apx = x - SSA.X; apy = y - SSA.Y; apz = z - SSA.Z;

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
                        bcx = SSC.X - SSB.X; bcy = SSC.Y - SSB.Y; bcz = SSC.Z - SSB.Z;
                        bdx = SSD.X - SSB.X; bdy = SSD.Y - SSB.Y; bdz = SSD.Z - SSB.Z;
                        bpx = x - SSB.X; bpy = y - SSB.Y; bpz = z - SSB.Z;
                        if ((bax * bcy * bdz + bay * bcz * bdx + baz * bcx * bdy
                        - baz * bcy * bdx - bay * bcx * bdz - bax * bcz * bdy) *
                        (bpx * bcy * bdz + bpy * bcz * bdx + bpz * bcx * bdy
                             - bpz * bcy * bdx - bpy * bcx * bdz - bpx * bcz * bdy) > 0.0)
                        {
                            /* p is on same side of bcd as a */
                            /* Hence, p is inside cached tetrahedron */
                            /* so we start from there */
                            return (Planet(SSA, SSB, SSC, SSD, x, y, z, 11));
                        }
                    }
                }
            }
            /* otherwise, we start from scratch */

            return (Planet(Tetra[0], Tetra[1], Tetra[2], Tetra[3],
                          /* vertices of tetrahedron */
                          x, y, z,   /* coordinates of point we want colour of */
                          Depth)); /* subdivision depth */
        }
        #endregion
    }

    internal sealed class Peter: Projector
    {
        public override void Map()
        {
            double y, cos2, theta1, scale1;
            int k, i, j, water, land;

            y = 2.0 * Math.Sin(LatRadians);
            k = (int)(0.5 * y * Width * Scale / PI + 0.5);
            water = land = 0;
            for (j = 0; j < Height; j++)
            {
                if (Debug && ((j % (Height / 25)) == 0))
                    Console.WriteLine(View);
                y = 0.5 * PI * (2.0 * (j - k) - Height) / Width / Scale;
                if (Math.Abs(y) > 1.0)
                    for (i = 0; i < Width; i++)
                    {
                        Col[i][j] = (ushort)BACK;
                        if (DoShade > 0) Shades[i][j] = 255;
                    }
                else
                {
                    cos2 = Math.Sqrt(1.0 - y * y);
                    if (cos2 > 0.0)
                    {
                        scale1 = Scale * Width / Height / cos2 / PI;
                        Depth = 3 * ((int)(PlanetGenerator.Log2(scale1 * Height))) + 3;
                        for (i = 0; i < Width; i++)
                        {
                            theta1 = LongiRadians - 0.5 * PI + PI * (2.0 * i - Width) / Width / Scale;
                            Planet0(Math.Cos(theta1) * cos2, y, -Math.Sin(theta1) * cos2, i, j);
                            if (Col[i][j] < LAND) water++; else land++;
                        }
                    }
                }
            }

            if (Debug)
                Console.WriteLine($"water percentage: {100 * water / (water + land)}\n");
        }
    }

    internal sealed class SquareP : Projector
    {
        public override void Map()
        {
            double y, scale1, theta1, cos2;
            int k, i, j;

            k = (int)(0.5 * LatRadians * Width * Scale / PI + 0.5);
            for (j = 0; j < Height; j++)
            {
                if (Debug && ((j % (Height / 25)) == 0))
                    Console.WriteLine(View);
                y = (2.0 * (j - k) - Height) / Width / Scale * PI;
                if (Math.Abs(y + y) > PI)
                    for (i = 0; i < Width; i++)
                    {
                        Col[i][j] = (ushort)BACK;
                        if (DoShade > 0) Shades[i][j] = 255;
                    }
                else
                {
                    cos2 = Math.Cos(y);
                    if (cos2 > 0.0)
                    {
                        scale1 = Scale * Width / Height / cos2 / PI;
                        Depth = 3 * ((int)(PlanetGenerator.Log2(scale1 * Height))) + 3;
                        for (i = 0; i < Width; i++)
                        {
                            theta1 = LongiRadians - 0.5 * PI + PI * (2.0 * i - Width) / Width / Scale;
                            Planet0(Math.Cos(theta1) * cos2, Math.Sin(y), -Math.Sin(theta1) * cos2, i, j);
                        }
                    }
                }
            }
        }
    }

    internal sealed class Mercator : Projector
    {
        public override void Map()
        {
            double y, scale1, cos2, theta1;
            int i, j, k;

            y = Math.Sin(LatRadians);
            y = (1.0 + y) / (1.0 - y);
            y = 0.5 * Math.Log(y);
            k = (int)(0.5 * y * Width * Scale / PI + 0.5);
            for (j = 0; j < Height; j++)
            {
                if (Debug && ((j % (Height / 25)) == 0))
                    Console.WriteLine(View);
                y = PI * (2.0 * (j - k) - Height) / Width / Scale;
                y = Math.Exp(2.0 * y);
                y = (y - 1.0) / (y + 1.0);
                scale1 = Scale * Width / Height / Math.Sqrt(1.0 - y * y) / PI;
                cos2 = Math.Sqrt(1.0 - y * y);
                Depth = 3 * ((int)(PlanetGenerator.Log2(scale1 * Height))) + 3;
                for (i = 0; i < Width; i++)
                {
                    theta1 = LongiRadians - 0.5 * PI + PI * (2.0 * i - Width) / Width / Scale;
                    Planet0(Math.Cos(theta1) * cos2, y, -Math.Sin(theta1) * cos2, i, j);
                }
            }
        }
    }

    internal sealed class Mollweide : Projector
    {
        public override void Map()
        {
            double y, y1, zz, scale1, cos2, theta1, theta2;
            int i, j, i1 = 1, k;

            for (j = 0; j < Height; j++)
            {
                if (Debug && ((j % (Height / 25)) == 0))
                    Console.WriteLine(View);
                y1 = 2 * (2.0 * j - Height) / Width / Scale;
                if (Math.Abs(y1) >= 1.0) for (i = 0; i < Width; i++)
                    {
                        Col[i][j] = (ushort)BACK;
                        if (DoShade > 0) Shades[i][j] = 255;
                    }
                else
                {
                    zz = Math.Sqrt(1.0 - y1 * y1);
                    y = 2.0 / PI * (y1 * zz + Math.Asin(y1));
                    cos2 = Math.Sqrt(1.0 - y * y);
                    if (cos2 > 0.0)
                    {
                        scale1 = Scale * Width / Height / cos2 / PI;
                        Depth = 3 * ((int)(PlanetGenerator.Log2(scale1 * Height))) + 3;
                        for (i = 0; i < Width; i++)
                        {
                            theta1 = PI / zz * (2.0 * i - Width) / Width / Scale;
                            if (Math.Abs(theta1) > PI)
                            {
                                Col[i][j] = (ushort)BACK;
                                if (DoShade > 0) Shades[i][j] = 255;
                            }
                            else
                            {
                                double x2, y2, z2, x3, y3, z3;
                                theta1 += -0.5 * PI;
                                x2 = Math.Cos(theta1) * cos2;
                                y2 = y;
                                z2 = -Math.Sin(theta1) * cos2;
                                x3 = Clo * x2 + Slo * Sla * y2 + Slo * Cla * z2;
                                y3 = Cla * y2 - Sla * z2;
                                z3 = -Slo * x2 + Clo * Sla * y2 + Clo * Cla * z2;

                                Planet0(x3, y3, z3, i, j);
                            }
                        }
                    }
                }
            }
        }
    }

    internal sealed class Sinusoid : Projector
    {
        public override void Map()
        {
            double y, theta1, theta2, cos2, l1, i1, scale1;
            int k, i, j, l, c;

            k = (int)(LatRadians * Width * Scale / PI + 0.5);
            for (j = 0; j < Height; j++)
            {
                if (Debug && ((j % (Height / 25)) == 0))
                    Console.WriteLine(View);
                y = (2.0 * (j - k) - Height) / Width / Scale * PI;
                if (Math.Abs(y + y) > PI) for (i = 0; i < Width; i++)
                    {
                        Col[i][j] = (ushort)BACK;
                        if (DoShade > 0) Shades[i][j] = 255;
                    }
                else
                {
                    cos2 = Math.Cos(y);
                    if (cos2 > 0.0)
                    {
                        scale1 = Scale * Width / Height / cos2 / PI;
                        Depth = 3 * ((int)(PlanetGenerator.Log2(scale1 * Height))) + 3;
                        for (i = 0; i < Width; i++)
                        {
                            l = (int)(i * 12 / Width / Scale);
                            l1 = l * Width * Scale / 12.0;
                            i1 = i - l1;
                            theta2 = LongiRadians - 0.5 * PI + PI * (2.0 * l1 - Width) / Width / Scale;
                            theta1 = (PI * (2.0 * i1 - Width * Scale / 12.0) / Width / Scale) / cos2;
                            if (Math.Abs(theta1) > PI / 12.0)
                            {
                                Col[i][j] = (ushort)BACK;
                                if (DoShade > 0) Shades[i][j] = 255;
                            }
                            else
                            {
                                Planet0(Math.Cos(theta1 + theta2) * cos2, Math.Sin(y), -Math.Sin(theta1 + theta2) * cos2,
                                        i, j);
                            }
                        }
                    }
                }
            }
        }
    }

    internal sealed class Stereo : Projector
    {
        public override void Map()
        {
            double x, y, ymin, ymax, z, zz, x1, y1, z1, theta1, theta2;
            int i, j;

            ymin = 2.0;
            ymax = -2.0;
            for (j = 0; j < Height; j++)
            {
                if (Debug && ((j % (Height / 25)) == 0))
                    Console.WriteLine(View);
                for (i = 0; i < Width; i++)
                {
                    x = (2.0 * i - Width) / Height / Scale;
                    y = (2.0 * j - Height) / Height / Scale;
                    z = x * x + y * y;
                    zz = 0.25 * (4.0 + z);
                    x = x / zz;
                    y = y / zz;
                    z = (1.0 - 0.25 * z) / zz;
                    x1 = Clo * x + Slo * Sla * y + Slo * Cla * z;
                    y1 = Cla * y - Sla * z;
                    z1 = -Slo * x + Clo * Sla * y + Clo * Cla * z;
                    if (y1 < ymin) ymin = y1;
                    if (y1 > ymax) ymax = y1;

                    /* for level-of-detail effect:
                       Depth = 3*((int)(log_2(scale*Height)/(1.0+x1*x1+y1*y1)))+6; */

                    Planet0(x1, y1, z1, i, j);
                }
            }
        }
    }

    internal sealed class Orthographic : Projector
    {
        public override void Map()
        {
            double x, y, z, x1, y1, z1, ymin, ymax, theta1, theta2, zz;
            int i, j;

            ymin = 2.0;
            ymax = -2.0;
            for (j = 0; j < Height; j++)
            {
                if (Debug && ((j % (Height / 25)) == 0))
                    Console.WriteLine(View);
                for (i = 0; i < Width; i++)
                {
                    x = (2.0 * i - Width) / Height / Scale;
                    y = (2.0 * j - Height) / Height / Scale;
                    if (x * x + y * y > 1.0)
                    {
                        Col[i][j] = (ushort)BACK;
                        if (DoShade > 0) Shades[i][j] = 255;
                    }
                    else
                    {
                        z = Math.Sqrt(1.0 - x * x - y * y);
                        x1 = Clo * x + Slo * Sla * y + Slo * Cla * z;
                        y1 = Cla * y - Sla * z;
                        z1 = -Slo * x + Clo * Sla * y + Clo * Cla * z;
                        if (y1 < ymin) ymin = y1;
                        if (y1 > ymax) ymax = y1;
                        Planet0(x1, y1, z1, i, j);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Modified version of gnomonic
    /// </summary>
    internal sealed class Icosahedral : Projector
    {
        public override void Map()
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
                if (Debug && ((j % (Height / 25)) == 0))
                    Console.WriteLine(View);
                for (i = 0; i < Width; i++)
                {

                    x0 = 198.0 * (2.0 * i - Width) / Width / Scale - 36;
                    y0 = 198.0 * (2.0 * j - Height) / Width / Scale - LatRadians / DEG2RAD;

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
                        Col[i][j] = (ushort)BACK;
                        if (DoShade > 0) Shades[i][j] = 255;
                    }
                    else
                    {
                        x = (x0 - longi1) / S;
                        y = (y0 + lat1) / S;

                        longi1 = longi1 * DEG2RAD - LongiRadians;
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
                        Planet0(x1, y1, z1, i, j);
                    }
                }
            }
        }
    }

    internal sealed class Gnomonic : Projector
    {
        public override void Map()
        {
            double x, y, z, x1, y1, z1, zz, theta1, theta2, ymin, ymax;
            int i, j;

            ymin = 2.0;
            ymax = -2.0;
            for (j = 0; j < Height; j++)
            {
                if (Debug && ((j % (Height / 25)) == 0))
                    Console.WriteLine(View);
                for (i = 0; i < Width; i++)
                {
                    x = (2.0 * i - Width) / Height / Scale;
                    y = (2.0 * j - Height) / Height / Scale;
                    zz = Math.Sqrt(1.0 / (1.0 + x * x + y * y));
                    x = x * zz;
                    y = y * zz;
                    z = Math.Sqrt(1.0 - x * x - y * y);
                    x1 = Clo * x + Slo * Sla * y + Slo * Cla * z;
                    y1 = Cla * y - Sla * z;
                    z1 = -Slo * x + Clo * Sla * y + Clo * Cla * z;
                    if (y1 < ymin) ymin = y1;
                    if (y1 > ymax) ymax = y1;
                    Planet0(x1, y1, z1, i, j);
                }
            }
        }
    }

    internal sealed class Azimuth : Projector
    {
        public override void Map()
        {
            double x, y, z, x1, y1, z1, zz, theta1, theta2, ymin, ymax;
            int i, j;

            ymin = 2.0;
            ymax = -2.0;
            for (j = 0; j < Height; j++)
            {
                if (Debug && ((j % (Height / 25)) == 0))
                    Console.WriteLine(View);
                for (i = 0; i < Width; i++)
                {
                    x = (2.0 * i - Width) / Height / Scale;
                    y = (2.0 * j - Height) / Height / Scale;
                    zz = x * x + y * y;
                    z = 1.0 - 0.5 * zz;
                    if (z < -1.0)
                    {
                        Col[i][j] = (ushort)BACK;
                        if (DoShade > 0) Shades[i][j] = 255;
                    }
                    else
                    {
                        zz = Math.Sqrt(1.0 - 0.25 * zz);
                        x = x * zz;
                        y = y * zz;
                        x1 = Clo * x + Slo * Sla * y + Slo * Cla * z;
                        y1 = Cla * y - Sla * z;
                        z1 = -Slo * x + Clo * Sla * y + Clo * Cla * z;
                        if (y1 < ymin) ymin = y1;
                        if (y1 > ymax) ymax = y1;
                        Planet0(x1, y1, z1, i, j);
                    }
                }
            }
        }
    }

    internal sealed class Conical : Projector
    {
        public override void Map()
        {
            double k1, c, y2, x, y, zz, x1, y1, z1, theta1, theta2, ymin, ymax, cos2;
            int i, j;

            ymin = 2.0;
            ymax = -2.0;
            if (LatRadians > 0)
            {
                k1 = 1.0 / Math.Sin(LatRadians);
                c = k1 * k1;
                y2 = Math.Sqrt(c * (1.0 - Math.Sin(LatRadians / k1)) / (1.0 + Math.Sin(LatRadians / k1)));
                for (j = 0; j < Height; j++)
                {
                    if (Debug && ((j % (Height / 25)) == 0))
                        Console.WriteLine(View);
                    for (i = 0; i < Width; i++)
                    {
                        x = (2.0 * i - Width) / Height / Scale;
                        y = (2.0 * j - Height) / Height / Scale + y2;
                        zz = x * x + y * y;
                        if (zz == 0.0) theta1 = 0.0; else theta1 = k1 * Math.Atan2(x, y);
                        if (theta1 < -PI || theta1 > PI)
                        {
                            Col[i][j] = (ushort)BACK;
                            if (DoShade > 0) Shades[i][j] = 255;
                        }
                        else
                        {
                            theta1 += LongiRadians - 0.5 * PI; /* theta1 is longitude */
                            theta2 = k1 * Math.Asin((zz - c) / (zz + c));
                            /* theta2 is latitude */
                            if (theta2 > 0.5 * PI || theta2 < -0.5 * PI)
                            {
                                Col[i][j] = (ushort)BACK;
                                if (DoShade > 0) Shades[i][j] = 255;
                            }
                            else
                            {
                                cos2 = Math.Cos(theta2);
                                y = Math.Sin(theta2);
                                if (y < ymin) ymin = y;
                                if (y > ymax) ymax = y;
                                Planet0(Math.Cos(theta1) * cos2, y, -Math.Sin(theta1) * cos2, i, j);
                            }
                        }
                    }
                }
            }
            else
            {
                k1 = 1.0 / Math.Sin(LatRadians);
                c = k1 * k1;
                y2 = Math.Sqrt(c * (1.0 - Math.Sin(LatRadians / k1)) / (1.0 + Math.Sin(LatRadians / k1)));
                for (j = 0; j < Height; j++)
                {
                    if (Debug && ((j % (Height / 25)) == 0))
                        Console.WriteLine(View);
                    for (i = 0; i < Width; i++)
                    {
                        x = (2.0 * i - Width) / Height / Scale;
                        y = (2.0 * j - Height) / Height / Scale - y2;
                        zz = x * x + y * y;
                        if (zz == 0.0) theta1 = 0.0; else theta1 = -k1 * Math.Atan2(x, -y);
                        if (theta1 < -PI || theta1 > PI)
                        {
                            Col[i][j] = (ushort)BACK;
                            if (DoShade > 0) Shades[i][j] = 255;
                        }
                        else
                        {
                            theta1 += LongiRadians - 0.5 * PI; /* theta1 is longitude */
                            theta2 = k1 * Math.Asin((zz - c) / (zz + c));
                            /* theta2 is latitude */
                            if (theta2 > 0.5 * PI || theta2 < -0.5 * PI)
                            {
                                Col[i][j] = (ushort)BACK;
                                if (DoShade > 0) Shades[i][j] = 255;
                            }
                            else
                            {
                                cos2 = Math.Cos(theta2);
                                y = Math.Sin(theta2);
                                if (y < ymin) ymin = y;
                                if (y > ymax) ymax = y;
                                Planet0(Math.Cos(theta1) * cos2, y, -Math.Sin(theta1) * cos2, i, j);
                            }
                        }
                    }
                }
            }
        }
    }
}
