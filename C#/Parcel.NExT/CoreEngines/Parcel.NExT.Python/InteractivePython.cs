using Python.Runtime;
using System.Text.RegularExpressions;
using System.Text;

namespace Parcel.NExT.Python;

public static class StringHelper
{
    public static string[] SplitCommandLineArguments(this string inputString, char separator = ',', bool includeQuotesInString = false)
    {
        List<string> parameters = [];
        StringBuilder current = new();

        bool inQuotes = false;
        foreach (var c in inputString)
        {
            if (c == '"')
            {
                inQuotes = !inQuotes;
                if (includeQuotesInString)
                    current.Append(c);
            }
            else if (c == separator)
            {
                if (!inQuotes)
                {
                    parameters.Add(current.ToString());
                    current.Clear();
                }
                else
                    current.Append(c);
            }
            else
            {
                current.Append(c);
            }
        }
        if (current.Length != 0)
            parameters.Add(current.ToString());
        return parameters.ToArray();
    }
}

public static class RuntimeHelper
{
    public static string? FindPythonDLL()
    {
        foreach (string path in SplitArgumentsLikeCsv(Environment.GetEnvironmentVariable("PATH")!, ';', true))
        {
            try
            {
                foreach (var file in Directory.EnumerateFiles(path))
                {
                    string filename = Path.GetFileName(file);
                    if (Regex.IsMatch(filename, @"python\d{2,3}\.dll"))
                        return file;
                }
            }
            // Remark-cz: Certain paths might NOT be enumerable due to access issues
            catch (Exception) { continue; }
        }
        return null;

        static string[] SplitArgumentsLikeCsv(string line, char separator = ',', bool ignoreEmptyEntries = false)
        {
            string[] arguments = line.SplitCommandLineArguments(separator);

            if (ignoreEmptyEntries)
                return arguments.Where(a => !string.IsNullOrWhiteSpace(a)).ToArray();
            else
                return arguments;
        }
    }
}

public sealed class InteractivePython
{
    public InteractivePython()
    {
        var installedPython = RuntimeHelper.FindPythonDLL();
        if (installedPython == null)
            throw new ArgumentException("Cannot find any usable Python installation on the machine.");

        Runtime.PythonDLL = installedPython;
        PythonEngine.Initialize();
        using (Py.GIL())
        {
            PythonScope = Py.CreateScope();
        }
    }
    PyModule PythonScope;

    public object? Evaluate(string scripts)
    {
        if (PythonScope == null)
            throw new ApplicationException("Python runtime is not initialized.");

        using (Py.GIL())
        {
            // Remark: Notice ipython is able to retrieve last result, however this is not the typical behavior of python repl
            // Remark-cz: At the moment we are not able to guess "last result"; But we definitely want to
            try
            {
                return PythonScope.Eval(scripts);
            }
            catch (Exception nonExpression)
            {
                try
                {
                    PythonScope.Exec(scripts);
                }
                catch (Exception realException)
                {
                    Console.WriteLine(realException.Message);
                }
                
                return null;
            }

            // Clue: Ipython does something like this: https://github.com/ipython/ipython/blob/main/IPython/core/displayhook.py which might involve sys.displayhook and builtins._
            // But do notice that `a = 5` in both python repl and ipython returns None
        }
    }
}
