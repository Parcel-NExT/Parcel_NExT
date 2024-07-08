using Parcel.CoreEngine.Helpers;

namespace Parcel.NExT.Python.Helpers
{
    public static class PythonRuntimeHelper
    {
        public static string? FindPythonDLL()
        {
            return EnvironmentVariableHelper.EnumerateEnvironmentVariablesSearchingForFilePattern(["PATH"], @"python\d{2,3}\.dll");
        }
        public static string? FindPythonPip()
        {
            return EnvironmentVariableHelper.EnumerateEnvironmentVariablesSearchingForFilePattern(["PATH"], @"pip.exe");
        }        
    }
}
