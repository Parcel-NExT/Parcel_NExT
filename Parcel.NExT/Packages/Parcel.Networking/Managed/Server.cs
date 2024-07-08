using System.Net.Sockets;
using Parcel.Infrastructure.Networking.LowLevel;

namespace Parcel.Infrastructure.Networking.Managed
{
    public class Server<TSendingType, TReceivingType>
        where TSendingType : IParcelSerializable
        where TReceivingType : IParcelSerializable
    {
        #region Internal Data
        private BidirectionalServerClient Service;
        public int ServicePort { get; private set; }
        private Func<TReceivingType, TSendingType> CommandHandler;
        #endregion

        #region Constructor
        public Server(Func<TReceivingType, TSendingType> handler)
            => CommandHandler = handler;
        #endregion

        #region Method
        public int Start()
        {
            Service = new();
            ServicePort = Service.StartBytesServer((length, data, client) =>
            {
                // Deal with multiple frames
                int remainingSize = length;
                while (remainingSize > 0)
                {
                    int startIndex = length - remainingSize;
                    int frameSize = BitConverter.ToInt32(data, startIndex);
                    Callback(frameSize, data, startIndex, client);
                    remainingSize -= frameSize;
                    if (remainingSize < 0)
                        throw new ApplicationException("Frame size error.");
                }
            });
            return ServicePort;
        }
        public void Stop()
        {
            Service.Dispose();
        }
        #endregion

        #region Data Marshal
        private void Callback(int length, byte[] data, int offset, Socket client)
        {
            try
            {
                TReceivingType command = (TReceivingType)Activator.CreateInstance<TReceivingType>().Deserialize(data, length, offset);
                TSendingType reply = CommandHandler.Invoke(command);
                if (reply != null)
                    Service.Send(client, reply.Serialize());
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }
        #endregion
    }
}
