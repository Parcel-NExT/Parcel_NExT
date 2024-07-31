using System.Reflection;

namespace Parcel.NExT.Interpreter.Helpers
{
    public static class MethodInfoHelper
    {
        public static string GetMethodSignature(this MethodInfo method)
        {
            return $"{method.DeclaringType!.Name}.{method.Name}({string.Join(", ", method.GetParameters().Select(p => p.ParameterType.GetFormattedName()))})";
        }
        public static string GetMethodFullSignature(this MethodInfo method)
        {
            return $"{method.DeclaringType!.Name}.{method.Name}({string.Join(", ", method.GetParameters().Select(p => p.ParameterType.GetFormattedName()))})->{method.ReturnType.GetFormattedName()}";
        }
        public static string GetConstructorFullSignature(this ConstructorInfo constructor)
        {
            return $"{constructor.DeclaringType!.Name}.{constructor.Name}({string.Join(", ", constructor.GetParameters().Select(p => p.ParameterType.GetFormattedName()))})->{constructor.DeclaringType.GetFormattedName()}";
        }
    }
}
