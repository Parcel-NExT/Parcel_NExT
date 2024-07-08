using System.Collections.Generic;
using System.Linq;
using Parcel.Types;
using Parcel.Neo.Base.Framework;
using Parcel.Neo.Base.Framework.ViewModels;
using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;
using Parcel.Neo.Base.Serialization;
using Parcel.Integration;

namespace Parcel.Neo.Base.Toolboxes.Basic.Nodes
{
    public class DatabaseTableInputConnector : InputConnector
    {
        #region Properties
        private string _tableName = "New Key";
        public string TableName
        {
            get => _tableName;
            set => SetField(ref _tableName, value);
        }
        #endregion

        #region Constructor
        public DatabaseTableInputConnector(string tableName) : base(typeof(DataGrid))
        {
            TableName = tableName;
            Title = "Data Table";
        }
        #endregion
    }

    /// <remarks>
    /// To replace such explicit definition of node, we need two infrastructure updates:
    /// 1. Allow coercion or at least automatic array input handling on automatic nodes
    /// 2. Allow automatic property editors based on function argument attribute
    /// </remarks>
    public class SQL : DynamicInputProcessorNode, INodeProperty
    {
        #region Node Interface
        private readonly OutputConnector _dataTableOutput = new(typeof(DataGrid))
        {
            Title = "Result"
        };
        public SQL()
        {
            // Serialization
            ProcessorNodeMemberSerialization = new Dictionary<string, NodeSerializationRoutine>()
            {
                {nameof(Code), new NodeSerializationRoutine(() => SerializationHelper.Serialize(Code), o => Code = SerializationHelper.GetString(o))}
            };
            VariantInputConnectorsSerialization = new NodeSerializationRoutine(() => SerializationHelper.Serialize(Input.Count), o =>
            {
                Input.Clear();
                int count = SerializationHelper.GetInt(o);
                for (int i = 0; i < count; i++)
                    AddInputs();
            });


            Editors =
            [
                new PropertyEditor("Code", PropertyEditorType.Code, () => _code, o => Code = (string)o)
            ];

            Title = NodeTypeName = "SQL";
            Output.Add(_dataTableOutput);

            AddInputs();

            AddEntryCommand = new RequeryCommand(
                AddInputs,
                () => true);
            RemoveEntryCommand = new RequeryCommand(
                RemoveInputs,
                () => Input.Count > 1);
        }
        #endregion

        #region View Binding/Internal Node Properties
        private string _code = "select * from @Table1";
        public string Code
        {
            get => _code;
            set => SetField(ref _code, value);
        }
        #endregion

        #region Property Editor Interface
        public List<PropertyEditor> Editors { get; }
        #endregion

        #region Routines
        private void AddInputs()
        {
            Input.Add(new DatabaseTableInputConnector($"Table {Input.Count + 1}"));
        }
        private void RemoveInputs()
        {
            Input.RemoveAt(Input.Count - 1);
        }
        #endregion

        #region Processor Interface
        public override OutputConnector MainOutput => _dataTableOutput;
        protected override NodeExecutionResult Execute()
        {
            DataGrid[] inputTables = Input.Select(i =>
            {
                DatabaseTableInputConnector connector = i as DatabaseTableInputConnector;
                var table = connector!.FetchInputValue<DataGrid>();
                // table.TableName = connector.TableName;
                return table;
            }).ToArray();
            string[] inputTableNames = Input.Select(i => (i as DatabaseTableInputConnector)!.TableName).ToArray();
            DataGrid output = DataProcessingHelper.SQL(inputTables, inputTableNames, Code);


            return new NodeExecutionResult(new NodeMessage($"{output.RowCount} Rows; {output.ColumnCount} Columns."), new Dictionary<OutputConnector, object>()
            {
                {_dataTableOutput, output}
            });
        }
        #endregion

        #region Serialization
        protected override Dictionary<string, NodeSerializationRoutine> ProcessorNodeMemberSerialization { get; }
        protected override NodeSerializationRoutine VariantInputConnectorsSerialization { get; }
        #endregion
    }
}