using Parcel.CoreEngine.Contracts;
using Parcel.NExT.Interpreter.Scripting;
using Parcel.NExT.Python.Helpers;
using System.Reflection;

namespace Parcel.CoreEngine.Service.LibraryProvider
{
    public static class LibraryServiceHelper
    {
        /// <summary>
        /// Get all loaded assemblies in current application domain that are Parcel-user facing, i.e. exclude Parcel runtime engine specific assemblies
        /// </summary>
        public static Assembly[] GetLoadedUserFacingAssemblies()
        {
            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            Assembly[] assemblies = allAssemblies
                .Except([
                    // Exclude Parcel.CoreEngine
                    typeof(NodeTargetDescriptor).Assembly,
                    // Exclude Parcel.CoreEngine.Services
                    typeof(LibraryServiceHelper).Assembly,
                    // Exclude Parcel.NExT.Interpreter
                    typeof(RoslynContext).Assembly,
                    // Exclude Parcel.NExT.Python
                    typeof(PythonRuntimeHelper).Assembly,
                ])
                .ToArray();
            return assemblies;
        }
    }
}
