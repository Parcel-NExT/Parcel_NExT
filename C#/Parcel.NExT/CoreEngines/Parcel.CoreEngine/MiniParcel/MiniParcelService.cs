using Parcel.CoreEngine.Document;

namespace Parcel.CoreEngine.MiniParcel
{
    public static class MiniParcelService
    {
        #region Interface
        public static ParcelDocument Parse(string scripts)
            => Parse(scripts.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));
        public static ParcelDocument Parse(string[] lines)
        {
            ParcelDocument document = new();

            ParcelGraph graph = document.MainGraph;
            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();
                if (trimmedLine.EndsWith(':'))
                {
                    graph = new ParcelGraph(trimmedLine.TrimEnd(':'));
                    document.Graphs.Add(graph);
                }
                else
                {
                    ParcelNode node = ParseNode(line);
                    document.AddNode(graph, node, new System.Numerics.Vector2());
                }
            }
            return document;
        }
        #endregion

        #region Routines
        private static ParcelNode ParseNode(string line)
        {
            List<string> parts = [];
            // TODO: pending more robust handling and parse quoted strings properly with state machine
            parts = new List<string>(line.Split(' '));

            return new ParcelNode(parts.First(), Enumerable.Range(0, parts.Count - 1).ToDictionary(i => $"Attribute {i}", i => parts[i + 1]));
        }
        #endregion
    }
}
