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
        public const string FrontEndLabelAnnotationNodeTarget = "Parcel.Annotation.Label:Frontend";
        public const string FrontEndImageAnnotationNodeTarget = "Parcel.Annotation.Image:Frontend";
        public const string FrontEndAudioAnnotationNodeTarget = "Parcel.Annotation.Audio:Frontend";
        public const string FrontEndShapeAnnotationNodeTarget = "Parcel.Annotation.Shape:Frontend";
        public const string FrontEndVideoAnnotationNodeTarget = "Parcel.Annotation.Video:Frontend";
        public const string PrimitiveNumberNodeTarget = "Parcel.Number:Primitive";
        public const string PrimitiveStringNodeTarget = "Parcel.String:Primitive";
        public const string FlowChartDataEntityNodeTarget = "Parcel.DataEntity:FlowChart";
        public const string FlowChartActionEntityNodeTarget = "Parcel.ActionEntity:FlowChart";
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

            // Basic raw data types
            PrimitiveNumberNodeTarget,
            PrimitiveStringNodeTarget,

            // Generic conceptual flow chart representation
            FlowChartDataEntityNodeTarget, // A representational node for flowchart data entity, it contains no logic and is used just like Custom node but semantically refers to a data input or output. Users can create attributes on this and make connections. All runtimes should generally just ignore this node during evaluation.
            FlowChartActionEntityNodeTarget, // A representational node for flowchart action entity, it contains no logic and is used just like Custom node but semantically refers to an action or processing step. Users can create attributes on this and make connections. All runtimes should generally just ignore this node during evaluation.

            // Frontend Use Annotations
            FrontEndLabelAnnotationNodeTarget,
            FrontEndImageAnnotationNodeTarget,
            FrontEndAudioAnnotationNodeTarget,
            FrontEndShapeAnnotationNodeTarget,
            FrontEndVideoAnnotationNodeTarget,
        ];
    }
}
