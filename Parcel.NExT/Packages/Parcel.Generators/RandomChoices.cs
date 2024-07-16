namespace Parcel.Data
{
    public static class RandomChoices
    {
        public static int Choice(int min, int max) => new Random().Next(min, max);
    }
}
