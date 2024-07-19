using System.Collections;
using System.Reflection;

namespace Parcel.CoreEngine.Service.Interpretation
{
    /// <summary>
    /// Provide services for coercion at method and parameter level,
    /// Provide services for converting well-known objects from strings to values and vice versa,
    /// Provide serivces for converting well-known objects from C# (Parcel) to Python.
    /// </summary>
    public static class InteroperationHelper
    {
        #region Coercion Related
        /// <summary>
        /// Tests as first positional argument for the target method whether this operation needs to be coerced
        /// </summary>
        public static bool ShouldCoerce(Type incomingArgumentType, MethodInfo targetMethod)
        {
            Type expectedFirstArgumentType = targetMethod.GetParameters().First().ParameterType;
            // Plain array
            if (incomingArgumentType.IsArray && incomingArgumentType.GetElementType() == expectedFirstArgumentType)
                return true;
            // Enumerable
            else if (incomingArgumentType.IsGenericType && incomingArgumentType.IsAssignableTo(typeof(IEnumerable))
                && incomingArgumentType.GetGenericArguments().Length == 1 && incomingArgumentType.GenericTypeArguments.First() == expectedFirstArgumentType)
                return true;
            else
                return false;
        }
        #endregion

        #region Arguments for Methods

        #endregion

        #region Arguments for Object Instantiation

        #endregion

        #region Runtime Type Conversion

        #endregion
    }
}
