using Parcel.CoreEngine;

namespace Parcel.NExT.CodeGen
{
    public struct ProjectGenerationOptions
    {
        #region Overall Configurations
        public string ProjectName { get; set; }
        public string SourceCodeOutputFolder { get; set; }
        public string BuildOutputFolder { get; set; }
        /// <summary>
        /// Produces a program that references exiting graphs (either MiniParcel, MicroParcel, PDS as file or plain string, or binary parcel document) instead of a dependency-free C# program;
        /// This is useful for cases where the generated program explicitly depends on and utilizes Parcel services for its functionalities.
        /// If this is true, we will generate the output project using a pre-defined project setup template referencing existing Parcel installation on local machine..
        /// </summary>
        /// <remarks>
        /// This is just project generation/bootstrap, there is very little code generation in this case.
        /// </remarks>
        public bool BuildParcelDependentProgram { get; set; }
        #endregion

        #region Source Code
        public bool KeepIntermediateSourceFiles { get; set; }
        public bool ProduceSingleSourceFile { get; set; }
        #endregion

        #region Package Type
        public bool ProduceSingleExecutable { get; set; }
        public bool ImportStandardLibrariesInPlace { get; set; }
        #endregion

        #region Python Support
        public bool IncludePythonIntegration { get; set; }
        public bool EmbedPython { get; set; }
        #endregion
    }

    public class ProjectGenerator
    {
        public string GenerateProject(ParcelDocument document, ProjectGenerationOptions options)
        {
            // Returns executable file path
            throw new NotImplementedException();
        }
    }
}
