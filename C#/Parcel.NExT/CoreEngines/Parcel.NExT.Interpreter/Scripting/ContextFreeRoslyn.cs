using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Parcel.NExT.Interpreter.Helpers;
using System.Text;

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
        public static ScriptState<object> RunLocalNoReturn(string codeFragment, ScriptOptions? options = null, object? contextObject = null)
        {
            options ??= ScriptOptions.Default;
            string functionInstanceName = GetDistinctFunctionInstanceName();
            return CSharpScript.RunAsync($$"""
                // Define a local function
                public static void {{functionInstanceName}}()
                {
                    {{codeFragment}}
                }
                // Call the function
                {{functionInstanceName}}();
                """, options, contextObject).Result;
        }
        /// <summary>
        /// Runs code in a local function so user code cannot import namespaces or define custom types etc.;
        /// Return value is provided inside a "result" variable.
        /// </summary>
        public static ScriptState<object> RunLocalWithReturn(string codeFragment, ScriptOptions? options = null, object? contextObject = null)
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
        public static ScriptState<object> RunGlobal(string codeFragment, ScriptOptions options, object contextObject)
        {
            return CSharpScript.RunAsync(codeFragment, options, contextObject).Result;
        }
        #endregion

        #region Common Scenarios
        public static void LogicLocal<TType>(TType hostObject, string codeFragment)
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
        public static Dictionary<string, object> LogicLocal(Dictionary<string, object> initial, string codeFragment)
        {
            ScriptOptions options = ScriptOptions.Default;

            // Remark-cz: We have to do some manual setup because of a limit at the moment: https://github.com/dotnet/roslyn/issues/3194
            // Also see: https://github.com/Charles-Zhang-Parcel/Prototypes/blob/bdb60100bf71fe76f1aabe0783c4a5e337aef2b9/CoreEngine/MultiInterpolation/Program.cs#L87

            // Initialize state
            ParcelNExTInternalContextSwitchPayload hostObject = new()
            {
                ParcelNExTInterpreterInternalPayload = initial
            };
            StringBuilder exposePayloadToGlobal = new();
            foreach ((string Key, object Value) in initial)
            {
                string typeName = Value.GetType().GetFormattedName();
                exposePayloadToGlobal.AppendLine($"{typeName} {Key} = ({typeName})((Dictionary<string, object>){nameof(ParcelNExTInternalContextSwitchPayload.ParcelNExTInterpreterInternalPayload)})[\"{Key}\"];");
            }
            StringBuilder importNamespaces = new();
            foreach (var value in initial.Values)
            {
                string? ns = value.GetType().Namespace;
                if (ns != null)
                    importNamespaces.AppendLine($"using {ns}; // Exposes {value.GetType().Name}");
            }

            string namespaceImportStatements = importNamespaces.ToString().TrimEnd();
            string payloadVariableDeclarationStatements = exposePayloadToGlobal.ToString().TrimEnd();
            ScriptState<object> state = CSharpScript.RunAsync($"""
                // Import namespaces
                using System.Collections.Generic; // Exposes Dictionary
                {namespaceImportStatements}

                // Expose the payload objects to global
                {payloadVariableDeclarationStatements}
                """, options, hostObject).Result;

            // Run custom code
            string functionInstanceName = GetDistinctFunctionInstanceName();
            string code = $$"""
                // Define a local function
                public void {{functionInstanceName}}()
                {
                    // The input dictionary values are accessible as global variables
                    {{codeFragment}}
                }
                // Call the function
                {{functionInstanceName}}();
                """;

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
        private static string GetDistinctFunctionInstanceName() 
            => $"LocalFunction_{DateTime.Now:yyyyMMddhhmmss}";
        #endregion
    }
}
