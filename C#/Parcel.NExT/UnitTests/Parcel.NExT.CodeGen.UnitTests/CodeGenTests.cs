using System.Diagnostics;
using static Parcel.NExT.CodeGen.CSharpScriptExecutableGenerator;

namespace Parcel.NExT.CodeGen.UnitTests
{
    public class CodeGenTests
    {
        [Fact]
        public void CodeGenShouldBeAbleToGenerateSimplePrograms()
        {
            string tempFile = Path.GetTempFileName();
            string tempFolder = Path.Combine(Path.GetDirectoryName(tempFile)!, Path.GetFileNameWithoutExtension(tempFile));
            Directory.CreateDirectory(tempFolder);

            const string testOutputMessage = "Hello World!";
            string outputExecutable = new CSharpScriptExecutableGenerator().Generate("MyProgram", null, [new ScriptFile("Main", $"""
                Console.WriteLine("{testOutputMessage}");
                """)], tempFolder, true, out string messages);

            // Assert generation is successful
            Assert.True(File.Exists(outputExecutable));

            // Assert output is as expected
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    CreateNoWindow = true,
                    FileName = outputExecutable,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    WorkingDirectory = tempFolder
                },
            };
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            Assert.Equal(testOutputMessage, output.TrimEnd());

            // Clearn up and delete
            Directory.Delete(tempFolder, true);
        }
    }
}
