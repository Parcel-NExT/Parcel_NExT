using Parcel.Infrastructure.Networking.LowLevel;

namespace Parcel.Infrastructure.Networking.Managed
{
    public class ListeningClient<TSendingType, TReceivingType> 
        where TSendingType : IParcelSerializable
        where TReceivingType : IParcelSerializable
    {
        #region Internal Data
        private readonly UnidirectionalClient ClientInstance;
        private readonly Func<TReceivingType, TSendingType> CommandHandler;
        private readonly int ServerPort;
        #endregion

        #region Constructor
        public ListeningClient(int serverPort, Func<TReceivingType, TSendingType> handler)
        {
            ClientInstance = new UnidirectionalClient();
            CommandHandler = handler;
            ServerPort = serverPort;
        }
        #endregion

        #region Method
        public void Start()
        {
            ClientInstance.StartClient(ServerPort);

            while (true)
            {
                TReceivingType message = Receive();
                Send(CommandHandler(message));
            }
        }
        public void Send(TSendingType data)
        {
            ClientInstance.Send(data.Serialize());
        }
        public TSendingType SendAndReceive(TSendingType data)
        {
            ClientInstance.SendAndReceive(data.Serialize(), out byte[] replyData, out int replyLength);
            return (TSendingType)Activator.CreateInstance<TSendingType>().Deserialize(replyData, replyLength);
        }
        public TReceivingType Receive()
        {
            ClientInstance.Receive(out byte[] replyData, out int replyLength);
            return (TReceivingType)Activator.CreateInstance<TReceivingType>().Deserialize(replyData, replyLength);
        }
        public void Close()
        {
            ClientInstance.Dispose();
        }
        #endregion
    }
}
