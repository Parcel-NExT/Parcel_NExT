using Parcel.CoreEngine.Helpers;
using Parcel.NExT.Python.Helpers;
using Parcel.Types;
using Python.Runtime;

namespace Zora.DomainSpecific.CGI
{
    public class Modeler
    {
        #region Explicit Session
        private readonly string TempBlendFilePath = Parcel.Standard.System.FileSystem.GetTempFilePath(".blend");
        private readonly string TempImageFilePath = Image.GetTempImagePath();

        private dynamic BlenderModule { get; }
        private Modeler()
        {
            ValidateDependencies();
            PythonRuntimeHelper.TryInitializeEngine();
            using (Py.GIL())
            {
                // Load module
                BlenderModule = Py.Import("bpy");

                // Initial configuration
                BlenderModule.ops.wm.save_as_mainfile(filepath: TempBlendFilePath);
                BlenderModule.ops.wm.open_mainfile(filepath: TempBlendFilePath);
                BlenderModule.context.scene.render.filepath = TempImageFilePath;
                BlenderModule.context.scene.render.resolution_percentage = 40;
            }
        }
        #endregion

        #region Primitives
        public static Modeler MakeCube()
        {
            return new();
        }
        #endregion

        #region Operations
        public void Bevel()
        {
            // PENDING
        }
        #endregion

        #region Routines
        /// <summary>
        /// Generates a preview render of current scene
        /// </summary>
        public Image GetPreviewRender()
        {
            BlenderModule.ops.render.render(write_still: true);
            return new(TempImageFilePath, false); // Don't save as reference since we are reusing same file and that will cause writing issues when making changes
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
