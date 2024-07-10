using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using MathNet.Numerics.LinearAlgebra.Factorization;
using Parcel.Neo.Base.Framework.ViewModels;

namespace Parcel.Neo.Base.Framework
{
    public interface IProcessor
    {
        /// <remarks>
        /// The concept of a "MainOutput" is needed mostly because we want to provide default Preview behavior.
        /// Conventionally, we could have just taken the first output pin as main output.
        /// </remarks>
        public OutputConnector MainOutput { get; }
        public void Evaluate();
        /// <remarks>
        /// Notice each node can have multiple outputs so it's essential that we provide cache at each output level.
        /// </remarks>
        public ConnectorCache this[OutputConnector cacheConnector] { get; }
        public bool HasCache(OutputConnector cacheConnector);
    }

    /// <summary>
    /// Automatic nodes provides a way to quickly define a large library of simple function nodes without explicitly defining classes for them
    /// </summary>
    public class AutomaticNodeDescriptor
    {
        #region Properties
        public string NodeName { get; }
        public Type[] InputTypes { get; }
        public object?[]? DefaultInputValues { get; }
        public Type[] OutputTypes { get; }
        public Func<object?[], object?[]> CallMarshal { get; }
        public string[]? InputNames { get; }
        public string[]? OutputNames { get; }
        #endregion

        #region Mapping
        public MethodInfo Method { get; }
        #endregion

        #region Construction
        public AutomaticNodeDescriptor(string nodeName, MethodInfo method)
        {
            // Parse definitions
            SeperateMethodParametersForNode(method, out (Type, string, object?)[] nodeInputTypes, out (Type, string)[] nodeOutputTypes);

            // Set properties
            NodeName = nodeName;
            Method = method;
            InputTypes = [.. nodeInputTypes.Select(p => p.Item1)];
            OutputTypes = [.. nodeOutputTypes.Select(p => p.Item1)];
            CallMarshal = nodeInputValues =>
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
                        // Automatic conversion of number values
                        if (nonOutMethodParameterValues[current].GetType() != item.ParameterType && TypeDescriptor.GetConverter(nonOutMethodParameterValues[current].GetType()).CanConvertTo(item.ParameterType)) // Requires IConvertible
                            methodExpectedParameterValuesArray[i] = Convert.ChangeType(nonOutMethodParameterValues[current], item.ParameterType);
                        else
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
                        return [instance, .. outParameterIndices.Select(i => methodExpectedParameterValuesArray[i])];
                    else
                        return [instance, returnValue, .. outParameterIndices.Select(i => methodExpectedParameterValuesArray[i])];
                }
            };
            InputNames = [.. nodeInputTypes.Select(t => t.Item2)];
            OutputNames = [.. nodeOutputTypes.Select(t => t.Item2)];
            DefaultInputValues = [.. nodeInputTypes.Select(t => t.Item3)];
        }
        #endregion

        #region Helper
        private static void SeperateMethodParametersForNode(MethodInfo method, out (Type, string, object?)[] nodeInputTypes, out (Type ValueType, string ValueName)[] nodeOutputTypes)
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
                nodeOutputTypesList.Add((method.DeclaringType!, FormalizeUnnammedPin(method.DeclaringType!))); // TODO: Pending POS standardization how should we name the return value pin by default
            }

            // Input order: (Instance), remaining (non-out) parameters
            foreach (ParameterInfo parameter in methodParameters.Where(p => !(p.IsOut && p.ParameterType.IsByRef)))
                nodeInputTypesList.Add((parameter.ParameterType, parameter.Name!, parameter.DefaultValue));

            // Output order: (Instance), (method return), any out parameters
            if (method.ReturnType != typeof(void))
                nodeOutputTypesList.Add((method.ReturnType, FormalizeUnnammedPin(method.ReturnType!)));
            foreach (ParameterInfo parameter in methodParameters.Where(p => (p.IsOut && p.ParameterType.IsByRef)))
                nodeOutputTypesList.Add((parameter.ParameterType, parameter.Name!));

            // Return
            nodeInputTypes = [.. nodeInputTypesList];
            nodeOutputTypes = [.. nodeOutputTypesList];

            static string FormalizeUnnammedPin(Type type)
            {
                string defaultName = type.Name;
                if (type.IsArray)
                    return $"{FormalizeUnnammedPin(type.GetElementType())} Array";
                else return defaultName;
            }
        }
        #endregion
    }
}