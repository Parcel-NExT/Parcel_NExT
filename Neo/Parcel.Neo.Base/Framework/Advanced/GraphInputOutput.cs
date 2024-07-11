using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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
        private Type _valueType;
        public Type ValueType
        {
            get => _valueType;
            set => SetField(ref _valueType, value);
        }
        #endregion

        #region Payload
        public object Payload { get; set; }
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
        private ObservableCollection<GraphInputOutputDefinition> _definitions = [];
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
            GraphInputOutputDefinition def = new() { Name = name, ValueType = typeof(double) };
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
            List<(string, string)> data = Definitions
                .Select(def => (def.Name, def.ValueType.FullName)) // TODO: Implement proper type referencing naming
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
            List<(string Name, string ValueType)> source = [];
            for (int i = 0; i < count; i++)
                source.Add((reader.ReadString(), reader.ReadString()));

            Definitions.AddRange(source.Select(tuple => new GraphInputOutputDefinition()
            {
                Name = tuple.Name,
                ValueType = typeof(object) // TODO: Impelement proper de-referencing for ValueType
            }));
            DeserializeFinalize();
        }
        #endregion
    }
}