using Parcel.CoreEngine.Document;

namespace Parcel.CoreEngine.Service.CoreExtensions
{
    /// <summary>
    /// Provides methods for payload marshaling and extraction
    /// </summary>
    public static class ParcelNodePayloadAccessHelper
    {
        /// <summary>
        /// Extract numerical values from payload following a few conventions - TO BE DEFINED
        /// </summary>
        public static double ExtractNumericalValue(ParcelPayload parcelPayload)
        {
            // TODO: This is a placeholder implementation, pending more detailed implementation and standardization
            if (parcelPayload.PayloadData.ContainsKey("value"))
                return double.Parse(parcelPayload.PayloadData["value"].ToString());
            else return 0;
        }
    }
}
