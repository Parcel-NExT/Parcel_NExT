namespace Parcel.CoreEngine.Contracts
{
    public enum NodeTargetRuntime
    {
        // Native/reflection based
        CSharpNative,
        PythonNative,
        ParcelNative,

        // Interpreted
        PythonInterpreted,
        RoslynInterpreted,

        // Macros, Archetype, Preset
        ParcelMacro,
        ParcelArchetype
    }

    /// <summary>
    /// Provides detailed information regarding node type binding/endpoint/target type definition, mostly to inform both the runtime engine and front end what the target type is and where to find it
    /// </summary>
    /// <remarks>
    /// TODO: Consider merge this with NodeTargetPathProtocolStructure, though do notice this is intended to be a plain generic data-only descriptor, and we DO NOT wish this to be bound to regex parsing protocol.
    /// </remarks>
    public struct NodeTargetDescriptor
    {
        #region Runtime
        /// <summary>
        /// Every node has a unique target runtime
        /// </summary>
        public NodeTargetRuntime Runtime;
        #endregion

        #region Interface Naming
        /// <summary>
        /// A human readable name for the target type that's guaranteed to be unique among all nodes in the Parcel ecosystem.
        /// Those are only available for "official" nodes, and not all nodes have such a name.
        /// All those names will be available in some central registry.
        /// A general format for such names shall be: Domain.Category.Node.Variant
        /// </summary>
        public string? UniqueParcelName;
        /// <summary>
        /// A node might have a bunch of aliases for searching purpose.
        /// </summary>
        public string[]? Aliases;
        /// <summary>
        /// A node may have a default display name
        /// </summary>
        public string? DefaultDisplayName;
        #endregion

        #region Formal Name
        /// <summary>
        /// For nodes defined in C#, they must have a namespace
        /// </summary>
        public string? Namespace;
        /// <summary>
        /// Assembly in C#, module in Python
        /// </summary>
        public string Module;
        /// <summary>
        /// For types and C# functions, they all have an encapsulating class container;
        /// For Python, methods can reside in files at global/module scope, so this would be null.
        /// </summary>
        public string? DeclaringType;
        /// <summary>
        /// Type, method, property
        /// </summary>
        public string TargetName;
        #endregion

        #region Metadata
        /// <summary>
        /// The node is referencing into a member property of a typed instance
        /// </summary>
        public bool IsNodeMemberProperty;
        /// <summary>
        /// The node is a call into instance function
        /// </summary>
        public bool IsNodeInstanceFunction;
        /// <summary>
        /// The node refers to a static function
        /// </summary>
        public bool IsNodeGlobalFunction;
        /// <summary>
        /// The node refers to a type and will instantiate an instance of that type
        /// </summary>
        public bool IsNodeObjectInstance;

        /// <summary>
        /// Parcel supports meta-programming and declaration-style type definitions; In such cases, it's dynamically generating and defining new types
        /// </summary>
        public bool IsNodeTypeDefinition;
        #endregion

        #region Addressing
        /// <summary>
        /// Globally unique single-string identifier for the target; A combination (the "Protocol") of values above.
        /// </summary>
        public string FullTargetPath;
        /// <summary>
        /// Globally unique shortest single-string identifier for the target.
        /// </summary>
        public string ShortestTargetPath => UniqueParcelName ?? FullTargetPath;
        #endregion
    }
}
