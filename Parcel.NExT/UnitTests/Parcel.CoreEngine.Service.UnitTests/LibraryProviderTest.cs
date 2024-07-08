using Parcel.CoreEngine.Service.LibraryProvider;

namespace Parcel.CoreEngine.Service.UnitTests
{
    public class LibraryProviderTest
    {
        [Fact]
        public void ShouldReturnAtLeastCSharpAsAvailableRuntime()
        {
            Assert.Contains("C#", new LibraryProviderServices().GetAvailableRuntimes());
        }
    }
}
