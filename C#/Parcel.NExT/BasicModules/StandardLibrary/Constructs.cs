using Parcel.CoreEngine.SemanticTypes;
using Parcel.NExT.Interpreter.Scripting;

namespace StandardLibrary
{
    public static class Constructs
    {
        /// <summary>
        /// A simple execute code node: executes code in a local function scope. The code must have a single return string return value that tells which next exec pin to pick.
        /// </summary>
        public static string Exec(Text codeFragment)
        {
            return ContextFreeRoslyn.EvaluateLocalReturn<string>(codeFragment.Value);
        }
    }
}
