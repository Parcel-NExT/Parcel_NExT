using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;

namespace Parcel.Neo.Base.Framework.ViewModels.Primitives
{
    public class NumberNode: PrimitiveNode
    {
        #region View Components
        public double Number
        {
            get => double.Parse(_value);
            set => SetField(ref _value, value.ToString());
        }
        #endregion
        
        #region Node Interface
        private readonly OutputConnector _numberOutput = new(typeof(double))
        {
            Title = "Number"
        }; 
        public NumberNode()
        {
            Title = NodeTypeName = "Number";
            Number = 0;
            ValueOutput.IsHidden = true;
            Output.Add(_numberOutput);
        }
        #endregion

        #region Interface
        public override OutputConnector MainOutput => _numberOutput;
        protected override NodeExecutionResult Execute()
        {
            NodeExecutionResult result = base.Execute();
            result.Caches.Add(_numberOutput, Number);
            return result;
        }
        #endregion
    }
}