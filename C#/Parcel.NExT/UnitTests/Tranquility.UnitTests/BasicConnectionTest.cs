using Parcel.CoreEngine.Service.LibraryProvider;

namespace Tranquility.UnitTests
{
    public class BasicConnectionTest
    {
        [Fact]
        public void BasicCommunicationTest()
        {
            string request = $"{nameof(LibraryProviderServices)}.{nameof(LibraryProviderServices.GetAvailableRuntimes)}";
            string replyMessage = TranquilityUnitTestHelper.RunSingleRequest(request);
            int count = replyMessage.Split('\n').Length;

            Assert.Equal(new LibraryProviderServices().GetAvailableRuntimes().Length, count);
        }
    }
}