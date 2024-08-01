using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Scripting;
using Parcel.NExT.Interpreter.Analyzer;
using Parcel.NExT.Interpreter.Helpers;
using System.Reflection;

namespace Parcel.NExT.Interpreter.Types
{
    /// <summary>
    /// Represents a method/constructor/delegate that can be called.
    /// In general, this is equivalent to MethodInfo but it enables containing constructors.
    /// </summary>
    public sealed class Callable
    {
        public enum CallableType
        {
            Constructor,
            StaticMethod,
            InstanceMethod,
            Snippet // Behaves like StaticMethod but has difference when it comes to serialization
        }

        #region Designation
        private ConstructorInfo? Constructor { get; }
        /// <summary>
        /// Used by reflected methods and snippets
        /// </summary>
        /// <remarks>
        /// TODO: Cleanup/optimization - Downstream shouldn't depend on ParameterInfo, or otherwise we should attempt to abstract only the parameters we need
        /// </remarks>
        private MethodInfo? Method { get; }
        private ScriptState<object> CompiledSnippet { get; }
        /// <summary>
        /// Snippet source code
        /// </summary>
        private SingleEntranceCodeSnippetComponents Snippet { get; }
        #endregion

        #region Property        
        public CallableType Type { get; }
        public bool IsStatic { get; }
        public ParameterInfo[] Parameters { get; } // TODO: Downstream shouldn't depend on this because it's not available from code snipepts (well actually it is, so we are fine for now) and also not available from Action/Func if we were to support those in the future; We definitely need to implement our own encapsulated version of this, preparing for future use case e.g. Python scripts (in-memory) and python modules (disk files)
        /// <remarks>
        /// Comparing to DeclaringType, ReflectedType takes inheritance into consideration.
        /// </remarks>
        public Type? ReflectedType { get; }
        /// <remarks>
        /// Will not be null even if it's from snippet
        /// </remarks>
        public Type DeclaringType { get; }
        public Type ReturnType { get; }

        public bool IsConstructor => Type == CallableType.Constructor;
        #endregion

        #region Constructor
        public Callable(MethodInfo method)
        {
            Method = method;

            Type = method.IsStatic ? CallableType.StaticMethod : CallableType.InstanceMethod;
            IsStatic = method.IsStatic;
            ReturnType = method.ReturnType;
            ReflectedType = method.ReflectedType!;
            DeclaringType = method.DeclaringType!;
            Parameters = Method!.GetParameters();
        }
        public Callable(ConstructorInfo constructor)
        {
            Constructor = constructor;

            Type = CallableType.Constructor;
            IsStatic = true;
            ReturnType = constructor.ReflectedType!;
            ReflectedType = constructor.ReflectedType!;
            DeclaringType = constructor.DeclaringType!;
            Parameters = Constructor.GetParameters();
        }
        /// <summary>
        /// Create a callable from a single-entrace snippet
        /// </summary>
        public Callable(SingleEntranceCodeSnippetComponents snippet)
        {
            LocalFunctionStatementSyntax entryFunction = snippet.EntryFunction;
            if (!entryFunction.Modifiers.Any(m => m.IsKind(SyntaxKind.StaticKeyword)) || !entryFunction.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword)))
                throw new ArgumentException("Entry function must be static public.");
            Snippet = snippet;

            // Semantics analysis (At the moment this doesn't do much, in the future we can add syntax checks)
            // (Remark) In general, we cannot directly get System.Reflection type from semantic model, which is good, because that way we can control exactly what to load; It just means a bit more infrastructure is needed to make sure semantics are clear and during runtime expected types and methods are loaded
            CSharpCompilation compilation = CSharpCompilation.Create("HelloWorld")
                .AddReferences(MetadataReference.CreateFromFile(typeof(string).Assembly.Location))
                .AddSyntaxTrees(entryFunction.Parent.Parent.SyntaxTree);
            SemanticModel semanticModel = compilation.GetSemanticModel(snippet.Tree);
            Microsoft.CodeAnalysis.TypeInfo returnType = semanticModel.GetTypeInfo(entryFunction.ReturnType);
            INamedTypeSymbol? typeSymbol = (INamedTypeSymbol)returnType.Type;

            // Actually compile the code for caching and reflection purpose
            ScriptOptions options = ScriptOptions.Default
                .AddReferences(typeof(Enumerable).Assembly)
                .AddReferences(typeof(HttpClient).Assembly);
            CompiledSnippet = CSharpScript.RunAsync($$"""
                using System.Reflection;
                using System.Linq;

                {{snippet.Code}}

                return Assembly.GetExecutingAssembly().GetExportedTypes().First().GetMethod("{{entryFunction.Identifier}}");
                """, options).Result;
            MethodInfo entryMethod = (MethodInfo)CompiledSnippet.ReturnValue;

            // Assign callable properties
            Method = entryMethod;
            Type = CallableType.StaticMethod;
            IsStatic = true;
            ReturnType = System.Type.GetType($"{typeSymbol.ContainingNamespace}.{typeSymbol.Name}");
            ReflectedType = null;
            DeclaringType = entryMethod.DeclaringType;
            Parameters = entryMethod.GetParameters();
        }
        #endregion

        #region Method
        public object? Invoke(object? instance, object?[]? arguments)
        {
            if (Type != CallableType.Constructor)
                return Method!.Invoke(instance, arguments);
            else throw new InvalidOperationException("Cannot call `Invoke` on constructor with instance parameter. Use `StaticInvoke` instead.");
        }
        public object? StaticInvoke(object?[]? arguments)
        {
            if (Type == CallableType.StaticMethod || Type == CallableType.Snippet)
                return Method!.Invoke(null, arguments);
            else if (Type == CallableType.Constructor)
                return Constructor!.Invoke(arguments);
            else
                throw new InvalidOperationException($"Callable is not static.");
        }
        public object? InstanceInvoke(object? instance, object?[]? arguments)
        {
            if (Type != CallableType.InstanceMethod)
                throw new InvalidOperationException($"Callable is not instance based.");

            return Method!.Invoke(instance, arguments);
        }
        #endregion

        #region Helpers
        public static Type ByName(string name)
        {
            // Remark: Type.GetType can only find types in mscorlib or current assembly when you pass namespace qualified name. To make it work you need "AssemblyQualifiedName".
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Reverse())
            {
                var tt = assembly.GetType(name);
                if (tt != null)
                {
                    return tt;
                }
            }

            return null;
        }
        #endregion

        #region Serialization (Parcel Open Standard)
        /// <summary>
        /// At the moment also called as "Function Resource Identifier".
        /// Assembly/Package related identifying name, including all information that is needed to load this particular node from any given assembly.
        /// </summary>
        /// <remarks>
        /// Pending better internal name/jargon for this; Pending consolidating with POS.
        /// </remarks>
        public string GetRuntimeNodeTypeIdentifier()
        {
            switch (Type)
            {
                case CallableType.Constructor:
                    return Constructor.GetConstructorFullSignature();
                case CallableType.StaticMethod:
                    return Method.GetMethodFullSignature();
                case CallableType.InstanceMethod:
                    return Method.GetMethodFullSignature();
                case CallableType.Snippet:
                    return Snippet.Code; // TODO: Implement per POS PDS. Remark: Apparently this is a hack. We are not making use of static Code section per POS on PDS, but simply store things in-place.
                default:
                    throw new ArgumentException($"Unknown callable type: {Type}");
            }
        }
        #endregion
    }
}
