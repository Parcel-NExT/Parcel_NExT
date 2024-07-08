using System.Text.RegularExpressions;

namespace Parcel.Standard.Types
{
    /// <remarks>
    /// Should we simply expose everything under System.String instead?
    /// </remarks>
    public static class StringRoutines
    {
        public static int Length(this string value) => value.Length;
        public static string Replace(this string value, string search, string replacement)
            => value.Replace(search, replacement);
        public static string RegularExpressionReplace(this string value, string search, string replacement)
            => Regex.Replace(value, search, replacement);
    }
}
