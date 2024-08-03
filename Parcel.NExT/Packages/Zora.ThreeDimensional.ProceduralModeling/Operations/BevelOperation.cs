namespace Zora.DomainSpecific.CGI.Operations
{
    internal class BevelOperation(double width = 0.5) : OperationProcedure
    {
        #region Properties
        public double Width { get; set; } = width;
        #endregion

        #region Implementation
        public override void Execute(dynamic blenderModule)
        {
            dynamic obj = blenderModule.context.active_object;
            dynamic mod = obj.modifiers.@new("Bevel", "BEVEL");
            mod.width = Width;
        }
        #endregion
    }
}
