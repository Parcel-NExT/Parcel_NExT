using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;

namespace Parcel.Neo.Base.Framework.ViewModels.Primitives
{
    public class BooleanNode: PrimitiveNode
    {
        #region View Components
        public bool Boolean
        {
            get
            {
                if (bool.TryParse(_value, out bool result))
                    return result;
                return false;
            }
            set => SetField(ref _value, value.ToString());
        }
        #endregion
        
        #region Node Interface
        private readonly OutputConnector _truthOutput = new OutputConnector(typeof(bool))
        {
            Title = "Truth"
        }; 
        public BooleanNode()
        {
            Title = NodeTypeName = "Boolean";
            Boolean = false;
            ValueOutput.IsHidden = true;
            Output.Add(_truthOutput);
        }
        #endregion

        #region Interface
        public override OutputConnector MainOutput => _truthOutput as OutputConnector;

        protected override NodeExecutionResult Execute()
        {
            NodeExecutionResult result = base.Execute();
            result.Caches.Add(_truthOutput, Boolean);
            return result;
        }
        #endregion
    }
}