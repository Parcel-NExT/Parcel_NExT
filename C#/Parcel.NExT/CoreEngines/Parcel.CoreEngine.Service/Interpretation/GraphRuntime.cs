using Parcel.CoreEngine.Document;
using System.Text.RegularExpressions;

namespace Parcel.CoreEngine.Service.Interpretation
{
    public partial class GraphRuntime
    {
        #region Constructor
        public GraphRuntime(ParcelGraph mainGraph)
        {
            Graph = mainGraph;
        }
        #endregion

        #region Constants
        [GeneratedRegex(@"^({(.*?)})?(\[(.*?)\])?((.*?):)?(.*?)$")]
        public static partial Regex NodeTargetPathStructureProtocolSyntax();
        public record NodeTargetPathProtocolStructure(string System, string EngineSymbol, string AssemblyPath, string TargetPath);
        #endregion

        #region Properties
        public ParcelGraph Graph { get; }
        public Dictionary<string, string> Variables { get; } = new();
        #endregion

        #region Methods
        public void Execute()
        {
            Layouts.CanvasLayout mainLayout = Graph.Layouts.First();
            foreach (ParcelNode node in mainLayout.Nodes.Select(n => n.Node))
                ExecuteNode(node);
        }
        #endregion

        #region Routines
        private void ExecuteNode(ParcelNode node)
        {
            NodeTargetPathProtocolStructure targets = ParseNodeTargets(node.Target);

            if (!string.IsNullOrEmpty(targets.System))
                ExecuteSystemNode(node, targets);
            // Typical C#
            else if (string.IsNullOrEmpty(targets.EngineSymbol) || targets.EngineSymbol.ToUpper() == "C#")
            {
                string[] arguments = NodeDefinitionHelper.SimpleExtractParameters(node);
                ExecuteMethodStatic(targets, arguments);
            }
        }
        private void ExecuteSystemNode(ParcelNode node, NodeTargetPathProtocolStructure targets)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Helpers
        public static NodeTargetPathProtocolStructure ParseNodeTargets(string target)
        {
            Match match = NodeTargetPathStructureProtocolSyntax().Match(target);
            string system = match.Groups[2].Value;
            string engineSymbol = match.Groups[4].Value;
            string assemblyPath = match.Groups[6].Value;
            string objectPath = match.Groups[7].Value;
            return new NodeTargetPathProtocolStructure(system, engineSymbol, assemblyPath, objectPath);
        }
        public static void ExecuteMethodStatic(NodeTargetPathProtocolStructure targets, string[] arguments)
        {
            InvokationService.InvokeRemoteFunction(targets.AssemblyPath, targets.TargetPath, arguments);
        }
        #endregion
    }
}
