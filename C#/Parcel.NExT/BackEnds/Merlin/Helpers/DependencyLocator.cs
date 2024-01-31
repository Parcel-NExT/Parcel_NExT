namespace Merlin.Helpers
{
    public static class DependencyLocator
    {
        public static string GetIntermediaryPath()
        {
            // TODO: Make it more deterministic
            // Remark-cz: Just a temporary way to define the executables
            return Environment.GetEnvironmentVariable("Parcel_Next_Intermediary_Bin");
        }
    }
}
