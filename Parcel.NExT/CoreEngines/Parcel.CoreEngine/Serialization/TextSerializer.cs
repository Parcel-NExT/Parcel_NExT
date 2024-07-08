using Parcel.CoreEngine.Document;
using Parcel.CoreEngine.Layouts;
using Parcel.CoreEngine.Versioning;

namespace Parcel.CoreEngine.Serialization
{
    public static class TextSerializer
    {
        #region Serialization
        public static void Serialize(ParcelDocument document, string outputFile)
        {
            using StreamWriter writer = new StreamWriter(outputFile);
            
            // File header
            writer.WriteLine(GenericSerializer.ParcelSerializationFormatTextualFormatSymbol);
            writer.WriteLine(GenericSerializer.BannerText);
            WriteAdditionalSoftwareAndFileFormatInformation(writer);
            writer.WriteLine();

            // Document Meta-Data
            writer.WriteLine("# Document Meta-Data");
            writer.WriteLine("Document:");
            // TODO: YAML
            writer.WriteLine();

            // Nodes
            writer.WriteLine("# Nodes Section");
            writer.WriteLine($"Nodes Count: {document.Nodes.Count}");
            foreach (ParcelNode node in document.Nodes)
            {
                string attributesText = string.Join(", ", node.Attributes.Select(a => $"{a.Key}: {a.Value}"));

                // TODO: Below is just placeholder implementation
                writer.WriteLine($" {node.Name} [{node.Target}] {{{attributesText}}}");
            }
            writer.WriteLine();

            // Graphs
            writer.WriteLine("# Graphs Section");
            writer.WriteLine($"Graphs Count: {document.Graphs.Count}");
            writer.WriteLine($"Main Graph: {document.MainGraph.Name}");
            foreach (ParcelGraph graph in document.Graphs)
            {
                writer.WriteLine($"{graph.Name}:");
                foreach (CanvasLayout layout in graph.Layouts)
                {
                    writer.WriteLine($"- {layout.GetType().Name} (x{layout.Placements.Count})");
                    foreach (CanvasElement element in layout.Placements)
                        writer.WriteLine($"  {element.Position} {element.Node.Name}"); // TODO: Placeholder implementation; pending using node id.
                }
            }
            writer.WriteLine();

            // Revisions
            writer.WriteLine("# Graph Revisions");
            writer.WriteLine();
            writer.WriteLine("# Node Revisions");
            writer.WriteLine();

            // Payloads
            writer.WriteLine("# Payloads Section");
            writer.WriteLine();
        }
        #endregion

        #region Deserialization
        public static ParcelDocument Deserialize(string inputFile)
        {
            using StreamReader reader = new StreamReader(inputFile);
            
            // Discard header
            string? _ = null;
            _ = reader.ReadLine();
            _ = reader.ReadLine();

            throw new NotImplementedException();

            return new ParcelDocument();
        }
        #endregion

        #region Subroutine
        private static void WriteAdditionalSoftwareAndFileFormatInformation(StreamWriter writer)
        {
            writer.WriteLine($"Engine Version: {EngineVersion.Version}");
            WritePoem(writer);

            static void WritePoem(StreamWriter writer)
            {
                writer.WriteLine(ChatGPTPoem);
            }
        }
        #endregion

        #region Contents
        public const string ChatGPTPoem =
            """
            # In data's tapestry, numbers dance and weave,
            # Mathematical tools, the threads they retrieve,
            # In their elegant patterns, truths they conceive.
            """;
        #endregion
    }
}
