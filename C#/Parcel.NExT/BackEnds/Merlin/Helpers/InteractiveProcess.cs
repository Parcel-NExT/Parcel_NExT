using System.Diagnostics;
using System.Text;

namespace Merlin.Helpers
{
    /// <summary>
    /// Emutates a REPL like process
    /// </summary>
    /// <remarks>
    /// Notice this implementation expects line based sending/reciving and explicit [EOF] handshake and won't work on arbitrary proceses.
    /// </remarks>
    public class InteractiveProcess
    {
        private const string EndOfMessageSymbol = "[EOF]";
        
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
            InputStream.WriteLine(EndOfMessageSymbol);
            // Simulate EOF by sending Ctrl+Z and flushing
            //InputStream.Write((char)26); // ASCII code for Ctrl+Z
            //InputStream.Flush();

            StringBuilder output = new();
            string? line = OutputStream.ReadLine();
            while (line != EndOfMessageSymbol)
            {
                output.AppendLine(line);
                line = OutputStream.ReadLine();
            }
            return output.ToString().TrimEnd();
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
