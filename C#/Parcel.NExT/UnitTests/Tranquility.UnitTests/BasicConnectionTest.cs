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
            // TODO: Not working - messages are not exchanged
            Assert.True(false);

            var port = Program.FindNextFreeTcpPort();
            var address = $"ws://localhost:{port}";

            WebSocketServer server = new(address);
            server.AddWebSocketService<TranquilitySession>("/Tranquility");
            server.Start();

            var client = new WebSocket(address);
            client.OnMessage += (sender, e) =>
                Assert.Equal(new LibraryProviderServices().GetAvailableRuntimes().Length, e.Data.Split('\n').Length);
            var task = Task.Run(() =>
            {
                client.Connect();
                client.Send(nameof(LibraryProviderServices.GetAvailableRuntimes));
            });

            task.Wait(1000);
            Thread.Sleep(2000);
            client.Close();
            server.Stop();
        }
    }
}