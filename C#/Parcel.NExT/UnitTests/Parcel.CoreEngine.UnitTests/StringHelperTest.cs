using Parcel.CoreEngine.Helpers;

namespace Parcel.CoreEngine.UnitTests
{
    public class StringHelperTest
    {
        [Fact]
        public void ParametersSplitShouldConsiderQuotes()
        {
            Assert.Equal(2, "hello world".SplitCommandLineArguments().Length);
            Assert.Equal(2, "Command \"Argument 1\"".SplitCommandLineArguments().Length);
            Assert.Equal("Argument 2", "Command \"Argument 1\" \"Argument 2\"".SplitCommandLineArguments().Last());
        }
        [Fact]
        public void ParametersSplitShouldConsiderIncludeQuotes()
        {
            Assert.Equal(2, "hello world".SplitCommandLineArguments(true).Length);
            Assert.Equal(2, "Command \"Argument 1\"".SplitCommandLineArguments(true).Length);
            Assert.Equal("\"Argument 2\"", "Command \"Argument 1\" \"Argument 2\"".SplitCommandLineArguments(true).Last());
        }
    }
}