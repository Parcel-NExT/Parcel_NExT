using Parcel.CoreEngine.Helpers;

namespace Parcel.FileFormats
{
    public static class CSV
    {
        #region Interface
        public static string[][] Parse(string csv, out string[]? headers, bool containsHeader)
        {
            IEnumerable<string[]> lines = CSVHelper.ParseCSV(csv, out headers, containsHeader);
            return lines.ToArray();
        }
        #endregion
    }
}
