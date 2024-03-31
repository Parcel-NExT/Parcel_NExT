using Parcel.CoreEngine.Interfaces;

namespace Parcel.CoreEngine.SemanticTypes
{
    /// <summary>
    /// A plain CSV-intermediate transfer format for communicating 2D data grid between different runtimes and between backend and front-end
    /// </summary>
    public sealed class DataGrid : IParcelSerializable
    {
        #region Constructor
        public DataGrid(string csv, bool hasHeader = true)
        {
            Raw = csv;
        }
        public string Raw { get; }
        #endregion

        #region Temporary Implementation
        public string[] Headers => Raw.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)[0].Split(',');
        public IEnumerable<string[]> QuickSplit(bool skipFirstRow)
        {
            var lines = Raw.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            return lines.Skip(skipFirstRow ? 1 : 0).Select(r => r.Split(',')); // TODO: Proper csv value escaping
        }
        #endregion
    }
}
