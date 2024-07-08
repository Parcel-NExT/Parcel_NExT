using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;
using Parcel.Neo.Base.Serialization;

namespace Parcel.Neo.Base.Framework.ViewModels
{
    public class CanvasSerialization
    {
        public List<BaseNode> Nodes { get; set; }
        public List<BaseConnection> Connections { get; set; }
    }
    
    public class NodesCanvas: ObservableObject
    {
        public NodesCanvas()
        {
            Schema = new GraphSchema();
            
            PendingConnection = new PendingConnection()
            {
                Graph = this
            };

            DeleteSelectionCommand = new DelegateCommand(DeleteSelection, () => SelectedNodes.Count > 0);
            CommentSelectionCommand = new DelegateCommand(() => Schema.AddCommentAroundNodes(SelectedNodes, "New comment"), () => SelectedNodes.Count > 0);
            DisconnectConnectorCommand = new DelegateCommand<BaseConnector>(c => c.Disconnect());
            CreateConnectionCommand = new DelegateCommand<object>(target => Schema.TryAddConnection(PendingConnection.Source!, target), target => PendingConnection.Source != null && target != null);

            Connections.WhenAdded(c =>
            {
                c.Graph = this;
                c.Input.Connections.Add(c);
                c.Output.Connections.Add(c);
            })
            // Called when the collection is cleared
            .WhenRemoved(c =>
            {
                c.Input.Connections.Remove(c);
                c.Output.Connections.Remove(c);
            });

            Nodes.WhenAdded(x => x.Graph = this)
                 // Not called when the collection is cleared
                 .WhenRemoved(x =>
                 {
                     if (x is ProcessorNode flow)
                     {
                         flow.Disconnect();
                     }
                     else if (x is KnotNode knot)
                     {
                         knot.Connector.Disconnect();
                     }
                 })
                 .WhenCleared(x => Connections.Clear());
        }

        #region Public View Properties
        private NotifyObservableCollection<BaseNode> _nodes = new NotifyObservableCollection<BaseNode>();
        public NotifyObservableCollection<BaseNode> Nodes
        {
            get => _nodes;
            set => SetField(ref _nodes, value);
        }

        private NotifyObservableCollection<BaseNode> _selectedNodes = new NotifyObservableCollection<BaseNode>();
        public NotifyObservableCollection<BaseNode> SelectedNodes
        {
            get => _selectedNodes;
            set => SetField(ref _selectedNodes, value);
        }

        private NotifyObservableCollection<BaseConnection> _connections = new NotifyObservableCollection<BaseConnection>();
        public NotifyObservableCollection<BaseConnection> Connections
        {
            get => _connections;
            set => SetField(ref _connections, value);
        }
        #endregion

        #region Interface
        public void Open(string path)
        {
            CanvasSerialization loaded = new GraphSerializer().Deserialize(path, this);
            
            SelectedNodes.Clear();
            Nodes.Clear(); Nodes.AddRange(loaded.Nodes);
            Connections.Clear(); Connections.AddRange(loaded.Connections);
        }

        public void Save(string path)
        {
            new GraphSerializer().Serialize(path, new CanvasSerialization()
            {
                Nodes = Nodes.ToList(),
                Connections = Connections.ToList()
            });
        }
        #endregion

        #region Intermediate Data
        public PendingConnection PendingConnection { get; }
        public GraphSchema Schema { get; }
        #endregion

        #region Commands
        public ICommand DeleteSelectionCommand { get; }
        public ICommand DisconnectConnectorCommand { get; }
        public ICommand CreateConnectionCommand { get; }
        public ICommand CommentSelectionCommand { get; }
        #endregion

        #region Routines
        private void DeleteSelection()
        {
            var selected = SelectedNodes.ToList();

            for (int i = 0; i < selected.Count; i++)
            {
                Nodes.Remove(selected[i]);
            }
        }
        #endregion
    }
}