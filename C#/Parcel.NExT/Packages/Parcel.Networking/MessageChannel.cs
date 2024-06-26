using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Parcel.Infrastructure.Networking
{
    /// <summary>
    /// Message-based bidirectional server/client.
    /// </summary>
    public class MessageChannel : IDisposable
    {
        public enum ServiceMode
        {
            Server,
            Client
        }

        #region Config
        private const string DefaultHostAddress = "127.0.0.1";
        private const int BufferSize = 8 * 1024 * 1024; // 8 Mb; Max message size.
        private const int DefaultNetworkPort = 12900;
        #endregion

        #region Properties
        /// <summary>
        /// Friendly identifier, bookkeeped for auxiliary/(non-unique)identification purpose.
        /// </summary>
        public string Username { get; private set; }
        public Action<string> MessageHandler { get; private set; }
        public string? ServerAddress { get; private set; }
        #endregion

        #region Thread Management
        private ServiceMode Mode;
        private Socket? Socket = null;
        private Thread? ServiceThread = null;
        private bool IsRunning = false;
        private readonly List<Socket> Clients = [];
        #endregion

        #region Construction & Disposal
        public MessageChannel() { }
        public MessageChannel(string username, Action<string> handler)
        {
            Username = username;
            MessageHandler = handler;
        }
        public void Dispose()
            => Stop();
        #endregion

        #region Entry & Stop
        public static MessageChannel Start(ServiceMode mode, Action<string> messageHandler, string identificationName)
        {
            MessageChannel channel = new()
            {
                Mode = mode,
                Username = identificationName,
                IsRunning = true,
                MessageHandler = messageHandler
            };

            switch (mode)
            {
                case ServiceMode.Server:
                    channel.StartAsServer();
                    break;
                case ServiceMode.Client:
                    channel.StartAsClient();
                    break;
                default:
                    throw new ArgumentException("Unidentified mode.");
            }

            return channel;
        }
        public void Stop()
        {
            IsRunning = false;
            ServiceThread?.Interrupt();

            try
            {
                if (Mode == ServiceMode.Client)
                    Socket?.Shutdown(SocketShutdown.Both);
            }
            catch (Exception) { }
            finally
            {
                Socket?.Dispose();
            }

            ServiceThread = null;
            Socket = null;
        }
        #endregion

        #region Methods
        public void MakeRequest(string message)
        {
            if (ServiceThread == null || Socket == null)
                throw new ApplicationException("Service is not initialized.");

            switch (Mode)
            {
                case ServiceMode.Server:
                    ServerBroadcastMessage(message);
                    break;
                case ServiceMode.Client:
                    SendMessage(Socket, message);
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Routines
        private void StartAsServer()
        {
            IPHostEntry entry = Dns.GetHostEntry(DefaultHostAddress);
            IPEndPoint endpoint = new(entry.AddressList[0], DefaultNetworkPort);
            ServerAddress = endpoint.Address.ToString();
            Socket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Socket.Bind(endpoint);
            Socket.Listen(10);
            ServiceThread = new Thread(() =>
            {
                try
                {
                    Console.WriteLine("Server started.");
                    while (IsRunning)
                    {
                        Socket client = Socket.Accept();
                        Clients.Add(client);
                        string clientName = ReceiveMessage(client);  // The first message
                        SendMessage(client, Username);
                        Console.WriteLine($"New client is connected: {clientName}");
                        new Thread(() => WaitForMessage(client)).Start();
                    }
                }
                catch (Exception)
                {
                    // This happens when server is closed
                    Console.WriteLine("Server stopped.");
                }
            });
            ServiceThread.Start();
        }
        private void StartAsClient()
        {
            IPHostEntry entry = Dns.GetHostEntry(DefaultHostAddress);
            IPEndPoint endpoint = new(entry.AddressList[0], DefaultNetworkPort);
            ServerAddress = endpoint.Address.ToString();
            Socket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Socket.Connect(endpoint);
            SendMessage(Socket, Username);  // The first message
            Console.WriteLine($"Server username: {ReceiveMessage(Socket)}");
            ServiceThread = new Thread(() => WaitForMessage(Socket));
            ServiceThread.Start();
        }
        private void WaitForMessage(Socket socket)
        {
            try
            {
                while (IsRunning)
                {
                    string message = ReceiveMessage(socket);
                    MessageHandler?.Invoke(message);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Connection is closed.");
                switch (Mode)
                {
                    case ServiceMode.Server:
                        Clients.Remove(socket);
                        break;
                    case ServiceMode.Client:
                        Stop();
                        break;
                    default:
                        throw new ApplicationException();
                }
            }
        }
        #endregion

        #region Messaging Helper
        private void ServerBroadcastMessage(string message)
        {
            if (Mode != ServiceMode.Server)
                throw new ApplicationException();

            foreach (var client in Clients)
                SendMessage(client, message);
        }
        private static void SendMessage(Socket socket, string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            Send(socket, data);
        }
        private static void Send(Socket socket, byte[] data)
        {
            if (data.Length > BufferSize)
                throw new ArgumentException("Invalid data size.");

            socket.Send(data);
        }
        private static string ReceiveMessage(Socket socket, int maxSize = BufferSize)
        {
            byte[] buffer = new byte[maxSize];
            var size = socket.Receive(buffer);
            var message = Encoding.UTF8.GetString(buffer, 0, size);
            return message;
        }
        #endregion
    }
}
