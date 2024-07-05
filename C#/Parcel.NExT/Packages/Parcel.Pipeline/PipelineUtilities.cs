using Parcel.CoreEngine.Helpers;
using System.Diagnostics;

namespace Parcel.Processing.Utilities
{
    /// <summary>
    /// Library main exposed interface
    /// </summary>
    public static class PipelineUtilities
    {
        #region Run Methods
        /// <summary>
        /// Run program
        /// </summary>
        public static string Run(string program)
        {
            string? programPath = EnvironmentVariableHelper.FindProgram(program);
            if (programPath == null)
                throw new ArgumentException($"Cannot find program {program}.");

            using Process p = new()
            {
                StartInfo = new()
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    FileName = program
                }
            };
            p.Start();

            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            return output;
        }
        /// <summary>
        /// Run program with arguments (final form)
        /// </summary>
        public static string Run(string program, string arguments)
        {
            string programPath = EnvironmentVariableHelper.FindProgram(program);
            if (programPath == null)
                throw new ArgumentException($"Cannot find program {program}.");

            using Process p = new()
            {
                StartInfo = new()
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    FileName = program,
                    Arguments = arguments
                }
            };
            p.Start();

            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            return output;
        }
        /// <summary>
        /// Run program with arguments as array
        /// </summary>
        public static string Run(string program, params string[] arguments)
        {
            string programPath = EnvironmentVariableHelper.FindProgram(program);
            if (programPath == null)
                throw new ArgumentException($"Cannot find program {program}.");

            using Process p = new()
            {
                StartInfo = new()
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    FileName = program,
                    Arguments = string.Join(" ", arguments.Select(EscapeArgument))
                }
            };
            p.Start();

            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            return output;
        }
        /// <summary>
        /// Run program with arguments as dictionary
        /// </summary>
        public static string Run(string program, Dictionary<string, string> arguments)
        {
            string programPath = EnvironmentVariableHelper.FindProgram(program);
            if (programPath == null)
                throw new ArgumentException($"Cannot find program {program}.");

            using Process p = new()
            {
                StartInfo = new()
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    FileName = program,
                    Arguments = string.Join(" ", arguments.Select(p => $"{p.Key} {EscapeArgument(p.Value)}"))
                }
            };
            p.Start();

            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            return output;
        }
        #endregion

        #region Feeding Method
        /// <summary>
        /// Run program and feed standard input from string
        /// </summary>
        public static string Feed(string program, string arguments, string standardInput)
        {
            string programPath = EnvironmentVariableHelper.FindProgram(program);
            if (programPath == null)
                throw new ArgumentException($"Cannot find program {program}.");

            using Process p = new()
            {
                StartInfo = new()
                {
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    FileName = program,
                    Arguments = arguments
                }
            };
            p.Start();

            StreamWriter redirectStreamWriter = p.StandardInput;
            redirectStreamWriter.WriteLine(standardInput);
            redirectStreamWriter.Close();

            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            return output;
        }
        #endregion

        #region Piping Methods
        /// <summary>
        /// Start a pipeline with program
        /// </summary>
        public static Pipeline Pipe(string program)
        {
            string programPath = EnvironmentVariableHelper.FindProgram(program);
            if (programPath == null)
                throw new ArgumentException($"Cannot find program {program}.");

            using Process p = new()
            {
                StartInfo = new()
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    FileName = program
                }
            };
            p.Start();

            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            return new Pipeline(output);
        }
        /// <summary>
        /// Start a pipeline with program and arguments
        /// </summary>
        public static Pipeline Pipe(string program, string arguments)
        {
            string programPath = EnvironmentVariableHelper.FindProgram(program);
            if (programPath == null)
                throw new ArgumentException($"Cannot find program {program}.");

            using Process p = new()
            {
                StartInfo = new()
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    FileName = program,
                    Arguments = arguments
                }
            };
            p.Start();

            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            return new Pipeline(output);
        }
        /// <summary>
        /// Start a pipeline with program and arguments in array format
        /// </summary>
        public static Pipeline Pipe(string program, params string[] arguments)
        {
            string programPath = EnvironmentVariableHelper.FindProgram(program);
            if (programPath == null)
                throw new ArgumentException($"Cannot find program {program}.");

            using Process p = new()
            {
                StartInfo = new()
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    FileName = program,
                    Arguments = string.Join(" ", arguments.Select(EscapeArgument))
                }
            };
            p.Start();

            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            return new Pipeline(output);
        }
        /// <summary>
        /// Start a pipeline program and arguments in dictionary format
        /// </summary>
        public static Pipeline Pipe(string program, Dictionary<string, string> arguments)
        {
            string programPath = EnvironmentVariableHelper.FindProgram(program);
            if (programPath == null)
                throw new ArgumentException($"Cannot find program {program}.");

            using Process p = new()
            {
                StartInfo = new()
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    FileName = program,
                    Arguments = string.Join(" ", arguments.Select(p => $"{p.Key} {EscapeArgument(p.Value)}"))
                }
            };
            p.Start();

            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            return new Pipeline(output);
        }
        #endregion

        #region Helpers
        private static string EscapeArgument(string original)
        {
            if (original.Contains('"'))
                return "\"" + original.Replace("\"", "\\\"") + "\"";
            else if (original.Contains(' '))
                return "\"" + original + "\"";
            else
                return original;
        }
        #endregion
    }
}