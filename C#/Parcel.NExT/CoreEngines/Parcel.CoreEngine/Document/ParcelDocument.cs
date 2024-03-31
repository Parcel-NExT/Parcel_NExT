using Parcel.CoreEngine.Document;
using Parcel.CoreEngine.Helpers;
using Parcel.CoreEngine.Layouts;
using System.Numerics;

namespace Parcel.CoreEngine
{
    public sealed class ParcelDocument: ParcelDocumentBase
    {
        #region Management Properties
        public Dictionary<ParcelNode, ParcelGraph> NodeGraph { get; } = [];
        public Dictionary<ParcelGraph, ParcelGraphRuntime> GraphRuntimeLookUps { get; } = [];
        #endregion

        #region Construction API
        public long AddNode(ParcelGraph graph, ParcelNode node, Vector2 position)
        {
            // Add to layout
            graph.MainLayout.Placements.Add(new CanvasElement(node)
            {
                Position = position
            });

            // Add to main data
            Nodes.Add(node);

            // Add to runtime bookkeeeping
            long nodeID = DocumentGUIDCounter;
            DocumentGUIDCounter++;
            NodeGraph.Add(node, graph);
            NodeGUIDs.Add(node, nodeID);

            return nodeID;
        }
        #endregion
    }
    public abstract class ParcelDocumentBase
    {
        #region Constructors
        public ParcelDocumentBase()
        {
            MainGraph = new("Default");
            Graphs.Add(MainGraph);
        }
        #endregion

        #region Document-Wise Properties
        public long DocumentGUIDCounter { get; set; } = 0;
        public TwoWayDictionary<ParcelNode, long> NodeGUIDs { get; set; } = [];
        #endregion

        #region Meta-Data
        public DateTime CreationTime { get; set; } = DateTime.Now;
        public DateTime LastModificationTime { get; set; }
        #endregion

        #region Static Sections
        public List<ParcelGraph> Graphs = new();
        public List<ParcelNode> Nodes = new();
        #endregion

        #region Version Control Sections

        #endregion

        #region Runtime Data Sections
        public List<ParcelGraphRuntime> GraphRuntimes = new();
        public Dictionary<ParcelNode, ParcelPayload> NodePayloads = [];
        #endregion

        #region Document Properties
        public ParcelGraph MainGraph { get; set; }
        #endregion
    }
}
