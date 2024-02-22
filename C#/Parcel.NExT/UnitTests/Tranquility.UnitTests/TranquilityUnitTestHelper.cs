using WebSocketSharp;
using WebSocketSharp.Server;

namespace Tranquility.UnitTests
{
    internal static class TranquilityUnitTestHelper
    {
        #region Requests
        public static string RunSingleRequest(string message)
        {
            int port = Program.FindNextFreeTcpPort();
            string address = $"ws://localhost:{port}";
            string endpoint = "/Tranquility";

            WebSocketServer server = new(address);
            server.AddWebSocketService<TranquilitySession>(endpoint);
            server.Start();

            string? replyMessage = null;
            var client = new WebSocket($"{address}{endpoint}");
            client.OnMessage += (sender, e) =>
                replyMessage = e.Data;
            bool task = Task.Run(() =>
            {
                client.Connect();
                client.Send(message);
            }).Wait(1000);

            Thread.Sleep(2000);
            client.Close();
            server.Stop();

            if (replyMessage == null)
                throw new ApplicationException("No reply message received.");

            return replyMessage!;
        }
        #endregion

        #region MyRegion
        public static object DefaultDeserialization(string message)
        {
            return Parcel.CoreEngine.Messaging.StandardDeserialization.Deserialize(message);
        }
        #endregion
    }
}
