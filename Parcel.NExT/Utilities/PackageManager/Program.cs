using System.Net.Sockets;

namespace PackageManager
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
        }
    }

    public class ParcelPackageManager
    {
        public void Start(string address, int port)
        {
            TcpClient client = new TcpClient(address, port);
        }
    }
}
