using System.Reflection;

namespace Parcel.CoreEngine.Service.Interpretation
{
    /// <summary>
    /// Notice this doesn't deal with state isolation at all; 
    /// For proper state isolation, we should depend on Parcel.NExT.Interpreter
    /// </summary>
    public static class InvokationService
    {
        public static void InvokeRemoteFunction(string assemblyPath, string functionPath, params object[] arguments)
        {
            int splitterIndex = functionPath.LastIndexOf('.'); ;
            string typeName = functionPath.Substring(0, splitterIndex);
            string function = functionPath.Substring(splitterIndex + 1);

            // Alternatively, use Activator.CreateInstance(assemblyName, typeName)
            Assembly assembly = Assembly.Load(assemblyPath.Replace(".dll", string.Empty));    // LoadFrom()
            Type type = assembly.GetType(typeName);
            MethodInfo staticMethod = type.GetMethod(function, [arguments.GetType()]);
            object returnValue = staticMethod.Invoke(null, arguments);
        }
    }
}
