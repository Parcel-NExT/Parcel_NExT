using WebSocketSharp;
using Parcel.CoreEngine.Service.Document;
using Parcel.CoreEngine.Conversion;
using Parcel.CoreEngine.Helpers;
using System.Reflection;

namespace Tranquility.Sessions
{
    /// <summary>
    /// A document-oriented session mostly for listeing only and provides document-in-sync editing from the front-end.
    /// This endpoint is silent - for the most part it doesn't send any replies.
    /// </summary>
    public class StatedSession : BaseSession
    {
        #region States
        private Dictionary<string, ServiceEndpoint>? _AvailableEndPoints;
        private DocumentProvisionService? DocumentService;
        #endregion

        #region Framework Functions
        protected override void OnOpen()
        {
            base.OnOpen();

            LogInfo("New connection.");

            DocumentService = new DocumentProvisionService();

            _AvailableEndPoints = [];
            Dictionary<string, MethodInfo> availableServices = DocumentService.GetAvailableServices();
            foreach (KeyValuePair<string, MethodInfo> service in availableServices)
                _AvailableEndPoints.TryAdd($"{service.Key}", new(DocumentService, service.Value));
        }
        protected override void OnClose(CloseEventArgs e)
        {
            base.OnClose(e);
            LogInfo("Session ended.");
        }
        protected override void OnMessage(MessageEventArgs e)
        {
            string message = e.Data;
            LogInfo(message);
            HandleMessage(message);
        }
        #endregion

        #region Routines
        private void HandleMessage(string message)
        {
            // TODO: Handle ParcelNodeRuntimeException
            string[] arguments = message.SplitCommandLineArguments();
            string methodName = arguments.First();
            if (_AvailableEndPoints!.TryGetValue(methodName, out ServiceEndpoint? endPoint))
            {
                var provider = endPoint.Provider;
                var methodInfo = endPoint.Method;

                object? result;
                if (methodInfo.GetParameters().Length == 0)
                    result = methodInfo.Invoke(provider, null);
                else
                {
                    object[]? functionInputs = MarshalStringArgumentsToMethodInputs(methodInfo.GetParameters(), arguments.Skip(1).ToArray());
                    result = methodInfo.Invoke(provider, functionInputs); // TODO: Potentially need type conversion to deal with non-string arguments
                }

                if (result != null)
                    SendMultiPartReply(StringTypeConverter.SerializeResult(result));
            }
        }
        #endregion
    }
}
