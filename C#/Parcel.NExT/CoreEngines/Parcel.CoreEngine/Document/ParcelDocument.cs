using Parcel.CoreEngine.Document;

namespace Parcel.CoreEngine
{
    public class ParcelDocument
    {
        #region Constructors
        public ParcelDocument()
        {
            MainGraph = new ();
            Graphs.Add(MainGraph);
        }
        #endregion

        #region Sections
        public List<ParcelGraph> Graphs = new();
        public List<ParcelNode> Nodes = new();
        #endregion

        #region Document Properties
        public ParcelGraph MainGraph { get; set; }
        #endregion
    }
}
