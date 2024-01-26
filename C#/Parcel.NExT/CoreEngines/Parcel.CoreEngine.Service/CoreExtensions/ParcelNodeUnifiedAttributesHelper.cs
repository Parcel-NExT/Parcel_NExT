using Parcel.CoreEngine.Document;

namespace Parcel.CoreEngine.Service.CoreExtensions
{
    public static class ParcelNodeUnifiedAttributesHelper
    {
        public static string? GetFromUnifiedAttribute(ParcelNode node, ParcelPayload payload, string attributeName)
        {
            if (node.Attributes.ContainsKey(attributeName))
            {
                return node.Attributes[attributeName].ToString();
            }
            else if (payload.PayloadData.ContainsKey(attributeName))
            {
                return payload.PayloadData[attributeName].ToString();
            }
            return null;
        }
        public static string? GetFromUnifiedAttribute(Dictionary<ParcelNode, ParcelPayload> payloads, ParcelNode[] searchNodes, string nodeAttributeReferencePath)
        {
            int splitterIndex = nodeAttributeReferencePath.LastIndexOf('.');
            ParcelNode node = DereferenceNode(searchNodes, nodeAttributeReferencePath.Substring(0, splitterIndex));
            return GetFromUnifiedAttribute(node, payloads[node], nodeAttributeReferencePath.Substring(splitterIndex + 1));
        }

        public static ParcelNode DereferenceNode(ParcelNode[] searchNodes, string nodeName)
        {
            return searchNodes.Single(n => n.Name == nodeName);
        }
    }
}
