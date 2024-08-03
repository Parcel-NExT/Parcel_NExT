namespace PlanetCSharp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            new OldPlanetC().Run(args);
            BatchGeneration();
        }

        static void BatchGeneration()
        {
            var width = 512;
            var height = 512;
            var magnification = 1;
            var longitude = 0.0;
            var latitude = 0.0;
            var hGridSize = 10;
            var vGridSize = 20;
            var projection = 'm';
            double rotation1 = 0;
            double rotation2 = 0;

            for (int i = 0; i < 100; i++)
            {
                var seed = new Random().NextDouble();

                var arguments1 = $"-ps -s {seed} -m {magnification} -L {latitude} -l {longitude} -G {hGridSize} -g {vGridSize} -w {width} -h {height} -p {projection} -T {rotation1} {rotation2} -o output_{i}.bmp -E -b -z";
                var arguments2 = $"-ps -s {seed} -m 3 -L 30 -l 35 -G 10 -g 20 -w 320 -h 256 -o output_{i}.bmp";

                Console.WriteLine(i);
                new OldPlanetC().Run(arguments1);
            }
        }
    }
}