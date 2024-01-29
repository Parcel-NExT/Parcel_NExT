namespace Parcel.CoreEngine.InstructionSets
{
    public static class InstructionSetMapping
    {
        /// <remarks>
        /// A fully automatic FRONTEND should be able to identify ALL known names and map to correponding assemblies (and use defaults if necessary). 
        /// This is NOT to be handled by the engine/backend!
        /// </remarks>
        public static Dictionary<string, string> ParcelCustomNameMappings = new Dictionary<string, string>()
        {
            // Standard Aliaes
            { "Print", "System.Console.dll:System.Console.Write" }
        };
    }
}
