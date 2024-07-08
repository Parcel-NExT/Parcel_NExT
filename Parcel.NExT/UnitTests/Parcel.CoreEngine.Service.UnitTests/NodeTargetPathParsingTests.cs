using static Parcel.CoreEngine.Service.Interpretation.GraphRuntime;

namespace Parcel.CoreEngine.Service.UnitTests
{
    public class NodeTargetPathParsingTests
    {
        [Fact]
        public void ShouldDecipherFullComponentsAndCommonVariations()
        {
            {
                NodeTargetPathProtocolStructure targets = ParseNodeTargets(@"{Preview}");
                Assert.Equal("Preview", targets.System);
            }

            {
                NodeTargetPathProtocolStructure targets = ParseNodeTargets(@"[C#]System.Console.dll:System.Console.WriteLine");
                Assert.Equal("C#", targets.EngineSymbol);
                Assert.Equal("System.Console.dll", targets.AssemblyPath);
                Assert.Equal("System.Console.WriteLine", targets.TargetPath);
            }

            {
                NodeTargetPathProtocolStructure targets = ParseNodeTargets(@"Console.WriteLine");
                Assert.Equal("Console.WriteLine", targets.TargetPath);
            }
        }
    }
}
