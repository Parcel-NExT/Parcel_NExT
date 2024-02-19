using Microsoft.CodeAnalysis.Scripting;
using Parcel.NExT.Interpreter.Scripting;
using System.Numerics;

namespace Parcel.NExT.Interpreter.UnitTests
{
    public class ContextFreeRoslynTests
    {
        [Fact]
        public void LocalRunShouldNotBeAbleToDefineTypesOrImportNamespacesOrHavePersistentNonLocalObjects()
        {
            {
                Assert.Throws<CompilationErrorException>(() =>
                {
                    ContextFreeRoslyn.RunLocalNoReturn("""
                    using System.Linq;
                    """);
                });
            }

            {
                Assert.Throws<CompilationErrorException>(() =>
                {
                    ContextFreeRoslyn.RunLocalNoReturn("""
                    public record MyRecord(string Name, double Value);
                    """);
                });
            }

            {
                ScriptState<object> result = ContextFreeRoslyn.RunLocalNoReturn("""
                    var myLocalVariable = 15;
                    """);
                Assert.Null(result.GetVariable("myLocalVariable"));
            }
        }
        [Fact]
        public void RunCodeWithDictionaryAsInitialsShouldReturnDictionaryAsPromised()
        {
            Dictionary<string, object> initials = new()
            {
                { "Scalar", 5 },
                { "Compound", new Vector3(1, 2, 3) },
                { "Instance", new string[]{ "Hello", "World" } }
            };
            Dictionary<string, object> result = ContextFreeRoslyn.LogicLocal(initials, """
            Scalar = 7;
            Compound = new Vector3(2, 4, 5);
            Instance[0] = "Hola";
            """);
            Assert.Equal(7, result["Scalar"]);
            Assert.Equal(4, ((Vector3)result["Compound"]).Y);
            Assert.Equal("Hola", ((string[])result["Instance"])[0]);
        }
    }
}