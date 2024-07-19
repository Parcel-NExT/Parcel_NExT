using System;
using System.Collections.Generic;
using System.Linq;
using Parcel.Types;
using Parcel.Neo.Base.Serialization;
using Parcel.Neo.Base.DataTypes;

namespace Parcel.Neo.Base.Framework.ViewModels.BaseNodes
{
    public interface IMainOutputNode
    {
        public OutputConnector MainOutput { get; }
    }
    public abstract class ProcessorNode: BaseNode, IProcessor, IMainOutputNode, IAutoConnect
    {
        #region Public View Properties - State
        private string _title;
        public string Title
        {
            get => !string.IsNullOrWhiteSpace(_title) ? _title : _nodeTypeName;
            set => SetField(ref _title, value);
        }
        private bool _isPreview;
        public bool IsPreview
        {
            get => _isPreview;
            set => SetField(ref _isPreview, value);
        }
        #endregion

        #region Public View Properties - Node Type
        private string _nodeTypeName;
        public string NodeTypeName
        {
            get => _nodeTypeName;
            set => SetField(ref _nodeTypeName, value);
        }
        private string _tooltip;
        public string Tooltip
        {
            get => _tooltip;
            set => SetField(ref _tooltip, value);
        }
        #endregion

        #region Public View Properties - Transient State
        private NodeMessage _message = new NodeMessage();
        public NodeMessage Message
        {
            get => _message;
            set => SetField(ref _message, value);
        }
        #endregion

        #region Accesor
        public string FullName => (!string.IsNullOrWhiteSpace(_title) && _title != _nodeTypeName) ? $"({_nodeTypeName}) {_title}" : _nodeTypeName;
        #endregion

        #region Connectors
        public NotifyObservableCollection<InputConnector> Input { get; } = [];
        public NotifyObservableCollection<OutputConnector> Output { get; } = [];
        #endregion

        #region Interface
        public ProcessorNode()
        {
            BaseProcessorMemberSerialization = new Dictionary<string, NodeSerializationRoutine>()
            {
                {nameof(Title), new NodeSerializationRoutine(() => SerializationHelper.Serialize(_title), value => _title = SerializationHelper.GetString(value))},
                {nameof(IsPreview), new NodeSerializationRoutine(() => SerializationHelper.Serialize(_isPreview), value => _isPreview = SerializationHelper.GetBool(value))},
            };
            
            Input.WhenAdded(c => c.Node = this)
                .WhenRemoved(c => c.Disconnect());

            Output.WhenAdded(c => c.Node = this)
                .WhenRemoved(c => c.Disconnect());
        }
        public void Disconnect()
        {
            Input.Clear();
            Output.Clear();
        }
        public virtual OutputConnector MainOutput => Output.Count == 0 ? null : Output[0];
        public void Evaluate()
        {
            NodeExecutionResult result = this.Execute();

            if (result.Message != null)
            {
                Message.Content = result.Message.Content;
                Message.Type = result.Message.Type;
            }
            
            if (result.Caches == null) return;
            foreach ((OutputConnector outputConnector, object value) in result.Caches)
                ProcessorCache[outputConnector] = value is ConnectorCache descriptor ? descriptor : new ConnectorCache(value);
        }
        public ConnectorCache this[OutputConnector cacheConnector] => ProcessorCache[cacheConnector];
        public bool HasCache(OutputConnector cacheConnector) => ProcessorCache.ContainsKey(cacheConnector);
        #endregion

        #region Routines
        protected abstract NodeExecutionResult Execute();
        private Dictionary<OutputConnector, ConnectorCache> ProcessorCache { get; } = [];
        #endregion

        #region Auto Connect Interface
        protected bool InputConnectorShouldRequireAutoConnection(InputConnector connector)
            => !IsPrimitiveInput(connector) && connector.Connections.Count == 0 &&
               connector.DataType != typeof(DataGrid); // Technically DataGrid connectors do need connection but we can't auto generate for it
        private static bool IsPrimitiveInput(InputConnector connector)
            => connector is PrimitiveInputConnector;
        public virtual bool ShouldHaveAutoConnection => Input.Count != 0 && Input.Any(InputConnectorShouldRequireAutoConnection);
        public virtual Tuple<ToolboxNodeExport, Vector2D, InputConnector>[] AutoPopulatedConnectionNodes
        {
            get
            {
                List<Tuple<ToolboxNodeExport, Vector2D, InputConnector>> auto = [];
                for (int i = 0; i < Input.Count; i++)
                {
                    if(!InputConnectorShouldRequireAutoConnection(Input[i])) continue;

                    ToolboxNodeExport toolDef = new(Input[i].Title, Input[i].DataType);
                    auto.Add(new Tuple<ToolboxNodeExport, Vector2D, InputConnector>(toolDef, new Vector2D(-180, -20 + (i - 1) * 50), Input[i]));
                }
                return [.. auto];
            }
        }

        #endregion

        #region Serialization
        public sealed override Dictionary<string, NodeSerializationRoutine> MemberSerialization =>
            BaseProcessorMemberSerialization.Select(d => d)
                .Union(ProcessorNodeMemberSerialization?.Select(d => d) ?? Array.Empty<KeyValuePair<string, NodeSerializationRoutine>>())
                .Union(VariantInputConnectorsSerialization != null
                    ? [new KeyValuePair<string, NodeSerializationRoutine>(nameof(VariantInputConnectorsSerialization), VariantInputConnectorsSerialization)]
                    : Array.Empty<KeyValuePair<string, NodeSerializationRoutine>>())
                .ToDictionary(d => d.Key, d => d.Value);
        private Dictionary<string, NodeSerializationRoutine> BaseProcessorMemberSerialization { get; }
        protected abstract Dictionary<string, NodeSerializationRoutine> ProcessorNodeMemberSerialization { get; }
        /// <remarks>
        /// It's not possible/not wise to generalize this further - because (specialized) nodes can have internal states that change when adding/removing dynamic
        /// inputs; As such both the number of inputs (and potentially outputs) and the default storage value associated with them are responsibilities of
        /// those derived ProcessorNodes. 
        /// </remarks>
        protected abstract NodeSerializationRoutine VariantInputConnectorsSerialization { get; }
        public override int GetOutputPinID(OutputConnector connector) => Output.IndexOf(connector);
        public override int GetInputPinID(InputConnector connector) => Input.IndexOf(connector);
        public override BaseConnector GetOutputPin(int id) => Output[id];
        public override BaseConnector GetInputPin(int id) => Input[id];
        #endregion
    }
}