using System;
using System.Collections.Generic;
using System.Linq;
using Parcel.Neo.Base.Framework;
using Parcel.Neo.Base.Framework.Advanced;
using Parcel.Neo.Base.Framework.ViewModels;
using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;
using Parcel.Neo.Base.Serialization;
using Parcel.Neo.Base.DataTypes;

namespace Parcel.Neo.Base.Toolboxes.Basic.Nodes
{
    public class GraphReferenceNode : ProcessorNode
    {
        #region Node Interface
        public GraphReferenceNode()
        {
            ProcessorNodeMemberSerialization = new Dictionary<string, NodeSerializationRoutine>()
            {
                {nameof(GraphPath), new NodeSerializationRoutine(() => SerializationHelper.Serialize(GraphPath), value => GraphPath = SerializationHelper.GetString(value))}
            };

            Title = NodeTypeName = "Graph Reference";
        }
        #endregion

        #region View Binding/Internal Node Properties
        private string _graphPath = null;
        public string GraphPath
        {
            get => _graphPath;
            set
            {
                SetField(ref _graphPath, value);
                UpdateGraphReference();
            }
        }
        #endregion

        #region Routines
        private void UpdateGraphReference()
        {
            if (System.IO.File.Exists(GraphPath))
            {
                CanvasSerialization subgraph = new Subgraph().Load(_graphPath);

                // Clear pins
                Input.Clear();
                Output.Clear();
                InputDefinitions.Clear();
                OutputDefinitions.Clear();

                // Find inputs
                foreach (GraphInputOutputDefinition definition in subgraph.Nodes
                    .Where(n => n is GraphInput).OfType<GraphInput>()
                    .SelectMany(i => i.Definitions))
                {
                    Dictionary<string, GraphInputOutputDefinition> unique = [];
                    if (!unique.ContainsKey(definition.Name))
                    {
                        unique[definition.Name] = definition;
                        Input.Add(new InputConnector(definition.ObjectType) { Title = definition.Name });
                        InputDefinitions.Add(definition);
                    }
                }

                // Find outputs
                foreach (GraphInputOutputDefinition definition in subgraph.Nodes
                    .Where(n => n is GraphOutput).OfType<GraphOutput>()
                    .SelectMany(i => i.Definitions))
                {
                    Dictionary<string, GraphInputOutputDefinition> unique = [];
                    if (!unique.ContainsKey(definition.Name))
                    {
                        unique[definition.Name] = definition;
                        Output.Add(new OutputConnector(definition.ObjectType) { Title = definition.Name });
                        OutputDefinitions.Add(definition);
                    }
                }

                Message.Content = $"{subgraph.Nodes.Count} Nodes";
                Message.Type = NodeMessageType.Normal;
            }
        }
        private static Type GetInputNodeType(GraphInputOutputDefinition definition)
            => definition.ObjectType;
        #endregion

        #region Private States
        private List<GraphInputOutputDefinition> InputDefinitions { get; set; } =
            new List<GraphInputOutputDefinition>();
        private List<GraphInputOutputDefinition> OutputDefinitions { get; set; } =
            new List<GraphInputOutputDefinition>();
        #endregion

        #region Processor Interface
        protected override NodeExecutionResult Execute()
        {
            Dictionary<string, object> parameterSet = new Dictionary<string, object>();
            foreach (InputConnector inputConnector in Input)
                parameterSet[inputConnector.Title] = inputConnector.FetchInputValue<object>();


            Dictionary<string, object> outputParameterSet = ReferenceGraph(GraphPath, parameterSet);

            Dictionary<OutputConnector, object> cache = [];
            foreach ((string key, object value) in outputParameterSet)
            {
                OutputConnector output = Output.SingleOrDefault(o => o.Title == key);
                if (output != null)
                    cache[output] = value;
            }

            return new NodeExecutionResult(new NodeMessage($"{parameterSet.Count} Inputs -> {outputParameterSet.Count} Outputs"), cache);
        }
        #endregion

        #region Serialization
        protected override Dictionary<string, NodeSerializationRoutine> ProcessorNodeMemberSerialization { get; }
        protected override NodeSerializationRoutine VariantInputConnectorsSerialization { get; } = null;
        #endregion

        #region Auto Populate Connections Interface
        public override Tuple<ToolboxNodeExport, Vector2D, InputConnector>[] AutoPopulatedConnectionNodes
        {
            get
            {
                List<Tuple<ToolboxNodeExport, Vector2D, InputConnector>> auto = [];

                // Add nodes for additional variable-inputs
                for (int i = 0; i < Input.Count; i++)
                {
                    if (Input[i].Connections.Count != 0) continue;

                    ToolboxNodeExport toolDef = new(Input[i].Title, GetInputNodeType(InputDefinitions[i]));
                    auto.Add(new Tuple<ToolboxNodeExport, Vector2D, InputConnector>(toolDef, new Vector2D(-100, -50 + (i - 1) * 50), Input[i]));
                }

                return [.. auto];
            }
        }
        public override bool ShouldHaveAutoConnection => Input.Count > 0 && Input.Any(i => i.Connections.Count == 0);
        #endregion

        #region Implementation
        /// <returns>Returns output parameter set</returns>
        public static Dictionary<string, object> ReferenceGraph(string inputGraph, Dictionary<string, object> inputParameterSet)
        {
            // Instantiate
            NodesCanvas canvas = new();
            canvas.Open(inputGraph);

            // Execute
            return new Subgraph().Execute(canvas, inputParameterSet);
        }
        #endregion
    }
}