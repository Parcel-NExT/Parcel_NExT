using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;

namespace Parcel.NExT.Interpreter.Analyzer
{
    /// <summary>
    /// Holder and analyzer for global statements only (Roslyn scripting/Pure style)
    /// </summary>
    public class CodeSnippetComponents
    {
        #region Construction
        public CodeSnippetComponents(string code)
        {
            Code = code;
            Analyze(code);
        }
        #endregion

        #region Properties
        public string Code { get; }
        public SyntaxTree Tree { get; private set; }
        public LocalFunctionStatementSyntax EntryFunction { get; private set; }
        public LocalFunctionStatementSyntax[] StaticFunctions { get; private set; }
        public LocalFunctionStatementSyntax[] AllFunctions { get; private set; }
        public UsingDirectiveSyntax[] Usings { get; private set; }
        #endregion

        #region Routines
        private void Analyze(string code)
        {
            Tree = CSharpSyntaxTree.ParseText(code);
            CompilationUnitSyntax root = Tree.GetCompilationUnitRoot();
            if (root.Kind() != SyntaxKind.CompilationUnit)
                throw new ApplicationException("Unexpected root");

            SyntaxList<UsingDirectiveSyntax> usings = root.Usings;
            Usings = usings.ToArray();

            SyntaxList<MemberDeclarationSyntax> members = root.Members;
            if (members.Count() == 0 || members.Any(m => m.Kind() != SyntaxKind.GlobalStatement))
                throw new InvalidScriptException("Requires statements at the global level.");

            LocalFunctionStatementSyntax[] localFunctions = members.Select(m => m as GlobalStatementSyntax)
                .Where(gm => gm.Statement.Kind() == SyntaxKind.LocalFunctionStatement)
                .Select(gm => gm.Statement as LocalFunctionStatementSyntax)
                .ToArray();

            if (localFunctions.Length == 0)
                throw new InvalidScriptException("Requires function definition.");

            EntryFunction = localFunctions.Single(f => f.Modifiers.Any(t => t.IsKind(SyntaxKind.PublicKeyword)) && f.Modifiers.Any(t => t.IsKind(SyntaxKind.StaticKeyword)));
            StaticFunctions = localFunctions.Where(f => f.Modifiers.Any(t => t.IsKind(SyntaxKind.StaticKeyword))).ToArray();
            AllFunctions = localFunctions.ToArray();
        }
        #endregion
    }
}
