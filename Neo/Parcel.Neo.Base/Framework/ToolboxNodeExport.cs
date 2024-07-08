using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;
using System;
using System.Collections.Generic;
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
                    // TODO: Replace with some more suitable implementation (e.g. a custom class specialized in handling those)
                    // TODO: Finish implementation; Likely we will require a new custom node descriptor type to handle this kind of behavior)) (Or maybe not)

                    SeperateMethodParametersForNode(Method, out (Type, string, object?)[] nodeInputTypes, out (Type, string)[] nodeOutputTypes);
                    return new AutomaticProcessorNode(new AutomaticNodeDescriptor(Name, [.. nodeInputTypes.Select(p => p.Item1)], [.. nodeOutputTypes.Select(p => p.Item1)], nodeInputValues =>
                    {
                        object[] nonOutMethodParameterValues = nodeInputValues.Skip(Method.IsStatic ? 0 : 1).ToArray();

                        int current = 0;
                        ParameterInfo[] declaredParameters = Method.GetParameters();
                        object?[] methodExpectedParameterValuesArray = new object[declaredParameters.Length];
                        List<int> outParameterIndices = [];
                        for (int i = 0; i < declaredParameters.Length; i++)
                        {
                            ParameterInfo item = declaredParameters[i];
                            if (item.IsOut && item.ParameterType.IsByRef) // `Out` parameter
                            {
                                methodExpectedParameterValuesArray[i] = null;
                                outParameterIndices.Add(i);
                            }
                            else
                            {
                                methodExpectedParameterValuesArray[i] = nonOutMethodParameterValues[current];
                                current++;
                            }
                        }
                        if (Method.IsStatic)
                        {
                            object? returnValue = Method.Invoke(null, methodExpectedParameterValuesArray);
                            if (Method.ReturnType == typeof(void))
                                return [.. outParameterIndices.Select(i => methodExpectedParameterValuesArray[i])];
                            else
                                return [returnValue, .. outParameterIndices.Select(i => methodExpectedParameterValuesArray[i])];
                        }
                        else
                        {
                            object? instance = nodeInputValues[0];
                            object? returnValue = Method.Invoke(instance, methodExpectedParameterValuesArray);
                            if (Method.ReturnType == typeof(void))
                                return [instance, ..outParameterIndices.Select(i => methodExpectedParameterValuesArray[i])];
                            else
                                return [instance, returnValue,..outParameterIndices.Select(i => methodExpectedParameterValuesArray[i])];
                        }
                    })
                    {
                        InputNames = [.. nodeInputTypes.Select(t => t.Item2)],
                        OutputNames = [.. nodeOutputTypes.Select(t => t.Item2)],
                        DefaultInputValues = [.. nodeInputTypes.Select(t => t.Item3)]
                    });
                default:
                    throw new ApplicationException("Invalid implementation type.");
            }
        }
        #endregion

        #region Helper
        private static void SeperateMethodParametersForNode(MethodInfo method, out (Type, string, object?)[] nodeInputTypes, out (Type, string)[] nodeOutputTypes)
        {
            ParameterInfo[] methodParameters = method.GetParameters();

            // Goal: Deal with instance methods, `out` (and don't worry about `ref`) parameters and method return type
            // Rules:
            // - Instance methods will have an artificial "instance" reference pin, like C/Python style "static" instance function
            // - Instance methods also automatically have an additional instance output pin (DO NOT provide fluent API in package implementation on the instance level!)
            // - `Ref` parameter is not touched
            // - `Out` parameter is treated as additional return value
            // Example calculation for clarity:
            //int nodeInputsCount = methodParameters.Where(p => !(p.IsOut && p.ParameterType.IsByRef) /* Disregard `out` parameters, keep `ref` */).Count() + (method.IsStatic ? 1 : 0);
            //int nodeOutputsCount = methodParameters.Where(p => (p.IsOut && p.ParameterType.IsByRef) /* Keep `out` parameters*/).Count() + (method.IsStatic ? 0 : 1);

            // Gather inputs and outputs
            List<(Type, string, object?)> nodeInputTypesList = [];
            List<(Type, string)> nodeOutputTypesList = [];

            if (!method.IsStatic)
            {
                nodeInputTypesList.Add((method.DeclaringType!, method.DeclaringType!.Name, null));
                nodeOutputTypesList.Add((method.DeclaringType!, method.DeclaringType!.Name));
            }

            // Input order: (Instance), remaining (non-out) parameters
            foreach (ParameterInfo parameter in methodParameters.Where(p => !(p.IsOut && p.ParameterType.IsByRef)))
                nodeInputTypesList.Add((parameter.ParameterType, parameter.Name!, parameter.DefaultValue));

            // Output order: (Instance), (method return), any out parameters
            if (method.ReturnType != typeof(void))
                nodeOutputTypesList.Add((method.ReturnType, method.ReturnType!.Name));
            foreach (ParameterInfo parameter in methodParameters.Where(p => (p.IsOut && p.ParameterType.IsByRef)))
                nodeOutputTypesList.Add((parameter.ParameterType, parameter.Name!));

            // Return
            nodeInputTypes = [.. nodeInputTypesList];
            nodeOutputTypes = [.. nodeOutputTypesList];
        }
        #endregion
    }
}