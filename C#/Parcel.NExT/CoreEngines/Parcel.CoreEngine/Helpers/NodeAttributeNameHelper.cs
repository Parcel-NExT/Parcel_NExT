using Parcel.CoreEngine.Document;

namespace Parcel.CoreEngine.Helpers
{
    /// <summary>
    /// Provide standard parsing for node attribute names
    /// </summary>
    public static class NodeAttributeNameHelper
    {
        #region Name Syntax
        public static string GetNameOnly(string annotatedAttributeKey)
        {
            // Extract attribute name from annotated syntax
            return annotatedAttributeKey.Split(':').First().TrimStart('<').TrimEnd('>');
        }
        #endregion

        #region Values Syntax
        public static bool IsAttributeValuePayloadReference(string attributeValue)
        {
            return attributeValue.StartsWith('#');
        }
        public static bool IsAttributeValueNodeReference(string attributeValue)
        {
            return attributeValue.StartsWith('@');
        }
        #endregion

        #region Document Extension
        public static (ParcelNode Node, string AttributeName) DereferenceNodeAttribute(this ParcelDocument document, string reference)
        {
            if (!IsAttributeValueNodeReference(reference))
                throw new ArgumentException($"Invalid reference syntax: {reference}");

            string[] parts = reference.TrimStart('@').Split('.');
            string name = parts[0];
            string attribute = parts[1];
            if (long.TryParse(name, out long id))
                return (document.NodeGUIDs.Reverse[id], attribute);
            else
                return (document.Nodes.Single(n => n.Name == name), attribute);
        }
        #endregion
    }
}
