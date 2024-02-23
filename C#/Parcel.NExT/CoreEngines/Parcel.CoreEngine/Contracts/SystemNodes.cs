namespace Parcel.CoreEngine.Contracts
{
    public static class SystemNodes
    {
        /// <summary>
        /// Those are the reserved names of certain nodes serving special functions to the runtime
        /// TODO: Can we make those unique names even shorter?
        /// </summary>
        public static readonly string[] ReservedNodeTargetNames = [
            "Parcel.Graph:Construct", // Denotes a graph reference
            "Parcel.Subgraph:Construct", // Denotes a subgraph region

            "Parcel.Custom:Construct", // Denotes a customly specified target path

            "Parcel.Set:Construct", // Sets value for an object property (e.g. struct or class member)
            "Parcel.Get:Construct", // Get value of an object property (e.g. struct or class member)

            "Parcel.FunctionStart:Construct", // Denotes functional function start
            "Parcel.FunctionReturn:Construct",  // Denotes functional function return/end

            "Parcel.Print:Core",  // Print messages to standard output/console (affects both backend and front-end)

            "Parcel.Preview:Frontend", // Denotes a preview node
        ];
    }
}
