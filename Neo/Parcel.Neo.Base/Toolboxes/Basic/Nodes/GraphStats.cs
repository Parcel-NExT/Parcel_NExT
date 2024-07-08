using System.Collections.Generic;
using Parcel.Neo.Base.Framework;
using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;

namespace Parcel.Neo.Base.Toolboxes.Basic.Nodes
{
    /// <summary>
    /// The property panel of this can have special referencing to Graph-level setting
    /// </summary>
    public class GraphStats : ProcessorNode
    {
        #region Node Interface
        public GraphStats()
        {
            Title = NodeTypeName = "Graph Stats";
        }
        #endregion

        #region Processor Interface
        protected override NodeExecutionResult Execute()
        {
            return new NodeExecutionResult(new NodeMessage(string.Empty), null);
        }
        #endregion

        #region Serialization

        protected override Dictionary<string, NodeSerializationRoutine> ProcessorNodeMemberSerialization { get; } =
            null;
        protected override NodeSerializationRoutine VariantInputConnectorsSerialization { get; } = null;
        #endregion
    }
}