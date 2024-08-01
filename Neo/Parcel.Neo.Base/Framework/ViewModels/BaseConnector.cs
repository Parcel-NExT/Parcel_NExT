using System;
using System.Collections.Generic;
using System.Linq;
using Parcel.Types;
using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;
using Parcel.Neo.Base.DataTypes;
using System.ComponentModel;
using System.Numerics;
using Parcel.Neo.Base.Serialization;

namespace Parcel.Neo.Base.Framework.ViewModels
{
    /// <remarks>
    /// Storage is Boolean
    /// </remarks>
    public sealed class PrimitiveBooleanInputConnector : PrimitiveInputConnector
    {
        public PrimitiveBooleanInputConnector(bool? defaultValue = null) : base(typeof(bool))
            => Value = defaultValue ?? false;

        public override object Value 
        {  
            get => _defaultDataStorage;
            set => SetField(ref _defaultDataStorage, value is string s ? bool.Parse(s) : value); 
        }

        #region Serialization
        public override byte[] SerializeStorage()
            => SerializationHelper.Serialize((bool)_defaultDataStorage);
        public override void DeserializeStorage(in byte[] raw)
            => _defaultDataStorage = SerializationHelper.GetBool(raw);
        #endregion
    }

    /// <remarks>
    /// Storage is Vector2
    /// </remarks>
    public sealed class PrimitiveVector2InputConnector : PrimitiveInputConnector
    {
        public PrimitiveVector2InputConnector(Vector2? defaultValue = null) : base(typeof(Vector2))
            => Value = defaultValue ?? new(); // Struct cannot be null

        public override object? Value
        {
            get => _defaultDataStorage;
            set
            {
                if (value is string s)
                {
                    string[] parts = (s.Contains('x') ? s.Split('x') : s.Split(',')) // Separator can be either , or x
                        .Select(s => s.Trim())
                        .ToArray();
                    SetField(ref _defaultDataStorage, new Vector2(float.Parse(parts.First()), float.Parse(parts.Last())));
                }
                else 
                    SetField(ref _defaultDataStorage, (Vector2)value);
            }
        }

        public string ValueX
        {
            get => ((Vector2)_defaultDataStorage).X.ToString();
            set
            {
                var original = (Vector2)_defaultDataStorage;
                SetField(ref _defaultDataStorage, new Vector2(float.Parse(value), original.Y));
            }
        }
        public string ValueY
        {
            get => ((Vector2)_defaultDataStorage).Y.ToString();
            set
            {
                var original = (Vector2)_defaultDataStorage;
                SetField(ref _defaultDataStorage, new Vector2(original.X, float.Parse(value)));
            }
        }

        #region Serialization
        public override byte[] SerializeStorage()
            => SerializationHelper.Serialize((Vector2)_defaultDataStorage);
        public override void DeserializeStorage(in byte[] raw)
            => _defaultDataStorage = SerializationHelper.GetVector2(raw);
        #endregion
    }

    /// <remarks>
    /// Storage is DateTimSize
    /// </remarks>
    public sealed class PrimitiveSizeInputConnector : PrimitiveInputConnector
    {
        public PrimitiveSizeInputConnector(System.Drawing.Size? defaultValue = null) : base(typeof(System.Drawing.Size))
            => Value = defaultValue ?? new(); // Struct cannot be be null

        public override object? Value
        {
            get => _defaultDataStorage;
            set
            {
                if (value is string s)
                {
                    string[] parts = (s.Contains('x') ? s.Split('x') : s.Split(',')) // Separator can be either , or x
                        .Select(s => s.Trim())
                        .ToArray();
                    SetField(ref _defaultDataStorage, new System.Drawing.Size(int.Parse(parts.First()), int.Parse(parts.Last())));
                }
                else
                    SetField(ref _defaultDataStorage, (System.Drawing.Size)value);
            }
        }

        public string Width
        {
            get => ((System.Drawing.Size)_defaultDataStorage).Width.ToString();
            set
            {
                var original = (System.Drawing.Size)_defaultDataStorage;
                SetField(ref _defaultDataStorage, new System.Drawing.Size(int.Parse(value), original.Height));
            }
        }
        public string Height
        {
            get => ((System.Drawing.Size)_defaultDataStorage).Height.ToString();
            set
            {
                var original = (System.Drawing.Size)_defaultDataStorage;
                SetField(ref _defaultDataStorage, new System.Drawing.Size(original.Width, int.Parse(value)));
            }
        }

