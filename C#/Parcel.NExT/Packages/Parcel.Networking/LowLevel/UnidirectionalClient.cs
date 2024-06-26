using System.Net.Sockets;
using System.Net;

namespace Parcel.Infrastructure.Networking.LowLevel
{
    public class UnidirectionalClient : IDisposable
    {
        #region Config
        public static readonly string HostAddress = "127.0.0.1";
        public const int BufferSize = BidirectionalServerClient.BufferSize;
        #endregion

        #region Lifetime
        public void Dispose()
        {
            try
            {
                Socket?.Shutdown(SocketShutdown.Both);
            }
            catch (Exception) { }
            finally
            {
                Socket?.Dispose();
            }
        }
        #endregion

        #region Members
        private Socket? Socket;
        #endregion

        #region Entry
        public void StartClient(int servicePort)
        {
            IPHostEntry entry = Dns.GetHostEntry(HostAddress);
            IPEndPoint endpoint = new(entry.AddressList[0], servicePort);
            Socket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Socket.Connect(endpoint);
        }
        #endregion

        #region Messaging
        public void Send(byte[] data)
        {
            if (data.Length > BufferSize)
                throw new ArgumentException("Invalid data size.");

            Socket?.Send(data);
        }
        public void SendAndReceive(byte[] data, out byte[] replyData, out int replyLength)
        {
            if (data.Length > BufferSize)
                throw new ArgumentException("Invalid data size.");

            // Send
            Socket?.Send(data);
            // Receive
            Receive(out replyData, out replyLength);
        }
        public void Receive(out byte[] replyData, out int replyLength)
        {
            // Receive
            replyData = new byte[BufferSize];
            replyLength = Socket?.Receive(replyData) ?? 0;
        }
        #endregion
    }
}
