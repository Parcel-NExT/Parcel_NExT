using System.Text.RegularExpressions;

namespace Parcel.Neo.Base.Toolboxes.FileSystem
{
    public static class FileSystemHelper
    {
        #region Queries
        public static void Fetch(string inputFolder, string inputPattern, out string[] outputEntries, out string[] outputFolders, out string[] outputFiles)
        {
            if (string.IsNullOrWhiteSpace(inputFolder))
                throw new ArgumentException("Input folder is empty.");
            if (!Directory.Exists(inputFolder))
                throw new ArgumentException("Input folder doesn't exist.");

            string[] files = Directory.GetFiles(inputFolder);
            string[] folders = Directory.GetDirectories(inputFolder);

            if (!string.IsNullOrWhiteSpace(inputPattern))
            {
                files = files
                    .Where(e => Regex.IsMatch(Path.GetFileName(e), inputPattern))
                    .ToArray();
                folders = folders
                    .Where(e => Regex.IsMatch(Path.GetFileName(e), inputPattern))
                    .ToArray();
            }

            outputEntries = files.Concat(folders).ToArray();
            outputFiles = files;
            outputFolders = folders;
        }
        public static string[] GetFilePathsWithExtension(string folder, string extension, out string[] fileNames, out string[] names)
        {
            string[] files = Directory.EnumerateFiles(folder)
                .Where(f => Path.GetExtension(f).Equals(extension, StringComparison.InvariantCultureIgnoreCase))
                .ToArray();
            fileNames = files.Select(Path.GetFileName).ToArray();
            names = files.Select(Path.GetFileNameWithoutExtension).ToArray();
            return files;
        }
        public static string[] GetFilePathsWithPattern(string folder, string pattern, out string[] fileNames, out string[] names)
        {
            string[] files = Directory.EnumerateFiles(folder, pattern).ToArray();
            fileNames = files.Select(Path.GetFileName).ToArray();
            names = files.Select(Path.GetFileNameWithoutExtension).ToArray();
            return files;
        }
        #endregion

        #region Modifications
        public static void Rename(string[] inputPaths, string pattern, string replacementPattern, out string[] originalPaths, out string[] newPaths, bool previewOnly = true)
        {
            if (string.IsNullOrWhiteSpace(pattern))
                throw new ArgumentException("Name pattern is empty.");
            if (string.IsNullOrWhiteSpace(replacementPattern))
                throw new ArgumentException("Replacement pattern is empty.");

            originalPaths = inputPaths;
            newPaths = inputPaths
                .Select(originalPath =>
                {
                    string name = Path.GetFileName(originalPath);
                    string newName = Regex.Replace(name, pattern, replacementPattern);
                    string newPath = Path.Combine(Path.GetDirectoryName(originalPath), newName);
                    if (!previewOnly)
                        File.Move(originalPath, newPath);
                    return newPath;
                }).ToArray();
        }
        #endregion
    }
}