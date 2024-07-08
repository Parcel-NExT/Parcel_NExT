namespace Parcel.CoreEngine.Contracts
{
    /// <summary>
    /// Base exception for runtime user-facing application-level node-related exceptions (i.e. non-runtime/engine-level/implementation exceptions)
    /// </summary>
    public abstract class ParcelNodeRuntimeExceptionBase: Exception
    {
        public ParcelNodeRuntimeExceptionBase() { }
        public ParcelNodeRuntimeExceptionBase(string message) : base(message) { }
        public ParcelNodeRuntimeExceptionBase(string message, Exception inner) : base(message, inner) { }
    }
    public sealed class ParcelNodeArgumentException: ParcelNodeRuntimeExceptionBase
    {
        public ParcelNodeArgumentException(){}
        public ParcelNodeArgumentException(string message): base(message) {}
        public ParcelNodeArgumentException(string message, Exception inner) : base(message, inner) {}
    }
    public sealed class ParcelNodeExecutionException : ParcelNodeRuntimeExceptionBase
    {
        public ParcelNodeExecutionException() { }
        public ParcelNodeExecutionException(string message) : base(message) { }
        public ParcelNodeExecutionException(string message, Exception inner) : base(message, inner) { }
    }
}
