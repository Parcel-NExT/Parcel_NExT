using K4os.Compression.LZ4.Streams;
using Parcel.CoreEngine.Document;
using Parcel.CoreEngine.Layouts;
using Parcel.CoreEngine.Versioning;
using System.Numerics;
using System.Text;

namespace Parcel.CoreEngine.Serialization
{
    public static class BinarySerializer
    {
        #region Serialization
        public static void Serialize(ParcelDocument document, string outputFile)
        {
            using LZ4EncoderStream stream = LZ4Stream.Encode(File.Create(outputFile));
            using BinaryWriter writer = new(stream, Encoding.UTF8, false);
            WriteToStream(writer, document);
        }
        #endregion

        #region Deserialization
        public static ParcelDocument Deserialize(string inputFile)
        {
            using LZ4DecoderStream source = LZ4Stream.Decode(File.OpenRead(inputFile));
            using BinaryReader reader = new(source, Encoding.UTF8, false);
            return ReadFromStream(reader);
        }
        #endregion

        #region Routines
        private static void WriteToStream(BinaryWriter writer, ParcelDocument document)
        {
            // File Header
            writer.Write(GenericSerializer.ParcelSerializationFormatBinaryFormatSymbol);
            writer.Write(GenericSerializer.BannerText);
            writer.Write(EngineVersion.Version);
            writer.Write(TextSerializer.ChatGPTPoem);

            // Document Meta-Data
            // ...

            // Nodes
            writer.Write(document.Nodes.Count);
            foreach (ParcelNode node in document.Nodes)
                WriteNode(writer, node);

            // Graphs
            writer.Write(document.Graphs.Count);
            writer.Write(document.MainGraph.Name);
            foreach (ParcelGraph graph in document.Graphs)
                WriteGraph(writer, graph);

            // Revisions
            // ...

            // Payloads
            // ...

            static void WriteNode(BinaryWriter writer, ParcelNode node)
            {
                writer.Write(node.Name);
                writer.Write(node.Target);
                writer.Write(node.Attributes.Count);
                foreach (KeyValuePair<string, string> attribute in node.Attributes)
                {
                    writer.Write(attribute.Key);
                    writer.Write(attribute.Value);
                }
            }
            static void WriteGraph(BinaryWriter writer, ParcelGraph graph)
            {
                writer.Write(graph.Name);
                writer.Write(graph.Layouts.Count);
                foreach (CanvasLayout layout in graph.Layouts)
                    WriteGraphLayout(writer, layout);
            }
            static void WriteGraphLayout(BinaryWriter writer, CanvasLayout layout)
            {
                writer.Write(layout.GetType().Name);
                writer.Write(layout.Placements.Count);
                foreach (CanvasElement element in layout.Placements)
                {
                    writer.Write(element.Position.X);
                    writer.Write(element.Position.Y);
                    writer.Write(element.Node.Name); // TODO: Placeholder implementation; pending using node id.
                }
            }
        }

        private static ParcelDocument ReadFromStream(BinaryReader reader)
        {
            // File Header
            string formatSymbol = reader.ReadString();
            string bannerText = reader.ReadString();
            string version = reader.ReadString();
            string poem = reader.ReadString();

            // Document Meta-Data
            // ...

            // Nodes
            int nodesCount = reader.ReadInt32();
            List<ParcelNode> nodes = new();
            for (int i = 0; i < nodesCount; i++)
                nodes.Add(ReadNode(reader));

            // Graphs
            int graphsCount = reader.ReadInt32();
            string mainGraphName = reader.ReadString();
            List<ParcelGraph> graphs = new();
            for (int i = 0; i < graphsCount; i++)
                graphs.Add(ReadGraph(reader, nodes));

            // Revisions
            // ...

            // Payloads
            // ...

            return new ParcelDocument()
            {
                MainGraph = graphs.Single(g => g.Name == mainGraphName),
                Graphs = graphs,
                Nodes = nodes,
            };

            static ParcelNode ReadNode(BinaryReader reader)
            {
                string nodeName = reader.ReadString();
                string nodeTarget = reader.ReadString();
                int attributesCount = reader.ReadInt32();
                Dictionary<string, string> attributes = new();
                for (int i = 0; i < attributesCount; i++)
                {
                    string key = reader.ReadString();
                    string value = reader.ReadString();
                    attributes[key] = value;
                }
                return new ParcelNode(nodeName, nodeTarget, attributes);
            }
            static ParcelGraph ReadGraph(BinaryReader reader, List<ParcelNode> nodes)
            {
                string graphName = reader.ReadString();
                int layoutsCount = reader.ReadInt32();
                List<CanvasLayout> layouts = new();
                for (int i = 0; i < layoutsCount; i++)
                    layouts.Add(ReadGraphLayout(reader, nodes));
                return new ParcelGraph(graphName)
                {
                    Layouts = layouts,
                    MainLayout = layouts.First()
                };
            }
            static CanvasLayout ReadGraphLayout(BinaryReader reader, List<ParcelNode> nodes)
            {
                string layoutType = reader.ReadString();
                int layoutNodesCount = reader.ReadInt32();
                List<CanvasElement> elements = new();
                for (int i = 0; i < layoutNodesCount; i++)
                {
                    float x = reader.ReadSingle();
                    float y = reader.ReadSingle();
                    string nodeID = reader.ReadString(); // TODO: Placeholder implementation; pending using node id.
                    elements.Add(new CanvasElement(nodes.Single(n => n.Name == nodeID))
                    {
                        Position = new Vector2(x, y)
                    });
                }
                return new CanvasLayout()
                {
                    Placements = elements,
                };
            }
        }
        #endregion
    }
}
