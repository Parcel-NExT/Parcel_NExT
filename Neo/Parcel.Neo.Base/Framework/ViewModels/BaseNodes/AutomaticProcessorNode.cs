using System;
using System.Collections.Generic;
using System.Linq;
using Parcel.Types;
using Parcel.Neo.Base.Serialization;
using Parcel.Neo.Base.DataTypes;
using System.Diagnostics;
using System.IO;

namespace Parcel.Neo.Base.Framework.ViewModels.BaseNodes
{
    /// <summary>
    /// An encapsulation of a base node instance that's generated directly from methods;
    /// We will start with only a single output but there shouldn't be much difficulty outputting more outputs
    /// </summary>
    public class AutomaticProcessorNode: ProcessorNode
    {
        #region Constructor
        public AutomaticProcessorNode()
        {
            ProcessorNodeMemberSerialization = new Dictionary<string, NodeSerializationRoutine>()
            {
                {nameof(AutomaticNodeType), new NodeSerializationRoutine(() => SerializationHelper.Serialize(AutomaticNodeType), value => AutomaticNodeType = SerializationHelper.GetString(value))},
                //{nameof(InputTypes), new NodeSerializationRoutine(() => SerializationHelper.Serialize(InputTypes), value => InputTypes = SerializationHelper.GetCacheDataTypes(value))},
                //{nameof(OutputTypes), new NodeSerializationRoutine(() => SerializationHelper.Serialize(OutputTypes), value => OutputTypes = SerializationHelper.GetCacheDataTypes(value))},
                {nameof(InputNames), new NodeSerializationRoutine(() => SerializationHelper.Serialize(InputNames), value => InputNames = SerializationHelper.GetStrings(value))},
                {nameof(OutputNames), new NodeSerializationRoutine(() => SerializationHelper.Serialize(OutputNames), value => OutputNames = SerializationHelper.GetStrings(value))},
            };
        }
        public AutomaticNodeDescriptor Descriptor { get; } // Remark-cz: Hack we are saving descriptor here for easier invoking of dynamic types; However, this is not serializable at the moment! The reason we don't want it is because the descriptor itself is not serialized which means when the graph is loaded all such information is gone - and that's why we had IToolboxDefinition before.
        public AutomaticProcessorNode(AutomaticNodeDescriptor descriptor) :this()
        {
            // Remark-cz: Hack we are saving descriptor here for easier invoking of dynamic types; However, this is not serializable at the mometn!
            Descriptor = descriptor;

            // Serialization
            AutomaticNodeType = descriptor.NodeName;
            InputTypes = descriptor.InputTypes;
            DefaultInputValues = descriptor.DefaultInputValues;
            OutputTypes = descriptor.OutputTypes;
            InputNames = descriptor.InputNames;
            OutputNames = descriptor.OutputNames;
            
            // Population
            PopulateInputsOutputs();
        }
        #endregion

        #region Routines
        private Func<object[], object[]> RetrieveCallMarshal()
        {
            try
            {
                if (Descriptor != null)
                {
                    // This is runtime only!
                    return Descriptor.CallMarshal;
                }
                else 
                {
                    // Remark-cz: This is more general and can handle serialization well
                    //IToolboxDefinition toolbox = (IToolboxDefinition)Activator.CreateInstance(Type.GetType(ToolboxFullName));
                    //AutomaticNodeDescriptor descriptor = toolbox.AutomaticNodes.Single(an => an != null && an.NodeName == AutomaticNodeType);
                    //return descriptor.CallMarshal;
                    throw new NotImplementedException();
                }
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Failed to retrieve node: {e.Message}.");
            }
        }
        private void PopulateInputsOutputs()
        {
            Title = NodeTypeName = AutomaticNodeType;
            for (int index = 0; index < InputTypes.Length; index++)
            {
                Type inputType = InputTypes[index];
                object? defaultValue = DefaultInputValues?[index];
                string preferredTitle = InputNames?[index];
                if (Nullable.GetUnderlyingType(inputType) != null)
                    CreateInputPin(Nullable.GetUnderlyingType(inputType), defaultValue, preferredTitle); // TODO: Current implementation has issue making nullable default values as type default rather than null
                else 
                    CreateInputPin(inputType, defaultValue, preferredTitle);
            }

            for (int index = 0; index < OutputTypes.Length; index++)
            {
                Type outputType = OutputTypes[index];
                string? preferredTitle = OutputNames == null ? GetPreferredTitle(outputType) : OutputNames?[index];
                Output.Add(new OutputConnector(outputType) { Title = preferredTitle ?? "Result" });
            }

            void CreateInputPin(Type inputType, object? defaultValue, string preferredTitle)
            {
                if (inputType == typeof(bool))
                    Input.Add(new PrimitiveBooleanInputConnector(defaultValue != DBNull.Value ? (bool)defaultValue : null) { Title = preferredTitle ?? "Bool" });
                else if (inputType == typeof(string))
                    Input.Add(new PrimitiveStringInputConnector(defaultValue != DBNull.Value ? (string)defaultValue : null) { Title = preferredTitle ?? "String" });
                else if (IsNumericalType(inputType))
                    Input.Add(new PrimitiveNumberInputConnector(inputType, defaultValue == DBNull.Value ? null : defaultValue) { Title = preferredTitle ?? "Number" });
                else if (inputType == typeof(DateTime))
                    Input.Add(new PrimitiveDateTimeInputConnector(defaultValue != DBNull.Value ? (DateTime)defaultValue : null) { Title = preferredTitle ?? "Date" });
                else
                    Input.Add(new InputConnector(inputType) { Title = preferredTitle ?? "Input" });
            }
            static string? GetPreferredTitle(Type type)
            {
                if (type == typeof(bool))
                    return "Truth";
                else if (type == typeof(string))
                    return "Value";
                else if (type == typeof(double))
                    return "Number";
                else if (type == typeof(DateTime))
                    return "Date";
                else if (type == typeof(DataGrid) || type == typeof(DataColumn))
                    return "Data";
                else
                    return null;
            }
            static bool IsNumericalType(Type inputType)
            {
                Type checkType = inputType;
                if (Nullable.GetUnderlyingType(inputType) != null)
                    // It's nullable
                    checkType = Nullable.GetUnderlyingType(inputType)!;

                switch (Type.GetTypeCode(checkType))
                {
                    case TypeCode.Byte:
                    case TypeCode.SByte:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.Decimal:
                    case TypeCode.Double:
                    case TypeCode.Single:
                        return true;
                    default:
                        return false;
                }
            }
        }
        #endregion

