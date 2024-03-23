namespace Parcel.CoreEngine.Contracts
{
    /// <summary>
    /// Base exception for runtime user-facing application-level node-related exceptions (i.e. non-runtime/engine-level/implementation exceptions)
    /// </summary>
    public abstract class ParcelNodeRuntimeException: Exception
    {
        public ParcelNodeRuntimeException() { }
        public ParcelNodeRuntimeException(string message) : base(message) { }
        public ParcelNodeRuntimeException(string message, Exception inner) : base(message, inner) { }
    }
    public sealed class ParcelNodeArgumentException: ParcelNodeRuntimeException
    {
        public ParcelNodeArgumentException(){}
        public ParcelNodeArgumentException(string message): base(message) {}
        public ParcelNodeArgumentException(string message, Exception inner) : base(message, inner) {}
    }
    public sealed class ParcelNodeExecutionException : ParcelNodeRuntimeException
    {
        public ParcelNodeExecutionException() { }
        public ParcelNodeExecutionException(string message) : base(message) { }
        public ParcelNodeExecutionException(string message, Exception inner) : base(message, inner) { }
    }
}
