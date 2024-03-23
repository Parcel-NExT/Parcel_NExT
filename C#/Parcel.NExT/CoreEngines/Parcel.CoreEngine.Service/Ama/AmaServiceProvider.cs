using Parcel.CoreEngine.Contracts;
using Parcel.CoreEngine.Conversion;
using Parcel.CoreEngine.Document;
using Parcel.CoreEngine.Helpers;
using Parcel.CoreEngine.Service.LibraryProvider;
using System.Reflection;

namespace Parcel.CoreEngine.Service.Interpretation
{
    /// <summary>
    /// A context-safe interpolation stated service provider;
    /// It provides "step-wise" construction of the entire document.
    /// </summary>
    public sealed class AmaServiceProvider: ServiceProvider
    {
        #region Constructor
        public AmaServiceProvider() 
        {
            _parcelDocument = new();
        }
        #endregion

        #region States
        private ParcelDocument _parcelDocument;
        ParcelDocument ParcelDocument { 
            get
            {
                if (_parcelDocument == null) 
                    _parcelDocument = new ParcelDocument();
                return _parcelDocument;
            }
        }
        #endregion

        #region Basic Single-Node Evaluation
        /// <summary>
        /// Ask server to replicate a node. The client will be responsible for feeding all definition information about the node - this is essential especially for front-end only nodes, which the server should just store without questioning what's it used for.
        /// </summary>
        public long CreateNode(string target, string graph, string tags, IDictionary<string, object> attributes)
        {
            // TODO: Make use of `graph` and `tags`
            // Add node to document
            return ParcelDocument.AddNode(ParcelDocument.MainGraph, new ParcelNode(target, attributes.ToDictionary(v => v.Key, v => v.Value.ToString()!)), new System.Numerics.Vector2());
        }
        public object? EvaluateNode(int nodeID)
        {
            if (!_parcelDocument.NodeGUIDs.Reverse.Contains(nodeID))
                throw new ArgumentException($"Invalid nodeID: {nodeID}. This indicates a application-level implementation error. Check states and makes sure front-end and in-sync with backend.");

            ParcelNode node = _parcelDocument.NodeGUIDs.Reverse[nodeID];
            try
            {
                return EvaluateNodeImplementation(node);
            }
            catch (ArgumentException e)
            {
                // TODO: Return a payload containing error
                return PayloadConstructionHelper.ConstructError(node, new ParcelNodeArgumentException(e.Message));
            }
            catch (Exception e)
            {
                // TODO: Return a payload containing error
                return PayloadConstructionHelper.ConstructError(node, new ParcelNodeExecutionException(e.Message));
            }
        }
        #endregion

        #region Routines
        private object? EvaluateNodeImplementation(ParcelNode node)
        {
            // TODO: Change this. Make use of existing node payloads and states instead of valuating from scratch.

            string target = node.Target;
            Dictionary<string, string> attributes = node.Attributes;

            Dictionary<string, string> formattedAttributes = attributes
                .ToDictionary(a => FormatAttribute(a.Key), a => a.Value);

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
                return NodeAttributeNameHelper.GetNameOnly(annotatedAttribute);
            }
        }
        #endregion
    }
}
