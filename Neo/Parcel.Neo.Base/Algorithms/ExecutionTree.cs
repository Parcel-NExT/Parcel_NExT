using System.Collections.Generic;
using System.Linq;
using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;

namespace Parcel.Neo.Base.Algorithms
{
    public class ExecutionTree: IExecutionGraph
    {
        #region Internal State
        private List<ExecutionTreeNode> Roots { get; set; } = new List<ExecutionTreeNode>();
        private Dictionary<ProcessorNode, ExecutionTreeNode> Traversed { get; set; } =
            new Dictionary<ProcessorNode, ExecutionTreeNode>();
        #endregion

        #region Interface
        public void InitializeGraph(IEnumerable<ProcessorNode> targetNodes)
        {
            // TODO: Currently we are not able to deal with a single node has multiple inputs
            foreach (ProcessorNode node in targetNodes)
                DraftBranchesForNode(null, node);
        }
        public void ExecuteGraph()
            => Roots.ForEach(ExecuteTreeNode);
        #endregion

        #region Routines
        private void DraftBranchesForNode(ExecutionTreeNode last, ProcessorNode node)
        {
            if (Traversed.ContainsKey(node))
                return;

            ExecutionTreeNode treeNode = new ExecutionTreeNode(node);
            if(last != null) treeNode.AddChild(last);
            Traversed.Add(node, treeNode);
            
            if(!node.Input.Any(i => i.IsConnected))
                Roots.Add(treeNode);
            else
            {
                foreach (BaseNode iter in node.Input.Where(i => i.IsConnected)
                    .Select(i => i.Connections.Single())
                    .Select(c => c.Input.Node))
                {
                    BaseNode input = iter;
                    
                    while (input is KnotNode knot)
                        input = knot.Previous;

                    if(input is ProcessorNode processor)
                        DraftBranchesForNode(treeNode, processor);
                }
            }
        }

        private void ExecuteTreeNode(ExecutionTreeNode node)
        {
            node.Processor.Evaluate();
            
            foreach (ExecutionTreeNode childNode in node.Children)
                ExecuteTreeNode(childNode);
        }
        #endregion
    }

    internal class ExecutionTreeNode
    {
        public ProcessorNode Processor { get; }
        public HashSet<ExecutionTreeNode> Children { get; } = new HashSet<ExecutionTreeNode>();

        public ExecutionTreeNode(ProcessorNode processor)
        {
            Processor = processor;
        }

        public void AddChild(ExecutionTreeNode child)
        {
            Children.Add(child);
        }
    }
}