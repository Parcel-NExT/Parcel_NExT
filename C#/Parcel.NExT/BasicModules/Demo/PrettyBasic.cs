using Parcel.CoreEngine.Document;

namespace Demo
{
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

    /// <summary>
    /// A very basic module that exports some very basic operations
    /// </summary>
    public static class PrettyBasic
    {
        /// <summary>
        /// Reads a CSV and returns content as CSV string
        /// </summary>
        public static string ReadCSV(string path)
        {
            return File.ReadAllText(path);
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
    }
}
