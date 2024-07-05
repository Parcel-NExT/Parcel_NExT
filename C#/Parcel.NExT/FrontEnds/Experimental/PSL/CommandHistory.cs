using System.Text.RegularExpressions;

namespace ProcessScriptingLanguage
{
    public class CommandHistory
    {
        public record CommandRecord(DateTime Time, string Command);

        #region Construction
        public string HistoryCommandsDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Parcel NExT", "PSL", "CommandsHistory.txt");
        public CommandHistory()
        {
            InitializeCommands();
        }
        #endregion

        #region Methods
        public void InitializeCommands()
        {
            if (!File.Exists(HistoryCommandsDataPath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(HistoryCommandsDataPath)!);
                File.Create(HistoryCommandsDataPath);
            }
        }
        public void AddCommand(string command)
        {
            string record = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {command}\n";
            File.AppendAllText(HistoryCommandsDataPath, record);
        }
        public CommandRecord[] GetCommands()
        {
            return File.ReadLines(HistoryCommandsDataPath)
                .Select(line =>
                {
                    var match = Regex.Match(line, @"\[(.*?)\] (.*)");
                    string timeString = match.Groups[1].Value;
                    string commandString = match.Groups[2].Value;
                    return new CommandRecord(DateTime.Parse(timeString), commandString);
                })
                .ToArray();
        }
        public string? GetCommand(int index)
        {
            try
            {
                if (index < 0)
                    return GetCommands()[^(-index)].Command;
                else
                    return GetCommands()[index].Command;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public void ClearCommands()
        {
            if (File.Exists(HistoryCommandsDataPath))
                File.Delete(HistoryCommandsDataPath);
        }
        #endregion

        #region Navigation

        #endregion
    }
}
