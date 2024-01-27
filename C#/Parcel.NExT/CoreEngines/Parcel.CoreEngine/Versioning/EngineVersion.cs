namespace Parcel.CoreEngine.Versioning
{
    /// <summary>
    /// This is the only version across the entire Parcel.NExT implementation and considers changes to all components including front-end and back-end
    /// The version number should match release number on Github, and match the csproj build version
    /// </summary>
    public static class EngineVersion
    {
        public static readonly string Version = "Parcel.NExT v0.0.1 (Build: 2024.01)";

        /// <summary>
        /// Format: version, key summary, component highlights, timeline.
        /// </summary>
        public static readonly string Changelog = """
            Parcel.NExT
            v 0.0.1: Initialization. 2024.01 - Now.
            """;
    }
}
