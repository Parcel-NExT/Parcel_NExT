using Parcel.CoreEngine.Helpers;
using System.Text.RegularExpressions;

namespace Parcel.NExT.Python.Helpers
{
    public static class RuntimeHelper
    {
        #region Methods
        public static string? FindPythonDLL()
        {
            return EnumerateEnvironmentVariablesSearchingForFilePattern(["PATH"], @"python\d{2,3}\.dll");
        }
        public static string? FindPythonPip()
        {
            return EnumerateEnvironmentVariablesSearchingForFilePattern(["PATH"], @"pip.exe");
        }
        /// <summary>
        /// Find disk location of program
        /// </summary>
        public static string? FindProgram(string program)
        {
            if (File.Exists(Path.GetFullPath(program))) return Path.GetFullPath(program);

            string[] paths = Environment.GetEnvironmentVariable("PATH")!.Split(';');
            return paths
                .SelectMany(folder =>
                {
                    if (program.ToLower().EndsWith(".exe"))
                        return new[] { Path.Combine(folder, program) };
                    else
                        return new[] { Path.Combine(folder, program), Path.Combine(folder, program + ".exe") };
                })
                .FirstOrDefault(File.Exists);
        }
        #endregion

        #region Routines
        static string? EnumerateEnvironmentVariablesSearchingForFilePattern(string[] environmentVariables, string fileNamePattern)
        {
            foreach (var variable in environmentVariables)
            {
                foreach (string path in SplitArgumentsLikeCsv(Environment.GetEnvironmentVariable(variable)!, ';', true))
                {
                    try
                    {
                        foreach (var file in Directory.EnumerateFiles(path))
                        {
                            string filename = Path.GetFileName(file);
                            if (Regex.IsMatch(filename, fileNamePattern))
                                return file;
                        }
                    }
                    // Remark-cz: Certain paths might NOT be enumerable due to access issues
                    catch (Exception) { continue; }
                }
            }
            return null;
        }
        static string[] SplitArgumentsLikeCsv(string line, char separator = ',', bool ignoreEmptyEntries = false)
        {
            string[] arguments = line.SplitCommandLineArguments(separator);

            if (ignoreEmptyEntries)
                return arguments.Where(a => !string.IsNullOrWhiteSpace(a)).ToArray();
            else
                return arguments;
        }
        #endregion
    }
}
