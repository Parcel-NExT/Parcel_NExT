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
        public string ArgumentsList
        {
            get
            {
                switch (ImplementationType)
                {
                    case NodeImplementationType.PV1NativeFrontendImplementedGraphNode:
                        var baseNode = (BaseNode)Activator.CreateInstance(ProcessorNodeType);
                        if (baseNode is ProcessorNode processor)
                            return string.Join(", ", processor.Input.Select(i => i.Title));
                        else return string.Empty;
                    case NodeImplementationType.MethodInfo: 
                        return string.Join(", ", Method.GetParameters().Select(p => (Nullable.GetUnderlyingType(p.ParameterType) != null ? $"{p.Name}?" : p.Name)));
                    default:
                        throw new ArgumentOutOfRangeException($"Unrecognized implementation type: {ImplementationType}");
                }
            }
        }
        /// <summary>
        /// Documentation of node in human-readable text
        /// </summary>
        public string? Tooltip { get; set; }
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
        }
        public ToolboxNodeExport(string name, Type type)
        {
            Name = name;
            ProcessorNodeType = type;
            ImplementationType = NodeImplementationType.PV1NativeFrontendImplementedGraphNode;
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
                    return new AutomaticProcessorNode(new AutomaticNodeDescriptor(Name, Method));
                default:
                    throw new ApplicationException("Invalid implementation type.");
            }
        }
        #endregion
    }
}