using Parcel.CoreEngine.Serialization;
using Parcel.CoreEngine.Service.CoreExtensions;

namespace Parcel.CoreEngine.Service.Document
{
    /// <summary>
    /// The operations offered by Ama are equivalent to creating and editing graphs in the GUI
    /// </summary>
    public sealed class DocumentProvisionService : ServiceProvider
    {
        #region Construction
        public DocumentProvisionService() 
        {
            Document = new ParcelDocument();
        }
        #endregion

        #region Document State
        /// <summary>
        /// State of the document
        /// </summary>
        public ParcelDocument Document { get; private set; }
        #endregion

        #region Document Editing
        /// <summary>
        /// Delete all nodes and graphs, reset "Default" graph
        /// </summary>
        public void DeleteAll()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Graph Creation and Editing
        /// <summary>
        /// Create new graph with given name.
        /// </summary>
        public void CreateGraph(string name)
        {
            throw new NotImplementedException();
        }
        public void RemoveGraph(string name)
        {
            throw new NotImplementedException();
        }
        public void RenameGraph(string graph, string newName)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Clear all nodes in a graph
        /// </summary>
        public void ClearGraph(string name)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Node Creation and Editing
        public void CreateNode(string graph, string title, string targetPath)
        {
            throw new NotImplementedException();
        }
        public void UpdateNodeAttribute(string graph, string node, string attribute, string newValue)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Saving and Loading Actions
        public void Save(string filePath)
        {
            Document.Save(filePath);
        }
        public void Load(string filePath)
        {
            Document = GenericSerializer.Deserialize(filePath);
        }
        #endregion
    }
}
