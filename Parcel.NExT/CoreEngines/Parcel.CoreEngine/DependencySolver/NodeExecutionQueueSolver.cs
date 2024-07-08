using Parcel.CoreEngine.Contracts;
using Parcel.CoreEngine.Document;
using Parcel.CoreEngine.Standardization;

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

    /// <summary>
    /// The execution queue is for solving dependancy for functional nodes
    /// </summary>
    public class ExecutionQueue
    {
        #region Internal State
        private List<ParcelNode> _Queue = [];
        #endregion

        #region Constructor
        public Dictionary<string, ParcelNode> NameLookUp { get; }
        public ExecutionQueue(Dictionary<string, ParcelNode> lookup)
        {
            NameLookUp = lookup;
        }
        #endregion

        #region Interface
        public void InitializeGraph(IEnumerable<ParcelNode> targetNodes)
        {
            foreach (ParcelNode processorNode in targetNodes)
                UpdateNodePosition(null, processorNode);
        }
        #endregion

        #region Routines
        private void UpdateNodePosition(ParcelNode? last, ParcelNode node)
        {
            if (!node.HasInputs())
                _Queue.Insert(0, node);
            else
            {
                if (!_Queue.Contains(node))
                {
                    if (last == null)
                        _Queue.Add(node);
                    else
                        _Queue.Insert(_Queue.IndexOf(last), node);
                }
                else if (last != null)
                {
                    _Queue.Remove(node);
                    _Queue.Insert(_Queue.IndexOf(last), node);
                }

                foreach (ParcelNode iter in node.GetInputnames()
                    .Select(n => NameLookUp[n]))
                {
                    ParcelNode input = iter;

                    while (input.Target == SystemNodes.KnotNodeTarget)
                        input = NameLookUp[input.Attributes.Single().Value.TrimStart('@')];

                    if (input.IsProcessorNode())
                        UpdateNodePosition(node, input);
                }
            }
        }
        #endregion
    }
}
