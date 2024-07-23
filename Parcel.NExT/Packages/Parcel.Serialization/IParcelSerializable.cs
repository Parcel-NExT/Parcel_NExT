using System.Text;

// TODO: Consider rename namespace to Parcel.CoreEngine.Interfaces
namespace Parcel.Infrastructure
{
    /// <summary>
    /// Represents a Parcel serializable feature.
    /// Parcel serializable features are two fold: They can be represented safely as self-contained texts (not necessarily JSON); 
    /// And they can serialize into binary data
    /// Mostly used by specific Payload objects.
    /// </summary>
    public interface IParcelSerializable
    {
        #region Binary Serialization
        public byte[] Serialize()
        {
            using MemoryStream memory = new();
            using BinaryWriter writer = new(memory, Encoding.UTF8, false);
            WriteToStream(writer, this);
            return memory.ToArray();
        }
        public IParcelSerializable Deserialize(byte[] data, int length, int offset = 0)
        {
            using MemoryStream memory = new(data, offset, length);
            using BinaryReader reader = new(memory, Encoding.UTF8, false);
            return ReadFromStream(reader, out _);
        }
        public IParcelSerializable[] DeserializeBatchMessage(byte[] data, int length, int offset = 0)
        {
            List<IParcelSerializable> entities = [];

            // Remark: Ia network environment, when multiple Send is sent, they may come all together in one single Receive (by the server), so we must implement custom breakdown of such messaging
            long totalConsumed = offset;
            using MemoryStream memory = new(data, offset, length);
            using BinaryReader reader = new(memory, Encoding.UTF8, false);
            while (totalConsumed <= length)
            {
                entities.Add(ReadFromStream(reader, out long consumedBytes));
                totalConsumed += consumedBytes;
            }

            return [.. entities];
        }
        public void WriteToStream(BinaryWriter writer, IParcelSerializable data);
        public IParcelSerializable ReadFromStream(BinaryReader reader, out long consumedBytes);
        #endregion

        #region Textual Serialization
        public string ToText();
        public IParcelSerializable FromText(string text);
        #endregion
    }
}
