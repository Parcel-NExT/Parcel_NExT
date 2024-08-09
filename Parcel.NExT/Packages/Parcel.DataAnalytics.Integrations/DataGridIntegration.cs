using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis;
using Parcel.FileFormats;
using Parcel.NExT.Interpreter.Scripting; // TODO: Replace direct Roslyn usage with a layer of abstraction in Parcel.NExT.Interpreter.Scripting
using Parcel.Types;
using Microsoft.CodeAnalysis.Scripting;
using System.Collections;
using System.Linq;
using System.Dynamic;

namespace Parcel.Integration
{
    /// <summary>
    /// Expose strongly named functions
    /// </summary>
    public static class DataGridIntegration
    {
        #region Construction
        public static DataGrid InitializeDataGridFromCsvText(string tableName, string csvText)
            => new(tableName, CSV.Parse(csvText, out string[]? headers, true), headers);
        #endregion

        #region Roslyn
        /// <summary>
        /// Provides a mini-language for processing rows
        /// </summary>
        public static DataGrid ProcessRow(this DataGrid original, string snippet)
        {
            return original.Apply(row =>
            {
                ScriptState<object> resultState = CSharpScript.RunAsync(snippet, ScriptOptions.Default, row).Result;
                Dictionary<string, object> variables = resultState.Variables.ToDictionary(v => v.Name, v => v.Value);
                return row;
            });
        }
        /// <summary>
        /// Provides a mini-language for filtering rows
        /// </summary>
        public static DataGrid FilterRows(this DataGrid original, string snippet)
        {
            // Remark: Notice Roslyn does NOT work with dynamic/expando object as globals
            return new(original.TableName, original.Rows.Where(row =>
            {
                IDictionary<string, object> expando = row as ExpandoObject;
                Dictionary<string, object> typedDictionary = new Dictionary<string, object>();
                foreach (var key in expando.Keys)
                    typedDictionary[key.Replace(" ", string.Empty)] = expando[key]; // Remove illegal characters for variable name

                // TODO: Might want to do a pass of quick syntax analysis to ensure snippet conforms to expectation

                return ContextFreeRoslyn.EvaluateLocalReturn<bool>($"return {snippet};" /*Mini DSL*/, typedDictionary); // `(bool?)CSharpScript.RunAsync(snippet, ScriptOptions.Default, row).Result.ReturnValue ?? false` won't work due to expando/dynamic object issue
            }));
        }
        #endregion
    }
}
