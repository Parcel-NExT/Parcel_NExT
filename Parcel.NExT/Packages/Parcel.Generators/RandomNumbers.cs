namespace Parcel.Data
{
    public static class RandomNumbers
    {
        public static double GenerateRandomNumber()
            => new Random().NextDouble();
        public static double GenerateRandomNumber(double min, double max)
            => new Random().NextDouble() * (max - min) + min;
        public static double GenerateRandomInteger(int min, int max)
            => new Random().Next(min, max);
    }
}
