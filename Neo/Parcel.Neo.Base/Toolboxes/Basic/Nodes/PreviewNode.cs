using System.Collections.Generic;
using Parcel.Neo.Base.Framework;
using Parcel.Neo.Base.Framework.ViewModels;
using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;

namespace Parcel.Neo.Base.Toolboxes.Basic.Nodes
{
    public class PreviewNode : ProcessorNode
    {
        #region Node Interface
        private readonly InputConnector _objectInput = new(typeof(object))
        {
            Title = "Object",
        };
        private readonly OutputConnector _objectOutput = new(typeof(object))
        {
            Title = "Object",
        };
        public PreviewNode()
        {
            Title = NodeTypeName = "Preview";
            Input.Add(_objectInput);
            Output.Add(_objectOutput);
        }
        #endregion

        #region Processor Interface
        protected override NodeExecutionResult Execute()
        {
            object obj = _objectInput.FetchInputValue<object>();

            return new NodeExecutionResult(new NodeMessage(obj.ToString()), new Dictionary<OutputConnector, object>()
            {
                {_objectOutput, new ConnectorCache(obj)}
            });
        }
        #endregion

        #region Serialization
        protected override Dictionary<string, NodeSerializationRoutine> ProcessorNodeMemberSerialization { get; } = null;
        protected override NodeSerializationRoutine VariantInputConnectorsSerialization { get; } = null;
        #endregion
    }
}