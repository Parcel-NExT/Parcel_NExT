namespace Parcel.Data
{
    public static class RandomDates
    {
        public static DateTime GetToday() => DateTime.Today;
        public static DateTime GetRandomDay() => DateTime.MinValue.AddDays(RandomNumbers.GenerateRandomInteger(0, (int)(DateTime.MaxValue - DateTime.MinValue).TotalDays));
    }
}
