using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using K4os.Compression.LZ4.Streams;
using Parcel.Neo.Base.Framework.ViewModels;
using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;

namespace Parcel.Neo.Base.Serialization
{
    internal class GraphSerializer
    {
        #region Interface
        public static void Serialize(string filePath, CanvasSerialization canvas)
        {
            // Book-keeping structures
            Dictionary<BaseNode, NodeData> nodeMapping = [];

            // Serialize nodes
            NodeData[] nodes = canvas.Nodes.Select(n =>
            {
                NodeData serialized = n.Serialize();
                nodeMapping[n] = serialized;
                return serialized;
            }).ToArray();
            Dictionary<NodeData, int> nodeIndices = nodes
                .Select((n, i) => (n, i))
                .ToDictionary(p => p.n, p => p.i);
            
            // Serialize connections
            ConnectionData[] connections = canvas.Connections.Select(c =>
            {
                return new ConnectionData()
                {
                    SourceNodeIndex = nodeIndices[nodeMapping[c.Input.Node]],
                    SourcePin = c.Input.Node.GetOutputPinID(c.Input as OutputConnector),
                    DestinationNodeIndex = nodeIndices[nodeMapping[c.Output.Node]],
                    DestinationPin = c.Output.Node.GetInputPinID(c.Output as InputConnector)
                };
            }).ToArray();
            
            // Serialize
            NodeGraphData graph = new()
            {
                Nodes = nodes,
                Connections = connections
            };
            using LZ4EncoderStream stream = LZ4Stream.Encode(File.Create(filePath));
            using BinaryWriter writer = new(stream, Encoding.UTF8, false);
            WriteToStream(writer, graph);
        }
        public static CanvasSerialization Deserialize(string filePath, NodesCanvas canvas)
        {
            // Load raw graph data
            using LZ4DecoderStream stream = LZ4Stream.Decode(File.OpenRead(filePath));
            using BinaryReader reader = new(stream, Encoding.UTF8, false);
            NodeGraphData graph = ReadFromStream(reader);
            
            // Book-keeping structures
            Dictionary<NodeData, BaseNode> nodeMapping = [];
            
            // Deserialize nodes
            List<BaseNode> nodes = graph.Nodes.Select(n =>
            {
                BaseNode deserialized = n.Deserialize(canvas);
                nodeMapping[n] = deserialized;
                return deserialized;
            }).ToList();
            // Deserialize connections
            List<BaseConnection> connections = graph.Connections.Select(c =>
            {
                return new BaseConnection()
                {
                    Graph = canvas,
                    Input = nodeMapping[graph.Nodes[c.SourceNodeIndex]].GetOutputPin(c.SourcePin),
                    Output = nodeMapping[graph.Nodes[c.DestinationNodeIndex]].GetInputPin(c.DestinationPin)
                };
            }).ToList();
            
            // Reconstruct canvas
            CanvasSerialization loaded = new()
            {
                Nodes = nodes,
                Connections = connections
            };
            return loaded;
        }
        #endregion

        #region Binary Serialization
        private static void WriteToStream(BinaryWriter writer, NodeGraphData graph)
        {
            // Remark: Subgraphs not implemented

            // Runtime metadata
            writer.Write(graph.RuntimeMetadata.EditorName);
            writer.Write(graph.RuntimeMetadata.EditorVersion);
            writer.Write(graph.RuntimeMetadata.EditorDescription);
            writer.Write(graph.RuntimeMetadata.FileformatIdentifier);
            writer.Write(graph.RuntimeMetadata.FileformatVersion);
            writer.Write(graph.RuntimeMetadata.Remark);

            // Graph Metadata
            writer.Write(graph.Title);
            writer.Write(graph.Author);
            writer.Write(graph.Description);
            writer.Write(graph.CreationTime.ToString("yyyy-MM-dd"));
            writer.Write(graph.UpdateTime.ToString("yyyy-MM-dd"));
            writer.Write(graph.Revision);

            // Nodes
            writer.Write(graph.Nodes.Length);
            foreach (NodeData node in graph.Nodes)
            {
                writer.Write(node.NodeType);
                writer.Write(node.NodeMembers.Length);
                foreach ((string Key, byte[] Value) in node.NodeMembers)
                {
                    writer.Write(Key);
                    writer.Write(Value.Length);
                    writer.Write(Value);
                }
            }

            // Connections
            writer.Write(graph.Connections.Length);
            foreach (ConnectionData connection in graph.Connections)
            {
                writer.Write(connection.SourceNodeIndex);
                writer.Write(connection.SourcePin);
                writer.Write(connection.DestinationNodeIndex);
                writer.Write(connection.DestinationPin);
            }
        }
        private static NodeGraphData ReadFromStream(BinaryReader reader)
        {
            // Runtime metadata
            RuntimeMetadata runtimeMetadata = new()
            {
                EditorName = reader.ReadString(),
                EditorVersion = reader.ReadString(),
                EditorDescription = reader.ReadString(),
                FileformatIdentifier = reader.ReadString(),
                FileformatVersion = reader.ReadString(),
                Remark = reader.ReadString()
            };

            // Initialize graph
            NodeGraphData graph = new()
            {
                RuntimeMetadata = runtimeMetadata,

                // Graph Metadata
                Title = reader.ReadString(),
                Author = reader.ReadString(),
                Description = reader.ReadString(),
                CreationTime = DateTime.Parse(reader.ReadString()),
                UpdateTime = DateTime.Parse(reader.ReadString()),
                Revision = reader.ReadInt32()
            };

            // Nodes
            int nodesLength = reader.ReadInt32();
            graph.Nodes = new NodeData[nodesLength];
            for (int i = 0; i < nodesLength; i++)
            {
                graph.Nodes[i] = new NodeData()
                {
                    NodeType = reader.ReadString(),
                };
                int membersCount = reader.ReadInt32();
                graph.Nodes[i].NodeMembers = new (string Key, byte[] Value)[membersCount];
                for (int m = 0; m < membersCount; m++)
                {
                    string key = reader.ReadString();
                    int bufferSize = reader.ReadInt32();
                    byte[] value = reader.ReadBytes(bufferSize);
                    graph.Nodes[i].NodeMembers[m] = (key, value);
                }
            }

            // Connections
            int connectionsLength = reader.ReadInt32();
            graph.Connections = new ConnectionData[connectionsLength];
            for (int i = 0; i < connectionsLength; i++)
            {
                graph.Connections[i] = new ConnectionData()
                {
                    SourceNodeIndex = reader.ReadInt32(),
                    SourcePin = reader.ReadInt32(),
                    DestinationNodeIndex = reader.ReadInt32(),
                    DestinationPin = reader.ReadInt32()
                };
            }

            return graph;
        }
        #endregion
    }
}