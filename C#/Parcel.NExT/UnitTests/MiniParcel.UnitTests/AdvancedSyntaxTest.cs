using Parcel.CoreEngine.MiniParcel;
using Parcel.CoreEngine.Service.CoreExtensions;

namespace MiniParcel.UnitTests
{
    public class AdvancedSyntaxTest
    {
        #region Tests
        [Fact]
        public void FileSystemEntryRegexRenamePipelineSingleLine()
        {
            string tempFolderPath = CreateInitialFiles();
            MiniParcelService.Parse($"""
                GetFiles {tempFolderPath} | Enhance !#(Replace ":File_Sequence_00(.)" :File-Sequence-00$1) | Rename
                """).Execute();
            VerifyFolderFiles(tempFolderPath);
        }

        [Fact]
        public void FileSystemEntryRegexRenamePipelineSingleLineShorter()
        {
            string tempFolderPath = CreateInitialFiles();
            MiniParcelService.Parse($"""
                ls {tempFolderPath} | Enhance !#(Replace ":File_Sequence_00(.)" :File-Sequence-00$1) | Rename
                """).Execute();
            VerifyFolderFiles(tempFolderPath);
        }

        /// <summary>
        /// This one uses many features of MiniParcel syntax besides standard library functions: Pipeline, Default Positional Arguments, Annoymous Graphs (not lambda, no default captures) with heads-up processing (Implicit Annoymous Graph), Enhance-Reduce-Map-Filter operations, Action Passing (as arguments, in-line array initialization, Explicit Annoymous Graph, in-line evaluations, line comments
        /// </summary>
        [Fact]
        public void FileSystemEntryRegexRenamePipelineRobustMultiplines()
        {
            string tempFolderPath = CreateInitialFiles();
            MiniParcelService.Parse($"""
                final = GetFiles {tempFolderPath}
                    | Objectify :FullPath
                    | Enhance :FolderPath !GetOnlyFolder
                    | Enhance :FileName !GetOnlyName
                    | Enhance :ReplacementName !#(  ~ Implicit Graph
                        GetAttribute :ReplacementName
                        | Replace ":File_Sequence_00(.)" :File-Sequence-00$1
                    ) 
                    | Enhance :NewPath !#(.    ~ Explicit graph
                        a = GetAttribute :FolderPath
                        c = CombinePath @a (GetAttribute :ReplacementName)
                        Return @c
                    )
                    | Reduce [:FolderPath, :FileName, :ReplacementName]
                
                ~ We use a name instead continue the pipeline just to test name assignment is working fine
                Rename final
                """).Execute();
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