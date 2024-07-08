using RandomNameGeneratorNG;

namespace Parcel.Neo.Base.Toolboxes.Generator
{
    public static class RandomNames
    {
        public static string GetRandomEnglishName() => new PersonNameGenerator().GenerateRandomFirstAndLastName();
        public static string[] GetRandomEnglishNames(int count) => new PersonNameGenerator().GenerateMultipleFirstAndLastNames(count).ToArray();
    }
}