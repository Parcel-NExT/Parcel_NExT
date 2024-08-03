namespace ProjectNine.Tooling.Generative
{
    public abstract class MapFunction
    {
        public int Width = 512;
        public int Height = 512;
        public double Longitude = 0.0;
        public double Latitude = 0.0;
        public int HGridSize = 10;
        public int VGridSize = 20;
        public char Projection = 'm';
        public string OutputPrefix = "Output";
        public int Seed = 1;
    }

    public sealed class ZoomIn : MapFunction
    {
        public void Run(int minMagnification, int maxMagnification)
        {
            for (int magnification = minMagnification; magnification <= maxMagnification; magnification++)
            {
                Console.WriteLine(magnification);

                var arguments = $"-ps -s {Seed} -m {magnification * magnification} -L {Latitude} -l {Longitude} -G {HGridSize} -g {VGridSize} -w {Width} -h {Height} -p {Projection} -o {OutputPrefix}_{magnification}.bmp -E -b -z";
                new CommandLineParser().ParseArguments(arguments).Run();
            }
        }
    }
}