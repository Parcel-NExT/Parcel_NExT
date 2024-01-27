using System.Diagnostics;

namespace Parcel.CoreEngine.Serialization
{
    public static class GenericSerializer
    {
        #region Symbols
        public const string ParcelGenericSuffix = ".psf";
        public const string ParcelBinarySuffix = ".psfb";
        public const string ParcelTextualSuffix = ".psft";
        public const string ParcelSerializationFormatTextualFormatSymbol = "PSF-T";
        public const string ParcelSerializationFormatBinaryFormatSymbol = "PSF-B";
        public const string BannerText = "PARCEL WORKFLOW ENGINE";
        #endregion

        #region Serialization
        public static void Serialize(ParcelDocument document, string outputFile)
        {
            string extension = Path.GetExtension(outputFile);
            switch (extension)
            {
                case ParcelGenericSuffix:
                case ParcelTextualSuffix:
                    TextSerializer.Serialize(document, outputFile);
                    break;
                case ParcelBinarySuffix:
                    BinarySerializer.Serialize(document, outputFile);
                    break;
                default:
                    throw new ArgumentException($"Unknown extension: {extension}");
            }
        }
        #endregion

        #region Deserialization
        public static ParcelDocument Deserialize(string inputFile)
        {
            string extension = Path.GetExtension(inputFile);
            switch (extension)
            {
                case ParcelTextualSuffix:
                    return TextSerializer.Deserialize(inputFile);
                case ParcelBinarySuffix:
                    return BinarySerializer.Deserialize(inputFile);
                case ParcelGenericSuffix:
                    string serializationFormat = DetermineSerializationFormatSymbol(inputFile);
                    if (serializationFormat == ParcelSerializationFormatTextualFormatSymbol)
                        return TextSerializer.Deserialize(inputFile);
                    else if (serializationFormat == ParcelSerializationFormatBinaryFormatSymbol)
                        return BinarySerializer.Deserialize(inputFile);
                    else
                        throw new ArgumentException($"Unknown extension: {extension}");
                default:
                    throw new ArgumentException($"Unknown extension: {extension}");
            }
        }
        #endregion

        #region Routines
        private static string DetermineSerializationFormatSymbol(string inputFile)
        {
            const int parcelSerializationFormatSymbolLength = 5;
            byte[] buffer = new byte[parcelSerializationFormatSymbolLength];
            try
            {
                using (FileStream stream = new(inputFile, FileMode.Open, FileAccess.Read))
                {
                    int bytesCount = stream.Read(buffer, 0, buffer.Length);
                    stream.Close();

                    if (bytesCount != buffer.Length)
                        throw new ArgumentException($"Corrupted file format: {inputFile}");

                    return System.Text.Encoding.UTF8.GetString(buffer);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Debug.Print(ex.Message);
                throw ex;
            }
        }
        #endregion
    }
}
