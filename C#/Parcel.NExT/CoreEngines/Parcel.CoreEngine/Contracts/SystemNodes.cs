namespace Parcel.CoreEngine.Contracts
{
    public static class SystemNodes
    {
        #region Named Constants
        public const string GraphReferenceNodeTarget = "Parcel.Graph:Construct";
        public const string SubgraphContainerNodeTarget = "Parcel.Subgraph:Construct";
        public const string KnotNodeTarget = "Parcel.Knot:Construct";
        public const string CustomNodeTarget = "Parcel.Custom:Construct";
        public const string SetObjectPropertyValueNodeTarget = "Parcel.Set:Construct";
        public const string GetObjectPropertyValueNodeTarget = "Parcel.Get:Construct";
        public const string FunctionDefinitionStartNodeTarget = "Parcel.FunctionStart:Construct";
        public const string FunctionDefinitionReturnNodeTarget = "Parcel.FunctionReturn:Construct";
        public const string StandardPrintNodeTarget = "Parcel.Print:Core";
        public const string FrontEndUsePreviewNodeTarget = "Parcel.Preview:Frontend";
        #endregion

        /// <summary>
        /// Those are the reserved names of certain nodes serving special functions to the runtime
        /// TODO: Can we make those unique names even shorter?
        /// </summary>
        public static readonly string[] ReservedNodeTargetNames = [
            GraphReferenceNodeTarget, // Denotes a graph reference
            SubgraphContainerNodeTarget, // Denotes a subgraph region

            KnotNodeTarget, // Denotes a rerouter
            CustomNodeTarget, // Denotes a customly specified target path

            SetObjectPropertyValueNodeTarget, // Sets value for an object property (e.g. struct or class member)
            GetObjectPropertyValueNodeTarget, // Get value of an object property (e.g. struct or class member)

            FunctionDefinitionStartNodeTarget, // Denotes functional function start
            FunctionDefinitionReturnNodeTarget,  // Denotes functional function return/end

            StandardPrintNodeTarget,  // Print messages to standard output/console (affects both backend and front-end)

            FrontEndUsePreviewNodeTarget, // Denotes a preview node
        ];
    }
}
