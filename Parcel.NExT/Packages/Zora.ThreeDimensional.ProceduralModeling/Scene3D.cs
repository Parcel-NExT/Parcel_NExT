using Parcel.CoreEngine.Helpers;
using Parcel.NExT.Python.Helpers;
using Parcel.Types;
using Python.Runtime;
using Zora.DomainSpecific.CGI.Operations;

namespace Zora.DomainSpecific.CGI
{
    #region Procedural Modeling
    public enum BaseModel
    {
        Cube
    }
    public abstract class OperationProcedure
    {
        public abstract void Execute(dynamic blenderModule);
    }
    public sealed class ModelProcedure
    {
        public BaseModel BaseModel { get; set; }
        public List<OperationProcedure> Operations { get; set; } = [];

        #region Method
        internal void Execute(dynamic blenderModule)
        {
            foreach (OperationProcedure operation in Operations)
                operation.Execute(blenderModule);
        }
        #endregion
    }
    public sealed class SceneSetupProcedure
    {
        public bool Clear { get; set; }
    }
    /// <summary>
    /// An abstraction to represent order of operations on various entities, forming a "program" for final execution.
    /// </summary>
    public sealed class SceneProcedure
    {
        public SceneSetupProcedure SceneSetupProcedure { get; set; } = new();
        public List<Model3D> Models { get; set; } = [];

        #region Method
        internal void Execute(dynamic blenderModule)
        {
            foreach (Model3D model in Models)
                model.Execute(blenderModule);
        }
        #endregion
    }
    #endregion

    public class Model3D
    {
        #region Construction
        private Model3D() { }
        #endregion

        #region Properties
        public ModelProcedure Procedure { get; set; } = new();
        #endregion

        #region Primitives
        public static Model3D CreateCube()
        {
            Model3D model = new();
            return model;
        }
        #endregion

        #region Model Operations
        public void Bevel()
        {
            Procedure.Operations.Add(new BevelOperation());
        }
        #endregion

        #region Method
        internal void Execute(dynamic blenderModule)
        {
            Procedure.Execute(blenderModule);
        }
        #endregion
    }

    /// <summary>
    /// A rerepresentation of final constructed scene, storing procedures and providing rendering visualization.
    /// </summary>
    public class Scene3D
    {
        #region Explicit Session
        private readonly string TempBlendFilePath = Parcel.Standard.System.FileSystem.GetTempFilePath(".blend");
        private readonly string TempImageFilePath = Image.GetTempImagePath();

        private Scene3D(){}
        public SceneProcedure Procedure { get; } = new();
        #endregion

        #region Entry Points
        public static Scene3D CreateScene(params Model3D[] models)
        {
            Scene3D scene = new();
            scene.Procedure.SceneSetupProcedure.Clear = false;
            scene.Procedure.Models = [.. models];
            return scene;
        }
        public static Scene3D CreateEmptyScene()
        {
            Scene3D scene = new();
            scene.Procedure.SceneSetupProcedure.Clear = true;
            return scene;
        }
        #endregion

        #region Scene Operations
        #endregion

        #region Routines
        /// <summary>
        /// Generates a preview render of current scene
        /// </summary>
        /// <remarks>
        /// This is the only place that we actually interact with Python context
        /// </remarks>
        public Image GetPreviewRender()
        {
            dynamic blenderModule;
            ValidateDependencies();
            PythonRuntimeHelper.TryInitializeEngine();
            using (Py.GIL())
            {
                // Load module
                blenderModule = Py.Import("bpy");

                // Initial configuration
                blenderModule.ops.wm.read_homefile(app_template: ""); // This effectivesly restarts current bpy context
                blenderModule.ops.wm.save_as_mainfile(filepath: TempBlendFilePath);
                blenderModule.context.scene.render.filepath = TempImageFilePath;
                blenderModule.context.scene.render.resolution_percentage = 40;

                // Construct in-place
                Procedure.Execute(blenderModule);

                // Generate render
                blenderModule.ops.render.render(write_still: true);
            }

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
