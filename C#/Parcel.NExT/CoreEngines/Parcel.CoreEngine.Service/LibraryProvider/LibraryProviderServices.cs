using Parcel.CoreEngine.Contracts;
using Parcel.CoreEngine.Service.Interpretation;
using Parcel.CoreEngine.Service.Types;
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
    /// Methods here do NOT need to return string-serialized values and can return any general type that's serializable (though in general should avoid complex types) - serialization is the responsibility of back-ends.
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
        /// <summary>
        /// Get plain list of names of attributes available on a target
        /// </summary>
        public string[]? GetTargetAttributes(string targetPath)
        {
            // TODO: Consult and merge implementation of GraphRuntime.ExecuteNode
            GraphRuntime.NodeTargetPathProtocolStructure target = GraphRuntime.ParseNodeTargets(targetPath);
            TargetEndPoint? endpoint = ResolveTarget(target);
            if (endpoint == null)
                return null;
            switch (endpoint.Nature)
            {
                case EndPointNature.Type:
                    IEnumerable<string> members = endpoint.Type!.GetFields(BindingFlags.Public | BindingFlags.Instance).Select(f => $"{f.Name}:{f.FieldType.Name}");
                    IEnumerable<string> properties = endpoint.Type!.GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(p => $"{p.Name}.{p.PropertyType.Name}");
                    return [.. members, .. properties];
                case EndPointNature.StaticMethod:
                    IEnumerable<string> arguments = endpoint.Method!.GetParameters().Select(p => $"{p.Name}:{p.ParameterType.Name}");
                    string returnAttribute = $"result:{endpoint.Method!.ReturnType.Name}";
                    return [.. arguments, returnAttribute];
                    break;
                case EndPointNature.InstanceMethod:
                    throw new NotImplementedException();
                case EndPointNature.System:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentException();
            }
        }
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
        public Dictionary<string, SimplexString> GetModuleMembers(string moduleName)
        {
            Assembly assembly = Assembly.LoadFrom(moduleName);
            // TODO: Might want to remove types that are *Options aka. define function options (maybe they can inherit from specific class? Or we just use naming convention as filter: <MethodName>Options struct).
            Type[] types = assembly.GetExportedTypes()
                .Where(t => t.Name != "Object").ToArray();
            MethodInfo[] methods = types.SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
                // Every static class seems to export the methods exposed by System.Object, i.e. Object.Equal, Object.ReferenceEquals, etc. and we don't want that.
                .Where(m => m.DeclaringType != typeof(object))
                .ToArray();

            Dictionary<string, SimplexString> members = [];
            members.Add("Methods", new(methods.Select(m => $"{m.DeclaringType!.Name}.{m.Name}({string.Join(", ", m.GetParameters().Select(p => p.ParameterType.Name))})").OrderBy(n => n).ToArray()));
            members.Add("Types", new(types.Select(t => t.Name).OrderBy(n => n).ToArray()));
            members.Add("Module", new(moduleName));
            return members;
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
                // TODO: Remark-cz: This is temporary indexing - due to various reasons there are types with same names and this will not work perfectly; We need better identification methods
                if (!endpoints.ContainsKey(type.Name))
                    endpoints.Add(type.Name, new TargetEndPoint(EndPointNature.Type, type.Name, type, null));
            foreach (MethodInfo method in exportedStaticMethods)
            {
                string identifier = $"{method.DeclaringType!.Name}.{method.Name}({string.Join(", ", method.GetParameters().Select(p => p.ParameterType.Name))})";
                // TODO: Similar to above, we need better identification names
                if (!endpoints.ContainsKey(identifier))
                    // TODO: Remark: Notice we are exporting just the names of methods because we consider them "top-level"
                    endpoints.Add(identifier, new TargetEndPoint(EndPointNature.StaticMethod, method.Name, method.DeclaringType, method));
            }
            foreach (var name in SystemNodes.ReservedNodeTargetNames)
                endpoints.Add(name, new TargetEndPoint(EndPointNature.System, name, null, null));

            return endpoints;
        }
        /// <summary>
        /// Given a target protocol, find the matching endpoint to use.
        /// </summary>
        private TargetEndPoint? ResolveTarget(GraphRuntime.NodeTargetPathProtocolStructure target)
        {
            // TODO: Remark-cz: At the moment we are using simple names as match, more robust ways of doing this might be desirable
            return TargetEndPoints.TryGetValue(target.TargetPath, out TargetEndPoint? value) ? value : null;
        }
        #endregion
    }
}
