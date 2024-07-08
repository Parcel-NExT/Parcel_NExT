using Parcel.CoreEngine.Document;
using Parcel.CoreEngine.Helpers;

namespace Parcel.CoreEngine.Service.CoreExtensions
{
    public static class ParcelNodeExtensions
    {
        public static void AddInput(this ParcelNode node, string name, string value)
        {
            if (!name.StartsWith(NodeAttributeNameHelper.InputAttributeLeadingSymbol))
                name = NodeAttributeNameHelper.InputAttributeLeadingSymbol + name;

            node.Attributes[name] = value;
        }
    }
}
