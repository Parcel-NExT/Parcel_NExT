using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using Parcel.CoreEngine.Helpers;
using Parcel.CoreEngine.Versioning;
using Parcel.Neo.Base.Framework.Advanced;
using Parcel.Neo.Base.Framework.ViewModels;
using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;
using static Parcel.Neo.Base.Algorithms.AlgorithmHelper.ScriptDependencySummary;

namespace Parcel.Neo.Base.Algorithms
{
    public static class AlgorithmHelper
    {
        /// <summary>
        /// Detect cyclic connection
        /// </summary>
        public static bool FindSelf(BaseNode startingNode, BaseNode self)
        {
            if (self == startingNode)
                return true;
            
            if (self is KnotNode knot)
            {
                IEnumerable<BaseNode> outConnections = knot.Next;
                return outConnections.Any(c => FindSelf(startingNode, c));
            }
            else if (self is ProcessorNode processor)
            {
                var outConnections = processor.Output.SelectMany(o => o.Connections)
                    .Where(c => c.Output.IsConnected)
                    .Select(c => c.Output.Node);
                return outConnections.Any(c => FindSelf(startingNode, c));
            }
            else throw new ArgumentException("Invalid node type");
        }

        public static void ExecuteGraph(NodesCanvas canvas)
        {
            IEnumerable<ProcessorNode> processors = canvas.Nodes
                .Where(n => n is ProcessorNode node && node.IsPreview == true)
                .Select(n => n as ProcessorNode);
            
            IExecutionGraph graph = new ExecutionQueue();
            graph.InitializeGraph(processors);
            graph.ExecuteGraph();
        }

