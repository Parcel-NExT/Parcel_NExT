using System.Net;
using System.Net.Sockets;

namespace PackageManager
{
    public static class GenesisConfigurations
    {
        #region Server Ports
        public const int ControlPort = 7775;
        public const int DataPort = 7776;
        #endregion
    }


    internal class Program
    {
        static void Main(string[] args)
        {
            string mode = args[0];
            switch (mode)
            {
                case "server":
                    StartServer();
                    break;
                case "client":
                    StartClient();
                    break;
            }
        }

        private static void StartClient()
        {
            throw new NotImplementedException();
        }

        private static void StartServer()
        {
            throw new NotImplementedException();
        }
    }

    public class ParcelpackageManagerServer
    {
        #region Control
        public static string? AcceptMessage(IPAddress address, int port)
        {
            TcpListener listener = new(address, port);
            listener.Start();
            TcpClient client = listener.AcceptTcpClient();
            StreamReader streamReader = new(client.GetStream());
            string? line = streamReader.ReadLine();
            listener.Stop();
            client.Close();
            return line;
        }
        #endregion

        #region Data
        public static void AcceptFile(IPAddress address, int port, int fileSize, string outputPath)
        {
            TcpListener listener = new(address, port);
            listener.Start();
            TcpClient client = listener.AcceptTcpClient();
            Stream stream = client.GetStream();
            byte[] buffer = new byte[fileSize];
            stream.Read(buffer, 0, buffer.Length);
            File.WriteAllBytes(outputPath, buffer);
            listener.Stop();
            client.Close();
        }
        #endregion
    }
    public class ParcelPackageManagerClient
    {
        #region Control
        public static void SendMessage(string address, int port, string message)
        {
            TcpClient client = new(address, port);
            StreamWriter sw = new(client.GetStream());
            sw.WriteLine(message);
            sw.Flush();
        }
        #endregion

        #region Data
        public static void TransferFile(string address, int port, string path)
        {
            TcpClient client = new TcpClient(address, port);
            Stream stream = client.GetStream();
            byte[] bytes = File.ReadAllBytes(path);
            stream.Write(bytes, 0, bytes.Length);
            client.Close();
        }
        #endregion
    }
}
