using Parcel.CoreEngine.Service.Interpretation;

namespace Parcel.CoreEngine.Service.CoreExtensions
{
    public static class ParcelDocumentExtensions
    {
        public static void Execute(this ParcelDocument document)
        {
            new GraphRuntime(document.MainGraph, document.NodePayloadLookUps).Execute();
        }
    }
}
