using Parcel.CoreEngine;

namespace Parcel.NExT.CodeGen
{
    public struct ProjectGenerationOptions
    {
        public enum OptimizationLevel
        {
            /// <summary>
            /// Keeps all node/graph names
            /// </summary>
            Verbose,
            /// <summary>
            /// Use as little code as possible while maintaining readability, fully utilize dependency solver
            /// </summary>
            Succinct
        }

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
        public bool UseTopLevelStatementsForEntryScript { get; set; }
        public OptimizationLevel Optimization { get; set; }
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
        /// <summary>
        /// Generate source files, build, and return final executable.
        /// Throws exceptions during compilation errors.
        /// </summary>
        public string GenerateProject(ParcelDocument document, ProjectGenerationOptions options)
        {
            CSharpScriptExecutableGenerator.ScriptFile[] scripts = document.GraphNodes
                .Select(p => CodeGenerator.GenerateGraphCodes(options.ProjectName, document, p.Key, p.Value, document.MainGraph == p.Key, options.UseTopLevelStatementsForEntryScript))
                .ToArray();

            // Provide main function (Program.cs) when we have more than one graphs and the main graph is not using top level statement
            if (document.Graphs.Count != 1 && !options.UseTopLevelStatementsForEntryScript)
                scripts = [MakeMainScript(options.ProjectName, document), .. scripts];

            // TODO: Handle build errors (throw as exception)
            // Returns executable file path
            string executable = new CSharpScriptExecutableGenerator().Generate(options.ProjectName, null, scripts, options.SourceCodeOutputFolder, options.BuildOutputFolder, options.ProduceSingleExecutable, out string messages); // TODO: Support dependencies
            return executable;
        }

        #region Routines
        private CSharpScriptExecutableGenerator.ScriptFile MakeMainScript(string projectName, ParcelDocument document)
        {
            return new CSharpScriptExecutableGenerator.ScriptFile("Program.cs", $$"""
                namespace {{projectName}}
                {
                    public static class Program
                    {
                        public static void Main(string[] args)
                        {
                            throw new NotImplementedException();
                        }
                    }
                }
                """);
        }
        #endregion
    }
}
