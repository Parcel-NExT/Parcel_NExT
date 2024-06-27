using System.Text.RegularExpressions;

namespace Parcel.CoreEngine.Helpers
{
    public static class EnvironmentVariableHelper
    {
        #region Helpers
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

        #region Environment Runtime
        public static string? EnumerateEnvironmentVariablesSearchingForFilePattern(string[] environmentVariables, string fileNamePattern)
        {
            foreach (var variable in environmentVariables)
            {
                foreach (string path in Environment.GetEnvironmentVariable(variable)!.SplitArgumentsLikeCsv(';', true))
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
        #endregion
    }
}
