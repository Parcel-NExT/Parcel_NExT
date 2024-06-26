using Parcel.CoreEngine.Helpers;
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
            string? python = EnvironmentVariableHelper.FindProgram("python");
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
                return output.SplitLines(true)
                    .Where(line => !string.IsNullOrWhiteSpace(line)) // SKip Empty lines
                    .Where(line => Regex.IsMatch(line, @"^((\w+?)\s+)+$")) // Match table-like lines
                    .SelectMany(line => line.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                    .OrderBy(name => name)
                    .ToArray();
            }
        }
        public static string[] GetInstalledPackages()
        {
            string? pip = Helpers.PythonRuntimeHelper.FindPythonPip();
            if (pip != null)
                return ParsePipPackageOutputs(ProcessHelper.GetOutput(pip, "list")); // Remark: Alternative is to use `pkg_resources`
            return [];

            static string[] ParsePipPackageOutputs(string output)
            {
                return output
                    .SplitLines(true)
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
            HttpClient client = new();
            string html = client.GetStringAsync(@"https://pypi.org/simple/").Result; // Remark: See https://discuss.python.org/t/getting-full-list-of-published-packages-from-pypi-org/23337
            (string Link, string Name)[] definitions = html
                .SplitLines(true)
                .Select(line => Regex.Match(line, @"<a href=""(.*?)"">(.*)</a>"))
                .Where(match => match.Success)
                .Select(match => (Link: match.Groups[1].Value, Name: match.Groups[2].Value))
                .ToArray();
            // TODO: Refine more details about the module, e.g. by doing subsequence query - might want to do it once doing software installation and never do it again unless explicitly called
            return definitions.Select(d => new PyPiModule(d.Name, string.Empty, $"https://pypi.org/simple{d.Link}", string.Empty)).ToArray();
        }
        public static string[] GetClassesInModule(string module)
        {
            // TODO: https://stackoverflow.com/questions/1796180/how-can-i-get-a-list-of-all-classes-within-current-module-in-python
            throw new NotImplementedException();
        }
    }
}
