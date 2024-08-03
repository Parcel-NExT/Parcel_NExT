using Parcel.CoreEngine.Helpers;
using Parcel.NExT.Python;
using Parcel.NExT.Python.Helpers;
using Parcel.Types;
using Python.Runtime;

namespace Parcel.DomainSpecific.CGI
{
    public sealed class Object3D
    {

    }

    public class Modeler
    {
        #region Session (Obsolete)
        private static InteractivePython _scriptSession;
        private static InteractivePython ScriptSession
        {
            get => _scriptSession ??= new();
        }
        #endregion

        #region Explicit Session
        private static Modeler _modeler;
        private static Modeler Session
        {
            get => _modeler ??= new Modeler();
        }
        private dynamic BlenderModule { get; }
        private Modeler()
        {
            ValidateDependencies();

            var installedPython = PythonRuntimeHelper.FindPythonDLL();
            if (installedPython == null)
                throw new ArgumentException("Cannot find any usable Python installation on the machine.");

            Runtime.PythonDLL = installedPython;
            PythonEngine.Initialize();
            using (Py.GIL())
            {
                BlenderModule = Py.Import("bpy");
            }
        }
        #endregion

        #region Primitives
        public static Image MakeCube()
        {
            string imagePath = Image.GetTempImagePath();
            string blendPath = Parcel.Standard.System.FileSystem.GetTempFilePath(".blend");

            Modeler session = Session;
            session.BlenderModule.ops.wm.save_as_mainfile(filepath: blendPath);
            session.BlenderModule.ops.wm.open_mainfile(filepath: blendPath);
            session.BlenderModule.context.scene.render.filepath = imagePath;
            session.BlenderModule.ops.render.render(write_still: true);
            return new(imagePath, true);
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
