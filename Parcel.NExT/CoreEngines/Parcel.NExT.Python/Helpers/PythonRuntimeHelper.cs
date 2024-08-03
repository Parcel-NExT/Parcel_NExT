using Parcel.CoreEngine.Helpers;
using Python.Runtime;
using System.Text.RegularExpressions;

namespace Parcel.NExT.Python.Helpers
{
    public static class PythonRuntimeHelper
    {
        #region Constants
        public const string PythonExecutableName = "python";
        public const string PipExecutableName = "pip";
        #endregion

        #region Executable
        public static string? FindPythonDLL()
        {
            return EnvironmentVariableHelper.EnumerateEnvironmentVariablesSearchingForFilePattern(["PATH"], @"python\d{2,3}\.dll");
        }
        public static string? FindPythonPip()
        {
            return EnvironmentVariableHelper.EnumerateEnvironmentVariablesSearchingForFilePattern(["PATH"], $"{PipExecutableName}.exe");
        }
        #endregion

        #region Queries
        public static string? GetModuleVersion(string module)
        {
            string? pip = EnvironmentVariableHelper.FindProgram(PipExecutableName);
            if (pip != null)
            {
                string outputs = ProcessHelper.GetOutput(pip, "show", "bpy");
                return Regex.Match(outputs, @"Version: (.*)")
                    .Groups[1].Value
                    .Replace(" ", string.Empty)
                    .Trim();
            }
            return null;
        }
        public static string? GetPythonVersion()
        {
            string? python = EnvironmentVariableHelper.FindProgram(PythonExecutableName);
            if (python != null)
            {
                string outputs = ProcessHelper.GetOutput(python, "--version");
                return outputs
                    .Replace("Python", string.Empty)
                    .Replace(" ", string.Empty)
                    .Trim();
            }
            return null;
        }
        #endregion

        #region Methods
        /// <remarks>
        /// Requires explicit return statement;
        /// Does not support imports - will cause syntax error or unexpected behaviors
        /// </remarks>
        public static PyObject RunScopedSnippetAsFunctionBody(string snippet)
        {
            string indentedSnippet = Regex.Replace(snippet, "^", "\t", RegexOptions.Multiline);
            string uniquenessIdentifier = DateTime.Now.ToString("yyyyMMddhhmmss");
            string uniqueFunctionname = $"ScopedSnippetFunction_{uniquenessIdentifier}";
            var pythonScope = RunTopLevelSnippet($"""
                    def {uniqueFunctionname}():
                    {indentedSnippet}
                    result = {uniqueFunctionname}()
                    """, false);
            PyObject? result = pythonScope.GetAttr("result");
            return result;
        }
        /// <summary>
        /// Runs a snippet as if running a script file using python
        /// </summary>
        public static PyModule RunTopLevelSnippet(string snippet, bool shutDown = true)
        {
            var installedPython = Helpers.PythonRuntimeHelper.FindPythonDLL();
            if (installedPython == null)
                throw new ArgumentException("Cannot find any usable Python installation on the machine.");

            Runtime.PythonDLL = installedPython;
            PythonEngine.Initialize();
            PyModule pythonScope;
            using (Py.GIL())
            {
                pythonScope = Py.CreateScope();
                pythonScope.Exec(snippet);
            }
            if (shutDown)
                PythonEngine.Shutdown();
            return pythonScope;
        }
        #endregion
    }
}
