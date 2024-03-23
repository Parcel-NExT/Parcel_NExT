using Parcel.CoreEngine.Contracts;
using Parcel.CoreEngine.Conversion;
using Parcel.CoreEngine.DependencySolver;
using Parcel.CoreEngine.Document;
using Parcel.CoreEngine.Service.LibraryProvider;
using Parcel.CoreEngine.Standardization;
using System.Reflection;

namespace Parcel.CoreEngine.Service.Interpretation
{
    /// <summary>
    /// A context-safe interpolation serfice provider; 
    /// The services provided here are stateless, use Ama Service Provider for stated services.
    /// </summary>
    public sealed class InterpolationServiceProvider : ServiceProvider
    {
        #region General Services
        /// <returns>
        /// This function may return null, and backends are responsible for handling it well with suitable protocol with front-ends
        /// </returns>
        /// <remarks>
        /// The attributes will be a Dictionary<string, string> - because of the way upstream handles it, we are accepting IDictionary<string, object> here
        /// </remarks>
        public object? EvaluateSingleNode(string target, IDictionary<string, object> attributes)
        {
            try
            {
                return EvaluateSingleNodeImplementation(target, attributes);
            }
            catch (ArgumentException e)
            {
                throw new ParcelNodeArgumentException(e.Message);
            }
            catch (Exception e)
            {
                throw new ParcelNodeExecutionException(e.Message);
            }
        }
        /// <summary>
        /// Given a self-contained set of Parcel Nodes, evaluate them all and return results for each node.
        /// This function assumes a graph-free loose set of nodes witht interdependancies specified using standard attribute syntax.
        /// The return payload will be in the order of input nodes and have one payload for each node.
        /// </summary>
        public ParcelPayload[] EvaluateSubGraph(ParcelNode[] nodes)
        {
            Dictionary<ParcelNode, ParcelPayload> payloads = [];

            // Solve nodes
            Dictionary<string, ParcelNode> indexedNodes = UniquelyIdentifiableNaming.TagUniqueNamesInSelfContainedNodes(nodes);
            IEnumerable<ParcelNode> sortedNodes = FunctionalDependencySolver.ResolveNodesOrder(nodes, indexedNodes);

            return nodes.Select(n => payloads[n]).ToArray();
        }
        #endregion

        #region Routines
        private object? EvaluateSingleNodeImplementation(string target, IDictionary<string, object> attributes)
        {
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
