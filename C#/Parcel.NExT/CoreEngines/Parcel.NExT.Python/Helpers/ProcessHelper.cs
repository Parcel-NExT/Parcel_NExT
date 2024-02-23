using System.Diagnostics;

namespace Parcel.NExT.Python.Helpers
{
    public static class ProcessHelper
    {
        public static string GetOutput(string program, params string[] args)
        {
            ProcessStartInfo startInfo = new()
            {
                FileName = program,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                Arguments = args.JoinAsArguments()
            };

            // Start the process
            var process = new Process
            {
                StartInfo = startInfo
            };
            process.Start();

            // Get outputs
            var outputs = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return outputs;
        }

        public static string GetOutputWithInput(string program, string[] args, string input)
        {
            using Process p = new()
            {
                StartInfo = new()
                {
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    FileName = program,
                    Arguments = args.JoinAsArguments()
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
    }
}
