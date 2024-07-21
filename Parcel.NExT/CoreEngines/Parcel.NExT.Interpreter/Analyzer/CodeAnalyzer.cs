using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
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
            SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();

            SyntaxKind kind = root.Kind();
            SyntaxList<UsingDirectiveSyntax> usings = root.Usings;
            SyntaxList<MemberDeclarationSyntax> members = root.Members;

            if (members.Where(m => m.Kind() == SyntaxKind.LocalFunctionStatement).Count() != 1)
                throw new InvalidScriptException("Expecting only a single function definition.");

            return null;
        }
    }
}
