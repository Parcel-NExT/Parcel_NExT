using Parcel.CoreEngine;
using Parcel.CoreEngine.Document;
using System.Text;

namespace Parcel.NExT.CodeGen
{
    public class CodeGenerator
    {
        #region Main
        public static CSharpScriptExecutableGenerator.ScriptFile GenerateGraphCodes(string projectName, ParcelDocument document, ParcelGraph containingGraph, ParcelNode[] graphNodes, bool entryScript, bool useTopLevelStatementsForEntryScript)
        {
            bool topLevel = entryScript && useTopLevelStatementsForEntryScript;

            StringBuilder scriptBuilder = new();
            // Usings
            foreach (string usingStatement in GatherUsings(graphNodes))
                scriptBuilder.AppendLine(usingStatement);

            // Namespace and class definition
            string indentation = string.Empty;
            if (!topLevel)
            {
                scriptBuilder.AppendLine($$"""
                    namespace {{projectName}}
                    {
                    """);
            }
            // TODO: Implement class
            // TODO: Implement entry functions

            // Node statements
            foreach (ParcelNode node in SortDependentNodes(document, containingGraph, graphNodes))
            {
                throw new NotImplementedException();
            }

            if (indentation != string.Empty)
                scriptBuilder.AppendLine("}");
            return new CSharpScriptExecutableGenerator.ScriptFile(containingGraph.Name, scriptBuilder.ToString().TrimEnd());
        }
        #endregion

        #region Routines
        private static string[] GatherUsings(ParcelNode[] nodes)
        {
            // TODO: Implement
            return Array.Empty<string>();
        }
        private static ParcelNode[] SortDependentNodes(ParcelDocument document, ParcelGraph graph, ParcelNode[] nodes)
        {
            // TODO: Filter relevant nodes, sort node execution order
            throw new NotImplementedException();
        }
        #endregion
    }
}
