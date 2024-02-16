namespace Parcel.CoreEngine.Contracts
{
    public enum NodeTargetRuntime
    {
        CSharp,
        Python,
        ParcelNative
    }

    /// <summary>
    /// Provides detailed information regarding node type binding/endpoint/target type definition, mostly to inform both the runtime engine and front end what the target type is and where to find it
    /// </summary>
    /// <remarks>
    /// TODO: Consider merge this with NodeTargetPathProtocolStructure, though do notice this is intended to be a plain generic data-only descriptor, and we DO NOT wish this to be bound to regex parsing protocol.
    /// </remarks>
    public struct NodeTargetDescriptor
    {
        public NodeTargetRuntime Runtime;

    }
}