        #region Serialization
        public override byte[] SerializeStorage()
            => SerializationHelper.Serialize((System.Drawing.Size)_defaultDataStorage);
        public override void DeserializeStorage(in byte[] raw)
            => _defaultDataStorage = SerializationHelper.GetSize(raw);
        #endregion
    }

    /// <remarks>
    /// Storage is Color
    /// </remarks>
    public sealed class PrimitiveColorInputConnector : PrimitiveInputConnector
    {
        public PrimitiveColorInputConnector(Color? defaultValue = null) : base(typeof(Color))
            => Value = defaultValue; // Can be null

        public override object? Value
        {
            get => _defaultDataStorage;
            set => SetField(ref _defaultDataStorage, value is string s ? Color.Parse(s) : (Color)value);
        }

        #region Serialization
        public override byte[] SerializeStorage()
            => SerializationHelper.Serialize((Color)_defaultDataStorage);
        public override void DeserializeStorage(in byte[] raw)
            => _defaultDataStorage = SerializationHelper.GetColor(raw);
        #endregion
    }

    /// <remarks>
    /// Storage is DateTime
    /// </remarks>
    public sealed class PrimitiveDateTimeInputConnector : PrimitiveInputConnector
    {
        public PrimitiveDateTimeInputConnector(DateTime? defaultValue = null) : base(typeof(DateTime)) 
            => Value = defaultValue ?? DateTime.Now.Date;

        public override object Value 
        {  
            get => _defaultDataStorage;
            set => SetField(ref _defaultDataStorage, value is string s ? DateTime.Parse(s) : value); 
        }

        #region Serialization
        public override byte[] SerializeStorage()
            => SerializationHelper.Serialize((DateTime)_defaultDataStorage);
        public override void DeserializeStorage(in byte[] raw)
            => _defaultDataStorage = SerializationHelper.GetDateTime(raw);
        #endregion
    }

    /// <remarks>
    /// Storage is string
    /// </remarks>
    public sealed class PrimitiveStringInputConnector : PrimitiveInputConnector
    {
        public PrimitiveStringInputConnector(string? defaultValue = null) : base(typeof(string))
            => Value = defaultValue ?? string.Empty;

        #region Serialization
        public override byte[] SerializeStorage()
            => SerializationHelper.Serialize((string)_defaultDataStorage);
        public override void DeserializeStorage(in byte[] raw)
            => _defaultDataStorage = SerializationHelper.GetString(raw);
        #endregion
    }

    /// <remarks>
    /// Storage is int
    /// </remarks>
    public sealed class PrimitiveEnumInputConnector : PrimitiveInputConnector
    {
        #region View Binding
        private Dictionary<string, int> _options;
        public string[] Options
        {
            get => _options.Keys.ToArray();
        }
        #endregion

        #region Construction
        public PrimitiveEnumInputConnector(Type type, object? defaultValue = null) : base(type)
        {
            defaultValue ??= 0;

            if (!type.IsEnum)
                throw new ArgumentException($"Invalid type for enum input: {type.Name}");
            if (defaultValue is not int)
                throw new ApplicationException("Expect raw int value from enum.");

            string[] names = Enum.GetNames(type);
            int[] values = Enum.GetValues(type).Cast<int>().ToArray();
            _options = Enumerable.Range(0, names.Length).ToDictionary(i => names[i], i => values[i]);

            Value = defaultValue;
        }
        #endregion

        #region Value Setting
        public string DisplayValue
        {
            get => _options.First(o => o.Value == (int)_defaultDataStorage).Key;
            set => SetField(ref _defaultDataStorage, value is string s ? _options[value] : throw new ApplicationException());
        }

        /// <remarks>Stored value will be actual numerical value of the enum item (not its index)</remarks>
        public override object Value
        {
            get => _defaultDataStorage;
            set => SetField(ref _defaultDataStorage, value is string s ? TypeDescriptor.GetConverter(DataType).ConvertFromInvariantString(s) : value);
        }
        public override byte[] SerializeStorage()
            => SerializationHelper.Serialize((int)_defaultDataStorage);
        public override void DeserializeStorage(in byte[] raw)
            => _defaultDataStorage = SerializationHelper.GetInt(raw);
        #endregion
    }

    /// <remarks>
    /// Storage is double
    /// </remarks>
    public sealed class PrimitiveNumberInputConnector : PrimitiveInputConnector
    {
        public PrimitiveNumberInputConnector(Type type, object? defaultValue = null) : base(type)
        {
            if (!type.IsValueType)
                throw new ArgumentException($"Invalid type for number input: {type.Name}");
            Value = defaultValue ?? Activator.CreateInstance(type)!;
        }
        
