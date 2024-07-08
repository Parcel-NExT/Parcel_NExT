using System;
using Parcel.Neo.Base.Framework.ViewModels;
using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;

namespace Parcel.Neo.Base.Serialization
{
    internal class NodeGraphData
    { 
        #region Graph Metadata
        public string Version { get; set; }
        
        public string Title { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        
        public DateTime CreationTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public int Revision { get; set; }
        #endregion
        
        #region Nodes Data
        public NodeData[] Nodes { get; set; }
        public ConnectionData[] Connections { get; set; }
        #endregion
    }

    internal class NodeData
    {
        /// <summary>
        /// Full name of corresponding type
        /// </summary>
        public string NodeType { get; set; }
        public (string Key, byte[] Value)[] NodeMembers { get; set; }

        public BaseNode Deserialize(NodesCanvas canvas)
        {
            BaseNode node = (BaseNode)Activator.CreateInstance(Type.GetType(NodeType));
            if (node != null)
            {
                node.Deserialize(NodeMembers, canvas);
                node.PostDeserialization();
                return node;
            }

            return null;
        }
    }

    internal class ConnectionData
    {
        public int SourceNodeIndex { get; set; }
        public int SourcePin { get; set; }
        public int DestinationNodeIndex { get; set; }
        public int DestinationPin { get; set; }
    }
}