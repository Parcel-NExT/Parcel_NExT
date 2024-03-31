using Parcel.CoreEngine.Contracts;
using Parcel.CoreEngine.Document;

namespace Parcel.CoreEngine.Standardization
{
    /// <summary>
    /// Provides some static extension functions related to standard parsing, meta-data comprehension and information extraction of Parcel Nodes
    /// </summary>
    public static class NodeContextHelper
    {
        #region Connections
        /// <summary>
        /// Checks whethere a node has input dependancies.
        /// </summary>
        /// <remarks>
        /// Notice an input attribute (a front-end concept) is different from an actual input connection.
        /// </remarks>
        public static bool HasInputs(this ParcelNode node)
        {
            return node.Attributes.Keys.Any(k => k.StartsWith('@'));
        }
        public static string[] GetInputnames(this ParcelNode node)
        {
            return node.Attributes.Keys.Where(k => k.StartsWith('@')).Select(k => k.TrimStart('@')).ToArray();
        }
        #endregion

        #region Tagging
        public static bool IsProcessorNode(this ParcelNode node)
        {
            return !node.Tags.Contains(SystemTags.AnnotationTag);
        }
        #endregion
    }
}
