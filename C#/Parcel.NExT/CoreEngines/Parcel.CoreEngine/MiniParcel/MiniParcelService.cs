using Parcel.CoreEngine.Document;
using Parcel.CoreEngine.Helpers;

namespace Parcel.CoreEngine.MiniParcel
{
    public static class MiniParcelService
    {
        #region Interface
        public static ParcelDocument Parse(string scripts)
            => Parse(scripts.SplitLines(true));
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
            string[] parts = line.SplitCommandLineArguments();
            return new ParcelNode(parts.First(), Enumerable.Range(0, parts.Length - 1).ToDictionary(i => $"${i}", i => parts[i + 1]));
        }
        #endregion
    }
}
