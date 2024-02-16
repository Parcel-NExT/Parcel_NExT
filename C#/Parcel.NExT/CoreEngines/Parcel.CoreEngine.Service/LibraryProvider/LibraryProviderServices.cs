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
                .. LibraryServiceHelper.GetLoadedAssemblies().Select(a => a.FullName),
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
        /// Get all valid endpoints including types and (global/static) methods; Excluding type specific instance methods
        /// </summary>
        /// <remarks>
        /// Potentially heavy
        /// </remarks>
        public string[] GetAllTargetPaths()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
