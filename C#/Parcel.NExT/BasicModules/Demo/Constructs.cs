using Parcel.NExT.Interpreter.Scripting;

namespace Demo
{
    public static class Constructs
    {
        /// <summary>
        /// A simple execute code node: executes code in a local function scope. The code must have a single return string return value that tells which next exec pin to pick.
        /// </summary>
        public static void Exec(string codeFragment)
        {
            Dictionary<string, string> globals = new()
            {
                { "Next", "Default" }
            };
            ContextFreeRoslyn.EvaluateLocalLogic(codeFragment)
        }
    }
}
