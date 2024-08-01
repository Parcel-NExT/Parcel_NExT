using System;
using System.Collections.Generic;
using System.Linq;
using Parcel.Types;
using Parcel.Neo.Base.Serialization;
using System.Diagnostics;
using Parcel.CoreEngine.Helpers;
using Parcel.CoreEngine.Service.Interpretation;
using System.Numerics;

namespace Parcel.Neo.Base.Framework.ViewModels.BaseNodes
{
    /// <summary>
    /// An encapsulation of a base node instance that's generated directly from methods;
    /// We will start with only a single output but there shouldn't be much difficulty outputting more outputs
    /// </summary>
    public class AutomaticProcessorNode: ProcessorNode
    {
        #region Constructor
        private Dictionary<string, NodeSerializationRoutine>? GeneratedMemberSerializers = null;
        public AutomaticProcessorNode()
        {
            ProcessorNodeMemberSerialization = new Dictionary<string, NodeSerializationRoutine>()
            {
                {
                    nameof(Descriptor), new NodeSerializationRoutine(
                        () => SerializationHelper.Serialize(Descriptor.Method.GetRuntimeNodeTypeIdentifier()), 
                        value =>
                        {
                            string identifer = SerializationHelper.GetString(value);
                            Descriptor = ToolboxIndexer.LoadTool(identifer);
                            GeneratedMemberSerializers = InitializeNodeProperties(Descriptor).ToDictionary(s => s.Item1, s => s.Item2);
                        })
                },
            };
        }
        public AutomaticProcessorNode(FunctionalNodeDescription descriptor) :this()
        {
            Descriptor = descriptor;

            InitializeNodeProperties(descriptor);
        }
        internal void SecondStageDeserialization(Dictionary<string, byte[]> members)
        {
            foreach ((string key, byte[] data) in members)
                if (GeneratedMemberSerializers?.ContainsKey(key) ?? false)
                    GeneratedMemberSerializers[key].Deserialize(data);
        }
        #endregion

