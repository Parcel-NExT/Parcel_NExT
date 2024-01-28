using Parcel.CoreEngine.Document;
using Parcel.CoreEngine.Serialization;

namespace Parcel.CoreEngine.UnitTests
{
    public class BinarySerializationTest
    {
        [Fact]
        public void DocumentNodesShouldReturnCorrectCount()
        {
            string path = Path.GetTempFileName();

            // Generate
            ParcelDocument document = CreateTestDocument();

            // Save
            BinarySerializer.Serialize(document, path);
            // Read
            var loaded = BinarySerializer.Deserialize(path);

            // TODO: Enhance test cases
            // Compare
            Assert.Equal(document.Nodes.Count, loaded.Nodes.Count);
        }

        [Fact]
        public void DocumentNodesShouldHaveCorrectPositions()
        {
            // TODO: Implement test
        }

        [Fact]
        public void PayloadsShouldSerializeCorrectly()
        {
            // TODO: Implement test
        }

        #region Routines
        public static ParcelDocument CreateTestDocument()
        {
            ParcelDocument document = new();
            for (int i = 0; i < 10; i++)
                document.AddNode(document.MainGraph, new ParcelNode($"Node {i}", "WriteLine", new Dictionary<string, string>
                {
                    { "$1", $"Hello from Node {i}!" }
                }), new System.Numerics.Vector2());

            return document;
        }
        #endregion
    }
}
