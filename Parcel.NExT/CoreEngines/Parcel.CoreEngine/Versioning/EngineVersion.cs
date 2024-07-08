namespace Parcel.CoreEngine.Versioning
{
    /// <summary>
    /// This is the only version across the entire Parcel.NExT implementations and considers changes to all components including (canonical) front-end and back-end; (This version number may exclude certain front-ends)
    /// The version number should match release number on Github, and match the csproj build version
    /// </summary>
    public static class EngineVersion
    {
        public static readonly string Version = "Parcel.NExT v0.0.3 (Build: 2024.03)";

        /// <summary>
        /// Format: version, key summary, component highlights, timeline.
        /// </summary>
        public static readonly string Changelog = """
            Parcel.NExT
            v0.0.1: Initialization. 2024.01 - 2024.02.
            v0.0.2: Service class implementation; Tranquility. 2024.02 - 2024.03.
            v0.0.3: Ama runtime engine. 2024.03 - Now.
            """;
    }
}
