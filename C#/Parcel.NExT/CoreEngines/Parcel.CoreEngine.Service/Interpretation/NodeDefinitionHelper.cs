using Parcel.CoreEngine.Document;

namespace Parcel.CoreEngine.Service.Interpretation
{
    public static class NodeDefinitionHelper
    {
        public static string[] SimpleExtractParameters(ParcelNode node)
        {
            return [.. node.Attributes.Values, .. node.Inputs.Select(i => i.Source)];
        }
    }
}
