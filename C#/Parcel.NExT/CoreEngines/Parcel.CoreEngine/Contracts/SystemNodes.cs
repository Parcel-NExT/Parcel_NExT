namespace Parcel.CoreEngine.Contracts
{
    public static class SystemNodes
    {
        /// <summary>
        /// Those are the reserved names of certain nodes serving special functions to the runtime
        /// TODO: Can we make those unique names even shorter?
        /// </summary>
        public static readonly string[] ReservedNodeTargetNames = [
            "Parcel-Construct:Graph", // Denotes a graph reference
            "Parcel-Construct:Subgraph", // Denotes a subgraph region

            "Parcel-Construct:Custom", // Denotes a customly specified target path

            "Parcel-Construct:Set", // Sets value for an object property (e.g. struct or class member)
            "Parcel-Construct:Get", // Get value of an object property (e.g. struct or class member)

            "Parcel-Construct:FunctionStart", // Denotes functional function start
            "Parcel-Construct:FunctionReturn",  // Denotes functional function return/end

            "Parcel-Core:Print",  // Print messages to standard output/console (affects both backend and front-end)

            "Parcel-Frontend:Preview", // Denotes a preview node
        ];
    }
}
