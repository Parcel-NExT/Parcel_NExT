using System;
using System.Collections.Generic;
using System.Linq;
using Parcel.Neo.Base.Serialization;
using Parcel.Neo.Base.DataTypes;

namespace Parcel.Neo.Base.Framework.ViewModels.BaseNodes
{
    public class NodeSerializationRoutine
    {
        public Func<byte[]> Serialize { get; set; }
        public Action<byte[]> Deserialize { get; set; }

        public NodeSerializationRoutine(Func<byte[]> serialize, Action<byte[]> deserialize)
        {
            Serialize = serialize;
            Deserialize = deserialize;
        }
    }
    
    public abstract class BaseNode : ObservableObject
    {
        #region Public View Properties
        private Vector2D _location;
        public Vector2D Location
        {
            get => _location;
            set => SetField(ref _location, value);
        }
        #endregion

        #region References
        private NodesCanvas _graph = default!;
        public NodesCanvas Graph
        {
            get => _graph;
            internal set => SetField(ref _graph, value);
        }
        #endregion

        #region Serialization Interface
        public abstract Dictionary<string, NodeSerializationRoutine> MemberSerialization { get; }
        #endregion

        #region Serialization
        internal NodeData Serialize()
        {
            // Instance members
            Dictionary<string, byte[]> members = MemberSerialization.ToDictionary(ms => ms.Key, ms => ms.Value.Serialize());
            // Base members
            members[nameof(Location)] = SerializationHelper.Serialize(_location);

            return new NodeData()
            {
                NodeType = GetType().AssemblyQualifiedName,
                NodeMembers = members.Select(m => (m.Key, m.Value)).ToArray()
            };
        }

        internal void Deserialize((string Key, byte[] Value)[] tuples, NodesCanvas canvas)
        {
            Dictionary<string, byte[]> members = tuples.ToDictionary(t => t.Key, t => t.Value);

            // Base members
            Graph = canvas;
            _location = SerializationHelper.GetVector2D(members[nameof(Location)]);
            
            // Instance members
            Dictionary<string, NodeSerializationRoutine> instanceMembers = MemberSerialization;
            foreach ((string key, byte[] data) in members)
            {
                if(instanceMembers.ContainsKey(key))
                    instanceMembers[key].Deserialize(data);
            }
        }
        internal virtual void PostDeserialization(){}
        
        public abstract int GetOutputPinID(OutputConnector connector);
        public abstract int GetInputPinID(InputConnector connector);
        public abstract BaseConnector GetOutputPin(int id);
        public abstract BaseConnector GetInputPin(int id);
        #endregion
    }
}