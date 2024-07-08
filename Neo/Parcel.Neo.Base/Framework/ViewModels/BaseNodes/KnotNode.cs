using System;
using System.Collections.Generic;
using System.Linq;
using Parcel.Neo.Base.DataTypes;

namespace Parcel.Neo.Base.Framework.ViewModels.BaseNodes
{
    public class KnotNode : BaseNode
    {
        #region View Components
        private BaseConnector _connector = default!;
        public BaseConnector Connector
        {
            get => _connector;
            set
            {
                if (SetField(ref _connector, value))
                {
                    _connector.Node = this;
                }
            }
        }
        #endregion

        public ConnectorFlowType Flow { get; set; }

        #region Accessor
        public BaseNode Previous =>
            Connector.Connections.SingleOrDefault(c => c.Input.Node != this)?.Input.Node ?? null;
        public IEnumerable<BaseNode> Next => Connector.Connections
            .Where(c => c.Input.Node == this || c.Output.IsConnected)
            .Select(c => c.Output.Node);
        #endregion

        #region Serialization
        public override Dictionary<string, NodeSerializationRoutine> MemberSerialization { get; } =
            new Dictionary<string, NodeSerializationRoutine>();
        public override int GetOutputPinID(OutputConnector connector) =>
            connector == Connector ? 0 : throw new ArgumentException("Invalid connector.");
        public override int GetInputPinID(InputConnector connector) =>
            connector == Connector ? 0 : throw new ArgumentException("Invalid connector.");
        public override BaseConnector GetOutputPin(int id) => Connector;
        public override BaseConnector GetInputPin(int id) => Connector;
        #endregion
    }
}