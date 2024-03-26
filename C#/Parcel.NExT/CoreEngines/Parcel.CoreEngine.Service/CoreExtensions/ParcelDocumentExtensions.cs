using Parcel.CoreEngine.Serialization;
using Parcel.CoreEngine.Service.Interpretation;

namespace Parcel.CoreEngine.Service.CoreExtensions
{
    public static class ParcelDocumentExtensions
    {
        #region Runtime
        public static void Execute(this ParcelDocument document)
        {
            new GraphRuntime(document.MainGraph, document.NodePayloads).Execute();
        }
        #endregion

        #region Serialization
        public static void Save(this ParcelDocument document, string outputPath)
        {
            GenericSerializer.Serialize(document, outputPath);
        }
        #endregion
    }
}
