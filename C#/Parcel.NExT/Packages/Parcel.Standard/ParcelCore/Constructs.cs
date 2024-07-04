using Parcel.CoreEngine.Primitives;
using Parcel.NExT.Interpreter.Scripting;

namespace Parcel.Standard.ParcelCore
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
