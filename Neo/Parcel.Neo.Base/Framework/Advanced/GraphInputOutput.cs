using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Parcel.Neo.Base.DataTypes;
using Parcel.Neo.Base.Framework.ViewModels;
using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;

namespace Parcel.Neo.Base.Framework.Advanced
{
    public class GraphInputOutputDefinition: ObservableObject
    {
        #region Properties
        private string _name = "New";
        public string Name
        {
            get => _name;
            set => SetField(ref _name, value);
        }
        private CacheDataType _type = CacheDataType.Number;
        public CacheDataType Type
        {
            get => _type;
            set => SetField(ref _type, value);
        }
        #endregion

        #region Payload
        public object Payload { get; set; }
        #endregion

        #region Accessor
        public Type ObjectType => typeof(object); // Remark: Do we actually need anything more specific than that (object type)?
        #endregion
    }

    public abstract class GraphInputOutputNodeBase : ProcessorNode
    {
        #region Node Interface
        protected GraphInputOutputNodeBase()
        {
            Title = NodeTypeName = "Graph Input Output";

            AddEntryCommand = new RequeryCommand(
                AddEntry,
                () => true);
            RemoveEntryCommand = new RequeryCommand(
                RemoveEntry,
                () => Definitions.Count > 1);
            
            // Serialization
            VariantInputConnectorsSerialization = new NodeSerializationRoutine(SerializeEntries,
                source => DeserializeEntries(source)); 
        }
        #endregion

        #region View Binding/Internal Node Properties
        private ObservableCollection<GraphInputOutputDefinition> _definitions = new ObservableCollection<GraphInputOutputDefinition>();
        public ObservableCollection<GraphInputOutputDefinition> Definitions { get => _definitions; private set => SetField(ref _definitions, value); }
        public IProcessorNodeCommand AddEntryCommand { get; }
        public IProcessorNodeCommand RemoveEntryCommand { get; }
        #endregion
        
        #region Additional View Binding
        protected abstract Action<GraphInputOutputDefinition> DefinitionChanged { get; }
        #endregion
        
        #region Routines
        private void AddEntry()
        {
            string name = $"{NewEntryPrefix} {Definitions.Count + 1}";
            GraphInputOutputDefinition def = new GraphInputOutputDefinition() {Name = name};
            def.PropertyChanged += (sender, args) => DefinitionChanged(sender as GraphInputOutputDefinition);
            
            Definitions.Add(def);
            PostAddEntry(def);
        }
        private void RemoveEntry()
        {
            Definitions.RemoveAt(Definitions.Count - 1);
            PostRemoveEntry();
        }
        protected abstract void PostAddEntry(GraphInputOutputDefinition definition);
        protected abstract void PostRemoveEntry();
        protected abstract string NewEntryPrefix { get; }
        #endregion
        
        #region Processor Interface
        protected abstract override NodeExecutionResult Execute();
        #endregion
        
        #region Serialization

        protected sealed override Dictionary<string, NodeSerializationRoutine>
            ProcessorNodeMemberSerialization { get; } = null;
        protected abstract void DeserializeFinalize();
        protected override NodeSerializationRoutine VariantInputConnectorsSerialization { get; }
        #endregion

        #region Routiens
        private byte[] SerializeEntries()
        {
            List<Tuple<string, int>> data = Definitions.Select(def => new Tuple<string, int>(def.Name, (int)def.Type))
                .ToList();

            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);
            writer.Write(data.Count);
            foreach (var item in data)
            {
                writer.Write(item.Item1);
                writer.Write(item.Item2);
            }
            return stream.ToArray();
        }
        private void DeserializeEntries(byte[] entryData)
        {
            var stream = new MemoryStream(entryData);
            var reader = new BinaryReader(stream);
            int count = reader.ReadInt32();
            List<Tuple<string, int>> source = [];
            for (int i = 0; i < count; i++)
                source.Add(new Tuple<string, int>(reader.ReadString(), reader.ReadInt32()));

            Definitions.AddRange(source.Select(tuple => new GraphInputOutputDefinition()
            {
                Name = tuple.Item1,
                Type = (CacheDataType) tuple.Item2
            }));
            DeserializeFinalize();
        }
        #endregion
    }
}