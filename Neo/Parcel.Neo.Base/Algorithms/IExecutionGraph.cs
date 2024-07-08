using System.Collections.Generic;
using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;

namespace Parcel.Neo.Base.Algorithms
{
    public interface IExecutionGraph
    {
        public void InitializeGraph(IEnumerable<ProcessorNode> targetNodes);
        public void ExecuteGraph();
    }
}