namespace Parcel.Foundational
{
    /// <summary>
    /// Provides a common interface for defining package meta-data;
    /// Provides strongly typed in-assembly plain-data retrievable values providing all informations about the package available at runtime.
    /// Provides standard facilities that automates package meta-data dumping.
    /// 
    /// All properties are optional.
    /// </summary>
    public abstract class ParcelPackageMetadataBase
    {
        #region Basic Information
        /// <summary>
        /// Preferred display name of the package; May be ignored by specific front-end
        /// </summary>
        public virtual string FriendlyName { get; }
        /// <summary>
        /// Provides version of package, should be the same as assembly file version
        /// </summary>
        public virtual string Version { get; }
        /// <summary>
        /// Provides brief description of package
        /// </summary>
        public virtual string Description { get; }
        /// <summary>
        /// Provides textual description of package dependancies, should be the same as assembly project file dependancies and as shown in Nuget
        /// </summary>
        public virtual string Dependancies { get; }
        #endregion

        #region Node, Node Parameters and Node Returns Mapping

        #endregion

        #region Advanced Node Parameters Semantic Typing

        #endregion

        #region Package Usage Examples and Node Usage Examples
        /// <remarks>
        /// Could be the same as README file if the package is available as open source
        /// </remarks>
        public virtual string HighlevelUsageDocumentation { get; }
        #endregion

        #region Whole-Package Documentation and Manual

        #endregion

        #region Automation Interface
        /// <summary>
        /// Provides standardway of outputing package metadata as files, e.g. for packaging purpose and for static documentation website hosting.
        /// </summary>
        public virtual void DumpMetadataFiles(string outputFolder)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
