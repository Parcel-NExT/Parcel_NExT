using Parcel.CoreEngine.Contracts;
using Parcel.CoreEngine.Document;

namespace Parcel.CoreEngine.Helpers
{
    public static class PayloadConstructionHelper
    {
        public static ParcelPayload ConstructError(ParcelNode node, ParcelNodeRuntimeExceptionBase exception)
        {
            return new ParcelPayload(node, new Dictionary<string, object>()
            {
                { "Error", $"{exception.GetType().Name}: {exception.Message}" }
            });
        }
        public static ParcelPayload ConstructContent(ParcelNode node, string content)
        {
            return new ParcelPayload(node, new Dictionary<string, object>()
            {
                { "Content", content }
            });
        }
    }
}
