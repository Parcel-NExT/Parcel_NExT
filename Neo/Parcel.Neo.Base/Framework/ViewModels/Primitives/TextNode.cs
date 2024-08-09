using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;

namespace Parcel.Neo.Base.Framework.ViewModels.Primitives
{
    public class TextNode : PrimitiveNode
    {
        public TextNode()
        {
            Title = NodeTypeName = "Text";
            Value = string.Empty; // Remark-cz: Make sure it's not null, so it's more friendly with serialization
        }

        public override OutputConnector MainOutput => ValueOutput as OutputConnector;
    }
}