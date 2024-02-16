using System.Reflection;

namespace Parcel.CoreEngine.Service.LibraryProvider
{
    public static class LibraryServiceHelper
    {
        public static Assembly[] GetLoadedAssemblies()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            return assemblies;
        }
    }
}
