using Parcel.CoreEngine.Helpers;

namespace Parcel.Processing.Utilities
{
    /// <summary>
    /// Process scripting language.
    /// (Goal) Thread-safe, fully contextual.
    /// String based variables.
    /// </summary>
    public sealed class PSL
    {
        #region Properties
        private const char VariableSymbol = '$';
        public string CurrentWorkingDirectory { get; private set; }
        public Dictionary<string, string> Variables { get; }
        #endregion

        #region Construction
        public PSL(string? workingDirectory = null, Dictionary<string, string>? initialVariables = null) 
        {
            CurrentWorkingDirectory = workingDirectory ?? Directory.GetCurrentDirectory();
            Variables = initialVariables ?? [];
        }
        #endregion

        #region Single-Shot Interface
        public static PSL Process(string script, string? workingDirectory = null, Dictionary<string, string>? initialVariables = null)
        {
            PSL context = new(workingDirectory, initialVariables);
            foreach (string line in SplitScriptLines(script))
                context.Feed(line);
            return context;
        }
        #endregion

        #region Low-Level Stated Interface
        public bool IsFinished { get; private set; }
        public void Feed(string? input)
        {
            if (IsFinished) return;

            // Ignore empty and comment lines
            if (string.IsNullOrWhiteSpace(input)) return;
            input = input.Trim();
            if (input.StartsWith('#')) return;

            string[] arguments = input.SplitCommandLine().ToArray();
            // Handle variable creation
            if (arguments.Length == 3 && arguments.First().Equals("set", StringComparison.CurrentCultureIgnoreCase))
            {
                Variables[arguments[1]] = arguments[2];
                return;
            }
            if (arguments.Length == 3 && arguments[0].StartsWith('$') && arguments[1].Equals("=", StringComparison.CurrentCultureIgnoreCase))
            {
                Variables[arguments[0].TrimStart('$')] = arguments[2];
                return;
            }
            // Handle variable printing
            if (arguments.Length == 1 && arguments[0].StartsWith('$'))
            {
                Console.WriteLine(Variables.TryGetValue(arguments[0].TrimStart('$'), out string? value) ? value : string.Empty);
                return;
            }

            // Handle commands
            HandleCommand(arguments.Select(ParseVariable).ToArray());
        }
        #endregion

        #region Routines
        private void HandleCommand(string[] arguments)
        {
            if (arguments.Length == 0) return;
            string command = arguments.First();

            // Handle built-in commands (case-insensitive)
            switch (command.ToLower())
            {
                case "exit":
                    IsFinished = true;
                    break;
                case "cd":
                    CurrentWorkingDirectory = arguments.Skip(1).FirstOrDefault() ?? CurrentWorkingDirectory;
                    break;
                case "ls":
                    int maxWidth = Directory.EnumerateFileSystemEntries(CurrentWorkingDirectory).Max(f => Path.GetFileName(f).Length);
                    string[] entries = [.. Directory.EnumerateDirectories(CurrentWorkingDirectory).OrderBy(f => Path.GetFileName(f)), .. Directory.EnumerateFiles(CurrentWorkingDirectory).OrderBy(f => Path.GetFileName(f))];
                    foreach (string entry in entries)
                        Console.WriteLine($"{Path.GetFileName(entry).PadRight(maxWidth)} {(Directory.Exists(entry) ? $"Folder (x{Directory.EnumerateFileSystemEntries(entry).Count()})" : $"{new FileInfo(entry).Length:N2} bytes")}");
                    break;
                case "cat":
                    string? file = arguments.Skip(1).FirstOrDefault();
                    if (File.Exists(file))
                    {
                        string extension = Path.GetExtension(file).ToLower().Trim();
                        if (new FileInfo(file).Length > 10 * 1024 * 1024) // Limit 10Mb
                            Console.WriteLine("File size exceeds 10Mb");
                        else if (new string[] { ".exe", ".dll", ".mp3", ".avi", ".mkv", ".wav", ".jpg", ".png", ".gif", ".obj", ".fbx" }.Contains(extension))
                            Console.WriteLine("Cannot view binary file.");
                        else Console.WriteLine(File.ReadAllText(file));
                    }
                    break;
                case "echo":
                    Console.WriteLine(arguments.Skip(1).FirstOrDefault() ?? string.Empty);
                    break;
                default:
                    break;
            }

            // Handle as ordinary program
            string output = string.Empty;
            try
            {
                if (arguments.Length == 1)
                    output = PipelineUtilities.Run(command);
                else
                    output = PipelineUtilities.Run(command, arguments.Skip(1).ToArray()); // TODO: Handle REPL cases
                if (!string.IsNullOrEmpty(output))
                    Console.WriteLine(output);
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}");
            }
        }
        private string ParseVariable(string statement)
        {
            if (!statement.StartsWith(VariableSymbol))
                return statement;

            // TODO: Provide more advanced expression evaluation; Notice we cannot use Parcel.Calculator or anything else because this package must be AOT.
            string key = statement[1..];
            if (Variables.TryGetValue(key, out string? value))
                return value;
            else
                return string.Empty; // Invalid variables just return empty string as result
        }
        #endregion

        #region Helpers
        public static string[] SplitScriptLines(string script)
            => script.SplitLines(true);
        #endregion
    }
}
