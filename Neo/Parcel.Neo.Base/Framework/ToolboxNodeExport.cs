using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;
using System;
using System.Linq;
using System.Reflection;

namespace Parcel.Neo.Base.Framework
{
    public class ToolboxNodeExport
    {
        private enum NodeImplementationType
        {
            PV1NativeFrontendImplementedGraphNode,
            MethodInfo
        }

        #region Attributes
        public string Name { get; }
        /// <summary>
        /// Documentation of node in human-readable text
        /// </summary>
        public string? Tooltip { get; set; }
        #endregion

        #region Cached Attributes
        public string ArgumentsListFull { get; }
        public string ArgumentsListSimple{ get; }
        public string ReturnsList { get; }
        public bool HasReturnValue { get; }
        public AutomaticNodeDescriptor Descriptor { get; }
        #endregion

        #region Payload Type
        private NodeImplementationType ImplementationType { get; }
        private MethodInfo Method { get; }
        private Type ProcessorNodeType { get; }
        #endregion

        #region Constructor
        public ToolboxNodeExport(string name, MethodInfo method)
        {
            Name = name;
            Method = method;
            ImplementationType = NodeImplementationType.MethodInfo;

            // Precomputed for access efficiency
            ArgumentsListFull = GetArgumentsListFull();
            ArgumentsListSimple = GetArgumentsListSimple();
            ReturnsList = GetReturnsList();
            HasReturnValue = GetHasReturnValue();
            Descriptor = new AutomaticNodeDescriptor(Name, Method);
        }
        public ToolboxNodeExport(string name, Type type)
        {
            Name = name;
            ProcessorNodeType = type;
            ImplementationType = NodeImplementationType.PV1NativeFrontendImplementedGraphNode;

            // Precomputed for access efficiency
            ArgumentsListFull = GetArgumentsListFull();
            ArgumentsListSimple = GetArgumentsListSimple();
            ReturnsList = GetReturnsList();
            HasReturnValue = GetHasReturnValue();
        }
        #endregion

        #region Method
        public BaseNode InstantiateNode()
        {
            switch (ImplementationType)
            {
                case NodeImplementationType.PV1NativeFrontendImplementedGraphNode:
                    // TODO: We can use automatic node to invoke constructors for types that have constructor
                    return (BaseNode)Activator.CreateInstance(ProcessorNodeType);
                case NodeImplementationType.MethodInfo:
                    return new AutomaticProcessorNode();
                default:
                    throw new ApplicationException("Invalid implementation type.");
            }
        }
        #endregion

        #region Accessor
        private string GetArgumentsListSimple()
        {
            switch (ImplementationType)
            {
                case NodeImplementationType.PV1NativeFrontendImplementedGraphNode:
                    var baseNode = (BaseNode)Activator.CreateInstance(ProcessorNodeType);
                    if (baseNode is ProcessorNode processor)
                        return string.Join(", ", processor.Input.Where(i => !i.IsHidden).Select(i => i.Title));
                    else return string.Empty;
                case NodeImplementationType.MethodInfo:
                    return string.Join(", ", Method.GetParameters().Select(p => (Nullable.GetUnderlyingType(p.ParameterType) != null ? $"{p.Name}?" : p.Name)));
                default:
                    throw new ArgumentOutOfRangeException($"Unrecognized implementation type: {ImplementationType}");
            }
        }
        private string GetArgumentsListFull()
        {
            switch (ImplementationType)
            {
                case NodeImplementationType.PV1NativeFrontendImplementedGraphNode:
                    var baseNode = (BaseNode)Activator.CreateInstance(ProcessorNodeType);
                    if (baseNode is ProcessorNode processor)
                        return string.Join(", ", processor.Input.Where(i => !i.IsHidden).Select(i => i.Title));
                    else return string.Empty;
                case NodeImplementationType.MethodInfo:
                    return string.Join(", ", Method.GetParameters().Select(p => (Nullable.GetUnderlyingType(p.ParameterType) != null ? $"{p.Name}:{p.ParameterType.Name}?" : $"{p.Name}:{p.ParameterType.Name}")));
                default:
                    throw new ArgumentOutOfRangeException($"Unrecognized implementation type: {ImplementationType}");
            }
        }
        private string GetReturnsList()
        {
            switch (ImplementationType)
            {
                case NodeImplementationType.PV1NativeFrontendImplementedGraphNode:
                    var baseNode = (BaseNode)Activator.CreateInstance(ProcessorNodeType);
                    if (baseNode is ProcessorNode processor)
                        return string.Join(", ", processor.Output.Where(o => !o.IsHidden).Select(i => i.Title));
                    else return string.Empty;
                case NodeImplementationType.MethodInfo:
                    return Method.ReturnType == typeof(void) ? string.Empty : Method.ReturnType.Name; // TODO: This is NOT complete return values; Implement handling of out parameters
                default:
                    throw new ArgumentOutOfRangeException($"Unrecognized implementation type: {ImplementationType}");
            }
        }
        private bool GetHasReturnValue()
        {
            switch (ImplementationType)
            {
                case NodeImplementationType.PV1NativeFrontendImplementedGraphNode:
                    var baseNode = (BaseNode)Activator.CreateInstance(ProcessorNodeType);
                    if (baseNode is ProcessorNode processor)
                        return processor.Output.Any();
                    else return false;
                case NodeImplementationType.MethodInfo:
                    return Method.ReturnType != typeof(void);
                default:
                    throw new ArgumentOutOfRangeException($"Unrecognized implementation type: {ImplementationType}");
            }
        }
        #endregion
    }
}