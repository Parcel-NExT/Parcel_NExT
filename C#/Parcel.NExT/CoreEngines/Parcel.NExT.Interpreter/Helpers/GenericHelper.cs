namespace Parcel.NExT.Interpreter.Helpers
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Returns the type name. If this is a generic type, appends the list of generic type arguments between angle brackets.
        /// (Does not account for embedded / inner generic arguments)
        /// </summary>
        /// <remarks>
        /// Handles nested type.
        /// </remarks>
        /// <param name="type">The type.</param>
        /// <returns>System.String.</returns>
        public static string GetFormattedName(this Type type)
        {
            if (type.IsNested)
            {
                if (type.IsGenericType)
                {
                    // Non-standard naming generic types: Enumerator
                    string parentTypeName = type.DeclaringType!.IsGenericType ? GetGenericTypeName(type.DeclaringType!) : type.DeclaringType.Name;
                    return $"{parentTypeName}.{GetGenericTypeName(type)}";
                }
                else
                    return $"{type.DeclaringType!.GetFormattedName()}.{type.Name}";
            }
            else
                return type.IsGenericType ? GetGenericTypeName(type) : type.Name;
        }

        #region Helpers
        private static string GetGenericTypeName(Type type)
        {
            string genericArguments = type.GetGenericArguments()
                .Select(x => x.Name)
                .Aggregate((x1, x2) => $"{x1}, {x2}");
            int containsTilt = type.Name.IndexOf("`");
            string typeName = containsTilt > 0 ? type.Name.Substring(0, containsTilt) : type.Name;
            return $"{typeName}<{genericArguments}>";
        }
        #endregion
    }
}
