using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Scripting;
using Parcel.CoreEngine.Service.Interpretation;
using System.Reflection;

namespace Parcel.NExT.Interpreter.Analyzer
{
    /// <summary>
    /// Not a grammar error but doesn't conform to expectation
    /// </summary>
    public class InvalidScriptException : ArgumentException
    {
        public InvalidScriptException(){}
        public InvalidScriptException(string message) : base(message) {}
        public InvalidScriptException(string message, Exception innerException) 
            : base(message, innerException) {}
    }

    public static class CodeAnalyzer
    {
        #region Syntax/Semantics Analysis
        /// <summary>
        /// Analyze code without generating Roslyn contexts
        /// </summary>
        public static SingleEntranceCodeSnippetComponents? AnalyzeFunctionalNode(string code)
        {
            SingleEntranceCodeSnippetComponents snippet = new(code);
            if (snippet.AllFunctions.Length != 1)
                throw new InvalidScriptException("Expecting only a single function definition.");

            return snippet;
        }
        #endregion

        #region Roslyn Compilation
        /// <summary>
        /// Generate Roslyn context from code (Single entrance)
        /// </summary>
        public static FunctionalNodeDescription? CompileFunctionalNode(string code)
        {
            SingleEntranceCodeSnippetComponents snippet = new(code);
            if (snippet.AllFunctions.Length != 1)
                throw new InvalidScriptException("Expecting only a single function definition.");

            return new FunctionalNodeDescription(snippet.EntryFunction.Identifier.Text, new Types.Callable(snippet));
        }
        /// <summary>
        /// Generate Roslyn context from code (multiple functions)
        /// </summary>
        public static FunctionalNodeDescription[]? CompileNodeFunctions(string code)
        {
            // Remark: At the moment we are just providing a quick implementation for the purpose of proof of concept.

            // Compile the code and reflect available methods
            ScriptOptions options = ScriptOptions.Default
                .AddReferences(typeof(Enumerable).Assembly)
                .AddReferences(typeof(HttpClient).Assembly);
            ScriptState<object> context = CSharpScript.RunAsync($$"""
                using System.Reflection;
                using System.Linq;

                {{code}}

                return Assembly.GetExecutingAssembly().GetExportedTypes().First().GetMethods(BindingFlags.Public | BindingFlags.Static);
                """, options).Result;
            MethodInfo[] methods = ((MethodInfo[])context.ReturnValue)
                .Where(m => !m.Name.StartsWith('<')) // Remove internal stuff
                .ToArray();
            return [.. methods.Select(m => new FunctionalNodeDescription(m.Name, new Types.Callable(m)))];

            // TODO: Definitely need more work, e.g. consolidate with SingleEntranceCodeSnippet, otherwise it would create trouble for serialization.
        }
        #endregion

        #region Helper
        public static void ExtractFunctionInformation(LocalFunctionStatementSyntax function, out string name, out string[] inputName, out string[] outputNames)
        {
            name = function.Identifier.Text;
            inputName = [.. function.ParameterList.Parameters.Select(p => p.Identifier.Text)];
            outputNames = [function.ReturnType.ToString()]; // TODO: Extract and append `out` parameters
        }
        #endregion
    }
}
