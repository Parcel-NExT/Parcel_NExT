namespace Parcel.Data
{
    public static class RandomChoices
    {
        public static int Choice(int min, int max) => new Random().Next(min, max);
        public static TType Pick<TType>(TType[] choices)
        {
            var rand = new Random();
            return choices[rand.Next(choices.Length)];
        }
    }
}
