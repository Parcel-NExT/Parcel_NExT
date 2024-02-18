
namespace Parcel.CoreEngine.Service.Types
{
    /// <summary>
    /// A string structure that can either represent a single scalar string or an array of strings
    /// </summary>
    public sealed class SimplexString
    {
        #region Construction
        public SimplexString(params string[] initialization)
        {
            ItemCount = initialization.Length;
            if (ItemCount == 1)
                SingleValue = initialization.Single();
            else ArrayValues = initialization;
        }
        #endregion

        #region Properties
        public int ItemCount { get; }
        public string[]? ArrayValues { get; }
        public string? SingleValue { get; }
        #endregion

        #region String Conversion
        public string ToJSONString() => ItemCount == 1
            ? $"{Escape(SingleValue!)}"
            : $"[{string.Join(", ", ArrayValues!.Select(Escape))}]";
        public override string ToString() => ToJSONString();
        #endregion

        #region Helpers
        private object Escape(string value)
        {
            return $"\"{value.Replace("\"", @"\""")}\"";
        }
        #endregion
    }
}
