using Parcel.CoreEngine.Document;
using Parcel.CoreEngine.Service.CoreExtensions;

namespace Parcel.CoreEngine.Service.UnitTests
{
    public class BasicProceduralLogicTest
    {
        /// <summary>
        /// This test is not thread safe because Console is globally static per process and xUnit cannot handle parallel testing of functions safely
        /// </summary>
        [Fact]
        public void SimplestDocumentHelloWorldTest()
        {
            // Creates and executes a graph that writes references the final output in a preview node
            ParcelDocument document = CreateGraph();
            document.Execute();
            ParcelNode previewNode = document.MainGraph.MainLayout.Nodes.Single(n => n.Node.Target == "Preview").Node!;
            Assert.Equal(20, (double)document.NodePayloadLookUps[previewNode].PayloadData.First().Value);
        }

        #region Routines
        private ParcelDocument CreateGraph()
        {
            // Set graph variable
            ParcelNode create = new("SetVariable", new Dictionary<string, string>
            {
                { "name", "Test Variable" },
                { "value", "5" },
            });

            // Get graph variable
            ParcelNode readVariable = new("GetVariable", new Dictionary<string, string>
            {
                { "name", "Test Variable" },
            });

            // Perform mathamtical computation
            ParcelNode add = new("Primitives.Number.Add");
            add.Inputs.Add(new ParcelNodeInputDefinition("A", "variable"));
            add.Inputs.Add(new ParcelNodeInputDefinition("B", "15"));

            // Hook up preview node
            ParcelNode preview = new("Preview");
            preview.Inputs.Add(new ParcelNodeInputDefinition("source", "Result.value"));

            // Create document
            ParcelDocument document = new();
            var layout = document.MainGraph.Layouts.First();

            // Add nodes
            ParcelNode[] nodes = [create, readVariable, add, preview];
            for (int i = 0; i < nodes.Length; i++)
            {
                ParcelNode node = nodes[i];
                layout.Nodes.Add(new Layouts.CanvasElement(node)
                {
                    Position = new System.Numerics.Vector2(i * 50, i * 50)
                });
            }

            return document;
        }
        #endregion
    }
}
