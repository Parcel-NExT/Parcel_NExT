namespace StandardLibrary.DependencyFree
{
    public static class FileUtilities
    {
        /// <summary>
        /// Search keywords in a list of files
        /// </summary>
        /// <returns>Result file and line number containing keywords and a snippet of the line.</returns>
        public static string[] SearchInFiles(params string[] files, string pattern)
        {
            return files
                .Where(File.Exists)
                .Select(Path.GetFullPath)
                .Select(p => (Path: p, Lines: File.ReadLines(p)))
                .Select(pair => (
                    Path: pair.Path,
                    FirstFound: pair.Lines
                        .Select((l, i) => (Line: l, Index: i))
                        .FirstOrDefault(l => l.Line.Contains(pattern))
                ))
                .Where(pair => pair.FirstFound.Line != null)
                .Select(pair => $"{pair.Path}:{pair.FirstFound.Index} \"{pair.FirstFound.Line}\"")
                .ToArray();
        }
    }
}
