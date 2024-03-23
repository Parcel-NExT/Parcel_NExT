using Parcel.CoreEngine.Contracts;
using Parcel.CoreEngine.Conversion;
using Parcel.CoreEngine.Service.LibraryProvider;
using System.Reflection;

namespace Parcel.CoreEngine.Service.Interpretation
{
    /// <summary>
    /// A context-safe interpolation stated service provider;
    /// </summary>
    public sealed class AmaServiceProvider: ServiceProvider
    {
        #region States
        
        #endregion

        #region Basic Single-Node Evaluation
        public int CreateNode()
        {

        }
        public object? EvaluateNode(int nodeID)
        {
            try
            {
                return EvaluateSingleNodeImplementation(nodeID);
            }
            catch (ArgumentException e)
            {
                // TODO: Return a payload containing error
            }
            catch (Exception e)
            {
                // TODO: Return a payload containing error
            }
        }
        #endregion

        #region Routines
        private object? EvaluateNodeImplementation(int nodeID)
        {
            string target, IDictionary< string, object> attributes;

            Dictionary<string, string> formattedAttributes = attributes
                .ToDictionary(a => FormatAttribute(a.Key), a => (string)a.Value);

            // TODO: At the moment we are not making using `graph` and `tags` arugment
            // TODO: Consult and merge implementation of GraphRuntime.ExecuteNode
            GraphRuntime.NodeTargetPathProtocolStructure targetDef = GraphRuntime.ParseNodeTargets(target);
            TargetEndPoint? endpoint = ResolveTarget(targetDef);
            if (endpoint == null)
                return null;
            switch (endpoint.Nature)
            {
                case EndPointNature.Type:
                    throw new NotImplementedException();
                case EndPointNature.StaticMethod:
                    MethodInfo methodInfo = endpoint.Method!;
                    return methodInfo.Invoke(null, methodInfo.GetParameters().Select(p => StringTypeConverter.ConvertType(p.ParameterType, formattedAttributes[p.Name!])).ToArray());
                case EndPointNature.InstanceMethod:
                    throw new NotImplementedException();
                case EndPointNature.System:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentException();
            }

            static string FormatAttribute(string annotatedAttribute)
            {
                // Extract attribute name from annotated syntax
                return annotatedAttribute.Split(':').First().TrimStart('<').TrimEnd('>');
            }
        }
        #endregion
    }
}
