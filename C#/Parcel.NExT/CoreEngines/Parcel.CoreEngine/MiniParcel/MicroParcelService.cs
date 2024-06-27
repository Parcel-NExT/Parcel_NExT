using Parcel.CoreEngine.Document;

namespace Parcel.CoreEngine.MiniParcel
{
    public static class MicroParcelService
    {
        #region Main
        public static ParcelDocument Parse(string microParcel)
        {
            string[] lines = microParcel
                .Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(line => !line.TrimStart().StartsWith('#')) // Remove comment lines
                .Where(line => !string.IsNullOrWhiteSpace(line)) // Remove empty lines
                .ToArray();

            // Parinsg states
            Dictionary<string, ParcelNode> nodes = [];
            ParcelGraph currentGraph = new("Default");
            ParcelNode? node = null;
            Dictionary<string, string>? nodeAttributes = null;
            ParcelDocument document = new()
            {
                MainGraph = currentGraph,
                Graphs = [currentGraph]
            };
            foreach (string line in lines)
            {
                // Get parts
                int breaker = line.IndexOf(':');
                string name = line.Substring(0, breaker).Trim();
                string value = line.Substring(breaker + 1).Trim();

                // New graph
                if (!line.Contains(':'))
                {
                    currentGraph = new ParcelGraph(line.Trim());
                    document.Graphs.Add(currentGraph);
                }
                // New node
                if (!char.IsWhiteSpace(line.First()))
                {
                    nodeAttributes = [];
                    node = new ParcelNode(name, value, nodeAttributes);
                    nodes.Add(name, node);
                    document.AddNode(currentGraph, node);
                }
                // New attribute
                else
                    nodeAttributes!.Add(name, value);
            }

            return document;
        }
        #endregion
    }
}
