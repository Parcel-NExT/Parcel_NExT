using System;
using System.Collections.Generic;
using Parcel.Neo.Base.Framework.ViewModels;

namespace Parcel.Neo.Base.Framework.Advanced
{
    public class GraphOutput: GraphInputOutputNodeBase
    {
        #region Node Interface
        public GraphOutput()
        {
            Title = NodeTypeName = "Graph Output";
            DefinitionChanged = definition =>
            {
                InputConnector input = Input[Definitions.IndexOf(definition)];
                input.Title = definition.Name;
                input.DataType = definition.ObjectType;
                input.UpdateConnectorShape();
            };
        }
        #endregion
        
        #region Routines
        protected override string NewEntryPrefix { get; } = "Output";
        protected override Action<GraphInputOutputDefinition> DefinitionChanged { get; }
        protected sealed override void PostAddEntry(GraphInputOutputDefinition definition)
        {
            Input.Add(new InputConnector(definition.ObjectType)
            {
                Title = definition.Name
            });
        }
        protected sealed override void PostRemoveEntry()
        {
            Input.RemoveAt(Input.Count - 1);
        }
        #endregion
        
        #region Processor Interface

        protected override NodeExecutionResult Execute()
        {
            // Fetch value from input and update payload
            for (int index = 0; index < Definitions.Count; index++)
            {
                GraphInputOutputDefinition definition = Definitions[index];
                definition.Payload = Input[index].FetchInputValue<object>();
            }

            return new NodeExecutionResult(new NodeMessage($"{Definitions.Count} Outputs."), null);
        }
        #endregion

        #region Serialization
        protected override void DeserializeFinalize()
        {
            foreach (GraphInputOutputDefinition definition in Definitions)
                Input.Add(new InputConnector(definition.ObjectType)
                {
                    Title = definition.Name
                });
        }
        #endregion
    }
}