        #region Code Gen (Draft)
        public static void CompileGraphAOT(string saveFilePath, NodesCanvas canvas)
        {
            IEnumerable<ProcessorNode> processors = canvas.Nodes
                .Where(n => n is ProcessorNode node)
                .Select(n => n as ProcessorNode);

            // Generate scripts (a script contains functions and other information, a function contains sections and other information)
            ExecutionQueue graph = new();
            graph.InitializeGraph(processors);

            foreach (ProcessorNode item in graph.Queue)
            {
                
            }

            // Generate code files

            // Initialize C# proejct

            // Compile and validate

            // Copy compiled result to destination path
        }
        public static void GenerateGraphPureScripts(string folderPath, string mainScriptFilename, NodesCanvas canvas)
        {
            // Generate scripts (a script contains functions and other information, a function contains sections and other information)

            // Filter executable nodes
            IEnumerable<ProcessorNode> processors = canvas.Nodes
                .Where(n => n is ProcessorNode node)
                .Select(n => n as ProcessorNode);

            // Fetch dependency graph
            ExecutionQueue graph = new();
            graph.InitializeGraph(processors);

            // Gather essential information
            ScriptDependencySummary summary = GatherScriptDependencies(graph, new PureSyntaxHandler());

            // Pre-build scripts
            StringBuilder mainSection = new();
            foreach (string line in summary.ScriptSectionStatements)
                mainSection.AppendLine($"{line};");
            StringBuilder[] scriptSections = [mainSection];

            // Generate script contents
            StringBuilder mainScriptBuilder = new();
            // Import package references
            foreach ((string importName, string nickName) in summary.StandardPackageImports)
                mainScriptBuilder.AppendLine($"Import({importName})");
            mainScriptBuilder.AppendLine();
            // Make necessary namespace usage and static usage
            // Style preference: if the script is very short, use static, otherwise, use type name based addressing.
            if (summary.UniqueTypes.Count() < 10)
                foreach (Type type in summary.InvolvedStaticTypes)
                    mainScriptBuilder.AppendLine($"using static {type.FullName};");
            else
                foreach (string uniqueNamespace in summary.UniqueNamespaces.Select(n => n.Namespace).Distinct())
                    mainScriptBuilder.AppendLine($"using {uniqueNamespace};");
            mainScriptBuilder.AppendLine();
            // Do variable declarations first
            foreach ((TypedVariable key, string value) in summary.VariableDeclarations)
                mainScriptBuilder.AppendLine($"{key.Type.Name} {key.Name} = {value};");
            // Append script sections
            foreach (StringBuilder section in scriptSections)
            {
                mainScriptBuilder.Append(section.ToString().TrimEnd());
                mainScriptBuilder.AppendLine();
            }

            // Create output folder if not exist
            Directory.CreateDirectory(folderPath);

            // Save main script
            string mainScript = $"{GetPureScriptGeneratedHeader()}\n{mainScriptBuilder.ToString().TrimEnd()}";
            File.WriteAllText(Path.Combine(folderPath, mainScriptFilename), mainScript);

            // TODO: Implement generating other script files
            // ...

            static string GetPureScriptGeneratedHeader()
            {
                return """
                    /* Generated Script File Header
                    This file is generated with Parcel NExT CodeGen for Pure 2.
                    You should have Pure installed in order to execute this script; 
                    You can downoad Pure at https://methodox.io/Pure. Notice to run the script you must make sure all `Import()`
                    packages are available under your environment `PATH` variable.

                    Alternatively, if you are using PV1 Neo you can export a .csproj C# project to be used in Visual Studio,
                    or if you graph doesn't involve advanced features like reflection, 
                    you can export a native executable file directly from the GUI.
                    */
                    """;
            }
        }
        public static void GenerateGraphPythonScripts(string folderPath, string mainScriptFilename, NodesCanvas canvas)
        { 
            const string defaultIndentation = "    "; // Python default indentation is 4 spaces

            // TODO: Below code definitely is worth some refactoring

            // Filter executable nodes
            IEnumerable<ProcessorNode> processors = canvas.Nodes
                .Where(n => n is ProcessorNode node)
                .Select(n => n as ProcessorNode);

            // Fetch dependency graph
            ExecutionQueue graph = new();
            graph.InitializeGraph(processors);
            // Gather essential information
            ScriptDependencySummary summary = GatherScriptDependencies(graph, new PythonSyntaxHandler());

            // Pre-build scripts
            StringBuilder mainSection = new();
            foreach(var line in summary.ScriptSectionStatements)
                mainSection.AppendLine(line);
            StringBuilder[] scriptSections = [mainSection];

            // Generate script contents
            StringBuilder mainScriptBuilder = new();
            // Main import
            mainScriptBuilder.AppendLine("""
                # Import Root Parcel NExT Module
                from ParcelNExT import LoadPackage

                """);
            // Import package references
            mainScriptBuilder.AppendLine("# Load Parcel NExT packages");
            foreach ((string importName, string nickName) in summary.StandardPackageImports)
                mainScriptBuilder.AppendLine($"LoadPackage('{importName}')"); // TODO: Add brief summary of package description if available
            if (!summary.StandardPackageImports.ContainsKey("Parcel.Standard")) // Import console print
                mainScriptBuilder.AppendLine("LoadPackage('Parcel.Standard')");
            mainScriptBuilder.AppendLine();
            // Import static types from corresponding namespaces; Notice Pythonnet doesn't support using static methods at the top level
            mainScriptBuilder.AppendLine("# Import submodules");
            foreach (Type type in summary.InvolvedStaticTypes)
                mainScriptBuilder.AppendLine($"from {type.Namespace} import {type.Name}"); // TODO: Add brief summary of class description if available
            if (!summary.StandardPackageImports.ContainsKey("Parcel.Standard")) // Import console print
                mainScriptBuilder.AppendLine("from Parcel.Standard.System import Console");
            mainScriptBuilder.AppendLine();
            // Additional imports
            mainScriptBuilder.AppendLine("""
                # Additional system imports
                import sys
                import textwrap

                """);
            // Main function
            TypedVariable[] programInputs = [.. summary.GraphInputs];
            mainScriptBuilder.AppendLine("# Main script content");
            mainScriptBuilder.AppendLine($"def Main({string.Join(", ", programInputs.Select(p => p.Name))}):{(summary.GraphOutputs.Count > 0 ? $" # With {summary.GraphOutputs.Count} {(summary.GraphOutputs.Count > 1 ? "outputs" : "output")}: {string.Join(", ", summary.GraphOutputs)}" : string.Empty)}");
            StringBuilder bodyBuilder = new();
            // Do variable declarations first
            foreach ((TypedVariable key, string value) in summary.VariableDeclarations)
                bodyBuilder.AppendLine($"{defaultIndentation}{key.Name} = {value}");
            // Append script sections
            foreach (StringBuilder section in scriptSections)
            {
                bodyBuilder.Append(Regex.Replace(section.ToString().TrimEnd(), @"^", defaultIndentation, RegexOptions.Multiline)); // Apply line indentation
                bodyBuilder.AppendLine();
            }
            string functionBody = bodyBuilder.ToString().TrimEnd();
            mainScriptBuilder.AppendLine(string.IsNullOrWhiteSpace(functionBody) ? $"{defaultIndentation}return" : functionBody);
            mainScriptBuilder.AppendLine();
            // Help Function
            mainScriptBuilder.AppendLine(""""
                def PrintHelp():
                    helpMessage = """
                        PENDING (Fetch from graph document metadata and generated from graph inputs usages)
                        """
                    Console.Print(textwrap.dedent(helpMessage).strip())
                """");
            mainScriptBuilder.AppendLine();
            // Entry point
            mainScriptBuilder.AppendLine("# Program entry");
            if (programInputs.Length == 0)
                // Zero arguments
                mainScriptBuilder.AppendLine("""
                    if __name__ == '__main__':
                        if len(sys.argv) > 1 and sys.argv[1] == "--help":
                            PrintHelp()
                        else:
                            Main()
                    """);
            else
                // Handle command line arguments
                mainScriptBuilder.AppendLine($"""
                    if __name__ == '__main__':
                        if len(sys.argv) > 1 and sys.argv[1] == "--help":
                            PrintHelp()
                        else:
                            if len(sys.argv) != 1 + {programInputs.Length}:
                                Console.Print("Missing required arguments.")
                            else:
                                {string.Join("\n            ", programInputs.Select((v, i) => $"{v.Name} = {(v.RepresentsNumber ? $"float(sys.argv[{1 + i}])" : $"sys.argv[{1 + i}]")}"))}
                                Main({string.Join(", ", programInputs.Select(p => p.Name))}){(summary.GraphOutputs.Count > 0 ? " # The program generates outputs; Somehow make use of outputs..." : string.Empty)}
                    """);

            // Create output folder if not exist
            Directory.CreateDirectory(folderPath);

            // Save main script
            string mainScript = $"{GetPureScriptGeneratedHeader()}\n{mainScriptBuilder.ToString().TrimEnd()}";
            File.WriteAllText(Path.Combine(folderPath, mainScriptFilename), mainScript);

            // TODO: Implement generating other script files
            // ...

            static string GetPureScriptGeneratedHeader()
            {
                return $""""
                    """
                    Parcel NExT Executable Workflow (Version {EngineVersion.Version})
                    This python script is generated with Parcel NExT CodeGen feature.

                    You should have Python installed in order to execute this script; 
                    You can downoad Python at https://www.python.org/downloads/. We recommend latest version.
                    Notice to run the script you must make sure all essential dependencies are available under PYTHONPATH.

                    Use `pip install pythonnet` to install essential dependency.
                    Requires .NET Core to be installed or the `dotnet` CLI tool available on PATH.
                    You can download .Net 8 SDK at https://dotnet.microsoft.com/en-us/download/dotnet/8.0.
                    """

                    """"; // TODO: Publish Parcel NExT as a standalone python package
            }
        }
        // TODO: Do not handle statement generation inside this class; Handle it in a dedicated place instead
        public sealed class ScriptDependencySummary
        {
            #region Construction
            public ScriptDependencySummary(string subGraphID, ExecutionQueue graph, ISyntaxHandler syntaxHandler)
            {
                SubgraphID = subGraphID;
                GatherScriptDependencies(graph, syntaxHandler);
            }
            #endregion

            #region Properties
            public string SubgraphID { get; }
            public HashSet<TypedVariable> GraphInputs { get; } = [];
            public HashSet<string> GraphOutputs { get; } = [];
            public Dictionary<TypedVariable, string> VariableDeclarations { get; } = [];
            public HashSet<TypedVariable> ScopedVariables { get; } = [];
            public string[] ScriptSectionStatements { get; private set; } = [];
            public (string PackageID, string Namespace)[] UniqueNamespaces { get; private set; }
            public Dictionary<string, string> StandardPackageImports { get; private set; }
            public Assembly[] UniqueAssemblies { get; private set; }
            public Type[] UniqueTypes { get; private set; }
            public Type[] InvolvedStaticTypes => UniqueTypes.Where(t => t.IsAbstract && t.IsSealed).ToArray();
            #endregion

            public record NodeHandlingResult(bool IsVariable = false, Dictionary<string, TypedVariable>? Variables = null /*From attribute to final scoped variable name*/);
            public record FunctionCallParameter(string OriginalName, Type OriginalType, string FinalEvaluatedValue, bool IsVariableReference, Type? ReferencedVariableType);

            #region Method
            private void GatherScriptDependencies(in ExecutionQueue graph, ISyntaxHandler syntaxHandler)
            {
                // TODO: This code definitely is worth some refactoring

                List<AutomaticProcessorNode> automaticProcessors = [];
                // TOOD: Special handle math nodes that have corresponding C# operators: +-*/
                List<string> statements = [];
                Dictionary<ProcessorNode, NodeHandlingResult> handledNodes = [];
                var enumeration = graph.Queue.Where(n => n is not GraphOutput).Concat(graph.Queue.Where(n => n is GraphOutput));
                foreach (ProcessorNode processorNode in enumeration) // Enumerate in the execution order, keeping GraphOuts at the end
                {
                    // Remark-cz: notice as of PV1 Neo we no longer have c#-lambda impelemented nodes; Everything is eitehr completely front-end implemented as ProcessorNode or it's C# defined in packages

                    // Deal with package functions
                    if (processorNode is AutomaticProcessorNode autoNode)
                    {
                        automaticProcessors.Add(autoNode);
                        Type[] parameterTypes = autoNode.Descriptor.InputTypes;
                        FunctionCallParameter[] parameters = Enumerable.Range(0, autoNode.Input.Count).Select(i => new FunctionCallParameter(autoNode.Input[i].Title, parameterTypes[i], autoNode.Input[i].Title, false, null)).ToArray();
                        string methodCallName = $"{(autoNode.Descriptor.Method.IsStatic ? autoNode.Descriptor.Method.DeclaringType.Name + ".": string.Empty)}{autoNode.Descriptor.NodeName}"; // TODO: We do not need to address full type name if we are using static (that's why we should not handle statement generation directly here and just parse essential information and let the actual code generation for specific target languages (pure vs c# vs python) handle it

                        // Translate parameters
                        string[] parameterLiteralValues = Enumerable.Range(0, parameters.Length)
                            .Select(i =>
                            {
                                var connector = autoNode.Input[i];
                                object? value = null;
                                if (connector is PrimitiveInputConnector primitive)
                                    value = primitive.Value;
                                else value = autoNode.Descriptor.DefaultInputValues[i];
                                if (TypeHelper.IsNumericalType(value.GetType()))
                                    return value.ToString();
                                return $"\"{value}\"";
                            })
                            .ToArray(); // Get actual input values instead of assigned default values from function signature, because user may have changed it on the node surface (e.g. primitive number inputs connectors)
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            FunctionCallParameter parameter = parameters[i];
                            NotifyObservableCollection<BaseConnection> connections = autoNode.Input[i].Connections;
                            if (connections.Any())
                            {
                                // Typical singular argument
                                if (connections.Count == 1)
                                {
                                    BaseConnector connection = connections.Single().Input;
                                    TypedVariable variableReference = GetVariableReference(handledNodes, connection);
                                    parameters[i] = new FunctionCallParameter(parameters[i].OriginalName, parameters[i].OriginalType, variableReference.Name, true, variableReference.Type);
                                }
                                // Array coercion handling
                                else
                                {
                                    TypedVariable[] variableReferences = connections.Select(con => GetVariableReference(handledNodes, con.Input)).ToArray();
                                    string arrayVariableName = GetNewVariableName(ScopedVariables, parameters[i].OriginalName);
                                    TypedVariable newVariable = new(arrayVariableName, autoNode.Input[i].DataType);
                                    statements.Add(syntaxHandler.CreateArrayVariable(newVariable, variableReferences.Select(v => v.Name).ToArray()));
                                    parameters[i] = new FunctionCallParameter(parameters[i].OriginalName, parameters[i].OriginalType, newVariable.Name, true, newVariable.Type);
                                    ScopedVariables.Add(newVariable);
                                }
                            }
                            else
                                // Use literal value from the pin
                                parameters[i] = new FunctionCallParameter(parameters[i].OriginalName, parameters[i].OriginalType, parameterLiteralValues[i], false, null);
                        }

                        // Save outputs
                        string[] parameterNames = autoNode.Descriptor.InputNames;
                        TypedVariable[] functionCallArguments = Enumerable.Range(0, autoNode.Descriptor.InputTypes.Length).Select(i => new TypedVariable(parameterNames[i], parameterTypes[i])).ToArray();
                        if (autoNode.Output.Any())
                        {
                            // TODO: Deal with multiple outputs
                            string outputVariableName = GetNewVariableName(ScopedVariables, autoNode.MainOutput.Title.Camelize());
                            TypedVariable newVariable = new(outputVariableName, autoNode.MainOutput.DataType);
                            statements.Add(syntaxHandler.CreateVariable(newVariable, syntaxHandler.CallFunction(methodCallName, functionCallArguments, parameters)));
                            ScopedVariables.Add(newVariable);

                            // Book keep node outputs
                            handledNodes[processorNode] = new(true, new Dictionary<string, TypedVariable> { { autoNode.MainOutput.Title, newVariable } });
                        }
                        // Plain call
                        else
                        {
                            statements.Add(syntaxHandler.CallFunction(methodCallName, functionCallArguments, parameters));
                            handledNodes[processorNode] = new();
                        }
                    }
                    // Deal with front-end implemented nodes
                    else
                    {
                        // Primitives are processed as variable definition
                        if (processorNode is PrimitiveNode primitive)
                        {
                            string variableName = GetNewVariableName(ScopedVariables, processorNode.Title.Camelize());
                            TypedVariable newVariable = new(variableName, processorNode.MainOutput.DataType);
                            VariableDeclarations[newVariable] = newVariable.IsStringLiteral ? $"\"{primitive.Value}\"" : primitive.Value; // TODO: Instead of using MainOutput which depdends on cache which requires us to execute the graph, we should fetch directly its stored values.
                            ScopedVariables.Add(newVariable);
                            handledNodes[processorNode] = new(true, new Dictionary<string, TypedVariable> { { primitive.MainOutput.Title, newVariable } });
                        }
                        else if (processorNode is GraphInput graphInput)
                        {
                            foreach (OutputConnector output in graphInput.Output)
                            {
                                string inputVariableName = GetNewVariableName(ScopedVariables, output.Title.Camelize());
                                TypedVariable newVariable = new(inputVariableName, output.DataType);
                                GraphInputs.Add(newVariable);
                                ScopedVariables.Add(newVariable);
                            }
                            handledNodes[processorNode] = new(true, graphInput.Output.ToDictionary(o => o.Title, o => new TypedVariable(o.Title.Camelize(), o.DataType)));
                        }
                        else if (processorNode is GraphOutput graphOutput)
                        {
                            List<string> outputVariables = [];
                            foreach (InputConnector input in graphOutput.Input)
                            {
                                string outputVariableName = input.Title.Camelize();
                                GraphOutputs.Add(outputVariableName);

                                // Resolve value
                                string outputVariableValue = ""; // TODO: Fetch default value
                                if (input.Connections.Any())
                                {
                                    BaseConnector connection = input.Connections.Single().Input;
                                    ProcessorNode source = (connection.Node as ProcessorNode)!;
                                    if (!handledNodes.TryGetValue(source, out NodeHandlingResult? connectedNodeResult))
                                        throw new ApplicationException("Source should have already been handled.");
                                    if (!connectedNodeResult.IsVariable)
                                        throw new ApplicationException("Source should have generated some outputs.");
                                    outputVariableValue = connectedNodeResult.Variables[connection.Title].Name;
                                }
                                // Generate statement
                                statements.Add(syntaxHandler.CreateVariable(new TypedVariable(outputVariableName, input.DataType), outputVariableValue));
                                outputVariables.Add(outputVariableName);
                                // No need to add this to ScopedVariables
                            }

                            // Final return statement
                            statements.Add(syntaxHandler.ReturnResults([.. outputVariables]));

                            handledNodes[processorNode] = new();
                        }
                    }
                }

                // Gather unique dependent assemblies
                UniqueTypes = automaticProcessors.Select(p => p.Descriptor.Method.DeclaringType).Distinct().ToArray(); // TODO: Not sure whether type comparison in Disctin() works as expected
                UniqueAssemblies = automaticProcessors.Select(p => p.Descriptor.Method.DeclaringType.Assembly).Distinct().ToArray();
                UniqueNamespaces = automaticProcessors.Select(p => p.Descriptor.Method.DeclaringType)
                    .GroupBy(t => new ValueTuple<string, string>(t.Assembly.GetName().Name, t.Namespace))
                    .Select(g => g.Key)
                    .ToArray();
                StandardPackageImports = UniqueAssemblies.Select(a => a.CodeBase)
                    .Select(codeBase => Uri.UnescapeDataString(new UriBuilder(codeBase).Path))
                    .Where(File.Exists)
                    .Select(FindStandardPackageFriendlyName)
                    .ToDictionary(n => n, n => n.Split('.').Last());

                // Final assignments
                ScriptSectionStatements = [.. statements];
            }
            private static string GetNewVariableName(HashSet<TypedVariable> scope, string preferredName)
            {
                int counter = 0;
                while (true)
                {
                    string finalName = counter == 0 ? preferredName : $"{preferredName}{counter}";
                    if (scope.Any(s => s.Name == finalName))
                        counter++;
                    else
                        return finalName;
                }
            }
            private static TypedVariable GetVariableReference(Dictionary<ProcessorNode, NodeHandlingResult> handledNodes, BaseConnector connection)
            {
                ProcessorNode source = (connection.Node as ProcessorNode)!;
                if (!handledNodes.TryGetValue(source, out NodeHandlingResult? connectedNodeResult))
                    throw new ApplicationException("Source should have already been handled.");
                if (!connectedNodeResult.IsVariable)
                    throw new ApplicationException("Source should have generated some outputs.");
                TypedVariable variableReference = connectedNodeResult.Variables[connection.Title];
                return variableReference;
            }
            #endregion
        }
        private static ScriptDependencySummary GatherScriptDependencies(in ExecutionQueue graph, ISyntaxHandler syntaxHandler)
            => new("Main Script", graph, syntaxHandler);
        #endregion

        #region Helpers
        private static string FindStandardPackageFriendlyName(string filePath)
        {
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
            if (fileNameWithoutExtension.StartsWith("Parcel."))
                return fileNameWithoutExtension;
            else return filePath;
        }
        #endregion
    }

