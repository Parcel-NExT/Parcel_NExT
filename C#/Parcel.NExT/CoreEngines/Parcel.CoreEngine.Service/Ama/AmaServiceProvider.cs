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
        public long CreateNode(string name, string target, string graph, string tags, IDictionary<string, object> attributes)
        {
            // TODO: Make use of `graph` and `tags`
            // Add node to document
            return ParcelDocument.AddNode(ParcelDocument.MainGraph, new ParcelNode(name, target, attributes.ToDictionary(v => v.Key, v => v.Value.ToString()!)), new System.Numerics.Vector2());
        }
        /// <remarks>
        /// The front-end caller of this function expects all objects return values represent Payload, so we should explicitly return Paylaod from this endpoint
        /// </remarks>
        public ParcelPayload? EvaluateNode(long nodeID, string graph, string tags, IDictionary<string, object> attributes)
        {
            if (!_parcelDocument.NodeGUIDs.Reverse.Contains(nodeID))
                throw new ArgumentException($"Invalid nodeID: {nodeID}. This indicates a application-level implementation error. Check states and makes sure front-end and in-sync with backend.");

            // TODO: Check existing data cache - for purely functional nodes, we may NOT wish to execute unless attributes have changed
            ParcelNode node = _parcelDocument.NodeGUIDs.Reverse[nodeID];
            node.Attributes = attributes.ToDictionary(v => v.Key, v => v.Value.ToString()!);
            try
            {
                return EvaluateNodeResursive(node);
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

        #region Document State Management
        public string? UpdateNodeAttributes(long id, IDictionary<string, object> attributes)
        {
            if (_parcelDocument.NodeGUIDs.Reverse.Contains(id))
                _parcelDocument.NodeGUIDs.Reverse[id].Attributes = attributes.ToDictionary(v => v.Key, v => v.Value.ToString()!);
            return "Success";
        }
        public string? UpdateNodePayload(long id, IDictionary<string, object> values)
        {
            if (_parcelDocument.NodeGUIDs.Reverse.Contains(id))
                _parcelDocument.NodePayloads[_parcelDocument.NodeGUIDs.Reverse[id]].PayloadData = values.ToDictionary(v => v.Key, v => v.Value); // TODO: Implement better marshaling
            return "Success";
        }
        #endregion

        #region Routines
        private ParcelPayload? EvaluateNodeResursive(ParcelNode node)
        {
            // TODO: At the moment we are not making using `graph` and `tags` arugment
            // TODO: Consult and merge implementation of GraphRuntime.ExecuteNode
            string target = node.Target;
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
                    var result = methodInfo.Invoke(null, methodInfo.GetParameters().Select(p => StringTypeConverter.ConvertType(p.ParameterType, GetResolvedAttributeValue(node, p.Name!, true))).ToArray());
                    if (result != null)
                    {
                        var payload = CreatePayloadFromResult(node, result);
                        _parcelDocument.NodePayloads[node] = payload;
                        return payload;
                    }
                    return null;
                case EndPointNature.InstanceMethod:
                    throw new NotImplementedException();
                case EndPointNature.System:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentException();
            }
        }
        private object GetResolvedAttributeValue(ParcelNode node, string parameterName, bool originatingNode)
        {
            Dictionary<string, string> attributes = node.Attributes;
            Dictionary<string, string> formattedAttributes = attributes.ToDictionary(
                a => FormatAttribute(a.Key),
                a => a.Value
            );
            if (!formattedAttributes.TryGetValue(parameterName, out string? attributeValue))
                throw new ArgumentException($"Cannot find parameter for: {parameterName}");

            // Make use of existing node payloads and states instead of valuating from scratch.
            return ResolveAttribute(node, attributeValue, originatingNode);
        }
        #endregion

        #region Helpers
        private static ParcelPayload CreatePayloadFromResult(ParcelNode node, object result)
        {
            if (result is ParcelPayload payload)
                return payload;

            return new ParcelPayload(node, new Dictionary<string, object>()
            {
                { "value", result },
            });
        }
        private object ResolveAttribute(ParcelNode node, string attributeValue, bool originatingNode)
        {
            // Remark: At the moment we are only processing the case where the attribute points to a payload
            // TODO: Provide richer resolutions

            // Resolve to payload
            if (NodeAttributeNameHelper.IsAttributeValuePayloadReference(attributeValue))
            {
                if (!_parcelDocument.NodePayloads.TryGetValue(node, out ParcelPayload? payload))
                {
                    if (originatingNode)
                        throw new ApplicationException("Node is self-referencing uninitialized payload during evaluation. Notice dereferencing self-payload should only be used on nodes AFTER evaluation for value reading purposes.");
                    else
                        EvaluateNodeResursive(node);
                }

                string payloadKey = attributeValue[1..];
                payload = _parcelDocument.NodePayloads[node];
                if (payload!.PayloadData.TryGetValue(payloadKey, out object? value))
                    return value;
                else
                    throw new ArgumentException($"Cannot resolve {payloadKey} on {node.Name} from its payload.");
            }
            else if (NodeAttributeNameHelper.IsAttributeValueNodeReference(attributeValue))
            {
                (ParcelNode Node, string AttributeName) result = _parcelDocument.DereferenceNodeAttribute(attributeValue);
                return GetResolvedAttributeValue(result.Node, result.AttributeName, false);
            }
            else
                // Use raw value
                return attributeValue;
        }
        private static string FormatAttribute(string annotatedAttribute)
                => NodeAttributeNameHelper.GetNameOnly(annotatedAttribute);
        #endregion
    }
}
