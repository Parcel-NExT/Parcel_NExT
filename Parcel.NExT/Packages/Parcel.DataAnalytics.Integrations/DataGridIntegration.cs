using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis;
using Parcel.FileFormats;
using Parcel.NExT.Interpreter.Scripting; // TODO: Replace direct Roslyn usage with a layer of abstraction in Parcel.NExT.Interpreter.Scripting
using Parcel.Types;
using Microsoft.CodeAnalysis.Scripting;
using System.Collections;
using System.Linq;
using System.Dynamic;
using Parcel.CoreEngine.Service.Interpretation;
using Parcel.NExT.Interpreter.Analyzer;

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
        public static DataGrid FilterRows(this DataGrid original, string expression)
        {
            // TODO: Might want to do a pass of quick syntax analysis to ensure snippet conforms to expectation
            // TODO: Implement proper support using "and" and "or" for && and || operator in mini expression syntax
            // Remark: Notice Roslyn does NOT work with dynamic/expando object as globals

            List<DataColumn> columns = original.Columns;
            string pesudoFunction = $$"""
                using System;

                public static bool CheckRow({{string.Join(", ", columns.Select(c => $"{c.Type.FullName} {c.Header.Replace(" ", string.Empty) /*Deal with illegal variable names*/}"))}})
                {
                    return {{expression.Replace(" and ", " && ").Replace(" or ", " || ").Replace(" = ", " == ") /*Notice those are not safe e.g. won't work with strings; Might require proper syntax analysis*/}}; // Mini DSL
                }
                """;
            FunctionalNodeDescription? compilation = CodeAnalyzer.CompileFunctionalNode(pesudoFunction);

            // TODO: Refine DataGrid to provide proper and more convinient constructor
            DataGrid result = new(original.TableName, original.ColumnHeaders.Select(c => new DataColumn(c)).ToArray()); // TODO: The API with IEnumerable is very bad and very misleading and easily get things confused
            foreach (var row in original.Rows.Where(row =>
            {
                IDictionary<string, object> expando = row as ExpandoObject;
                bool result = (bool)compilation.Method.StaticInvoke(expando.Values.ToArray()); // TODO: Check property order match column order

                return result;

                // Remark: `(bool?)CSharpScript.RunAsync(snippet, ScriptOptions.Default, row).Result.ReturnValue ?? false` won't work due to expando/dynamic object issue
            }))
            {
                result.AddRow(((IDictionary<string, object>)(row as ExpandoObject)).Values.ToArray());
            }
            return result;
        }
        /// <summary>
        /// Group a data grid by group key then perform transform to get a flat table
        /// </summary>
        public static DataGrid GroupBy(this DataGrid origina, string groupKeyExpression, Func<Dictionary<string, object>, object> processorFunction)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Group a data grid by group key then perform transform to get a flat table
        /// </summary>
        public static DataGrid GroupBy(this DataGrid origina, string groupKeyExpression, string processingSnippet)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
