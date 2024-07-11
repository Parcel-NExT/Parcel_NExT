using System;
using System.Collections.Generic;
using System.Linq;
using Parcel.Neo.Base.Framework;
using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;

namespace Parcel.Neo.Base.Algorithms
{
    public class ExecutionQueue: IExecutionGraph
    {
        #region Internal State
        public List<ProcessorNode> Queue { get; private set; } = [];
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
        private void UpdateNodePosition(ProcessorNode iterationLast, ProcessorNode currentNode)
        {
            if (!currentNode.Input.Any(i => i.IsConnected)) // This is one of the "root" nodes
            {
                if (Queue.Contains(currentNode)) // Remove existing node in queue then re-add it
                    Queue.Remove(currentNode);
                Queue.Insert(0, currentNode);
            }
            else
            {
                if (!Queue.Contains(currentNode))
                {
                    if (iterationLast == null)
                        Queue.Add(currentNode);
                    else
                        Queue.Insert(Queue.IndexOf(iterationLast), currentNode);
                }
                else if (iterationLast != null && Queue.IndexOf(iterationLast) < Queue.IndexOf(currentNode)) // Swap
                {
                    Queue.Remove(currentNode);
                    Queue.Insert(Queue.IndexOf(iterationLast), currentNode);
                }
                
                // Go through the entire chain and update node positions on the chain
                foreach (BaseNode iter in currentNode.Input.Where(i => i.IsConnected)
                    .SelectMany(i => i.Connections)
                    .Select(c => c.Input.Node))
                {
                    BaseNode input = iter;
                    
                    while (input is KnotNode knot)
                        input = knot.Previous;

                    if(input is ProcessorNode processor)
                        UpdateNodePosition(currentNode, processor);
                }
            }
        }
        #endregion
    }
}