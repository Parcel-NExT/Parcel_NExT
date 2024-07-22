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
        public static FunctionalNodeDescription? AnalyzeFunctionalNode(string code)
        {
            CodeSnippetComponents snippet = new(code);
            if (snippet.AllFunctions.Length != 1)
                throw new InvalidScriptException("Expecting only a single function definition.");
            
            return new FunctionalNodeDescription(snippet.EntryFunction.Identifier.Text, new Types.Callable(snippet));
        }
    }
}
