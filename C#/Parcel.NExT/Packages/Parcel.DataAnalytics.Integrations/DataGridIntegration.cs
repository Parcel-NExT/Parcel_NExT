using Parcel.CoreEngine.Helpers;
using Parcel.Types;

namespace Parcel.Integration
{
    /// <summary>
    /// Expose strongly named functions
    /// </summary>
    public static class DataGridIntegration
    {
        #region Construction
        public static DataGrid InitializeDataGridFromCsvText(string tableName, string csvText)
            => new(tableName, csvText.ParseCSV(out string[]? headers, true), headers);
        #endregion
    }
}
