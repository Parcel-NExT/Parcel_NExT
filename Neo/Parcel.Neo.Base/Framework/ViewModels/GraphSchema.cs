using System.Collections.Generic;
using System.Linq;
using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;
using Parcel.Neo.Base.DataTypes;

namespace Parcel.Neo.Base.Framework.ViewModels
{
    public class GraphSchema
    {
        #region Add Connection
        public bool CanAddConnection(BaseConnector source, object target)
        {
            if (target is BaseConnector con)
            {
                return source != con
                    && source.Node != con.Node
                    && source.Node.Graph == con.Node.Graph
                    && source.Shape == con.Shape
                    && source.AllowsNewConnections()
                    && con.AllowsNewConnections()
                    && (source.FlowType != con.FlowType || con.Node is KnotNode)
                    && !source.IsConnectedTo(con);
            }
            else if (source.AllowsNewConnections() && target is ProcessorNode node)
            {
                IEnumerable<BaseConnector> allConnectors = source.FlowType == ConnectorFlowType.Input ? node.Output.Cast<BaseConnector>() : node.Input;
                return allConnectors.Any(c => c.AllowsNewConnections());
            }

            return false;
        }

        public bool TryAddConnection(BaseConnector source, object target)
        {
            if (target != null && CanAddConnection(source, target))
            {
                if (target is BaseConnector connector)
                {
                    AddConnection(source, connector);
                    return true;
                }
                else if (target is ProcessorNode node)
                {
                    AddConnection(source, node);
                    return true;
                }
            }

            return false;
        }

        private void AddConnection(BaseConnector connector1, BaseConnector connector2)
        {
            bool shouldReverse = !(connector1.FlowType == ConnectorFlowType.Output ||
                                   connector1.FlowType == ConnectorFlowType.Knot);
            var newConnection = new BaseConnection()
            {
                Input =  shouldReverse ? connector2 : connector1,
                Output = shouldReverse ? connector1 : connector2
            };
            connector1.Node.Graph.Connections.Add(newConnection);
        }

        private void AddConnection(BaseConnector connector1, ProcessorNode target)
        {
            var allConnectors = connector1.FlowType == ConnectorFlowType.Input ? target.Output.Cast<BaseConnector>() : target.Input;
            var connector = allConnectors.First(c => c.AllowsNewConnections());

            AddConnection(connector1, connector);
        }

        #endregion

        public void DisconnectConnector(BaseConnector connector)
        {
            var graph = connector.Node.Graph;
            var connections = connector.Connections.ToList();
            connections.ForEach(c => graph.Connections.Remove(c));
        }

        public void SplitConnection(BaseConnection connection, Vector2D location)
        {
            var connector = connection.Output;

            var knot = new KnotNode()
            {
                Location = location,
                Flow = connector.FlowType,
                Connector = new KnotConnector(connector.DataType)
                {
                    MaxConnections = connection.Output.MaxConnections + connection.Input.MaxConnections,
                    Shape = connection.Input.Shape
                }
            };
            connection.Graph.Nodes.Add(knot);

            AddConnection(connector, knot.Connector);
            AddConnection(knot.Connector, connection.Input);    // TODO: How does this handle input/output direction since Knot has only a single connector?

            connection.Remove();
        }

        public void AddCommentAroundNodes(IList<BaseNode> nodes, string text = default)
        {
            var rect = nodes.GetBoundingBox(50);
            var comment = new CommentNode()
            {
                Location = rect.Location,
                Size = rect.Size,
                Title = text ?? "New comment"
            };

            nodes[0].Graph.Nodes.Add(comment);
        }
    }
}
