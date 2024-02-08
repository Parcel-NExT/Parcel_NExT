using Python.Runtime;
using System.Text.RegularExpressions;

namespace Parcel.NExT.Python
{
    public static class PythonRuntimeHelper
    {
        /// <remarks>
        /// Requires explicit return statement;
        /// Does not support imports - will cause syntax error or unexpected behaviors
        /// </remarks>
        public static PyObject RunScopedSnippetAsFunctionBody(string snippet)
        {
            var installedPython = RuntimeHelper.FindPythonDLL();
            if (installedPython == null)
                throw new ArgumentException("Cannot find any usable Python installation on the machine.");

            Runtime.PythonDLL = installedPython;
            PythonEngine.Initialize();
            using (Py.GIL())
            {
                PyModule pythonScope = Py.CreateScope();

                string indentedSnippet = Regex.Replace(snippet,"^", "\t", RegexOptions.Multiline);
                string uniquenessIdentifier = DateTime.Now.ToString("yyyyMMddhhmmss");
                string uniqueFunctionname = $"ScopedSnippetFunction_{uniquenessIdentifier}";
                pythonScope.Exec($"""
                    def {uniqueFunctionname}():
                    {indentedSnippet}
                    result = {uniqueFunctionname}()
                    """);
                return pythonScope.GetAttr("result"); ;
            }
        }
    }
}
