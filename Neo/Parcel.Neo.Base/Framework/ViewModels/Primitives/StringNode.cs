using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;

namespace Parcel.Neo.Base.Framework.ViewModels.Primitives
{
    public class StringNode: PrimitiveNode
    {
        public StringNode()
        {
            Title = NodeTypeName = "String";
        }

        public override OutputConnector MainOutput => ValueOutput as OutputConnector;
    }
}