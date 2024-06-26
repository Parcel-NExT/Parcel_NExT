using Parcel.Infrastructure.Networking.LowLevel;

namespace Parcel.Infrastructure.Networking.Managed
{
    /// <summary>
    /// Initiation-only client; Does not passively listens
    /// </summary>
    public class ActiveClient<TSendingType, TReceivingType>
        where TSendingType : IParcelSerializable
        where TReceivingType : IParcelSerializable
    {
        #region Internal Data
        private readonly UnidirectionalClient ClientInstance;
        private readonly int ServerPort;
        #endregion

        #region Constructor
        public ActiveClient(int serverPort)
        {
            ClientInstance = new UnidirectionalClient();
            ServerPort = serverPort;
        }
        #endregion

        #region Method
        public void Start()
        {
            ClientInstance.StartClient(ServerPort);
        }
        public void Send(TSendingType data)
        {
            ClientInstance.Send(data.Serialize());
        }
        public TReceivingType SendAndReceive(TSendingType data)
        {
            ClientInstance.SendAndReceive(data.Serialize(), out byte[] replyData, out int replyLength);
            return (TReceivingType)Activator.CreateInstance<TReceivingType>().Deserialize(replyData, replyLength);
        }
        public void Close()
        {
            ClientInstance.Dispose();
        }
        #endregion
    }
}
