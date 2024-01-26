using Parcel.CoreEngine.Document;
using Parcel.CoreEngine.Layouts;
using System.Numerics;

namespace Parcel.CoreEngine
{
    public sealed class ParcelDocument: ParcelDocumentBase
    {
        #region Management Properties
        public Dictionary<ParcelNode, ParcelPayload> NodePayloadLookUps { get; set; } = new();
        public Dictionary<ParcelNode, ParcelGraph> NodeGraph { get; set; } = new();
        public Dictionary<ParcelGraph, ParcelGraphRuntime> GraphRuntimeLookUps { get; set; } = new();
        #endregion

        #region Construction API
        public void AddNode(ParcelGraph graph, ParcelNode node, Vector2 position)
        {
            graph.MainLayout.Nodes.Add(new CanvasElement(node)
            {
                Position = position
            });
            NodeGraph.Add(node, graph);
        }
        #endregion
    }
    public abstract class ParcelDocumentBase
    {
        #region Constructors
        public ParcelDocumentBase()
        {
            MainGraph = new ();
            Graphs.Add(MainGraph);
        }
        #endregion

        #region Static Sections
        public List<ParcelGraph> Graphs = new();
        public List<ParcelNode> Nodes = new();
        #endregion

        #region Version Control Sections

        #endregion

        #region Runtime Data Sections
        public List<ParcelGraphRuntime> GraphRuntimes = new();
        public List<ParcelPayload> Payloads = new();
        #endregion

        #region Document Properties
        public ParcelGraph MainGraph { get; set; }
        #endregion
    }
}
