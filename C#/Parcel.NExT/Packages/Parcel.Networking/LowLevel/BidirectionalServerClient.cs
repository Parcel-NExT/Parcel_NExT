using System.Net.Sockets;
using System.Net;

namespace Parcel.Infrastructure.Networking.LowLevel
{
    /// <summary>
    /// Byte-based bidirectional server/client.
    /// Used for local between-app communications.
    /// </summary>
    public class BidirectionalServerClient : IDisposable
    {
        #region Config
        public static readonly string HostAddress = "127.0.0.1";
        public const int BufferSize = 8 * 1024 * 1024; // 8 Mb
        #endregion

        #region Lifetime
        public void Dispose()
        {
            try
            {
                Socket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception) { }
            finally
            {
                Socket.Dispose();
            }
        }
        #endregion

        #region Members
        Socket Socket; // Represents server (listening port) on server (not the client!), and server/client on client.
        #endregion

        #region Entry Point
        public int StartBytesServer(Action<int, byte[], Socket> clientMessageHandlingCallback)
        {
            List<Socket> clients = [];

            int servicePort = TcpHelper.FindAvailablePort();
            IPHostEntry entry = Dns.GetHostEntry(HostAddress);
            IPEndPoint endpoint = new(entry.AddressList[0], servicePort);
            Socket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Socket.Bind(endpoint);
            Socket.Listen(100);
            new Thread(() =>
            {
                try
                {
                    while (true)
                    {
                        Socket client = Socket.Accept();
                        Console.WriteLine("New client is connected.");
                        clients.Add(client);
                        new Thread(() => ServerHandleClient(client, clientMessageHandlingCallback)).Start();
                    }
                }
                catch (Exception) { }
            }).Start();
            return servicePort;

            static void ServerHandleClient(Socket client, Action<int, byte[], Socket> callback)
            {
                try
                {
                    while (true)
                    {
                        byte[] buffer = new byte[BufferSize];
                        var size = client.Receive(buffer);
                        callback(size, buffer, client);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
        public Socket StartBytesClient(int servicePort, Action<int, byte[]> serverMessageCallback)
        {
            IPHostEntry entry = Dns.GetHostEntry(HostAddress);
            IPEndPoint endpoint = new(entry.AddressList[0], servicePort);
            Socket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Socket.Connect(endpoint);
            new Thread(() => ClientReceiveMessage(Socket, serverMessageCallback)).Start();
            return Socket;

            static void ClientReceiveMessage(Socket socket, Action<int, byte[]> callback)
            {
                try
                {
                    while (true)
                    {
                        byte[] buffer = new byte[BufferSize];
                        int size = socket.Receive(buffer);
                        callback(size, buffer);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
        #endregion

        #region Messaging
        public void Send(Socket connection, byte[] data)
        {
            if (data.Length > BufferSize)
                throw new ArgumentException("Invalid data size.");

            connection.Send(data);
        }
        #endregion
    }
}
