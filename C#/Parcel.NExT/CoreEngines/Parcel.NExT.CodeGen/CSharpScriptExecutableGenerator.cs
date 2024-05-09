using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace Parcel.NExT.CodeGen
{
    /// <summary>
    /// Generates .exe files from C# scripts using dotnet build tool;
    /// Expects dotnet SDK to be present
    /// </summary>
    public sealed class CSharpScriptExecutableGenerator
    {
        #region Subtypes
        public record ScriptFile(string ScriptName, string ScriptContent);
        #endregion

        #region Methods
        /// <summary>
        /// Generates a platform-specific executable, optionally as a single file.
        /// </summary>
        /// <returns>
        /// Returns the path of the executable file itself, also outputs build messages as an out parameter.
        /// </returns>
        /// <remarks>
        /// This does not deal with "main" (top-level) script and assumes scripts are correct; This function is not dealing with potential compilation errors.
        /// </remarks>
        public string Generate(string programName, string[]? dependancies, ScriptFile[] scripts, string projectOutputFolder, string? publishOutputFolder, bool singleFile, out string messages)
        {
            string sdkVersion = GetDotnetSDKVersion();
            if (!AssertSDKVersion(sdkVersion))
                throw new Exception($"Invalid sdk version: {sdkVersion}");

            string generatedProjectPath = Path.Combine(projectOutputFolder, programName);
            Directory.CreateDirectory(generatedProjectPath);

            StringBuilder outputs = new();
            // Generate project
            outputs.AppendLine(RunGenerateProject(generatedProjectPath));
            File.Delete(Path.Combine(generatedProjectPath, "Program.cs")); // Delete default empty top-level file
            // TODO: Handle dependancies - modify csproj file as needed
            // Dump scripts
            foreach (var script in scripts)
            {
                string path = Path.Combine(generatedProjectPath, $"{script.ScriptName}.cs");
                File.WriteAllText(path, script.ScriptContent);
            }
            // Publish
            if (singleFile)
                outputs.AppendLine(RunBuildProjectSingleExecutable(generatedProjectPath, publishOutputFolder ?? "publish"));
            else
                outputs.AppendLine(RunBuildProject(generatedProjectPath, publishOutputFolder ?? "publish"));

            messages = outputs.ToString().TrimEnd();
            return Path.Combine(generatedProjectPath, "publish", $"{Path.GetFileName(generatedProjectPath)}.exe"); // TODO: Fix suffix for non-windows platforms
        }
        #endregion

        #region Helpers
        private const string SDKPath = "dotnet";
        public static bool AssertSDKVersion(string sdkVersion)
            => Regex.IsMatch(sdkVersion, @"^8\.(.+)$"); // Remark: At the moment we assert version 8.+
        public static string GetDotnetSDKVersion()
            => RunCommand(SDKPath, "--version", Directory.GetCurrentDirectory());
        private static string RunGenerateProject(string folder)
            => RunCommand(SDKPath, "new console", folder);
        private static string RunBuildProject(string projectFolder, string publishFolder) // TODO: Escape folder paths properly
            => RunCommand(SDKPath, $"publish --output {publishFolder}", projectFolder);
        private static string RunBuildProjectSingleExecutable(string projectFolder, string publishFolder) // TODO: Escape folder paths properly
            => RunCommand(SDKPath, $"publish --use-current-runtime --output {publishFolder} -p:PublishSingleFile=true --self-contained false", projectFolder);
        private static string RunCommand(string program, string arguments, string workingDirectory) // TODO: Unify all run process functions inside a single utility class
        {
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = program,
                    Arguments = arguments,
                    WorkingDirectory = workingDirectory,
                    RedirectStandardOutput = true
                }
            };
            process.Start();
            string outputs = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return outputs;
        }
        #endregion
    }
}
