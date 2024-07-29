using Parcel.CoreEngine.Helpers;
using Parcel.Infrastructure;

namespace Parcel.CoreEngine.Primitives
{
    /// <summary>
    /// A plain CSV-intermediate transfer format for communicating 2D data grid between different runtimes and between backend and front-end
    /// </summary>
    public sealed class RawDataGrid(string csv, bool hasHeader = true) : IParcelSerializable
    {
        #region Properties
        public string Raw { get; } = csv;
        public bool HasHeader { get; } = hasHeader;
        #endregion

        #region Temporary Implementation
        public string[] Headers => Raw.SplitLines(true)[0].Split(',');
        public IEnumerable<string[]> QuickSplit(bool skipFirstRow)
        {
            var lines = Raw.SplitLines(true);
            return lines.Skip(skipFirstRow ? 1 : HasHeader ? 1 : 0).Select(row => row.SplitCSVLine().ToArray());
        }
        #endregion

        #region Serialization
        public IParcelSerializable FromText(string text)
        {
            throw new NotImplementedException();
        }
        public IParcelSerializable ReadFromStream(BinaryReader reader, out long consumedBytes)
        {
            throw new NotImplementedException();
        }
        public string ToText()
        {
            throw new NotImplementedException();
        }
        public void WriteToStream(BinaryWriter writer, IParcelSerializable data)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
