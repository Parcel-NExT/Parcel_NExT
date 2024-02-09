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
            if (args.Length == 0 || args.FirstOrDefault() == "--help")
            {
                PrintHelp();
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

        private static void PrintHelp()
        {
            Console.WriteLine("""
                Tranquility is the websocket backend server for Parcel NExT.
                Typically it should be started by the front-end on an as-needed basis.
                Command line options:
                    --help: Print this help message.
                    --address: The address to listen on. Default is ws://localhost:9915.
                    --default: Start tranquility using default options.
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
    }
}
