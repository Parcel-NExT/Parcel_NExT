
namespace Parcel.CoreEngine.Service.Types
{
    /// <summary>
    /// A string structure that can either represent a single scalar string or an array of strings
    /// </summary>
    public sealed class SimplexString
    {
        #region Construction
        public SimplexString(bool forceArray, params string[] initialization)
        {
            if (initialization.Length == 1 && !forceArray)
                SingleValue = initialization.Single();
            else ArrayValues = initialization;
        }
        #endregion

        #region Properties
        public string[]? ArrayValues { get; }
        public string? SingleValue { get; }
        #endregion

        #region String Conversion
        public string ToJSONString() => ArrayValues != null
            ? $"[{string.Join(", ", ArrayValues!.Select(Escape))}]"
            : $"{Escape(SingleValue!)}";
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
