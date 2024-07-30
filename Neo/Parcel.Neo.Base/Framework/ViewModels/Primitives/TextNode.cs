using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;

namespace Parcel.Neo.Base.Framework.ViewModels.Primitives
{
    public class TextNode : PrimitiveNode
    {
        public TextNode()
        {
            Title = NodeTypeName = "Text";
        }

        public override OutputConnector MainOutput => ValueOutput as OutputConnector;
    }
}