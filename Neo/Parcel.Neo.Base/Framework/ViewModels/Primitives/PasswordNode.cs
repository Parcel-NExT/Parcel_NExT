using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;

namespace Parcel.Neo.Base.Framework.ViewModels.Primitives
{
    public class PasswordNode : PrimitiveNode
    {
        public PasswordNode()
        {
            Title = NodeTypeName = "Password";
        }

        public override OutputConnector MainOutput => ValueOutput as OutputConnector;
    }
}