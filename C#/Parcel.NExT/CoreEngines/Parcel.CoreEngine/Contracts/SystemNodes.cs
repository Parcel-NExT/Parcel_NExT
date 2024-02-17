namespace Parcel.CoreEngine.Contracts
{
    public static class SystemNodes
    {
        /// <summary>
        /// Those are the reserved names of certain nodes serving special functions to the runtime
        /// TODO: Can we make those unique names even shorter?
        /// </summary>
        public static string[] ReservedNodeTargetNames = [
            "Parcel-Construct:Graph", // Denotes a graph reference
            "Parcel-Construct:Subgraph", // Denotes a subgraph region
            "Parcel-Construct:Custom", // Denotes a customly specified target path
            "Parcel-Construct:FunctionStart", // Denotes functional function start
            "Parcel-Construct:FunctionReturn",  // Denotes functional function return/end

            "Parcel-Frontend:Preview", // Denotes a preview node
        ];
    }
}
