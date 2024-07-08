using System;
using System.Collections.Generic;
using Parcel.Neo.Base.Framework.ViewModels;

namespace Parcel.Neo.Base.Framework.Advanced
{
    public class GraphInput: GraphInputOutputNodeBase
    {
        #region Node Interface
        public GraphInput()
        {
            Title = NodeTypeName = "Graph Input";
            DefinitionChanged = definition =>
            {
                OutputConnector output = Output[Definitions.IndexOf(definition)];
                output.Title = definition.Name;
                output.DataType = definition.ObjectType;
                output.UpdateConnectorShape();
            };
        }
        #endregion

        #region Routines
        protected override string NewEntryPrefix { get; } = "Input";
        protected override Action<GraphInputOutputDefinition> DefinitionChanged { get; }
        protected sealed override void PostAddEntry(GraphInputOutputDefinition definition)
        {
            Output.Add(new OutputConnector(definition.ObjectType)
            {
                Title = definition.Name
            });
        }
        protected sealed override void PostRemoveEntry()
        {
            Output.RemoveAt(Input.Count - 1);
        }
        #endregion
        
        #region Processor Interface
        protected override NodeExecutionResult Execute()
        {
            Dictionary<OutputConnector, object> cache = new Dictionary<OutputConnector, object>();
            for (int index = 0; index < Definitions.Count; index++)
            {
                GraphInputOutputDefinition definition = Definitions[index];
                cache[Output[index]] = definition.Payload;
            }

            return new NodeExecutionResult(new NodeMessage($"{Definitions.Count} Inputs."), cache);
        }
        #endregion

        #region Serialization
        protected override void DeserializeFinalize()
        {
            foreach (GraphInputOutputDefinition definition in Definitions)
                Output.Add(new OutputConnector(definition.ObjectType)
                {
                    Title = definition.Name
                });
        }
        #endregion
    }
}