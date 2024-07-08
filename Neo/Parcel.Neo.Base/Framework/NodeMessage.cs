using Parcel.Neo.Base.Framework.ViewModels;

namespace Parcel.Neo.Base.Framework
{
    public enum NodeMessageType
    {
        Empty,
        Normal,
        Error,
        Documentation
    }
    
    public class NodeMessage: ObservableObject
    {
        #region Public View Properties
        private NodeMessageType _type;
        public NodeMessageType Type
        {
            get => _type;
            set => SetField(ref _type, value);
        }
        private string _content;
        public string Content
        {
            get => _content;
            set => SetField(ref _content, value);
        }
        #endregion

        public NodeMessage()
        {
        }

        public NodeMessage(string content, NodeMessageType type = NodeMessageType.Normal)
        {
            _type = type;
            _content = content;
        }
    }
}