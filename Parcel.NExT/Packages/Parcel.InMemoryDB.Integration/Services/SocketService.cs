using Parcel.Infrastructure.Networking;
using Parcel.Types;

namespace Parcel.Database.InMemoryDB.Services
{
    internal class SocketService
    {
        #region Properties
        private InMemorySQLIte Database { get; set; }
        private MessageChannel? Channel { get; set; }
        #endregion

        #region Entrance
        public SocketService(InMemorySQLIte database)
            => Database = database;
        public string Start()
        {
            Channel = MessageChannel.Start(MessageChannel.ServiceMode.Server, CommandHandler, "Server");
            return Channel.ServerAddress!;
        }
        #endregion

        #region Handler
        private void CommandHandler(string command)
        {
            System.Data.DataTable? result = Database.Execute(command);
            string reply = result != null ? result.ToCSV() : "Ok.";

            Channel!.MakeRequest(reply);
        }
        #endregion
    }
}
