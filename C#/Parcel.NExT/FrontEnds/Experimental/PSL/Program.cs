using Parcel.Processing.Utilities;

namespace ProcessScriptingLanguage
{
    internal class Program
    {
        #region Entry
        static void Main(string[] args)
        {
            CLIMode();
        }
        #endregion

        #region Sub-entries
        private static void CLIMode()
        {
            Console.WriteLine("$PSL - Process Scripting Language");
            Console.WriteLine($"Current working directory: {Directory.GetCurrentDirectory()}");
            PSL context = new();
            while (true)
            {
                Console.Write("> ");
                string? input = Console.ReadLine();
                context.Feed(input);
                if (context.IsFinished)
                    return;
            }
        }
        #endregion
    }
}