        #region Properties
        private string AutomaticNodeType { get; set; }
        private Type[] InputTypes { get; set; }
        private Type[] OutputTypes { get; set; }
        private object?[]? DefaultInputValues { get; set; }
        private string[]? InputNames { get; set; }
        private string[] OutputNames { get; set; }
        #endregion

        #region Processor Interface
        protected override NodeExecutionResult Execute()
        {
            Dictionary<OutputConnector, object> cache = [];

            try
            {
                Stopwatch timer = new();
                timer.Start();
                Func<object[], object[]> marshal = RetrieveCallMarshal();
                object[] outputs = marshal.Invoke(Input.Select(i => i.FetchInputValue<object>()).ToArray());
                for (int index = 0; index < outputs.Length; index++)
                {
                    object output = outputs[index];
                    OutputConnector connector = Output[index];
                    cache[connector] = output;
                }
                timer.Stop();

                if ((int)timer.Elapsed.TotalMilliseconds > 0) // Millisecond scale
                    return new NodeExecutionResult(new NodeMessage($"Finished in {timer.Elapsed.TotalMilliseconds:F2}ms"), cache);
                else if ((int)timer.Elapsed.TotalMicroseconds > 0) // Microsecond scale
                    return new NodeExecutionResult(new NodeMessage($"Finished in {timer.Elapsed.TotalMicroseconds:F2}μs"), cache);
                else // Nanosecond scale
                    return new NodeExecutionResult(new NodeMessage($"Finished in {timer.Elapsed.TotalNanoseconds:F2}ns"), cache);
            }
            catch (Exception e)
            {
                return new NodeExecutionResult(new NodeMessage($"Error: {e.InnerException?.Message ?? e.Message}", NodeMessageType.Error), null);
            }
        }
        #endregion

        #region Serialization
        protected sealed override Dictionary<string, NodeSerializationRoutine> ProcessorNodeMemberSerialization { get; }
        internal override void PostDeserialization()
        {
            base.PostDeserialization();
            PopulateInputsOutputs();
        }
        protected override NodeSerializationRoutine VariantInputConnectorsSerialization { get; } = null;
        #endregion

        #region Auto-Connect Interface
        public override Tuple<ToolboxNodeExport, Vector2D, InputConnector>[] AutoPopulatedConnectionNodes
        {
            get
            {
                List<Tuple<ToolboxNodeExport, Vector2D, InputConnector>> auto = [];
                for (int i = 0; i < Input.Count; i++)
                {
                    if(!InputConnectorShouldRequireAutoConnection(Input[i])) continue;

                    // TODO: Review and examine this part of logic and see whether we can improve it
                    Type nodeType = InputTypes[i];
                    ToolboxNodeExport toolDef = new(Input[i].Title, nodeType);
                    auto.Add(new Tuple<ToolboxNodeExport, Vector2D, InputConnector>(toolDef, new Vector2D(-100, -50 + (i - 1) * 50), Input[i]));
                }
                return [.. auto];
            }
        }

        public override bool ShouldHaveAutoConnection => Input.Count > 0 && Input.Any(InputConnectorShouldRequireAutoConnection);
        #endregion
    }
}