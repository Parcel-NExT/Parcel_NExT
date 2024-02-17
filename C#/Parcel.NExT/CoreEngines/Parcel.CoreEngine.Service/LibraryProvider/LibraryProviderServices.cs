using Parcel.CoreEngine.Contracts;
using Parcel.NExT.Python;
using System.Reflection;

namespace Parcel.CoreEngine.Service.LibraryProvider
{
    public enum EndPointNature
    {
        Type,
        StaticMethod,
        InstanceMethod,
        System // Requires special handling from either frontend or backend or runtime
    }
    /// <summary>
    /// Conceptually either Type or Method will be available as per EndPointNature; 
    /// However, since all methods in C# have a type, when referring to C# endpoints, Type is always available, even for static methods;
    /// For Python, however, since there is no Type, and since there is no C# MethodInfo, only Identifier will be available.
    /// </summary>
    public record TargetEndPoint(EndPointNature Nature, string Identifier, Type? Type, MethodInfo? Method);

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
        #region Properties
        private Dictionary<string, TargetEndPoint>? _targetEndPoints;
        private Dictionary<string, TargetEndPoint> TargetEndPoints
        {
            get
            {
                if (_targetEndPoints == null)
                    _targetEndPoints = IndexTargetEndPoints();
                return _targetEndPoints;
            }
        }
        #endregion

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
                "PyTorch" // We need to try our best to provide available Python modules as well
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
            return [.. TargetEndPoints.Keys];
        }
        #endregion

        #region Routines
        private static Dictionary<string, TargetEndPoint> IndexTargetEndPoints()
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

            Dictionary<string, TargetEndPoint> endpoints = [];
            // TODO: Standardize path name and make sure identifiable
            foreach (Type type in exportedTypes)
                endpoints.Add(type.Name, new TargetEndPoint(EndPointNature.Type, type.Name, type, null));
            foreach (MethodInfo method in exportedStaticMethods)
                // TODO: Remark: Notice we are exporting just the names of methods because we consider them "top-level"
                endpoints.Add(method.Name, new TargetEndPoint(EndPointNature.StaticMethod, method.Name, method.DeclaringType, method));
            foreach (var name in SystemNodes.ReservedNodeTargetNames)
                endpoints.Add(name, new TargetEndPoint(EndPointNature.System, name, null, null));

            return endpoints;
        }
        #endregion
    }
}
