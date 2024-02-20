using Parcel.NExT.Python.Helpers;
using Python.Runtime;
using System.Text.RegularExpressions;

namespace Parcel.NExT.Python
{
    public static class PythonRuntimeHelper
    {
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
            var installedPython = RuntimeHelper.FindPythonDLL();
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
