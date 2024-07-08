using Parcel.CoreEngine.Document;
using Parcel.CoreEngine.Service.CoreExtensions;

namespace Parcel.CoreEngine.Service.UnitTests.HelloWorld
{
    public class HelloWorldTest
    {
        /// <summary>
        /// This test is not thread safe because Console is globally static per process and xUnit cannot handle parallel testing of functions safely
        /// </summary>
        [Fact]
        public void SimplestDocumentHelloWorldTest()
        {
            TextWriter originalConsoleOut = Console.Out;
            using (StringWriter writer = new())
            {
                Console.SetOut(writer);

                // Creates and executes a graph that writes "Hello World!" to console
                ParcelDocument graph = CreateGraph();
                graph.Execute();

                writer.Flush();
                string outputString = writer.GetStringBuilder().ToString();
                Assert.Equal("Hello World!", outputString.TrimEnd());
            }

            Console.SetOut(originalConsoleOut);
        }

        #region Routines
        private ParcelDocument CreateGraph()
        {
            var node = new ParcelNode("System.Console.dll:System.Console.WriteLine");
            node.Attributes.Add("value", "Hello World!");

            ParcelDocument document = new();
            document.MainGraph.Layouts.First().Placements.Add(new Layouts.CanvasElement(node));
            return document;
        }
        #endregion
    }
}