using System.Diagnostics;

namespace Parcel.CoreEngine.Helpers
{
    /// <summary>
    /// Provides foundational routines for Python interpreter, Pipeline package and server/client inter-operation
    /// </summary>
    public static class ProcessHelper
    {
        #region Process Summoning
        public static string GetOutput(string program, params string[] args)
            => GetOutput(program, Directory.GetCurrentDirectory(), args);
        public static string GetOutput(string program, string? workingDirectory, string[] args)
        {
            workingDirectory ??= Directory.GetCurrentDirectory();

            ProcessStartInfo startInfo = new()
            {
                FileName = program,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                Arguments = args.JoinAsArguments(),
                WorkingDirectory = workingDirectory,
            };

            // Start the process
            Process process = new()
            {
                StartInfo = startInfo
            };
            process.Start();

            // Get outputs
            string outputs = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return outputs;
        }
        public static string GetOutputWithInput(string program, string[]? args, string input)
        {
            using Process p = new()
            {
                StartInfo = new()
                {
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    FileName = program,
                    Arguments = args?.JoinAsArguments() ?? string.Empty
                }
            };
            p.Start();

            StreamWriter redirectStreamWriter = p.StandardInput;
            redirectStreamWriter.WriteLine(input);
            redirectStreamWriter.Close();

            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            return output;
        }
        public static string RunProgramIfAvailable(string programName, string startingFolder, params string[] arguments)
        {
            string? programPath = EnvironmentVariableHelper.FindProgram(programName);
            if (programPath != null && File.Exists(programPath))
                return GetOutput(programPath, startingFolder, arguments);
            return string.Empty;
        }
        #endregion

        #region Desktop System Behavior
        public static void OpenFileWithDefaultProgram(string path)
        {
            new Process
            {
                StartInfo = new ProcessStartInfo(path)
                {
                    UseShellExecute = true
                }
            }.Start();
        }
        #endregion

        #region Interoperation
        public delegate void OpenPathWithDefaultHandlerDelegate(string path);
        /// <summary>
        /// Typically implemented by the server, which will then communicate with the front-end to open a program/path/url in an appropriate fashion.
        /// </summary>
        public static OpenPathWithDefaultHandlerDelegate? UrlOpeningHandler;
        /// <summary>
        /// Seeks opening behavior through the delegate <see cref="OpenPathWithDefaultHandlerDelegate"/>.
        /// Packages should generally use this instead of <seealso cref="OpenFileWithDefaultProgram(string)"/>.
        /// </summary>
        /// <remarks>
        /// To facilitate cross-platform behavior and server-client architecture, the act of opening a file/url with default program (or just to start some program on the user side) should ultimately be handled by the front-end (e.g. on a web environment if we open a process/url it's started on the server, but what we really want would be to open that url in a new browser tab); This function provides an indirection and delete the job to any particular server/front-end.
        /// </remarks>
        public static void OpenFileWithDefaultProgramInterpreted(string path)
        {
            if (UrlOpeningHandler != null)
                UrlOpeningHandler.Invoke(path);
            else OpenFileWithDefaultProgram(path);
        }
        #endregion
    }
}
