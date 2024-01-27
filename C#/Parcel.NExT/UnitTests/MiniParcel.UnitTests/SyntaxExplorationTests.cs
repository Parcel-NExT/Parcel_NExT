using Parcel.CoreEngine.MiniParcel;
using Parcel.CoreEngine.Service.CoreExtensions;

namespace MiniParcel.UnitTests
{
    public class SyntaxExplorationTests
    {
        #region Basic Syntax
        [Fact]
        public void SumNumbersInCSVColumn_MiniParcelLight()
        {
            string filePath = CreateSampleCSVData();
            string outputPath = Path.GetTempFileName();
            MiniParcelService.Parse($"""
                lines = GetLines {filePath}
                skipped = Skip @lines :1
                | Split :,
                | Pick :2nd ~ Explicit confusion free ordinal symbol
                values = double.Convert strings ~ Supports array input, or automatically coerce to enumerated array (backend feature)
                result = Sum @values
                WriteFile ":{outputPath}" @result
                """).Execute();
            Assert.Equal("150", File.ReadAllText(outputPath));

            static string CreateSampleCSVData()
            {
                var filePath = Path.GetTempFileName();
                return filePath;
            }
        }
        #endregion

        #region Advanced Syntax
        [Fact]
        public void SumNumbersInCSVColumn_MiniParcelAdvanced()
        {
            string filePath = CreateSampleCSVData();
            string outputPath = Path.GetTempFileName();
            MiniParcelService.Parse($"""
                GetLines {filePath}
                | Skip :1
                | Split :,
                | Pick :2nd ~ Explicit confusion free ordinal symbol
                | Convert :double ~ Equivalent to Map !Convert; This invokes dynamic typing (it returns `object` type) and might be tricky for frontend
                ~ Alternatively: | double.Parse, which returns explicit double type
                | Sum
                |< WriteFile ":{outputPath}" ~ Feed last argument, because WriteFile expects first argument to be output file path
                """).Execute();
            Assert.Equal("150", File.ReadAllText(outputPath));

            static string CreateSampleCSVData()
            {
                var filePath = Path.GetTempFileName();
                return filePath;
            }
        }
        #endregion
    }
}