using Parcel.CoreEngine.Contracts;
using Parcel.NExT.Python;
using System.Reflection;

namespace Parcel.CoreEngine.Service.LibraryProvider
{
    /// <summary>
    /// Single-entry provider for all service-inquiry related functions.
    /// Methods exposed here are backbone for all backend services related to service inquiry.
    /// </summary>
    /// <remarks>
    /// Methods provides here are generally meaningful for direct backend use, i.e. the return types are typically well-defined serializable primitives, structures, etc.
    /// Methods here do NOT need to return string-serialized values and can return any general type that's serializable (though in general should avoid complex types) - serialization is the responsibility of front-ends.
    /// </remarks>
    public class LibraryProviderServices
    {
        #region Runtime Queries
        public string[] GetAvailableRuntimes()
        {
            if (RuntimeHelper.FindPythonDLL() != null)
                return ["C#", "Python"];
            else
                return ["C#"];
        }
        #endregion

        #region Runtime Specific Module Queries

        #endregion

        #region Reflection Services
        /// <summary>
        /// Get all available public instance methods in this class that are meaningful for backend use.
        /// </summary>
        /// <returns>Return excludes this method itself and some C# standard instance methods</returns>
        public Dictionary<string, MethodInfo> GetAvailableServices()
        {
            var thisType = GetType();
            return thisType
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Except([
                    // Exclude self
                    thisType.GetMethod(nameof(GetAvailableServices)),
                    // Exclude C# framework functions
                    thisType.GetMethod(nameof(ToString)),
                    thisType.GetMethod(nameof(GetType)),
                    thisType.GetMethod(nameof(Equals)),
                    thisType.GetMethod(nameof(GetHashCode)),
                ])
                .ToDictionary(m => m.Name, m => m);
        }
        #endregion

        #region General Queries
        public string[] GetAvailableModules()
        {
            return [
                // Loaded assemblies
                .. LibraryServiceHelper.GetLoadedUserFacingAssemblies().Select(a => a.GetName().Name),
                // TODO: This is just demo
                "System",
                "MathNet",
                "Roslyn",
                "PyTorch"
            ];
        }
        public int GetAvailableModulesCount()
        {
            return GetAvailableModules().Length;
        }
        /// <summary>
        /// Get all valid endpoints including types and (global/static) methods; Excluding type specific instance methods.
        /// TODO: Provide a method to get full NodeTargetDescriptors instead of just names
        /// </summary>
        /// <remarks>
        /// Potentially heavy
        /// </remarks>
        public string[] GetAllTargetPaths()
        {
            // TODO: At the moment we are excluding system to avoid overhead, in the future we definitely want to expose all native runtime targets as well
            // TODO: We also need to make sure behaviors of this function is consistent with many other module related queries above
            // TODO: Might also want to expose public properties and members
            Type[] exportedTypes = LibraryServiceHelper.GetLoadedUserFacingAssemblies()
                .SelectMany(m => m.GetExportedTypes())
                .ToArray();
            MethodInfo[] exportedStaticMethods = exportedTypes
                .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
                .ToArray();
            return [
                // TODO: Standardize path name and make sure identifiable
                .. exportedTypes.Select(t => t.Name), 
                .. exportedStaticMethods.Select(m => m.Name), // TODO: Remark: Notice we are exporting just the names of methods because we consider them "top-level"
                .. SystemNodes.ReservedNodeTargetNames
            ];
        }
        #endregion
    }
}