    public record struct TypedVariable(string Name, Type Type)
    {
        public bool RepresentsNumber = TypeHelper.IsNumericalType(Type);
        public bool IsStringLiteral = Type == typeof(string);
    }
    public interface ISyntaxHandler
    {
        public string CreateVariable(TypedVariable variable, string value);
        public string AssignVariable(TypedVariable variable, string value);
        public string CreateArrayVariable(TypedVariable variable, string[] elementVariableNames);
        public string CallFunction(string functionName, TypedVariable[] parameterNames, FunctionCallParameter[] parameters);
        public string ReturnResults(string[] returns);
    }
    public sealed class PythonSyntaxHandler : ISyntaxHandler
    {
        public string CreateVariable(TypedVariable variable, string value)
            => $"{variable.Name} = {value}";
        public string AssignVariable(TypedVariable variable, string value)
            => $"{variable.Name} = {value}";
        public string CreateArrayVariable(TypedVariable variable, string[] elementVariableNames)
            => $"{variable.Name} = [{string.Join(", ", elementVariableNames)}]";
        public string CallFunction(string functionName, TypedVariable[] parameterNames, FunctionCallParameter[] parameters)
            => $"{functionName}({string.Join(", ", Enumerable.Range(0, parameters.Length).Select(i =>
            {
                TypedVariable expectedType = parameterNames[i];
                FunctionCallParameter parameter = parameters[i];
                if (expectedType.Type != (parameter.IsVariableReference ? parameter.ReferencedVariableType : parameter.OriginalType))
                    return $"({expectedType.Type.Name}){parameter.FinalEvaluatedValue}"; // Add a cast
                return parameter.FinalEvaluatedValue;
            }))})";
        public string ReturnResults(string[] returns)
            => returns.Any() ? $"return {string.Join(", ", returns)}" : "return";
    }
    public sealed class PureSyntaxHandler : ISyntaxHandler
    {
        public string CreateVariable(TypedVariable variable, string value)
            => $"{variable.Type.Name} {variable.Name} = {value};";
        public string AssignVariable(TypedVariable variable, string value)
            => $"{variable.Name} = {value};";
        public string CreateArrayVariable(TypedVariable variable, string[] elementVariableNames)
            => $"{variable.Type.Name} {variable.Name}{variable.Name} = [{string.Join(", ", elementVariableNames)}];";
        public string CallFunction(string functionName, TypedVariable[] parameterNames, FunctionCallParameter[] parameters)
            => $"{functionName}({string.Join(", ", parameters.Select(p => p.FinalEvaluatedValue))});";
        public string ReturnResults(string[] returns)
            => returns.Any() ? $"return {(returns.Length == 1 ? returns.Single() : $"({string.Join(", ", returns)})")};" : "return;";
    }
}