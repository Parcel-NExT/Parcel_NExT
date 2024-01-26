namespace Parcel.CoreEngine.Document
{
    public sealed class ParcelPayload
    {
        public ParcelPayload(ParcelNode node, Dictionary<string, object> data) 
        {
            Node = node;
            PayloadData = data ?? [];
        }

        public ParcelNode Node;
        public Dictionary<string, object> PayloadData { get; }
    }
}
