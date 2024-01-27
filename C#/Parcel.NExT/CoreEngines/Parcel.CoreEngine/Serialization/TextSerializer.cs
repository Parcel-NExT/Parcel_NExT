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

            // Graphs
            writer.WriteLine("# Graphs Section");
            writer.WriteLine($"Graphs Count: {document.Graphs.Count}");
            writer.WriteLine($"Main Graph: {document.MainGraph.Name}");
            foreach (ParcelGraph graph in document.Graphs)
            {
                writer.WriteLine($"{graph.Name}:");
                foreach (CanvasLayout layout in graph.Layouts)
                {
                    writer.WriteLine($"- {layout.GetType().Name} (x{layout.Nodes.Count})");
                    foreach (CanvasElement element in layout.Nodes)
                        writer.WriteLine($"  {element.Position} {element.Node.Name}"); // TODO: Placeholder implementation; pending using node id.
                }
            }
            writer.WriteLine();

            // Nodes
            writer.WriteLine("# Nodes Section");
            writer.WriteLine($"Nodes Count: {document.Nodes.Count}");
            foreach(ParcelNode node in document.Nodes)
            {
                string attributesText = string.Join(", ", node.Attributes.Select(a => $"{a.Key}: {a.Value}"));

                // TODO: Below is just placeholder implementation
                writer.WriteLine($" {node.Name} [{node.Target}] {{{attributesText}}}");
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

            return new ParcelDocument();
        }
        #endregion

        #region Subroutine
        private static void WriteAdditionalSoftwareAndFileFormatInformation(StreamWriter writer)
        {
            writer.WriteLine($"Engine Version: {EngineVersion.Version}");
        }
        #endregion
    }
}
