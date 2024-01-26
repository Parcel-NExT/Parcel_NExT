using Parcel.CoreEngine.Service.Interpretation;

namespace Parcel.CoreEngine.Service.CoreExtensions
{
    public static class ParcelDocumentExtensions
    {
        public static void Execute(this ParcelDocument document)
        {
            Layouts.CanvasLayout mainLayout = document.MainGraph.Layouts.First();
            foreach (Document.ParcelNode node in mainLayout.Nodes.Select(n => n.Node))
            {
                string[] protocol = node.Target.Split(':');
                string assemblyPath = protocol[0];
                string functionPath = protocol[1];

                string[] arguments = NodeDefinitionHelper.SimpleExtractParameters(node);
                InvokationService.InvokeRemoteFunction(assemblyPath, functionPath, arguments);
            }
        }
    }
}
