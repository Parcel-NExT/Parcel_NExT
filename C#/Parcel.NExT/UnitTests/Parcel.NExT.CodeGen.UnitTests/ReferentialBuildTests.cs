using Parcel.CoreEngine;
using Parcel.CoreEngine.MiniParcel;

namespace Parcel.NExT.CodeGen.UnitTests
{
    public class ReferentialBuildTests
    {
        [Fact]
        public void Test1()
        {
            string tempFolder = Path.Combine(Path.GetTempPath(), "Parcel", nameof(ReferentialBuildTests));

            // Check software development environment
            Assert.True(CodeGenService.CheckSDKsInstalled(false));

            // Defines graph
            string microParcel = """
                Output: WriteLine
                    Value: Hello World!
                """;

            // Parse and generate project
            ProjectGenerationOptions options = new()
            {
                ProjectName = "Temp",
                BuildOutputFolder = Path.Combine(tempFolder, "Build"),
                SourceCodeOutputFolder = Path.Combine(tempFolder, "Source"),

                ImportStandardLibrariesInPlace = true,
                KeepIntermediateSourceFiles = true,
                ProduceSingleExecutable = true,
                ProduceSingleSourceFile = true
            };
            ParcelDocument document = MicroParcelService.Parse(microParcel);
            string executable = new ProjectGenerator().GenerateProject(document, options);

            // Run executable and get standard output
            string output = ProcessHelper.RunProcess(executable);
            Assert.Equal("Hello World!", output);
        }
    }
}