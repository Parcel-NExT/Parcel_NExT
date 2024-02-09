using WebSocketSharp.Server;

namespace Tranquility
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const string envVar = "PARCEL_TRANQUILITY_SERVER_ADDRESS";
            const string defaultAddress = "ws://localhost:9915";

            string serverAddress = args.Length > 0
                ? args[0]
                : Environment.GetEnvironmentVariable(envVar) != null
                    ? Environment.GetEnvironmentVariable(envVar)!
                    : defaultAddress;

            Logging.Info($"Start {nameof(Tranquility)} at {serverAddress}...");
            WebSocketServer wssv = new(serverAddress);

            wssv.AddWebSocketService<TranquilitySession>("/Tranquility");
            wssv.Start();

            Console.WriteLine("Tranquility is started. Press any key to quit.");
            Console.ReadKey(true);
            wssv.Stop();
        }
    }
}
