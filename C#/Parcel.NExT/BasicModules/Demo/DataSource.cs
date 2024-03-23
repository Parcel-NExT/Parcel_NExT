using Parcel.CoreEngine.SemanticTypes;

namespace Demo
{
    public static class DataSource
    {
        public static DataGrid? CSV(Uri path)
        {
            if (path == null)
                return null;

            if (path.HostNameType == UriHostNameType.Basic) // TODO: I expect for local files the protocol starts like this: `file://` (to be verified)
            {
                // TODO: Implement DataGrid handling and proper data check
                string csv = File.ReadAllText(path.AbsolutePath);
                return new DataGrid(csv);
            }
            else
                throw new NotImplementedException();
        }
        /// <summary>
        /// Fetch from ODBC
        /// </summary>
        public static string ODBC(string dsn, string query)
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
    }
}
