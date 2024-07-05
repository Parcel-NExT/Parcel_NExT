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
            CommandHistory history = new();
            while (true)
            {
                PrintPromptSymbol();
                string? input = Console.ReadLine(); // TODO: Enable command history by replacing Console.ReadLine() with ReadLineOrEsc(). At the moment ReadLineOrEsc() is not ready to be used due to lack of proper left/right arrow and improper buffer handling.

                // Process
                context.Feed(input);
                if (input != null)
                    history.AddCommand(input);

                if (context.IsFinished)
                    return;
            }
        }
        #endregion

        #region Helpers
        // TODO: Complete the behavior to ordinary ReadLine() - currently we cannot use left/right arrow to edit
        // TODO: Enhance experience to support Ctrl-Left/Right for word skipping
        private static string? ReadLineOrEsc(CommandHistory history)
        {
            string returnString = "";

            int currentIndex = Console.CursorLeft;
            do
            {
                ConsoleKeyInfo readKeyResult = Console.ReadKey(true);

                // Handle Esc
                if (readKeyResult.Key == ConsoleKey.Escape)
                {
                    Console.WriteLine();
                    return null;
                }

                // Handle Enter
                if (readKeyResult.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    return returnString;
                }

                // Handle backspace
                if (readKeyResult.Key == ConsoleKey.Backspace)
                {
                    if (currentIndex > 0)
                    {
                        returnString = returnString.Remove(returnString.Length - 1);
                        Console.Write(readKeyResult.KeyChar);
                        Console.Write(' ');
                        Console.Write(readKeyResult.KeyChar);
                        currentIndex--;
                    }
                }
                // Handle up arrow
                else if (readKeyResult.Key == ConsoleKey.UpArrow)
                {
                    // Get history
                    string? lastCommand = history.GetCommand(-1);
                    if (lastCommand != null)
                    {
                        // Clear
                        ClearCurrentConsoleLine();
                        PrintPromptSymbol();
                        // Preview
                        Console.Write(lastCommand);
                        // Save
                        returnString = lastCommand;
                    }
                }
                // Handle all other keypresses
                else
                {
                    returnString += readKeyResult.KeyChar;
                    Console.Write(readKeyResult.KeyChar);
                    currentIndex++;
                }
            }
            while (true);
        }
        private static void PrintPromptSymbol()
        {
            var foreground = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write("> ");
            Console.ForegroundColor = foreground;
        }
        private static void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }
        #endregion
    }
}
