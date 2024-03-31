using Parcel.CoreEngine.Document;
using Parcel.CoreEngine.SemanticTypes;

namespace StandardLibrary.ParcelCore
{
    /// <summary>
    /// A very basic module that exports some very basic operations
    /// </summary>
    public static class DataAnalytics
    {
        #region Method Options
        public struct ReadCSVOptions
        {
            #region Inputs
            public string Path;
            public bool ContainsHeader;
            public string[] ExpectedColumnTypes;
            public string[] ColumnFiltering;
            public int TakeNRows;
            public char Delimiter;
            #endregion

            #region Outputs
            public bool GeneratesStats;
            public bool GeneratePlainCSVString;
            #endregion
        }
        #endregion

        #region Methods
        public static Dictionary<string, double>? Summarize(DataGrid? data)
        {
            if (data == null)
                return null;

            return new Dictionary<string, double>()
            {
                { "Sum", 15 },
                { "Average", 15 },
                { "Min", 5 },
                { "Length", data.Raw.Length }
            };
        }

        /// <summary>
        /// Reads a csv string and computes all known stats on computable columns;
        /// Assumes csv has header
        /// </summary>
        public static string ComputeTableStats(string csvString)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// A mega node that reads CSV and performs advanced post-processing and generates a bunch of optional outputs
        /// </summary>
        public static ParcelPayload ReadCSV(ReadCSVOptions options)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Queries datagrid using SQL.
        /// </summary>
        /// <remarks>
        /// Internally uses SQLite; Provides oneshot query only.
        /// </remarks>
        /// <returns>
        /// Returns a new DataGrid.
        /// </returns>
        public static ParcelPayload Query(DataGrid data, string sql)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Run SQL query on data
        /// </summary>
        public static string Query(string source1, string source2)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
