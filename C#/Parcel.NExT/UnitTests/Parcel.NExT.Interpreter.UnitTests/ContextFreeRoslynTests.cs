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
                    ContextFreeRoslyn.LowLevelRunLocalNoReturn("""
                    using System.Linq;
                    """);
                });
            }

            {
                Assert.Throws<CompilationErrorException>(() =>
                {
                    ContextFreeRoslyn.LowLevelRunLocalNoReturn("""
                    public record MyRecord(string Name, double Value);
                    """);
                });
            }

            {
                ScriptState<object> result = ContextFreeRoslyn.LowLevelRunLocalNoReturn("""
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
            Dictionary<string, object> result = ContextFreeRoslyn.EvaluateLocalSnippet(initials, """
            Scalar = 7;
            Compound = new Vector3(2, 4, 5);
            Instance[0] = "Hola";
            """);
            Assert.Equal(7, result["Scalar"]);
            Assert.Equal(4, ((Vector3)result["Compound"]).Y);
            Assert.Equal("Hola", ((string[])result["Instance"])[0]);
        }
        [Fact]
        public void LocalReturnFunctionsShouldReturnWhateverTypeRequired()
        {
            // Without global state
            {
                Dictionary<string, object> globals = new()
                {
                    { "InitialValue", "Hello World!" }
                };
                string result = ContextFreeRoslyn.EvaluateLocalReturn<string>("""
                    return InitialValue;
                    """, globals);
                Assert.Equal(globals["InitialValue"], result);
            }

            // With global state
            {
                int result = ContextFreeRoslyn.EvaluateLocalReturn<int>("""
                    return 5;
                    """);
                Assert.Equal(5, result);
            }
        }
        [Fact]
        public void CodeGenShouldBeAbleToHandleAutomaticImportingWellKnownSystemTypes()
        {
            try
            {
                ContextFreeRoslyn.LowLevelRunLocalNoReturn("""
                    Vector3 myVector = new Vector3(1, 2, 3);
                    """, autoImport: true);
            }
            catch (Exception e)
            {
                Assert.Fail("Expected no exception, but got: " + e.Message);
            }
        }
    }
}