using Parcel.Neo.Base.Algorithms;
using Parcel.Neo.Base.Framework.ViewModels;
using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;
using Parcel.Neo.Base.Serialization;

namespace Stewer
{
    internal class Program
    {
        #region Main Entrance
        static void Main(string[] args)
        {
            if (args.Length == 0)
                Console.WriteLine("Nothing to cook!");
            else if (args.FirstOrDefault() == "--help")
                Console.WriteLine("""
                    stew <Graph Path>
                    """);
            else if (File.Exists(args[0]))
            {
                string graphPath = args[0];
                string extension = Path.GetExtension(graphPath);
                if (extension != SerializationHelper.PV1NeoGraphExtension)
                {
                    Console.WriteLine("Unexpected file format.");
                    return;
                }
                else
                    ExecuteFile(graphPath);
            }
        }
        #endregion

        #region Routines
        private static void ExecuteFile(string graphPath)
        {
            // Load
            var canvas = new NodesCanvas();
            canvas.Open(graphPath);

            // TODO: Implement better "active node" selection sceheme
            // Hack: Just put every node to preview
            foreach (var node in canvas.Nodes)
                if (node is ProcessorNode p)
                    p.IsPreview = true;

            // Execute
            AlgorithmHelper.ExecuteGraph(canvas);
        }
        #endregion
    }
}
