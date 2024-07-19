using Parcel.CoreEngine.Document;
using Parcel.Types;

namespace Parcel.NExT.NodeGraph
{
    public enum RenderMode
    {
        MiniParcel, // Strongly typed
        NodesAndConnections // Arbitrary nodes
    }
    public class NodeGraphRenderer
    {
        public static Image Render(ParcelGraph graph, RenderMode mode)
        {
            throw new NotImplementedException();
        }
    }
}