        #region Routines
        private Func<object[], object[]> RetrieveCallMarshal()
        {
            try
            {
                if (Descriptor != null)
                    return Descriptor.CallMarshal;
                else
                    throw new ApplicationException("Node is not properly initialized!");
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Failed to retrieve node: {e.Message}.");
            }
        }
        private (string, NodeSerializationRoutine)[] InitializeNodeProperties(FunctionalNodeDescription descriptor)
        {
            // Basic properties
            InputTypes = descriptor.InputTypes;
            DefaultInputValues = descriptor.DefaultInputValues;
            OutputTypes = descriptor.OutputTypes;
            InputNames = descriptor.InputNames;
            OutputNames = descriptor.OutputNames;

            // Node display initialization
            return PopulateInputsOutputs();
        }
        private (string, NodeSerializationRoutine)[] PopulateInputsOutputs()
        {
            (string, NodeSerializationRoutine)[] serializers = new (string, NodeSerializationRoutine)[InputTypes.Length]; 

            Title = NodeTypeName = Descriptor.NodeName;
            for (int index = 0; index < InputTypes.Length; index++)
            {
                Type inputType = InputTypes[index];
                object? defaultValue = DefaultInputValues?[index];
                string preferredTitle = InputNames?[index];
                InputConnector? connector = null;

                if (Nullable.GetUnderlyingType(inputType) != null)
                    connector = CreateInputPin(Nullable.GetUnderlyingType(inputType), defaultValue, preferredTitle); // TODO: Current implementation has issue making nullable default values as type default rather than null
                else
                    connector = CreateInputPin(inputType, defaultValue, preferredTitle);

                // Update serialization
                // TODO: (Remark-cz) At the moment this is not working yet because when deserialization happens connectors are not initialized yet and when PopulateInputsOutputs runs it's already past serialization
                NodeSerializationRoutine serializer = new NodeSerializationRoutine(
                    () => connector is PrimitiveInputConnector p ? p.SerializeStorage() : [],
                    bytes =>
                    {
                        if (connector is PrimitiveInputConnector p)
                            p.DeserializeStorage(bytes);
                    }
                );
                ProcessorNodeMemberSerialization.Add(preferredTitle, serializer);
                serializers[index] = (preferredTitle, serializer);
            }

            for (int index = 0; index < OutputTypes.Length; index++)
            {
                Type outputType = OutputTypes[index];
                string? preferredTitle = OutputNames == null ? GetPreferredTitle(outputType) : OutputNames?[index];
                Output.Add(new OutputConnector(outputType) { Title = preferredTitle ?? "Result" });
            }

            return serializers;

            InputConnector CreateInputPin(Type inputType, object? defaultValue, string preferredTitle)
            {
                bool supportsCoercion = inputType.IsArray; // TODO: Notice IsArray is potentially unsafe since it doesn't work on pass by ref arrays e.g. System.Double[]&; Consider using HasElementType

                InputConnector? connector = null;
                if (inputType == typeof(bool))
                    connector = new PrimitiveBooleanInputConnector(defaultValue != DBNull.Value ? (bool)defaultValue : null) { Title = preferredTitle ?? "Bool", AllowsArrayCoercion = supportsCoercion };
                else if (inputType == typeof(string))
                    connector = new PrimitiveStringInputConnector(defaultValue != DBNull.Value ? (string)defaultValue : null) { Title = preferredTitle ?? "String", AllowsArrayCoercion = supportsCoercion };
                else if (inputType.IsEnum)
                    connector = new PrimitiveEnumInputConnector(inputType, defaultValue != DBNull.Value ? defaultValue : null) { Title = preferredTitle ?? "Enum", AllowsArrayCoercion = supportsCoercion };
                else if (TypeHelper.IsNumericalType(inputType))
                    connector = new PrimitiveNumberInputConnector(inputType, defaultValue == DBNull.Value ? null : defaultValue) { Title = preferredTitle ?? "Number", AllowsArrayCoercion = supportsCoercion };
                else if (inputType == typeof(DateTime))
                    connector = new PrimitiveDateTimeInputConnector(defaultValue != DBNull.Value ? (DateTime)defaultValue : null) { Title = preferredTitle ?? "Date", AllowsArrayCoercion = supportsCoercion };
                else if (inputType == typeof(Color))
                    connector = new PrimitiveColorInputConnector(defaultValue != DBNull.Value ? (Color)defaultValue : null) { Title = preferredTitle ?? "Color", AllowsArrayCoercion = supportsCoercion };
                else if (inputType == typeof(Vector2))
                    connector = new PrimitiveVector2InputConnector(defaultValue != DBNull.Value ? (Vector2)defaultValue : null) { Title = preferredTitle ?? "Vector2", AllowsArrayCoercion = supportsCoercion };
                else if (inputType == typeof(System.Drawing.Size))
                    connector = new PrimitiveSizeInputConnector(defaultValue != DBNull.Value ? (System.Drawing.Size)defaultValue : null) { Title = preferredTitle ?? "Vector2", AllowsArrayCoercion = supportsCoercion };
                else
                    connector = new InputConnector(inputType) { Title = preferredTitle ?? "Input", AllowsArrayCoercion = supportsCoercion };

                Input.Add(connector);
                return connector;
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
        }
        #endregion

        #region Properties
        /// <remarks>
        /// Remark-cz: Hack we are saving descriptor here for easier invoking of dynamic types; However, this is not serializable at the moment! The reason we don't want it is because the descriptor itself is not serialized which means when the graph is loaded all such information is gone - and that's why we had IToolboxDefinition before.
        /// </remarks>
        public FunctionalNodeDescription Descriptor { get; private set; }

        private Type[] InputTypes { get; set; }
        private Type[] OutputTypes { get; set; }
        private object?[]? DefaultInputValues { get; set; }
        /// <remarks>
        /// For display purpose.
        /// </remarks>
        private string[]? InputNames { get; set; }
        /// <remarks>
        /// For display purpose.
        /// </remarks>
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
                object[] outputs = marshal.Invoke(Input.Select((input, index) =>
                {
                    if (input.AllowsArrayCoercion && !input.Connections.Any(c => c.Input.DataType.HasElementType)) //Remark: Notice IsArray is not robust enough since it doesn't work on pass by ref arrays e.g. System.Double[]&
                        return input.FetchArrayInputValues(InputTypes[index].GetElementType());
                    else 
                        return input.FetchInputValue<object>();
                }).ToArray());
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
        protected override NodeSerializationRoutine VariantInputConnectorsSerialization { get; } = null;
        #endregion

        #region Auto-Connect Interface
        public override bool ShouldHaveAutoConnection => Input.Count > 0 && Input.Any(InputConnectorShouldRequireAutoConnection);
        #endregion
    }
}