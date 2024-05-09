using Parcel.CoreEngine;
using Parcel.CoreEngine.MiniParcel;
using System.Diagnostics;

namespace Parcel.NExT.CodeGen.UnitTests
{
    public class ReferentialBuildTests
    {
        [Fact]
        public void Test1()
        {
            string tempFolder = Path.Combine(Path.GetTempPath(), "Parcel", nameof(ReferentialBuildTests));
            string publishFolder = Path.Combine(tempFolder, "Build");
            string sourceFolder = Path.Combine(tempFolder, "Source");

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
                BuildOutputFolder = publishFolder,
                SourceCodeOutputFolder = sourceFolder,

                ImportStandardLibrariesInPlace = true,
                KeepIntermediateSourceFiles = true,
                ProduceSingleExecutable = true,
                ProduceSingleSourceFile = true
            };
            ParcelDocument document = MicroParcelService.Parse(microParcel);
            string executable = new ProjectGenerator().GenerateProject(document, options);

            // Run executable and get standard output
            string output = RunProcess(executable, publishFolder);
            Assert.Equal("Hello World!", output);
        }

        #region Helpers
        private static string RunProcess(string executable, string workingDirectory)
        {
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    CreateNoWindow = true,
                    FileName = executable,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    WorkingDirectory = workingDirectory
                },
            };
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return output.TrimEnd();
        }
        #endregion
    }
}