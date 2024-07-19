using System.Reflection;

namespace Parcel.CoreEngine.Interfaces
{
    /// <summary>
    /// Represents a method/constructor/delegate that can be called.
    /// In general, this is equivalent to MethodInfo but it enables containing constructors.
    /// </summary>
    public sealed class Callable
    {
        public enum CallableType
        { 
            Constructor,
            StaticMethod,
            InstanceMethod
        }

        #region Property
        private ConstructorInfo? Constructor { get; set; }
        private MethodInfo? Method { get; set; }
        
        public CallableType Type { get;  }
        public bool IsStatic { get; }
        /// <remarks>
        /// Comparing to DeclaringType, ReflectedType takes inheritance into consideration.
        /// </remarks>
        public Type ReflectedType { get; }
        public Type DeclaringType { get; }
        public Type ReturnType { get; }

        public bool IsConstructor => Type == CallableType.Constructor;
        #endregion

        #region Constructor
        public Callable(MethodInfo method)
        {
            Method = method;

            Type = method.IsStatic ? CallableType.StaticMethod : CallableType.InstanceMethod;
            IsStatic = method.IsStatic;
            ReturnType = method.ReturnType;
            ReflectedType = method.ReflectedType!;
            DeclaringType = method.DeclaringType!;
        }
        public Callable(ConstructorInfo constructor)
        {
            Constructor = constructor;
            
            Type = CallableType.Constructor;
            IsStatic = true;
            ReturnType = constructor.ReflectedType!;
            ReflectedType = constructor.ReflectedType!;
            DeclaringType = constructor.DeclaringType!;
        }
        #endregion

        #region Queries
        public ParameterInfo[] GetParameters()
            => Constructor != null ? Constructor.GetParameters() : Method!.GetParameters();
        #endregion

        #region Method
        public object? Invoke(object? instance, object?[]? arguments)
        {
            if (Type != CallableType.Constructor)
                return Method!.Invoke(instance, arguments);
            else throw new InvalidOperationException("Cannot call `Invoke` on constructor with instance parameter. Use `StaticInvoke` instead.");
        }
        public object? StaticInvoke(object?[]? arguments)
        {
            if (Type != CallableType.StaticMethod && Type != CallableType.Constructor)
                throw new InvalidOperationException($"Callable is not static.");

            if (Type == CallableType.StaticMethod)
                return Method!.Invoke(null, arguments);
            else
                return Constructor!.Invoke(arguments);
        }
        public object? InstanceInvoke(object? instance, object?[]? arguments)
        {
            if (Type != CallableType.InstanceMethod)
                throw new InvalidOperationException($"Callable is not instance based.");

            return Method!.Invoke(instance, arguments);
        }
        #endregion
    }
}
