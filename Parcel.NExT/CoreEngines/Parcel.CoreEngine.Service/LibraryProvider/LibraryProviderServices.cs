using Parcel.CoreEngine.Primitives;
using Parcel.CoreEngine.Service.Interpretation;
using Parcel.NExT.Interpreter.Helpers;
using Parcel.NExT.Python.Helpers;
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
    public class LibraryProviderServices: ServiceProvider
    {
        #region Runtime Queries
        public string[] GetAvailableRuntimes()
        {
            if (PythonRuntimeHelper.FindPythonDLL() != null)
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
        public override Dictionary<string, MethodInfo> GetAvailableServices()
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
        /// Get plain list of names of attributes available on a target;
        /// This will try its best to conform to POS and contextualize attributes using attribute name syntax, e.g. `<` for input and `>` for outputs;
        /// Notice per convention, node attributes follow camelCase - but this will be implemented on the backend, not core service here.
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
                    IEnumerable<string> arguments = endpoint.Method!.GetParameters().Select(p => $"<{p.Name}:{p.ParameterType.GetFormattedName()}");
                    if (endpoint.Method!.ReturnType == typeof(void))
                        return [.. arguments];
                    else
                    {
                        string returnAttribute = $"result>:{endpoint.Method!.ReturnType.Name}";
                        return [.. arguments, returnAttribute];
                    }
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
            Assembly assembly = Assembly.Load(moduleName); // Remark-cz: Consult https://codingarchitect.wordpress.com/2006/09/11/assembly-load-vs-loadfile-vs-loadfrom/
            // TODO: Might want to remove types that are *Options aka. define function options (maybe they can inherit from specific class? Or we just use naming convention as filter: <MethodName>Options struct).
            Type[] types = assembly.GetExportedTypes()
                .Where(t => t.Name != "Object").ToArray();
            MethodInfo[] methods = types.SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
                // Every static class seems to export the methods exposed by System.Object, i.e. Object.Equal, Object.ReferenceEquals, etc. and we don't want that.
                .Where(m => m.DeclaringType != typeof(object))
                .ToArray();
            Type[] parcelExportTypes = types
                .Where(t => !t.IsAbstract) // Don't want abstract types (including interfaces)
                .Where(t => !(t.IsAbstract && t.IsSealed)) // Don't want static types
                .ToArray();

            Dictionary<string, SimplexString> members = [];
            members.Add("Methods", new(false, methods
                .Select(GetMethodFullSignature)
                .OrderBy(n => n)
                .ToArray()));
            members.Add("Types", new(true, parcelExportTypes.Select(t => t.Name).OrderBy(n => n).ToArray()));
            members.Add("Module", new(false, moduleName));
            return members;
        }
        #endregion

        #region Helpers
        private string GetMethodSignature(MethodInfo method)
        {
            return $"{method.DeclaringType!.Name}.{method.Name}({string.Join(", ", method.GetParameters().Select(p => p.ParameterType.GetFormattedName()))})";
        }
        private string GetMethodFullSignature(MethodInfo method)
        {
            return $"{method.DeclaringType!.Name}.{method.Name}({string.Join(", ", method.GetParameters().Select(p => p.ParameterType.GetFormattedName()))})->{method.ReturnType.GetFormattedName()}";
        }
        #endregion
    }
}