        public override object Value 
        {  
            get => _defaultDataStorage;
            set => SetField(ref _defaultDataStorage, value is string s ? TypeDescriptor.GetConverter(DataType).ConvertFromInvariantString(s) : value); 
        }
        public override byte[] SerializeStorage()
            => SerializationHelper.Serialize((double)_defaultDataStorage);
        public override void DeserializeStorage(in byte[] raw)
            => _defaultDataStorage = SerializationHelper.GetDouble(raw);
    }
    
    public abstract class PrimitiveInputConnector : InputConnector
    {
        public virtual object? Value
        {
            get => _defaultDataStorage;
            set => SetField(ref _defaultDataStorage, value);
        }

        protected PrimitiveInputConnector(Type dataType) : base(dataType) { }
        public abstract byte[] SerializeStorage();
        public abstract void DeserializeStorage(in byte[] raw);
    }

    public class InputConnector : BaseConnector
    {
        public InputConnector(Type dataType) : base(dataType)
            => FlowType = ConnectorFlowType.Input;
    }

    public class OutputConnector : BaseConnector
    {
        public OutputConnector(Type dataType) : base(dataType)
            => FlowType = ConnectorFlowType.Output;
    }
    
    public class KnotConnector : BaseConnector
    {
        public KnotConnector(Type dataType) : base(dataType)
            => FlowType = ConnectorFlowType.Knot;
    }
    
    public abstract class BaseConnector: ObservableObject
    {
        #region View Properties
        private string _title;
        public string Title
        {
            get => _title;
            set => SetField(ref _title, value);
        }
        private bool _isConnected;
        public bool IsConnected
        {
            get => _isConnected;
            set => SetField(ref _isConnected, value);
        }
        private bool _isHidden;
        public bool IsHidden
        {
            get => _isHidden;
            set => SetField(ref _isHidden, value);
        }
        private Vector2D _anchor;
        public Vector2D Anchor
        {
            get => _anchor;
            set => SetField(ref _anchor, value);
        }
        private BaseNode _node = default!;
        public BaseNode Node
        {
            get => _node;
            internal set
            {
                if (SetField(ref _node, value))
                {
                    OnNodeChanged();
                }
            }
        }
        private ConnectorShape _shape;
        public ConnectorShape Shape
        {
            get => _shape;
            set => SetField(ref _shape, value);
        }
        #endregion

        #region Other Properties
        public ConnectorFlowType FlowType { get; protected set; }
        public int MaxConnections { get; set; } = int.MaxValue;
        public bool AllowsArrayCoercion { get; set; } = false;
        public NotifyObservableCollection<BaseConnection> Connections { get; } = [];
        
        public Type DataType { get; set; }
        /// <summary>
        /// Used for input nodes that haven't had any input yet
        /// </summary>
        public object DefaultDataStorage 
        { 
            get => _defaultDataStorage;
            set => SetField(ref _defaultDataStorage, value);
        }
        protected object _defaultDataStorage;
        #endregion

        #region Node Framework
        public BaseConnector(Type dataType)
        {
            DataType = dataType;
            Shape = DecideShape(dataType);
            
            Connections.WhenAdded(c =>
            {
                c.Input.IsConnected = true;
                c.Output.IsConnected = true;
            }).WhenRemoved(c =>
            {
                if (c.Input.Connections.Count == 0)
                {
                    c.Input.IsConnected = false;
                }

                if (c.Output.Connections.Count == 0)
                {
                    c.Output.IsConnected = false;
                }
            });
        }
        protected virtual void OnNodeChanged()
        {
            if (Node is ProcessorNode processorNode)
            {
                FlowType = processorNode.Input.Contains(this) ? ConnectorFlowType.Input : ConnectorFlowType.Output;
            }
            else if (Node is KnotNode knotNode)
            {
                FlowType = knotNode.Flow;
            }
        }
        public bool IsConnectedTo(BaseConnector connector)
            => Connections.Any(c => c.Input == connector || c.Output == connector);
        public virtual bool AllowsNewConnections(BaseConnector other)
        {
            if (FlowType != ConnectorFlowType.Input)
                // Output pins and knot pins allows infinite amount of connections
                return Connections.Count < MaxConnections; 
            else
            {
                // Input pins accepts either a single connection, or if it supports array type, we accept more than one connection only if all existing connections are of element type
                if (Connections.Count == 0) return true;
                else if (AllowsArrayCoercion)
                {
                    if (Connections.First().Input.DataType == DataType) // Already connected to an input with corresponding array type
                        return false;
                    else
                        return other.DataType != DataType;
                }
                else return false;
            }
        }
        public void Disconnect()
            => Node.Graph.Schema.DisconnectConnector(this);
        public void UpdateConnectorShape()
            => Shape = DecideShape(DataType);
        #endregion

