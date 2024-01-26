using Parcel.CoreEngine.Document;

namespace Parcel.CoreEngine
{
    public sealed class ParcelDocument
    {
        #region Constructors
        public ParcelDocument()
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
        public Dictionary<ParcelNode, ParcelPayload> NodePayloadLookUps { get; set; }
        public Dictionary<ParcelGraph, ParcelGraphRuntime> GraphRuntimeLookUps { get; set; }
        #endregion
    }
}
