using Parcel.CoreEngine.Interfaces;

namespace Parcel.CoreEngine.SemanticTypes
{
    /// <summary>
    /// A plain CSV-intermediate transfer format for communicating 2D data grid between different runtimes and between backend and front-end
    /// </summary>
    public sealed class DataGrid : IParcelSerializable
    {
        public DataGrid(string csv)
        {
            Raw = csv;
        }

        public string Raw { get; }
    }
}
