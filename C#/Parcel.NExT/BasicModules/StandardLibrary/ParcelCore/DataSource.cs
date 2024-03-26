using Parcel.CoreEngine.SemanticTypes;

namespace StandardLibrary.ParcelCore
{
    public static class DataSource
    {
        /// <summary>
        /// Reads a CSV and returns content as DataGrid
        /// </summary>
        public static DataGrid? LoadFromCSV(Uri uri)
        {
            if (uri == null)
                return null;

            if (uri.HostNameType == UriHostNameType.Basic) // TODO: I expect for local files the protocol starts like this: `file://` (to be verified)
            {
                // TODO: Implement DataGrid handling and proper data check;
                // TODO: Handle loading CSV from http/https
                string filePath = uri.AbsolutePath;
                if (File.Exists(filePath))
                {
                    string csv = File.ReadAllText(filePath);
                    return new DataGrid(csv);
                }
                return null;
            }
            else
                throw new NotImplementedException();
        }

        /// <summary>
        /// Fetch from ODBC
        /// </summary>
        public static string LoadFromODBC(string dsn, string query)
        {
            throw new NotImplementedException();
        }
    }
}
