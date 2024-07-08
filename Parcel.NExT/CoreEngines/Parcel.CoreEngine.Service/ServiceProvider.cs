using Parcel.CoreEngine.Contracts;
using Parcel.CoreEngine.Service.Interpretation;
using Parcel.CoreEngine.Service.LibraryProvider;
using Parcel.NExT.Interpreter.Helpers;
using System.Reflection;

namespace Parcel.CoreEngine.Service
{
    public abstract class ServiceProvider
    {
        #region Reflection Services
        /// <summary>
        /// Get all available public instance methods in this class that are meaningful for backend use.
        /// </summary>
        /// <returns>Return excludes this method itself and some C# standard instance methods</returns>
        public virtual Dictionary<string, MethodInfo> GetAvailableServices()
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

        #region Module Endpoint Indexing
        private Dictionary<string, TargetEndPoint>? _targetEndPoints;
        protected Dictionary<string, TargetEndPoint> TargetEndPoints
        {
            get
            {
                if (_targetEndPoints == null)
                    _targetEndPoints = IndexTargetEndPoints();
                return _targetEndPoints;
            }
        }
        #endregion

        #region Routines
        protected static Dictionary<string, TargetEndPoint> IndexTargetEndPoints()
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
                string identifier = $"{method.DeclaringType!.Name}.{method.Name}({string.Join(", ", method.GetParameters().Select(p => p.ParameterType.GetFormattedName()))})";
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
        protected TargetEndPoint? ResolveTarget(GraphRuntime.NodeTargetPathProtocolStructure target)
        {
            // TODO: Remark-cz: At the moment we are using simple names as match, more robust ways of doing this might be desirable
            return TargetEndPoints.TryGetValue(target.TargetPath, out TargetEndPoint? value) ? value : null;
        }
        #endregion
    }
}
