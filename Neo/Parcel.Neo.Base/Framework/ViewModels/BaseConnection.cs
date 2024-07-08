using Parcel.Neo.Base.DataTypes;

namespace Parcel.Neo.Base.Framework.ViewModels
{
    public class BaseConnection: ObservableObject
    {
        private NodesCanvas _graph = default!;
        public NodesCanvas Graph
        {
            get => _graph;
            internal set => SetField(ref _graph, value);
        }

        #region View Components
        private BaseConnector _input = default!;
        public BaseConnector Input
        {
            get => _input;
            set => SetField(ref _input, value);
        }

        private BaseConnector _output = default!;
        public BaseConnector Output
        {
            get => _output;
            set => SetField(ref _output, value);
        }
        #endregion

        #region Interface Methods
        public void Split(Vector2D point)
            => Graph.Schema.SplitConnection(this, point);

        public void Remove()
            => Graph.Connections.Remove(this);
        #endregion
    }
}