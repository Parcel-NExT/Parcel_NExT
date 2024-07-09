using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Parcel.Neo.Base.Framework.ViewModels;
using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;

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
        public static void GenerateGraphScripts(string folderPath, string mainScriptFilename, NodesCanvas canvas)
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
            GatherScriptDependencies(graph, out Dictionary<string, string> variableDeclarations, out List<string> mainScriptSection, out (string Package, string Namespace)[] uniqueNamespaces, out Dictionary<string, string> standardPackageImports);

            // Pre-build scripts
            StringBuilder mainSection = new();
            foreach (var line in mainScriptSection)
                mainSection.AppendLine($"{line};");
            StringBuilder[] scriptSections = [mainSection];

            // Generate script contents
            StringBuilder mainScriptBuilder = new();
            // Import package references
            foreach ((string importName, string nickName) in standardPackageImports)
                mainScriptBuilder.AppendLine($"Import({importName})");
            mainScriptBuilder.AppendLine();
            // Make necessary namespace usage and static usage
            foreach (string uniqueNamespace in uniqueNamespaces.Select(n => n.Namespace).Distinct())
                mainScriptBuilder.AppendLine($"using {uniqueNamespace};");
            mainScriptBuilder.AppendLine();
            // Do variable declarations first
            foreach ((string key, string value) in variableDeclarations)
                mainScriptBuilder.AppendLine($"var {key} = {value};");
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

            // Filter executable nodes
            IEnumerable<ProcessorNode> processors = canvas.Nodes
                .Where(n => n is ProcessorNode node)
                .Select(n => n as ProcessorNode);

            // Fetch dependency graph
            ExecutionQueue graph = new();
            graph.InitializeGraph(processors);
            // Gather essential information
            GatherScriptDependencies(graph, out Dictionary<string, string> variableDeclarations, out List<string> mainScriptSection, out (string Package, string Namespace)[] uniqueNamespaces, out Dictionary<string, string> standardPackageImports);

            // Pre-build scripts
            StringBuilder mainSection = new();
            foreach(var line in mainScriptSection)
                mainSection.AppendLine(line);
            StringBuilder[] scriptSections = [mainSection];

            // Generate script contents
            StringBuilder mainScriptBuilder = new();
            // Main import
            mainScriptBuilder.AppendLine("""
                # Import Root Parcel NExT Module
                from parcel_next import LoadPackage

                """);
            // Import package references
            mainScriptBuilder.AppendLine("# Load Parcel NExT packages");
            foreach ((string importName, string nickName) in standardPackageImports)
                mainScriptBuilder.AppendLine($"LoadPackage('{importName}')");
            mainScriptBuilder.AppendLine();
            // Make necessary namespace usage and static usage
            mainScriptBuilder.AppendLine("# Import submodules");
            foreach ((string Package, string Namespace) in uniqueNamespaces)
                mainScriptBuilder.AppendLine($"from {Namespace} import *");
            mainScriptBuilder.AppendLine();
            // Import static types
            // TODO: Import static types from used nodes
            mainScriptBuilder.AppendLine();
            mainScriptBuilder.AppendLine("# Main script content");
            // Entry point
            mainScriptBuilder.AppendLine("""
                if __name__ == '__main__':
                """);
            // Do variable declarations first
            foreach ((string key, string value) in variableDeclarations)
                mainScriptBuilder.AppendLine($"{defaultIndentation}{key} = {value}");
            // Append script sections
            foreach (StringBuilder section in scriptSections)
            {
                mainScriptBuilder.Append(Regex.Replace(section.ToString().TrimEnd(), @"^", defaultIndentation, RegexOptions.Multiline)); // Apply line indentation
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
                return """"
                    """
                    Parcel NExT Executable Workflow
                    This python script is generated with Parcel NExT CodeGen.
                    You should have Python installed in order to execute this script; 
                    You can downoad Python at https://www.python.org/downloads/. We recommend latest version.
                    Notice to run the script you must make sure all essential dependencies are available under PYTHONPATH.

                    Use `pip install pythonnet` to install essential dependency.
                    Requires .NET Core to be installed or the dotnet CLI tool to be on the PATH.
                    """

                    """"; // TODO: Publish Parcel NExT as a standalone python package
            }
        }

        private static void GatherScriptDependencies(ExecutionQueue graph, out Dictionary<string, string> variableDeclarations, out List<string> scriptSection, out (string Package, string Namespace)[] uniqueNamespaces, out Dictionary<string, string> standardPackageImports)
        {
            List<AutomaticProcessorNode> automaticProcessors = [];
            // TOOD: Special handle math nodes that have corresponding C# operators: +-*/
            variableDeclarations = [];
            scriptSection = new();
            foreach (ProcessorNode processorNode in graph.Queue)
            {
                // Remark-cz: notice as of PV1 Neo we no longer have c#-lambda impelemented nodes; Everything is eitehr completely front-end implemented as ProcessorNode or it's C# defined in packages

                // Deal with package functions
                if (processorNode is AutomaticProcessorNode autoNode)
                {
                    automaticProcessors.Add(autoNode);
                    string[] parameters = autoNode.Input.Select(i => i.Title).ToArray();
                    
                    // Save outputs
                    if (autoNode.Output.Any())
                        scriptSection.Add($"{autoNode.MainOutput.Title} = {autoNode.Descriptor.NodeName}({string.Join(", ", parameters)})");
                    // Plain call
                    else 
                        scriptSection.Add($"{autoNode.Descriptor.NodeName}({string.Join(", ", parameters)})");

                    // Deal with missing parameters
                    foreach (string parameter in parameters)
                        variableDeclarations.TryAdd(parameter, "\"\"");
                }
                // Deal with front-end implemented nodes
                else
                {
                    // Primitives are processed as variable definition
                    if (processorNode is PrimitiveNode primitive)
                        variableDeclarations[processorNode.Title] = primitive.Value; // TODO: Instead of using MainOutput which depdends on cache which requires us to execute the graph, we should fetch directly its stored values.
                }
            }

            // Gather unique dependent assemblies
            System.Reflection.Assembly[] uniqueAssemblies = automaticProcessors.Select(p => p.Descriptor.Method.DeclaringType.Assembly).Distinct().ToArray();
            uniqueNamespaces = automaticProcessors.Select(p => p.Descriptor.Method.DeclaringType).GroupBy(t => new ValueTuple<string, string>(t.Assembly.GetName().Name, t.Namespace)).Select(g => g.Key).ToArray();
            standardPackageImports = uniqueAssemblies.Select(a => a.CodeBase)
                .Select(codeBase => Uri.UnescapeDataString(new UriBuilder(codeBase).Path))
                .Where(File.Exists)
                .Select(FindStandardPackageFriendlyName)
                .ToDictionary(n => n, n => n.Split('.').Last());
        }
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
}