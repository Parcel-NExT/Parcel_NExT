using Parcel.NExT.Python.Helpers;
using System.Text.RegularExpressions;

namespace Parcel.NExT.Python
{
    public record PyPiModule(string Name, string Description, string Url, string Version);

    /// <summary>
    /// Provide reflection related functions for Python
    /// </summary>
    public static class PythonReflection
    {
        public static string[] GetAllAvailableModules()
        {
            string? python = RuntimeHelper.FindProgram("python");
            if (python != null) 
            {
                string outputs = ProcessHelper.GetOutputWithInput(python, null, """
                    import pydoc
                    pydoc.help('modules')
                    import sys
                    sys.exit(0)
                    """);
                return ParsePythonPydocOutputs(outputs);
            }
            return [];

            static string[] ParsePythonPydocOutputs(string output)
            {
                return output.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(line => !string.IsNullOrWhiteSpace(line)) // SKip Empty lines
                    .Where(line => Regex.IsMatch(line, @"^((\w+?)\s+)+$")) // Match table-like lines
                    .SelectMany(line => line.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                    .OrderBy(name => name)
                    .ToArray();
            }
        }
        public static string[] GetInstalledPackages()
        {
            string? pip = RuntimeHelper.FindPythonPip();
            if (pip != null)
                return ParsePipPackageOutputs(ProcessHelper.GetOutput(pip, "list"));
            return [];

            static string[] ParsePipPackageOutputs(string output)
            {
                return output.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Skip(2) // Skip table headers
                    .Where(line => !string.IsNullOrWhiteSpace(line)) // SKip Empty lines
                    .Where(line => !Regex.IsMatch(line, @"^\[.*?\].*$")) // Skip notice lines
                    .Where(line => Regex.IsMatch(line, @"^(\w+)\s+([\d\.]+)$")) // Match table-like lines
                    .Select(line => line.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)[0])
                    .ToArray();
            }
        }
        public static PyPiModule[] GetPyPiModules()
        {
            throw new NotImplementedException();
        }
    }
}
