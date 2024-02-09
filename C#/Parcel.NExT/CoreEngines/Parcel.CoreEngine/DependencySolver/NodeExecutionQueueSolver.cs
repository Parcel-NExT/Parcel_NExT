using Parcel.CoreEngine.Document;

namespace Parcel.CoreEngine.DependencySolver
{
    /// <summary>
    /// Given abstract connection information and node type (functional vs procedural), this solver resolves and outputs a list of nodes to be executed in the correct order.
    /// This class takes in ParcelDocument for simplicity, but internally it doesn't depend on such structure should be able to take in any abstract connection information.
    /// </summary>
    public class NodeExecutionQueueSolver
    {
        #region Internalized Abstraction Structure

        #endregion

        #region Construction

        #endregion

        #region Solver
        public List<ParcelNode> Solve(ParcelDocument document, ParcelGraph mainGraph)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    public class ExecutionQueue
    {
        #region Internal State
        private List<ProcessorNode> Queue { get; set; } = new List<ProcessorNode>();
        #endregion

        #region Interface
        public void InitializeGraph(IEnumerable<ProcessorNode> targetNodes)
        {
            foreach (ProcessorNode processorNode in targetNodes)
                UpdateNodePosition(null, processorNode);
        }

        public void ExecuteGraph()
        {
            foreach (ProcessorNode node in Queue)
            {
                try
                {
                    node.Evaluate();
                }
                catch (Exception e)
                {
                    node.Message.Content = e.Message;
                    node.Message.Type = NodeMessageType.Error;
                    break;
                }
            }
        }
        #endregion

        #region Routines
        private void UpdateNodePosition(ProcessorNode last, ProcessorNode node)
        {
            if (!node.Input.Any(i => i.IsConnected))
                Queue.Insert(0, node);
            else
            {
                if (!Queue.Contains(node))
                {
                    if (last == null)
                        Queue.Add(node);
                    else
                        Queue.Insert(Queue.IndexOf(last), node);
                }
                else if (last != null)
                {
                    Queue.Remove(node);
                    Queue.Insert(Queue.IndexOf(last), node);
                }

                foreach (BaseNode iter in node.Input.Where(i => i.IsConnected)
                    .Select(i => i.Connections.Single())
                    .Select(c => c.Input.Node))
                {
                    BaseNode input = iter;

                    while (input is KnotNode knot)
                        input = knot.Previous;

                    if (input is ProcessorNode processor)
                        UpdateNodePosition(node, processor);
                }
            }
        }
        #endregion
    }
}
