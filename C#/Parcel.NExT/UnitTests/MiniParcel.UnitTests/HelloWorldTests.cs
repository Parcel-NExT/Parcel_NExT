using Parcel.CoreEngine.MiniParcel;
using Parcel.CoreEngine.Service.CoreExtensions;

namespace MiniParcel.UnitTests
{
    public class HelloWorldTests
    {
        #region Tests
        [Fact]
        public void VerYbasicHelloWorld()
        {
            string tempFilePath = Path.GetTempFileName();
            MiniParcelService.Parse($"""
            WriteFile "{tempFilePath}" "Hello World!"
            """).Execute();
            Assert.Equal("Hello World!", File.ReadAllText(tempFilePath));
        }

        [Fact]
        public void BasicMath()
        {
            string tempFilePath = Path.GetTempFileName();
            MiniParcelService.Parse($"""
            A = Number 5
            B = Number 6
            C = Multiply @A @B
            WriteFile "{tempFilePath}" @C
            """).Execute();
            Assert.Equal($"{5 * 6}", File.ReadAllText(tempFilePath));
        }

        [Fact]
        public void FileSystemEntryRegexRenaming()
        {
            // Setup initial files
            string tempFolderPath = CreateInitialFiles();

            // Regex rename in MiniParcel
            // Remark: The goal is to have a functional programming syntax yet shorter and cleaner than Elixir, and in a declarative way
            MiniParcelService.Parse($"""
            Default:
                FileList = GetFiles "{tempFolderPath}"
                FileRenameList = Enhance @FileList !#Rename ~ This is new
                Apply @FileRenameList !MoveFile
                ~ Since MoveFile supports object arrays, an alternative way it just to call the following:
                ~ MoveFile @FileRenameList
            Rename:
                OriginalName = GetArgument :0
                FileName = System.IO.Path.GetFileNameWithoutExtension @OriginalName
                NewName = Replace @FileName ":File_Sequence_00(.)" ":File-Sequence-00$1"
                Return @NewName
            """).Execute();

            // Compare names
            VerifyFolderFiles(tempFolderPath);
        }
        [Fact]
        public void FileSystemEntryRegexRenamingEnhancedSyntax()
        {
            // Setup initial files
            string tempFolderPath = CreateInitialFiles();

            // Regex rename in MiniParcel using advanced syntax (tentative) - supporting in-line evaluations
            // Remark: Automatic name discovery or "smart referencing"
            MiniParcelService.Parse($"""
            Default:
                FileList = GetFiles "{tempFolderPath}"
                FileRenameList = Enhance @FileList !#Rename
                Apply @FileRenameList !MoveFile
            Rename:
                FileName = GetOnlyName (GetArgument :0)
                Return (Replace @FileName ":File_Sequence_00(.)" ":File-Sequence-00$1")
            """).Execute();
            // Remark: Can we make it shorter, simpler and more functional, and more flexible?

            // Compare names
            VerifyFolderFiles(tempFolderPath);
        }
        #endregion

        #region Routines

        private static string CreateInitialFiles()
        {
            string tempFilePath = Path.GetTempFileName();
            Directory.CreateDirectory(tempFilePath);
            for (int i = 0; i < 9; i++)
                File.Create(Path.Combine(tempFilePath, $"File_Sequence_00{i}.txt"));
            return tempFilePath;
        }
        private static void VerifyFolderFiles(string tempFolderPath)
        {
            foreach (var name in Directory.EnumerateFiles(tempFolderPath)
                .Select(p => Path.GetFileNameWithoutExtension(p)))
                Assert.Matches(@"File-Sequence-00\d", name);
        }
        #endregion
    }
}