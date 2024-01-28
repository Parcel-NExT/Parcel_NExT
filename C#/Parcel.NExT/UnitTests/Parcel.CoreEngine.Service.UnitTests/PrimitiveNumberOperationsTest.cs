using Parcel.CoreEngine.Document;
using Parcel.CoreEngine.Service.CoreExtensions;

namespace Parcel.CoreEngine.Service.UnitTests
{
    public class PrimitiveNumberOperationsTest
    {
        /// <summary>
        /// This test is not thread safe because Console is globally static per process and xUnit cannot handle parallel testing of functions safely
        /// </summary>
        [Fact]
        public void PrimitiveNumberOperationsShouldWork()
        {
            // Creates and executes a graph that just computes something trivial
            ParcelDocument document = CreateGraph();
            document.Execute();
            ParcelNode previewNode = document.MainGraph.MainLayout.Placements.Single(n => n.Node.Target == "{Preview}").Node!;
            string? value = ParcelNodeUnifiedAttributesHelper.GetFromUnifiedAttribute(previewNode, document.NodePayloadLookUps[previewNode], "value");
            Assert.Equal(5 + 12 + 15, double.Parse(value));
        }

        #region Routines
        private ParcelDocument CreateGraph()
        {
            // Perform mathamtical computation
            ParcelNode n1 = new("Result", "Parcel.CoreEngine.Primitives.Number.Add");
            n1.Inputs.Add(new ParcelNodeInputDefinition("A", ":5"));
            n1.Inputs.Add(new ParcelNodeInputDefinition("B", ":12"));
            n1.Inputs.Add(new ParcelNodeInputDefinition("C", ":15"));

            // Hook up preview node
            ParcelNode n2 = new("{Preview}");
            n2.Inputs.Add(new ParcelNodeInputDefinition("source", "@Result.value"));

            // Create document
            ParcelDocument document = new();
            var graph = document.MainGraph;

            // Add nodes
            ParcelNode[] nodes = [n1, n2];
            for (int i = 0; i < nodes.Length; i++)
                document.AddNode(graph, nodes[i], new System.Numerics.Vector2(i * 50, i * 50));

            return document;
        }
        #endregion
    }
}
