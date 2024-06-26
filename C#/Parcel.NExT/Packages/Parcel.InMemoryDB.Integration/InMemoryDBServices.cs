using Parcel.CoreEngine.Helpers;
using Parcel.Database.InMemoryDB.Services;
using System.Text;

namespace Parcel.Database.InMemoryDB
{
    public static class InMemoryDBServices
    {
        #region State Bookkeeping

        #endregion

        #region Service Entry
        /// <param name="connection">Can be either on disk or :empty:</param>
        public static string StartEmptyDatabaseService(out InMemorySQLIte database, bool startSocketServer, bool startRESTServer, bool startWebServer, string connection = InMemorySQLIte.InMemorySQLiteDatabaseConnectionSourceNameToken)
        {
            // Validate connection
            try
            {
                if (connection != InMemorySQLIte.InMemorySQLiteDatabaseConnectionSourceNameToken && Path.GetFullPath(connection) != connection) // Remark: The Path.GetFullPath is to validate the path is a valid windows location, while the file may not exist yet
                    throw new ArgumentException($"Connection {connection} is invalid. Only in-memory or on-disk source is supported.");
            }
            catch (Exception)
            {
                throw new ArgumentException($"Connection {connection} is invalid. Only in-memory or on-disk source is supported.");
            }

            // Create new database and configure services for it
            database = new(connection);
            string result = ConfigureService(database, startSocketServer, startRESTServer, startWebServer);
            return result;
        }
        /// <returns>Service endpoints summary</returns>
        public static string ConfigureService(this InMemorySQLIte database, bool startSocketServer, bool startRESTServer, bool startWebsiteServer)
        {
            // TODO: Reconfigure existing service instead of starting new ones
            // TODO: Provide more practical configuration options
            // TODO: Provide dedicated method for actually starting new ones instead of reconfigure existing ones
            // TODO: Provide dedicated method for killing/stopping existing services
            // TODO: Bookkeep started services

            StringBuilder builder = new();
            if (startSocketServer)
            {
                SocketService socketService = new(database);
                string socketServerAddress = socketService.Start();
                builder.AppendLine($"Socket server: {socketServerAddress}");
            }
            if (startRESTServer)
            {
                RESTAPIService restServerService = new(database);
                string restServerAddress = restServerService.Start();
                builder.AppendLine($"REST API server: {restServerAddress}");
            }
            if (startWebsiteServer)
            {
                HTTPWebsiteService websiteService = new(database);
                string websiteServerAddress = websiteService.Start();
                builder.AppendLine($"Website (HTTP) server: {websiteServerAddress}");

                ProcessHelper.OpenFileWithDefaultProgramInterpreted(websiteServerAddress);
            }

            return builder.ToString().TrimEnd();
        }
        #endregion
    }
}
