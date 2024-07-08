using System;
using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;

namespace Parcel.Neo.Base.Framework.ViewModels.Primitives
{
    public class DateTimeNode: PrimitiveNode
    {
        #region View Components
        public DateTime DateTime
        {
            get
            {
                if (DateTime.TryParse(_value, out DateTime result))
                    return result;
                return DateTime.Now.Date;
            }
            set => SetField(ref _value, value.ToString());
        }
        #endregion
        
        #region Node Interface
        private readonly OutputConnector _dateTimeOutput = new OutputConnector(typeof(DateTime))
        {
            Title = "DateTime"
        }; 
        public DateTimeNode()
        {
            Title = NodeTypeName = "DateTime";
            DateTime = DateTime.Now.Date;
            ValueOutput.IsHidden = true;
            Output.Add(_dateTimeOutput);
        }
        #endregion

        #region Interface
        public override OutputConnector MainOutput => _dateTimeOutput;
        protected override NodeExecutionResult Execute()
        {
            NodeExecutionResult result = base.Execute();
            result.Caches.Add(_dateTimeOutput, DateTime);
            return result;
        }
        #endregion
    }
}