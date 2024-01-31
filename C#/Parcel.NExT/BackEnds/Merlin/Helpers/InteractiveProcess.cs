using System.Diagnostics;

namespace Merlin.Helpers
{
    /// <summary>
    /// Emutates a REPL like process
    /// </summary>
    public class InteractiveProcess
    {
        #region Properties
        public string ProgramPath { get; }
        private Process? ChildProcess;
        private StreamWriter? InputStream;
        private StreamReader? OutputStream;
        #endregion

        #region Construction
        public InteractiveProcess(string path)
        {
            ProgramPath = path;
        }
        #endregion

        #region Interface Methods
        public void Start()
        {
            ProcessStartInfo psi = new()
            {
                FileName = ProgramPath,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            // Start the process
            ChildProcess = new Process
            {
                StartInfo = psi
            };
            ChildProcess.Start();

            // Get the input and output streams
            InputStream = ChildProcess.StandardInput;
            OutputStream = ChildProcess.StandardOutput;
        }
        public string SendCommand(string command)
        {
            InputStream.WriteLine(command);

            string output = OutputStream.ReadToEnd();
            return output;
        }
        public void Stop()
        {
            ChildProcess.Kill(); // ChildProcess.WaitForExit();

            // Close the streams
            InputStream.Close();
            OutputStream.Close();

            // Dispose of the process
            ChildProcess.Dispose();
        }
        #endregion
    }
}
