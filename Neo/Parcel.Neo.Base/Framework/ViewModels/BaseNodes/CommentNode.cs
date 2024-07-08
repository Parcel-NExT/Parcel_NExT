using Parcel.Neo.Base.Serialization;
using System;
using System.Collections.Generic;
using Parcel.Neo.Base.DataTypes;

namespace Parcel.Neo.Base.Framework.ViewModels.BaseNodes
{
    public class CommentNode: BaseNode
    {
        #region Construction
        public CommentNode()
        {
            MemberSerialization = new Dictionary<string, NodeSerializationRoutine>()
            {
                {nameof(Title), new NodeSerializationRoutine(() => SerializationHelper.Serialize(_title), value => _title = SerializationHelper.GetString(value))},
                {nameof(Comment), new NodeSerializationRoutine(() => SerializationHelper.Serialize(_comment), value => _comment = SerializationHelper.GetString(value))},
                {nameof(Size), new NodeSerializationRoutine(() => SerializationHelper.Serialize(_size), value => _size = SerializationHelper.GetVector2D(value))},
            };
        }
        #endregion
        
        #region View Components
        private string _title = "Comment";
        public string Title
        {
            get => _title;
            set => SetField(ref _title, value);
        }
        
        private string _comment = string.Empty;
        public string Comment
        {
            get => _comment;
            set => SetField(ref _comment, value);
        }

        private Vector2D _size;
        public Vector2D Size
        {
            get => _size;
            set => SetField(ref _size, value);
        }
        #endregion

        #region Serialization
        public sealed override Dictionary<string, NodeSerializationRoutine> MemberSerialization { get; }
        public override int GetOutputPinID(OutputConnector connector) => throw new InvalidOperationException();
        public override int GetInputPinID(InputConnector connector) => throw new InvalidOperationException();
        public override BaseConnector GetOutputPin(int id) => throw new InvalidOperationException();
        public override BaseConnector GetInputPin(int id) => throw new InvalidOperationException();
        #endregion
    }
}