using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;

namespace Parcel.Neo.Base.Framework.ViewModels
{
    public class PendingConnection: ObservableObject
    {
        private NodesCanvas _graph = default!;
        public NodesCanvas Graph
        {
            get => _graph;
            internal set => SetField(ref _graph, value);
        }

        #region Public View Properties
        private BaseConnector _source;
        public BaseConnector Source
        {
            get => _source;
            set => SetField(ref _source, value);
        }

        private object _previewTarget;
        public object PreviewTarget
        {
            get => _previewTarget;
            set
            {
                if (SetField(ref _previewTarget, value))
                {
                    OnPreviewTargetChanged();
                }
            }
        }

        private string _previewText = "Drop on connector";
        public string PreviewText
        {
            get => _previewText;
            set => SetField(ref _previewText, value);
        }
        #endregion
        

        protected virtual void OnPreviewTargetChanged()
        {
            bool canConnect = PreviewTarget != null && Graph.Schema.CanAddConnection(Source!, PreviewTarget);
            PreviewText = PreviewTarget switch
            {
                BaseConnector con when con == Source => $"Can't connect to self",
                BaseConnector con => $"{(canConnect ? "Connect" : "Can't connect")} to {con.Title ?? "pin"}",
                ProcessorNode flow => $"{(canConnect ? "Connect" : "Can't connect")} to {flow.Title ?? "node"}",
                _ => $"Drop on connector"
            };
        }
    }
}