using System;
using System.Collections.Generic;
using Parcel.Neo.Base.Framework;
using Parcel.Neo.Base.Framework.ViewModels;
using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;

namespace Parcel.Neo.Base.Toolboxes.Basic.Nodes
{
    /// <summary>
    /// Provides a dedicated routing (compared to default main output and on-canvas preview of contents.
    /// </summary>
    /// <remarks>
    /// TODO: @intern Revamp Preview node to provide on-surface preview instead of summoning a result window
    /// </remarks>
    public class PreviewNode : ProcessorNode
    {
        #region Node Interface
        public readonly InputConnector ObjectInput = new(typeof(object))
        {
            Title = string.Empty, // Make it empty to keep node surface more efficient
        };
        public PreviewNode()
        {
            Title = NodeTypeName = "Preview";
            Input.Add(ObjectInput);
        }
        public Action? OnExecute;
        #endregion

        #region Processor Interface
        protected override NodeExecutionResult Execute()
        {
            object obj = ObjectInput.FetchInputValue<object>();
            OnExecute?.Invoke();
            return new NodeExecutionResult(new NodeMessage(obj.ToString()), []);
        }
        #endregion

        #region Serialization
        protected override Dictionary<string, NodeSerializationRoutine> ProcessorNodeMemberSerialization { get; } = null;
        protected override NodeSerializationRoutine VariantInputConnectorsSerialization { get; } = null;
        #endregion
    }
}