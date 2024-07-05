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

            // Handle commands
            HandleCommand(input.SplitCommandLine().Select(ParseVariable).ToArray());
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
                default:
                    break;
            }

            // Handle as program
            // TODO: ....
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
