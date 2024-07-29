using Microsoft.CodeAnalysis.CSharp.Syntax;
using Parcel.CoreEngine.Service.Interpretation;

namespace Parcel.NExT.Interpreter.Analyzer
{
    /// <summary>
    /// Not a grammar error but doesn't conform to expectation
    /// </summary>
    public class InvalidScriptException : ArgumentException
    {
        public InvalidScriptException()
        {
        }

        public InvalidScriptException(string message) : base(message)
        {
        }

        public InvalidScriptException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
    public static class CodeAnalyzer
    {
        public static CodeSnippetComponents? AnalyzeFunctionalNode(string code)
        {
            CodeSnippetComponents snippet = new(code);
            if (snippet.AllFunctions.Length != 1)
                throw new InvalidScriptException("Expecting only a single function definition.");
            
            return snippet;
        }
        public static FunctionalNodeDescription? CompileFunctionalNode(string code)
        {
            CodeSnippetComponents snippet = new(code);
            if (snippet.AllFunctions.Length != 1)
                throw new InvalidScriptException("Expecting only a single function definition.");

            return new FunctionalNodeDescription(snippet.EntryFunction.Identifier.Text, new Types.Callable(snippet));
        }
        public static void ExtractFunctionInformation(LocalFunctionStatementSyntax function, out string name, out string[] inputName, out string[] outputNames)
        {
            name = function.Identifier.Text;
            inputName = [.. function.ParameterList.Parameters.Select(p => p.Identifier.Text)];
            outputNames = [function.ReturnType.ToString()]; // TODO: Extract and append `out` parameters
        }
    }
}
