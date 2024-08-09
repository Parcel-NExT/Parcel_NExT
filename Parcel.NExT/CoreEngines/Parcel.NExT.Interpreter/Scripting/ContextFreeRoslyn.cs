using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Parcel.NExT.Interpreter.Helpers;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Parcel.NExT.Interpreter.Scripting
{
    /// <summary>
    /// Provides standalone use context-free Roslyn
    /// </summary>
    public static class ContextFreeRoslyn
    {
        #region Helper Construct
        public class ParcelNExTInternalContextSwitchPayload
        {
            public object? ParcelNExTInterpreterInternalPayload;
        }
        #endregion

        #region Low-Level
        /// <summary>
        /// Runs code in a local function so user code cannot import namespaces or define custom types etc.
        /// </summary>
        /// <remarks>
        /// Do not use this if you do you need ScriptState; Use higher level functions instead.
        /// </remarks>
        public static ScriptState<object> LowLevelRunLocalNoReturn(string codeFragment, ScriptOptions? options = null, object? contextObject = null, bool autoImport = false)
        {
            options ??= ScriptOptions.Default;

            string namespaceImports = string.Empty;
            if (autoImport)
                namespaceImports = TryFindAutomaticImports(codeFragment, null, out _);

            string functionInstanceName = GetDistinctFunctionInstanceName();
            return CSharpScript.RunAsync($$"""
                {{namespaceImports}}
                // Define a local function
                public static void {{functionInstanceName}}()
                {
                    {{codeFragment}}
                }
                // Call the function
                {{functionInstanceName}}();
                """.Trim(), options, contextObject).Result;
        }
        /// <summary>
        /// Runs code in a local function so user code cannot import namespaces or define custom types etc.;
        /// Return value is provided inside a "result" variable.
        /// </summary>
        /// <remarks>
        /// Do not use this if you do you need ScriptState; Use higher level functions instead.
        /// </remarks>
        public static ScriptState<object> LowLeveRunLocalWithReturn(string codeFragment, ScriptOptions? options = null, object? contextObject = null)
        {
            options ??= ScriptOptions.Default;
            string functionInstanceName = GetDistinctFunctionInstanceName();
            return CSharpScript.RunAsync($$"""
                // Define a local function
                public static object {{functionInstanceName}}()
                {
                    {{codeFragment}}
                }
                // Call the function
                object result = {{functionInstanceName}}();
                """, options, contextObject).Result;
        }
        /// <summary>
        /// Runs code at Roslyn top-level
        /// </summary>
        /// <remarks>
        /// Do not use this if you do you need ScriptState; Use higher level functions instead.
        /// </remarks>
        public static ScriptState<object> LowLeveRunGlobal(string codeFragment, ScriptOptions options, object contextObject)
        {
            return CSharpScript.RunAsync(codeFragment, options, contextObject).Result;
        }
        #endregion

        #region Common Scenarios
        /// <summary>
        /// Runs a code fragment as a local function with optionally provided global variables.
        /// Expects a return of target type from code snippet.
        /// </summary>
        /// <remarks>
        /// This function is similar to <seealso cref="EvaluateLocalSnippet(Dictionary{string, object}, string)"/> but serves different purpose.
        /// 
        /// This is not efficient for things that runs often - in which case we should compile and extract MethodInfo using CodeAnalyzer instead rely on Roslyn.
        /// </remarks>
        public static TType EvaluateLocalReturn<TType>(string codeFragment, Dictionary<string, object>? globalVariables = null)
        {
            ScriptOptions options = ScriptOptions.Default;

            Type returnType = typeof(TType);
            if (globalVariables == null)
            {
                string namespaceImportStatements = GetTypesNamespaceImportStatements([returnType]);
                ScriptState<object> state = InitializeStateWithGlobalContexts(options, null, namespaceImportStatements, null);

                CodeGenWrapAndCallLocalFunctionWithSingleReturn(codeFragment, returnType, out string resultVariableName, out string code);
                state = state.ContinueWithAsync(code).Result;
                return (TType)state.GetVariable(resultVariableName).Value;
            }
            else
            {
                // Initialize state
                ParcelNExTInternalContextSwitchPayload hostObject = new()
                {
                    ParcelNExTInterpreterInternalPayload = globalVariables
                };
                string namespaceImportStatements = GetTypesNamespaceImportStatements(globalVariables.Values
                    .Select(v => v.GetType())
                    .Concat([returnType]));
                string globalVariableDeclarationStatements = GetTypedDeclarationsForScriptGlobalVariables(globalVariables!);
                ScriptState<object> state = InitializeStateWithGlobalContexts(options, hostObject, namespaceImportStatements, globalVariableDeclarationStatements);

                // Run custom code
                CodeGenWrapAndCallLocalFunctionWithSingleReturn(codeFragment, returnType, out string resultVariableName, out string code);

                state = state.ContinueWithAsync(code).Result;
                return (TType)state.GetVariable(resultVariableName).Value;
            }
        }
        public static void EvaluateLocalSnippet<TType>(TType hostObject, string codeFragment)
        {
            string functionInstanceName = GetDistinctFunctionInstanceName();
            string code = $$"""
                // Define a local function
                public static void {{functionInstanceName}}()
                {
                    // The input host object members is accessible as global variable
                    {{codeFragment}}
                }
                // Call the function
                {{functionInstanceName}}();
                """;
            ScriptOptions options = ScriptOptions.Default;
            _ = CSharpScript.RunAsync(code, options, hostObject).Result;
        }
        /// <summary>
        /// Run code fragment on initial set of provided objects and return final values on such objects;
        /// Specific names from the original dictionary will be available as global variables.
        /// </summary>
        /// <remarks>
        /// Automatically exposes namespace of referenced objects from initial values.
        /// </remarks>
        public static Dictionary<string, object> EvaluateLocalSnippet(Dictionary<string, object> initial, string codeFragment)
        {
            ScriptOptions options = ScriptOptions.Default;

            // Initialize state
            ParcelNExTInternalContextSwitchPayload hostObject = new()
            {
                ParcelNExTInterpreterInternalPayload = initial
            };
            string namespaceImportStatements = GetTypesNamespaceImportStatements(initial.Values.Select(v => v.GetType()));
            string payloadVariableDeclarationStatements = GetTypedDeclarationsForScriptGlobalVariables(initial);
            ScriptState<object> state = InitializeStateWithGlobalContexts(options, hostObject, namespaceImportStatements, payloadVariableDeclarationStatements);

            // Run custom code
            string code = CodeGenWrapAndCallLocalFunctionWithoutReturn(codeFragment);

            state = state.ContinueWithAsync(code).Result;
            return initial.ToDictionary(i => i.Key, i => state.GetVariable(i.Key).Value);
        }
        /// <summary>
        /// Enumerate code fragment on collection
        /// </summary>
        public static void Map<TType>(IEnumerable<TType> enumeration, string codeFragment)
        {

        }
        #endregion

        #region Code Analysis
        /// <summary>
        /// Figure out all the types used in the code and try to generate automatic import statements for them.
        /// </summary>
        /// <remarks>
        /// Current implementation of this is slow, so we should use it with care.
        /// </remarks>
        public static string TryFindAutomaticImports(string codeFragment, IEnumerable<Type>? knownTypes, out Type[] importTypes)
        {
            // Fetch all allowed system types
            Assembly[] allDefaultAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            IEnumerable<IGrouping<string, Type>> exportedTypes = allDefaultAssemblies.SelectMany(a => a.ExportedTypes)
                .GroupBy(t => t.GetFormattedName()); // Surprisingly, there are lots of types with the same name
            Dictionary<string, Type[]> allDefaultTypes = exportedTypes
                .ToDictionary(g => g.Key, g => g.ToArray()); // Remark-cz: Notice we can only handle "formatted" names

            // Let it compile and see what types we need to deal with
            string importStatement = knownTypes != null ? GetTypesNamespaceImportStatements(knownTypes) : string.Empty;
            List<Type> neededTypes = new();
            while (true) // Remark-cz: This might loop inefficient - although pragmatically speaking, this is what's going to happen if a human were to deal with such situation himself; So even though this is not optimal, this is at least sufficiently automated.
            {
                try
                {
                    CSharpScript.RunAsync(importStatement + Environment.NewLine + codeFragment);
                }
                catch (CompilationErrorException e)
                {
                    Match match = Regex.Match(e.Message, "The type or namespace name '(.*?)' could not be found");
                    string name = match.Groups[1].Value;
                    if (e.Message.Contains("CS0246") && match.Success && allDefaultTypes.ContainsKey(name))
                    {
                        neededTypes.Add(allDefaultTypes[name].First() /*Just pick the first we can find*/);
                        importStatement = GetTypesNamespaceImportStatements(neededTypes);
                    }
                }
                break;
            }

            importTypes = neededTypes.ToArray();
            return importStatement;
        }
        #endregion

        #region Async
        public static Task<ScriptState<object>> RunLocalAsync(string codeFragment, ScriptOptions options, object contextObject)
        {
            throw new NotImplementedException();
        }
        public static Task<ScriptState<object>> RunGlobalAsync(string codeFragment, ScriptOptions options, object contextObject)
        {
            return CSharpScript.RunAsync(codeFragment, options, contextObject);
        }
        #endregion

        #region Helpers
        private static string Indent(string codeFragment, int level = 1)
        {
            return Regex.Replace(codeFragment, "^", new string('\t', level), RegexOptions.Multiline);
        }
        private static string GetDistinctFunctionInstanceName() 
            => $"LocalFunction_{DateTime.Now:yyyyMMddhhmmss}";
        private static string GetTypedDeclarationsForScriptGlobalVariables(Dictionary<string, object> globalVariables)
        {
            // Remark-cz: We have to do some manual setup because of a limit at the moment: https://github.com/dotnet/roslyn/issues/3194
            // Also see: https://github.com/Charles-Zhang-Parcel/Prototypes/blob/bdb60100bf71fe76f1aabe0783c4a5e337aef2b9/CoreEngine/MultiInterpolation/Program.cs#L87

            StringBuilder contextSwitchPayloadToExplictGlobalDictionaryDeclarations = new();
            foreach ((string Key, object Value) in globalVariables)
            {
                string typeName = Value.GetType().GetFormattedName();
                contextSwitchPayloadToExplictGlobalDictionaryDeclarations.AppendLine($"{typeName} {Key} = ({typeName})((Dictionary<string, object>){nameof(ParcelNExTInternalContextSwitchPayload.ParcelNExTInterpreterInternalPayload)})[\"{Key}\"];");
            }
            return contextSwitchPayloadToExplictGlobalDictionaryDeclarations.ToString().TrimEnd();
        }
        private static string CodeGenWrapAndCallLocalFunctionWithoutReturn(string codeFragment)
        {
            string functionInstanceName = GetDistinctFunctionInstanceName();
            string code = $$"""
                // Define a local function
                public void {{functionInstanceName}}()
                {
                    // The input dictionary values are accessible as global variables
                    {{Indent(codeFragment, 1)}}
                }
                // Call the function
                {{functionInstanceName}}();
                """;
            return code;
        }
        private static void CodeGenWrapAndCallLocalFunctionWithSingleReturn(string codeFragment, Type returnType, out string resultVariableName, out string code)
        {
            string functionInstanceName = GetDistinctFunctionInstanceName();
            resultVariableName = $"result_{functionInstanceName}";
            code = $$"""
                // Define a local function
                public {{returnType.GetFormattedName()}} {{functionInstanceName}}()
                {
                    // The input dictionary values are accessible as global variables
                    {{Indent(codeFragment, 1)}}
                }
                // Call the function
                {{returnType.GetFormattedName()}} {{resultVariableName}} = {{functionInstanceName}}();
                """;
        }
        private static string GetTypesNamespaceImportStatements(IEnumerable<Type> types)
        {
            StringBuilder importNamespaces = new();
            foreach (var typeGroup in types
                .Where(v => v.Namespace != null)
                .GroupBy(t => t.Namespace))
            {
                string ns = typeGroup.Key!;
                importNamespaces.AppendLine($"using {ns}; // Exposes {string.Join(", ", typeGroup.Select(t => t.Name).Distinct())}");
            }
            return importNamespaces.ToString().TrimEnd();
        }
        private static ScriptState<object> InitializeStateWithGlobalContexts(ScriptOptions options, ParcelNExTInternalContextSwitchPayload? hostObject, string? namespaceImportStatements, string? globalVariableDeclarationStatements)
        {
            StringBuilder builder = new();
            if (!string.IsNullOrEmpty(namespaceImportStatements))
                builder.AppendLine($"""
                    // Import namespaces
                    using System.Collections.Generic; // Exposes Dictionary
                    {namespaceImportStatements}
                    """);
            builder.AppendLine();
            if (!string.IsNullOrEmpty(globalVariableDeclarationStatements))
                builder.AppendLine($"""
                    // Expose the payload objects to global
                    {globalVariableDeclarationStatements}
                    """);
            string initializationCode = builder.ToString().Trim();
            return CSharpScript.RunAsync(initializationCode, options, hostObject).Result;
        }
        #endregion
    }
}
