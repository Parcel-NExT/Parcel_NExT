namespace Parcel.CoreEngine.Helpers
{
    /// <summary>
    /// Provide standard parsing for node attribute names
    /// </summary>
    public static class NodeAttributeNameHelper
    {
        public static string GetNameOnly(string annotatedAttributeKey)
        {
            // Extract attribute name from annotated syntax
            return annotatedAttributeKey.Split(':').First().TrimStart('<').TrimEnd('>');
        }
    }
}
