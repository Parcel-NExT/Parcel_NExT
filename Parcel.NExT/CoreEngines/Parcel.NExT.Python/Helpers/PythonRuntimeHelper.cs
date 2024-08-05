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
                string outputs = ProcessHelper.GetOutput(pip, "show", module);
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

        /// <summary>
        /// Notice due to the way GIC works - the entire (Parcel) application can only have one single (shared) Python context.
        /// All the libraries/packages that uses any part of Python functionality must play nicely and share such a context.
        /// And this function (aka. python initialization) must be called only once ever.
        /// </summary>
        public static bool PythonEngineAlreadyInitialized { get; private set; } = false;
        public static void TryInitializeEngine()
        {
            if (!PythonEngineAlreadyInitialized)
                InitializeEngine();
        }
        public static void InitializeEngine()
        {
            if (PythonEngineAlreadyInitialized)
                throw new ApplicationException($"Python engine can only be initialized once!");

            string? installedPython = FindPythonDLL()
                ?? throw new ArgumentException("Cannot find any usable Python installation on the machine.");

            Runtime.PythonDLL = installedPython;
            PythonEngine.Initialize();

            PythonEngineAlreadyInitialized = true;
        }
        /// <summary>
        /// This function should generally be called by the runtime (otherwise usually implemented on the front-end side), otherwise the entire application will NOT shutdown properly
        /// </summary>
        public static void TryShutdownEngine()
        {
            if (PythonEngineAlreadyInitialized)
                PythonEngine.Shutdown(); // Remark: This may cause binary serialization exception (Remark) This was a well-known issue; We might have already fixed it. I remember it's related to changing some serialization setting.
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
            InitializeEngine();

            PyModule pythonScope;
            using (Py.GIL())
            {
                pythonScope = Py.CreateScope();
                pythonScope.Exec(snippet);
            }
            if (shutDown)
                TryShutdownEngine();
            return pythonScope;
        }
        #endregion
    }
}