        #region Interface
        public TReturn FetchInputValue<TReturn>()
        {
            if (FlowType != ConnectorFlowType.Input)
                throw new InvalidOperationException("Can't fetch value for output connector.");
            if (Connections.Count > 1)
                throw new InvalidOperationException("Input connector has more than 1 connection.");

            // Typical case, single value per parameter
            BaseConnection? connection = Connections.SingleOrDefault();
            return ExtractSingleValue<TReturn>(connection);
        }
        public object FetchArrayInputValues(Type elementType)
        {
            // Coercion for array or params input
            if (AllowsArrayCoercion)
            {
                Array array = Array.CreateInstance(elementType, Connections.Count);
                for (int i = 0; i < Connections.Count; i++)
                {
                    BaseConnection con = Connections[i];
                    object value = ExtractSingleValue(con, elementType);
                    array.SetValue(value, i);
                }
                return array;
            }
            else
                throw new ApplicationException("Unexpected case");
        }
        #endregion

        #region Routines
        private readonly Dictionary<Type, ConnectorShape> _mappings = new()
        {
            {typeof(bool), ConnectorShape.Triangle},
            {typeof(string), ConnectorShape.Circle},
            {typeof(double), ConnectorShape.Circle},
            {typeof(int), ConnectorShape.Circle},
            {typeof(DateTime), ConnectorShape.Circle},
            {typeof(DataGrid), ConnectorShape.Square},
            {typeof(ControlFlow), ConnectorShape.RightTriangle},
        };
        private ConnectorShape DecideShape(Type dataType)
        {
            if (_mappings.TryGetValue(dataType, out ConnectorShape value))
                return value;
            return ConnectorShape.Circle;
        }
        #endregion

        #region Helpers
        private T ExtractSingleValue<T>(BaseConnection? connection)
        {
            if (typeof(T) == DataType || typeof(T).IsAssignableFrom(DataType))
            {
                if (connection != null)
                {
                    if (connection.Input.Node is KnotNode search)
                    {
                        BaseNode prev = search;

                        while (prev is KnotNode knot)
                            prev = knot.Previous;

                        if (prev is ProcessorNode processor)
                            return (T)processor[connection.Input as OutputConnector].DataObject;

                        throw new InvalidOperationException("Knot nodes connect to empty source.");
                    }
                    else if (connection.Input.Node is ProcessorNode processor)
                    {
                        return (T)processor[connection.Input as OutputConnector].DataObject;
                    }
                    else throw new InvalidOperationException("Invalid node type.");
                }
                else
                {
                    if (this is OutputConnector _outputConnector && Node is ProcessorNode processor && processor.HasCache(_outputConnector))
                        return (T)processor[_outputConnector].DataObject;
                    else
                        return DefaultDataStorage != null ? (T)DefaultDataStorage : default(T);
                }
            }
            else throw new ArgumentException("Wrong type.");
        }
        private object ExtractSingleValue(BaseConnection? connection, Type valueType) // Runtime version of the generic version
        {
            if (connection != null)
            {
                if (connection.Input.Node is KnotNode search)
                {
                    BaseNode prev = search;

                    while (prev is KnotNode knot)
                        prev = knot.Previous;

                    if (prev is ProcessorNode processor)
                        return processor[connection.Input as OutputConnector].DataObject;

                    throw new InvalidOperationException("Knot nodes connect to empty source.");
                }
                else if (connection.Input.Node is ProcessorNode processor)
                {
                    return processor[connection.Input as OutputConnector].DataObject;
                }
                else throw new InvalidOperationException("Invalid node type.");
            }
            else
            {
                if (this is OutputConnector _outputConnector && Node is ProcessorNode processor && processor.HasCache(_outputConnector))
                    return processor[_outputConnector].DataObject;
                else
                    return DefaultDataStorage != null ? DefaultDataStorage : GetDefault(valueType);
            }

            static object GetDefault(Type type)
            {
                if (type.IsValueType)
                {
                    return Activator.CreateInstance(type);
                }
                return null;
            }
        }
        #endregion
    }
}