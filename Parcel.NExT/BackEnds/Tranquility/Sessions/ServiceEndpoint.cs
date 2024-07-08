using Parcel.CoreEngine.Service;
using System.Reflection;

namespace Tranquility.Sessions
{
    public record ServiceEndpoint(ServiceProvider Provider, MethodInfo Method);
}
