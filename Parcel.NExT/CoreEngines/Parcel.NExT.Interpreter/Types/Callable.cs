using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Scripting;
using Parcel.NExT.Interpreter.Analyzer;
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
            Snippet
        }

        #region Designation
        private ConstructorInfo? Constructor { get; }
        private MethodInfo? Method { get; }
        private CodeSnippetComponents Snippet { get; }
        private ScriptState<object> CompiledSnippet { get; }
        #endregion

        #region Property        
        public CallableType Type { get; }
        public bool IsStatic { get; }
        public ParameterInfo[] Parameters { get; } // TODO: Downstream shouldn't depend on this because it's not available from code snipepts and also not available from Action/Func if we were to support those in the future
        /// <remarks>
        /// Comparing to DeclaringType, ReflectedType takes inheritance into consideration.
        /// </remarks>
        public Type? ReflectedType { get; }
        /// <remarks>
        /// Could be null if it's from snippet
        /// </remarks>
        public Type? DeclaringType { get; }
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
        public Callable(CodeSnippetComponents snippet)
        {
            LocalFunctionStatementSyntax entryFunction = snippet.EntryFunction;
            if (!entryFunction.Modifiers.Any(m => m.IsKind(SyntaxKind.StaticKeyword)) || !entryFunction.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword)))
                throw new ArgumentException("Entry function must be static public.");
            Snippet = snippet;

            // Build compilation
            // Remark: In general, we cannot directly get System.Reflection type from semantic model, which is good, because that way we can control exactly what to load; It just means a bit more infrastructure is needed to make sure semantics are clear and during runtime expected types and methods are loaded
            CSharpCompilation compilation = CSharpCompilation.Create("HelloWorld")
                .AddReferences(MetadataReference.CreateFromFile(typeof(string).Assembly.Location))
                .AddSyntaxTrees(entryFunction.Parent.Parent.SyntaxTree);
            SemanticModel semanticModel = compilation.GetSemanticModel(snippet.Tree);
            Microsoft.CodeAnalysis.TypeInfo returnType = semanticModel.GetTypeInfo(entryFunction.ReturnType);
            INamedTypeSymbol? typeSymbol = (INamedTypeSymbol)returnType.Type;

            // Quick hacK: Actually compile the code
            ScriptOptions options = ScriptOptions.Default
                .AddReferences(typeof(HttpClient).Assembly);
            CompiledSnippet = CSharpScript.RunAsync(snippet.Code, options).Result;
            IAssemblySymbol assembly = CompiledSnippet.Script.GetCompilation().Assembly;
            // TODO: Need more refactoring
            // MethodInfo[] methods = CompiledSnippet.Script.GetCompilation().Assembly.GetMethods(); // Remark: Downstream shouldn't depend on ParameterInfo
            // Notice snippet global/local functions appear to be Func/Action instead of proper methods because they seem not to belong to any Type/class
            //ParameterInfo[] result = CompiledSnippet
            //    .ContinueWithAsync("""
            //    ParameterInfo[] temporaryFetchedParameterInfos = ;
            //    """).Result
            //    .ContinueWithAsync<ParameterInfo[]>("temporaryFetchedParameterInfos")
            //    .Result.ReturnValue; // Actually we cannot do it because it's not safe
            // https://stackoverflow.com/questions/47219017/roslyn-how-can-i-instantiate-a-class-in-a-script-during-runtime-and-invoke-meth

            // Assign callable properties
            Type = CallableType.Constructor;
            IsStatic = true;
            ReturnType = System.Type.GetType($"{typeSymbol.ContainingNamespace}.{typeSymbol.Name}");
            ReflectedType = null;
            DeclaringType = null;
            Parameters = null;
        }
        #endregion

        #region Method
        public object? Invoke(object? instance, object?[]? arguments)
        {
            if (Type == CallableType.Snippet)
                throw new NotImplementedException();

            if (Type != CallableType.Constructor)
                return Method!.Invoke(instance, arguments);
            else throw new InvalidOperationException("Cannot call `Invoke` on constructor with instance parameter. Use `StaticInvoke` instead.");
        }
        public object? StaticInvoke(object?[]? arguments)
        {
            if (Type == CallableType.Snippet)
                throw new NotImplementedException();

            if (Type != CallableType.StaticMethod && Type != CallableType.Constructor)
                throw new InvalidOperationException($"Callable is not static.");

            if (Type == CallableType.StaticMethod)
                return Method!.Invoke(null, arguments);
            else
                return Constructor!.Invoke(arguments);
        }
        public object? InstanceInvoke(object? instance, object?[]? arguments)
        {
            if (Type == CallableType.Snippet)
                throw new NotImplementedException();

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
    }
}
