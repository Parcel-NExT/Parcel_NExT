using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            // TOOD: Special handle math nodes that have corresponding C# operators: +-*/
            List<string> variableDeclarations = [];
            StringBuilder scriptSection = new();
            foreach (ProcessorNode processorNode in graph.Queue)
            {
                // Remark-cz: notice as of PV1 Neo we no longer have c#-lambda impelemented nodes; Everything is eitehr completely front-end implemented as ProcessorNode or it's C# defined in packages

                // Deal with package functions
                if (processorNode is AutomaticProcessorNode autoNode)
                {
                    scriptSection.AppendLine($"{autoNode.Descriptor.NodeName}({string.Join(", ", autoNode.Input.Select(i => i.Title))})");
                }
                // Deal with front-end implemented nodes
                else
                {
                    // Primitives are processed as variable definition
                    if (processorNode is PrimitiveNode primitive)
                        variableDeclarations.Add($"var {processorNode.Title} = {processorNode.MainOutput};"); // TODO: Instead of using MainOutput which depdends on cache which requires us to execute the graph, we should fetch directly its stored values.
                }
            }
            StringBuilder[] scriptSections = [scriptSection];

            // Create output folder if not exist
            Directory.CreateDirectory(folderPath);

            // Generate code files
            StringBuilder mainScriptBuilder = new();
            // Do variable declarations first
            foreach (string declaration in variableDeclarations)
                mainScriptBuilder.Append(declaration);
            // Append script sections
            foreach (StringBuilder section in scriptSections)
            {
                mainScriptBuilder.Append(section.ToString().TrimEnd());
                mainScriptBuilder.AppendLine();
            }

            // Save main script
            string mainScript = mainScriptBuilder.ToString().TrimEnd();
            File.WriteAllText(mainScriptFilename, mainScript);

            // TODO: Implement generating other script files
        }
        #endregion
    }
}