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
        private object _payload;
        public object Payload 
        {
            get => _payload;
            set => SetField(ref _payload, value);
        }
        public string ValueString // For view binding use
        {
            get => _payload?.ToString() ?? (_valueType.IsValueType ? Activator.CreateInstance(_valueType).ToString() : string.Empty);
            set
            {
                GraphInputOutputNodeBase.ConvertStoredSerialization(ValueType.FullName, value, out _, out object realObject);
                SetField(ref _payload, realObject);
            }
        }
        #endregion
    }

    // TODO: Implement string as base/foundamental representation instead of using object.
    /// <remarks>
    /// See GraphInputOutputComboDataTypeNameToTypeConverter for supported primitive types
    /// </remarks>
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
            List<(string Name, string Type, string Value)> data = Definitions
                .Select(def => (def.Name, def.ValueType.FullName, def.ValueString)) // TODO: Implement proper type referencing naming
                .ToList();

            MemoryStream stream = new();
            BinaryWriter writer = new(stream);
            writer.Write(data.Count);
            foreach ((string Name, string Type, string Value) item in data)
            {
                writer.Write(item.Name);
                writer.Write(item.Type);
                writer.Write(item.Value);
            }
            return stream.ToArray();
        }
        private void DeserializeEntries(byte[] entryData)
        {
            MemoryStream stream = new(entryData);
            BinaryReader reader = new(stream);
            int count = reader.ReadInt32();
            List<(string Name, string ValueType, string Serialization)> source = [];
            for (int i = 0; i < count; i++)
                source.Add((reader.ReadString(), reader.ReadString(), reader.ReadString()));

            Definitions.AddRange(source.Select(tuple =>
            {
                ConvertStoredSerialization(tuple.ValueType, tuple.Serialization, out Type type, out object value);
                return new GraphInputOutputDefinition()
                {
                    Name = tuple.Name,
                    ValueType = type,
                    Payload = value
                };
            }));
            DeserializeFinalize();
        }
        #endregion

        #region Helpers
        internal static void ConvertStoredSerialization(string typeName, string serialization, out Type type, out object value)
        {
            switch (typeName)
            {
                case "System.Boolean":
                    type = typeof(bool);
                    value = bool.Parse(serialization);
                    break;
                case "System.Double":
                    type = typeof(double);
                    value = double.Parse(serialization);
                    break;
                case "System.String":
                    type = typeof(string);
                    value = serialization;
                    break;
                case "System.DateTime":
                    type = typeof(DateTime);
                    value = DateTime.Parse(serialization);
                    break;
                default:
                    throw new ApplicationException();
            }
        }
        #endregion
    }
}