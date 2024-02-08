namespace Parcel.NExT.Python.UnitTests
{
    public class IndependantScopeTests
    {
        [Fact]
        public void Test1()
        {
            var result = PythonRuntimeHelper.RunScopedSnippetAsFunctionBody("""
                return 15
                """);
            Assert.Equal(15, (int)result.AsManagedObject());
        }
    }
}