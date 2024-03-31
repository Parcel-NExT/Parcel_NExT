
using Tranquility.Services;

namespace Tranquility
{
    public static class Logging
    {
        #region Output Writer
        public static TextWriter? StandardOutput { get; internal set; }
        #endregion

        #region Semantic Logging
        public static void Info(string message, bool print = true)
        {
            Log($"[Info] {message}", print);
        }
        #endregion

        #region Methods
        public static void Log(string message, bool print = true)
        {
            DateTime date = DateTime.Now;
            string outputLine = $"{date:yyyy-MM-dd HH:mm:ss} - {message}";

            if (print)
            {
                PrintToStandardOutput(outputLine);
            }
        }
        #endregion

        #region Helpers
        public static void PrintToStandardOutput(string outputLine)
        {
            lock (ConsoleSessionRedirectedTextWriter.ConsoleStateChangeLock)
            {
                if (StandardOutput != null)
                {
                    var currentWriter = Console.Out;
                    Console.SetOut(StandardOutput);
                    Console.WriteLine(outputLine);
                    Console.SetOut(currentWriter);
                }
            }
        }
        #endregion
    }
}
