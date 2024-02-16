using Parcel.CoreEngine.Service.LibraryProvider;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Tranquility.UnitTests
{
    public class BasicConnectionTest
    {
        [Fact]
        public void BasicCommunicationTest()
        {
            int port = Program.FindNextFreeTcpPort();
            string address = $"ws://localhost:{port}";
            string endpoint = "/Tranquility";

            WebSocketServer server = new(address);
            server.AddWebSocketService<TranquilitySession>(endpoint);
            server.Start();

            int count = 0;
            var client = new WebSocket($"{address}{endpoint}");
            client.OnMessage += (sender, e) =>
                count = e.Data.Split('\n').Length;
            var task = Task.Run(() =>
            {
                client.Connect();
                client.Send(nameof(LibraryProviderServices.GetAvailableRuntimes));
            }).Wait(1000);

            Thread.Sleep(2000);
            client.Close();
            server.Stop();

            Assert.Equal(new LibraryProviderServices().GetAvailableRuntimes().Length, count);
        }
    }
}