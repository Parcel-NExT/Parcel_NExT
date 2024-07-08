using Parcel.CoreEngine.Document;

namespace Parcel.CoreEngine.Standardization
{
    /// <summary>
    /// Provides unique naming solving for nodes
    /// </summary>
    public static class UniquelyIdentifiableNaming
    {
        /// <remarks>
        /// Current implementation cares not node target type and only concerns the explicit names.
        /// </remarks>
        public static Dictionary<string, ParcelNode> TagUniqueNamesInSelfContainedNodes(IEnumerable<ParcelNode> nodeSet)
        {
            // TODO: Update standard on how this should be done under POS > PVM

            Dictionary<string, ParcelNode> nodeNames = [];
            foreach (IGrouping<string, ParcelNode> group in nodeSet.GroupBy(n => n.Name))
            {
                string preferredName = group.Key;
                foreach ((ParcelNode Node, int Index) in group.Select((n, i) => (n, i)))
                {
                    string fullAddressName = $"{preferredName}@{Index}";
                    if (Index == 0)
                        fullAddressName = preferredName;

                    nodeNames[fullAddressName] = Node;
                }
            }

            return nodeNames;
        }
    }
}
