using System.Net.Sockets;
using System.Net;
using WebSocketSharp.Server;

namespace Tranquility
{
    public class TranquilityOptions
    {
        public string ServerAddress { get; set; }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0 || args.First() == "--help")
            {
                PrintHelp();
                return;
            }
            else if (args.First() == "--get_port")
            {
                int nextPort = FindNextFreeTcpPort();
                Console.WriteLine(nextPort);
                return;
            }

            TranquilityOptions options = ParseOptions(args);

            Logging.Info($"Start {nameof(Tranquility)} at {options.ServerAddress}...");
            WebSocketServer wssv = new(options.ServerAddress);

            wssv.AddWebSocketService<TranquilitySession>("/Tranquility");
            wssv.Start();

            Console.WriteLine("Tranquility is started. Press any key to quit.");
            Console.ReadKey(true);
            wssv.Stop();
        }

        #region Routines
        private static void PrintHelp()
        {
            Console.WriteLine("""
                Tranquility is the websocket backend server for Parcel NExT.
                Typically it should be started by the front-end on an as-needed basis.
                Command line options:
                    --help: Print this help message.
                    --address: The address to listen on. Default is ws://localhost:9915.
                    --default: Start tranquility using default options.
                    --get_port: Print a free port number and exit.
                """);
        }

        private static TranquilityOptions? ParseOptions(string[] args)
        {
            const string envVar = "PARCEL_TRANQUILITY_SERVER_ADDRESS";

            // Defaults
            string serverAddress = Environment.GetEnvironmentVariable(envVar) != null
                ? Environment.GetEnvironmentVariable(envVar)!
                : "ws://localhost:9915";

            string? keyword = null;
            string[] validOptions = ["--default", "--address"];
            foreach (var arg in args)
            {
                if (arg.StartsWith("--"))
                    keyword = arg;
                else if (keyword == null)
                {
                    Console.WriteLine($"Invalid argument format: {arg}");
                    return null;
                }
                else
                {
                    switch (keyword)
                    {
                        case "--address":
                            serverAddress = arg;
                            break;
                    }
                }
            }

            return new TranquilityOptions()
            {
                ServerAddress = serverAddress
            };
        }
        public static int FindNextFreeTcpPort()
        {
            TcpListener listener = new(IPAddress.Loopback, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }
        #endregion
    }
}
