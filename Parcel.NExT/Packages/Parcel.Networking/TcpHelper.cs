using System.Net.Sockets;
using System.Net;

namespace Parcel.Infrastructure.Networking
{
    public static class TcpHelper
    {
        public static int FindAvailablePort()
        {
            TcpListener listener = new(IPAddress.Loopback, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }
    }
}
