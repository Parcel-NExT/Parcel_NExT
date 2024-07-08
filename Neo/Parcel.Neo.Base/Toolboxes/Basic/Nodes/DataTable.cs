using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Parcel.Types;
using Parcel.Neo.Base.Framework;
using Parcel.Neo.Base.Framework.ViewModels;
using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;
using Parcel.Neo.Base.DataTypes;

namespace Parcel.Neo.Base.Toolboxes.Basic.Nodes
{
    /// <summary>
    /// TODO: Editing for Data Table node is not working yet at this moment due to a transform as Expando object during presentation
    /// </summary>
    public class DataTableFieldDefinition : ObservableObject
    {
        #region Properties
        private string _name = "New Field";
        public string Name
        {
            get => _name;
            set => SetField(ref _name, value);
        }
        private DictionaryEntryType _type = DictionaryEntryType.Number;
        public DictionaryEntryType Type
        {
            get => _type;
            set => SetField(ref _type, value);
        }
        #endregion
    }

    public class DataTable : ProcessorNode
    {
        #region Node Interface
        private readonly OutputConnector _dataTableOutput = new OutputConnector(typeof(DataGrid))
        {
            Title = "Data Table"
        };
        public DataTable()
        {
            // Serialization
            ProcessorNodeMemberSerialization = new Dictionary<string, NodeSerializationRoutine>()
            {
                // TODO: Why do we even need to serialize anything for this node?
                // {nameof(Data), new NodeSerializationRoutine(() => SerializationHelper.Serialize(Data), o => Data = o as object[][])},
                {nameof(Data), null},
            };
            VariantInputConnectorsSerialization = new NodeSerializationRoutine(SerializeEntries,
                DeserializeEntries);

            Definitions =
            [
                new DataTableFieldDefinition() { Name = "New Field" }
            ];

            AddEntryCommand = new RequeryCommand(
                () => Definitions.Add(new DataTableFieldDefinition() { Name = $"New Field {Definitions.Count + 1}" }),
                () => true);
            RemoveEntryCommand = new RequeryCommand(
                () => Definitions.RemoveAt(Definitions.Count - 1),
                () => Definitions.Count > 1);

            Title = NodeTypeName = "Data Table";
            Output.Add(_dataTableOutput);
        }
        #endregion

        #region Native Data
        public object[][] Data { get; set; }
        #endregion

        #region View Binding/Internal Node Properties
        private ObservableCollection<DataTableFieldDefinition> _definitions;
        public ObservableCollection<DataTableFieldDefinition> Definitions
        {
            get => _definitions;
            private set => SetField(ref _definitions, value);
        }
        public IProcessorNodeCommand AddEntryCommand { get; }
        public IProcessorNodeCommand RemoveEntryCommand { get; }
        #endregion

        #region Methods
        public DataGrid InitializeDataGrid()
        {
            // Create table
            DataGrid dataGrid = new();
            // Update columns
            foreach (DataTableFieldDefinition definition in Definitions)
            {
                DataColumn column = dataGrid.AddColumn(definition.Name);
                // Add data to fix column type
                switch (definition.Type)
                {
                    case DictionaryEntryType.Number:
                        column.Add(0.0);
                        break;
                    case DictionaryEntryType.String:
                        column.Add(string.Empty);
                        break;
                    case DictionaryEntryType.Boolean:
                        column.Add(false);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                if (Data != null)
                    column.RemoveAt(0); // Remove redundant data
            }

            // Populate data
            if (Data != null)
            {
                for (int col = 0; col < Math.Min(Data.Length, Definitions.Count); col++)
                    for (int row = 0; row < Data[col].Length; row++)
                        dataGrid.Columns[col].Add(Data[col][row]);
            }

            return dataGrid;
        }
        #endregion

        #region Processor Interface
        protected override NodeExecutionResult Execute()
        {
            DataGrid dataGrid = InitializeDataGrid();
            return new NodeExecutionResult(new NodeMessage($"{dataGrid.ColumnCount} Fields."), new Dictionary<OutputConnector, object>()
            {
                {_dataTableOutput, dataGrid}
            });
        }
        #endregion

        #region Serialization
        protected override Dictionary<string, NodeSerializationRoutine> ProcessorNodeMemberSerialization { get; }
        protected override NodeSerializationRoutine VariantInputConnectorsSerialization { get; }
        #endregion

        #region Routines
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

            Definitions.Clear();
            Definitions.AddRange(source.Select(tuple => new DataTableFieldDefinition()
            {
                Name = tuple.Item1,
                Type = (DictionaryEntryType)tuple.Item2
            }));
        }
        #endregion
    }
}