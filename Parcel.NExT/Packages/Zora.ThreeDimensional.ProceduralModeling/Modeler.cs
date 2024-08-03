using Parcel.CoreEngine.Helpers;
using Parcel.NExT.Python.Helpers;

namespace Parcel.DomainSpecific.CGI
{
    public sealed class Object3D
    {

    }

    public class Modeler
    {
        #region Primitives
        public static Object3D MakeCube()
        {
            ValidateDependencies();

            return new();
        }
        #endregion


        #region Helper
        private static void ValidateDependencies()
        {
            // Check python availability
            string? python = EnvironmentVariableHelper.FindProgram(PythonRuntimeHelper.PythonExecutableName) 
                ?? throw new FileNotFoundException($"Cannot find python on current computer.");

            // Check python version
            string? version = PythonRuntimeHelper.GetPythonVersion();
            string[] validMajorVersions = ["3.10", "3.11", "3.12", "3.13"];
            if (!validMajorVersions.Any(version.Contains))
                throw new InvalidProgramException($"Expects a python version in: {string.Join(", ", validMajorVersions)}");

            // Check pip availability
            string? pip = EnvironmentVariableHelper.FindProgram(PythonRuntimeHelper.PipExecutableName)
                ?? throw new FileNotFoundException($"Cannot find pip on current computer.");

            // Check bpy availability
            string? bpyVersion = PythonRuntimeHelper.GetModuleVersion("bpy");
            if (!bpyVersion.StartsWith("4.2"))
                throw new InvalidProgramException("Expect bpy 4.2+.");
        }
        #endregion
    }
}
