using Parcel.Neo.Base.Serialization;
using System.Collections.Generic;

namespace Parcel.Neo.Base.Framework.ViewModels.BaseNodes
{
    public abstract class PrimitiveNode: ProcessorNode
    {
        #region Public View Properties
        protected string _value;
        public string Value
        {
            get => _value;
            set => SetField(ref _value, value);
        }
        #endregion

        #region Node Interface
        protected readonly OutputConnector ValueOutput = new OutputConnector(typeof(string))
        {
            Title = "Value"
        };
        protected PrimitiveNode()
        {
            ProcessorNodeMemberSerialization = new Dictionary<string, NodeSerializationRoutine>()
            {
                {nameof(Value), new NodeSerializationRoutine( () => SerializationHelper.Serialize(_value), value => _value = SerializationHelper.GetString(value))}
            };
            
            Output.Add(ValueOutput);
        }
        #endregion
        
        #region Processor Interface
        protected override NodeExecutionResult Execute()
        {
            return new NodeExecutionResult(null, new Dictionary<OutputConnector, object>()
            {
                {ValueOutput, _value}                
            });
        }
        #endregion

        #region Serialization
        protected override Dictionary<string, NodeSerializationRoutine> ProcessorNodeMemberSerialization { get; }
        protected override NodeSerializationRoutine VariantInputConnectorsSerialization { get; } = null;
        #endregion
    }
}